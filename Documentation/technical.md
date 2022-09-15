# Relations

Various versions of the IfcSchema allow several relationships; 

| Relation type                               | partOf  | Related attribute name         | Relating attribute name      | IFC Schemas and notes      |
|---------------------------------------------|---------|--------------------------------|------------------------------|----------------------------|
| IfcRelAssigns  (abstract)                   |         |                                |                              |                            |
|   IfcRelAssignsToActor                      |         | RelatedObjects                 | RelatingActor                | 2x3, 4, 4x3                |
|   IfcRelAssignsToControl                    |         | RelatedObjects                 | RelatingControl              | 2x3, 4, 4x3                |
|   IfcRelAssignsToGroup                      |   ✔    | RelatedObjects                 | RelatingGroup (IFCGROUP)     | 2x3, 4, 4x3                |
|     IfcRelAssignsToGroupByFactor            |         | RelatedObjects                 | RelatingGroup                | 2x3, 4, 4x3                |
|   IfcRelAssignsToProcess                    |         | RelatedObjects                 | RelatingProcess              | 2x3, 4, 4x3                |
|   IfcRelAssignsToProduct                    |         | RelatedObjects                 | RelatingProduct              | 2x3, 4, 4x3                |
|   IfcRelAssignsToResource                   |         | RelatedObjects                 | RelatingResource             | 2x3, 4, 4x3                |
| IfcRelAssociates  (abstract)                |         | RelatedObjects                 |                              |                            |
|   IfcRelAssociatesApproval                  |         | RelatedObjects                 | RelatingApproval             | 2x3, 4, 4x3                |
|   IfcRelAssociatesClassification            |         | RelatedObjects                 | RelatingClassification       | 2x3, 4, 4x3; use classification facet instead |
|   IfcRelAssociatesConstraint                |         | RelatedObjects                 | RelatingConstraint           | 2x3, 4, 4x3                |
|   IfcRelAssociatesDocument                  |         | RelatedObjects                 | RelatingDocument             | 2x3, 4, 4x3                |
|   IfcRelAssociatesLibrary                   |         | RelatedObjects                 | RelatingLibrary              | 2x3, 4, 4x3                |
|   IfcRelAssociatesMaterial                  |         | RelatedObjects                 | RelatingMaterial             | 2x3, 4, 4x3; use material facet instead |
| 	IfcRelAssociatesProfileDef                |         | RelatedObjects                 | RelatingProfileDef           | 4x3                        |
| IfcRelConnects  (abstract)                  |         |                                |                              |                            |
|   IfcRelConnectsElements                    |         | RelatedElement                 | RelatingElement              | 2x3, 4, 4x3                |
|     IfcRelConnectsPathElements              |         | RelatedElement                 | RelatingElement              | 2x3, 4, 4x3                |
|     IfcRelConnectsWithRealizingElements     |         | RelatedElement                 | RelatingElement              | 2x3, 4, 4x3                |
|   IfcRelConnectsPorts                       |         | RelatedPort                    | RelatingPort                 | 2x3, 4, 4x3                |
|   IfcRelConnectsPortToElement               |         | RelatedElement                 | RelatingPort                 | 2x3, 4, 4x3                |
|   IfcRelConnectsStructuralActivity          |         | RelatedStructuralActivity      | RelatingElement              | 2x3, 4, 4x3                |
|   IfcRelConnectsStructuralMember            |         | RelatedStructuralConnection    | RelatingStructuralMember     | 2x3, 4, 4x3                |
|     IfcRelConnectsWithEccentricity          |         | RelatedStructuralConnection    | RelatingStructuralMember     | 2x3, 4, 4x3                |
|   IfcRelContainedInSpatialStructure         |   ✔    | RelatedElements                | RelatingStructure (IFCSPATIALELEMENT) | 2x3, 4, 4x3                |
|   IfcRelCoversBldgElements                  |         | RelatedCoverings               | RelatingBuildingElement      | 2x3, 4, 4x3                |
|   IfcRelCoversSpaces                        |         | RelatedCoverings               | RelatingSpace                | 2x3, 4, 4x3; RelatingSpace was called RelatedSpace in 2x3; this relationship is deprecated in Ifc4 and Ifc4x3 |
|   IfcRelFillsElement                        |         | RelatedBuildingElement         | RelatingOpeningElement       | 2x3, 4, 4x3                |
|   IfcRelFlowControlElements                 |         | RelatedControlElements         | RelatingFlowElement          | 2x3, 4, 4x3                |
|   IfcRelInterferesElements                  |         | RelatedElement                 | RelatingElement              | 4, 4x3                     |
|   IfcRelReferencedInSpatialStructure        |         | RelatedElements                | RelatingStructure            | 2x3, 4, 4x3                |
|   IfcRelSequence                            |         | RelatedProcess                 | RelatingProcess              | 2x3, 4, 4x3                |
|   IfcRelServicesBuildings                   |         | RelatedBuildings               | RelatingSystem               | 2x3, 4, 4x3; Relationship deprecated in Ifc4x3 |
|   IfcRelSpaceBoundary                       |         | RelatedBuildingElement         | RelatingSpace                | 2x3, 4, 4x3                |
|     IfcRelSpaceBoundary1stLevel             |         | RelatedBuildingElement         | RelatingSpace                | 2x3, 4, 4x3                |
|       IfcRelSpaceBoundary2ndLevel           |         | RelatedBuildingElement         | RelatingSpace                | 2x3, 4, 4x3                |
| IfcRelDeclares                              |         | RelatedDefinitions             | RelatingContext              | 4, 4x3                     |
| IfcRelDecomposes  (abstract)                |         |                                |                              |                            |
|   IfcRelAggregates                          |    ✔   | RelatedObjects (OBJECTDEFINITION)| RelatingObject (OBJECTDEFINITION) | 2x3, 4, 4x3                |
|   IfcRelNests                               |    ✔   | RelatedObjects (OBJECTDEFINITION)| RelatingObject (OBJECTDEFINITION) | 2x3, 4, 4x3                |
|   IfcRelProjectsElement                     |         | RelatedFeatureElement          | RelatingElement              | 2x3, 4, 4x3                |
|   IfcRelVoidsElement                        |         | RelatedOpeningElement          | RelatingBuildingElement      | 2x3, 4, 4x3                |
|   IfcRelAdheresToElement                    |         | RelatedSurfaceFeatures         | RelatingElement              | 4x3                        |
| IfcRelDefines  (abstract)                   |         |                                |                              |                            |
|   IfcRelDefinesByObject                     |         | RelatedObjects                 | RelatingObject               | 2x3, 4, 4x3                |
|   IfcRelDefinesByProperties                 |         | RelatedObjects                 | RelatingPropertyDefinition   | 2x3, 4, 4x3; use property facet instead |
|   IfcRelDefinesByTemplate                   |         | RelatedPropertySets            | RelatingTemplate             | 4, 4x3                     |
|   IfcRelDefinesByType                       |         | RelatedObjects                 | RelatingType                 | 2x3, 4, 4x3                |
