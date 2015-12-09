using System;

namespace ClangServer
{
    public class ExpressionTypeRequest
    {
        public string FileName { get; set; } = string.Empty;
        public uint Line { get; set; } = 0;
        public uint Column { get; set; } = 0;
    }
}

