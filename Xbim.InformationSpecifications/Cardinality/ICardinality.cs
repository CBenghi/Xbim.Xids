﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Xbim.InformationSpecifications
{
    public interface ICardinality
    {
        /// <summary>
        /// Function to export the bS XML content of the instance
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="logger"></param>
        void ExportBuildingSmartIDS(XmlWriter xmlWriter, ILogger? logger);

        /// <summary>
        /// Determines if a requirement is expected for the specication, given the values in the ICardinality instance.
        /// </summary>
        bool ExpectsRequirements { get; }

        /// <summary>
        /// A string describing the nature of the cardinality.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// True if the cardinality requires the entire model for evaluation.
        /// </summary>
        bool IsModelConstraint { get; }
    }
}