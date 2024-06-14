using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
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
            if (IsBsXml(sourceFile, logger))
                return true;
            return false;
        }

        const int ZipMagic = 0x04034b50;      // Zip magic number:  PK/003/004

        /// <summary>
        /// Determines if the stream is zipped
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool IsZipped(Stream stream, ILogger? logger = null)
        {
            if(!stream.CanSeek) 
            {
                logger?.LogError("Sream cannot seek, IsZipped function cannot be completed.");
                return false;
            }
            try
            {
                stream.Position = 0;
                var bytes = new byte[4];
                stream.Read(bytes, 0, 4);
                var magic = BitConverter.ToInt32(bytes, 0);
                stream.Position = 0;

                return magic == ZipMagic;
            }
            catch (Exception ex) 
            {
                logger?.LogError(ex, "IsZipped function cannot be completed with error: {errorMessage}", ex.Message);
                return false;
            }
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
            if (IsBsXml(sourceFile, logger))
                return Xids.LoadBuildingSmartIDS(sourceFile.FullName, logger);
            return null;
        }

        private static bool IsBsXml(FileInfo sourceFile, ILogger? logger = null)
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

                var doc = XDocument.Load(rdr);
                if(doc?.Root != null && doc.Root.Name.LocalName == "ids")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Cannot load IDS file '{sourceFile.Name}' : {ex.Message}");
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
