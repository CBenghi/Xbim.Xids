using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Xbim.Xids
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
            xmlWriter.WriteStartElement("ids", @"http://standards.buildingsmart.org/IDS");
            // writer.WriteAttributeString("xsi", "xmlns", @"http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xs", null, "http://www.w3.org/2001/XMLSchema");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", null, "http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids.xsd");

            foreach (var requirement in AllSpecifications())
            {
                ExportBuildingSmartIDS(requirement, xmlWriter);
            }

            // writer.WriteString("text");

            // writer.WriteProcessingInstruction("pName", "pValue");
            xmlWriter.WriteElementString("info", null);
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        private void ExportBuildingSmartIDS(Specification requirement, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("specification");

            xmlWriter.WriteAttributeString("name", requirement.Name);
            xmlWriter.WriteStartElement("applicability");
            foreach (var item in requirement.Applicability.Facets)
            {
                ExportBuildingSmartIDS(item, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("requirements");
            foreach (var item in requirement.Requirement.Facets)
            {
                ExportBuildingSmartIDS(item, xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        private void ExportBuildingSmartIDS(IFacet item, XmlWriter xmlWriter)
        {
            if (item is IfcTypeFacet tf)
            {
                xmlWriter.WriteStartElement("entity");
                if (!string.IsNullOrWhiteSpace(tf.IfcType))
                {
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(tf.IfcType);
                    xmlWriter.WriteEndElement();
                }
                if (!string.IsNullOrWhiteSpace(tf.PredefinedType))
                {
                    xmlWriter.WriteStartElement("predefinedtype");
                    xmlWriter.WriteString(tf.PredefinedType);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
            else if (item is IfcClassificationFacet cf)
            {
                xmlWriter.WriteStartElement("classification");
                WriteLocationAttributes(cf, xmlWriter); // attribute
                WriteValue(cf.Identification, xmlWriter);
                Dictionary<string, string> attributes = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace( cf.ClassificationSystemHref))
				{
                    attributes.Add("href", cf.ClassificationSystemHref);
				}
                WriteValue(cf.ClassificationSystem, xmlWriter, "system", attributes);
                WriteLocationElements(cf, xmlWriter);

                xmlWriter.WriteEndElement();
            }
            else if (item is IfcPropertyFacet pf)
            {
                xmlWriter.WriteStartElement("property");
                WriteLocationAttributes(pf, xmlWriter);
                if (!string.IsNullOrWhiteSpace(pf.PropertySetName))
                {
                    xmlWriter.WriteStartElement("propertyset");
                    xmlWriter.WriteString(pf.PropertySetName);
                    xmlWriter.WriteEndElement();
                }
                if (!string.IsNullOrWhiteSpace(pf.PropertyName))
                {
                    xmlWriter.WriteStartElement("name");
                    xmlWriter.WriteString(pf.PropertyName);
                    xmlWriter.WriteEndElement();
                }
                WriteValue(pf.PropertyValue, xmlWriter);
                WriteLocationElements(pf, xmlWriter);
                xmlWriter.WriteEndElement();
            }
            else if (item is MaterialFacet mf)
            {
                xmlWriter.WriteStartElement("material");
                WriteLocationAttributes(mf, xmlWriter);
                WriteValue(mf.Value, xmlWriter);
                WriteLocationElements(mf, xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }

		

		private void WriteValue(Value value, XmlWriter xmlWriter, string name = "value", Dictionary<string, string> attributes = null)
        {
            if (value == null)
                return;
            xmlWriter.WriteStartElement(name);
            if (attributes != null)
            {
                foreach (var att in attributes)
                {
                    xmlWriter.WriteAttributeString(att.Key, att.Value);
                }
            }
            if (value.IsSingleUndefinedExact(out string exact))
            {
                xmlWriter.WriteString(exact);                
            }
            else if (value.AcceptedValues != null)
            {
                xmlWriter.WriteStartElement("restriction", @"http://www.w3.org/2001/XMLSchema");
                if (value.BaseType != TypeName.Undefined)
                {
                    var val = Value.GetXsdTypeString(value.BaseType);
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
                return null;
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
                    if (sub.Name.LocalName == "specification")
                    {
                        AddSpecification(ret, grp, sub);
                    }
                }
                return ret;
            }
            return null;
        }

        private static void AddSpecification(Xids ids, SpecificationsGroup destGroup, XElement spec)
        {
            var req = new Specification(ids);
            destGroup.Specifications.Add(req);
            var nm = spec.Attribute("name");
            if (nm != null)
                req.Name = nm.Value;
            foreach (var elem in spec.Elements())
            {
                if (elem.Name.LocalName == "applicability")
                {
                    var fs = GetFacets(elem);
                    if (fs.Any())
                    {
                        req.SetFilters(fs);
                    }
                }
                else if (elem.Name.LocalName == "requirements")
                {
                    var fs = GetFacets(elem);
                    if (fs.Any())
                    {
                        req.SetExpectations(fs);
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
                    ret = ret ?? new MaterialFacet();
                    GetBaseEntity(sub, ret);
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret = ret ?? new MaterialFacet();
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
                    ret = ret ?? new MaterialFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret = ret ?? new MaterialFacet();
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
                    ret = ret ?? new IfcPropertyFacet();
                    GetBaseEntity(sub, ret);
                }
                if (sub.Name.LocalName == "propertyset")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.PropertySetName = sub.Value;
                }
                else if (
                    sub.Name.LocalName == "property" ||
                    sub.Name.LocalName == "name"
                    )
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.PropertyName = sub.Value;
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.PropertyValue = GetConstraint(sub);
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
                    ret = ret ?? new IfcPropertyFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.Location = attribute.Value;
                }
                else
                {
                    Debug.WriteLine($"skipping attribute {attribute.Name}");
                }
            }
            return ret;
        }

        private static Value GetConstraint(XElement elem)
        {
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            var restriction = elem.Element(ns + "restriction");
            if (restriction == null)
            {
                // get the textual content as a fixed 
                var content = elem.Value;
                var tc = Value.SingleUndefinedExact(content);
                return tc;
            }
            TypeName t = TypeName.Undefined;
            var bse = restriction.Attribute("base");
            if (bse != null && bse.Value != null)
            {
                var tval = bse.Value;
                t = Value.GetNamedTypeFromXsd(tval);
            }

            // we prepare the different possible scenarios, but then check in the end that the 
            // xml encoutnered is solid.
            //
            List<object> enumeration = null;
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
                        var tVal = Value.GetObject(val.Value, t);
                        if (tVal != null)
                        {
                            enumeration = enumeration ?? new List<object>();
                            enumeration.Add(tVal);
                        }
                    }
                }
                else if (
                    sub.Name.LocalName == "minInclusive"
                    ||
                    sub.Name.LocalName == "minExclusive"
                    )
                {
                    var val = Value.GetObject(sub.Attribute("value")?.Value, t);
                    if (val != null && val is IComparable cmp)
                    {
                        range = range ?? new RangeConstraint();
                        range.MinValue = cmp;
                        range.MinInclusive = sub.Name.LocalName == "minInclusive";
                    }
                    else
                    {
                        // todo: 2021: log error in conversion
                    }
                }
                else if (
                    sub.Name.LocalName == "maxInclusive"
                    ||
                    sub.Name.LocalName == "maxExclusive"
                    )
                {
                    var val = Value.GetObject(sub.Attribute("value")?.Value, t);
                    if (val != null && val is IComparable cmp)
                    {
                        range = range ?? new RangeConstraint();
                        range.MaxValue = cmp;
                        range.MaxInclusive = sub.Name.LocalName == "maxInclusive";
                    }
                    else
                    {
                        // todo: 2021: log error in conversion
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
                        structure = structure ?? new StructureConstraint();
                        structure.MinLength = ival;
                    }
                }
                else if (sub.Name.LocalName == "maxLength")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure = structure ?? new StructureConstraint();
                        structure.MaxLength = ival;
                    }
                }
                else if (sub.Name.LocalName == "length")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure = structure ?? new StructureConstraint();
                        structure.Length = ival;
                    }
                }
                else if (sub.Name.LocalName == "totalDigits")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure = structure ?? new StructureConstraint();
                        structure.TotalDigits = ival;
                    }
                }
                else if (sub.Name.LocalName == "fractionDigits")
                {
                    var val = sub.Attribute("value");
                    if (val != null && int.TryParse(val.Value, out var ival))
                    {
                        structure = structure ?? new StructureConstraint();
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
                var ret = new Value(t)
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
                var ret = new Value(t)
                {
                    AcceptedValues = new List<IValueConstraint>() { range }
                };
                return ret;
            }
            if (patternc != null)
            {
                var ret = new Value(t)
                {
                    AcceptedValues = new List<IValueConstraint>() { patternc }
                };
                return ret;
            }
            if (structure != null)
            {
                var ret = new Value(t)
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
                if (sub.Name.LocalName == "entity")
                {
                    t = GetEntity(sub);
                }
                else if (sub.Name.LocalName == "classification")
                {
                    t = GetClassification(sub);
                }
                else if (sub.Name.LocalName == "property")
                {
                    t = GetProperty(sub);
                }
                else if (sub.Name.LocalName == "material")
                {
                    t = GetMaterial(sub);
                }
                else
                {

                }
                if (t != null)
                    fs.Add(t);
            }

            return fs;
        }



        private static IfcClassificationFacet GetClassification(XElement elem)
        {
            IfcClassificationFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                if (IsBaseEntity(sub))
                {
                    ret = ret ?? new IfcClassificationFacet();
                    GetBaseEntity(sub, ret);
                }
                else if (sub.Name.LocalName == "system")
                {
                    ret = ret ?? new IfcClassificationFacet();
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
                    ret = ret ?? new IfcClassificationFacet();
                    ret.Identification = GetConstraint(sub);
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (IsBaseAttribute(attribute))
                {
                    ret = ret ?? new IfcClassificationFacet();
                    GetBaseAttribute(attribute, ret);
                }
                else if (attribute.Name.LocalName == "location")
                {
                    ret = ret ?? new IfcClassificationFacet();
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
                if (sub.Name.LocalName == "name")
                {
                    ret = ret ?? new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                    ret.IfcType = sub.Value;
                }
                else if (sub.Name.LocalName == "predefinedtype")
                {
                    ret = ret ?? new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                    ret.PredefinedType = sub.Value;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                Debug.WriteLine($"skipping attribute {attribute.Name}");
            }
            return ret;
        }
    }
}