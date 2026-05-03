using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
	/// <summary>
	/// Constrain model parts on the ground of their class and predefined type.
	/// </summary>
	public partial class IfcTypeFacet : FacetBase, IEquatable<IfcTypeFacet>, IFacet, IFacetCleanup
	{
		/// <summary>
		/// Required 
		/// </summary>
		public ValueConstraint? IfcType { get; set; }

		/// <summary>
		/// Optional
		/// </summary>
		public ValueConstraint? PredefinedType { get; set; }

		/// <summary>
		/// Not part of buildingSmart specification
		/// </summary>
		public bool IncludeSubtypes { get; set; } = true;

		/// <inheritdoc/>
		public string ApplicabilityDescription
		{
			get
			{
				var predefined = IsNullOrEmpty(PredefinedType) ? Any : PrettifyPredefinedType(PredefinedType.Short());
				var ifcTypes = IsNullOrEmpty(IfcType) ? Any : PrettifyIfcType(IfcType.Short());
				if (!IsNullOrEmpty(IfcType) && IncludeSubtypes)
				{
					if (IfcType.AcceptedValues is not null)
					{
						if (IfcType.AcceptedValues.Count == 1)
							ifcTypes += " or any of its subtypes";
						else
							ifcTypes += " or any of their subtypes";
					}
				}
				return $"of entity {ifcTypes} and of predefined type {predefined}";
			}
		}

		/// <inheritdoc />
		public string RequirementDescription
		{
			get
			{
				var predefined = IsNullOrEmpty(PredefinedType) ? Any : PrettifyPredefinedType(PredefinedType.Short());
				var ifcTypes = IsNullOrEmpty(IfcType) ? Any : PrettifyIfcType(IfcType.Short());
				return $"an entity {ifcTypes} and of predefined type {predefined}";
			}
		}

		private static readonly Regex reReplaceIFC = new Regex(@"\bIFC", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static string PrettifyIfcType(string ifcText)
		{
			var words = ifcText.Split(' ');
			var sb = new StringBuilder();
			foreach (var word in words)
			{
				string cleaned = reReplaceIFC.Replace(word, "");
				if (cleaned != word)
				{
					sb.Append(cleaned.ToLowerInvariant().FirstCharToUpper());
				}
				else
				{
					sb.Append(word);
				}
				sb.Append(' ');
			}
			return sb.ToString().TrimEnd();
		}

		private static string PrettifyPredefinedType(string text)
		{
			if (string.IsNullOrWhiteSpace(text)) return "";
			return text.ToLowerInvariant().FirstCharToUpper();
		}

		/// <inheritdoc />
		public string Short()
		{
			var desc = new List<string>();
			if (!FacetBase.IsNullOrEmpty(IfcType))
			{
				var tmpT = $"is of type {IfcType}";
				if (IncludeSubtypes)
					tmpT += " or one of its subtypes";
				desc.Add(tmpT);
			}
			if (!FacetBase.IsNullOrEmpty(PredefinedType))
			{
				desc.Add($"has a predefined type value of '{PredefinedType}'");
			}
			var tmp = string.Join(" and ", [.. desc]) + ".";
			return tmp.FirstCharToUpper();
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			return this.Equals(obj as IfcTypeFacet);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{IfcType}-{PredefinedType}-{IncludeSubtypes}-{base.ToString()}";
		}

		/// <inheritdoc />
		public override int GetHashCode() => 23 + 31 * (IfcType, PredefinedType, IncludeSubtypes).GetHashCode() + 31 * base.GetHashCode();

		/// <inheritdoc />
		public bool Equals(IfcTypeFacet? other)
		{
			if (other == null)
				return false;
			var thisEq = (IfcType, PredefinedType, IncludeSubtypes)
				.Equals((other.IfcType, other.PredefinedType, other.IncludeSubtypes));
			if (!thisEq)
				return false;
			return base.Equals(other);
		}

		/// <summary>
		/// Valid (see <see cref="IFacet.IsValid"/>) if at least IfcType is meaningful.
		/// </summary>
		/// <returns>true if valid</returns>
		[MemberNotNullWhen(true, nameof(IfcType))]
		public bool IsValid()
		{
			return FacetBase.IsValidAndNotEmpty(IfcType)
				&& FacetBase.IsValidOrNull(PredefinedType);
		}

		/// <summary>
		/// Tries to reconduct IFC class names to CamelCase
		/// </summary>
		public void Cleanup()
		{
			if (IfcType is not null && IfcType.HasAnyAcceptedValue())
			{
				int iCanRemoveSubtypes = 0;
				foreach (var item in IfcType.AcceptedValues.OfType<ExactConstraint>())
				{
					if (IfcSchemaHelper.TryGetExactClassInfo(item.Value, out var className, out var hasSubtypes))
					{
						item.Value = className;
						if (!hasSubtypes)
							iCanRemoveSubtypes++;
					}
				}
				// we must check if all the accepted values are of a single class
				if (IncludeSubtypes && iCanRemoveSubtypes == IfcType.AcceptedValues.Count())
				{
					IncludeSubtypes = false;
				}
			}
		}
	}
}
