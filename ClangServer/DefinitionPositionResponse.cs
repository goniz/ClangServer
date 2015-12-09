using System;

namespace ClangServer
{
    public class DefinitionPositionResponse
    {
        public string FileName { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public bool Valid { get; set; }
        public ClangDiagnosticResult[] Diagnostics { get; set; }
    }
}

