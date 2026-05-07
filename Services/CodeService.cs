using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BreakpointAssistant.Models;
using BreakpointAssistant.Helpers;

namespace BreakpointAssistant.Services
{
    /// <summary>
    /// Service for locating code elements and their positions within source files.
    /// </summary>
    public class CodeService
    {
        private readonly DTE2 _dte;

        /// <summary>
        /// Initializes a new instance of the CodeLocator class.
        /// </summary>
        /// <param name="dte">The DTE2 service instance.</param>
        public CodeService(DTE2 dte)
        {
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));
        }

        /// <summary>
        /// Finds all methods matching the search text.
        /// </summary>
        /// <param name="searchText">The method name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within (start, end). If null, searches entire document.</param>
        /// <returns>List of CodeDetail objects representing found methods.</returns>
        public List<CodeDetail> FindMethods(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var functions = CodeHelper.GetAllFunctions(document, lineRange);

            foreach (var func in functions)
            {
                try
                {
                    if (string.IsNullOrEmpty(searchText) || func.Name.Contains(searchText))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = func.StartPoint.Line,
                            CharStart = func.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.Method,
                            CodeText = func.Name,
                            Context = func.FullName
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing method {func.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all property setters matching the search text.
        /// </summary>
        /// <param name="searchText">The property name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found setters.</returns>
        public List<CodeDetail> FindSetters(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var properties = CodeHelper.GetAllProperties(document, lineRange);

            foreach (var prop in properties)
            {
                try
                {
                    if (prop.Setter != null && (string.IsNullOrEmpty(searchText) || prop.Name.Contains(searchText)))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = prop.Setter.StartPoint.Line,
                            CharStart = prop.Setter.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.Setter,
                            CodeText = $"{prop.Name}.set",
                            Context = prop.FullName
                        });
                    }
                }
                catch (NotImplementedException)
                {
                    // Some property setters don't support StartPoint (e.g., auto-properties in certain contexts)
                    // Fall back to using the property's start point
                    try
                    {
                        if (prop.Setter != null && (string.IsNullOrEmpty(searchText) || prop.Name.Contains(searchText)))
                        {
                            results.Add(new CodeDetail
                            {
                                LineStart = prop.StartPoint.Line,
                                CharStart = prop.StartPoint.LineCharOffset,
                                CodeType = CodeDetailType.Setter,
                                CodeText = $"{prop.Name}.set",
                                Context = prop.FullName
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error getting setter for property {prop.Name}: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing setter for property {prop.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all property getters matching the search text.
        /// </summary>
        /// <param name="searchText">The property name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found getters.</returns>
        public List<CodeDetail> FindGetters(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var properties = CodeHelper.GetAllProperties(document, lineRange);

            foreach (var prop in properties)
            {
                try
                {
                    if (prop.Getter != null && (string.IsNullOrEmpty(searchText) || prop.Name.Contains(searchText)))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = prop.Getter.StartPoint.Line,
                            CharStart = prop.Getter.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.Getter,
                            CodeText = $"{prop.Name}.get",
                            Context = prop.FullName
                        });
                    }
                }
                catch (NotImplementedException)
                {
                    // Some property getters don't support StartPoint (e.g., auto-properties in certain contexts)
                    // Fall back to using the property's start point
                    try
                    {
                        if (prop.Getter != null && (string.IsNullOrEmpty(searchText) || prop.Name.Contains(searchText)))
                        {
                            results.Add(new CodeDetail
                            {
                                LineStart = prop.StartPoint.Line,
                                CharStart = prop.StartPoint.LineCharOffset,
                                CodeType = CodeDetailType.Getter,
                                CodeText = $"{prop.Name}.get",
                                Context = prop.FullName
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error getting getter for property {prop.Name}: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing getter for property {prop.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all variable assignments in the active document.
        /// Matches patterns like: variable = value, Type x = value, var y = value,
        /// obj.Property = value, arr[i] = value, compound assignments (+=, -=, etc.),
        /// auto-property initializers, and tuple deconstruction assignments.
        /// </summary>
        /// <param name="searchText">The variable name or pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found variable assignments.</returns>
        public List<CodeDetail> FindVariableAssignments(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var textDoc = document.Object("TextDocument") as TextDocument;
            if (textDoc == null) return results;

            var (startLine, endLine) = CodeHelper.GetLineRange(textDoc, lineRange);

            for (int lineNum = startLine; lineNum <= endLine; lineNum++)
            {
                var line = CodeHelper.GetLineText(textDoc, lineNum);
                if (string.IsNullOrEmpty(line)) continue;

                // Skip comments and strings
                if (CodeHelper.IsCommentOrString(line)) continue;

                // Apply search filter early if provided
                if (!string.IsNullOrEmpty(searchText) && !line.Contains(searchText))
                    continue;

                var trimmedLine = line.Trim();
                bool foundAssignment = false;
                int assignmentIndex = -1;

                // Pattern 1: Tuple/Deconstruction Assignments
                // (int x, int y) = GetTuple(); or var (a, b) = (1, 2);
                var tuplePattern = @"(var\s+)?\([^)]+\)\s*=\s*(?!=)";
                var tupleMatch = Regex.Match(line, tuplePattern);
                if (tupleMatch.Success)
                {
                    foundAssignment = true;
                    assignmentIndex = tupleMatch.Index;
                }

                // Pattern 2: Compound Assignments (+=, -=, *=, /=, %=, &=, |=, ^=, ??=)
                if (!foundAssignment)
                {
                    var compoundPattern = @"[A-Za-z_][a-zA-Z0-9_\.]*(\[[^\]]*\])?\s*([+\-*/%&|^]|[?]{2})=";
                    var compoundMatch = Regex.Match(line, compoundPattern);
                    if (compoundMatch.Success)
                    {
                        foundAssignment = true;
                        assignmentIndex = compoundMatch.Index;
                    }
                }

                // Pattern 3: Indexer Assignments
                // array[0] = value; or dict["key"] = value;
                if (!foundAssignment)
                {
                    var indexerPattern = @"[A-Za-z_][a-zA-Z0-9_\.]*\[[^\]]+\]\s*=\s*(?!=)";
                    var indexerMatch = Regex.Match(line, indexerPattern);
                    if (indexerMatch.Success)
                    {
                        foundAssignment = true;
                        assignmentIndex = indexerMatch.Index;
                    }
                }

                // Pattern 4: Member/Property Assignments
                // obj.Property = value; or this.field = value;
                if (!foundAssignment)
                {
                    var memberPattern = @"(this\.|[A-Za-z_][a-zA-Z0-9_]*\.)[A-Za-z_][a-zA-Z0-9_]*\s*=\s*(?!=)";
                    var memberMatch = Regex.Match(line, memberPattern);
                    if (memberMatch.Success)
                    {
                        foundAssignment = true;
                        assignmentIndex = memberMatch.Index;
                    }
                }

                // Pattern 5: Auto-Property Initializers
                // public int Id { get; set; } = 5;
                if (!foundAssignment)
                {
                    var autoPropPattern = @"\{\s*get;.*?set;.*?\}\s*=\s*(?!=)";
                    var autoPropMatch = Regex.Match(line, autoPropPattern);
                    if (autoPropMatch.Success)
                    {
                        foundAssignment = true;
                        assignmentIndex = autoPropMatch.Index;
                    }
                }

                // Pattern 6: Standard Variable Assignments
                // identifier = expression; or Type identifier = expression; or var identifier = expression;
                if (!foundAssignment)
                {
                    var standardPattern = @"([A-Za-z_][a-zA-Z0-9_<>\.]*\s+)?([A-Za-z_][a-zA-Z0-9_]*)\s*=\s*(?!=)";
                    var standardMatch = Regex.Match(line, standardPattern);
                    if (standardMatch.Success)
                    {
                        // Skip property declarations without initializers
                        if (line.Contains("{ get") || line.Contains("{ set") || line.Contains("{get") || line.Contains("{set"))
                        {
                            // Unless it's an auto-property initializer (already handled above)
                            if (!line.Contains("="))
                                continue;
                        }

                        foundAssignment = true;
                        assignmentIndex = standardMatch.Index;
                    }
                }

                // Pattern 7: Multiple Assignments on One Line
                // int x = 1, y = 2, z = 3;
                if (!foundAssignment)
                {
                    var multiplePattern = @"([A-Za-z_][a-zA-Z0-9_<>\.]*\s+)?([A-Za-z_][a-zA-Z0-9_]*)\s*=\s*[^,;]+,\s*([A-Za-z_][a-zA-Z0-9_]*)\s*=";
                    var multipleMatch = Regex.Match(line, multiplePattern);
                    if (multipleMatch.Success)
                    {
                        foundAssignment = true;
                        assignmentIndex = multipleMatch.Index;
                    }
                }

                // If we found any assignment pattern, add it to results
                if (foundAssignment && assignmentIndex >= 0)
                {
                    results.Add(new CodeDetail
                    {
                        LineStart = lineNum,
                        CharStart = Math.Max(1, assignmentIndex + 1), // 1-based
                        CodeType = CodeDetailType.VariableAssignment,
                        CodeText = trimmedLine,
                        Context = document.Name
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all properties matching the search text.
        /// </summary>
        /// <param name="searchText">The property name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found properties.</returns>
        public List<CodeDetail> FindProperties(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var properties = CodeHelper.GetAllProperties(document, lineRange);

            foreach (var prop in properties)
            {
                try
                {
                    if (string.IsNullOrEmpty(searchText) || prop.Name.Contains(searchText))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = prop.StartPoint.Line,
                            CharStart = prop.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.Property,
                            CodeText = prop.Name,
                            Context = prop.FullName
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing property {prop.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all 'if' statements matching the search text.
        /// </summary>
        /// <param name="searchText">The condition or pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found if statements.</returns>
        public List<CodeDetail> FindIf(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Matches: if (condition)
            var pattern = @"^\s*if\s*\(";
            return FindByPattern(pattern, searchText, CodeDetailType.If, lineRange);
        }

        /// <summary>
        /// Finds all 'if-else' statements matching the search text.
        /// </summary>
        /// <param name="searchText">The condition or pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found if-else statements.</returns>
        public List<CodeDetail> FindIfElse(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var textDoc = document.Object("TextDocument") as TextDocument;
            if (textDoc == null) return results;

            var (startLine, endLine) = CodeHelper.GetLineRange(textDoc, lineRange);
            
            // Find if statements and check if they have a corresponding else
            var ifPattern = @"^\s*if\s*\(";
            var elsePattern = @"^\s*else\b";

            for (int lineNum = startLine; lineNum <= endLine; lineNum++)
            {
                var line = CodeHelper.GetLineText(textDoc, lineNum);
                if (Regex.IsMatch(line, ifPattern))
                {
                    // Look ahead to find if there's an else
                    for (int nextLine = lineNum + 1; nextLine <= endLine && nextLine <= lineNum + 100; nextLine++)
                    {
                        var nextLineText = CodeHelper.GetLineText(textDoc, nextLine);
                        if (Regex.IsMatch(nextLineText, elsePattern))
                        {
                            // Found an if-else pair
                            if (string.IsNullOrEmpty(searchText) || line.Contains(searchText))
                            {
                                results.Add(new CodeDetail
                                {
                                    LineStart = lineNum,
                                    CharStart = CodeHelper.GetCharStart(line, ifPattern),
                                    CodeType = CodeDetailType.IfElse,
                                    CodeText = line.Trim(),
                                    Context = $"if...else at lines {lineNum}-{nextLine}"
                                });
                            }
                            break;
                        }
                        // Stop if we hit another control structure at same level
                        if (Regex.IsMatch(nextLineText, @"^\s*if\s*\(") || 
                            Regex.IsMatch(nextLineText, @"^\s*(public|private|protected|internal|class|namespace)"))
                        {
                            break;
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all 'else' statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found else statements.</returns>
        public List<CodeDetail> FindElse(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*else\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Else, lineRange);
        }

        /// <summary>
        /// Finds all 'for' loops matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found for loops.</returns>
        public List<CodeDetail> FindFor(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*for\s*\(";
            return FindByPattern(pattern, searchText, CodeDetailType.For, lineRange);
        }

        /// <summary>
        /// Finds all 'foreach' loops matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found foreach loops.</returns>
        public List<CodeDetail> FindForeach(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*foreach\s*\(";
            return FindByPattern(pattern, searchText, CodeDetailType.Foreach, lineRange);
        }

        /// <summary>
        /// Finds all 'try-catch' blocks matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found try-catch blocks.</returns>
        public List<CodeDetail> FindTryCatch(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*try\b";
            return FindByPattern(pattern, searchText, CodeDetailType.TryCatch, lineRange);
        }

        /// <summary>
        /// Finds all 'while' loops matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found while loops.</returns>
        public List<CodeDetail> FindWhile(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*while\s*\(";
            return FindByPattern(pattern, searchText, CodeDetailType.While, lineRange);
        }

        /// <summary>
        /// Finds all 'do-while' loops matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found do-while loops.</returns>
        public List<CodeDetail> FindDoWhile(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*do\b";
            return FindByPattern(pattern, searchText, CodeDetailType.DoWhile, lineRange);
        }

        /// <summary>
        /// Finds all 'return' statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found return statements.</returns>
        public List<CodeDetail> FindReturn(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*return\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Return, lineRange);
        }

        /// <summary>
        /// Finds all public methods matching the search text.
        /// </summary>
        /// <param name="searchText">The method name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found public methods.</returns>
        public List<CodeDetail> FindPublicMethods(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var functions = CodeHelper.GetAllFunctions(document, lineRange);
            
            foreach (var func in functions)
            {
                try
                {
                    if (func.Access == vsCMAccess.vsCMAccessPublic &&
                        (string.IsNullOrEmpty(searchText) || func.Name.Contains(searchText)))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = func.StartPoint.Line,
                            CharStart = func.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.PublicMethod,
                            CodeText = func.Name,
                            Context = func.FullName
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing public method {func.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all private methods matching the search text.
        /// </summary>
        /// <param name="searchText">The method name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found private methods.</returns>
        public List<CodeDetail> FindPrivateMethods(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var functions = CodeHelper.GetAllFunctions(document, lineRange);
            
            foreach (var func in functions)
            {
                try
                {
                    if (func.Access == vsCMAccess.vsCMAccessPrivate &&
                        (string.IsNullOrEmpty(searchText) || func.Name.Contains(searchText)))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = func.StartPoint.Line,
                            CharStart = func.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.PrivateMethod,
                            CodeText = func.Name,
                            Context = func.FullName
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing private method {func.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all method closing braces matching the search text.
        /// </summary>
        /// <param name="searchText">The method name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found method closing braces.</returns>
        public List<CodeDetail> FindMethodsClosingBrace(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var functions = CodeHelper.GetAllFunctions(document, lineRange);
            
            foreach (var func in functions)
            {
                if (string.IsNullOrEmpty(searchText) || func.Name.Contains(searchText))
                {
                    try
                    {
                        // Get the end point of the function (closing brace)
                        var endLine = func.EndPoint.Line;
                        var endChar = func.EndPoint.LineCharOffset;

                        // Try to find the actual closing brace position
                        var textDoc = document.Object("TextDocument") as TextDocument;
                        if (textDoc != null)
                        {
                            var lineText = CodeHelper.GetLineTextAtLine(textDoc, endLine);
                            // Find the last closing brace on the line
                            var bracePos = lineText.LastIndexOf('}');
                            if (bracePos >= 0)
                            {
                                endChar = bracePos + 1; // 1-based index
                            }
                        }

                        results.Add(new CodeDetail
                        {
                            LineStart = endLine,
                            CharStart = endChar,
                            CodeType = CodeDetailType.MethodClosingBrace,
                            CodeText = $"}} // End of {func.Name}",
                            Context = func.FullName
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error getting method end point: {ex.Message}");
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all switch statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found switch statements.</returns>
        public List<CodeDetail> FindSwitch(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*switch\s*\(";
            return FindByPattern(pattern, searchText, CodeDetailType.Switch, lineRange);
        }

        /// <summary>
        /// Finds all case statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found case statements.</returns>
        public List<CodeDetail> FindCase(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*case\s+";
            return FindByPattern(pattern, searchText, CodeDetailType.Case, lineRange);
        }

        /// <summary>
        /// Finds all break statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found break statements.</returns>
        public List<CodeDetail> FindBreak(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*break\s*;";
            return FindByPattern(pattern, searchText, CodeDetailType.Break, lineRange);
        }

        /// <summary>
        /// Finds all continue statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found continue statements.</returns>
        public List<CodeDetail> FindContinue(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*continue\s*;";
            return FindByPattern(pattern, searchText, CodeDetailType.Continue, lineRange);
        }

        /// <summary>
        /// Finds all try blocks matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found try blocks.</returns>
        public List<CodeDetail> FindTry(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*try\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Try, lineRange);
        }

        /// <summary>
        /// Finds all catch blocks matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found catch blocks.</returns>
        public List<CodeDetail> FindCatch(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*catch\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Catch, lineRange);
        }

        /// <summary>
        /// Finds all finally blocks matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found finally blocks.</returns>
        public List<CodeDetail> FindFinally(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*finally\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Finally, lineRange);
        }

        /// <summary>
        /// Finds all throw statements matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found throw statements.</returns>
        public List<CodeDetail> FindThrow(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"^\s*throw\b";
            return FindByPattern(pattern, searchText, CodeDetailType.Throw, lineRange);
        }

        /// <summary>
        /// Finds all await expressions matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found await expressions.</returns>
        public List<CodeDetail> FindAwait(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"\bawait\s+";
            return FindByPattern(pattern, searchText, CodeDetailType.Await, lineRange);
        }

        /// <summary>
        /// Finds all constructors matching the search text.
        /// </summary>
        /// <param name="searchText">The constructor name or pattern to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found constructors.</returns>
        public List<CodeDetail> FindConstructor(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var functions = CodeHelper.GetAllFunctions(document, lineRange);
            
            foreach (var func in functions)
            {
                try
                {
                    if (func.FunctionKind == vsCMFunction.vsCMFunctionConstructor &&
                        (string.IsNullOrEmpty(searchText) || func.Name.Contains(searchText)))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = func.StartPoint.Line,
                            CharStart = func.StartPoint.LineCharOffset,
                            CodeType = CodeDetailType.Constructor,
                            CodeText = func.Name,
                            Context = func.FullName
                        });
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing constructor {func.Name}: {ex.Message}");
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all ILogger references matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found ILogger references.</returns>
        public List<CodeDetail> FindILogger(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"ILogger|_logger\.|\.Log(Information|Warning|Error|Debug|Trace|Critical)\(";
            return FindByPattern(pattern, searchText, CodeDetailType.ILogger, lineRange);
        }

        /// <summary>
        /// Finds all lambda expressions matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found lambda expressions.</returns>
        public List<CodeDetail> FindLambda(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"=>\s*";
            return FindByPattern(pattern, searchText, CodeDetailType.Lambda, lineRange);
        }

        /// <summary>
        /// Finds all LINQ queries matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found LINQ queries.</returns>
        public List<CodeDetail> FindLinq(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"\.(Where|Select|OrderBy|FirstOrDefault|SingleOrDefault|Any|All|Count|Sum|Average|Min|Max|GroupBy|Join|Take|Skip)\(";
            return FindByPattern(pattern, searchText, CodeDetailType.Linq, lineRange);
        }

        /// <summary>
        /// Finds all 'new' keyword usage matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found 'new' keywords.</returns>
        public List<CodeDetail> FindNew(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"\bnew\s+";
            return FindByPattern(pattern, searchText, CodeDetailType.New, lineRange);
        }

        /// <summary>
        /// Finds all null checks and references matching the search text.
        /// </summary>
        /// <param name="searchText">The pattern to search for (optional).</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found null references.</returns>
        public List<CodeDetail> FindNull(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var pattern = @"\bnull\b|== null|!= null|\?\?|\?\.";
            return FindByPattern(pattern, searchText, CodeDetailType.Null, lineRange);
        }

        /// <summary>
        /// Finds all occurrences of custom text matching the search text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found text occurrences.</returns>
        public List<CodeDetail> FindCustomText(string searchText, Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            
            if (string.IsNullOrEmpty(searchText))
            {
                return results;
            }

            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var textDoc = document.Object("TextDocument") as TextDocument;
            if (textDoc == null) return results;

            var (startLine, endLine) = CodeHelper.GetLineRange(textDoc, lineRange);

            // Search for the exact text (case-sensitive)
            for (int lineNum = startLine; lineNum <= endLine; lineNum++)
            {
                var line = CodeHelper.GetLineText(textDoc, lineNum);
                
                // Find all occurrences of the search text in the line
                int index = 0;
                while ((index = line.IndexOf(searchText, index)) != -1)
                {
                    results.Add(new CodeDetail
                    {
                        LineStart = lineNum,
                        CharStart = index + 1, // 1-based
                        CodeType = CodeDetailType.CustomText,
                        CodeText = line.Trim(),
                        Context = document.Name
                    });
                    
                    // Move to next potential occurrence
                    index += searchText.Length;
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all custom method calls (downstream) - where YOUR methods are called.
        /// Excludes Microsoft, System, and common framework methods.
        /// Only searches within the active document or selection.
        /// </summary>
        /// <param name="searchText">Optional filter for method names.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of CodeDetail objects representing found method calls.</returns>
        public List<CodeDetail> FindMethodDownstream(string searchText = "", Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();

            try
            {
                var document = CodeHelper.GetActiveDocument(_dte);
                if (document == null) return results;

                var textDoc = document.Object("TextDocument") as TextDocument;
                if (textDoc == null) return results;

                var (startLine, endLine) = CodeHelper.GetLineRange(textDoc, lineRange);

                // Get list of method names defined in the active document
                HashSet<string> ownMethods = null;
                try
                {
                    ownMethods = CodeHelper.GetMethodNamesFromDocument(document);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting method names: {ex.Message}");
                    // Continue with empty set if we can't get method names
                    ownMethods = new HashSet<string>();
                }

                // Pattern to match method calls: identifier followed by (
                // Matches: MethodName(, object.MethodName(, _service.MethodName(
                var methodCallPattern = @"([A-Za-z_][a-zA-Z0-9_]*\.)*([A-Z][a-zA-Z0-9_]*)\s*\(";

                for (int lineNum = startLine; lineNum <= endLine; lineNum++)
                {
                    try
                    {
                        var line = CodeHelper.GetLineText(textDoc, lineNum);
                        if (string.IsNullOrEmpty(line)) continue;
                        
                        // Skip comments and strings
                        if (CodeHelper.IsCommentOrString(line)) continue;

                        // Skip method/constructor definitions (lines with access modifiers)
                        if (CodeHelper.IsMethodDefinition(line)) continue;

                        // Skip lines with 'new' keyword used for variable assignment/declaration
                        // But allow 'new' inside method parameters: Method(new Type())
                        if (CodeHelper.IsConstructorAssignment(line)) continue;

                        var matches = Regex.Matches(line, methodCallPattern);
                        if (matches == null || matches.Count == 0) continue;

                        foreach (Match match in matches)
                        {
                            try
                            {
                                if (!match.Success || match.Groups.Count < 3) continue;

                                var methodName = match.Groups[2].Value;
                                if (string.IsNullOrEmpty(methodName)) continue;

                                var qualifier = match.Groups[1].Value; // e.g., "System.", "Microsoft.EntityFrameworkCore."
                                
                                // Skip if it has a namespace qualifier that starts with Microsoft or System
                                if (!string.IsNullOrEmpty(qualifier))
                                {
                                    if (qualifier.StartsWith("System.") || 
                                        qualifier.StartsWith("Microsoft.") ||
                                        qualifier.StartsWith("EntityFrameworkCore."))
                                    {
                                        continue;
                                    }
                                }
                                
                                // Filter: Only include if it's one of our own methods and not excluded
                                if (ownMethods.Contains(methodName) && !CodeHelper.IsExcludedMethod(methodName))
                                {
                                    if (string.IsNullOrEmpty(searchText) || methodName.Contains(searchText))
                                    {
                                        results.Add(new CodeDetail
                                        {
                                            LineStart = lineNum,
                                            CharStart = Math.Max(1, match.Index + 1), // 1-based, ensure >= 1
                                            CodeType = CodeDetailType.MethodDownstream,
                                            CodeText = line.Trim(),
                                            Context = $"Call to: {methodName}"
                                        });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log but continue processing other matches on this line
                                System.Diagnostics.Debug.WriteLine($"Error processing match on line {lineNum}: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log but continue processing other lines
                        System.Diagnostics.Debug.WriteLine($"Error processing line {lineNum}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FindMethodDownstream: {ex.Message}");
                // Return whatever results we have so far
            }

            return results;
        }

        /// <summary>
        /// Finds code elements by regex pattern.
        /// </summary>
        private List<CodeDetail> FindByPattern(string pattern, string searchText, CodeDetailType codeType, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var results = new List<CodeDetail>();
            var document = CodeHelper.GetActiveDocument(_dte);
            if (document == null) return results;

            var textDoc = document.Object("TextDocument") as TextDocument;
            if (textDoc == null) return results;

            var (startLine, endLine) = CodeHelper.GetLineRange(textDoc, lineRange);

            for (int lineNum = startLine; lineNum <= endLine; lineNum++)
            {
                var line = CodeHelper.GetLineText(textDoc, lineNum);
                
                if (Regex.IsMatch(line, pattern))
                {
                    if (string.IsNullOrEmpty(searchText) || line.Contains(searchText))
                    {
                        results.Add(new CodeDetail
                        {
                            LineStart = lineNum,
                            CharStart = CodeHelper.GetCharStart(line, pattern),
                            CodeType = codeType,
                            CodeText = line.Trim(),
                            Context = document.Name
                        });
                    }
                }
            }

            return results;
        }
    }
}
