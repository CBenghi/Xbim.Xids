using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
		public void ReusesRequirements()
		{
			var s = Xids.ImportBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			s.SaveAsJson(@"..\..\reuse.json");
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

			var originalHash = GetFileHash(jFile);
			var copiedHash = GetFileHash(jFile2);

			Assert.AreEqual(copiedHash, originalHash);
		}

		[TestMethod]
		[DeploymentItem(@"Files\bS\fromLeon\IDS-full.xml", "fullLoad")]
		public void FullLoadBuildingSmartIdsFormats()
		{
			var s = Xids.ImportBuildingSmartIDS(@"fullLoad\IDS-full.xml");
			AssertOk(s);
			var reqs = s.AllRequirements().ToList();
			
		}

		public string GetFileHash(string filename)
		{
			var hash = new SHA1Managed();
			var clearBytes = File.ReadAllBytes(filename);
			var hashedBytes = hash.ComputeHash(clearBytes);
			return ConvertBytesToHex(hashedBytes);
		}

		public string ConvertBytesToHex(byte[] bytes)
		{
			var sb = new StringBuilder();

			for (var i = 0; i < bytes.Length; i++)
			{
				sb.Append(bytes[i].ToString("x"));
			}
			return sb.ToString();
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
