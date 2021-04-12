using System;
using Xunit;
using Xbim.Xids;
using System.IO;
using System.Diagnostics;
using FluentAssertions;
using System.Linq;

namespace Xbim.Xids.NewTests
{
	public class NewJsonTests
	{
		[Fact]
		public void CanWriteSimpleFormat()
		{
			var d = new DirectoryInfo(".");
			Debug.WriteLine(d.FullName);

			var x = new Xids();
			var newspec = x.NewSpecification();
			newspec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcWall" });

			newspec.Requirement.Facets.Add(
				new IfcPropertyFacet()
				{
					PropertySetName = "Pset",
					PropertyName = "Prop"
				}
				);
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
	}
}
