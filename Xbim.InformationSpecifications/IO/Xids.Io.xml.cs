using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xbim.InformationSpecifications.Cardinality;
using Xbim.InformationSpecifications.Facets.buildingSMART;
using Xbim.InformationSpecifications.Helpers;

namespace Xbim.InformationSpecifications
{
    public partial class Xids
    {
        private static XmlWriterSettings WriteSettings
        {
            get
            {
                var settings = new XmlWriterSettings();
                settings.Async = false;
                if(PrettyOutput)
                {
                    settings.Indent = true;
                    settings.IndentChars = "\t";
                }

                return settings;
            }
        }

        /// <summary>
        /// Determines if the XML output should be indented for readability
        /// </summary>
        public static bool PrettyOutput { get; set; } = true;

        /// <summary>
        /// When exporting to bS IDS, export files can be one of the formats in this enum.
        /// </summary>
        public enum ExportedFormat
        {
            /// <summary>
            /// A single IDS in XML format
            /// </summary>
            XML,
            /// <summary>
            /// multiple IDS files in a compressed ZIP format
            /// </summary>
            ZIP
        }

        /// <summary>
        /// Exports the entire XIDS to a buildingSmart file, depending on the number of groups exports an XML or a ZIP file.
        /// </summary>
        /// <param name="destinationFileName">the path of a writeable location on disk</param>
        /// <param name="logger">the logging context</param>
        /// <returns>An enum determining if XML or ZIP files were written</returns>
        public ExportedFormat ExportBuildingSmartIDS(string destinationFileName, ILogger? logger = null)
        {
            using FileStream fs = File.OpenWrite(destinationFileName);
            return ExportBuildingSmartIDS(fs, logger);
        }

        /// <summary>
        /// Exports the entire XIDS to a buildingSmart file, depending on the number of groups exports an XML or a ZIP file.
        /// </summary>
        /// <param name="destinationStream">a writeable stream</param>
        /// <param name="logger">the logging context</param>
        /// <returns>An enum determining if XML or ZIP files were written</returns>
        public ExportedFormat ExportBuildingSmartIDS(Stream destinationStream, ILogger? logger = null)
        {
            if (SpecificationsGroups.Count == 1)
            {
                using XmlWriter writer = XmlWriter.Create(destinationStream, WriteSettings);
                ExportBuildingSmartIDS(SpecificationsGroups.First(), writer, logger);
                return ExportedFormat.XML;
            }

            using var zipArchive = new ZipArchive(destinationStream, ZipArchiveMode.Create, true);
            int i = 0;
            foreach (var specGroup in SpecificationsGroups)
            {

                var name = (string.IsNullOrEmpty(specGroup.Name))
                    ? $"{++i}.ids"
                    : $"{++i} - {specGroup.Name!.MakeSafeFileName()}.ids";
                var file = zipArchive.CreateEntry(name);
                using var str = file.Open();
                using XmlWriter writer = XmlWriter.Create(str, WriteSettings);
                ExportBuildingSmartIDS(specGroup, writer, logger);

            }
            return ExportedFormat.ZIP;
        }



        private static void ExportBuildingSmartIDS(SpecificationsGroup specGroup, XmlWriter xmlWriter, ILogger? logger)
        {
            xmlWriter.WriteStartElement("ids", "ids", @"http://standards.buildingsmart.org/IDS");
            // writer.WriteAttributeString("xsi", "xmlns", @"http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids.xsd");

            // info goes first
            WriteInfo(specGroup, xmlWriter);

            // then the specifications
            xmlWriter.WriteStartElement("specifications", IdsNamespace);
            foreach (var spec in specGroup.Specifications)
            {
                ExportBuildingSmartIDS(spec, xmlWriter, logger);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        private static void WriteInfo(SpecificationsGroup specGroup, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("info", IdsNamespace);
            // title needs to be written in any case
            xmlWriter.WriteElementString("title", IdsNamespace, specGroup.Name);
            // copy
            if (!string.IsNullOrEmpty(specGroup.Copyright))
                xmlWriter.WriteElementString("copyright", IdsNamespace, specGroup.Copyright);
            // version
            if (!string.IsNullOrEmpty(specGroup.Version))
                xmlWriter.WriteElementString("version", IdsNamespace, specGroup.Version);
            // description
            if (!string.IsNullOrEmpty(specGroup.Description))
                xmlWriter.WriteElementString("description", IdsNamespace, specGroup.Description);
            // author
            if (!string.IsNullOrEmpty(specGroup.Author))
                xmlWriter.WriteElementString("author", IdsNamespace, specGroup.Author);
            // date
            if (specGroup.Date.HasValue)
                xmlWriter.WriteElementString("date", IdsNamespace, $"{specGroup.Date.Value:yyyy-MM-dd}");
            // purpose
            if (!string.IsNullOrEmpty(specGroup.Purpose))
                xmlWriter.WriteElementString("purpose", IdsNamespace, specGroup.Purpose);
            // milestone
            if (!string.IsNullOrEmpty(specGroup.Milestone))
                xmlWriter.WriteElementString("milestone", IdsNamespace, specGroup.Milestone);

            xmlWriter.WriteEndElement();
        }

        private const string IdsNamespace = @"http://standards.buildingsmart.org/IDS";
        private const string IdsPrefix = "";

        private static void ExportBuildingSmartIDS(Specification spec, XmlWriter xmlWriter, ILogger? logger)
        {
            xmlWriter.WriteStartElement("specification", IdsNamespace);
            if (spec.IfcVersion != null)
                xmlWriter.WriteAttributeString("ifcVersion", string.Join(" ", spec.IfcVersion));
            else
            {
                var allVersions = new[] {
                    IfcSchemaVersion.IFC2X3,
                    IfcSchemaVersion.IFC4,
                    IfcSchemaVersion.IFC4X3,
                };
                xmlWriter.WriteAttributeString("ifcVersion", string.Join(" ", allVersions)); // required for bS schema
                                                                                             // if (requirement.Name != null)
            }
            xmlWriter.WriteAttributeString("name", spec.Name ?? ""); // required
            if (spec.Description != null)
                xmlWriter.WriteAttributeString("description", spec.Description);
            // instructions
            if (spec.Instructions != null)
                xmlWriter.WriteAttributeString("instructions", spec.Instructions);

            if (spec.Cardinality is null)
                logger?.LogError("Cardinality is required for specification '{specname}' ({guid}).", spec.Name, spec.Guid);
            else
                spec.Cardinality.ExportBuildingSmartIDS(xmlWriter, logger);

            // applicability
            xmlWriter.WriteStartElement("applicability", IdsNamespace);
            if (spec.Applicability is not null)
            {
                foreach (var item in spec.Applicability.Facets)
                {
                    Xids.ExportBuildingSmartIDS(item, xmlWriter, false, logger, spec.Applicability, null);
                }
            }
            xmlWriter.WriteEndElement();

            // requirements
            xmlWriter.WriteStartElement("requirements", IdsNamespace);
            if (spec.Requirement is not null)
            {
                var opts = spec.Requirement.RequirementOptions;
                for (int i = 0; i < spec.Requirement.Facets.Count; i++)
                {
                    var option = GetProgressive(opts, i, RequirementCardinalityOptions.Expected);
                    IFacet? facet = spec.Requirement.Facets[i];
                    Xids.ExportBuildingSmartIDS(facet, xmlWriter, true, logger, spec.Requirement, option);
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        static private RequirementCardinalityOptions GetProgressive(ObservableCollection<RequirementCardinalityOptions>? opts, int i, RequirementCardinalityOptions defaultValue)
        {
            if (opts is null)
                return defaultValue;
            if (i >= opts.Count)
                return defaultValue;
            return opts[i];
        }

        private static void ExportBuildingSmartIDS(IFacet facet, XmlWriter xmlWriter, bool forRequirement, ILogger? logger, FacetGroup context, RequirementCardinalityOptions? requirementOption = null)
        {
            switch (facet)
            {
                case IfcTypeFacet tf:
                    xmlWriter.WriteStartElement("entity", IdsNamespace);
                    WriteFacetBaseAttributes(tf, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(tf.IfcType, xmlWriter, "name", logger);
                    WriteConstraintValue(tf.PredefinedType, xmlWriter, "predefinedType", logger);
                    xmlWriter.WriteEndElement();
                    break;
                case IfcClassificationFacet cf:
                    xmlWriter.WriteStartElement("classification", IdsNamespace);
                    WriteFacetBaseAttributes(cf, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(cf.Identification, xmlWriter, "value", logger);
                    WriteConstraintValue(cf.ClassificationSystem, xmlWriter, "system", logger);
                    WriteFacetBaseElements(cf, xmlWriter); // from classifcation
                    xmlWriter.WriteEndElement();
                    break;
                case IfcPropertyFacet pf:
                    xmlWriter.WriteStartElement("property", IdsNamespace);
                    WriteFacetBaseAttributes(pf, xmlWriter, logger, forRequirement, requirementOption);
                    if (!string.IsNullOrWhiteSpace(pf.DataType))
                        xmlWriter.WriteAttributeString("datatype", pf.DataType.ToUpperInvariant());
                    else
                        xmlWriter.WriteAttributeString("datatype", "");
                    WriteConstraintValue(pf.PropertySetName, xmlWriter, "propertySet", logger);
                    WriteConstraintValue(pf.PropertyName, xmlWriter, "name", logger);
                    WriteConstraintValue(pf.PropertyValue, xmlWriter, "value", logger);
                    WriteFacetBaseElements(pf, xmlWriter); // from Property
                    xmlWriter.WriteEndElement();
                    break;
                case MaterialFacet mf:
                    xmlWriter.WriteStartElement("material", IdsNamespace);
                    WriteFacetBaseAttributes(mf, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(mf.Value, xmlWriter, "value", logger);
                    WriteFacetBaseElements(mf, xmlWriter); // from material
                    xmlWriter.WriteEndElement();
                    break;
                case AttributeFacet af:
                    xmlWriter.WriteStartElement("attribute", IdsNamespace);
                    WriteFacetBaseAttributes(af, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(af.AttributeName, xmlWriter, "name", logger);
                    WriteConstraintValue(af.AttributeValue, xmlWriter, "value", logger);
                    xmlWriter.WriteEndElement();
                    break;
                case PartOfFacet pof:
                    xmlWriter.WriteStartElement("partOf", IdsNamespace);
                    WriteFacetBaseAttributes(pof, xmlWriter, logger, forRequirement, requirementOption);
                    xmlWriter.WriteAttributeString("relation", pof.EntityRelation.ToString());
                    if (pof.EntityType is not null)
                    {
                        // todo: review the forRequirement parameter here
                        ExportBuildingSmartIDS(pof.EntityType, xmlWriter, false, logger, context, null);
                    }
                    xmlWriter.WriteEndElement();                    
                    break;
                default:
                    logger?.LogWarning("TODO: ExportBuildingSmartIDS missing case for {type}.", facet.GetType());
                    break;
            }
        }
        static private void LogDataLoss(ILogger? logger, FacetGroup context, IFacet facet, string propertyName, bool forRequirement)
        {
            logger?.LogError("Loss of data exporting group {grp}: property {prop} not available in {tp} for {ctx}.", context.Guid, propertyName, facet.GetType().Name, forRequirement ? "requirement" : "applicability");
        }

        static private void WriteSimpleValue(XmlWriter xmlWriter, string stringValue)
        {
            xmlWriter.WriteStartElement("simpleValue", IdsNamespace);
            xmlWriter.WriteString(stringValue);
            xmlWriter.WriteEndElement();
        }

        static private void WriteConstraintValue(ValueConstraint? value, XmlWriter xmlWriter, string name, ILogger? logger)
        {
            if (value == null)
                return;
            xmlWriter.WriteStartElement(name, IdsNamespace);
            if (value.IsSingleUndefinedExact(out string? exact))
            {
                if (exact is null)
                {
                    logger?.LogError("Invalid null constraint found, added comment in exported file.");
                    xmlWriter.WriteComment("Invalid null constraint found at this position"); // not sure this might even ever happen
                }
                else
                    WriteSimpleValue(xmlWriter, exact);

            }
            else if (value.AcceptedValues != null)
            {
                xmlWriter.WriteStartElement("restriction", @"http://www.w3.org/2001/XMLSchema");
                if (value.BaseType != NetTypeName.Undefined)
                {
                    var val = ValueConstraint.GetXsdTypeString(value.BaseType);
                    xmlWriter.WriteAttributeString("base", val);
                }
                foreach (var item in value.AcceptedValues)
                {
                    if (item is PatternConstraint pc)
                    {
                        xmlWriter.WriteStartElement("pattern", @"http://www.w3.org/2001/XMLSchema");
                        xmlWriter.WriteAttributeString("value", pc.Pattern);
                        xmlWriter.WriteEndElement();
                    }
                    else if (item is ExactConstraint ec)
                    {
                        xmlWriter.WriteStartElement("enumeration", @"http://www.w3.org/2001/XMLSchema");
                        xmlWriter.WriteAttributeString("value", ec.Value.ToString());
                        xmlWriter.WriteEndElement();
                    }
                    else if (item is RangeConstraint rc)
                    {
                        if (rc.MinValue != null)
                        {
                            var tp = rc.MinInclusive ? "minInclusive" : "minExclusive";
                            xmlWriter.WriteStartElement(tp, @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", rc.MinValue.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (rc.MaxValue != null)
                        {
                            var tp = rc.MinInclusive ? "maxInclusive" : "maxExclusive";
                            xmlWriter.WriteStartElement(tp, @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", rc.MaxValue.ToString());
                            xmlWriter.WriteEndElement();
                        }
                    }
                    else if (item is StructureConstraint sc)
                    {
                        if (sc.Length != null)
                        {
                            xmlWriter.WriteStartElement("length", @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", sc.Length.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (sc.MinLength != null)
                        {
                            xmlWriter.WriteStartElement("minLength", @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", sc.MinLength.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (sc.MaxLength != null)
                        {
                            xmlWriter.WriteStartElement("maxLength", @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", sc.MaxLength.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (sc.TotalDigits != null)
                        {
                            xmlWriter.WriteStartElement("totalDigits", @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", sc.TotalDigits.ToString());
                            xmlWriter.WriteEndElement();
                        }
                        if (sc.FractionDigits != null)
                        {
                            xmlWriter.WriteStartElement("fractionDigits", @"http://www.w3.org/2001/XMLSchema");
                            xmlWriter.WriteAttributeString("value", sc.FractionDigits.ToString());
                            xmlWriter.WriteEndElement();
                        }
                    }
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        static private void WriteFacetBaseAttributes(FacetBase cf, XmlWriter xmlWriter, ILogger? logger, bool forRequirement, RequirementCardinalityOptions? option)
        {
            if (forRequirement)
            {
                if (!option.HasValue)
                    option = RequirementCardinalityOptions.Expected; // should be redundant, but makes some be not null

                if (
                    cf is IBuilsingSmartCardinality 
                )
                {
                    // use is required
                    switch (option.Value)
                    {
                        case RequirementCardinalityOptions.Prohibited:
                            xmlWriter.WriteAttributeString("minOccurs", "0");
                            xmlWriter.WriteAttributeString("maxOccurs", "0");
                            break;
                        case RequirementCardinalityOptions.Expected:
                            // xmlWriter.WriteAttributeString("minOccurs", "1"); 1 is the default, anyway
                            xmlWriter.WriteAttributeString("maxOccurs", "unbounded");
                            break;
                        case RequirementCardinalityOptions.Optional:
                            xmlWriter.WriteAttributeString("minOccurs", "0");
                            xmlWriter.WriteAttributeString("maxOccurs", "unbounded");
                            break;
                        default:
                            logger?.LogError("Invalid RequirementOption persistence for '{option}'", option);
                            break;
                    }
                }
                // instruction is optional
                if (!string.IsNullOrWhiteSpace(cf.Instructions))
                    xmlWriter.WriteAttributeString("instructions", cf.Instructions);
                
            }

            if (
                cf is IfcPropertyFacet ||
                cf is IfcClassificationFacet ||
                cf is MaterialFacet
                )
            {
                if (!string.IsNullOrWhiteSpace(cf.Uri))
                    xmlWriter.WriteAttributeString("uri", cf.Uri);
            }


        }

#pragma warning disable IDE0060 // Remove unused parameter
        static private void WriteFacetBaseElements(FacetBase cf, XmlWriter xmlWriter)
        {
            // function is kept in case it's gonna be useful again for structure purposes
        }
#pragma warning restore IDE0060 // Remove unused parameter

        /// <summary>
        /// Should use <see cref="LoadBuildingSmartIDS(Stream, ILogger?)"/> instead.
        /// </summary>
        [Obsolete("Use LoadBuildingSmartIDS instead.")]
        public static Xids? ImportBuildingSmartIDS(Stream stream, ILogger? logger = null)
        { 
            return LoadBuildingSmartIDS(stream, logger);
        }
    

        /// <summary>
        /// Attempts to load an XIDS from a stream, where the stream is either an XML IDS or a zip file containing multiple IDS XML files
        /// </summary>
        /// <param name="stream">The XML or ZIP source stream to parse.</param>
        /// <param name="logger">The logger to send any errors and warnings to.</param>
        /// <returns>an XIDS or null if it could not be read.</returns>
        public static Xids? LoadBuildingSmartIDS(Stream stream, ILogger? logger = null)
        {
            if (IsZipped(stream))
            {
                using(var zip = new ZipArchive(stream, ZipArchiveMode.Read, false))
                {
                    var xids = new Xids();
                    foreach(var entry in zip.Entries)
                    {
                        try
                        { 
                            if(entry.Name.EndsWith(".ids", StringComparison.InvariantCultureIgnoreCase))
                            {
                                using(var idsStream = entry.Open())
                                {
                                    var element = XElement.Load(idsStream);
                                    LoadBuildingSmartIDS(element, logger, xids);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            logger?.LogError(ex, "Failed to load IDS file from zip stream");
                        }
                    }
                    if(!xids.AllSpecifications().Any())
                    {
                        logger?.LogWarning("No specifications found in this zip file. Ensure the zip contains *.ids files");
                    }
                    return xids;
                }
            }
            else
            {
                var t = XElement.Load(stream);
                return LoadBuildingSmartIDS(t, logger);
            }
        }

        /// <summary>
        /// Should use <see cref="LoadBuildingSmartIDS(string, ILogger?)"/> instead.
        /// </summary>
        [Obsolete("Use LoadBuildingSmartIDS instead.")]
        public static Xids? ImportBuildingSmartIDS(string fileName, ILogger? logger = null)
        {
            return LoadBuildingSmartIDS(fileName, logger);
        }

        /// <summary>
        /// Attempts to unpersist an XIDS from the provider IDS XML file or zip file containing IDS files.
        /// </summary>
        /// <param name="fileName">File name of the Xids to load</param>
        /// <param name="logger">The logger to send any errors and warnings to.</param>
        /// <returns>an XIDS or null if it could not be read.</returns>
        public static Xids? LoadBuildingSmartIDS(string fileName, ILogger? logger = null)
        {
            if (!File.Exists(fileName))
            {
                var d = new DirectoryInfo(".");
                logger?.LogError("File '{fileName}' not found from executing directory '{fullDirectoryName}'", fileName, d.FullName);
                return null;
            }
            if(fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                using var stream  = File.OpenRead(fileName);
                if(IsZipped(stream))
                {
                    return LoadBuildingSmartIDS(stream, logger);
                }
                else
                {
                    logger?.LogError("Not a valid zip file");
                    return null;
                }
            }
            else
            {
                var main = XElement.Parse(File.ReadAllText(fileName));
                return LoadBuildingSmartIDS(main, logger);
            }
        }

        /// <summary>
        /// Should use <see cref="LoadBuildingSmartIDS(XElement, ILogger?, Xids?)"/> instead.
        /// </summary>
        [Obsolete("Use LoadBuildingSmartIDS instead.")]
        public static Xids? ImportBuildingSmartIDS(XElement main, ILogger? logger = null)
        {
            return LoadBuildingSmartIDS(main, logger);
        }

        /// <summary>
        /// Attempts to unpersist an XIDS from an XML element.
        /// </summary>
        /// <param name="main">the IDS element to load.</param>
        /// <param name="logger">the logging context</param>
        /// <param name="ids"></param>
        /// <returns>an entire new XIDS of null on errors</returns>
        public static Xids? LoadBuildingSmartIDS(XElement main, ILogger? logger = null, Xids? ids = null)
        {
            if (main.Name.LocalName == "ids")
            {
                var ret = ids ?? new Xids();
                var grp = new SpecificationsGroup(ret);
                ret.SpecificationsGroups.Add(grp);
                foreach (var sub in main.Elements())
                {
                    var name = sub.Name.LocalName.ToLowerInvariant();
                    if (name == "specifications")
                    {
                        AddSpecifications(grp, sub, logger);
                    }
                    else if (name == "info")
                    {
                        AddInfo(ret, grp, sub, logger);
                    }
                    else
                    {
                        LogUnexpected(sub, main, logger);
                    }
                }
                return ret;
            }
            else
            {
                logger?.LogError("Unexpected element in ids: '{unexpectedName}'", main.Name.LocalName);
            }
            return null;
        }

        private static void AddInfo(Xids ret, SpecificationsGroup grp, XElement info, ILogger? logger)
        {
            if (ret is null)
                throw new ArgumentNullException(nameof(ret));

            foreach (var elem in info.Elements())
            {
                var name = elem.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "title":
                        grp.Name = elem.Value;
                        break;
                    case "copyright":
                        grp.Copyright = elem.Value;
                        break;
                    case "version":
                        grp.Version = elem.Value;
                        break;
                    case "description":
                        grp.Description = elem.Value;
                        break;
                    case "author":
                        grp.Author = elem.Value;
                        break;
                    case "date":
                        grp.Date = ReadDate(elem, logger);
                        break;
                    case "purpose":
                        grp.Purpose = elem.Value;
                        break;
                    case "milestone":
                        grp.Milestone = elem.Value;
                        break;
                    default:
                        LogUnexpected(elem, info, logger);
                        break;
                }
            }
        }

        private static void LogUnexpected(XElement unepected, XElement parent, ILogger? logger)
        {
            logger?.LogWarning("Unexpected element '{unexpected}' in '{parentName}'.", unepected.Name.LocalName, parent.Name.LocalName);
        }

        private static void LogUnexpected(XAttribute unepected, XElement parent, ILogger? logger)
        {
            logger?.LogWarning("Unexpected attribute '{unexpected}' in '{parentName}'.", unepected.Name.LocalName, parent.Name.LocalName);
        }

        private static void LogUnexpectedValue(XAttribute unepected, XElement parent, ILogger? logger)
        {
            logger?.LogWarning("Unexpected value '{unexpValue}' for attribute '{unexpected}' in '{parentName}'.", unepected.Value, unepected.Name.LocalName, parent.Name.LocalName);
        }

        private static void LogUnsupportedOccurValue(XElement parent, ILogger? logger)
        {
            logger?.LogError("Unsupported values for cardinality in '{parentName}'. Defaulting to expected.", parent.Name.LocalName);
        }

        private static DateTime ReadDate(XElement elem, ILogger? logger)
        {
            try
            {
                var dt = XmlConvert.ToDateTime(elem.Value, XmlDateTimeSerializationMode.Unspecified);
                return dt;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Invalid value for date: {invalidDate}.", elem.Value);
                return DateTime.MinValue;
            }
        }

        private static void AddSpecifications(SpecificationsGroup destGroup, XElement specifications, ILogger? logger)
        {
            foreach (var elem in specifications.Elements())
            {
                var name = elem.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "specification":
                        AddSpecification(destGroup, elem, logger);
                        break;
                    default:
                        LogUnexpected(elem, specifications, logger);
                        break;
                }
            }
        }

        private static void AddSpecification(SpecificationsGroup destGroup, XElement specificationElement, ILogger? logger)
        {
            var ret = new Specification(destGroup);
            var cardinality = new MinMaxCardinality();
            destGroup.Specifications.Add(ret);

            foreach (var att in specificationElement.Attributes())
            {
                var attName = att.Name.LocalName.ToLower();
                switch (attName)
                {
                    case "name":
                        ret.Name = att.Value;
                        break;
                    case "description":
                        ret.Description = att.Value;
                        break;
                    case "ifcversion":
                        var tmp = att.Value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        var tmp2 = new List<IfcSchemaVersion>();
                        foreach (var tmpIter in tmp)
                        {
                            if (Enum.TryParse(tmpIter, out IfcSchemaVersion tmpIter2))
                                tmp2.Add(tmpIter2);
                            else
                                LogUnexpectedValue(att, specificationElement, logger);
                        }
                        if (!tmp2.Any())
                            tmp2.Add(IfcSchemaVersion.IFC2X3);
                        ret.IfcVersion = tmp2;
                        break;
                    case "minoccurs":
                        if (int.TryParse(att.Value, out int tmpMin))
                            cardinality.MinOccurs = tmpMin;
                        else
                            LogUnexpectedValue(att, specificationElement, logger);
                        break;
                    case "maxoccurs":
                        if (att.Value == "unbounded")
                            cardinality.MaxOccurs = null; // null is considered to mean unbounded
                        else if (int.TryParse(att.Value, out int tmpMax))
                            cardinality.MaxOccurs = tmpMax;
                        else
                            LogUnexpectedValue(att, specificationElement, logger);
                        break;
                    case "instructions":
                        ret.Instructions = att.Value;
                        break;
                    default:
                        LogUnexpected(att, specificationElement, logger);
                        break;
                }

            }
            foreach (var sub in specificationElement.Elements())
            {
                var name = sub.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "applicability":
                        {
                            var fs = GetFacets(sub, logger, out _);
                            if (fs.Any())
                                ret.SetFilters(fs);
                            break;
                        }
                    case "requirements":
                        {
                            var fs = GetFacets(sub, logger, out var options);
                            if (fs.Any())
                            {
                                ret.SetExpectations(fs);
                                // todo: as an alternative, RequirementOptions could be set only if they are different from the 
                                // default value (i.e. Expected).
                                //if (options.Any(x => x != RequirementCardinalityOptions.Expected))
                                ret.Requirement!.RequirementOptions = new System.Collections.ObjectModel.ObservableCollection<RequirementCardinalityOptions>(options);
                            }
                            break;
                        }
                    default:
                        LogUnexpected(sub, specificationElement, logger);
                        break;
                }
            }
            ret.Cardinality = cardinality.Simplify();
        }

        private static IFacet GetMaterial(XElement elem, ILogger? logger, out RequirementCardinalityOptions opt)
        {
            MaterialFacet? ret = new(); // material is always initialized, because it's meaningful even if empty
            foreach (var sub in elem.Elements())
            {
                if (IsFacetBaseEntity(sub))
                {
                    ret ??= new MaterialFacet();
                    GetBaseEntity(sub, ret, logger);
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret ??= new MaterialFacet();
                    ret.Value = GetConstraint(sub, logger);
                }
                else
                {
                    LogUnexpected(sub, elem, logger);
                }
            }
            var minMax = new BsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new MaterialFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (BsMinMaxOccur.IsRelevant(attribute, ref minMax))
                {
                    // nothing to do, IsRelevant takes care of minMax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = minMax.Evaluate(elem, logger); // from material
            return ret;
        }

        private static IFacet? GetPartOf(XElement elem, ILogger? logger, out RequirementCardinalityOptions opt)
        {
            PartOfFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "entity":
                        var t = GetEntity(sub, logger);
                        if (t is IfcTypeFacet fct)
                        {
                            ret ??= new PartOfFacet();
                            ret.EntityType = fct;
                        }
                        break;
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
            }
            var minMax = new BsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {

                if (IsBaseAttribute(attribute))
                {
                    ret ??= new PartOfFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (BsMinMaxOccur.IsRelevant(attribute, ref minMax))
                {
                    // nothing to do, IsRelevant takes care of minMax
                }
                else
                {
                    var locName = attribute.Name.LocalName.ToLowerInvariant();
                    switch (locName)
                    {
                        case "relation":
                            ret ??= new PartOfFacet();
                            ret.EntityRelation = attribute.Value;
                            break;
                        default:
                            LogUnexpected(attribute, elem, logger);
                            break;
                    }
                }
            }
            opt = minMax.Evaluate(elem, logger); // from partOf
            return ret;
        }

        private static IFacet? GetProperty(XElement elem, ILogger? logger, out RequirementCardinalityOptions opt)
        {
            IfcPropertyFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                if (IsFacetBaseEntity(sub))
                {
                    ret ??= new IfcPropertyFacet();
                    GetBaseEntity(sub, ret, logger);
                    continue;
                }
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "propertyset":
                        ret ??= new IfcPropertyFacet();
                        ret.PropertySetName = GetConstraint(sub, logger);
                        break;
                    case "name": // either property or name is redundant
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyName = GetConstraint(sub, logger);
                        break;
                    case "value":
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyValue = GetConstraint(sub, logger);
                        break;
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
            }
            var minMax = new BsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcPropertyFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (attribute.Name.LocalName == "datatype")
                {
                    ret ??= new IfcPropertyFacet();
                    ret.DataType = attribute.Value;
                }
                else if (BsMinMaxOccur.IsRelevant(attribute, ref minMax))
                {
                    // nothing to do, IsRelevant takes care of minMax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = minMax.Evaluate(elem, logger); // from property
            return ret;
        }

        private static ValueConstraint? GetConstraint(XElement elem, ILogger? logger)
        {
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            var restriction = elem.Element(ns + "restriction");
            if (restriction == null)
            {
                // get the textual content as a fixed 
                var content = elem.Value;
                var tc = ValueConstraint.SingleUndefinedExact(content);
                return tc;
            }
            NetTypeName t = NetTypeName.Undefined;
            var bse = restriction.Attribute("base");
            if (bse != null && bse.Value != null)
            {
                var tval = bse.Value;
                t = ValueConstraint.GetNamedTypeFromXsd(tval);
            }

            // we prepare the different possible scenarios, but then check in the end that the 
            // xml encountered is solid.
            //
            List<string>? enumeration = null;
            RangeConstraint? range = null;
            PatternConstraint? patternc = null;
            StructureConstraint? structure = null;

            foreach (var sub in restriction.Elements())
            {
                if (sub.Name.LocalName == "enumeration")
                {
                    var val = sub.Attribute("value");
                    if (val != null)
                    {
                        enumeration ??= new List<string>();
                        enumeration.Add(val.Value);
                    }
                }
                else if (
                    sub.Name.LocalName == "minInclusive"
                    ||
                    sub.Name.LocalName == "minExclusive"
                    )
                {
                    var val = sub.Attribute("value")?.Value;
                    if (val != null)
                    {
                        range ??= new RangeConstraint();
                        range.MinValue = val;
                        range.MinInclusive = sub.Name.LocalName == "minInclusive";
                    }
                }
                else if (
                    sub.Name.LocalName == "maxInclusive"
                    ||
                    sub.Name.LocalName == "maxExclusive"
                    )
                {
                    var val = sub.Attribute("value")?.Value;
                    if (val != null)
                    {
                        range ??= new RangeConstraint();
                        range.MaxValue = val;
                        range.MaxInclusive = sub.Name.LocalName == "maxInclusive";
                    }
                }
                else if (sub.Name.LocalName == "pattern")
                {
                    var val = sub.Attribute("value");
                    if (val != null)
                    {
                        patternc = new PatternConstraint() { Pattern = val.Value };
                    }
                }
                else if (sub.Name.LocalName == "minLength")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure ??= new StructureConstraint();
                        structure.MinLength = ival;
                    }
                }
                else if (sub.Name.LocalName == "maxLength")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure ??= new StructureConstraint();
                        structure.MaxLength = ival;
                    }
                }
                else if (sub.Name.LocalName == "length")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure ??= new StructureConstraint();
                        structure.Length = ival;
                    }
                }
                else if (sub.Name.LocalName == "totalDigits")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure ??= new StructureConstraint();
                        structure.TotalDigits = ival;
                    }
                }
                else if (sub.Name.LocalName == "fractionDigits")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure ??= new StructureConstraint();
                        structure.FractionDigits = ival;
                    }
                }
                else if (sub.Name.LocalName == "annotation") // todo: IDSTALK: complexity of annotation
                {
                    // is the implementation of xs:annotation a big overkill for the app?
                    // see  https://www.w3schools.com/xml/el_appinfo.asp
                    //      https://www.w3schools.com/xml/el_annotation.asp
                    //      xs:documentation also has any xml in the body... complicated.

                }
                else
                {
                    LogUnexpected(sub, elem, logger);
                }
            }
            // check that the temporary variable are coherent with valid value
            var count = (enumeration != null) ? 1 : 0;
            count += (range != null) ? 1 : 0;
            count += (patternc != null) ? 1 : 0;
            count += (structure != null) ? 1 : 0;
            if (count != 1)
            {
                logger?.LogWarning("Invalid value constraint for {localname} full xml '{elem}'.", elem.Name.LocalName, elem);
                return null;
            }
            if (enumeration != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraintComponent>()
                };
                foreach (var val in enumeration)
                {
                    ret.AcceptedValues.Add(new ExactConstraint(val));
                }
                return ret;
            }
            if (range != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraintComponent>() { range }
                };
                return ret;
            }
            if (patternc != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraintComponent>() { patternc }
                };
                return ret;
            }
            if (structure != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraintComponent>() { structure }
                };
                return ret;
            }
            return null;
        }

        private static List<IFacet> GetFacets(XElement elem, ILogger? logger, out IEnumerable<RequirementCardinalityOptions> options)
        {
            var fs = new List<IFacet>();
            var opts = new List<RequirementCardinalityOptions>();
            foreach (var sub in elem.Elements())
            {
                IFacet? t = null;
                RequirementCardinalityOptions opt = RequirementCardinalityOptions.Expected;
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "entity":
                        t = GetEntity(sub, logger);
                        break;
                    case "classification":
                        t = GetClassification(sub, logger, out opt);
                        break;
                    case "property":
                        t = GetProperty(sub, logger, out opt);
                        break;
                    case "material":
                        t = GetMaterial(sub, logger, out opt);
                        break;
                    case "attribute":
                        t = GetAttribute(sub, logger, out opt);
                        break;
                    case "partof":
                        t = GetPartOf(sub, logger, out opt);
                        break;
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
                if (t != null)
                {
                    fs.Add(t);
                    opts.Add(opt);
                }
            }
            options = opts;
            return fs;
        }

        private static IFacet? GetAttribute(XElement elem, ILogger? logger, out RequirementCardinalityOptions opt)
        {
            AttributeFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                var subname = sub.Name.LocalName.ToLowerInvariant();
                switch (subname)
                {
                    case "name":
                        ret ??= new AttributeFacet();
                        ret.AttributeName = GetConstraint(sub, logger);
                        break;
                    case "value":
                        ret ??= new AttributeFacet();
                        ret.AttributeValue = GetConstraint(sub, logger);
                        break;
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
            }
            var minMax = new BsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                var subName = attribute.Name.LocalName.ToLowerInvariant();
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new AttributeFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (BsMinMaxOccur.IsRelevant(attribute, ref minMax))
                {
                    // nothing to do, IsRelevant takes care of minMax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = minMax.Evaluate(elem, logger); // from attribute
            return ret;
        }

        private class BsMinMaxOccur
        {
            public string Min { get; set; } = "";
            public string Max { get; set; } = "";

            internal static bool IsRelevant(XAttribute attribute, ref BsMinMaxOccur minMax)
            {
                if (attribute.Name == "minOccurs")
                {
                    minMax.Min = attribute.Value;
                    return true;
                }
                if (attribute.Name == "maxOccurs")
                {
                    minMax.Max = attribute.Value;
                    return true;
                }
                return false;
            }

            private static readonly RequirementCardinalityOptions DefaultCardinality = RequirementCardinalityOptions.Expected;

            internal RequirementCardinalityOptions Evaluate(XElement elem, ILogger? logger)
            {
                if (Min == "" && Max == "")
                    return DefaultCardinality; // set default
                if (Min == "0" && Max == "0")
                    return RequirementCardinalityOptions.Prohibited;
                if (Max == "unbounded" || Max == "" || Max == "1")
                {
                    if (Min == "1" || Min=="") // default is 1
                        return RequirementCardinalityOptions.Expected;
                    if (Min == "0")
                        return RequirementCardinalityOptions.Optional;
                }              
                // throw warning and set default value
                LogUnsupportedOccurValue(elem, logger);
                return DefaultCardinality; // set default
            }
        }

        private static IFacet GetClassification(XElement elem, ILogger? logger, out RequirementCardinalityOptions opt)
        {
            IfcClassificationFacet? ret = new(); // classification is always initialized, because it's meaningful even if empty
            foreach (var sub in elem.Elements())
            {
                if (IsFacetBaseEntity(sub))
                {
                    ret ??= new IfcClassificationFacet();
                    GetBaseEntity(sub, ret, logger);
                }
                else if (sub.Name.LocalName == "system")
                {
                    ret ??= new IfcClassificationFacet();
                    ret.ClassificationSystem = GetConstraint(sub, logger);
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret ??= new IfcClassificationFacet();
                    ret.Identification = GetConstraint(sub, logger);
                }
                else
                {
                    LogUnexpected(sub, elem, logger);
                }
            }

            var minMax = new BsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                var locAtt = attribute.Name.LocalName;
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcClassificationFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (BsMinMaxOccur.IsRelevant(attribute, ref minMax))
                {
                    // nothing to do, IsRelevant takes care of minMax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = minMax.Evaluate(elem, logger); // from classification
            return ret;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private static void GetBaseEntity(XElement sub, FacetBase ret, ILogger? logger)
        {
            var local = sub.Name.LocalName.ToLowerInvariant();
            //if (local == "instructions")
            //    ret.Instructions = sub.Value;
            //else
            logger?.LogWarning("Unexpected element {local} reading FacetBase.", local);
        }
#pragma warning restore IDE0060 // Remove unused parameter

#pragma warning disable IDE0060 // Remove unused parameter (sub)
        private static bool IsFacetBaseEntity(XElement sub)
        {
            //switch (sub.Name.LocalName)
            //{
            //    case "instructions":
            //        return true;
            //    default:
            //        return false;
            //}
            return false;
        }
#pragma warning restore IDE0060 // Remove unused parameter

        private static bool IsBaseAttribute(XAttribute attribute)
        {
            return attribute.Name.LocalName switch
            {
                "uri" or "instructions" => true,
                _ => false,
            };
        }

        private static void GetBaseAttribute(XAttribute attribute, FacetBase ret, ILogger? logger)
        {
            if (attribute.Name.LocalName == "uri")
                ret.Uri = attribute.Value;
            else if (attribute.Name.LocalName == "instructions")
                ret.Instructions = attribute.Value;
            else
            {
                logger?.LogError("Unrecognised base attribute {attributeName}", attribute.Name);
            }
        }


        private const bool defaultSubTypeInclusion = false;

        private static IFacet? GetEntity(XElement elem, ILogger? logger)
        {
            IfcTypeFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "name":
                        ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                        ret.IfcType = GetConstraint(sub, logger);
                        break;
                    case "predefinedtype":
                        ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                        ret.PredefinedType = GetConstraint(sub, logger);
                        break;
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                    GetBaseAttribute(attribute, ret, logger);
                }
                else
                    LogUnexpected(attribute, elem, logger);
            }
            return ret;
        }



       

        private static string GetFirstString(XElement sub)
        {
            if (!string.IsNullOrEmpty(sub.Value))
                return sub.Value;
            var nm = sub.Name.LocalName.ToLowerInvariant();
            switch (nm)
            {
                case "pattern":
                    {
                        var val = sub.Attribute("value");
                        if (!string.IsNullOrEmpty(val?.Value))
                        {
                            return val!.Value; // bang is redundant in net5, but net2 is capricious with nullability checks
                        }
                        break;
                    }
            }
            foreach (var sub2 in sub.Elements())
            {
                var subS = GetFirstString(sub2); // recursive
                if (!string.IsNullOrEmpty(subS))
                    return subS;
            }
            return "";
        }
    }
}