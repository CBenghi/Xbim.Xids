using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Xbim.IDS
{
	public partial class Ids
	{
		public static Ids FromBuildingSmartIDS(string fileName)
		{
			if (!File.Exists(fileName))
				return null;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            var ret = new Ids();
            using (var stream = File.OpenRead(fileName))
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    Debug.WriteLine(reader.NodeType);
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "specification")
					{
                        var destGroup = new RequirementsCollection();
                        ret.RequirementGroups.Add(destGroup);
                        AddSpecfication(ret, destGroup, reader);
					}
                }
            }
            return ret;
        }

        private static void AddSpecfication(Ids ids, RequirementsCollection destGroup, XmlReader reader)
        {
            var req = new Requirement();
            destGroup.Requirements.Add(req);
            req.Name = reader.GetAttribute("name");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "applicability")
                {
                    AddSelection(req, reader);
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "requirements")
                {
                    AddExpectation(req, reader);
                }
            }
        }

		private static void AddExpectation(Requirement req, XmlReader reader)
		{
			
		}

		private static void AddSelection(Requirement e, XmlReader reader)
		{
            e.ModelSubset = new ModelPart();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "entity")
                {
                    IfcTypeQuery t = GetEntity(reader);
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "classification")
                {
                    IfcClassificationQuery t = GetClassification(reader);
                }
            }
        }

		private static IfcClassificationQuery GetClassification(XmlReader reader)
		{
            IfcClassificationQuery ret = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "system")
                {
                    if (ret != null)
                        ret = new IfcClassificationQuery();
                    ret.ClassificationSystem = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "value")
                {
                    if (ret != null)
                        ret = new IfcClassificationQuery();
                    ret.Node = reader.Value;
                }
            }
            return ret;
        }

		private static IfcTypeQuery GetEntity(XmlReader reader)
		{
            IfcTypeQuery ret = null;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "name")
                {
                    ret = new IfcTypeQuery();
                    ret.IfcType = reader.Value;
                }
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "name")
                    return ret;
            }
            return null;
        }
	}
}
