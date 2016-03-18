using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MyDriving.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory;
            for (int i = 0; i < 3; i++)
            {
                path = Path.Combine(Path.GetDirectoryName(path), string.Empty);
            }
            var projects = new List<Solution> {


                new Solution {
                    Name = "Android",
                    ProjectFiles = new List<string> {
                        Path.Combine(path, "MyDriving.Android/MyDriving.Android.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Abstractions/MyDriving.DataStore.Abstractions.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Azure/MyDriving.DataStore.Azure.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Mock/MyDriving.DataStore.Mock.csproj"),
                            Path.Combine(path, "MyDriving.AzureClient/MyDriving.AzureClient.csproj"),
                            Path.Combine(path, "MyDriving.Utils/MyDriving.Utils.csproj"),
                            Path.Combine(path, "MyDriving/MyDriving.csproj"),
                            Path.Combine(path, "MyDriving.DataObjects/MyDriving.DataObjects.projitems"),
                            Path.Combine(path, "MyDriving.Shared/MyDriving.Shared.projitems")
                    },
                },

                new Solution {
                    Name = "iOS",
                    ProjectFiles = new List<string> {
                        Path.Combine(path, "MyDriving.iOS/MyDriving.iOS.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Abstractions/MyDriving.DataStore.Abstractions.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Azure/MyDriving.DataStore.Azure.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Mock/MyDriving.DataStore.Mock.csproj"),
                            Path.Combine(path, "MyDriving.AzureClient/MyDriving.AzureClient.csproj"),
                            Path.Combine(path, "MyDriving.Utils/MyDriving.Utils.csproj"),
                            Path.Combine(path, "MyDriving/MyDriving.csproj"),
                            Path.Combine(path, "MyDriving.DataObjects/MyDriving.DataObjects.projitems"),
                            Path.Combine(path, "MyDriving.Shared/MyDriving.Shared.projitems")
                    },
                },
                new Solution {
                    Name = "UWP",
                    ProjectFiles = new List<string> {
                        Path.Combine(path, "MyDriving.UWP/MyDriving.UWP.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Abstractions/MyDriving.DataStore.Abstractions.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Azure/MyDriving.DataStore.Azure.csproj"),
                            Path.Combine(path, "MyDriving.DataStore.Mock/MyDriving.DataStore.Mock.csproj"),
                            Path.Combine(path, "MyDriving.AzureClient/MyDriving.AzureClient.csproj"),
                            Path.Combine(path, "MyDriving.Utils/MyDriving.Utils.csproj"),
                            Path.Combine(path, "MyDriving/MyDriving.csproj"),
                            Path.Combine(path, "MyDriving.DataObjects/MyDriving.DataObjects.projitems"),
                            Path.Combine(path, "MyDriving.Shared/MyDriving.Shared.projitems")
                    },
                },
            };


            new Program().Run(projects);

            Console.ReadLine();
        }

        class Solution
        {
            public string Name = "";
            public List<string> ProjectFiles = new List<string>();
            public List<FileInfo> CodeFiles = new List<FileInfo>();
            public override string ToString()
            {
                return Name;
            }

            public int UniqueLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                            where f.Solutions.Count == 1
                            select f.LinesOfCode).Sum();
                }
            }

            public int SharedLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                            where f.Solutions.Count > 1
                            select f.LinesOfCode).Sum();
                }
            }

            public int TotalLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                            select f.LinesOfCode).Sum();
                }
            }
        }

        class FileInfo
        {
            public string Path = "";
            public List<Solution> Solutions = new List<Solution>();
            public int LinesOfCode = 0;
            public override string ToString()
            {
                return Path;
            }
        }

        Dictionary<string, FileInfo> _files = new Dictionary<string, FileInfo>();

        void AddRef(string path, Solution sln)
        {

            if (_files.ContainsKey(path))
            {
                _files[path].Solutions.Add(sln);
                sln.CodeFiles.Add(_files[path]);
            }
            else
            {
                var info = new FileInfo { Path = path, };
                info.Solutions.Add(sln);
                _files[path] = info;
                sln.CodeFiles.Add(info);
            }
        }

        void Run(List<Solution> solutions)
        {
            //
            // Find all the files
            //
            foreach (var sln in solutions)
            {
                foreach (var projectFile in sln.ProjectFiles)
                {
                    var dir = Path.GetDirectoryName(projectFile);
                    var projectName = Path.GetFileNameWithoutExtension(projectFile);
                    var doc = XDocument.Load(projectFile);
                    var q = from x in doc.Descendants()
                                         let e = x as XElement
                                             where e != null
                                             where e.Name.LocalName == "Compile"
                                             where e.Attributes().Any(a => a.Name.LocalName == "Include")
                                         select e.Attribute("Include").Value;
                    foreach (var inc in q)
                    {
                        //skip over some things that are added automatically
                        if (inc.Contains("Resource.designer.cs") || //auto generated
                            inc.Contains("DebugTrace.cs") || //not needed mvvmcross
                            inc.Contains("LinkerPleaseInclude.cs") || //not needed mvvmcross
                            inc.Contains("AssemblyInfo.cs") || //in every place
                            inc.Contains("Bootstrap.cs") || //not needed mvvmcross
                            inc.Contains(".designer.cs") || //auto generated, not code
                            inc.Contains(".Designer.cs") || //Android designer file
                            inc.Contains("App.xaml.cs") || //generic WP setup
                            inc.EndsWith(".xaml") ||
                            inc.EndsWith(".xml") ||
                            inc.EndsWith(".axml"))
                        {
                            continue;
                        }

                        var inc2 = inc.Replace("\\", Path.DirectorySeparatorChar.ToString());
                        inc2 = inc2.Replace("$(MSBuildThisFileDirectory)", string.Empty);
                        AddRef(Path.GetFullPath(Path.Combine(dir, inc2)), sln);
                    }
                }
            }

            //
            // Get the lines of code
            //
            foreach (var f in _files.Values)
            {
                try
                {
                    var lines = File.ReadAllLines(f.Path).ToList();

                    f.LinesOfCode = lines.Count;
                }
                catch (Exception ex)
                {
                }
            }

            //
            // Output
            //
            Console.WriteLine("app\ttotal\tunique\tshared\tunique%\tshared%");
            foreach (var sln in solutions)
            {

                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4:p}\t{5:p}",
                                  sln.Name,
                                  sln.TotalLinesOfCode,
                                  sln.UniqueLinesOfCode,
                                  sln.SharedLinesOfCode,
                                  sln.UniqueLinesOfCode / (double)sln.TotalLinesOfCode,
                                  sln.SharedLinesOfCode / (double)sln.TotalLinesOfCode);
            }
            Console.WriteLine(string.Empty);
            Console.WriteLine("DONE");
        }
    }
}
