using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Xbim.InformationSpecifications.IO;

namespace Xbim.InformationSpecifications
{
    public partial class Xids
    {
        /// <summary>
        /// Determines if a file can be loaded by the library
        /// </summary>
        /// <param name="sourceFile">The file to load</param>
        /// <param name="logger">The logging context</param>
        /// <returns>true if can be loaded, false otherwise</returns>
        public static bool CanLoad(FileInfo sourceFile, ILogger? logger = null)
        {
            if (!sourceFile.Exists)
                return false;
            // test if string is json
            if (IsXidsJson(sourceFile, logger))
                return true;
            if (IsBsXml(sourceFile))
                return true;
            return false;
        }

        /// <summary>
        /// Loads a xids model from any of the suppoerted files.
        /// </summary>
        /// <returns>Null if not successful.</returns>
        public static Xids? Load(FileInfo sourceFile, ILogger? logger = null)
        {
            if (!sourceFile.Exists)
                return null;
            if (IsXidsJson(sourceFile, logger))
                return Xids.LoadFromJson(sourceFile.FullName, logger);
            if (IsBsXml(sourceFile))
                return Xids.ImportBuildingSmartIDS(sourceFile.FullName, logger);
            return null;
        }

        private static bool IsBsXml(FileInfo sourceFile)
        {
            try
            {
                var settings = new XmlReaderSettings
                {
                    CheckCharacters = false, //optimisation - best avoided.
                    DtdProcessing = DtdProcessing.Ignore
                };
                using var str = sourceFile.OpenRead();
                using var rdr = XmlReader.Create(str, settings);
                while (rdr.Read())
                {
                    if (rdr.NodeType == XmlNodeType.Element)
                        return rdr.LocalName == "ids";
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private static bool IsXidsJson(FileInfo sourceFile, ILogger? logger)
        {
            try
            {
                using var s = File.OpenRead(sourceFile.FullName);
                var fnd = JsonParseHelp.QuickFindValue(s, "ContentType");
                if (!"XIDS".Equals(fnd))
                    return false;

                s.Position = 0;
                var vers = JsonParseHelp.QuickFindValue(s, "Version");
                if (!Xids.AssemblyVersion.Equals(vers))
                    logger?.LogWarning("Attempting to load from a different program version.");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
