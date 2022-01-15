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
        //[Theory]
        //[InlineData("bsFiles/IDS_aachen_example.xml", 1, 2)]
        public void CanLoadFile(string fileName, int specificationsCount, int facetGroupsCount)
        {
            DirectoryInfo d = new DirectoryInfo(".");
            Debug.WriteLine(d.FullName);

            var tmp = Xids.ImportBuildingSmartIDS(fileName);
            Assert.NotNull(tmp);    

            Assert.Equal(specificationsCount, tmp.AllSpecifications().Count());
            var grps = tmp.FacetGroups(FacetGroup.FacetUse.All);

            var tot = grps.Sum(x => x.Facets.Count());

            var t = grps.Select(x=>x.GetType().Name).ToList();
            Debug.WriteLine(string.Join("\t", t));
            Assert.Equal(facetGroupsCount, tot);
        }

        [Fact]
        public void LoadOneFile()
        {
            CanLoadFile("bsFiles/IDS_aachen_example.xml", 1, 2);
            CanLoadFile("bsFiles/IDS_random_example_04.xml", 2, 7);
            CanLoadFile("bsFiles/IDS_SimpleBIM_examples.xml", 3, 9);
            CanLoadFile("bsFiles/IDS_ucms_prefab_pipes_IFC2x3.xml", 2, 16);
            CanLoadFile("bsFiles/IDS_ucms_prefab_pipes_IFC4.3.xml", 1, 9);
        }
    }
}
