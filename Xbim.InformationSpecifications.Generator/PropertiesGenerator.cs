using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Properties;

namespace Xbim.InformationSpecifications.Generator
{
    public class PropertiesGenerator
    {
        /// <summary>
        /// Computes the GetPropertiesIFC2x3 and GetPropertiesIFC4 of the PropertySetInfo.Generated.cs file 
        /// Depends on the Xbim.Properties assembly.
        /// </summary>
        public static string Execute()
        {
            var source = stub;
            var schemas = new[] { Xbim.Properties.Version.IFC2x3, Xbim.Properties.Version.IFC4 };

            foreach (var schema in schemas)
            {
                var sb = new StringBuilder();
                var propertyDefinitions = new Definitions<PropertySetDef>(schema);
                propertyDefinitions.LoadAllDefault();
                foreach (var set in propertyDefinitions.DefinitionSets)
                {
                    var classes = set.ApplicableClasses.Select(x => x.ClassName).ToArray();
                    var richProp = new List<string>();
                    foreach (var prop in set.PropertyDefinitions)
                    {
                        string def = "";
                        if (!string.IsNullOrWhiteSpace(prop.Definition))
                        {
                            def = $" {{ Definition = \"{FixForCode(prop.Definition)}\"}}";
                        }
                        if (prop.PropertyType.PropertyValueType is TypePropertySingleValue singleV)
                        {
                            var t = $"new SingleValuePropertyType(\"{prop.Name}\", \"{singleV.DataType.Type}\"){def}";
                            richProp.Add(t);
                        }
                        else if (prop.PropertyType.PropertyValueType is TypePropertyBoundedValue range)
                        {
                            // todo: this case would have a range for value, that we are ignoring.
                            var t = $"new SingleValuePropertyType(\"{prop.Name}\", \"{range.DataType.Type}\"){def}";
                            richProp.Add(t);
                        }
                        else if (prop.PropertyType.PropertyValueType is TypePropertyEnumeratedValue enumV)
                        {
                            if (enumV.ConstantList.Any())
                            {
                                throw new Exception("Not implemented data structure.");
                            }
                            else
                            {
                                richProp.Add($"new EnumerationPropertyType(\"{prop.Name}\", {NewStringArray(enumV.EnumList.Items)} ){def}");
                            }
                        }
                        else if (prop.PropertyType.PropertyValueType is TypePropertyReferenceValue refP)
                        {
                            // reference values do not have a testing method, and are ignored
                            // e.g. they would refer to an IfcMaterial or IfcPerson
                        }
                        else if (prop.PropertyType.PropertyValueType is TypePropertyListValue lst)
                        {
                            // list values do not have a testing method, and are ignored
                            // e.g. they could have multiple values for the property
                        }
                        else if (prop.PropertyType.PropertyValueType is TypeSimpleProperty simple)
                        {
                            // todo: maybe some SimpleProperties can be added to the list, this would need a review
                            // list values do not have a testing method, and are ignored
                            // e.g. they could have multiple values for the property
                        }
                        else if (prop.PropertyType.PropertyValueType is TypePropertyTableValue table)
                        {
                            // list values do not have a testing method, and are ignored
                            // e.g. they could have multiple values for the property
                        }
                        else if (prop.PropertyType.PropertyValueType is TypeComplexProperty cmplex)
                        {
                            // complex types do not have a testing method, and are ignored
                            // e.g. they could have multiple values for the property
                        }
                        else
                        {
                            var t = $"new NamedPropertyType(/* {prop.PropertyType.PropertyValueType?.GetType().Name} */\"{prop.Name}\"){def}";
                            richProp.Add(t);
                        }


                        // prop.PropertyType
                    }
                    var cArr = NewStringArray(classes);

                    var rpArr = NewArray("IPropertyTypeInfo", richProp);

                    // sb.AppendLine($@"			// {string.Join(",", classes)}");
                    // sb.AppendLine($@"			yield return new PropertySetInfo(""{set.Name}"", {pArr}, {cArr});");
                    sb.AppendLine($@"			yield return new PropertySetInfo(""{set.Name}"", {rpArr}, {cArr});");
                }
                source = source.Replace($"<PlaceHolder{schema}>\r\n", sb.ToString());
            }
            // context.AddSource("generated2.cs", source);
            return source;
        }

        private static string FixForCode(string definition)
        {
            var tmp = definition;
            tmp = tmp.Replace("\t", "\\t");
            tmp = tmp.Replace("\r", "\\r");
            tmp = tmp.Replace("\n", "\\n");
            tmp = tmp.Replace("\"", "\\\"");
            return tmp;
        }

        private static string NewStringArray(IEnumerable<string> values)
        {
            var strippedValues = values.Select(x => x.Trim(' ', '\r', '\n'));
            return @$"new [] {{ ""{string.Join("\", \"", strippedValues)}"" }}";
        }

        private static string NewArray(string type, IEnumerable<string> values)
        {
            return @$"new {type}[] {{ {string.Join(", ", values)} }}";
        }

        private const string stub = @"// generated via source generation from xbim.xids.generator

using System.Collections.Generic;

namespace Xbim.InformationSpecifications.Helpers
{
	public partial class PropertySetInfo
	{
		static IEnumerable<PropertySetInfo> GetPropertiesIFC2x3()
		{
<PlaceHolderIFC2x3>
		}

		private static IEnumerable<PropertySetInfo> GetPropertiesIFC4()
		{
<PlaceHolderIFC4>
		}
	}
}
";
    }
}