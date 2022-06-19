using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.InformationSpecifications
{
    // todo: IDSTALK: partof asymmetry
    //
    // why is it that partof can only be in the requirement side of a specification?
    // I can see the need to have a certain property/classification for anything belonging to a system

    // todo: IDSTALK: instuctions is missing only on partof facet
    // 

    /// <summary>
    /// Constrain model parts on the ground of their belonging to a collection defined by the container enum.
    /// </summary>
    public class PartOfFacet : FacetBase, IFacet, IEquatable<PartOfFacet>
    {
        public enum Container
        {
            [CompatibleSchema(new IfcSchemaVersion[] { })]
            Undefined,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcAsset,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcBuildingSystem,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4X3 })]
            IfcBuiltSystem,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3 })]
            IfcCondition,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcDistributionCircuit,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcDistributionSystem,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3 })]
            IfcElectricalCircuit,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcElementAssembly,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcGroup,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcInventory,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralAnalysisModel,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralLoadCase,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralLoadGroup,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcStructuralResultGroup,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcSystem,

            [CompatibleSchema(new[] { IfcSchemaVersion.IFC2X3, IfcSchemaVersion.IFC4, IfcSchemaVersion.IFC4X3 })]
            IfcZone
        } 

        /// <summary>
        /// Constraints the containing entity type to one of <see cref="Container"/> enum.
        /// This is a string value, to get/set the enum values use <see cref="GetEntity"/> and <see cref="SetEntity(Container)"/>.
        /// </summary>
        public string Entity { get; set; } = string.Empty;

        /// <summary>
        /// Filter on the name of the collecting entity.
        /// </summary>
        public ValueConstraint? EntityName { get; set; }

        /// <summary>
        /// Returns the enum value of <see cref="Entity"/>.
        /// </summary>
        /// <returns></returns>
        public Container GetEntity()
        {
            if (Enum.TryParse<Container>(Entity, out var loc))
            {
                return loc;
            }
            return Container.Undefined;
        }
        /// <summary>
        /// Sets the enum value of <see cref="Entity"/>.
        /// </summary>
        /// <param name="value"></param>
        public void SetEntity(Container value)
        {
            Entity = value.ToString();
        }

        /// <inheritdoc />
        public bool Equals(PartOfFacet? other)
        {
            if (other == null)
                return false;
            var thisEqual = (Entity, true).Equals((other.Entity, true));
            if (thisEqual == false)
                return false;
            if (!IFacetExtensions.NullEquals(EntityName, other.EntityName))
                return false;
            return base.Equals(other);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as PartOfFacet);
        }

        /// <summary>
        /// Valid (see <see cref="IFacet.IsValid"/>) if at least <see cref="Entity"/> is meaningful.
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {
            return GetEntity() != Container.Undefined;
        }

        /// <inheritdoc />
        public string Short()
        {
            if (IsValid())
                return $"belongs to {Entity}";
            return "belongs to undefined group";
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"belongs to :'{Entity}'";
        }

        /// <inheritdoc />
        public override int GetHashCode() => 23 + 31 * (Entity, true).GetHashCode() + 31 * base.GetHashCode();

    }
}
