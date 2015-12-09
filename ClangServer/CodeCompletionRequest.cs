using System;

namespace ClangServer
{
    public class CodeCompletionRequest
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public uint Line { get; set; }
        public uint Column { get; set; }
    }
}

