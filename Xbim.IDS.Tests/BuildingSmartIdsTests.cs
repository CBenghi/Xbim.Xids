using IdsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static IdsLib.CheckOptions;

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
			Assert.AreEqual(4, s.FacetRepository.Count);
		}

		[DeploymentItem(@"Files\bS\Example01Mod2.xml")]
		[TestMethod]
		public void ReusesRequirements()
		{
			var s = Xids.ImportBuildingSmartIDS(@"Example01Mod2.xml");
			Assert.IsNotNull(s);
			s.SaveAsJson(@"..\..\reuse.json");
			Assert.AreEqual(4, s.FacetRepository.Count);
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
			var jFile3 = @"..\..\out3.json";
			s.SaveAsJson(jFile);
			var unp = Xids.LoadFromJson(jFile);
			Assert.IsNotNull(unp);
			unp.SaveAsJson(jFile2);

			// try to read json via stream
			//
			var fromStream = Xids.LoadFromJson(File.OpenRead(jFile2));
			Assert.IsNotNull(fromStream);
			fromStream.SaveAsJson(jFile3);

			var originalHash = GetFileHash(jFile);
			var copiedHash = GetFileHash(jFile2);
			var streamHash = GetFileHash(jFile3);

			Assert.AreEqual(copiedHash, originalHash);
			Assert.AreEqual(copiedHash, streamHash);
		}

		[TestMethod]
		[DeploymentItem(@"Files\bS\fromLeon\IDS-full.xml", "fullLoad")]
		public void FullLoadBuildingSmartIdsFormats()
		{
			var s = Xids.ImportBuildingSmartIDS(@"fullLoad\IDS-full.xml");
			AssertOk(s);
			var reqs = s.AllSpecifications().ToList();
		}

		[TestMethod]
		[DeploymentItem(@"Files\bS\fromLeon\IDS-full.xml", "fullSave")]
		[DeploymentItem(@"Schema\ids.xsd", "fullSave")]
		public void FullSaveBuildingSmartIdsFormats()
		{
			var fileIn = @"fullSave\IDS-full.xml";
			// if the test fails here, because th input file was changed update the expected hash
			var readHash = GetFileHash(fileIn);
			Assert.AreEqual(readHash, "4ce9188fddd95e38caa76acaf065f9d7c5b252");

			var s = Xids.ImportBuildingSmartIDS(fileIn);
			AssertOk(s);
			var fileOut = @"..\..\saveattempt.xml";
			s.ExportBuildingSmartIDS(fileOut);

			CheckIDSSchema(fileOut, @"fullSave\IDS-full.xml");

			// if the test fails here, visually check that the data is correct and then
			// update the expected hash

			// files in debug have newlines and indents
#if DEBUG
			Assert.AreEqual("7cd8b3ad0aa38cad4ffbb304781511241824c4", GetFileHash(fileOut));
#else
			Assert.AreEqual("b5aa6b8054b7f2367aff1d7c85d9f3c29573f1", GetFileHash(fileOut));
#endif
		}

		private void CheckIDSSchema(string fileOut, string schema)
		{
			CheckOptions c = new CheckOptions();
			c.CheckSchema = new List<string> { schema };
			c.InputSource = fileOut;
			c.CheckSchemaDefinition = false;

			// to adjust once we fix the xml file in the other repo.
			var ret = CheckOptions.Run(c);
			Assert.AreEqual(Status.Ok, ret);
		}

		[TestMethod]
		public void WeirdValues()
		{
			var stringV = "someValue";
			var val = Value.SingleUndefinedExact(stringV);
			var itIs = val.IsSingleUndefinedExact(out var retVal);
			Assert.IsTrue(itIs);
			Assert.AreEqual(stringV, retVal);
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
			foreach (var req in s.AllSpecifications())
			{
				Assert.IsNotNull(req.Requirement);
				Assert.IsNotNull(req.Applicability);
			}
		}
	}
}
