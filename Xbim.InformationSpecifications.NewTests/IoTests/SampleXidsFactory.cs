using AutoBogus;
using Bogus;
using IdsLib.IfcSchema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xbim.InformationSpecifications;
using Xbim.InformationSpecifications.Helpers;

namespace XidsEditing.Xids;

/// <summary>
/// Generates a reasonably complete sample Xids instance using AutoBogus for
/// primitive values and manual wiring for the structural relationships that
/// the domain model requires.
/// </summary>
public static class SampleXidsFactory
{
	private static readonly Faker Faker = new Faker();

	public static Xbim.InformationSpecifications.Xids Create(int specificationCount = 3, bool retainBuildingSmartOnly = false)
	{
		var xids = new Xbim.InformationSpecifications.Xids();

		// Metadata
		xids.Name = Faker.Commerce.ProductName();
		xids.Version = Faker.System.Version().ToString();
		xids.Guid = Guid.NewGuid().ToString();

		// One specifications group containing N specifications
		var group = new SpecificationsGroup(xids)
		{
			Name = Faker.Commerce.Department()
		};
		xids.SpecificationsGroups.Add(group);

		if (!retainBuildingSmartOnly)
		{
			referencedFacet = new FacetGroup(xids.FacetRepository)
			{
				Name = $"Referenced building unit",
				Guid = Guid.NewGuid().ToString()
			};
			var typeFacet = new IfcTypeFacet
			{
				IfcType = "IfcBuilding",
			};
			referencedFacet.Facets.Add(typeFacet);
			referencedFacet.Facets.Add(Faker.PickRandom(GetFacetOptions(retainBuildingSmartOnly, typeFacet.IfcType, SchemaInfo.SchemaIfc4)));
			xids.FacetRepository.Add(referencedFacet);
		}

		for (int i = 0; i < specificationCount; i++)
			CreateSpecification(xids, i, retainBuildingSmartOnly);
		return xids;
	}

	private static FacetGroup? referencedFacet = null;

	private static Specification CreateSpecification(Xbim.InformationSpecifications.Xids xids, int index, bool retainBuildingSmartOnly)
	{
		var schema = Faker.PickRandom(IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3);
		var spec = xids.PrepareSpecification(
			new[] { schema }
			);
		var ts = IfcSchemaVersionHelper.ToIds(schema);
		var sInfo = SchemaInfo.GetSchemas(ts).FirstOrDefault()!;

		var mat = Faker.Commerce.ProductMaterial().ToLowerInvariant();
		var verb = Faker.Hacker.Verb().ToLowerInvariant();
		spec.Name = $"{verb} {mat} #{index + 1}";
		spec.Description = Faker.Lorem.Sentence();
		spec.Instructions = Faker.Lorem.Sentence();

		// Applicability — what the spec selects
		spec.Applicability ??= new FacetGroup(xids.FacetRepository);
		spec.Applicability.Name = $"{Faker.Hacker.Adjective()} {mat}";
		var typeFacet = CreateTypeFacet(sInfo);
		spec.Applicability.Facets.Add(typeFacet);
		spec.Applicability.Facets.Add(Faker.PickRandom(GetFacetOptions(retainBuildingSmartOnly, typeFacet.IfcType, sInfo)));

		// Requirement — what must be true
		spec.Requirement ??= new FacetGroup(xids.FacetRepository);
		spec.Requirement.Name = $"{verb} {Faker.Hacker.Adjective()}";
		spec.Requirement.Facets.Add(Faker.PickRandom(GetFacetOptions(retainBuildingSmartOnly, typeFacet.IfcType, sInfo)));
		spec.Requirement.Facets.Add(Faker.PickRandom(GetFacetOptions(retainBuildingSmartOnly, typeFacet.IfcType, sInfo)));
		spec.Requirement.Facets.Add(Faker.PickRandom(GetFacetOptions(retainBuildingSmartOnly, typeFacet.IfcType, sInfo)));

		return spec;
	}

	private static IEnumerable<IFacet> GetFacetOptions(bool retainBuildingSmartOnly, ValueConstraint? ifcType, SchemaInfo schema)
	{
		yield return CreateClassificationFacet(ifcType);
		yield return CreatePropertyFacet(ifcType, schema);
		yield return CreatePropertyFacet(ifcType, schema); // double the chance to get property
		yield return CreateAttributeFacet(ifcType,schema);
		yield return CreatePartOfFacet(schema);
		yield return CreateMaterialFacet();
		if (!retainBuildingSmartOnly)
		{
			yield return CreateRelationFacet();
			yield return CreateDocumentFacet();
		}
	}

	private static DocumentFacet CreateDocumentFacet()  =>
		new DocumentFacet
		{
			DocName = Faker.Commerce.ProductName(),
			DocId = Faker.PickRandom("PDF", "DOCX", "XLSX", "TXT", "IFC"),
			DocLocation = Faker.Internet.Url(),
			DocIntendedUse = Faker.Lorem.Sentence(),
			DocPurpose = $"Satisfy {Faker.Company.CompanyName()} documentation requirements."  
		};

	private static IfcRelationFacet CreateRelationFacet() =>
		new IfcRelationFacet
		{
			Source = referencedFacet,
			Relation = IfcRelationFacet.RelationType.ContainedElements.ToString()
		};

	private static MaterialFacet CreateMaterialFacet() =>
		new MaterialFacet
		{
			Value = Faker.PickRandom("Perhaps concrete", "Perhaps steel", "Perhaps wood", "Perhaps glass", "Perhaps brick")
		};

	public static readonly (string IfcEntity, string System, string Code, string Description)[] KnownClassifications =
		{
			// ── IfcWall ──
			("IfcWall", "Uniclass 2015", "EF_25_10", "External walls"),
			("IfcWall", "Uniclass 2015", "EF_25_30", "Internal walls and partitions"),
			("IfcWall", "OmniClass", "21-01 10 10 10", "Foundation walls"),
			("IfcWall", "OmniClass", "21-02 20 10", "Exterior wall construction"),
			("IfcWall", "OmniClass", "21-03 10 10", "Interior partitions"),
			("IfcWall", "CI/SfB", "(21)", "External walls"),
			("IfcWall", "CI/SfB", "(22)", "Internal walls and partitions"),
			("IfcWall", "NRM1 (RICS)", "2.5", "External walls"),
			("IfcWall", "NRM1 (RICS)", "2.7", "Internal walls and partitions"),
			("IfcWall", "MasterFormat (CSI)", "04 20 00", "Unit masonry (including masonry walls)"),
			("IfcWall", "MasterFormat (CSI)", "09 21 00", "Plaster and gypsum board assemblies"),
			// ── IfcBeam ──
			("IfcBeam", "Uniclass 2015", "Ss_20_20_75", "Structural beam systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_20_75_30", "Heavy steel beam systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_20_75_65", "Precast concrete beam systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_20_75_70", "Reinforced concrete beam systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_20_75_85", "Timber beam systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_05_15_71", "Reinforced concrete pilecap and ground beam foundation systems"),
			("IfcBeam", "Uniclass 2015", "Ss_20_05_15_80", "Steel ground beam foundation systems"),
			("IfcBeam", "Uniclass 2015", "EF_20_10", "Superstructure frame"),
			("IfcBeam", "Uniclass 2015", "Pr_20_85_08_11", "Carbon steel beams"),
			("IfcBeam", "OmniClass", "21-02 10 10", "Floor structural frame"),
			("IfcBeam", "OmniClass", "21-02 10 10 10", "Floor structural columns and beams"),
			("IfcBeam", "CI/SfB", "(16)", "Foundations (ground beams)"),
			("IfcBeam", "CI/SfB", "(28)", "Structural frame members"),
			("IfcBeam", "NRM1 (RICS)", "2.1", "Frame"),
			("IfcBeam", "IFC", "IfcBeam", "Beam entity (general)"),
			("IfcBeam", "IFC", "IfcBeamStandardCase", "Standard beam (extruded profile)"),
			("IfcBeam", "MasterFormat (CSI)", "03 30 00", "Cast-in-place concrete (beams)"),
			("IfcBeam", "MasterFormat (CSI)", "05 12 00", "Structural steel framing"),
			("IfcBeam", "MasterFormat (CSI)", "06 17 00", "Shop-fabricated structural wood (glulam beams)"),
			// ── IfcSlab ──
			("IfcSlab", "Uniclass 2015", "Ss_30", "Roof, floor and paving systems"),
			("IfcSlab", "Uniclass 2015", "Ss_30_12_05", "Beam and block floor systems"),
			("IfcSlab", "Uniclass 2015", "Ss_30_12_15", "Concrete plank floor systems"),
			("IfcSlab", "Uniclass 2015", "Ss_30_12_30_70", "Reinforced concrete floor slab systems"),
			("IfcSlab", "Uniclass 2015", "Ss_30_12_30_65", "Precast concrete floor slab systems"),
			("IfcSlab", "Uniclass 2015", "Ss_30_12_30_15", "Composite metal deck floor slab systems"),
			("IfcSlab", "Uniclass 2015", "EF_30_20", "Floors"),
			("IfcSlab", "Uniclass 2015", "EF_30_10", "Roof"),
			("IfcSlab", "OmniClass", "21-02 20 20", "Roof construction"),
			("IfcSlab", "OmniClass", "21-02 10 20", "Floor construction"),
			("IfcSlab", "OmniClass", "21-01 10 20", "Foundation slab on grade"),
			("IfcSlab", "CI/SfB", "(23)", "Floors and roof decks"),
			("IfcSlab", "CI/SfB", "(27)", "Roofs"),
			("IfcSlab", "NRM1 (RICS)", "2.3", "Upper floors"),
			("IfcSlab", "NRM1 (RICS)", "2.4", "Roof"),
			("IfcSlab", "NRM1 (RICS)", "1.1.3", "Lowest floor construction"),
			("IfcSlab", "MasterFormat (CSI)", "03 30 00", "Cast-in-place concrete (slabs)"),
			("IfcSlab", "MasterFormat (CSI)", "03 40 00", "Precast concrete"),
			("IfcSlab", "MasterFormat (CSI)", "05 31 00", "Steel decking"),
			// ── IfcColumn ──
			("IfcColumn", "Uniclass 2015", "Ss_20_20", "Superstructure frame systems"),
			("IfcColumn", "Uniclass 2015", "Ss_20_20_15_30", "Heavy steel frame systems"),
			("IfcColumn", "Uniclass 2015", "Ss_20_20_15_70", "Reinforced concrete frame systems"),
			("IfcColumn", "Uniclass 2015", "Ss_20_20_15_65", "Precast concrete frame systems"),
			("IfcColumn", "Uniclass 2015", "Ss_20_20_15_85", "Timber frame systems"),
			("IfcColumn", "Uniclass 2015", "EF_20_10", "Superstructure frame"),
			("IfcColumn", "OmniClass", "21-02 10 10", "Floor structural frame"),
			("IfcColumn", "OmniClass", "21-02 10 10 10", "Floor structural columns and beams"),
			("IfcColumn", "CI/SfB", "(28)", "Structural frame members"),
			("IfcColumn", "NRM1 (RICS)", "2.1", "Frame"),
			("IfcColumn", "MasterFormat (CSI)", "03 30 00", "Cast-in-place concrete (columns)"),
			("IfcColumn", "MasterFormat (CSI)", "05 12 00", "Structural steel framing"),
			("IfcColumn", "MasterFormat (CSI)", "06 11 00", "Wood framing"),
			// ── IfcDoor ──
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20", "Door, shutter and hatch systems"),
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20_25", "Doorset systems"),
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20_16", "Collapsible gate and grille doorset systems"),
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20_30", "Framed door panel systems"),
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20_75", "Roller shutter systems"),
			("IfcDoor", "Uniclass 2015", "Ss_25_30_20_80", "Sectional overhead door systems"),
			("IfcDoor", "Uniclass 2015", "EF_25_10", "External walls (external doors)"),
			("IfcDoor", "Uniclass 2015", "EF_25_30", "Internal walls and partitions (internal doors)"),
			("IfcDoor", "OmniClass", "21-02 20 30", "Exterior doors"),
			("IfcDoor", "OmniClass", "21-03 10 30", "Interior doors"),
			("IfcDoor", "OmniClass", "21-03 10 30 10", "Interior swinging doors"),
			("IfcDoor", "OmniClass", "21-03 10 30 30", "Interior sliding doors"),
			("IfcDoor", "CI/SfB", "(31)", "Windows and external doors"),
			("IfcDoor", "CI/SfB", "(32)", "Internal doors"),
			("IfcDoor", "NRM1 (RICS)", "2.6", "Windows and external doors"),
			("IfcDoor", "NRM1 (RICS)", "2.7", "Internal walls and partitions (internal doors)"),
			("IfcDoor", "MasterFormat (CSI)", "08 11 00", "Metal doors and frames"),
			("IfcDoor", "MasterFormat (CSI)", "08 14 00", "Wood doors"),
			("IfcDoor", "MasterFormat (CSI)", "08 31 00", "Access doors and panels"),
			("IfcDoor", "MasterFormat (CSI)", "08 33 00", "Coiling doors and grilles"),
		};

	private static IfcClassificationFacet CreateClassificationFacet(ValueConstraint? ifcType)
	{
		if (ifcType is not null && ifcType.IsSingleExact<string>(out var typeName))
		{
			var validClass = KnownClassifications.Where(x=>x.IfcEntity == typeName).ToArray();
			if (validClass.Length > 0)
			{
				var selected = Faker.PickRandom(validClass);
				var t = new IfcClassificationFacet
				{
					ClassificationSystem = selected.System,
					Identification = selected.Code
				};
				return t;
			}
		}
		return new IfcClassificationFacet
		{
			ClassificationSystem = "DummyClassification",
			Identification = Faker.PickRandom("DummyVal1", "DummyVal2", "")
		};
	}

	private static IfcTypeFacet CreateTypeFacet(SchemaInfo schema)
	{
		var typeName = Faker.PickRandom("IfcWall", "IfcBeam", "IfcSlab", "IfcColumn", "IfcDoor");
		var t = new IfcTypeFacet
		{
			IfcType = typeName
		};
		var b = Faker.Random.Bool();
		if (b)
		{
			var cls = schema[typeName]!.PredefinedTypeValues;
			if (cls is not null && cls.Any())
				t.PredefinedType = Faker.PickRandom(cls);
		}
		return t;
	}

	private static IfcPropertyFacet CreatePropertyFacet(ValueConstraint? ifcType, SchemaInfo schema)
	{
		var standardProp = Faker.Random.Bool();

		// attempt relevant property
		if (standardProp && ifcType is not null && ifcType.IsSingleExact<string>(out var typeName))
		{
			var psets = schema.PropertySets.Where(x => x.ApplicableClasses.Contains(typeName));
			if (psets.Any()) {
				var pset = Faker.PickRandom(psets);
				var props = pset.Properties.OfType<SingleValuePropertyType>();
				if (props.Any())
				{
					var prop = Faker.PickRandom(props);
					return new IfcPropertyFacet
					{
						PropertySetName = pset.Name,
						PropertyName = prop.Name,
						DataType = prop.DataType,
						PropertyValue = GetPropertyValue(prop.DataType)
					};
				}
			}
		}
		// any random property
		var t = new IfcPropertyFacet
		{
			PropertySetName = Faker.PickRandom($"Custom_Pset_{Faker.Commerce.ProductMaterial()}", $"Custom_Pset_{Faker.Commerce.ProductMaterial()}", $"Custom_Pset_{Faker.Commerce.ProductMaterial()}"),
			PropertyName = Faker.Hacker.Noun(),
		};
		if (Faker.Random.Bool())
		{
			t.DataType = "IfcLabel";
			t.PropertyValue = Faker.Lorem.Word();
		}
		return t;
	}

	private static ValueConstraint? GetPropertyValue(string dataType)
	{
		if (dataType == null)
			return null;
		var gotten = ValueConstraint.TryGetNetType(dataType, out var dt);
		if (!gotten)
			return null;

		// see what's possible
		var compatible = ValueConstraint.CompatibleConstraints(dt);
		var retValue = new ValueConstraint(""); // this is a risk of crashing
		var filterPercent = Faker.Random.Double();

		if (dt == NetTypeName.String)
		{
			if (filterPercent < 0.3 && compatible.Contains(ValueConstraint.Constraints.pattern))
			{
				retValue = new ValueConstraint();
				retValue.AddAccepted(new PatternConstraint("[ABC]{3}"));
			}
			else if (filterPercent < 0.5 && compatible.Contains(ValueConstraint.Constraints.enumeration))
			{
				retValue = new ValueConstraint();
				retValue.AddAccepted(new ExactConstraint(Faker.Lorem.Word()));
				retValue.AddAccepted(new ExactConstraint(Faker.Lorem.Word()));
			}
			else
				retValue = Faker.Lorem.Word();
		}
		else if (
			dt == NetTypeName.Double
			|| dt == NetTypeName.Integer
			)
		{
			string format = dt switch
			{
				NetTypeName.Double => "N2",
				NetTypeName.Integer => "N0",
				_ => "N2"
			};

			if (filterPercent < 0.3 && compatible.Contains(ValueConstraint.Constraints.minInclusive))
			{
				retValue = new ValueConstraint();
				retValue.AddAccepted(new RangeConstraint()
				{
					MinValue = Faker.Random.Double(4, 10).ToString(format, CultureInfo.InvariantCulture),
					MaxValue = Faker.Random.Double(13, 18).ToString(format, CultureInfo.InvariantCulture),
					MinInclusive = Faker.Random.Bool(),
					MaxInclusive = Faker.Random.Bool(),
				});
			}
			else if (filterPercent < 0.5 && compatible.Contains(ValueConstraint.Constraints.enumeration))
			{
				retValue = new ValueConstraint();
				retValue.AddAccepted(new ExactConstraint(Faker.Random.Double(12, 45).ToString(format, CultureInfo.InvariantCulture)));
				retValue.AddAccepted(new ExactConstraint(Faker.Random.Double(12, 45).ToString(format, CultureInfo.InvariantCulture)));
			}
			else
				retValue = Faker.Random.Double(12, 45).ToString(format, CultureInfo.InvariantCulture);
		}
		else
		{
			retValue = new ValueConstraint(ValueConstraint.GetDefaultValue(dt)?.ToString() ?? "");
		}
		retValue.BaseType = dt;

		// structure constraint
		int? lenCon = (compatible.Contains(ValueConstraint.Constraints.length)) ? Faker.Random.Int(6, 8) : null;
		int? minLenCon = (compatible.Contains(ValueConstraint.Constraints.minLength)) ? Faker.Random.Int(0, 2) : null;
		int? maxLenCon = (compatible.Contains(ValueConstraint.Constraints.maxLength)) ? Faker.Random.Int(8, 10) : null;
		int? fracDigi = (compatible.Contains(ValueConstraint.Constraints.fractionDigits)) ? Faker.Random.Int(0, 3) : null;
		int? totDigi = (compatible.Contains(ValueConstraint.Constraints.totalDigits)) ? Faker.Random.Int(8, 10) : null;
		if (lenCon is not null ||
			minLenCon is not null ||
			maxLenCon is not null ||
			fracDigi is not null ||
			totDigi is not null)
		{
			if (lenCon.HasValue && (minLenCon.HasValue || maxLenCon.HasValue))
			{
				var choice = Faker.Random.Double(0, 1);
				if (choice < 0.3)
					lenCon = null; // drop length constraint to avoid conflict with min/max
				else if (choice < 0.6)
				{
					minLenCon = null;
					maxLenCon = null;
				}
				else
				{
					minLenCon = lenCon;
					maxLenCon = lenCon;
				}
			}

			var tmp = new StructureConstraint()
			{
				Length = lenCon,
				MinLength = minLenCon,
				MaxLength = maxLenCon,
				FractionDigits = fracDigi,
				TotalDigits = totDigi
			};
			retValue.AddAccepted(tmp);
		}
		return retValue;
	}

	private static AttributeFacet CreateAttributeFacet(ValueConstraint? ifcType, SchemaInfo schema)
	{
		var t =
		new AttributeFacet
		{
			AttributeName = Faker.PickRandom("Name", "Description", "ObjectType", "Tag"),
			AttributeValue = Faker.Lorem.Word()
		};
		return t;
	}

	private static PartOfFacet CreatePartOfFacet(SchemaInfo schema) =>
		new PartOfFacet
		{
			EntityType = CreateTypeFacet(schema),
			EntityRelation = Faker.PickRandom<PartOfFacet.PartOfRelation>().ToString()
		};

}
