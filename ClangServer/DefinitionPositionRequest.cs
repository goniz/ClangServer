using System;

namespace ClangServer
{
    public class DefinitionPositionRequest
    {
        public string FileName { get; set; }
        public uint Line { get; set; }
        public uint Column { get; set; }
    }
}

