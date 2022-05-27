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
#if DEBUG
                settings.Indent = true;
                settings.IndentChars = "\t";
#endif
                return settings;
            }
        }

        public enum ExportedFormat
        {
            XML,
            ZIP
        }

        /// <summary>
        /// Exports the entire XIDS to a buildingSmart file, depending on the number of groups exports an XML or a ZIP file.
        /// </summary>
        /// <param name="destinationFileName">the path of a writeable location on disk</param>
        /// <returns>An enum determining if XML or ZIP files were written</returns>
        public ExportedFormat ExportBuildingSmartIDS(string destinationFileName, ILogger? logger = null)
        {
            using FileStream fs = File.OpenWrite(destinationFileName);
            return ExportBuildingSmartIDS(fs, logger);
        }

        public ExportedFormat ExportBuildingSmartIDS(Stream destinationStream, ILogger? logger = null)
        {
            if (SpecificationsGroups.Count == 1)
            {
                using XmlWriter writer = XmlWriter.Create(destinationStream, WriteSettings);
                ExportBuildingSmartIDS(SpecificationsGroups.First(), writer, logger);
                return ExportedFormat.XML;
            }

            using (var zipArchive = new ZipArchive(destinationStream, ZipArchiveMode.Create, true))
            {
                int i = 0;
                foreach (var specGroup in SpecificationsGroups)
                {
                    var name = (specGroup.Name is not null && !string.IsNullOrEmpty(specGroup.Name) && specGroup.Name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                        ? $"{++i} - {specGroup.Name}.xml"
                        : $"{++i}.xml";
                    var file = zipArchive.CreateEntry(name);
                    using var str = file.Open();
                    using XmlWriter writer = XmlWriter.Create(str, WriteSettings);
                    ExportBuildingSmartIDS(specGroup, writer, logger);

                }
            }
            return ExportedFormat.ZIP;
        }

        

        private void ExportBuildingSmartIDS(SpecificationsGroup specGroup, XmlWriter xmlWriter, ILogger? logger)
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
            foreach (var requirement in specGroup.Specifications)
            {
                ExportBuildingSmartIDS(requirement, xmlWriter, logger);
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

        private void ExportBuildingSmartIDS(Specification spec, XmlWriter xmlWriter, ILogger? logger)
        {
            xmlWriter.WriteStartElement("specification", IdsNamespace);
            if (spec.IfcVersion != null)
                xmlWriter.WriteAttributeString("ifcVersion", string.Join(" ", spec.IfcVersion));
            else
                xmlWriter.WriteAttributeString("ifcVersion", IfcSchemaVersion.IFC2X3.ToString()); // required for bS schema
            // if (requirement.Name != null)
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
                    ExportBuildingSmartIDS(item, xmlWriter, false, logger, null);
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
                    var option = GetProgressive(opts, i, RequirementOptions.Expected);
                    IFacet? item = spec.Requirement.Facets[i];
                    ExportBuildingSmartIDS(item, xmlWriter, true, logger, option);
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private RequirementOptions GetProgressive(ObservableCollection<RequirementOptions>? opts, int i, RequirementOptions defaultValue)
        {
            if (opts is null)
                return defaultValue;
            if (i >= opts.Count)
                return defaultValue;
            return opts[i];
        }

        private void ExportBuildingSmartIDS(IFacet item, XmlWriter xmlWriter, bool forRequirement, ILogger? logger, RequirementOptions? requirementOption = null)
        {
            switch (item)
            {
                case IfcTypeFacet tf:
                    xmlWriter.WriteStartElement("entity", IdsNamespace);
                    WriteFaceteBaseAttributes(tf, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(tf.IfcType, xmlWriter, "name", logger);
                    WriteConstraintValue(tf.PredefinedType, xmlWriter, "predefinedType", logger);
                    xmlWriter.WriteEndElement();
                    break;
                case IfcClassificationFacet cf:
                    xmlWriter.WriteStartElement("classification", IdsNamespace);
                    WriteFaceteBaseAttributes(cf, xmlWriter, logger, forRequirement, requirementOption); 
                    WriteConstraintValue(cf.Identification, xmlWriter, "value", logger);
                    WriteConstraintValue(cf.ClassificationSystem, xmlWriter, "system", logger);
                    WriteFaceteBaseElements(cf, xmlWriter); // from classifcation
                    xmlWriter.WriteEndElement();
                    break;                    
                case IfcPropertyFacet pf:
                    xmlWriter.WriteStartElement("property", IdsNamespace);
                    WriteFaceteBaseAttributes(pf, xmlWriter, logger, forRequirement, requirementOption);
                    if (!string.IsNullOrWhiteSpace(pf.Measure))
                        xmlWriter.WriteAttributeString("measure", pf.Measure);
                    WriteConstraintValue(pf.PropertySetName, xmlWriter, "propertySet", logger);
                    WriteConstraintValue(pf.PropertyName, xmlWriter, "name", logger);                  
                    WriteConstraintValue(pf.PropertyValue, xmlWriter, "value", logger);
                    WriteFaceteBaseElements(pf, xmlWriter); // from Property
                    xmlWriter.WriteEndElement();
                    break;
                case MaterialFacet mf:
                    xmlWriter.WriteStartElement("material", IdsNamespace);
                    WriteFaceteBaseAttributes(mf, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(mf.Value, xmlWriter, "value", logger);
                    WriteFaceteBaseElements(mf, xmlWriter); // from material
                    xmlWriter.WriteEndElement();
                    break;
                case AttributeFacet af:
                    xmlWriter.WriteStartElement("attribute", IdsNamespace);
                    WriteFaceteBaseAttributes(af, xmlWriter, logger, forRequirement, requirementOption);
                    WriteConstraintValue(af.AttributeName, xmlWriter, "name", logger);
                    WriteConstraintValue(af.AttributeValue, xmlWriter, "value", logger);
                    xmlWriter.WriteEndElement();
                    break;
                case PartOfFacet pof:
                    xmlWriter.WriteStartElement("partOf", IdsNamespace);
                    WriteFaceteBaseAttributes(pof, xmlWriter, logger, forRequirement, requirementOption);
                    xmlWriter.WriteAttributeString("entity", pof.Entity.ToString());
                    xmlWriter.WriteEndElement();
                    break;
                default:
                    logger?.LogWarning($"todo: missing case for {item.GetType()}.");
                    break;
            }
        }

        private void WriteSimpleValue(XmlWriter xmlWriter, string stringValue)
        {
            xmlWriter.WriteStartElement("simpleValue", IdsNamespace);
            xmlWriter.WriteString(stringValue);
            xmlWriter.WriteEndElement();
        }

        private void WriteConstraintValue(ValueConstraint? value, XmlWriter xmlWriter, string name, ILogger? logger)
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
                if (value.BaseType != TypeName.Undefined)
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

        private void WriteFaceteBaseAttributes(FacetBase cf, XmlWriter xmlWriter, ILogger? logger, bool forRequirement, RequirementOptions? option)
        {
            if (forRequirement)
            {
                if (!option.HasValue)
                    option = RequirementOptions.Expected; // should be redundant, but makes some be not null

                if (
                    cf is IfcPropertyFacet ||
                    cf is AttributeFacet
                )
                {
                    // use is required
                    switch (option.Value)
                    {
                        case RequirementOptions.Prohibited:
                            xmlWriter.WriteAttributeString("minOccurs", "0");
                            xmlWriter.WriteAttributeString("maxOccurs", "0");
                            break;
                        case RequirementOptions.Expected:
                            xmlWriter.WriteAttributeString("minOccurs", "1");
                            xmlWriter.WriteAttributeString("maxOccurs", "unbounded");
                            break;
                        default:
                            logger?.LogError("Invalid RequirementOption persistence for '{option}'", option);
                            break;
                    }   
                }

                if (cf is not PartOfFacet)
                {
                    // instruction is optional
                    if (!string.IsNullOrWhiteSpace(cf.Instructions))
                        xmlWriter.WriteAttributeString("instructions", cf.Instructions);
                }
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
        private void WriteFaceteBaseElements(FacetBase cf, XmlWriter xmlWriter)
        {
            // function is kept in case it's gonna be useful again for structure purposes
        }
#pragma warning restore IDE0060 // Remove unused parameter


        /// <summary>
        /// Attempts to unpersist an XIDS from a stream.
        /// </summary>
        /// <param name="stream">The source stream to parse.</param>
        /// <param name="logger">The logger to send any errors and warnings to.</param>
        /// <returns>an XIDS or null if it could not be read.</returns>
        public static Xids? ImportBuildingSmartIDS(Stream stream, ILogger? logger = null)
        {
            var t = XElement.Load(stream);
            return ImportBuildingSmartIDS(t, logger);
        }

        /// <summary>
        /// Attempts to unpersist an XIDS from a file, given the file name.
        /// </summary>
        /// <param name="logger">The logger to send any errors and warnings to.</param>
        /// <returns>an XIDS or null if it could not be read.</returns>
        public static Xids? ImportBuildingSmartIDS(string fileName, ILogger? logger = null)
        {
            if (!File.Exists(fileName))
            {
                var d = new DirectoryInfo(".");
                logger?.LogError($"File '{fileName}' not found from executing directory '{d.FullName}'");
                return null;
            }
            var main = XElement.Parse(File.ReadAllText(fileName));
            return ImportBuildingSmartIDS(main, logger);
        }

        /// <summary>
        /// Attempts to unpersist an XIDS from an XML element.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static Xids? ImportBuildingSmartIDS(XElement main, ILogger? logger = null)
        {
            if (main.Name.LocalName == "ids")
            {
                var ret = new Xids();
                var grp = new SpecificationsGroup();
                ret.SpecificationsGroups.Add(grp);
                foreach (var sub in main.Elements())
                {
                    var name = sub.Name.LocalName.ToLowerInvariant();
                    if (name == "specifications")
                    {
                        AddSpecifications(ret, grp, sub, logger);
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
                logger?.LogError($"Unexpected element in ids: '{main.Name.LocalName}'");
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
                logger?.LogError(ex, $"Invalid value for date: {elem.Value}.");
                return DateTime.MinValue;
            }
        }

        private static void AddSpecifications(Xids ids, SpecificationsGroup destGroup, XElement specifications, ILogger? logger)
        {
            foreach (var elem in specifications.Elements())
            {
                var name = elem.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "specification":
                        AddSpecification(ids, destGroup, elem, logger);
                        break;
                    default:
                        LogUnexpected(elem, specifications, logger);
                        break;
                }
            }
        }

        private static void AddSpecification(Xids ids, SpecificationsGroup destGroup, XElement specificationElement, ILogger? logger)
        {
            var ret = new Specification(ids, destGroup);
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
                                if (options.Any(x => x != RequirementOptions.Expected))
                                    ret.Requirement!.RequirementOptions = new System.Collections.ObjectModel.ObservableCollection<RequirementOptions>(options);
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

        private static IFacet? GetMaterial(XElement elem, ILogger? logger, out RequirementOptions opt)
        {
            MaterialFacet? ret = null;
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
            var mmax = new bsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new MaterialFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (bsMinMaxOccur.IsRelevant(attribute, ref mmax))
                {
                    // nothing to do, IsRelevant takes care of mmax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = mmax.Evaluate(elem, logger);
            return ret;
        }

        private static IFacet? GetProperty(XElement elem, ILogger? logger, out RequirementOptions opt)
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
                    case "property": // either property or name is redundant
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyName = sub.Value;
                        break;
                    case "name": // either property or name is redundant
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyName = sub.Value;
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
            var mmax = new bsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcPropertyFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (attribute.Name.LocalName == "measure")
                {
                    ret ??= new IfcPropertyFacet();
                    ret.Measure = attribute.Value;
                }
                else if (bsMinMaxOccur.IsRelevant(attribute, ref mmax))
                {
                    // nothing to do, IsRelevant takes care of mmax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = mmax.Evaluate(elem, logger);
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
            TypeName t = TypeName.Undefined;
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
                logger?.LogWarning($"Invalid value constraint for {elem.Name.LocalName} full xml '{elem}'.");
                return null;
            }
            if (enumeration != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraint>()
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
                    AcceptedValues = new List<IValueConstraint>() { range }
                };
                return ret;
            }
            if (patternc != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraint>() { patternc }
                };
                return ret;
            }
            if (structure != null)
            {
                var ret = new ValueConstraint(t)
                {
                    AcceptedValues = new List<IValueConstraint>() { structure }
                };
                return ret;
            }
            return null;
        }

        private static List<IFacet> GetFacets(XElement elem, ILogger? logger, out IEnumerable<RequirementOptions> options)
        {
            var fs = new List<IFacet>();
            var opts = new List<RequirementOptions>();
            foreach (var sub in elem.Elements())
            {
                IFacet? t = null;
                RequirementOptions opt = RequirementOptions.Expected;
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
                        t = GetPartOf(sub, logger);
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

        private static IFacet? GetAttribute(XElement elem, ILogger? logger, out RequirementOptions opt)
        {
            AttributeFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                var subname = sub.Name.LocalName.ToLowerInvariant();
                switch (subname)
                {
                    case "name":
                        ret ??= new AttributeFacet();
                        ret.AttributeName = sub.Value;
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
            var mmax = new bsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                var subname = attribute.Name.LocalName.ToLowerInvariant();
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new AttributeFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (bsMinMaxOccur.IsRelevant(attribute, ref mmax))
                {
                    // nothing to do, IsRelevant takes care of mmax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = mmax.Evaluate(elem, logger);
            return ret;
        }

        private class bsMinMaxOccur
        {
            public string Min { get; set; } = "";
            public string Max { get; set; } = "";

            internal static bool IsRelevant(XAttribute attribute, ref bsMinMaxOccur mmax)
            {
                if (attribute.Name == "minOccurs")
                {
                    mmax.Min = attribute.Value;
                    return true;
                }
                if (attribute.Name == "maxOccurs")
                {
                    mmax.Max = attribute.Value;
                    return true;
                }
                return false;
            }

            internal RequirementOptions Evaluate(XElement elem, ILogger? logger)
            {
                if (Min == "" && Max == "")
                    return RequirementOptions.Expected; // default value

                // managed min values
                if (Min != "1" && Min != "0")
                {
                    LogUnsupportedOccurValue(elem, logger);
                    return RequirementOptions.Expected;
                }

                // managed max values
                if (Max != "0" && Max != "unbounded" && Max != "")
                {
                    LogUnsupportedOccurValue(elem, logger);
                    return RequirementOptions.Expected;
                }

                if (Min == "0" && Max == "0")
                    return RequirementOptions.Prohibited;
                if (Min == "1" && 
                    (Max == "unbounded" || Max == "")
                    )
                    return RequirementOptions.Expected;

                LogUnsupportedOccurValue(elem, logger);
                return RequirementOptions.Expected;
            }
        }

        private static IFacet? GetClassification(XElement elem, ILogger? logger, out RequirementOptions opt)
        {
            IfcClassificationFacet? ret = null;
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

            var mmax = new bsMinMaxOccur();
            foreach (var attribute in elem.Attributes())
            {
                var locAtt = attribute.Name.LocalName;
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcClassificationFacet();
                    GetBaseAttribute(attribute, ret, logger);
                }
                else if (bsMinMaxOccur.IsRelevant(attribute, ref mmax))
                {
                    // nothing to do, IsRelevant takes care of mmax
                }
                else
                {
                    LogUnexpected(attribute, elem, logger);
                }
            }
            opt = mmax.Evaluate(elem, logger);
            return ret;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private static void GetBaseEntity(XElement sub, FacetBase ret, ILogger? logger)
        {
            var local = sub.Name.LocalName.ToLowerInvariant();
            //if (local == "instructions")
            //    ret.Instructions = sub.Value;
            //else
            logger?.LogWarning($"Unexpected element {local} reading FacetBase.");
        }
#pragma warning restore IDE0060 // Remove unused parameter

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

        private static bool IsBaseAttribute(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
            {
                case "uri":
                case "instructions":
                    return true;
                default:
                    return false;
            }
        }

        private static void GetBaseAttribute(XAttribute attribute, FacetBase ret, ILogger? logger)
        {
            if (attribute.Name.LocalName == "uri")
                ret.Uri = attribute.Value;
            else if (attribute.Name.LocalName == "instructions")
                ret.Instructions = attribute.Value;
            else
            {
                logger?.LogError("Unrecognised base attribute {0}", attribute.Name);
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
                        if (string.IsNullOrEmpty(sub.Value))
                        {
                            ret.IfcType = GetFirstString(sub);  // todo: dealing with uncomprehensible v0.5 values
                                                                // see: https://github.com/buildingSMART/IDS/blob/7903eae20127c10b52cd37abf42a7cd7c2bcf973/Development/0.5/IDS_random_example_04.xml#L12-L18
                        }
                        else
                            ret.IfcType = sub.Value;
                        break;
                    case "predefinedtype":
                        ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                        ret.PredefinedType = sub.Value;
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

        

        private static IFacet? GetPartOf(XElement elem, ILogger? logger)
        {
            PartOfFacet? ret = null;
            foreach (var sub in elem.Elements())
            {
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    default:
                        LogUnexpected(sub, elem, logger);
                        break;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                var locName = attribute.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "entity":
                        ret ??= new PartOfFacet();
                        ret.Entity = attribute.Value;
                        break;
                    default:
                        LogUnexpected(attribute, elem, logger);
                        break;
                }
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
#pragma warning disable CS8602 // Dereference of a possibly null reference. 
                            return val.Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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