using IdsLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using static IdsLib.CheckOptions;
using Xbim.InformationSpecifications;

namespace Xbim.InformationSpecifications.Tests
{
	[TestClass]
	public class BuildingSmartIdsTests
	{
		[DeploymentItem(@"Files\bSv3", @"Files\bS")]
		[TestMethod]
		public void CanLoadBuildingSmartIdsFormats()
		{
			DirectoryInfo d = new DirectoryInfo(@"Files\bS");
			foreach (var file in d.GetFiles("*.xml"))
			{
				var s = Xids.ImportBuildingSmartIDS(file.FullName);
				AssertOk(s);
			}

			//var jFile = @"..\..\out.json";
			//var jFile2 = @"..\..\out2.json";
			//var jFile3 = @"..\..\out3.json";
			//s.SaveAsJson(jFile);
			//var unp = Xids.LoadFromJson(jFile);
			//Assert.IsNotNull(unp);
			//unp.SaveAsJson(jFile2);

			//// try to read json via stream
			////
			//var fromStream = Xids.LoadFromJson(File.OpenRead(jFile2));
			//Assert.IsNotNull(fromStream);
			//fromStream.SaveAsJson(jFile3);

			//var originalHash = GetFileHash(jFile);
			//var copiedHash = GetFileHash(jFile2);
			//var streamHash = GetFileHash(jFile3);

			//Assert.AreEqual(copiedHash, originalHash);
			//Assert.AreEqual(copiedHash, streamHash);
		}

		[TestMethod]
		[DeploymentItem(@"Files\bSv3\IDS_example-with-restrictions.xml", "fullSave")]
		[DeploymentItem(@"Files\bSv3\id.xsd", "fullSave")]

		public void FullSaveBuildingSmartIdsFormats()
		{
			var fileIn = @"fullSave\IDS_example-with-restrictions.xml";
			// if the test fails here, because the input file was changed update the expected hash
			var readHash = GetFileHash(fileIn);
			Assert.AreEqual("819389d3bbd72c25f1ef1257c428d6c01cb477", readHash);

			var s = Xids.ImportBuildingSmartIDS(fileIn);
			AssertOk(s);
			var fileOut = @"..\..\saveattempt.xml";
			s.ExportBuildingSmartIDS(fileOut);

			CheckIDSSchema(fileOut, @"fullSave\ids.xsd");

			// if the test fails here, visually check that the data is correct and then
			// update the expected hash

			// files in debug have newlines and indents
#if DEBUG
			Assert.AreEqual("f65384bf7733bc8647e1db9de4331ed9c7bfe6", GetFileHash(fileOut));
#else
			Assert.AreEqual("3b51da0925d75c7992d6bf8787bdd6ef5241b", GetFileHash(fileOut));
#endif
		}

		private void CheckIDSSchema(string fileOut, string schema)
		{
			CheckOptions c = new CheckOptions();
			c.CheckSchema = new List<string> { @"C:\Data\Dev\BuildingSmart\IDS\Development\Third production release\ids.xsd" };
			c.InputSource = fileOut;

			// to adjust once we fix the xml file in the other repo.
			var w = new StringWriter();
			var ret = CheckOptions.Run(c, w);
#if DEBUG
			if (ret != Status.Ok)
			{
				string s = w.ToString();
				Clipboard.SetText(s);
			}
#endif
			Assert.AreEqual(Status.Ok, ret);
		}

		[TestMethod]
		public void WeirdValues()
		{
			var stringV = "someValue";
			var val = ValueConstraint.SingleUndefinedExact(stringV);
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
