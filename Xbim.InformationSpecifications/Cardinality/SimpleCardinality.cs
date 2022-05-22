using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Xbim.InformationSpecifications.Cardinality
{
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

    public class SimpleCardinality : ICardinality
    {
        /// <summary>
        /// Evaluates the possible range of entities that relate to the applicability of a specification
        /// Defaults to <see cref="CardinalityEnum.Optional"/>
        /// </summary>
        public CardinalityEnum ApplicabilityCardinality { get; set; } = CardinalityEnum.Optional;

        /// <summary>
        /// If a facetgroup is prohibited, it does not make sens to have requirements assoiated with it.
        /// </summary>
        public bool ExpectsRequirements => ApplicabilityCardinality != CardinalityEnum.Prohibited;

        public string Description => ApplicabilityCardinality.ToString();

        public void ExportBuildingSmartIDS(XmlWriter xmlWriter, ILogger? logger)
        {
            switch (ApplicabilityCardinality)
            {
                case CardinalityEnum.Required:
                    xmlWriter.WriteAttributeString("minOccurs", "1");
                    break;
                case CardinalityEnum.Optional:
                    xmlWriter.WriteAttributeString("minOccurs", "0");
                    break;
                case CardinalityEnum.Prohibited:
                    xmlWriter.WriteAttributeString("maxOccurs", "0");
                    break;
                default:
                    break;
            }
        }
    }
}
