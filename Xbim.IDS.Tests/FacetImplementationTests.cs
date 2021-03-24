using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Xids;

namespace Xbim.IDS.Tests
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
				ClassificationSystem = "1",
				Location = "2",
				Node = new Value(12),
				Uri = new Uri("http://www.gino.com")
			});


			TestAddRemove(lst, new IfcPropertyFacet());
			TestAddRemove(lst, new IfcPropertyFacet()
			{
				Location = "1",
				PropertyName = "2",
				PropertySetName = "3",
				Uri = new Uri("http://www.gino.com")
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
				Uri = new Uri("http://www.gino.com")
			});
		}

		[TestMethod]
		public void ValueEqualImplementationTest()
		{
			List<PatternConstraint> pcl = new List<PatternConstraint>();
			var pc = new PatternConstraint();
			TestAddRemove(pcl, pc);

			List<Value> vals = new List<Value>();
			var val = new Value();
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

		private Value MakeEnumVal()
		{
			var val = new Value();
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
