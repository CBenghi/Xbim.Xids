﻿using FluentAssertions;
using IdsLib;
using IdsLib.IdsSchema.IdsNodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Cardinality;
using Xbim.InformationSpecifications.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.InformationSpecifications.Tests
{
	public class IoTests
	{
		public IoTests(ITestOutputHelper outputHelper)
		{
			OutputHelper = outputHelper;
		}
		private ITestOutputHelper OutputHelper { get; }

		[Fact]
		public void CanIgnoreAnnotationsInRestriction()
		{
			var f = new FileInfo(@"bsFiles/bsFilesSelf/annotation.ids");
			Xids.CanLoad(f).Should().BeTrue();
			var x = Xids.Load(f);
			x.Should().NotBeNull();
			Assert.NotNull(x);
			var spec = x.AllSpecifications().First();
			var tpf = spec.Applicability.Facets.OfType<IfcTypeFacet>().First();
			tpf.Should().NotBeNull();
			Assert.NotNull(tpf);
			var tp = tpf.IfcType as ValueConstraint;
			tp.Should().NotBeNull();
			Assert.NotNull(tp);
			var frst = tp.AcceptedValues!.First();
			frst.Should().BeOfType<PatternConstraint>();
		}

		[Fact]
		public void CanLoadRestrictionXml()
		{
			var f = new FileInfo(@"bsFiles/others/pass-name_restrictions_will_match_any_result_1_3.ids");
			f.Exists.Should().BeTrue("file is deployed with test suite");
			Xids.CanLoad(f).Should().BeTrue();

			var xids = Xids.Load(f);
			Assert.NotNull(xids);
			var spec = xids.AllSpecifications().FirstOrDefault();
			spec.Should().NotBeNull();
			Assert.NotNull(spec);

			var attr = spec.Requirement!.Facets.First();
			attr.Should().BeOfType<AttributeFacet>();
			var asAttr = attr.As<AttributeFacet>();
			Assert.NotNull(asAttr.AttributeName);
			var av1 = asAttr.AttributeName.AcceptedValues!.First();
			av1.Should().NotBeNull();
			av1.Should().BeOfType<PatternConstraint>();
		}

		[Fact]
		public void CanSaveAndOpenMultipleRegexPatterns()
		{
			// See https://github.com/buildingSMART/IDS/issues/285

			// Arrange
			Xids xids = XidsTestHelpers.GetSimpleXids();
			IfcPropertyFacet propFacet = GetFirstPropertyFacet(xids);

			propFacet.PropertyValue = new ValueConstraint(NetTypeName.String);
			propFacet.PropertyValue!.AcceptedValues!.Add(new PatternConstraint("One"));
			propFacet.PropertyValue!.AcceptedValues!.Add(new PatternConstraint("Two"));

			// test buildingsmart IDS export
			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			xids.ExportBuildingSmartIDS(filename);
			var file = File.ReadAllText(filename);
			var occurrences = new Regex("<xs:pattern").Matches(file); // the file has it
			occurrences.Should().HaveCount(2, "Expected saved file to have TWO patterns");
			xids = Xids.LoadBuildingSmartIDS(filename)!;
			propFacet = GetFirstPropertyFacet(xids);
			propFacet.PropertyValue!.AcceptedValues.Should().HaveCount(2, "Expected to load TWO patterns");

			// test json export
			filename = Path.ChangeExtension(Path.GetTempFileName(), "json");
			xids.SaveAsJson(filename);
			xids = Xids.Load(new FileInfo(filename))!;
			propFacet = GetFirstPropertyFacet(xids);
			propFacet.PropertyValue!.AcceptedValues.Should().HaveCount(2, "Expected to load TWO patterns");
		}

		private static IfcPropertyFacet GetFirstPropertyFacet(Xids xids)
		{
			var spec = xids.AllSpecifications().FirstOrDefault();
			var facet = spec!.Requirement!.Facets.First();
			facet.Should().BeOfType<IfcPropertyFacet>();
			var propFacet = facet.As<IfcPropertyFacet>();
			return propFacet;
		}

		[Fact]
		public void CanLoadRestrictionXml2()
		{
			var file = new FileInfo(@"bsFiles/bsFilesSelf/TestFile.ids");
			Xids.CanLoad(file).Should().BeTrue();

			var xids = Xids.Load(file, LoggingTestHelper.GetXunitLogger<IoTests>(OutputHelper));
			Assert.NotNull(xids);
			var spec = xids.AllSpecifications().Single();

			var reqs = spec.Requirement;
			Assert.NotNull(reqs);

			var attr = reqs.Facets.First().As<AttributeFacet>();
			Assert.NotNull(attr);
			attr.AttributeName.Should().NotBeNull();
			var av1 = attr.AttributeName!.AcceptedValues!.First();
			av1.Should().NotBeNull();
			av1.Should().BeOfType<PatternConstraint>();

			reqs.Facets.Should().HaveCount(4);

			var ifcType = reqs.Facets[1].As<IfcTypeFacet>();
			Assert.NotNull(ifcType);
			av1 = ifcType.IfcType!.AcceptedValues![0];
			av1.Should().NotBeNull();
			av1.Should().BeOfType<ExactConstraint>();
			ifcType.IfcType.AcceptedValues.Should().HaveCount(2);

			// Pset
			var pset = spec.Requirement!.Facets[2];
			pset.Should().BeOfType<IfcPropertyFacet>();

			var psType = pset.As<IfcPropertyFacet>();
			Assert.NotNull(psType.PropertyName);
			av1 = psType.PropertyName.AcceptedValues!.First();
			av1.Should().NotBeNull();

			av1.Should().BeOfType<PatternConstraint>();
			psType.PropertyName.AcceptedValues.Should().HaveCount(1);

			// Material without value
			var mat = spec.Requirement.Facets[3];
			mat.Should().BeOfType<MaterialFacet>();

			var matType = mat.As<MaterialFacet>();
			matType.Value.Should().BeNull();
		}

		[Fact]
		public void CanLoadOptionalFacet()
		{
			var f = new FileInfo(@"bsFiles/bsFilesSelf/OptionalFacets.ids");
			Xids.CanLoad(f).Should().BeTrue();

			var xids = Xids.Load(f);
			Assert.NotNull(xids);

			var spec = xids.AllSpecifications().FirstOrDefault();
			Assert.NotNull(spec);
			Assert.NotNull(spec.Requirement);

			var attr = spec.Requirement.Facets.FirstOrDefault();
			attr.Should().BeOfType<AttributeFacet>();
			spec.Requirement.RequirementOptions.Should().HaveCount(1);
			spec.Requirement.RequirementOptions![0].RelatedFacetCardinality.Should().Be(RequirementCardinalityOptions.Cardinality.Optional);

		}

		[Fact]
		public void CannotLoadInvalidXml()
		{
			var f = new FileInfo(@"Files/IDS_with_invalid_entities.xml");
			Xids.CanLoad(f, LoggingTestHelper.GetXunitLogger<IoTests>(OutputHelper)).Should().BeFalse();
		}


		[Fact]
		public void ShouldIgnoreWhitespaceInElements()
		{
			var f = new FileInfo(@"bsFiles/others/SimpleValueString_whitepace.ids");
			Xids.CanLoad(f).Should().BeTrue();

			var x = Xids.Load(f);
			x.Should().NotBeNull();

			var spec = x!.AllSpecifications().FirstOrDefault();
			spec.Should().NotBeNull();


			var facets = spec!.Requirement!.Facets.Union(spec.Applicability.Facets);
			foreach (var facet in facets)
			{
				switch (facet)
				{
					case IfcTypeFacet type:
						ValidateConstraint(type.IfcType, "string");
						ValidateConstraint(type.PredefinedType, "string");
						break
							;
					case AttributeFacet attr:
						ValidateConstraint(attr.AttributeName, "string");
						ValidateConstraint(attr.AttributeValue, "string");
						break;

					case IfcPropertyFacet prop:
						ValidateConstraint(prop.PropertySetName, "string");
						ValidateConstraint(prop.PropertyName, "string");
						ValidateConstraint(prop.PropertyValue, "string");
						break;

					case IfcClassificationFacet cls:
						ValidateConstraint(cls.ClassificationSystem, "string");
						ValidateConstraint(cls.Identification, "string");
						break;

					case MaterialFacet mat:
						ValidateConstraint(mat.Value, "string");
						break;

					case PartOfFacet part:
						ValidateConstraint(part.EntityType!.IfcType, "string");
						ValidateConstraint(part.EntityType!.PredefinedType, "string");
						break;

					default:
						Assert.Fail($"Not implemented {facet.GetType().Name}");
						break;
				}
			}


		}

		private static void ValidateConstraint(ValueConstraint? constraint, string expected)
		{
			constraint.Should().NotBeNull();
			if (constraint!.IsSingleExact(out var value))
			{
				value.ToString().Should().Be(expected);
			}
			else
			{
				if (constraint.AcceptedValues!.Count > 1)
				{
					foreach (var enumValue in constraint.AcceptedValues!)
					{
						enumValue.IsSatisfiedBy(expected, constraint, false).Should().BeTrue();
					}
				}
				else
				{
					var complexConstraint = constraint.AcceptedValues!.FirstOrDefault();
					switch (complexConstraint)
					{
						case PatternConstraint p:
							p.Pattern.Should().Be(expected);
							break;
						case RangeConstraint r:
							r.MinValue.Should().Be(expected);
							break;
					}
				}
			}
		}


		[Fact]
		public void CanLoadXml()
		{
			var f = new FileInfo(@"Files/IDS_example-with-restrictions.xml");
			Xids.CanLoad(f).Should().BeTrue();

			var x = Xids.Load(f);
			x.Should().NotBeNull();
		}

		[Theory]
		[InlineData("TestContent.ids")]
		[InlineData("IDS_example-with-restrictions.xml")]
		public void CanLoadIDS(string fileName)
		{
			var f = new FileInfo($@"Files/{fileName}");
			Xids.CanLoad(f).Should().BeTrue();

			var x = Xids.Load(f);
			x.Should().NotBeNull();
		}

		[Fact]
		public void CanLoadJson()
		{
			var f = new FileInfo(@"Files/newFormat.json");
			Xids.CanLoad(f).Should().BeTrue();

			var x = Xids.Load(f);
			x.Should().NotBeNull();
		}

		[Fact]
		public void WarnsJsonVersion()
		{
			var loggerMock = Substitute.For<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
			var f = new FileInfo(@"Files/FutureFormat.json");
			Xids.CanLoad(f, loggerMock).Should().BeTrue();
			LoggingTestHelper.SomeIssues(loggerMock);
		}


		[Fact]
		public void NoWarnsOnCurrentJsonVersion()
		{
			Xids x = XidsTestHelpers.GetSimpleXids();
			var filename = Path.GetTempFileName();
			x.SaveAsJson(filename);
			Assert.True(File.Exists(filename));

			var loggerMock = Substitute.For<ILogger<BuildingSmartCompatibilityTests>>(); // this is to check events
			var f = new FileInfo(filename);
			Debug.WriteLine(f.FullName);
			Xids.CanLoad(f, loggerMock).Should().BeTrue();
			LoggingTestHelper.NoIssues(loggerMock);
			File.Delete(filename);
		}

		[Fact]
		public void CanSaveXmlAsZip()
		{
			Xids? x = BuildMultiSpecGroupIDS();

			using var ms = new MemoryStream();

			x.ExportBuildingSmartIDS(ms);

			// Check the stream is a PK Zip stream by looking at 'magic' first 4 bytes
			Xids.IsZipped(ms).Should().BeTrue();
		}

		[Fact]
		public void ZippedIDSSpecsContainsIDS()
		{
			Xids? x = BuildMultiSpecGroupIDS();
			using var ms = new MemoryStream();
			x.ExportBuildingSmartIDS(ms);

			// Check Contains IDS files & content
			using var archive = new ZipArchive(ms, ZipArchiveMode.Read, false);
			archive.Entries.Should().HaveCount(2);
			archive.Entries.Should().AllSatisfy(e => e.Name.Should().EndWith(".ids", "IDS file extension expected"));
			archive.Entries.Should().AllSatisfy(e => e.Length.Should().BeGreaterThan(0, "Content expected"));

			// entries are valid
			//
			var xlogger = LoggingTestHelper.GetXunitLogger<IoTests>(OutputHelper);
			var opt = new SingleAuditOptions()
			{
				IdsVersion = IdsFacts.DefaultIdsVersion,
				SchemaProvider = new IdsLib.SchemaProviders.FixedVersionSchemaProvider(IdsFacts.DefaultIdsVersion)
			};
			archive.Entries.Should().AllSatisfy(e => Audit.Run(e.Open(), opt, xlogger).Should().Be(Audit.Status.Ok));

		}

		[Fact]
		public void CanLoadXmlAsZip()
		{
			Xids? x = BuildMultiSpecGroupIDS();

			using var ms = new MemoryStream();

			x.ExportBuildingSmartIDS(ms);
			ms.Position = 0;

			var newIds = Xids.LoadBuildingSmartIDS(ms);

			newIds.Should().NotBeNull();
			newIds!.SpecificationsGroups.Should().HaveCount(2);
		}

		[Fact]
		public void CanLoadXmlFromZipFile()
		{
			Xids? x = BuildMultiSpecGroupIDS();
			var tempXmlFile = Path.ChangeExtension(Path.GetTempFileName(), "zip");
			using (var fs = new FileStream(tempXmlFile, FileMode.Create))
			{
				x.ExportBuildingSmartIDS(fs);
			}



			var newIds = Xids.LoadBuildingSmartIDS(tempXmlFile);
			newIds.Should().NotBeNull();
			newIds!.SpecificationsGroups.Should().HaveCount(2);
		}

		[InlineData(CardinalityEnum.Required)]
		[InlineData(CardinalityEnum.Optional)]
		[InlineData(CardinalityEnum.Prohibited)]
		[Theory]
		public void CanRoundTripCardinalityInXml(CardinalityEnum cardinality)
		{
			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			try
			{
				Xids x = XidsTestHelpers.GetSimpleXids();

				x.AllSpecifications().First().Cardinality = new SimpleCardinality(cardinality);
				x.ExportBuildingSmartIDS(filename, LoggingTestHelper.GetXunitLogger<IoTests>(OutputHelper));

				var newXids = Xids.LoadBuildingSmartIDS(filename);
				var cardinal = newXids!.AllSpecifications().First().Cardinality;
				cardinal.Should().BeOfType<SimpleCardinality>();
				((SimpleCardinality)cardinal).ApplicabilityCardinality.Should().Be(cardinality);
			}
			finally
			{
				if (File.Exists(filename))
					File.Delete(filename);
			}
		}

		[Theory]
		[MemberData(nameof(TestFacets))]
		public void CanRoundTripFacetRequirementCardinality(IFacet facet, RequirementCardinalityOptions.Cardinality cardinality)
		{
			var x = new Xids();
			var newspec = x.PrepareSpecification(IfcSchemaVersion.IFC2X3);
			newspec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IFCWALL" });
			newspec.Requirement!.Facets.Add(facet);
			newspec.Requirement!.RequirementOptions = new ObservableCollection<RequirementCardinalityOptions> { new(facet, cardinality) };

			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			x.ExportBuildingSmartIDS(filename, GetXunitLogger());

			var newXids = Xids.LoadBuildingSmartIDS(filename);
			var newRequirement = newXids!.SpecificationsGroups.First().Specifications.First().Requirement;
			var newCardinality = newRequirement?.RequirementOptions?.FirstOrDefault()?.RelatedFacetCardinality;
			newCardinality.Should().Be(cardinality);
		}

		private ILogger? GetXunitLogger()
		{
			return LoggingTestHelper.GetXunitLogger<IoTests>(OutputHelper);
		}

		[Fact]
		public void EvaluatesAllFacetTypes()
		{
			var expectedTypes = typeof(AttributeFacet).Assembly.GetTypes()
			.Where(t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(typeof(IFacet)) && !t.IsAssignableTo(typeof(IfcTypeFacet)));
			var testTypes = TestFacets.Select(f => f.FirstOrDefault() as IFacet).Select(f => f!.GetType());
			expectedTypes.Should().Contain(testTypes);
		}

		// combination of all types of facets and cardinalities
		private static readonly RequirementCardinalityOptions.Cardinality[] cardinalities = new RequirementCardinalityOptions.Cardinality[] {
			RequirementCardinalityOptions.Cardinality.Optional, RequirementCardinalityOptions.Cardinality.Prohibited, RequirementCardinalityOptions.Cardinality.Expected};
		public static IEnumerable<object[]> TestFacets => new IFacet[] { 
			// new IfcTypeFacet { IfcType = "IfcWall" },
			new AttributeFacet{ AttributeName = "Name"},
			new IfcClassificationFacet { ClassificationSystem = "NRM"},
			new IfcPropertyFacet { PropertyName = "Name" },
			new MaterialFacet { Value = "Concrete" },
			new PartOfFacet { EntityRelation =  PartOfFacet.PartOfRelation.IfcRelNests.ToString(), EntityType = new IfcTypeFacet { IfcType = "IfcBuildingElementPart"} }
		}
			.SelectMany(f => cardinalities.Select(c => new object[] { f!, c }));

		[InlineData(CardinalityEnum.Required)]
		[InlineData(CardinalityEnum.Optional)]
		[InlineData(CardinalityEnum.Prohibited)]
		[Theory]
		public void CanRoundTripCardinalityInJson(CardinalityEnum cardinality)
		{
			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			try
			{
				Xids x = XidsTestHelpers.GetSimpleXids();

				x.AllSpecifications().First().Cardinality = new SimpleCardinality(cardinality);
				x.SaveAsJson(filename);

				var newXids = Xids.LoadFromJson(filename);
				var cardinal = newXids!.AllSpecifications().First().Cardinality;
				cardinal.Should().BeOfType<SimpleCardinality>();
				((SimpleCardinality)cardinal).ApplicabilityCardinality.Should().Be(cardinality);
			}
			finally
			{
				if (File.Exists(filename)) File.Delete(filename);
			}
		}

		[Fact]
		public void ResavingXmlShouldTruncateFileWhenShorter()
		{
			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			var logger = GetXunitLogger();
			try
			{
				// Arrange
				Xids x = XidsTestHelpers.GetSimpleXids();

				var spec = x.AllSpecifications().First();
				spec.Description = "Some longish descripion, just to pad the file out a bit...";

				x.ExportBuildingSmartIDS(filename, logger);

				var initialLength = new FileInfo(filename).Length;

				// Act
				spec.Description = "Shorter now";
				x.ExportBuildingSmartIDS(filename, logger); // resave over original, which _should_ truncate

				// Assert
				var latest = new FileInfo(filename);
				Xids.CanLoad(latest, logger).Should().BeTrue("file should not be corrupt");

				var latestLength = new FileInfo(filename).Length;
				latestLength.Should().BeLessThan(initialLength, "file expected to have shrunk");

			}
			finally
			{
				if (File.Exists(filename)) File.Delete(filename);
			}
		}

		[Fact]
		public void ResavingJsonShouldTruncateFileWhenShorter()
		{
			var filename = Path.ChangeExtension(Path.GetTempFileName(), "ids");
			var logger = GetXunitLogger();
			try
			{
				// Arrange
				Xids x = XidsTestHelpers.GetSimpleXids();

				var spec = x.AllSpecifications().First();
				spec.Description = "Some longish descripion, just to pad the file out a bit...";

				x.SaveAsJson(filename);

				var initialLength = new FileInfo(filename).Length;

				// Act
				spec.Description = "Shorter now";
				x.SaveAsJson(filename); // resave over original, which _should_ truncate

				// Assert
				var latest = new FileInfo(filename);
				Xids.CanLoad(latest, logger).Should().BeTrue("file should not be corrupt");

				var latestLength = new FileInfo(filename).Length;
				latestLength.Should().BeLessThan(initialLength, "file expected to have shrunk");

			}
			finally
			{
				if (File.Exists(filename)) File.Delete(filename);
			}
		}

		private static Xids BuildMultiSpecGroupIDS()
		{
			var file = new FileInfo(@"bsFiles/bsFilesSelf/TestFile.ids");
			var x = Xids.Load(file);
			x.Should().NotBeNull();
			x!.AllSpecifications().Should().HaveCount(1);

			// Add a 2nd group with a basic spec
			var specGroup = new SpecificationsGroup(x);
			x.SpecificationsGroups.Add(specGroup);

			var newSpec = x.PrepareSpecification(specGroup, IfcSchemaVersion.IFC4);
			newSpec.Applicability.Facets.Add(new IfcTypeFacet() { IfcType = "IfcDoor" });

			x!.AllSpecifications().Should().HaveCount(2);
			return x;
		}
	}
}
