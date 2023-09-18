using FluentAssertions;
using IdsLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.Tests
{
    public class BuildingSmartIDSLoadTests
    {
        public BuildingSmartIDSLoadTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }
        private ITestOutputHelper OutputHelper { get; }

        internal ILogger<BuildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<BuildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        [Theory]
        [InlineData("bsFiles/IDS_aachen_example.ids", 1, 2, 0)]
        [InlineData("bsFiles/IDS_Aedes_example.ids", 1, 2, 0)]
        [InlineData("bsFiles/IDS_ArcDox.ids", 5, 21, 0)]
        [InlineData("bsFiles/IDS_random_example.ids", 2, 7, 0)]
        [InlineData("bsFiles/IDS_SimpleBIM_examples.ids", 2, 7, 0)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC2x3.ids", 2, 16, 0)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC4.3.ids", 1, 9, 0)]
        [InlineData("bsFiles/IDS_wooden-windows.ids", 6, 34, 0)]
        [InlineData("bsFiles/IDS_demo_BIM-basis-ILS.ids", 3, 8, 0)]
        public void CanLoadAndSaveFile(string fileName, int expectedSpecificationCount, int expectedfacetGroupsCount, int expectedErrCount)
        {
            var outputFile = Path.Combine(Path.GetTempPath(), "out.ids");
            outputFile = Path.GetTempFileName();
            try
            {
                DirectoryInfo d = new(".");
                Debug.WriteLine(d.FullName);
                ILogger<BuildingSmartIDSLoadTests> logg = GetXunitLogger();
                CheckSchema(fileName, logg);
                var loggerMock = new Mock<ILogger<BuildingSmartIDSLoadTests>>();
                var loaded = Xids.LoadBuildingSmartIDS(fileName, logg); // this sends the log to xunit context, for debug purposes.
                loaded = Xids.LoadBuildingSmartIDS(fileName, loggerMock.Object); // we load again with the moq to check for logging events
                Assert.NotNull(loaded);
                var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
                var errorAndWarnings = loggingCalls.Where(x => x is not null && (x.Contains("Error") || x.Contains("Warning")));
                errorAndWarnings.Count().Should().Be(expectedErrCount, "mismatch with expected value");
                CheckCounts(loaded, expectedSpecificationCount, expectedfacetGroupsCount, "first check");
                loaded.ExportBuildingSmartIDS(outputFile);
                CheckSchema(outputFile, logg);
                var reloaded = Xids.LoadBuildingSmartIDS(outputFile);
                Assert.NotNull(reloaded);
                CheckCounts(reloaded, expectedSpecificationCount, expectedfacetGroupsCount, "reloaded count");
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(outputFile);
            }
        }

        private static void CheckSchema(string tmpFile, ILogger<BuildingSmartIDSLoadTests>? logg = null)
        {
            using var tmpStream = File.OpenRead(tmpFile);
            var opt = new SingleAuditOptions()
            {
                IdsVersion = IdsLib.IdsSchema.IdsNodes.IdsVersion.Ids0_9,
                SchemaProvider = new IdsLib.SchemaProviders.FixedVersionSchemaProvider(IdsLib.IdsSchema.IdsNodes.IdsVersion.Ids0_9)
            };
            var varlidationResult = Audit.Run(tmpStream, opt, logg);
            varlidationResult.Should().Be(Audit.Status.Ok, $"file '{tmpFile}' is expected to be valid");
        }

        private static void CheckCounts(Xids xidsToVerify, int specificationsCount, int facetGroupsCount, string stage)
        {
            Assert.NotNull(xidsToVerify);
            if (specificationsCount != -1)
            {
                xidsToVerify.AllSpecifications().Should().HaveCount(specificationsCount, $"it's the expected count at {stage} stage");
            }
            if (facetGroupsCount != -1)
            {
                var grps = xidsToVerify.FacetGroups(FacetGroup.FacetUse.All);
                var tally = 0;
                foreach (var item in grps)
                {
                    tally += item.Facets.Count;
                }
                tally.Should().Be(facetGroupsCount, $"it's the expected hardcoded count, performing the {stage} stage");   
            }
        }

    }
}
