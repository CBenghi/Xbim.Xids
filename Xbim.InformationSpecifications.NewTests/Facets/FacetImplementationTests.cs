using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
	public class FacetImplementationTests
	{

		[Fact]
		public void FacetEqualImplementation()
		{
			
			TestAddRemove(new IfcClassificationFacet());
			TestAddRemove(new IfcClassificationFacet()
			{
				ClassificationSystem = new ValueConstraint("2"),
				Location = "2",
				Identification = new ValueConstraint(12)
			});


			TestAddRemove(new IfcPropertyFacet());
			TestAddRemove(new IfcPropertyFacet()
			{
				Location = "1",
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
			TestAddRemove(new MaterialFacet()
			{
				Location = "1",
			});


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

			List<ValueConstraint> vals = new List<ValueConstraint>();
			var val = new ValueConstraint();
			TestAddRemove(val);
			val = MakeEnumVal();
			TestAddRemove(val);

			List<RangeConstraint> rcl = new List<RangeConstraint>();
			var rc = new RangeConstraint();
			TestAddRemove(rc);

			List<StructureConstraint> scl = new List<StructureConstraint>();
			var sc = new StructureConstraint();
			var t = sc.GetHashCode();
			TestAddRemove(sc, false);

			var val1 = MakeEnumVal();
			var val2 = MakeEnumVal();

			val1.Should().Be(val2);

		}


		[Fact]
		public void ValueIsEmpty()
		{
			new ValueConstraint().IsEmpty().Should().BeTrue();
			new ValueConstraint() { BaseType = TypeName.String }.IsEmpty().Should().BeFalse();
			new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() }.IsEmpty().Should().BeTrue();
			new ValueConstraint() { BaseType = TypeName.Boolean, AcceptedValues = new List<IValueConstraint>() }.IsEmpty().Should().BeFalse();
			new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() { new ExactConstraint("") } }.IsEmpty().Should().BeFalse();
		}


		[Fact]
		public void DataTypesOk()
		{
			var typeNames = Enum.GetValues(typeof(TypeName)).Cast<TypeName>();
			foreach (var tName in typeNames)
			{
				if (tName == TypeName.Undefined)
					continue;

				var t = ValueConstraint.GetXsdTypeString(tName);
				var back = ValueConstraint.GetNamedTypeFromXsd(t);
				back.Should().Be(tName);
				
			
				var newT = ValueConstraint.GetNetType(tName);
				var defval = ValueConstraint.GetDefault(tName);
				defval.Should().NotBeNull($"should be possible to have default type: {tName}, {newT}");
				newT.Should().NotBeNull($"should be possible to create type: {tName}, {newT}");
				defval.GetType().Should().Be(newT);
			}
		}

		private ValueConstraint MakeEnumVal()
		{
			var val = new ValueConstraint();
			val.BaseType = TypeName.String;
			val.AcceptedValues = new List<IValueConstraint>();
			val.AcceptedValues.Add(new ExactConstraint("30"));
			val.AcceptedValues.Add(new ExactConstraint("60"));
			return val;
		}

		// this one tests that the implementation of equals is correct on the object passed.
		private static void TestAddRemove<T>(T c, bool testForRandom = true)
		{
			var lst = new List<T>();
			var s = c.ToString();
			var t = c.GetHashCode(); // this must not crash
			
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
					var any = vc.IsSatisfiedBy("random", null);
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
