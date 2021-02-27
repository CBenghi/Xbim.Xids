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
		}
	}
}
