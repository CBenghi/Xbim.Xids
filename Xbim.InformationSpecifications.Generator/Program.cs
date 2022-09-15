﻿using System;
using System.IO;
using System.Xml;
using Xbim.InformationSpecifications.Generator.Measures;

namespace Xbim.InformationSpecifications.Generator
{
    class Program
    {
        internal static XmlDocument GetBuildingSmartSchemaXML()
        {
            var doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(@"Files\ids_09.xsd"));
            return doc;
        }

        public static void Main()
        {
            Console.WriteLine("Press `t` to generate full testfiles, any other key to continue with next steps of generation.");
            if (Console.ReadKey().Key == ConsoleKey.T)
            {
                // Whenever the schema changes
                // 1. get the latest files with the batch command
                // 2. execute the following function
                BuildingSmartSchema.GenerateFulltestFiles();
            }

            // wip
            var wip = false;
            if (wip)
            {
                var schemas = new[] {
                    Properties.Version.IFC2x3,
                    Properties.Version.IFC4
                };
                foreach (var schema in schemas)
                    Console.Write(ClassRelationTypes.Report(schema));
                return;
            }

            Console.WriteLine("Running code generation...");

            var study = false;
            var destPath = new DirectoryInfo(@"..\..\..\..\");
            // Initialization
            if (study)
            {
                Console.Write(IfcClassStudy.ReportMatchesToProperties());  // relevant classes preview
                                                                           // Console.Write(MeasureAutomation.Execute()); // measures and dimensional exponents
                return;
            }

            // depends on Xbim.Properties assembly
            Console.WriteLine("Running properties generation...");
            var tPropGen = PropertiesGenerator.Execute();
            string dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\PropertySetInfo.Generated.cs");
            File.WriteAllText(dest, tPropGen);

            // depends on ExpressMetaData and IfcClassStudy classes
            Console.WriteLine("Running class generation...");
            var tClassGen = ClassGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedClass.cs");
            File.WriteAllText(dest, tClassGen);

            // depends on documentation markdown
            Console.WriteLine("Ifc measure dictionary generation...");
            var tMeasures = MeasureAutomation.Execute_GenerateIfcMeasureDictionary();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.IfcMeasures.cs");
            File.WriteAllText(dest, tMeasures);

            if (false) // enums removed from schema
            {
                // depends on schema
                Console.WriteLine("Ifc ifcMeasure enum generation...");
                var tEnum = MeasureAutomation.Execute_GenerateIfcMeasureEnum();
                dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\Measures\Enums.cs");
                File.WriteAllText(dest, tEnum);
            }

            // depends on ExpressMetaData and IfcClassStudy classes
            Console.WriteLine("Running attributes generation...");
            var tAttGen = AttributesGenerator.Execute();
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedAttributes.cs");
            File.WriteAllText(dest, tAttGen);

            // depends on ExpressMetaData and IfcClassStudy classes
            var tRelTypeGen = ClassRelationTypes.Execute();
            Console.WriteLine("Running generation of class relationships...");
            dest = Path.Combine(destPath.FullName, @"Xbim.InformationSpecifications\Helpers\SchemaInfo.GeneratedRelTypes.cs");
            File.WriteAllText(dest, tRelTypeGen);

            // QA analysis 
            MeasureAutomation.Execute_CheckMeasureMetadata();

            if (false) // enums removed from schema
            {
                if (MeasureAutomation.Execute_CheckMeasureEnumeration())
                {
                    Message(ConsoleColor.Red, "Errors in measure helpers, try running again once, it might get fixed by code generation.");
                }
            }

            Message(ConsoleColor.DarkGreen, "Completed");
        }

        internal static void Message(ConsoleColor t, string msg)
        {
            var bkp = Console.ForegroundColor;
            Console.ForegroundColor = t;
            Console.WriteLine(msg);
            Console.ForegroundColor = bkp;
        }
    }
}
