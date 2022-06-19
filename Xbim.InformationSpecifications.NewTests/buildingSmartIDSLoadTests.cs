using FluentAssertions;
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

        [Theory]
        [InlineData("bsFiles/IDS_aachen_example.xml", 1, 2, 0)]
        [InlineData("bsFiles/IDS_Aedes_example.xml", 1, 2, 0)]
        [InlineData("bsFiles/IDS_ArcDox.xml", 5, 21, 0)]
        [InlineData("bsFiles/IDS_random_example.xml", 2, 7, 1)]
        [InlineData("bsFiles/IDS_SimpleBIM_examples.xml", 3, 9, 0)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC2x3.xml", 2, 16, 0)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC4.3.xml", 1, 9, 0)]
        [InlineData("bsFiles/IDS_wooden-windows.xml", 5, 31, 3)]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueString.xml", -1, -1, 0)]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueRestriction.xml", -1, -1, 0)]
        public void CanLoadAndSaveFile(string fileName, int specificationsCount, int facetGroupsCount, int err)
        {
            var outputFile = Path.Combine(Path.GetTempPath(), "out.xml");
            outputFile = Path.GetTempFileName();
            try
            {
                DirectoryInfo d = new(".");
                Debug.WriteLine(d.FullName);
                ILogger<BuildingSmartIDSLoadTests> logg = GetXunitLogger();
                CheckSchema(fileName, logg);
                var loggerMock = new Mock<ILogger<BuildingSmartIDSLoadTests>>();
                var loaded = Xids.ImportBuildingSmartIDS(fileName, logg); // this sends the log to xunit context, for debug purposes.
                loaded = Xids.ImportBuildingSmartIDS(fileName, loggerMock.Object); // we load again with the moq to check for logging events
                var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
                var errorAndWarnings = loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning"));
                errorAndWarnings.Count().Should().Be(err, "mismatch with expected value");
                CheckCounts(specificationsCount, facetGroupsCount, loaded);
                loaded.ExportBuildingSmartIDS(outputFile);
                CheckSchema(outputFile, logg);
                var reloaded = Xids.ImportBuildingSmartIDS(outputFile);
                CheckCounts(specificationsCount, facetGroupsCount, reloaded);
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

        internal ILogger<BuildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<BuildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        private static void CheckSchema(string tmpFile, ILogger<BuildingSmartIDSLoadTests> logg = null)
        {
            IdsLib.CheckOptions c = new()
            {
                CheckSchema = new[] { "bsFiles\\ids_06.xsd" },
                InputSource = tmpFile
            };

            StringWriter s = new();
            var varlidationResult = IdsLib.CheckOptions.Run(c, s);
            if (varlidationResult != IdsLib.CheckOptions.Status.Ok)
            {
#pragma warning disable CA2254 // Template should be a static expression, but we consider acceptable in the test.
                logg?.LogError(s.ToString());
#pragma warning restore CA2254 // Template should be a static expression
            }
            varlidationResult.Should().Be(IdsLib.CheckOptions.Status.Ok, $"file '{tmpFile}' is otherwise invalid");
        }

        private static void CheckCounts(int specificationsCount, int facetGroupsCount, Xids loaded)
        {
            Assert.NotNull(loaded);
            if (specificationsCount != -1)
                Assert.Equal(specificationsCount, loaded.AllSpecifications().Count());
            if (facetGroupsCount != -1)
            {
                var grps = loaded.FacetGroups(FacetGroup.FacetUse.All);
                var tally = 0;
                foreach (var item in grps)
                {
                    tally += item.Facets.Count;
                }
                //var t = grps.Select(x=>x.GetType().Name).ToList();
                //Debug.WriteLine(string.Join("\t", t));
                Assert.Equal(facetGroupsCount, tally);
            }
        }

    }
}
