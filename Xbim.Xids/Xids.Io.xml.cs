using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Xbim.Xids
{
	public partial class Xids
	{
        public static void ExportBuildingSmartIDS(string fileName)
		{
            throw new NotImplementedException();
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
				var grp = new RequirementsGroup();
				ret.RequirementGroups.Add(grp);

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

		private static void AddSpecification(Xids ids, RequirementsGroup destGroup, XElement spec)
        {
            var req = new Requirement(ids);
            destGroup.Requirements.Add(req);
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
                if (sub.Name.LocalName == "value")
                {
                    ret = ret ?? new MaterialFacet();
                    ret.Value = GetConstraint(sub);
                }
                else if (sub.Name.LocalName == "instructions")
                {
                    // todo: clarify what is the expected location of the instructions field... 
                }
                else
                {

                }
            }
			foreach (var attribute in elem.Attributes())
			{
                if (attribute.Name.LocalName == "location")
                {
                    ret = ret ?? new MaterialFacet();
                    ret.Location = attribute.Value;
                }
                else if (attribute.Name.LocalName == "href")
                {
                    ret = ret ?? new MaterialFacet();
                    if (Uri.TryCreate(attribute.Value, UriKind.RelativeOrAbsolute, out var created))
                        ret.Uri = created;
                    else
                    {
                        // todo: raise warning.
                    }
                }
                else if (attribute.Name.LocalName == "use" ||
                    attribute.Name.LocalName == "optional" 
                    )
                {
                    // todo: raise warning.
                }
                else
				{
				}
            }
            return ret;
        }

        private static IFacet GetProperty(XElement elem)
		{
			IfcPropertyFacet ret = null;
            foreach (var sub in elem.Elements())
            {
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
                    var href = sub.Attribute("href");
                    if (href != null)
                    {
                        if (Uri.TryCreate(href.Value, UriKind.RelativeOrAbsolute, out var created))
                            ret.Uri = created;
                        else
                        {
                            // todo: raise warning.
                        }
                    }
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.PropertyValue = GetConstraint(sub);
                }
                else if (sub.Name.LocalName == "instructions")
                {
                    // todo: clarify what is the expected location of the instructions field... 
                }
                else 
                {
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (attribute.Name.LocalName == "location")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    ret.Location = attribute.Value;
                }
                else if (attribute.Name.LocalName == "href")
                {
                    ret = ret ?? new IfcPropertyFacet();
                    if (Uri.TryCreate(attribute.Value, UriKind.RelativeOrAbsolute, out var created))
                        ret.Uri = created;
                    else
					{
                        // todo: raise warning.
					}
                }
                else
                {

                }

            }
            return ret;
        }

		private static Value GetConstraint(XElement elem)
		{
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            var restriction = elem.Element(ns + "restriction");
            if (restriction == null)
                return null;
            Type t = null;
            var bse = restriction.Attribute("base");
            if (bse != null && bse.Value != null)
            {
                if (bse.Value == "xs:string")
                    t = typeof(string);
                else if (bse.Value == "xs:integer")
                    t = typeof(int);
                else if (bse.Value == "xs:boolean")
                    t = typeof(bool);
                else if (bse.Value == "xs:double")
                    t = typeof(double);
                else if (bse.Value == "xs:date")
                    t = typeof(DateTime);
                else if (bse.Value == "xs:time")
                    t = typeof(DateTime);
                else if (bse.Value == "xs:anyURI")
                    t = typeof(string);
                // todo: 2021: evaluate more types?
                // see https://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes
            }

            // we prepare the different possible scenarios, but then check in the end that the 
            // xml encoutnered is solid.
            //
            List<object> enumeration = null;
            RangeConstraint range = null;
            PatternConstraint pc = null;

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
                        pc = new PatternConstraint() { Pattern = val.Value };
                    }
                }
                else
                {

                }
            }
            // check that the temporary variable are coherent with valid value
            var count = (enumeration != null) ? 1 : 0;
            count += (range != null) ? 1 : 0;
            count += (pc != null) ? 1 : 0;
            if (count != 1)
                return null;
            if (enumeration != null)
			{
				var ret = new Value(Value.Resolve(t))
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
				var ret = new Value(Value.Resolve(t))
				{
					AcceptedValues = new List<IValueConstraint>() { range }
				};
                return ret;
            }
            if (pc!=null)
			{
                var ret = new Value(Value.Resolve(t))
                {
                    AcceptedValues = new List<IValueConstraint>() { pc }
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
                if (sub.Name.LocalName == "system")
                {
                    if (ret == null)
                        ret = new IfcClassificationFacet();
                    ret.ClassificationSystem = sub.Value;

                    var href = sub.Attribute("href");
                    if (href != null)
                    {
                        if (Uri.TryCreate(href.Value, UriKind.RelativeOrAbsolute, out var created))
                            ret.Uri = created;
                        else
                        {
                            // todo: raise warning.
                        }
                    }
                }
                else if (sub.Name.LocalName == "value")
                {
                    if (ret == null)
                        ret = new IfcClassificationFacet();
                    ret.Node = sub.Value;
                }
            }
            foreach (var attribute in elem.Attributes())
            {
                if (attribute.Name.LocalName == "location")
				{
                    ret = ret ?? new IfcClassificationFacet();
                    ret.Location = attribute.Value;
				}
                else
				{
                    
				}


            }
            return ret;
        }

        private const bool defaultSubTypeInclusion = false;

        private static IfcTypeFacet GetEntity(XElement elem)
		{
            IfcTypeFacet ret = null;
            foreach (var sub in elem.Elements())
            {
                if (sub.Name.LocalName == "name")
                {
                    if (ret == null)
                        ret = new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                    ret.IfcType = sub.Value;
                }
                else if (sub.Name.LocalName == "predefinedtype")
                {
                    if (ret == null)
                        ret = new IfcTypeFacet() { IncludeSubtypes = defaultSubTypeInclusion };
                    ret.PredefinedType = sub.Value;
                }
            }
            foreach (var attribute in elem.Attributes())
            {

            }
            return ret;
        }
	}
}
