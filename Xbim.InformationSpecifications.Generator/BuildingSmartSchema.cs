using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TextCopy;

namespace Xbim.InformationSpecifications.Generator
{
    internal class BuildingSmartSchema
    {
        /// <summary>
        /// Produces test files that allow to test all fields of the buildingSmart schema
        /// </summary>
        public static void GenerateFulltestFiles()
        {
            var schemaFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\ids_06.xsd";
            DirectoryInfo d = new DirectoryInfo(".");
            // Debug.WriteLine(d.FullName);
            OpenUrl("https://www.liquid-technologies.com/online-xsd-to-xml-converter");

            var fullSchema = File.ReadAllText(schemaFile);

            // take away optional schema stuff, make it as rich as possible
            //
            fullSchema = fullSchema.Replace("minOccurs=\"0\"", "minOccurs=\"1\"");
            fullSchema = fullSchema.Replace("use=\"optional\"", "use=\"required\"");

            // first conversion
            string inst = RequestConversion(fullSchema);
            string simpleValueSignature = "<ids:simpleValue>string</ids:simpleValue>";
            inst = inst.Replace("<restriction />", simpleValueSignature);
            var sampleFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\bsFilesSelf\SimpleValueString.xml";
            File.WriteAllText(sampleFile, inst);

            // second conversion
            // place a restriction wherever possible
            // 

            // previously done via schema modification, now a simple replacement
            //
            //var simpleValueDef = @"<xs:element name=""simpleValue"" type=""xs:string"" minOccurs=""1"" maxOccurs=""1""/>";
            //if (!schemaFile.Contains(simpleValueDef))
            //    Console.WriteLine("ERROR: simple value definition not found");
            //fullSchema = fullSchema.Replace(simpleValueDef, "");
            //inst = RequestConversion(fullSchema);
            //
            inst = inst.Replace(simpleValueSignature, @"<xs:restriction base=""xs:string""><xs:enumeration value=""AlternativeOne""/><xs:enumeration value=""AlternativeTwo""/></xs:restriction>");
            sampleFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\bsFilesSelf\SimpleValueRestriction.xml";
            File.WriteAllText(sampleFile, inst);
            Debug.WriteLine(inst);
        }

        private static string RequestConversion(string fullSchema)
        {
            ClipboardService.SetText(fullSchema);
            Console.WriteLine("Please convert xsd currently in clipboard, then copy the converted to clipboard, finally press any key.");
            _ = Console.ReadKey();
            var inst = ClipboardService.GetText();
            var idsSignature = @"<ids:ids xmlns:xml=""http://www.w3.org/XML/1998/namespace"" xmlns:xhtml=""http://www.w3.org/1999/xhtml"" xmlns:ids=""http://standards.buildingsmart.org/IDS"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:hfp=""http://www.w3.org/2001/XMLSchema-hasFacetAndProperty"" xmlns=""http://www.w3.org/2001/XMLSchema"" xsi:schemaLocation=""http://standards.buildingsmart.org/IDS schema.xsd"">";
            if (!inst.Contains(idsSignature))
                Console.WriteLine("ERROR: IDS signature not found");
            var correctSignature = @"<ids:ids xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids.xsd"" xmlns:ids=""http://standards.buildingsmart.org/IDS"">";
            inst = inst.Replace(idsSignature, correctSignature);
            return inst;
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
