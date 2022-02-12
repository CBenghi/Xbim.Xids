using System;
using Xunit;
using Xbim.InformationSpecifications;
using System.IO;
using System.Diagnostics;
using FluentAssertions;
using System.Linq;
using Xbim.InformationSpecifications.NewTests.Helpers;

namespace Xbim.InformationSpecifications.NewTests
{
	public class JsonTests
	{
		[Fact]
		public void CanWriteSimpleFormat()
        {
            var d = new DirectoryInfo(".");
            Debug.WriteLine(d.FullName);

            Xids x = XidsTestHelpers.GetSimpleXids();
            var filename = @"temp.json";
            x.SaveAsJson(filename);
            Assert.True(File.Exists(filename));

            var readBack = Xids.LoadFromJson(filename);
            Assert.NotNull(readBack);
            x.SaveAsJson(@"temp2.json");

            Xids x2 = null;
            using (var s = File.OpenRead(@"temp2.json"))
            {
                x2 = Xids.LoadFromJsonAsync(s).GetAwaiter().GetResult();
            }
            Assert.NotNull(x2);
        }

        

        [Fact]
		public void CanSerializeRicherFormat()
		{
			DirectoryInfo d = new DirectoryInfo(@"Files");
			foreach (var file in d.GetFiles("*.xml"))
			{
				var s = Xids.ImportBuildingSmartIDS(file.FullName);
				var fn = Path.ChangeExtension(file.FullName, ".json");
				s.SaveAsJson(fn);
				var reloaded = Xids.LoadFromJson(fn);
				reloaded.AllSpecifications().Count().Should().Be(s.AllSpecifications().Count());

				var fn2 = Path.ChangeExtension(file.FullName, ".2.json");
				reloaded.SaveAsJson(fn2);
			}
		}

		[Fact]
		public void CanSerializeExtraFacets()
		{
			var x = new Xids();
			
			// relation: furniture contained in spaces
			//
			var spec = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);

			var spaces = new FacetGroup(x.FacetRepository);
			spaces.Facets.Add(new IfcTypeFacet() { IfcType = "IfcSpace" });

			var relFacet = new IfcRelationFacet()
			{
				Source = spaces,
				Relation = IfcRelationFacet.RelationType.ContainedElements.ToString()
			};

			spec.Applicability.Facets.Add(relFacet);
			spec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcFurnishingElement" });
			spec.Requirement.Facets.Add(new IfcPropertyFacet() { PropertySetName = "pset", PropertyName = "prop" });

			var fname = "CanSerializeExtraFacets.json";
			var fname2 = "CanSerializeExtraFacets2.json";
			x.SaveAsJson(fname);
			var reloaded = Xids.LoadFromJson(fname);
			reloaded.AllSpecifications().Count().Should().Be(x.AllSpecifications().Count());

			reloaded.SaveAsJson(fname2);

			var h1 = FileHashing.GetFileHash(fname);
			var h2 = FileHashing.GetFileHash(fname2);
			if (h1 != h2)
            {
				Debug.Write(@"""C:\Program Files (x86)\WinMerge\WinMergeU.exe"" ");
				Debug.Write($"\"{new FileInfo(fname).FullName}\" "); 
				Debug.Write($"\"{new FileInfo(fname2).FullName}\" ");
				Debug.WriteLine("");

			}
			h1.Should().Be(h2);

			relFacet.UsedGroups().Count().Should().Be(1);

			spaces.UseCount(x).Should().Be(1);
		}

		[Theory]
		[InlineData("Files/oldformat.json")]
		[InlineData("Files/newformat.json")]
		public void CanReadOldFile(string fileName)
        {
			var t = Xids.LoadFromJson(fileName);
        }

		[Fact]
		public void CanWriteSimpleFile()
        {
			var tmpFile = Path.GetTempFileName();
			Xids x = XidsTestHelpers.GetSimpleXids();
			x.SaveAsJson(tmpFile);
		}
	}
}