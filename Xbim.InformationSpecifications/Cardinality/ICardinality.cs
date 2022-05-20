using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Xbim.InformationSpecifications
{
    public interface ICardinality
    {
        void ExportBuildingSmartIDS(XmlWriter xmlWriter, ILogger? logger);
    }
}
