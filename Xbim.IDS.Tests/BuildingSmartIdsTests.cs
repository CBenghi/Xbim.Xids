using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class BuildingSmartIdsTests
	{
		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesAapplicability()
		{
			var s = Ids.FromBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ModelSetRepository.Count);
		}

		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesExpectation()
		{
			var s = Ids.FromBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ExpectationsRepository.Count);
		}

		[DeploymentItem(@"Files\bS", @"Files\bS")]
		[TestMethod]
		public void CanLoadBuildingSmartIdsFormat()
		{
			var s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");
			

			s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01Mod.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01Mod2.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			var jFile = @"..\..\out.json";
			var jFile2 = @"..\..\out2.json";
			s.SaveAsJson(jFile);
			var unp = Ids.LoadFromJson(jFile);
			Assert.IsNotNull(unp);
			unp.SaveAsJson(jFile2);
		}
	}
}
