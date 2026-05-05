using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

		t.ExportBuildingSmartIDS(fIds, logger);
		t.SaveAsJson(fileName, logger);
		var reloaded = Xids.LoadFromJson(fileName);
		reloaded.Should().NotBeNull();
		File.Delete(fileName);
		File.Delete(fIds);

		// Furthermore we want to check that serialization options are available
		// to serialize parts of the XIDS schema
		int i = 0;
		foreach (var id in reloaded.AllSpecifications())
		{
			TestSerialization(logger, id);
			if (i++ == 0)
			{
				List<IFacet> applic = [id.Applicability.Facets.Last()];
				TestSerialization(logger, applic);
				List<IFacet> requir = [id.Requirement!.Facets.Last()];
				TestSerialization(logger, requir);
			}
		}

	}

	private static void TestSerialization<T>(ILogger? logger, T value)
	{
		using var ms = new MemoryStream();
		SaveAsJson(value, ms, logger);
		ms.Position = 0;
		var canreloadSpec = JsonSerializer.Deserialize<T>(ms, Xids.GetJsonSerializerOptions(logger));
		canreloadSpec.Should().NotBeNull();
	}

	private static void SaveAsJson<T>(T value, Stream sw, ILogger? logger = null)
	{
		JsonSerializerOptions options = Xids.GetJsonSerializerOptions(logger);
		var t = new Utf8JsonWriter(sw, new JsonWriterOptions() { Indented = true });
		JsonSerializer.Serialize(t, value, options);
	}

	public static IEnumerable<object[]> GetTestData()
	{
		for (int i = 0; i < 50; i++)
		{
			yield return new object[] { $"RandomisedTest{i:D2}.json" };
		}
	}
}
