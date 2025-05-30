using IdsLib.IfcSchema;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xbim.InformationSpecifications.Facets.buildingSMART;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Constrain model parts on the ground of properties (of type IfcSingleProperty) associated via PropertySets
	/// Either directly or via a type relation.
	/// </summary>
	public partial class IfcPropertyFacet : FacetBase, IBuilsingSmartCardinality, IFacet, IEquatable<IfcPropertyFacet>
	{

		/// <summary>
		/// Constraint that is applied to the name of the PropertySet.
		/// </summary>
		public ValueConstraint? PropertySetName { get; set; }

		/// <summary>
		/// Constraint that is applied to the name of the Property.
		/// </summary>
		public ValueConstraint? PropertyName { get; set; }

		/// <summary>
		/// Constrained type of the identified property value.
		/// Use the <see cref="HasDataType(out IfcDataTypeInformation?)"/> method to test for the relevant information.
		/// </summary>
		public string? DataType { get; set; }

		/// <summary>
		/// Constraint that is applied to the value of the Property.
		/// </summary>
		public ValueConstraint? PropertyValue { get; set; }

		/// <inheritdoc />
		public string RequirementDescription
		{
			get
			{
				return $"a property {PropertyName?.Short() ?? Any} in the property set {PropertySetName?.Short() ?? Any} with {MeasureLabel}value {PropertyValue?.Short() ?? Any}";
			}
		}

		/// <inheritdoc />
		public string ApplicabilityDescription
		{
			get
			{
				return $"with property {PropertyName?.Short() ?? Any} in the property set {PropertySetName?.Short() ?? Any} with {MeasureLabel}value {PropertyValue?.Short() ?? Any}";
			}
		}

		/// <summary>
		/// Tries parsing the Measure string and returns success state
		/// </summary>
		/// <param name="measure">value of the parsing of the string into the enum</param>
		/// <returns>true if parsing successful</returns>
		public bool HasDataType([NotNullWhen(true)] out IfcDataTypeInformation? measure)
		{
			if (DataType is not null && IdsLib.IfcSchema.SchemaInfo.TryParseIfcDataType(DataType, out var found, false))
			{
				measure = found;
				return true;
			}
			measure = null;
			return false;
		}

		/// <inheritdoc />
		public string Short()
		{
			var sb = new StringBuilder();
			if (!FacetBase.IsNullOrEmpty(PropertyName))
				sb.Append($"Has property '{PropertyName}'");
			else
				sb.Append($"Has any property");
			if (!FacetBase.IsNullOrEmpty(PropertySetName))
				sb.Append($" in property set '{PropertySetName}'");
			if (DataType != null)
				sb.Append($" containing '{DataType}'");
			if (PropertyValue != null)
				sb.Append($" {PropertyValue.Short()}");
			return sb.ToString();
		}
		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return this.Equals(obj as IfcPropertyFacet);
		}
		/// <inheritdoc />
		public override int GetHashCode() => 23 + 31 * (PropertySetName, PropertyName, PropertyValue, DataType).GetHashCode() + 31 * base.GetHashCode();
		/// <inheritdoc />
		public override string ToString() => $"{PropertySetName}-{PropertyName}-{PropertyValue?.ToString() ?? ""}-{DataType}-{base.ToString()}";
		/// <inheritdoc />
		public bool Equals(IfcPropertyFacet? other)
		{
			if (other == null)
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(PropertySetName, other.PropertySetName))
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(PropertyName, other.PropertyName))
				return false;
			if (!IFacetExtensions.NullEquals(PropertyValue, other.PropertyValue))
				return false;
			if (!IFacetExtensions.CaseInsensitiveEquals(DataType, other.DataType))
				return false;
			return base.Equals(other);
		}

		/// <inheritdoc />
		[MemberNotNullWhen(true, nameof(PropertySetName))]
		[MemberNotNullWhen(true, nameof(PropertyName))]
		public bool IsValid()
		{
			return
				FacetBase.IsValidAndNotEmpty(PropertySetName)
				&& FacetBase.IsValidAndNotEmpty(PropertyName)
				&& FacetBase.IsValidOrNull(PropertyValue);
		}

		private string MeasureLabel => !string.IsNullOrWhiteSpace(DataType) ? $"{DataType} " : "";
	}
}
