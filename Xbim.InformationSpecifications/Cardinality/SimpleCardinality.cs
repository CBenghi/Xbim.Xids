using Microsoft.Extensions.Logging;
using System.Xml;

namespace Xbim.InformationSpecifications.Cardinality
{
	/// <summary>
	/// Supports the definiton of cardinality 
	/// </summary>
	public enum CardinalityEnum
	{
		/// <summary>
		/// At least one entity matches the applicability conditions 
		/// </summary>
		Required,
		/// <summary>
		/// Zero or more entities might match the applicability
		/// </summary>
		Optional,
		/// <summary>
		/// No entity might match the applicability
		/// </summary>
		Prohibited
	}

	/// <summary>
	/// A way of defining <see cref="ICardinality"/> by options in the <see cref="CardinalityEnum"/> enum.
	/// </summary>
	public class SimpleCardinality : ICardinality
	{
		/// <summary>
		/// Evaluates the possible range of entities that relate to the applicability of a specification
		/// Defaults to <see cref="CardinalityEnum.Optional"/>
		/// </summary>
		public CardinalityEnum ApplicabilityCardinality { get; set; } = CardinalityEnum.Optional;

		/// <summary>
		/// Default constructor; <see cref="ApplicabilityCardinality"/> is set to Optional
		/// </summary>
		public SimpleCardinality()
		{
		}

		/// <summary>
		/// Known value constructor
		/// </summary>
		/// <param name="crd">The required cardinality enum</param>
		public SimpleCardinality(CardinalityEnum crd)
		{
			ApplicabilityCardinality = crd;
		}

		/// <inheritdoc />
		public bool ExpectsRequirements => ApplicabilityCardinality == CardinalityEnum.Optional;

		/// <inheritdoc />
		public bool AllowsRequirements => ApplicabilityCardinality != CardinalityEnum.Prohibited;

		/// <inheritdoc />
		public string Description => ApplicabilityCardinality.ToString();

		/// <inheritdoc />
		public bool IsModelConstraint => ApplicabilityCardinality != CardinalityEnum.Optional;

		/// <inheritdoc />
		public bool NoMatchingEntities => ApplicabilityCardinality == CardinalityEnum.Prohibited;


		/// <inheritdoc />
		public void ExportBuildingSmartIDS(XmlWriter xmlWriter, ILogger? logger)
		{
			switch (ApplicabilityCardinality)
			{
				case CardinalityEnum.Required:
					xmlWriter.WriteAttributeString("minOccurs", "1"); // the default. Set for clarity
					xmlWriter.WriteAttributeString("maxOccurs", "unbounded");
					break;
				case CardinalityEnum.Optional:
					xmlWriter.WriteAttributeString("minOccurs", "0");
					xmlWriter.WriteAttributeString("maxOccurs", "unbounded");
					break;
				case CardinalityEnum.Prohibited:
					xmlWriter.WriteAttributeString("minOccurs", "0");
					xmlWriter.WriteAttributeString("maxOccurs", "0");
					break;
				default:
					break;
			}
		}

		/// <inheritdoc />
		public bool IsValid()
		{
			// SimpleCardinality is always valid
			return true;
		}

		/// <inheritdoc />
		public bool IsSatisfiedBy(int count)
		{
			return ApplicabilityCardinality switch
			{
				CardinalityEnum.Optional => true,
				CardinalityEnum.Required => count > 0,
				CardinalityEnum.Prohibited => count < 1,
				_ => false,
			};
		}
	}
}
