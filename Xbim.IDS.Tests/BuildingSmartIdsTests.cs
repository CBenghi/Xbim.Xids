using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace Xbim.Xids.Tests
{
	[TestClass]
	public class BuildingSmartIdsTests
	{
		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesAapplicability()
		{
			XElement e = XElement.Load(@"Example01Mod2.xml");
			var s = Xids.ImportBuildingSmartIDS(e);
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ModelSetRepository.Count);
		}

		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesExpectation()
		{
			var s = Xids.ImportBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ExpectationsRepository.Count);
		}

		[DeploymentItem(@"Files\bS", @"Files\bS")]
		[TestMethod]
		public void CanLoadBuildingSmartIdsFormats()
		{
			var s = Xids.ImportBuildingSmartIDS(@"Files\bS\Example01.xml");
			AssertOk(s);
			
			s = Xids.ImportBuildingSmartIDS(@"Files\bS\Example02.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");


			s = Xids.ImportBuildingSmartIDS(@"Files\bS\Example01Mod.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			s = Xids.ImportBuildingSmartIDS(@"Files\bS\Example01Mod2.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			var jFile = @"..\..\out.json";
			var jFile2 = @"..\..\out2.json";
			s.SaveAsJson(jFile);
			var unp = Xids.LoadFromJson(jFile);
			Assert.IsNotNull(unp);
			unp.SaveAsJson(jFile2);
		}

		private void AssertOk(Xids s)
		{
			Assert.IsNotNull(s);
			foreach (var req in s.AllRequirements())
			{
				Assert.IsNotNull(req.Need);
				Assert.IsNotNull(req.ModelSubset);
			}
		}
	}
}
