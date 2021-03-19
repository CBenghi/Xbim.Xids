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
			var s = IDS.FromBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ModelSetRepository.Count);
		}

		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesExpectation()
		{
			var s = IDS.FromBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			Assert.AreEqual(2, s.ExpectationsRepository.Count);
		}

		[DeploymentItem(@"Files\bS", @"Files\bS")]
		[TestMethod]
		public void CanLoadBuildingSmartIdsFormats()
		{
			var s = IDS.FromBuildingSmartIDS(@"Files\bS\Example01.xml");
			AssertOk(s);
			
			s = IDS.FromBuildingSmartIDS(@"Files\bS\Example02.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");


			s = IDS.FromBuildingSmartIDS(@"Files\bS\Example01Mod.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			s = IDS.FromBuildingSmartIDS(@"Files\bS\Example01Mod2.xml");
			Assert.IsNotNull(s);
			// Ids.ToBuildingSmartIDS("out.xml");

			var jFile = @"..\..\out.json";
			var jFile2 = @"..\..\out2.json";
			s.SaveAsJson(jFile);
			var unp = IDS.LoadFromJson(jFile);
			Assert.IsNotNull(unp);
			unp.SaveAsJson(jFile2);
		}

		private void AssertOk(IDS s)
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
