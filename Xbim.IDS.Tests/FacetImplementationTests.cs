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
			Test(lst, new IfcClassificationFacet());
			Test(lst, new IfcClassificationFacet()
			{
				ClassificationSystem = "1",
				Location = "2",
				Node = "3",
				Uri = new Uri("http://www.gino.com")
			});


			Test(lst, new IfcPropertyFacet());
			Test(lst, new IfcPropertyFacet()
			{
				Location = "1",
				PropertyName = "2",
				PropertySetName = "3",
				Uri = new Uri("http://www.gino.com")
			});

			Test(lst, new IfcTypeFacet());
			Test(lst, new IfcTypeFacet()
			{
				IfcType = "1",
				IncludeSubtypes = false,
				PredefinedType = "3"
			});

			Test(lst, new MaterialFacet());
			Test(lst, new MaterialFacet()
			{
				Location = "1",
				Uri = new Uri("http://www.gino.com")
			});
		}

		private static void Test(List<IFacet> lst, IFacet c)
		{
			var s = c.ToString();
			Assert.AreNotEqual("", s);
			lst.Add(c); lst.Remove(c);
			Assert.AreEqual(0, lst.Count);
		}
	}
}
