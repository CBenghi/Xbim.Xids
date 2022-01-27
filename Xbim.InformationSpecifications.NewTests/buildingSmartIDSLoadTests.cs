using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.NewTests
{
    public class buildingSmartIDSLoadTests
    {
        public buildingSmartIDSLoadTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        private ITestOutputHelper OutputHelper { get; }

        [Theory]
        //[InlineData("bsFiles/IDS_aachen_example.xml", 1, 2)]
        //[InlineData("bsFiles/IDS_random_example_04.xml", 2, 7)]
        //[InlineData("bsFiles/IDS_SimpleBIM_examples.xml", 3, 9)]
        //[InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC2x3.xml", 2, 16)]
        //[InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC4.3.xml", 1, 9)]
        
        [InlineData("bsFiles/bsFilesSelf/SimpleValueString.xml", -1, -1)]
        [InlineData("bsFiles/bsFilesSelf/SimpleValueRestriction.xml", -1, -1)]
        public void CanLoadFile(string fileName, int specificationsCount, int facetGroupsCount)
        {
            DirectoryInfo d = new DirectoryInfo(".");
            Debug.WriteLine(d.FullName);
            ILogger<buildingSmartIDSLoadTests> logg = GetXunitLogger();
            CheckSchema(fileName, logg);
            

            var loggerMock = new Mock<ILogger<buildingSmartIDSLoadTests>>();

            var loaded = Xids.ImportBuildingSmartIDS(fileName, logg); // this sends the log to xunit context, for debug purposes.
            loaded = Xids.ImportBuildingSmartIDS(fileName, loggerMock.Object); // we load again with the moq to check for logging events
            var loggingCalls = loggerMock.Invocations.Select(x => x.ToString()).ToArray(); // this creates the array of logging calls
            loggingCalls.Where(x => x.Contains("Error") || x.Contains("Warning")).Should().BeEmpty("no calls to errors or warnings are expected");
            CheckCounts(specificationsCount, facetGroupsCount, loaded);

            var outputFile = Path.Combine(Path.GetTempPath(), "out.xml");
            outputFile = Path.GetTempFileName(); 

            Debug.WriteLine(outputFile);
            loaded.ExportBuildingSmartIDS(outputFile);
            CheckSchema(outputFile, logg);

            var reloaded = Xids.ImportBuildingSmartIDS(outputFile);
            CheckCounts(specificationsCount, facetGroupsCount, reloaded);
        }

        internal ILogger<buildingSmartIDSLoadTests> GetXunitLogger()
        {
            var services = new ServiceCollection()
                        .AddLogging((builder) => builder.AddXUnit(OutputHelper));
            IServiceProvider provider = services.BuildServiceProvider();
            var logg = provider.GetRequiredService<ILogger<buildingSmartIDSLoadTests>>();
            Assert.NotNull(logg);
            return logg;
        }

        private static void CheckSchema(string tmpFile, ILogger<buildingSmartIDSLoadTests> logg = null)
        {
            IdsLib.CheckOptions c = new IdsLib.CheckOptions();
            c.CheckSchema = new[] { "bsFiles\\ids_05.xsd" };
            c.InputSource = tmpFile;

            StringWriter s = new StringWriter();
            var res = IdsLib.CheckOptions.Run(c, s);
            if (res != IdsLib.CheckOptions.Status.Ok)
            {
                logg.LogError(s.ToString());
            }
            res.Should().Be(IdsLib.CheckOptions.Status.Ok, $"file '{tmpFile}' is otherwise invalid");
        }

        private static void CheckCounts(int specificationsCount, int facetGroupsCount, Xids loaded)
        {
            Assert.NotNull(loaded);
            if (specificationsCount != -1)
                Assert.Equal(specificationsCount, loaded.AllSpecifications().Count());
            if (facetGroupsCount != -1)
            {
                var grps = loaded.FacetGroups(FacetGroup.FacetUse.All);
                var tot = grps.Sum(x => x.Facets.Count());
                //var t = grps.Select(x=>x.GetType().Name).ToList();
                //Debug.WriteLine(string.Join("\t", t));
                Assert.Equal(facetGroupsCount, tot);
            }
        }

    }
}
