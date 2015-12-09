using System;

namespace ClangServer
{
    public class CodeCompletionResponse
    {
        public string FileName { get; set; }
        public ClangCompletionResult[] Results { get; set; }
        public ClangDiagnosticResult[] Diagnostics { get; set; }
    }
}

