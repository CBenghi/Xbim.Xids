using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Xbim.InformationSpecifications.NewTests
{
    public class buildingSmartIDSLoadTests
    {
        [Theory]
        [InlineData("bsFiles/IDS_aachen_example.xml", 1, 2)]
        [InlineData("bsFiles/IDS_random_example_04.xml", 2, 7)]
        [InlineData("bsFiles/IDS_SimpleBIM_examples.xml", 3, 9)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC2x3.xml", 2, 16)]
        [InlineData("bsFiles/IDS_ucms_prefab_pipes_IFC4.3.xml", 1, 9)]
        public void CanLoadFile(string fileName, int specificationsCount, int facetGroupsCount)
        {
            DirectoryInfo d = new DirectoryInfo(".");
            Debug.WriteLine(d.FullName);
            CheckSchema(fileName);
            var loaded = Xids.ImportBuildingSmartIDS(fileName);
            CheckCounts(specificationsCount, facetGroupsCount, loaded);


            // comment the second line below to debug any problems.
            var tmpFile = Path.Combine(Path.GetTempPath(), "out.xml");
            tmpFile = Path.GetTempFileName();


            Debug.WriteLine(tmpFile);
            loaded.ExportBuildingSmartIDS(tmpFile);
            CheckSchema(tmpFile);

            var reloaded = Xids.ImportBuildingSmartIDS(tmpFile);
            CheckCounts(specificationsCount, facetGroupsCount, reloaded);
        }

        private static void CheckSchema(string tmpFile)
        {
            IdsLib.CheckOptions c = new IdsLib.CheckOptions();
            c.CheckSchema = new[] { "bsFiles\\ids_05.xsd" };
            c.InputSource = tmpFile;

            StringWriter s = new StringWriter();
            var res = IdsLib.CheckOptions.Run(c, s);
            if (res != IdsLib.CheckOptions.Status.Ok)
            {
                Debug.WriteLine(s.ToString());
            }
            Assert.Equal(IdsLib.CheckOptions.Status.Ok, res);
        }

        private static void CheckCounts(int specificationsCount, int facetGroupsCount, Xids loaded)
        {
            Assert.NotNull(loaded);
            Assert.Equal(specificationsCount, loaded.AllSpecifications().Count());
            var grps = loaded.FacetGroups(FacetGroup.FacetUse.All);
            var tot = grps.Sum(x => x.Facets.Count());
            //var t = grps.Select(x=>x.GetType().Name).ToList();
            //Debug.WriteLine(string.Join("\t", t));
            Assert.Equal(facetGroupsCount, tot);
        }

    }
}
