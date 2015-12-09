using System;
using System.Collections.Generic;
using ClangSharp;
using Newtonsoft.Json;

namespace ClangServer
{
    public class ClangCompletionResult
    {
        public string ResultType { get; set; } = string.Empty;
        public string CompletionText { get; set; } = string.Empty;
        public string LeftParen { get; set; } = string.Empty;
        public List<string> PlaceHolders { get; set; } = new List<string>();
        public string RightParen { get; set; } = string.Empty;
        public string Informative { get; set; } = string.Empty;

        public override string ToString()
        {
            return string.Format("[ClangCompletionResult: CompletionText={0}]", CompletionText);
        }

        public static ClangCompletionResult Parse(CodeCompletion cc)
        {
            Console.WriteLine(JsonConvert.SerializeObject(cc));

            ClangCompletionResult ccr = new ClangCompletionResult();
            foreach (var chunk in cc.Chunks)
            {
                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.TypedText)
                {
                    ccr.CompletionText = chunk.Text;
                }

                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.ResultType)
                {
                    ccr.ResultType = chunk.Text;
                }

                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.LeftParen)
                {
                    ccr.LeftParen = chunk.Text;
                }

                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.RightParen)
                {
                    ccr.RightParen = chunk.Text;
                }

                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.Placeholder)
                {
                    ccr.PlaceHolders.Add(chunk.Text);
                }

                if (chunk.Kind == CodeCompletion.Chunk.ChunkKind.Informative)
                {
                    ccr.Informative = chunk.Text;
                }
            }

            return ccr;
        }
    }}

