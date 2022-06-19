using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class FacetImplementationTests
    {
        private readonly Dictionary<string, string> guaranteedStructures = new()
        {
            { "AttributeFacet","ValueConstraint AttributeName,ValueConstraint AttributeValue,String Uri,String Instructions" },
            { "FacetBase","String Uri,String Instructions" },
            { "IfcClassificationFacet","ValueConstraint ClassificationSystem,ValueConstraint Identification,Boolean IncludeSubClasses,String Uri,String Instructions" },
            { "IfcPropertyFacet","ValueConstraint PropertySetName,ValueConstraint PropertyName,String Measure,ValueConstraint PropertyValue,String Uri,String Instructions" },
            { "IfcTypeFacet","ValueConstraint IfcType,ValueConstraint PredefinedType,Boolean IncludeSubtypes,String Uri,String Instructions" },
            { "MaterialFacet","ValueConstraint Value,String Uri,String Instructions" },
            { "DocumentFacet","ValueConstraint DocId,ValueConstraint DocName,ValueConstraint DocLocation,ValueConstraint DocPurpose,ValueConstraint DocIntendedUse,String Uri,String Instructions" },
            { "IfcRelationFacet","String SourceId,FacetGroup Source,String Relation,String Uri,String Instructions" },
            { "ExactConstraint","String Value" },
            { "PatternConstraint","String Pattern,Boolean IsValidPattern,String PatternError" },
            { "RangeConstraint","String MinValue,Boolean MinInclusive,String MaxValue,Boolean MaxInclusive" },
            { "StructureConstraint","Int32? TotalDigits,Int32? FractionDigits,Int32? Length,Int32? MinLength,Int32? MaxLength" },
            { "ValueConstraint","List<Xbim.InformationSpecifications.IValueConstraint> AcceptedValues,TypeName BaseType" },
            { "PartOfFacet","String Entity,ValueConstraint EntityName,String Uri,String Instructions" },
            { "DimensionalExponents","Int32 Length,Int32 Mass,Int32 Time,Int32 ElectricCurrent,Int32 Temperature,Int32 AmountOfSubstance,Int32 LuminousIntensity" },
			// for rich ways of automating multiple configurations, see Memberdata usage in (e.g.) DocumentFacetTests
		};

        /// when adding properties to an iequatable we need to make sure that they
        /// are considered in the equals implementation, this is a test that reminds us of that.
        [Fact]
        public void EnsureIEquatableIsOk()
        {
            var allEquatables = typeof(Xids).Assembly.GetTypes().Where(t => t.GetInterfaces().Any(interf => interf.Name.Contains("IEquatable")));
            foreach (var oneEqatable in allEquatables)
            {
                var foundInDictionary = guaranteedStructures.TryGetValue(oneEqatable.Name, out var expected);
                foundInDictionary.Should().BeTrue($"'{oneEqatable.Name}' should be a guaranteed equatable");
                if (expected == "<skip>")
                    continue;

                var verifiedAttributesList = string.Join(",", oneEqatable.GetProperties().Select(x => SmartName(x)).ToArray());
                verifiedAttributesList.Should().Be(expected, $"{oneEqatable.Name} must not have different properties to the ones guaranteed equatable");
            }
        }

        private readonly static Regex rNullable = new("\\[\\[([^,]*),");

        static private string SmartName(PropertyInfo x)
        {
            var t = x.PropertyType.FullName.Replace("System.", "");
            if (t.StartsWith("Nullable"))
            {
                var nM = rNullable.Match(t);
                if (nM.Success)
                {
                    return nM.Groups[1].Value + "? " + x.Name;
                }
            }
            if (t.Contains("Collections.Generic.List"))
            {
                var nM = rNullable.Match(t);
                if (nM.Success)
                {
                    return "List<" + nM.Groups[1].Value + "> " + x.Name;
                }
            }
            if (x.PropertyType.Name.Contains('`'))
            {

            }
            return x.PropertyType.Name + " " + x.Name;
        }

        [Fact]
        public void PartOfEqualImplementation()
        {
            TestAddRemove(new PartOfFacet());
            TestAddRemove(new PartOfFacet()
            {
                Entity = "IfcGroup"
            });

            var p1 = new PartOfFacet();
            var p2 = new PartOfFacet() { Entity = "none" };
            p1.Should().NotBeEquivalentTo(p2);

            var p3 = new PartOfFacet() { Entity = "" };
            p3.Should().BeEquivalentTo(p1);

        }


        [Fact]
        public void FacetEqualImplementation()
        {
            TestAddRemove(new IfcClassificationFacet());
            TestAddRemove(new IfcClassificationFacet()
            {
                ClassificationSystem = new ValueConstraint("2"),
                Identification = new ValueConstraint(12)
            });


            TestAddRemove(new IfcPropertyFacet());
            TestAddRemove(new IfcPropertyFacet()
            {
                PropertyName = "2",
                PropertySetName = "3",
            });

            TestAddRemove(new IfcTypeFacet());
            TestAddRemove(new IfcTypeFacet()
            {
                IfcType = "1",
                IncludeSubtypes = false,
                PredefinedType = "3"
            });

            TestAddRemove(new MaterialFacet());

            TestAddRemove(new IfcRelationFacet());
            TestAddRemove(new IfcRelationFacet()
            {
#pragma warning disable CS0618 // Type or member is obsolete
                Source = new FacetGroup(),
#pragma warning restore CS0618 // Type or member is obsolete
                Relation = IfcRelationFacet.RelationType.ContainedElements.ToString()
            });

            TestAddRemove(new DocumentFacet());
            TestAddRemove(new DocumentFacet()
            {
                DocId = "1",
                DocIntendedUse = "2",
                DocLocation = "3",
                DocName = "4",
                DocPurpose = "5"
            });

        }

        [Fact]
        public void ValueEqualImplementationTest()
        {

            var pc = new PatternConstraint();
            TestAddRemove(pc);

            // List<ValueConstraint> vals = new();
            var val = new ValueConstraint();
            TestAddRemove(val);
            val = MakeEnumVal();
            TestAddRemove(val);

            // List<RangeConstraint> rcl = new();
            var rc = new RangeConstraint();
            TestAddRemove(rc);

            // List<StructureConstraint> scl = new();
            var sc = new StructureConstraint();
            _ = sc.GetHashCode();
            TestAddRemove(sc, false);

            var val1 = MakeEnumVal();
            var val2 = MakeEnumVal();

            val1.Should().Be(val2);

        }


        [Fact]
        public void ValueIsEmpty()
        {
            new ValueConstraint().IsEmpty().Should().BeTrue();
            new ValueConstraint() { BaseType = NetTypeName.String }.IsEmpty().Should().BeFalse();
            new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() }.IsEmpty().Should().BeTrue();
            new ValueConstraint() { BaseType = NetTypeName.Boolean, AcceptedValues = new List<IValueConstraint>() }.IsEmpty().Should().BeFalse();
            new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() { new ExactConstraint("") } }.IsEmpty().Should().BeFalse();
        }


        [Fact]
        public void DataTypesOk()
        {
            var typeNames = Enum.GetValues(typeof(NetTypeName)).Cast<NetTypeName>();
            foreach (var tName in typeNames)
            {
                if (tName == NetTypeName.Undefined)
                    continue;

                var t = ValueConstraint.GetXsdTypeString(tName);
                var back = ValueConstraint.GetNamedTypeFromXsd(t);
                back.Should().Be(tName);


                var newT = ValueConstraint.GetNetType(tName);
                var defval = ValueConstraint.GetDefault(tName, null);
                defval.Should().NotBeNull($"should be possible to have default type: {tName}, {newT}");
                newT.Should().NotBeNull($"should be possible to create type: {tName}, {newT}");
                defval.GetType().Should().Be(newT);
            }
        }

        static private ValueConstraint MakeEnumVal()
        {
            var val = new ValueConstraint
            {
                BaseType = NetTypeName.String,
                AcceptedValues = new List<IValueConstraint>()
                {
                    new ExactConstraint("30"),
                    new ExactConstraint("60")
                }
            };
            return val;
        }

        // this one tests that the implementation of equals is correct on the object passed.
        internal static void TestAddRemove<T>(T c, bool testForRandom = true)
        {
            var lst = new List<T>();
            var s = c.ToString();
            _ = c.GetHashCode(); // this must not crash

            if (c is IFacet f)
            {
                var shortV = f.Short();
                shortV.Should().NotBeNull();
            }
            if (c is IValueConstraint vc)
            {
                var shortV = vc.Short();
                shortV.Should().NotBeNull();
                if (testForRandom)
                {
                    var any = vc.IsSatisfiedBy("random", null, false);
                    any.Should().BeFalse();
                }
            }

            s.Should().NotBe("");
            lst.Add(c);
            lst.Remove(c);
            lst.Count.Should().Be(0);
        }
    }
}
