using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.InformationSpecifications;

namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class FacetImplementationTests
	{

		[TestMethod]
		public void FacetEqualImplementation()
		{
			List<IFacet> lst = new List<IFacet>();
			TestAddRemove(lst, new IfcClassificationFacet());
			TestAddRemove(lst, new IfcClassificationFacet()
			{
				ClassificationSystem = new ValueConstraint("2"),
				Location = "2",
				Identification = new ValueConstraint(12)
			});


			TestAddRemove(lst, new IfcPropertyFacet());
			TestAddRemove(lst, new IfcPropertyFacet()
			{
				Location = "1",
				PropertyName = "2",
				PropertySetName = "3",
			});

			TestAddRemove(lst, new IfcTypeFacet());
			TestAddRemove(lst, new IfcTypeFacet()
			{
				IfcType = "1",
				IncludeSubtypes = false,
				PredefinedType = "3"
			});

			TestAddRemove(lst, new MaterialFacet());
			TestAddRemove(lst, new MaterialFacet()
			{
				Location = "1",
			});


			TestAddRemove(lst, new IfcRelationFacet());
			TestAddRemove(lst, new IfcRelationFacet()
			{
				Source = new FacetGroup(),
				Relation = IfcRelationFacet.RelationType.ContainedElements.ToString()
			});

		}

		[TestMethod]
		public void ValueEqualImplementationTest()
		{
			List<PatternConstraint> pcl = new List<PatternConstraint>();
			var pc = new PatternConstraint();
			TestAddRemove(pcl, pc);

			List<ValueConstraint> vals = new List<ValueConstraint>();
			var val = new ValueConstraint();
			TestAddRemove(vals, val);
			val = MakeEnumVal();
			TestAddRemove(vals, val);

			List<RangeConstraint> rcl = new List<RangeConstraint>();
			var rc = new RangeConstraint();
			TestAddRemove(rcl, rc);

			List<StructureConstraint> scl = new List<StructureConstraint>();
			var sc = new StructureConstraint();
			var t = sc.GetHashCode();
			TestAddRemove(scl, sc);

			var val1 = MakeEnumVal();
			var val2 = MakeEnumVal();
			Assert.AreEqual(val1, val2);

		}


		[TestMethod]
		public void ValueIsEmpty()
		{		
			Assert.IsTrue(new ValueConstraint().IsEmpty());
			Assert.IsFalse(new ValueConstraint() { BaseType = TypeName.String }.IsEmpty());

			Assert.IsTrue(new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() }.IsEmpty());
			Assert.IsFalse(new ValueConstraint() { BaseType = TypeName.Boolean, AcceptedValues = new List<IValueConstraint>() }.IsEmpty());
			Assert.IsFalse(new ValueConstraint() { AcceptedValues = new List<IValueConstraint>() { new ExactConstraint("") } }.IsEmpty());

		}


		[TestMethod]
		public void DataTypesOk()
		{
			var typeNames = Enum.GetValues(typeof(TypeName)).Cast<TypeName>();
			foreach (var tName in typeNames)
			{
				if (tName == TypeName.Undefined)
					continue;

				var t = ValueConstraint.GetXsdTypeString(tName);
				var back = ValueConstraint.GetNamedTypeFromXsd(t);
				Assert.AreEqual(tName, back);

				
				var newT = ValueConstraint.GetNetType(tName);
				var defval = ValueConstraint.GetDefault(tName);

				Assert.IsNotNull(defval, $"Cannot create type: {tName}, {newT}");
				Assert.IsNotNull(newT, $"Empty return type: {tName}, {newT}");
				Assert.AreEqual(newT, defval.GetType());
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

		private static void TestAddRemove<T>(List<T> lst, T c)
		{
			var s = c.ToString();
			Assert.AreNotEqual("", s);
			lst.Add(c); lst.Remove(c);
			Assert.AreEqual(0, lst.Count);
		}
	}
}
