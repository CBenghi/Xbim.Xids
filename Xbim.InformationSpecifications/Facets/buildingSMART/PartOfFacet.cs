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
            Undefined,
            IfcElementAssembly,
            IfcGroup,
            IfcSystem,
            IfcBuildingSystem,
            IfcBuiltSystem,
            IfcDistributionSystem,
            IfcZone,
            IfcAsset,
            IfcInventory
        }

        /// <summary>
        /// Constraints the containing entity type to one of <see cref="Container"/> enum.
        /// This is a string value, to get/set the enum values use <see cref="GetEntity"/> and <see cref="SetEntity(Container)"/>.
        /// </summary>
        public string Entity { get; set; } = string.Empty;

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

        public bool Equals(PartOfFacet? other)
        {
            if (other == null)
                return false;
            var thisEqual = (Entity, true).Equals((other.Entity, true));
            if (thisEqual == false)
                return false;
            return base.Equals(other);
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as PartOfFacet);
        }

        public bool IsValid()
        {
            return GetEntity() != Container.Undefined;
        }

        public string Short()
        {
            if (IsValid())
                return $"belongs to {Entity}";
            return "belongs to undefined group";
        }

        public override string ToString()
        {
            return $"belongs to :'{Entity}'";
        }

        public override int GetHashCode() => 23 + 31 * (Entity, true).GetHashCode() + 31 * base.GetHashCode();

    }
}
