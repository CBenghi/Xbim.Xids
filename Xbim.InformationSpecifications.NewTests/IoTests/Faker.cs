using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Xbim.InformationSpecifications;
using Xbim.InformationSpecifications.Helpers;

namespace XidsEditing.InformationSpecifications
{
	internal class Faker
	{
		Random r = new Random();
		ConcurrentDictionary<string, FakerContainer> _containers = new();

		private FakerContainer GetContainer(string name, Faker faker)
		{
			return _containers.GetOrAdd(name, key => new FakerContainer(key, faker));
		}
		public FakerContainer Commerce => GetContainer(nameof(Commerce), this);
		public FakerContainer Ifc => GetContainer(nameof(Ifc), this);
		public FakerContainer Construction => GetContainer(nameof(Construction), this);
		public FakerContainer Internet => GetContainer(nameof(Internet), this);
		public FakerContainer Lorem => GetContainer(nameof(Lorem), this);
		public FakerContainer Generic => GetContainer(nameof(Generic), this);

		internal T PickRandom<T>(IList<T> options)
		{
			int index = r.Next(options.Count);
			return options[index];
		}

		internal bool RandomBool()
		{
			return r.NextDouble() >= 0.5;
		}

		internal double RandomDouble(double from = 0, double to = 1)
		{
			return r.NextDouble() * (to - from) + from;
		}

		internal int? RandomInt(int min, int max)
		{
			return r.Next(min, max + 1);
		}
	}

	internal class FakerContainer
	{
		private string _containerType;
		private Faker _parent;
		public FakerContainer(string containerType, Faker faker)
		{
			_containerType = containerType;
			_parent = faker;
		}

		internal string Activity()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom(["Excavation", "Foundation", "Framing", "Finishing"]),
				_ => throw new NotImplementedException()
			};
		}

		internal string BuildingPart()
		{
			return _containerType switch
			{
				_ => _parent.PickRandom([
					"wall", 	"floor", 	"roof", 	"ceiling", 	"column", 	"beam", 	"foundation",
					"slab", 	"stair", 	"ramp", 	"door",  	"window", 	"curtain wall", 	"facade panel",
					"balcony", 	"parapet",  "canopy", 	"chimney", 	"skylight", 	"partition", 	"handrail", 	"balustrade",
					"lintel", 	"truss", 	"joist", 	"rafter", 	"duct", 	"pipe", 	"cable tray", 	"elevator shaft" 
					]),
			};
		}

		internal string EngineeringOrFunctionalNeeds()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom([
					"structural support", "thermal insulation", "acoustic performance", "fire resistance", "aesthetic appeal",
					"weatherproofing", "moisture control", "vapour barrier", "air tightness", "load bearing capacity",
					"seismic resistance", "wind load resistance", "impact resistance", "durability", "corrosion resistance",
					"chemical resistance", "daylighting", "solar shading", "ventilation", "energy efficiency",
					"carbon footprint", "sustainability", "recyclability", "accessibility", "security",
					"blast resistance", "vibration damping", "electromagnetic shielding", "hygiene", "ease of maintenance",
					"buildability", "cost efficiency", "modularity", "dimensional stability", "watertightness",
					"drainage", "thermal mass", "reflectivity", "transparency", "privacy"
					]),
				_ => throw new NotImplementedException()
			};
		}

		internal string ExcitingAdjective()
		{
			return _containerType switch
			{
				_ => _parent.PickRandom(["exciting", "amazing", "fantastic", "incredible", "wonderful"]),
			};
		}

		internal string Pset()
		{
			return _containerType switch
			{
				nameof(Faker.Ifc) => _parent.PickRandom(
					[
					$"Custom_{ExcitingAdjective().FirstCharToUpper()}_{BuildingPart().FirstCharToUpper()}Common",
					$"Custom_{ExcitingAdjective().FirstCharToUpper()}_{BuildingPart().FirstCharToUpper()}Common",
					$"Custom_{ExcitingAdjective().FirstCharToUpper()}_{BuildingPart().FirstCharToUpper()}Common",
					$"Custom_{ExcitingAdjective().FirstCharToUpper()}_{BuildingPart().FirstCharToUpper()}Common",
					$"Custom_{ExcitingAdjective().FirstCharToUpper()}_{BuildingPart().FirstCharToUpper()}Common"
					]
					),
				_ => throw new NotImplementedException()
			};
		}

		internal string Role()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom([
					"Architect", "Engineer", "Contractor", "Builder", "Surveyor",
					"Structural Engineer", "Electrical Subcontractor", "Employer", "Client", "Project Manager",
					"Quantity Surveyor", "Building Surveyor", "Land Surveyor", "Civil Engineer", "Mechanical Engineer",
					"MEP Engineer", "Geotechnical Engineer", "Acoustic Consultant", "Fire Engineer", "Facade Engineer",
					"Sustainability Consultant", "BIM Manager", "CDM Coordinator", "Principal Designer", "Principal Contractor",
					"Subcontractor", "Plumbing Subcontractor", "Mechanical Subcontractor", "Roofing Subcontractor", "Glazing Subcontractor",
					"Site Manager", "Site Foreman", "Clerk of Works", "Health and Safety Officer", "Building Control Officer",
					"Planning Officer", "Interior Designer", "Landscape Architect", "Urban Planner", "Cost Consultant",
					"Construction Manager", "Estimator", "Scheduler", "Crane Operator", 
					"Carpenter", "Bricklayer", "Concrete Worker", "Welder", "Inspector"
					]),
				_ => throw new NotImplementedException(),
			};
		}

		internal string SatisfyingSynonim()
		{
			return _containerType switch
			{
				nameof(Faker.Generic) => _parent.PickRandom([
					$"satisfying", "fulfilling", "meeting", "achieving", "completing"
					]),
				_ => throw new NotImplementedException()
			};
		}

		internal string Sentence()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom([
					"The structural steel delivery has been delayed by two weeks due to supplier issues.",
					"All workers on site must wear hard hats, safety glasses, and high-visibility vests at all times.",
					"The concrete pour for the foundation is scheduled for early Tuesday morning before temperatures rise.",
					"We need to coordinate with the electrical subcontractor before closing up the drywall on the third floor.",
					"The site inspector flagged three minor issues with the rebar spacing that need to be corrected.",
					"Crane operations will be suspended tomorrow if wind speeds exceed thirty miles per hour.",
					"The revised architectural drawings show the load-bearing wall has been moved six inches to the east.",
					"Material costs for lumber have increased by twelve percent since the original project estimate.",
					"The excavation crew hit unexpected bedrock, which will require additional blasting permits.",
					"Final punch list items must be completed before the certificate of occupancy can be issued.",
					"Stage 0 strategic definition confirmed the client's business case justifies proceeding with a new headquarters building.",
					"The Stage 1 preparation and briefing phase established the project budget, site constraints, and sustainability targets.",
					"During Stage 2 concept design, three massing options were presented to the client for review and feedback.",
					"Stage 3 spatial coordination resolved clashes between the structural grid and the mechanical services routing.",
					"The Stage 4 technical design package includes all specifications required for the contractor to price the works.",
					"Planning permission was granted at the end of Stage 3, allowing the team to progress to technical design.",
					"Stage 5 manufacturing and construction began on site following the appointment of the main contractor.",
					"The Stage 6 handover process included client training, snagging inspections, and the issue of as-built drawings.",
					"Stage 7 use feedback identified opportunities to improve thermal performance through minor commissioning adjustments.",
					"Post-occupancy evaluation during Stage 7 confirmed energy consumption was within five percent of the design target.",
					]),
				_ => throw new NotImplementedException(),
			};
		}

		internal string ShouldSynonimExpression()
		{
			return _containerType switch
			{
				nameof(Faker.Generic) => _parent.PickRandom(["should be", "ought to be", "must be", "needs to be", "has to be", "is required to be", "better be"]),
				_ => throw new NotImplementedException()
			};
		}

		internal string RibaStage()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom([
					"Handover", "Post-Handover", "Maintenance", "Demolition", "Design",
					"Strategic Definition", "Preparation and Briefing", "Concept Design",
					"Spatial Coordination", "Technical Design", "Manufacturing and Construction",
					"Handover", "Stage 0", "Stage 1", "Stage 2", "Stage 3", "Stage 4",
					"Stage 5", "Stage 6", "Stage 7",]),
				_ => throw new NotImplementedException()
			};
		}

		// internet
		internal string Url()
		{
			return _containerType switch
			{
				nameof(Faker.Internet) => _parent.PickRandom(["http://www.some.com", "https://example.org", "http://example.net"]),
				_ => throw new NotImplementedException(),
			};
		}

		internal string Word()
		{
			return _containerType switch
			{
				nameof(Faker.Lorem) => _parent.PickRandom(["innovation", "efficiency", "sustainability", "collaboration", "resilience"]),
				_ => throw new NotImplementedException()
			};
		}

		internal ValueConstraint Material()
		{
			return _containerType switch
			{
				nameof(Faker.Construction) => _parent.PickRandom([
					"concrete", "steel", "wood", "glass", "brick",
					"stone", "aluminium", "copper", "plaster", "tile",
					"asphalt", "gypsum", "mortar", "plywood", "drywall",
					"insulation", "rebar", "cement", "sandstone", "marble",
					"reinforced concrete", "structural steel", "cross-laminated timber", "glulam beams", "engineered hardwood",
					"tempered glass", "low-emissivity glazing", "fibre-reinforced polymer", "carbon fibre composite", 
					"stainless steel cladding", "weathering steel", "anodised aluminium", "powder-coated steel", 
					"galvanised steel", "high-performance concrete", "self-compacting concrete", 
					"ultra-high-performance fibre-reinforced concrete", "geopolymer concrete", "autoclaved aerated concrete", 
					"pre-stressed concrete", "ethylene tetrafluoroethylene membrane", "polycarbonate panel", "phenolic resin panel", 
					"terracotta rainscreen", "zinc standing seam", "expanded polystyrene insulation", "mineral wool insulation", 
					"aerogel insulation", "vacuum insulated panel", "phase change material"
					]),
				_ => throw new NotImplementedException()
			};
		}

		internal string RequestSynonym()
		{
			return _containerType switch
			{
				_ => _parent.PickRandom([
					"needs", "requests", "requirements", "demands", 
					"contractual criteria", "stipulations", "conditions",
					"petitions", "prerequisites", "necessities", "expectations",
					"wishes"
					]),
			};
		}

		internal string Subset()
		{
			return _containerType switch
			{
				_ => _parent.PickRandom([
					"any", "every", "each", "the", "a",
					"one", "this", "that", "some", "no",
					"another", "either", "neither"
					]),
			};
		}

		internal string BsddUri(string type)
		{
			return type switch
			{
				"MaterialFacet" => _parent.PickRandom([
					// IFC predefined material concepts (buildingSMART dictionary)
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WAG#Aggregates",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WAS#Asphalt",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WCBR#Brick",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WCN#Concrete",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WEM#Embankment",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WGM#Gypsum",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WGS#Glass",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WMC#MaterialRecycling",
					"https://identifier.buildingsmart.org/uri/rcn/WASTEie/0.1.2/class/WNF#Glass",
					]),
				"IfcClassificationFacet" => _parent.PickRandom([
					// Uniclass (NBS) — UK classification, organization "nbs", dictionary "uniclass2015"
					"https://identifier.buildingsmart.org/uri/nbs/uniclass2015/1/class/Pr_70_65_04",       // Air terminals and diffusers
					"https://identifier.buildingsmart.org/uri/nbs/uniclass2015/1/class/Pr_70_65_04_03",    // Air grilles
					"https://identifier.buildingsmart.org/uri/nbs/uniclass2015/1/class/En_20_80",          // Weapons training ranges
					"https://identifier.buildingsmart.org/uri/nbs/uniclass2015/1/class/Co_70_10_60",       // Onshore wind power generation complexes
					"https://identifier.buildingsmart.org/uri/nbs/uniclass2015/1",                         // Uniclass 2015 dictionary root

					// Molio CCI Construction — Danish classification, organization "molio", dictionary "cciconstruction"
					"https://identifier.buildingsmart.org/uri/molio/cciconstruction/1.0/class/L-A_",       // Assembly system
					"https://identifier.buildingsmart.org/uri/molio/cciconstruction/1.0/class/L-BD",       // Wall structure
					"https://identifier.buildingsmart.org/uri/molio/cciconstruction/1.0/class/L-NAA",      // Pane
					"https://identifier.buildingsmart.org/uri/molio/cciconstruction/1.0",                  // CCI Construction dictionary root
					"https://identifier.buildingsmart.org/uri/molio/cciconstruction/latest",               // Latest version (mutable)

					// ETIM — international product classification, organization "etim", dictionary "etim"
					"https://identifier.buildingsmart.org/uri/etim/etim/8.0/class/EC003162",               // Stucco net
					"https://identifier.buildingsmart.org/uri/etim/etim/8.0/class/EC002987",               // (used in bSDD docs as a HasMaterial example)
					"https://identifier.buildingsmart.org/uri/etim/etim/9.0/prop/EF021146",                // Property example from bSDD-IFC docs
					"https://identifier.buildingsmart.org/uri/etim/etim/8.0",                              // ETIM 8.0 dictionary root
					"https://identifier.buildingsmart.org/uri/etim/etim/9.0",
					]),
				"IfcPropertyFacet" => _parent.PickRandom([
					// Pset_WallCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/AcousticRating",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/FireRating",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/IsExternal",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/LoadBearing",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/ThermalTransmittance",

					// Pset_DoorCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/SecurityRating",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/SmokeStopping",

					// Pset_WindowCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/GlazingAreaFraction",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/Infiltration",

					// Pset_SpaceCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/GrossPlannedArea",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/NetPlannedArea",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/OccupancyNumber",

					// Pset_SlabCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/PitchAngle",

					// Pset_BeamCommon / Pset_ColumnCommon
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/Span",
					"https://identifier.buildingsmart.org/uri/buildingsmart/ifc/4.3/prop/Slope",
					]),
				_ => ""
			};
		}

		internal string ExampleSynonym()
		{
			return _containerType switch
			{
				_ => _parent.PickRandom([
					"example", "instance", "case", "sample", "specimen",
					"illustration", "demonstration", "exemplar", "model", "case in point",
					"occurrence", "representation", "prototype", "template", "precedent",
					"reference", "showcase", "exhibit", "scenario", "use case"
					]),
			};
		}
	}
}
