using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Xbim.InformationSpecifications.Generator
{
    internal class IdsRepo_Updater
    {
        internal static DirectoryInfo? GetSolutionDirectory()
        {
            DirectoryInfo? d = new(".");
            while (d is not null)
            {
                var subIDS = d.GetFiles("Xbim.Xids.sln").FirstOrDefault();
                if (subIDS != null)
                    return d;
                d = d.Parent;
            }
            return null;
        }

        internal static string ExecuteCommandLine(string argumentsString, bool strip = true)
        {
            var d = GetSolutionDirectory();
            var pathInclude = "Release";
#if DEBUG
            pathInclude = "Debug";
#endif

            var toolPath = d?.GetFiles("ids-tool.exe", SearchOption.AllDirectories).FirstOrDefault(x => x.FullName.Contains(pathInclude))
                ?? throw new Exception("Tool binary not found.");
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolPath.FullName,
                    Arguments = argumentsString,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            StringBuilder sb = new();
            proc.Start();
            while (!proc.StandardOutput.EndOfStream || !proc.StandardError.EndOfStream)
            {
                var line = proc.StandardOutput.ReadLine();
                if (line is not null)
                    sb.AppendLine(line);

                line = proc.StandardError.ReadLine();
                if (line is not null)
                    sb.AppendLine(line);
            }
            if (!strip)
                return sb.ToString();
            // remove first 3 lines
            var retArr = sb.ToString();
            string[] lines = retArr
                .Split(Environment.NewLine)
                .Skip(3)
                .ToArray();
            string ret = string.Join(Environment.NewLine, lines);
            return ret.Trim('\r', '\n');
        }

        internal class UpdatableFile
        {
            public string Name { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }

            public UpdatableFile(string name, string source, string destination)
            {
                Name = name;
                Source = source;
                Destination = destination;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if information has been changed locally... need to reboot.</returns>
        internal static bool UpdateRequiresRestart()
        {
            if (!Exists)
                return false;

            var solutionDir = GetSolutionDirectory();
            if (solutionDir is null)
                return false;

            var updatables = GetAllUpdatable(solutionDir);
            if (!updatables.Any())
                return false;

            Console.WriteLine("Updates are available from the IDS repository:");
            foreach (var updatable in updatables)
                Console.WriteLine($"- {updatable.Name}");

            Console.WriteLine("Get these updates from IDS repository? (y/n)");
            var k = Console.ReadKey();
            Console.WriteLine();
            if (k.Key != ConsoleKey.Y)
            {
                Console.WriteLine("Update skipped.");
                return false;
            }
            foreach (var updatableFile in updatables)
                File.Copy(updatableFile.Source, updatableFile.Destination, true);

            return true;
        }

        public static IEnumerable<UpdatableFile> GetAllUpdatable(DirectoryInfo solutionDir)
        {
            return Enumerable.Empty<UpdatableFile>()
                .Concat(UpdateTestingSchema(solutionDir));
        }

        private static IEnumerable<UpdatableFile> UpdateTestingSchema(DirectoryInfo solutionDir)
        {
            List<string> files = new()
            {
                "ids.xsd",
                "IDS_aachen_example.ids",
                "IDS_Aedes_example.ids",
                "IDS_ArcDox.ids",
                "IDS_demo_BIM-basis-ILS.ids",
                "IDS_random_example.ids",
                "IDS_SimpleBIM_examples.ids",
                "IDS_ucms_prefab_pipes_IFC2x3.ids",
                "IDS_ucms_prefab_pipes_IFC4.3.ids",
                "IDS_wooden-windows.ids",
            };

            foreach (var file in files)
            {
                var sourceFile = BuildingSmartRepoFiles.GetDevelopment(file);
                if (sourceFile.Exists)
                {
                    var destination = Path.Combine(
                       solutionDir.FullName,
                        "Xbim.InformationSpecifications.NewTests",
                        "bsFiles",
                        file
                        );
                    if (BuildingSmartRepoFiles.FilesAreIdentical(sourceFile, new FileInfo(destination)))
                        yield break;
                    yield return new UpdatableFile($"ids repository file {file}", sourceFile.FullName, destination);
                }
            }
        }

        internal static bool Exists
        {
            get
            {
                var schema = BuildingSmartRepoFiles.GetIdsSchema();
                return schema.Exists;
            }
        }

    }
}