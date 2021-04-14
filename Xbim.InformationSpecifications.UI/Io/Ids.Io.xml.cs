using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Xbim.IDS
{
	public partial class IDS
	{
        public static void ToBuildingSmartIDS(string fileName)
		{

		}

        public static IDS FromBuildingSmartIDS(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
            var main = XElement.Parse(File.ReadAllText(fileName));

            if (main.Name.LocalName == "ids")
            {
                var ret = new IDS();
                var grp = new RequirementsCollection();
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

        private static void AddSpecification(IDS ids, RequirementsCollection destGroup, XElement spec)
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
                    AddApplicability(req, elem);
                }
                else if (elem.Name.LocalName == "requirements")
                {
                    AddRequirements(req, elem);
                }
            }
        }

       

		private static ExpectationFacet GetProperty(XElement elem)
		{
            HasProperty ret = new HasProperty();
            foreach (var sub in elem.Elements())
            {
                if (sub.Name.LocalName == "propertyset")
                {
                    ret.PropertySetName = sub.Value;
                }
                else if (
                    sub.Name.LocalName == "name"
                    ||
                    sub.Name.LocalName == "property"
                    )
                {
                    ret.PropertyName = sub.Value;
                    var href = sub.Attribute("href");
                    if (href != null)
					{
                        ret.Uri = new Uri(href.Value);
					}
                }
                else if (sub.Name.LocalName == "value")
                {
                    ret.PropertyConstraint = GetConstraint(sub);
                }
            }
            return ret;
        }

		private static IValueConstraint GetConstraint(XElement elem)
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

            IValueConstraint ret = null; 
            foreach (var sub in restriction.Elements())
            {
                if (sub.Name.LocalName == "enumeration")
                {
                    if (!(ret is OneOfConstraint))
                        ret = new OneOfConstraint();
                    var enumV = ret as OneOfConstraint;
                    var val = sub.Attribute("value");
                    if (val != null)
                    {
                        var tVal = GetValue(val.Value, t);
                        if (tVal != null)
                            enumV.AddOption(tVal);
                    }
                }
                else if (
                    sub.Name.LocalName == "minInclusive"
                    ||
                    sub.Name.LocalName == "minExclusive"
                    )
                {
                    if (!(ret is RangeConstraint))
                        ret = new RangeConstraint();
                    RangeConstraint c = ret as RangeConstraint;
                    var val = sub.Attribute("value");
                    if (val != null)
                    {
                        var tVal = GetValue(val.Value, t);
                        c.MinValue = tVal;
                        c.MinInclusive = sub.Name.LocalName == "minInclusive";
                    }
                }
                else if (
                    sub.Name.LocalName == "maxInclusive"
                    ||
                    sub.Name.LocalName == "maxExclusive"
                    )
                {
                    if (!(ret is RangeConstraint))
                        ret = new RangeConstraint();
                    RangeConstraint c = ret as RangeConstraint;
                    var val = sub.Attribute("value");
                    if (val != null)
                    {
                        var tVal = GetValue(val.Value, t);
                        c.MaxValue = tVal;
                        c.MaxInclusive = sub.Name.LocalName == "maxInclusive";
                    }
                }
            }
            return ret;
		}

		private static IValueConstraint GetValue(string value, Type t)
		{
            if (t == typeof(string))
                return new ValueConstraint(value);
            if (t == typeof(int))
            {
                if (int.TryParse(value, out int val))
                {
                    return new ValueConstraint(val);
                }
                return null;
            }
            return null;
        }

        private static void AddRequirements(Requirement req, XElement elem)
        {
            List<ExpectationFacet> fs = new List<ExpectationFacet>();
            foreach (var sub in elem.Elements())
            {
                ExpectationFacet t = null;
                if (sub.Name.LocalName == "property")
                {
                    t = GetProperty(sub);
                }
                else if (sub.Name.LocalName == "classification")
                {
                    // t = GetClassification(elem);
                }
                if (t != null)
                {
                    if (t.Validate())
                        fs.Add(t);
                }
            }
            if (fs.Any())
            {
                req.SetExpectations(fs);
            }
        }

        private static void AddApplicability(Requirement e, XElement elem)
		{
            List<IFilter> fs = new List<IFilter>();
            foreach (var sub in elem.Elements())
            {
                IFilter t = null;
                if (sub.Name.LocalName == "entity")
                {
                    t = GetEntity(sub);
                }
                else if (sub.Name.LocalName == "classification")
                {
                    t = GetClassification(sub);
                }
                if (t != null)
                    fs.Add(t);
            }
            if (fs.Any())
			{
                e.SetFilters(fs);
			}
        }

		private static IfcClassificationQuery GetClassification(XElement elem)
		{
            IfcClassificationQuery ret = null;
            foreach (var sub in elem.Elements())
            {
                if (sub.Name.LocalName == "system")
                {
                    if (ret == null)
                        ret = new IfcClassificationQuery();
                    ret.ClassificationSystem = sub.Value;
                }
                else if (sub.Name.LocalName == "value")
                {
                    if (ret == null)
                        ret = new IfcClassificationQuery();
                    ret.Node = sub.Value;
                }
            }
            return ret;
        }

		private static IfcTypeQuery GetEntity(XElement elem)
		{
            foreach (var sub in elem.Elements())
            {
                if (sub.Name.LocalName == "name")
                {
					return new IfcTypeQuery
					{
						IfcType = sub.Value,
						IncludeSubtypes = false
					};
					
                }
            }
            return null;
        }
	}
}
