# Notes

## Object pointers

In the structure of IFC the relations to Properties, Classification and Material are as follows:

### Properties

The `IfcRelDefinesByProperties` type has the following inheritance in both ifc2x3 and ifc4 :
`IfcRelDefines => IfcRelationship => IfcRoot`.

| Field                         | IFC4                            | IFC2x3                   |
| :---------------------------- | :------------------------------ | :----------------------- |
| RelatedObjects                | IfcObjectDefinition             | IfcObject                |
| RelatingPropertyDefinition    | IfcPropertySetDefinitionSelect  | IfcPropertySetDefinition |


### Classification

The `IfcRelAssociatesClassification` type has the following inheritance in both ifc2x3 and ifc4 :
`IfcRelAssociates => IfcRelationship => IfcRoot`.

| Field                         | IFC4                      | IFC2x3                          |
| :---------------------------- | :------------------------ | :------------------------------ |
| RelatedObjects                | IfcDefinitionSelect       | IfcRoot                         |
| RelatingClassification        | IfcClassificationSelect   | IfcClassificationNotationSelect |


### Material

The `IfcRelAssociatesMaterial` type has the following inheritance in both ifc2x3 and ifc4 :
`IfcRelAssociates => IfcRelationship => IfcRoot`.

| Field                         | IFC4                      | IFC2x3                          |
| :---------------------------- | :------------------------ | :------------------------------ |
| RelatedObjects                | IfcDefinitionSelect       | IfcRoot                         |
| RelatingMaterial              | IfcMaterialSelect         | IfcMaterialSelect               |

## Deciding what classes info to export

From the info above the required structure would be:
- Ifc4
  - IfcObjectDefinition
  - IfcDefinitionSelect

- Ifc3
  - IfcObject
  - IfcRoot

But looking at the Properties there should be other consideratios added.
The info to be exported is defined by looking at `PropertyApplicabilityStudy.IncludeTypes`.
