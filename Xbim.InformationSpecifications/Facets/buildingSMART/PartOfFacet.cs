using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.InformationSpecifications.Facets.buildingSMART;

namespace Xbim.InformationSpecifications
{
    /// <summary>
    /// Constrain model parts on the ground of their belonging to a collection defined by the container enum.
    /// </summary>
    public class PartOfFacet : FacetBase, IBuilsingSmartCardinality, IFacet, IEquatable<PartOfFacet>
    {
        /// <summary>
        /// The type of relation defining the filtering criteria
        /// </summary>
        public enum PartOfRelation
        {
            /// <summary>
            /// Invalid relation placeholder
            /// </summary>
            Undefined,
            /// <summary>
            /// A relation of type IfcRelAggregates
            /// </summary>
            IfcRelAggregates,
            /// <summary>
            /// A relation of type IfcRelAssignsToGroup or IfcRelAssignsToGroupByFactor
            /// </summary>
            IfcRelAssignsToGroup,
            /// <summary>
            /// A relation of type IfcRelContainedInSpatialStructure
            /// </summary>
            IfcRelContainedInSpatialStructure,
            /// <summary>
            /// A relation of type IfcRelNests
            /// </summary>
            IfcRelNests
        }

        /// <summary>
        /// The type of IFC container
        /// </summary>
        public enum Container
        {
            /// <summary>
            /// Invalid
            /// </summary>
            [CompatibleSchema(new IfcSchemaVersion[] { })]
            Undefined,

            /// <summary>
            /// IfcAsset, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcAsset,

            /// <summary>
            /// IfcBuildingSystem, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcBuildingSystem,

            /// <summary>
            /// IfcBuiltSystem, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4X3 })]
            IfcBuiltSystem,

            /// <summary>
            /// IfcCondition, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3 })]
            IfcCondition,

            /// <summary>
            /// IfcDistributionCircuit, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcDistributionCircuit,

            /// <summary>
            /// IfcDistributionSystem, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcDistributionSystem,

            /// <summary>
            /// IfcElectricalCircuit, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3 })]
            IfcElectricalCircuit,

            /// <summary>
            /// IfcElementAssembly, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcElementAssembly,

            /// <summary>
            /// IfcGroup, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcGroup,

            /// <summary>
            /// IfcInventory, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcInventory,

            /// <summary>
            /// IfcStructuralAnalysisModel, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralAnalysisModel,

            /// <summary>
            /// IfcStructuralLoadCase, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralLoadCase,

            /// <summary>
            /// IfcStructuralLoadGroup, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralLoadGroup,

            /// <summary>
            /// IfcStructuralResultGroup, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralResultGroup,

            /// <summary>
            /// IfcSystem, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcSystem,

            /// <summary>
            /// IfcZone, or any subclass
            /// </summary>
            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcZone
        }

        /// <summary>
        /// Constraints the containing entity type to one of <see cref="Container"/> enum.
        /// This is a string value, to get/set the enum values use <see cref="GetRelation"/> and <see cref="SetRelation(PartOfRelation)"/>.
        /// </summary>
        public string EntityRelation { get; set; } = string.Empty;

        /// <summary>
        /// Filter on the type of the collecting entity.
        /// </summary>
        public IfcTypeFacet? EntityType { get; set; }

        /// <inheritdoc />
        public string RequirementDescription
        {
            get
            {
                return $"a part of a {EntityType?.Short() ?? Any} via {EntityRelation} relationship";
            }
        }

        /// <inheritdoc />
        public string ApplicabilityDescription
        {
            get
            {
                return $"where a part of a {EntityType?.Short() ?? Any} via {EntityRelation} relationship"; ;
            }
        }

        /// <summary>
        /// Returns the enum value of <see cref="EntityRelation"/>.
        /// </summary>
        /// <returns></returns>
        public PartOfRelation GetRelation()
        {
            if (Enum.TryParse<PartOfRelation>(EntityRelation, out var loc))
            {
                return loc;
            }
            return PartOfRelation.Undefined;
        }
        /// <summary>
        /// Sets the enum value of <see cref="EntityRelation"/>.
        /// </summary>
        /// <param name="value"></param>
        public void SetRelation(PartOfRelation value)
        {
            EntityRelation = value.ToString();
        }

        /// <summary>
        /// Replaces any existing <see cref="EntityType"/> with a new ValueConstraint built from the <paramref name="containers"/> enumeration.
        /// </summary>
        /// <param name="containers">Enumeration of accepted container values</param>
        public void SetContainers(IEnumerable<Container> containers)
        {
            var c = new ValueConstraint() { BaseType = NetTypeName.String };
            foreach (var cont in containers)
            {
                c.AddAccepted(new ExactConstraint(cont.ToString()));
            }
            EntityType ??= new IfcTypeFacet();
            EntityType.IfcType = c;

        }

        /// <summary>
        /// Looks at exact constraints in the entityType and converts them, if possible, to an enumeration of <see cref="Container"/>.
        /// </summary>
        /// <returns>Any convertible value, empty enumeration is possible if conversions cannot be carried out.</returns>
        public IEnumerable<Container> GetContainers()
        {
            if (EntityType?.IfcType?.AcceptedValues is null)
                yield break;
            foreach (var value in EntityType.IfcType.AcceptedValues.OfType<ExactConstraint>())
            {
                if (Enum.TryParse<Container>(value.Value, out var loc))
                {
                    yield return loc;
                }
            }
        }


        /// <inheritdoc />
        public bool Equals(PartOfFacet? other)
        {
            if (other == null)
                return false;
            var thisEqual = (EntityRelation, EntityType).Equals((other.EntityRelation, other.EntityType));
            if (thisEqual == false)
                return false;
            return base.Equals(other);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as PartOfFacet);
        }

        /// <summary>
        /// Valid (see <see cref="IFacet.IsValid"/>) if at least <see cref="EntityRelation"/> is meaningful.
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {
            return GetRelation() != PartOfRelation.Undefined
                &&
                (EntityType is null || EntityType.IsValid());
        }

        /// <inheritdoc />
        public string Short()
        {
            if (IsValid())
                return $"belongs to {EntityRelation}";
            return "belongs to undefined group";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"belongs to :'{EntityRelation}'";
        }

        /// <inheritdoc />
        public override int GetHashCode() => 23 + 31 * (EntityRelation, true).GetHashCode() + 31 * base.GetHashCode();

    }
}
