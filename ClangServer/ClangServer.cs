using System;
using ClangSharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ClangServer
{
    public class ClangServer : BaseServer
    {
        private Index _index = new Index();
        private string[] _clangArgs = null;

        public ClangServer(ushort httpPort, string workspaceRoot, string[] clangArgs) : base(httpPort)
        {
            _clangArgs = clangArgs;
            Console.WriteLine("Changing to workspace: {0}", workspaceRoot);
            Environment.CurrentDirectory = workspaceRoot;
        }

        [RouteAttribute("/complete")]
        public CodeCompletionResponse CompleteCodeAt(CodeCompletionRequest request)
        {
            TranslationUnitFlags tuFlags = TranslationUnitFlags.Incomplete | TranslationUnitFlags.None;
            UnsavedFile unsavedFile = new UnsavedFile(request.FileName, request.FileContent, request.FileContent.Length);
            TranslationUnit tu = _index.CreateTranslationUnit(request.FileName, _clangArgs, new UnsavedFile[] { unsavedFile }, tuFlags);

            var completionFlags = CodeCompletion.Options.IncludeMacros |
                                    CodeCompletion.Options.IncludeBriefComments |
                                    CodeCompletion.Options.IncludeCodePatterns;
            IList<CodeCompletion> res = tu.CodeCompleteAt(request.Line,
                                            request.Column,
                                            new UnsavedFile[] { unsavedFile },
                                            completionFlags);

            return new CodeCompletionResponse()
            {
                FileName = request.FileName,
                Results = res.Select(r => ClangCompletionResult.Parse(r)).ToArray(),
                Diagnostics = tu.Diagnostics.Select(d => ClangDiagnosticResult.Parse(d)).ToArray()
            };
        }

        [RouteAttribute("/definition")]
        public DefinitionPositionResponse DefinitionPosition(DefinitionPositionRequest request)
        {
            var response = new DefinitionPositionResponse()
            {
                Valid = true
            };

            try
            {
                TranslationUnit tu = _index.CreateTranslationUnit(request.FileName, _clangArgs);
                var reqFile = tu.GetFile(request.FileName);
                var reqLocation = tu.GetLocation(reqFile, request.Line, request.Column);
                var reqCursor = tu.GetCursor(reqLocation);
                var definition = reqCursor.Definition;
                var location = definition.Location;

                response.Diagnostics = tu.Diagnostics.Select(d => ClangDiagnosticResult.Parse(d)).ToArray();

                try
                {
                    response.FileName = Path.GetFullPath(location.File.Name);
                } catch (Exception e) {
                    Console.WriteLine("fileName: {0}, exception: {1}", location.File.Name, e);
                    response.FileName = location.File.Name;
                }

                response.Line = location.Line - 1;
                response.Column = location.Column - 1;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                response.Valid = false;
            }

            return response;
        }

        [RouteAttribute("/expressionType")]
        public ExpressionTypeResponse ExpressionType(ExpressionTypeRequest request)
        {
            var response = new ExpressionTypeResponse()
            {
                Valid = true
            };

            try
            {
                TranslationUnit tu = _index.CreateTranslationUnit(request.FileName, _clangArgs);
                ClangSharp.File reqFile = tu.GetFile(request.FileName);
                SourceLocation reqLocation = tu.GetLocation(reqFile, request.Line, request.Column);
                Cursor reqCursor = tu.GetCursor(reqLocation);
                Cursor definition = reqCursor.Definition;

                Console.WriteLine("reqCursor.Spelling: {0}", reqCursor.Spelling);
                Console.WriteLine("definition.Spelling: {0}", definition.Spelling);
                Console.WriteLine("reqCursor.Type.Spelling: {0}", reqCursor.Type.Spelling);
                Console.WriteLine("reqCursor.Kind: {0}", reqCursor.Kind);
                Console.WriteLine("definition.Type.Spelling: {0}", definition.Type.Spelling);
                Console.WriteLine("reqCursor.TypedefDeclUnderlyingType.Spelling: {0}", reqCursor.TypedefDeclUnderlyingType.Spelling);
                Console.WriteLine("definition.TypedefDeclUnderlyingType.Spelling: {0}", definition.TypedefDeclUnderlyingType.Spelling);
                Console.WriteLine("definition.Type.Result.Spelling: {0}", definition.Type.Result.Spelling);
                Console.WriteLine("definition.Type.Canonical.Spelling: {0}", definition.Type.Canonical.Spelling);
                Console.WriteLine("reqCursor.Type.Canonical.Spelling: {0}", reqCursor.Type.Canonical.Spelling);


                response.Diagnostics = tu.Diagnostics.Select(d => ClangDiagnosticResult.Parse(d)).ToArray();
                response.Start.Line = reqCursor.Extent.Start.Line;
                response.Start.Column = reqCursor.Extent.Start.Column;
                response.End.Line = reqCursor.Extent.End.Line;
                response.End.Column = reqCursor.Extent.End.Column;

                response.Content = reqCursor.Type.Spelling;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                response.Valid = false;
            }

            return response;
        }
    }
}
