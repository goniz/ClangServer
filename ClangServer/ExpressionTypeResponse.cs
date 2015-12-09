using System;

namespace ClangServer
{
    public class ExpressionTypeResponse
    {
        public class SourcePosition
        {
            public int Line { get; set; } = 0;
            public int Column { get; set; } = 0;
        }

        public bool Valid { get; set; } = false;
        public string Content { get; set; } = string.Empty;
        public SourcePosition Start { get; set; } = new SourcePosition();
        public SourcePosition End { get; set; } = new SourcePosition();
        public ClangDiagnosticResult[] Diagnostics { get; set; }
    }
}

