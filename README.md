# Xbim.InformationSpecifications

[![NuGet](https://img.shields.io/nuget/v/Xbim.InformationSpecifications)](https://www.nuget.org/packages/Xbim.InformationSpecifications)
[![Build](https://github.com/CBenghi/Xbim.Xids/actions/workflows/build.yaml/badge.svg)](https://github.com/CBenghi/Xbim.Xids/actions/workflows/build.yaml)

**Xbim.InformationSpecifications** is a .NET library that implements the
[Information Delivery Specification (IDS)](https://github.com/buildingSMART/IDS) standard
defined by buildingSMART International. It provides the core data model and
import/export functionality needed to create, read, and write IDS documents for
IFC-based building-model verification.

An interactive demo of the library's editing capabilities is available at [xbim.it/xids](https://xbim.it/xids).

---

## Status

The library tracks the development of the
[buildingSMART IDS repository](https://github.com/buildingSMART/IDS) and is
updated promptly when the standard changes.

The current release is **v1.1.0-preview**.

---

## Features

- **IDS data model** — full object model for the IDS standard (`Xids`,
  `SpecificationsGroup`, `Specification`, `FacetGroup`, and all IDS facets).
- **buildingSMART IDS import/export** — read and write IDS XML files that
  conform to the buildingSMART schema.
- **Native JSON persistence** — an additional JSON format for compact,
  human-readable storage.
- **All IDS facets are supported**
  - `IfcTypeFacet` — clauses on IFC entity type and predefined type.
  - `AttributeFacet` — clauses on IFC attribute value.
  - `IfcPropertyFacet` — clauses on property set and property value.
  - `IfcClassificationFacet` — clauses on classification system and reference.
  - `MaterialFacet` — clauses on material name.
  - `PartOfFacet` — clauses on spatial/assembly containment relationships.
- **Extra facets are under test development**
  - `DocumentFacet` — clauses on documents and their properties.
  - `IfcRelationFacet` — clauses across multiple selectors (facet groups).
- **Multi-target** — targets `netstandard2.0` and `net8.0`, (compatible with .NET Framework
  4.6.1, .NET Core 2.0, .NET 8.0 and later)
- **Strong-named assembly** — signed with the Xbim open-source key.

> **Note:** Several helper utilities that originated in this library have since been promoted to the upstream [ids-lib](https://github.com/buildingSMART/ids-lib) repository.

---

## Installation

Install the package from NuGet:

```shell
dotnet add package Xbim.InformationSpecifications
```

---

## Quick Start

### Reading an IDS file

```csharp
using Xbim.InformationSpecifications;

// Load an IDS XML file produced by any conformant tool
Xids? ids = Xids.ImportBuildingSmartIDS("path/to/file.ids");

foreach (var spec in ids!.AllSpecifications())
{
    Console.WriteLine(spec.Name);
}
```

### Creating and exporting an IDS

```csharp
using Xbim.InformationSpecifications;

var ids = new Xids();

// Create a specification targeting IFC4
var spec = ids.PrepareSpecification(IfcSchemaVersion.IFC4);
spec.Name = "Walls must have a fire rating";

// Add an applicability facet
var typeFacet = new IfcTypeFacet { IfcType = new ValueConstraint("IFCWALL") };
spec.Applicability!.Facets.Add(typeFacet);

// Add a requirement facet
var propFacet = new IfcPropertyFacet
{
    PropertySetName = new ValueConstraint("Pset_WallCommon"),
    PropertyName    = new ValueConstraint("FireRating"),
};
spec.Requirement!.Facets.Add(propFacet);

// Export to buildingSMART IDS XML
ids.ExportBuildingSmartIDS("output.ids");
```

---

## Project Structure

| Project | Description |
|---|---|
| `Xbim.InformationSpecifications` | Core library — data model, IO, and facet definitions |
| `Xbim.InformationSpecifications.NewTests` | xUnit test suite |

Key namespaces inside the core library:

| Namespace / Folder | Contents |
|---|---|
| `Facets/buildingSMART` | All concrete IDS facet classes |
| `IO` | XML and JSON import/export logic |
| `Collections` | `FacetGroup`, `FacetGroupRepository`, `SpecificationsGroup` |
| `Values` | Value constraint types (`ValueConstraint`, etc.) |
| `Cardinality` | Cardinality / optionality types |

---

## Building from Source

Requirements: **.NET 8 SDK** and **Git**.

```shell
git clone https://github.com/CBenghi/Xbim.Xids.git
cd Xbim.Xids
dotnet build
dotnet test
```

The CI pipeline (GitHub Actions) runs on every push to `main` and `preview` and
publishes a NuGet package automatically when a build on `main` succeeds.

---

## Contributing

Contributions are welcome. Please open an issue or a pull request on
[GitHub](https://github.com/CBenghi/Xbim.Xids). All pull requests trigger the
CI build and test suite automatically.

---

## Contact

- Bug reports and feature requests: [GitHub Issues](https://github.com/CBenghi/Xbim.Xids/issues)
- Private enquiries: [claudio@xbim.it](mailto:claudio@xbim.it)

---

## Licence

This project is licensed under the **CDDL-1.0** licence. See [LICENSE.md](LICENSE.md) for details.
