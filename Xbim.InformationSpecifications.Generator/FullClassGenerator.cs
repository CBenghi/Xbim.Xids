﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Metadata;

namespace Xbim.InformationSpecifications.Generator
{
    public class FullClassAttributeGenerator
    {
        /// <summary>
        /// SchemaInfo.GeneratedClass.cs
        /// </summary>
        public static string Execute()
        {
            var source = stub;
            var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };

            var classNames = new List<string>();
            var attNames = new List<string>();


            foreach (var schema in schemas)
            {
                System.Reflection.Module module = null;
                if (schema == Properties.Version.IFC2x3)
                    module = (typeof(Ifc2x3.Kernel.IfcProduct)).Module;
                else if (schema == Properties.Version.IFC4)
                    module = (typeof(Ifc4.Kernel.IfcProduct)).Module;
                var metaD = ExpressMetaData.GetMetadata(module);


                foreach (var daType in metaD.Types())
                {
                    // just class names
                    if (!classNames.Contains(daType.Name))
                    {
                        classNames.Add(daType.Name);
                    }

                    // Enriching schema with attribute names
                    var thisattnames = daType.Properties.Values.Select(x => x.Name);
                    foreach (var attributeName in thisattnames)
                    {
                        if (!attNames.Contains(attributeName))
                        {
                            attNames.Add(attributeName);
                        }
                    }
                }
            }
            var sbClasses = new StringBuilder();
            var sbAtts = new StringBuilder();
            int i = 0;
            foreach (var clNm in classNames.OrderBy(x => x))
            {
                sbClasses.AppendLine($"\t\t\t\tyield return \"{clNm}\"; // {++i}");
            }
            var done = false;
            i = 0;
            foreach (var atNm in attNames.OrderBy(x => x).ToList())
            {
                sbAtts.AppendLine($"\t\t\t\tyield return \"{atNm}\"; // {++i}");
            }
            if (!done)
            {

            }
            source = source.Replace($"<PlaceHolderClasses>\r\n", sbClasses.ToString());
            source = source.Replace($"<PlaceHolderAtts>\r\n", sbAtts.ToString());
            return source;
        }

        private const string stub = @"// generated code via xbim.xids.generator, any changes made directly here will be lost

using System;
using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
    public partial class SchemaInfo
    {
        /// <summary>
        /// The names of classes across all schemas.
        /// </summary>
        public static IEnumerable<string> AllSchemasClassNames
        {
            get
            {
<PlaceHolderClasses>
            }
        }

        /// <summary>
        /// The names of all attributes across all schemas.
        /// </summary>
        public static IEnumerable<string> AllSchemasAttributeNames
        {
            get
            {
<PlaceHolderAtts>
            }
        }
    }
}

";
    }
}