using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Xbim.IDS.Tests
{
	[TestClass]
	public class BuildingSmartIdsTests
	{
		[DeploymentItem(@"Files\bS", @"Files\bS")]
		[TestMethod]
		public void CanLoadBuildingSmartIdsFormat()
		{
			var s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01.xml");
			// Ids.ToBuildingSmartIDS("out.xml");
			

			s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01Mod.xml");
			// Ids.ToBuildingSmartIDS("out.xml");

			s = Ids.FromBuildingSmartIDS(@"Files\bS\Example01Mod2.xml");
			// Ids.ToBuildingSmartIDS("out.xml");

			var jFile = @"..\..\out.json";
			s.SaveAsJson(jFile);
			var unp = Ids.LoadFromJson(jFile);
		}
	}
}
