using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
            var schemaFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\ids.xsd";
            // DirectoryInfo d = new(".");
            // Debug.WriteLine(d.FullName);
            OpenUrl("https://www.liquid-technologies.com/online-xsd-to-xml-converter");

            var fullSchema = File.ReadAllText(schemaFile);

            // take away optional schema stuff, make it as rich as possible
            //
            fullSchema = fullSchema.Replace("minOccurs=\"0\"", "minOccurs=\"1\"");
            fullSchema = fullSchema.Replace("use=\"optional\"", "use=\"required\"");

            // first conversion
            string inst = RequestConversion(fullSchema);


            // we start by removing all values in min and max occurs, so they are easier to fix
            var re = new Regex(@"minOccurs=""[-\d]*""");
            inst = re.Replace(inst, "minOccurs");

            re = new Regex(@"maxOccurs=""[-\d]*""");
            inst = re.Replace(inst, "maxOccurs");

            // now get the min and max occur for specifications right
            var SpecOptions = new[] {
                @"minOccurs=""0""", // optional
                @"minOccurs=""1""", // required
                @"minOccurs=""0"" maxOccurs=""0""", // prohibited
            };
            inst = FixOccur(inst, "specification", SpecOptions);

            // then the facets
            SpecOptions = new[] {
                @"minOccurs=""1""", // required
                @"minOccurs=""0"" maxOccurs=""0""", // prohibited
            };
            inst = FixOccur(inst, "classification", SpecOptions);
            inst = FixOccur(inst, "property", SpecOptions);
            inst = FixOccur(inst, "material", SpecOptions);
            inst = FixOccur(inst, "attribute", SpecOptions);
            // done min and max occur

            string simpleValueSignature = "<ids:simpleValue>string</ids:simpleValue>";
            inst = inst.Replace("<restriction />", simpleValueSignature);
            var sampleFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\bsFilesSelf\SimpleValueString.ids";
            File.WriteAllText(sampleFile, inst);

            // second conversion
            // place a restriction wherever possible
            // previously done via schema modification, now a simple replacement
            //
            inst = inst.Replace(simpleValueSignature, @"<xs:restriction base=""xs:string""><xs:enumeration value=""AlternativeOne""/><xs:enumeration value=""AlternativeTwo""/></xs:restriction>");
            sampleFile = @"..\..\..\..\Xbim.InformationSpecifications.NewTests\bsFiles\bsFilesSelf\SimpleValueRestriction.ids";
            File.WriteAllText(sampleFile, inst);
            Debug.WriteLine(inst);
        }

        private static string FixOccur(string inst, string typecontraint, string[] replaceOptions)
        {
            int i = 0;
            Regex r = new(typecontraint + "[^>]* minOccurs maxOccurs");
            var m = r.Match(inst);
            while (m.Success)
            {
                var value = m.Value.Replace("minOccurs maxOccurs", replaceOptions[i++ % replaceOptions.Length]);
                var prev = inst[..m.Index];
                var post = inst[(m.Index + m.Value.Length)..];
                inst = prev + value + post;
                m = r.Match(inst);
            }


            return inst;
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
            var correctSignature = @"<ids:ids xmlns:ids=""http://standards.buildingsmart.org/IDS"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://standards.buildingsmart.org/IDS http://standards.buildingsmart.org/IDS/ids_09.xsd"">";
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
