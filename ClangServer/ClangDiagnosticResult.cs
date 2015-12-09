using System;
using ClangSharp;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

namespace ClangServer
{
    public class ClangDiagnosticResult
    {
        public class SourcePosition
        {
            public int Line { get; set; }
            public int Column { get; set; }
        }

        public string GCCLine { get; set; }
        public string Text { get; set; }
        public Diagnostic.Severity Level { get; set; }
        public string FileName { get; set; }
        public SourcePosition Position { get; set; }

        private static Regex GCCLineRegex = new Regex("^(.*):([0-9]+):([0-9]+): (fatal error|error|warning): (.*)$");
        public static ClangDiagnosticResult Parse(Diagnostic diag)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(diag, Formatting.Indented));
            Console.WriteLine(diag.Description);
            // /home/gz/devel/cuboxi4/gzOS/src/net/ethernet_layer.cpp:170:62: error: use of undeclared identifier
            Match match = ClangDiagnosticResult.GCCLineRegex.Match(diag.Description);
            
            string fileName = string.Empty;
            try {
                fileName = Path.GetFullPath(diag.Location.File.Name);
            } catch (Exception) {

            }
            
            // sorry, clang column is stupid, but the description line contains the right info....
            ClangDiagnosticResult res = new ClangDiagnosticResult() {
                GCCLine = diag.Description,
                Text = diag.Spelling,
                Level = diag.Level,
                FileName = fileName,
                Position = new SourcePosition() {
                    Line = Int32.Parse(match.Groups[2].ToString()) - 1,
                    Column = Int32.Parse(match.Groups[3].ToString()) - 1
                }
            };

            return res;
        }
    }
}

