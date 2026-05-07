using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BreakpointAssistant.Helpers
{
    public static class CodeHelper
    {
        public static Document GetActiveDocument(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                return dte.ActiveDocument;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active document: {ex.Message}");
                return null;
            }
        }

        public static string GetLineTextAtLine(TextDocument textDoc, int lineNum)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var editPoint = textDoc.CreateEditPoint();
                editPoint.MoveToLineAndOffset(lineNum, 1);
                var lineEnd = editPoint.CreateEditPoint();
                lineEnd.EndOfLine();
                return editPoint.GetText(lineEnd);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static List<CodeFunction> GetAllFunctions(Document document, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var functions = new List<CodeFunction>();
            
            try
            {
                var projectItem = document.ProjectItem;
                if (projectItem?.FileCodeModel == null) return functions;

                ParseFunctions(projectItem.FileCodeModel.CodeElements, functions, lineRange);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting functions: {ex.Message}");
            }

            return functions;
        }

        public static void ParseFunctions(CodeElements codeElements, List<CodeFunction> functions, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (codeElements == null) return;

            foreach (CodeElement element in codeElements)
            {
                try
                {
                    if (element.Kind == vsCMElement.vsCMElementFunction)
                    {
                        var func = element as CodeFunction;
                        if (func != null && IsInRange(func.StartPoint.Line, lineRange))
                        {
                            functions.Add(func);
                        }
                    }

                    if (element.Children != null && element.Children.Count > 0)
                    {
                        ParseFunctions(element.Children, functions, lineRange);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing function: {ex.Message}");
                }
            }
        }

        public static List<CodeProperty> GetAllProperties(Document document, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var properties = new List<CodeProperty>();

            try
            {
                var projectItem = document.ProjectItem;
                if (projectItem?.FileCodeModel == null) return properties;

                ParseProperties(projectItem.FileCodeModel.CodeElements, properties, lineRange);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting properties: {ex.Message}");
            }

            return properties;
        }

        public static void ParseProperties(CodeElements codeElements, List<CodeProperty> properties, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (codeElements == null) return;

            foreach (CodeElement element in codeElements)
            {
                try
                {
                    if (element.Kind == vsCMElement.vsCMElementProperty)
                    {
                        var prop = element as CodeProperty;
                        if (prop != null && IsInRange(prop.StartPoint.Line, lineRange))
                        {
                            properties.Add(prop);
                        }
                    }

                    if (element.Children != null && element.Children.Count > 0)
                    {
                        ParseProperties(element.Children, properties, lineRange);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing property: {ex.Message}");
                }
            }
        }

        public static string GetLineText(TextDocument textDoc, int lineNum)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var editPoint = textDoc.CreateEditPoint();
                editPoint.MoveToLineAndOffset(lineNum, 1);
                var lineEnd = editPoint.CreateEditPoint();
                lineEnd.EndOfLine();
                return editPoint.GetText(lineEnd);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetCharStart(string line, string pattern)
        {
            var match = Regex.Match(line, pattern);
            if (match.Success)
            {
                var trimmedStart = line.Length - line.TrimStart().Length;
                return trimmedStart + 1;
            }
            return 1;
        }

        public static Tuple<int, int> GetLineRange(TextDocument textDoc, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (lineRange != null)
            {
                return lineRange;
            }

            var startPoint = textDoc.StartPoint;
            var endPoint = textDoc.EndPoint;
            return Tuple.Create(startPoint.Line, endPoint.Line);
        }

        public static bool IsInRange(int lineNum, Tuple<int, int> lineRange)
        {
            if (lineRange == null) return true;
            return lineNum >= lineRange.Item1 && lineNum <= lineRange.Item2;
        }

        public static HashSet<string> GetMethodNamesFromDocument(Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var methodNames = new HashSet<string>();

            try
            {
                if (document == null) return methodNames;

                var functions = GetAllFunctions(document, null);
                if (functions == null) return methodNames;

                foreach (var func in functions)
                {
                    try
                    {
                        if (func != null && !string.IsNullOrEmpty(func.Name))
                        {
                            methodNames.Add(func.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error adding method name: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting method names from document: {ex.Message}");
            }

            return methodNames;
        }

        public static bool IsMethodDefinition(string line)
        {
            var trimmed = line.TrimStart();
            
            return trimmed.StartsWith("public ") ||
                   trimmed.StartsWith("private ") ||
                   trimmed.StartsWith("protected ") ||
                   trimmed.StartsWith("internal ") ||
                   trimmed.StartsWith("static ") ||
                   trimmed.StartsWith("virtual ") ||
                   trimmed.StartsWith("override ") ||
                   trimmed.StartsWith("abstract ") ||
                   trimmed.StartsWith("async ") ||
                   Regex.IsMatch(trimmed, @"^(void|int|string|bool|double|float|decimal|long|byte|char|object|var|Task|async\s+Task)\s+[A-Z]");
        }

        public static bool IsConstructorAssignment(string line)
        {
            var trimmed = line.Trim();
            
            var assignmentWithNew = Regex.IsMatch(trimmed, 
                @"^(var|[A-Z][a-zA-Z0-9_<>,\[\]]*)\s+[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*new\s+");
            
            if (assignmentWithNew) return true;
            
            var directAssignment = Regex.IsMatch(trimmed, 
                @"^[a-zA-Z_][a-zA-Z0-9_]*\s*=\s*new\s+");
            
            if (directAssignment) return true;
            
            if (trimmed.StartsWith("return new ")) return true;
            
            return false;
        }

        public static bool IsExcludedMethod(string methodName)
        {
            var excludedMethods = new HashSet<string>
            {
                // Object methods
                "ToString", "GetHashCode", "Equals", "GetType", "ReferenceEquals",
                "MemberwiseClone", "Finalize",
                
                // Collection methods  
                "Add", "Remove", "Clear", "Contains", "ToList", "ToArray", "ToDictionary",
                "First", "FirstOrDefault", "Single", "SingleOrDefault", "Last", "LastOrDefault",
                "Where", "Select", "SelectMany", "OrderBy", "OrderByDescending", "GroupBy",
                "Any", "All", "Count", "Sum", "Average", "Min", "Max", "Take", "Skip",
                "Distinct", "Union", "Intersect", "Except", "Concat", "Zip", "Join",
                "ThenBy", "ThenByDescending", "Reverse", "Cast", "OfType", "DefaultIfEmpty",
                
                // String methods
                "Format", "Concat", "Join", "Split", "Substring", "Replace", "Trim",
                "TrimStart", "TrimEnd", "StartsWith", "EndsWith", "IndexOf", "LastIndexOf",
                "ToLower", "ToUpper", "ToLowerInvariant", "ToUpperInvariant",
                "PadLeft", "PadRight", "IsNullOrEmpty", "IsNullOrWhiteSpace",
                "Contains", "Remove", "Insert",
                
                // Entity Framework
                "SaveChanges", "SaveChangesAsync", "Find", "FindAsync", "Include",
                "AsNoTracking", "AsQueryable", "AsEnumerable", "Entry", "Attach",
                "Update", "Remove", "AddRange", "RemoveRange", "UpdateRange",
                "FromSqlRaw", "FromSqlInterpolated", "ExecuteSqlRaw", "ExecuteSqlRawAsync",
                
                // Task/Async methods
                "Wait", "ContinueWith", "ConfigureAwait", "WaitAll", "WaitAny",
                "Run", "FromResult", "Delay", "WhenAll", "WhenAny", "FromCanceled",
                "FromException", "CompletedTask",
                
                // Dispose pattern
                "Dispose", "DisposeAsync", "Close", "CloseAsync",
                
                // Common DI/Service patterns
                "GetService", "GetRequiredService", "CreateScope", "BeginScope",
                "AddTransient", "AddScoped", "AddSingleton", "BuildServiceProvider",
                "Configure", "ConfigureServices", "UseStartup",
                
                // Common validation/guard methods
                "ThrowIfNull", "ArgumentNullException", "ArgumentException",
                "InvalidOperationException", "NotSupportedException",
                
                // Common test methods
                "Assert", "Should", "Verify", "Mock", "Setup", "Returns",
                
                // HTTP/Web methods
                "MapGet", "MapPost", "MapPut", "MapDelete", "UseRouting",
                "UseEndpoints", "UseAuthorization", "UseAuthentication",
                
                // Logging (ILogger)
                "LogInformation", "LogWarning", "LogError", "LogDebug", "LogTrace", "LogCritical"
            };

            return excludedMethods.Contains(methodName);
        }

        public static bool IsCommentOrString(string line)
        {
            var trimmed = line.TrimStart();
            return trimmed.StartsWith("//") || trimmed.StartsWith("/*") || trimmed.StartsWith("*") || trimmed.StartsWith("///");
        }
    }
}
