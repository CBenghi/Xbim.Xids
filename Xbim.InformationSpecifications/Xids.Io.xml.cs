using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Xbim.InformationSpecifications
{
    public partial class Xids
    {
        private static XmlWriterSettings WriteSettings
        {
            get
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Async = false;
#if DEBUG
                settings.Indent = true;
                settings.IndentChars = "\t";
#endif
                return settings;
            }
        }

        public void ExportBuildingSmartIDS(string destinationFile)
        {
            using (XmlWriter writer = XmlWriter.Create(destinationFile, WriteSettings))
            {
                ExportBuildingSmartIDS(writer);
            }
        }

        public void ExportBuildingSmartIDS(Stream destinationStream)
        {
            using (XmlWriter writer = XmlWriter.Create(destinationStream, WriteSettings))
            {
                ExportBuildingSmartIDS(writer);
            }
        }

        private void ExportBuildingSmartIDS(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ids", "ids", @"http://standards.buildingsmart.org/IDS");
            // writer.WriteAttributeString("xsi", "xmlns", @"http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids.xsd");

            // info goes first
            xmlWriter.WriteStartElement("info", IdsNamespace);
            var titles = string.Join(", ",
                    SpecificationsGroups.Select(x => x.Name).Distinct().ToArray());
            xmlWriter.WriteElementString("title", IdsNamespace, titles);

            var copy = string.Join(", ",
                    SpecificationsGroups.Select(x => x.Copyright).Distinct().ToArray());
            xmlWriter.WriteElementString("copyright", IdsNamespace, copy);
            xmlWriter.WriteElementString("ifcVersion", IdsNamespace, IfcVersion);

            var date = SpecificationsGroups.Select(x => x.Date).Max();
            if (date != DateTime.MinValue)
            {
                xmlWriter.WriteElementString("date", IdsNamespace, $"{date:yyyy-MM-dd}");
            }
            xmlWriter.WriteEndElement();

            // then the specifications
            xmlWriter.WriteStartElement("specifications", IdsNamespace);
            foreach (var requirement in AllSpecifications())
            {
                ExportBuildingSmartIDS(requirement, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        private const string IdsNamespace = @"http://standards.buildingsmart.org/IDS";
        private const string IdsPrefix = "";

        private void ExportBuildingSmartIDS(Specification requirement, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("specification", IdsNamespace);
            if (requirement.Name != null)
                xmlWriter.WriteAttributeString("name", requirement.Name);
            xmlWriter.WriteAttributeString("use", requirement.Use.ToString().ToLowerInvariant());
            xmlWriter.WriteStartElement("applicability", IdsNamespace);
            foreach (var item in requirement.Applicability.Facets)
            {
                ExportBuildingSmartIDS(item, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("requirements", IdsNamespace);
            foreach (var item in requirement.Requirement.Facets)
            {
                ExportBuildingSmartIDS(item, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private void ExportBuildingSmartIDS(IFacet item, XmlWriter xmlWriter)
        {
            switch (item)
            {
                case IfcTypeFacet tf:
                    xmlWriter.WriteStartElement("entity", IdsNamespace);
                    if (!string.IsNullOrWhiteSpace(tf.IfcType))
                    {
                        xmlWriter.WriteStartElement("name", IdsNamespace);
                        WriteSimpleValue(xmlWriter, tf.IfcType);
                        xmlWriter.WriteEndElement();
                    }
                    if (!string.IsNullOrWhiteSpace(tf.PredefinedType))
                    {
                        xmlWriter.WriteStartElement("predefinedType", IdsNamespace);
                        WriteSimpleValue(xmlWriter, tf.PredefinedType);
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                    break;
                case IfcClassificationFacet cf:
                    {
                        xmlWriter.WriteStartElement("classification", IdsNamespace);
                        WriteLocationAttributes(cf, xmlWriter); // attribute
                        WriteValue(cf.Identification, xmlWriter);
                        Dictionary<string, string> attributes = new Dictionary<string, string>();
                        if (!string.IsNullOrWhiteSpace(cf.ClassificationSystemHref))
                        {
                            attributes.Add("href", cf.ClassificationSystemHref);
                        }
                        WriteValue(cf.ClassificationSystem, xmlWriter, "system", attributes);
                        WriteLocationElements(cf, xmlWriter);

                        xmlWriter.WriteEndElement();
                        break;
                    }

                case IfcPropertyFacet pf:
                    xmlWriter.WriteStartElement("property", IdsNamespace);
                    WriteLocationAttributes(pf, xmlWriter);
                    if (!string.IsNullOrWhiteSpace(pf.PropertySetName))
                    {
                        xmlWriter.WriteStartElement("propertySet", IdsNamespace);
                        WriteSimpleValue(xmlWriter, pf.PropertySetName);
                        xmlWriter.WriteEndElement();
                    }
                    if (!string.IsNullOrWhiteSpace(pf.PropertyName))
                    {
                        xmlWriter.WriteStartElement("name", IdsNamespace);
                        WriteSimpleValue(xmlWriter, pf.PropertyName);
                        xmlWriter.WriteEndElement();
                    }
                    WriteValue(pf.PropertyValue, xmlWriter);
                    WriteLocationElements(pf, xmlWriter);
                    xmlWriter.WriteEndElement();
                    break;
                case MaterialFacet mf:
                    xmlWriter.WriteStartElement("material", IdsNamespace);
                    WriteLocationAttributes(mf, xmlWriter);
                    WriteValue(mf.Value, xmlWriter);
                    WriteLocationElements(mf, xmlWriter);
                    xmlWriter.WriteEndElement();
                    break;
                case AttributeFacet af:
                    xmlWriter.WriteStartElement("attribute", IdsNamespace);
                    xmlWriter.WriteAttributeString("location", af.Location.ToLowerInvariant());
                    xmlWriter.WriteStartElement("name", IdsNamespace);
                    WriteSimpleValue(xmlWriter, af.AttributeName);
                    xmlWriter.WriteEndElement();
                    WriteValue(af.AttributeValue, xmlWriter);
                    xmlWriter.WriteEndElement();
                    break;
                default:
                    Debug.WriteLine($"todo: missing case for {item.GetType()}.");
                    break;
            }
        }

        private void WriteSimpleValue(XmlWriter xmlWriter, string stringValue)
        {
            xmlWriter.WriteStartElement("simpleValue", IdsNamespace);
            xmlWriter.WriteString(stringValue);
            xmlWriter.WriteEndElement();
        }

        private void WriteValue(ValueConstraint value, XmlWriter xmlWriter, string name = "value", Dictionary<string, string> attributes = null)
        {
            if (value == null)
                return;
            
            xmlWriter.WriteStartElement(name, IdsNamespace);
            if (attributes != null)
            {
                foreach (var att in attributes)
                {
                    xmlWriter.WriteAttributeString(att.Key, att.Value);
                }
            }
            if (value.IsSingleUndefinedExact(out string exact))
            {
                // xmlWriter.WriteString(exact);
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

        private void WriteLocationAttributes(FacetBase cf, XmlWriter xmlWriter)
        {
            if (!string.IsNullOrWhiteSpace(cf.Location))
                xmlWriter.WriteAttributeString("location", cf.Location);
            if (!string.IsNullOrWhiteSpace(cf.Uri))
                xmlWriter.WriteAttributeString("href", cf.Uri);
            if (!string.IsNullOrWhiteSpace(cf.Use))
                xmlWriter.WriteAttributeString("use", cf.Use);
        }

        private void WriteLocationElements(FacetBase cf, XmlWriter xmlWriter)
        {
            if (!string.IsNullOrWhiteSpace(cf.Instructions))
                xmlWriter.WriteElementString("instructions", cf.Instructions);
        }

        public static Xids ImportBuildingSmartIDS(Stream stream)
        {
            var t = XElement.Load(stream);
            return ImportBuildingSmartIDS(t);
        }

        public static Xids ImportBuildingSmartIDS(string fileName)
        {
            if (!File.Exists(fileName))
            {
                DirectoryInfo d = new DirectoryInfo(".");
                Debug.WriteLine($"file '{fileName}' not found from '{d.FullName}'"); ;
                return null;
            }
            
            var main = XElement.Parse(File.ReadAllText(fileName));

            return ImportBuildingSmartIDS(main);
        }

        public static Xids ImportBuildingSmartIDS(XElement main)
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
                        AddSpecifications(ret, grp, sub);
                    }
                    else if (name == "info")
                    {
                        AddInfo(ret, grp, sub);
                    }
                    else
                    {
                        Debug.WriteLine($"Unexpected field evaluating main element: '{name}'");
                    }
                }
                return ret;
            }
            return null;
        }

        private static void AddInfo(Xids ret, SpecificationsGroup grp, XElement info)
        {
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
                    case "ifcversion":
                        ret.IfcVersion = elem.Value;
                        break;
                    case "date":
                        grp.Date = ReadDate(elem);
                        break;
                    default:
                        Debug.WriteLine($"Unexpected field evaluating info element: '{elem.Name.LocalName}'");
                        break;
                }
            }
        }

        private static DateTime ReadDate(XElement elem)
        {
            var dt = XmlConvert.ToDateTime(elem.Value, XmlDateTimeSerializationMode.Unspecified);
            return dt;
        }

        private static void AddSpecifications(Xids ids, SpecificationsGroup destGroup, XElement specifications)
        {
            foreach (var elem in specifications.Elements())
            {
                var name = elem.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "specification":
                        AddSpecification(ids, destGroup, elem);
                        break;
                    default:
                        Debug.WriteLine($"Unexpected field evaluating specifications element: '{name}'");
                        break;
                }
            }
        }

        private static void AddSpecification(Xids ids, SpecificationsGroup destGroup, XElement spec)
        {
            var req = new Specification(ids, destGroup);
            destGroup.Specifications.Add(req);
            var nm = spec.Attribute("name");
            if (nm != null)
                req.Name = nm.Value;
            foreach (var elem in spec.Elements())
            {
                var name = elem.Name.LocalName.ToLowerInvariant();
                switch (name)
                {
                    case "applicability":
                        {
                            var fs = GetFacets(elem);
                            if (fs.Any())
                            {
                                req.SetFilters(fs);
                            }

                            break;
                        }

                    case "requirements":
                        {
                            var fs = GetFacets(elem);
                            if (fs.Any())
                            {
                                req.SetExpectations(fs);
                            }

                            break;
                        }
                    default:
                        {
                            Debug.WriteLine($"skipping attribute {name}");
                            break;
                        }
                }
            }
        }

        private static IFacet GetMaterial(XElement elem)
        {
            MaterialFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                if (IsBaseEntity(sub))
                {
                    ret ??= new MaterialFacet();
                    GetBaseEntity(sub, ret);
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret ??= new MaterialFacet();
                    ret.Value = GetConstraint(sub);
                }
                else
                {
                    Debug.WriteLine($"skipping {sub.Name.LocalName}");
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new MaterialFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret ??= new MaterialFacet();
                    ret.Location = attribute.Value;
                }
                else
                {
                    Debug.WriteLine($"skipping attribute {attribute.Name}");
                }
            }
            return ret;
        }

        private static IFacet GetProperty(XElement elem)
        {
            IfcPropertyFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                if (IsBaseEntity(sub))
                {
                    ret ??= new IfcPropertyFacet();
                    GetBaseEntity(sub, ret);
                }
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "propertyset":
                        ret ??= new IfcPropertyFacet();
                        ret.PropertySetName = GetFirstString(sub);
                        break;
                    case "property":
                    case "name":
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyName = sub.Value;
                        break;
                    case "value":
                        ret ??= new IfcPropertyFacet();
                        ret.PropertyValue = GetConstraint(sub);
                        break;
                    default:
                        Debug.WriteLine($"skipping {locName}");
                        break;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcPropertyFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret ??= new IfcPropertyFacet();
                    ret.Location = attribute.Value;
                }
                else
                {
                    Debug.WriteLine($"skipping attribute {attribute.Name}");
                }
            }
            return ret;
        }

        private static ValueConstraint GetConstraint(XElement elem)
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
            List<string> enumeration = null;
            RangeConstraint range = null;
            PatternConstraint patternc = null;
            StructureConstraint structure = null;

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
                else
                {

                }
            }
            // check that the temporary variable are coherent with valid value
            var count = (enumeration != null) ? 1 : 0;
            count += (range != null) ? 1 : 0;
            count += (patternc != null) ? 1 : 0;
            count += (structure != null) ? 1 : 0;
            if (count != 1)
                return null;
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



        private static List<IFacet> GetFacets(XElement elem)
        {
            var fs = new List<IFacet>();
            foreach (var sub in elem.Elements())
            {
                IFacet t = null;
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "entity":
                        t = GetEntity(sub);
                        break;
                    case "classification":
                        t = GetClassification(sub);
                        break;
                    case "property":
                        t = GetProperty(sub);
                        break;
                    case "material":
                        t = GetMaterial(sub);
                        break;
                    case "attribute":
                        t = GetAttribute(sub);
                        break;
                    default:
                        break;
                }
                if (t != null)
                    fs.Add(t);
            }

            return fs;
        }

        private static AttributeFacet GetAttribute(XElement elem)
        {
            AttributeFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                var subname = sub.Name.LocalName.ToLowerInvariant();
                switch (subname)
                {
                    case "name":
                        {
                            ret ??= new AttributeFacet();
                            ret.AttributeName = sub.Value;
                            break;
                        }

                    case "value":
                        ret ??= new AttributeFacet();
                        ret.AttributeValue = GetConstraint(sub);
                        break;
                }
            }
            
            foreach (var sub in elem.Attributes())
            {
                var subname = sub.Name.LocalName.ToLowerInvariant();
                switch (subname)
                {
                    case "location":
                        {
                            ret.Location = sub.Value;
                            break;
                        }
                }
            }
            
            return ret;
        }

        private static IfcClassificationFacet GetClassification(XElement elem)
        {
            IfcClassificationFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                if (IsBaseEntity(sub))
                {
                    ret ??= new IfcClassificationFacet();
                    GetBaseEntity(sub, ret);
                }
                else if (sub.Name.LocalName == "system")
                {
                    ret ??= new IfcClassificationFacet();
                    ret.ClassificationSystem = GetConstraint(sub);
                    // classification has href attribute under system
                    foreach (var attribute in sub.Attributes())
                    { 
                        if (attribute.Name.LocalName == "href")
						{
                            ret.ClassificationSystemHref = attribute.Value;
						}
                    }
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret ??= new IfcClassificationFacet();
                    ret.Identification = GetConstraint(sub);
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret ??= new IfcClassificationFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret ??= new IfcClassificationFacet();
                    ret.Location = attribute.Value;
                }
                else
                {
                    Debug.WriteLine($"skipping attribute {attribute.Name}");
                }
            }
            return ret;
        }



        private static void GetBaseEntity(XElement sub, FacetBase ret)
        {
            if (sub.Name.LocalName == "instructions")
                ret.Instructions = sub.Value;
        }

        private static bool IsBaseEntity(XElement sub)
        {
            switch (sub.Name.LocalName)
            {
                case "instructions":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsBaseAttribute(XAttribute attribute)
        {
            switch (attribute.Name.LocalName)
            {
                case "href":
                case "location":
                case "use":
                    return true;
                default:
                    return false;
            }
        }

        private static void GetBaseAttribute(XAttribute attribute, FacetBase ret)
        {
            if (attribute.Name.LocalName == "href")
                ret.Uri = attribute.Value;
            else if (attribute.Name.LocalName == "location")
                ret.Location = attribute.Value;
            else if (attribute.Name.LocalName == "use")
                ret.Use= attribute.Value;
        }


        private const bool defaultSubTypeInclusion = false;

        private static IfcTypeFacet GetEntity(XElement elem)
        {
            IfcTypeFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                var locName = sub.Name.LocalName.ToLowerInvariant();
                switch (locName)
                {
                    case "name":
                        ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                        // todo: dealing with uncomprehensible v0.5 values
                        // see: https://github.com/buildingSMART/IDS/blob/7903eae20127c10b52cd37abf42a7cd7c2bcf973/Development/0.5/IDS_random_example_04.xml#L12-L18
                        if (string.IsNullOrEmpty(sub.Value))
                        {
                            ret.IfcType = GetFirstString(sub);
                        }
                        else
                            ret.IfcType = sub.Value;
                        break;
                    case "predefinedtype":
                        ret ??= new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                        ret.PredefinedType = sub.Value;
                        break;
                    default:
                        Debug.WriteLine($"skipping element {locName}");
                        break;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                Debug.WriteLine($"skipping attribute {attribute.Name}");
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
                            return val.Value;
                        break;
                    }
            }
            foreach (var sub2 in sub.Elements())
            {
                var subS = GetFirstString(sub2);
                if (!string.IsNullOrEmpty(subS))
                    return subS;
            }
            return "";
        }
    }
}