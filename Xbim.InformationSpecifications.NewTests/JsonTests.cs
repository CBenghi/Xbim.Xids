using FluentAssertions;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xbim.InformationSpecifications.Cardinality;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;

namespace Xbim.InformationSpecifications.Tests
{
    public class JsonTests
    {
        [Fact]
        public async Task CanWriteSimpleFormat()
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

            Xids? x2 = null;
            using (var s = File.OpenRead(@"temp2.json"))
            {
                x2 = await Xids.LoadFromJsonAsync(s);
            }
            Assert.NotNull(x2);
        }



        [Fact]
        public void CanSerializeRicherFormat()
        {
            DirectoryInfo d = new(@"Files");
            foreach (var file in d.GetFiles("*.xml"))
            {
                var s = Xids.LoadBuildingSmartIDS(file.FullName);
                var fn = Path.ChangeExtension(file.FullName, ".json");
                s!.SaveAsJson(fn);
                var reloaded = Xids.LoadFromJson(fn);
                reloaded!.AllSpecifications().Count().Should().Be(s.AllSpecifications().Count());

                var fn2 = Path.ChangeExtension(file.FullName, ".2.json");
                reloaded.SaveAsJson(fn2);
            }
        }

        [Fact]
        public void CanSerializeCardinality()
        {
            var tmpFile = Path.GetTempFileName();
            Xids x = XidsTestHelpers.GetSimpleXids();
            var OneSpec = x.AllSpecifications().First();

            // testing simple cardinality
            OneSpec.Cardinality = new SimpleCardinality() { ApplicabilityCardinality = CardinalityEnum.Prohibited };
            x.SaveAsJson(tmpFile);
            var reload = Xids.LoadFromJson(tmpFile)!;
            var simple = reload.AllSpecifications().First().Cardinality as SimpleCardinality;
            simple!.ApplicabilityCardinality.Should().Be(CardinalityEnum.Prohibited);

            // testing minmax
            OneSpec.Cardinality = new MinMaxCardinality() { MinOccurs = 4, MaxOccurs = 5 };
            x.SaveAsJson(tmpFile);
            reload = Xids.LoadFromJson(tmpFile)!;
            var mmax = reload.AllSpecifications()!.First().Cardinality as MinMaxCardinality;
            mmax!.MinOccurs.Should().Be(4);
            mmax.MaxOccurs.Should().Be(5);
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
            spec.Requirement!.Facets.Add(new IfcPropertyFacet() { PropertySetName = "pset", PropertyName = "prop" });

            var fname = "CanSerializeExtraFacetsFistPersisted.json";
            var fname2 = "CanSerializeExtraFacetReloadPersisted.json";
            x.SaveAsJson(fname);
            var reloaded = Xids.LoadFromJson(fname)!;
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
            _ = Xids.LoadFromJson(fileName);
        }

        [Fact]
        public void CanWriteSimpleFile()
        {
            var tmpFile = Path.GetTempFileName();
            Xids x = XidsTestHelpers.GetSimpleXids();
            x.SaveAsJson(tmpFile);
        }

        [Fact]
        public void CanSerializeFacetGroups()
        {
            var x = new Xids();
            var spaces = new FacetGroup(x.FacetRepository);
            spaces.Facets.Add(new IfcTypeFacet() { IfcType = "IFCSPACE" });
            spaces.Facets.Add(new MaterialFacet() { Value = "Concrete" });

            var tmpFile = Path.GetTempFileName();
            spaces.SaveAsJson(tmpFile);
            var reloaded = FacetGroupExtensions.LoadFromJson(tmpFile)!;

            reloaded.Facets.Should().HaveCount(2);
            reloaded.Facets.OfType<IfcTypeFacet>().First().IfcType = "IFCSPACE";
            reloaded.Facets.OfType<MaterialFacet>().First().Value = "Concrete";

            File.Delete(tmpFile);
        }

        [Fact]
        public void FileVersionWorks()
        {
            var x = new Xids();
            Debug.WriteLine(x.Version);
            x.Version.Should().NotBeEmpty();
        }
    }
}
