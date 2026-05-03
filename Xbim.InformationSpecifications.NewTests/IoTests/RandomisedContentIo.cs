using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Xbim.InformationSpecifications.Tests.IoTests;

public class RandomisedContentIo
{
	public RandomisedContentIo(ITestOutputHelper outputHelper)
	{
		OutputHelper = outputHelper;
	}

	private readonly ITestOutputHelper OutputHelper;

	internal ILogger<RandomisedContentIo> GetXunitLogger()
	{
		var services = new ServiceCollection()
					.AddLogging((builder) => builder.AddXUnit(OutputHelper));
		IServiceProvider provider = services.BuildServiceProvider();
		var logg = provider.GetRequiredService<ILogger<RandomisedContentIo>>();
		Assert.NotNull(logg);
		return logg;
	}

	[Theory(DisplayName = "Random")]
	[MemberData(nameof(GetTestData))]
	public void CanSaveRandomContent(string fileName)
	{
		var logger = GetXunitLogger();
		var d = new DirectoryInfo(".");
		logger.LogInformation($"Published in {d.FullName}");

		var fIds = fileName + ".ids";
		var t = XidsEditing.Xids.SampleXidsFactory.Create(5);
		t.ExportBuildingSmartIDS(fIds, GetXunitLogger());
		t.SaveAsJson(fileName, GetXunitLogger());

		var reloaded = Xids.LoadFromJson(fileName);

		File.Delete(fileName);
		File.Delete(fIds);
	}

	public static IEnumerable<object[]> GetTestData()
	{
		for (int i = 0; i < 50; i++)
		{
			yield return new object[] { $"RandomisedTest{i:D2}.json" };
		}
	}
}
