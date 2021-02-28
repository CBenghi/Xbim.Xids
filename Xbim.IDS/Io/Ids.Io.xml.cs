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
	public partial class Ids
	{
        public static void ToBuildingSmartIDS(string fileName)
		{

		}

        public static Ids FromBuildingSmartIDS(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
            var main = XElement.Parse(File.ReadAllText(fileName));

            if (main.Name == "specifications")
            {
                var ret = new Ids();
                var grp = new RequirementsCollection();
                ret.RequirementGroups.Add(grp);

                foreach (var sub in main.Elements())
                {
                    if (sub.Name == "specification")
                    {
                        AddSpecfication(ret, grp, sub);
                    }
                }

                AddSpecfication(ret, grp, main);
                return ret;
            }
            else if (main.Name == "specification")
			{
                var ret = new Ids();
                var grp = new RequirementsCollection();
                ret.RequirementGroups.Add(grp);
                AddSpecfication(ret, grp, main);
                return ret;
            }
            return null;
        }

        private static void AddSpecfication(Ids ids, RequirementsCollection destGroup, XElement spec)
        {
            var req = new Requirement(ids);
            destGroup.Requirements.Add(req);
            var nm = spec.Attribute("name");
            if (nm != null)
                req.Name = nm.Value;
			foreach (var elem in spec.Elements())
			{
                if (elem.Name == "applicability")
                {
                    AddSelection(req, elem);
                }
                else if (elem.Name == "requirements")
                {
                    AddExpectation(req, elem);
                }
            }
        }

        private static void AddExpectation(Requirement req, XElement elem)
        {
            List<ExpectationFacet> fs = new List<ExpectationFacet>();
            foreach (var sub in elem.Elements())
            {
                ExpectationFacet t = null;
                if (sub.Name == "property")
                {
                    t = GetProperty(sub);
                }
                else if (sub.Name == "classification")
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
                req.AddExpectations(fs);
            }
        }

		private static ExpectationFacet GetProperty(XElement elem)
		{
            HasProperty ret = new HasProperty();
            foreach (var sub in elem.Elements())
            {
                if (sub.Name == "propertyset")
                {
                    ret.PropertySetName = sub.Value;
                }
                else if (sub.Name == "property")
                {
                    ret.PropertyName = sub.Value;
                    var href = sub.Attribute("href");
                    if (href != null)
					{
                        ret.Uri = new Uri(href.Value);
					}
                }
                else if (sub.Name == "value")
                {
                    ret.PropertyConstraint = GetConstraint(sub);
                }
            }
            return ret;
        }

		private static IValueConstraint GetConstraint(XElement elem)
		{
            var restriction = elem.Element("restriction");
            if (restriction == null)
                return null;
            Type t = null;
            var bse = restriction.Attribute("base");
            if (bse != null && bse.Value != null)
            {
                if (bse.Value == "string")
                    t = typeof(string);
                else if (bse.Value == "integer")
                    t = typeof(int);
                else if (bse.Value == "boolean")
                    t = typeof(bool);
                else if (bse.Value == "double")
                    t = typeof(double);
                else if (bse.Value == "date")
                    t = typeof(DateTime);
                else if (bse.Value == "time")
                    t = typeof(DateTime);
                else if (bse.Value == "anyURI")
                    t = typeof(string);
                // todo: 2021: evaluate more types?
                // see https://www.w3.org/TR/xmlschema-2/#built-in-primitive-datatypes
            }

            IValueConstraint ret = null; 
            foreach (var sub in restriction.Elements())
            {
                if (sub.Name == "enumeration")
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
            }
            return null;
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

		private static void AddSelection(Requirement e, XElement elem)
		{
            List<IFilter> fs = new List<IFilter>();
            foreach (var sub in elem.Elements())
            {
                IFilter t = null;
                if (sub.Name == "entity")
                {
                    t = GetEntity(sub);
                }
                else if (sub.Name == "classification")
                {
                    t = GetClassification(sub);
                }
                if (t != null)
                    fs.Add(t);
            }
            if (fs.Any())
			{
                e.ModelSubset = new ModelPart();
				foreach (var item in fs)
				{
                    e.ModelSubset.Items.Add(item);
                }
			}
        }

		private static IfcClassificationQuery GetClassification(XElement elem)
		{
            IfcClassificationQuery ret = null;
            foreach (var sub in elem.Elements())
            {
                if (sub.Name == "system")
                {
                    if (ret == null)
                        ret = new IfcClassificationQuery();
                    ret.ClassificationSystem = sub.Value;
                }
                else if (sub.Name == "value")
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
                if (sub.Name == "name")
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
