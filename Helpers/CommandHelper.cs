using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using BreakpointAssistant.Models;
using BreakpointAssistant.Services;

namespace BreakpointAssistant.Helpers
{
    /// <summary>
    /// Helper class containing utility methods for command operations.
    /// </summary>
    public static partial class CommandHelper
    {
        /// <summary>
        /// Sets the breakpoint state for a list of code locations.
        /// </summary>
        /// <param name="dte">The DTE2 service instance.</param>
        /// <param name="codeDetails">List of code details to set breakpoints for.</param>
        /// <param name="state">The desired breakpoint state (Enable, Disable, or Remove).</param>
        public static void SetBreakpointState(DTE2 dte, List<CodeDetail> codeDetails, BreakpointState state)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (codeDetails == null || codeDetails.Count == 0)
            {
                Services.OutputWindow.WriteLine("No code locations to set breakpoints for.");
                return;
            }

            if (dte == null)
            {
                Services.OutputWindow.WriteLine("ERROR: DTE service is not available.");
                return;
            }

            try
            {
                var debugger = dte.Debugger;
                if (debugger == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Debugger service is not available.");
                    return;
                }

                var activeDoc = dte.ActiveDocument;
                if (activeDoc == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: No active document.");
                    return;
                }

                string fileName = activeDoc.FullName;
                int successCount = 0;
                int errorCount = 0;

                Services.OutputWindow.WriteLine("\n--- Setting Breakpoint State: " + state + " ---");

                foreach (var detail in codeDetails)
                {
                    try
                    {
                        switch (state)
                        {
                            case BreakpointState.Enable:
                                // Check if breakpoint already exists at this location
                                var existingBp = FindBreakpointAt(debugger, fileName, detail.LineStart, dte);
                                if (existingBp != null)
                                {
                                    existingBp.Enabled = true;
                                    Services.OutputWindow.WriteLine("  Enabled existing breakpoint at Line " + existingBp.FileLine);
                                }
                                else
                                {
                                    // Create new breakpoint
                                    debugger.Breakpoints.Add(File: fileName, Line: detail.LineStart, Column: detail.CharStart);
                                    Services.OutputWindow.WriteLine("  Created breakpoint at Line " + detail.LineStart + ", Char " + detail.CharStart);
                                }
                                successCount++;
                                break;

                            case BreakpointState.Disable:
                                existingBp = FindBreakpointAt(debugger, fileName, detail.LineStart, dte);
                                if (existingBp != null)
                                {
                                    existingBp.Enabled = false;
                                    Services.OutputWindow.WriteLine("  Disabled breakpoint at Line " + existingBp.FileLine + " (pattern at Line " + detail.LineStart + ")");
                                    successCount++;
                                }
                                else
                                {
                                    Services.OutputWindow.WriteLine("  No breakpoint found at Line " + detail.LineStart);
                                }
                                break;

                            case BreakpointState.Remove:
                                existingBp = FindBreakpointAt(debugger, fileName, detail.LineStart, dte);
                                if (existingBp != null)
                                {
                                    int actualLine = existingBp.FileLine;
                                    existingBp.Delete();
                                    Services.OutputWindow.WriteLine("  Removed breakpoint at Line " + actualLine + " (pattern at Line " + detail.LineStart + ")");
                                    successCount++;
                                }
                                else
                                {
                                    Services.OutputWindow.WriteLine("  No breakpoint found at Line " + detail.LineStart);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Services.OutputWindow.WriteLine("  Error at Line " + detail.LineStart + ": " + ex.Message);
                        errorCount++;
                    }
                }

                Services.OutputWindow.WriteLine("\nBreakpoint operation complete: " + successCount + " successful, " + errorCount + " errors");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine("ERROR setting breakpoint state: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("SetBreakpointState exception: " + ex);
            }
        }

        /// <summary>
        /// Finds an existing breakpoint at the specified file and line.
        /// Also checks nearby lines (up to +2) for breakpoints on opening braces,
        /// since Visual Studio often moves breakpoints to the first executable line.
        /// </summary>
        /// <param name="debugger">The debugger instance.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="line">The line number.</param>
        /// <param name="dte">Optional DTE2 instance for reading line text to check for opening braces.</param>
        /// <returns>The breakpoint if found, otherwise null.</returns>
        public static Breakpoint FindBreakpointAt(Debugger debugger, string fileName, int line, DTE2 dte = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                // First, check the exact line
                foreach (Breakpoint bp in debugger.Breakpoints)
                {
                    if (bp.File.Equals(fileName, StringComparison.OrdinalIgnoreCase) && bp.FileLine == line)
                    {
                        return bp;
                    }
                }

                // If not found and we have DTE access, check next 2 lines for opening braces
                if (dte != null)
                {
                    const int lineBuffer = 2;
                    for (int offset = 1; offset <= lineBuffer; offset++)
                    {
                        int checkLine = line + offset;

                        // Check if this line has an opening brace
                        if (IsOpeningBraceLine(dte, checkLine))
                        {
                            // Look for a breakpoint on this brace line
                            foreach (Breakpoint bp in debugger.Breakpoints)
                            {
                                if (bp.File.Equals(fileName, StringComparison.OrdinalIgnoreCase) && bp.FileLine == checkLine)
                                {
                                    return bp;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding breakpoint: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Checks if a specific line contains an opening brace.
        /// </summary>
        private static bool IsOpeningBraceLine(DTE2 dte, int lineNumber)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var activeDocument = dte.ActiveDocument;
                if (activeDocument == null) return false;

                var textDoc = activeDocument.Object("TextDocument") as TextDocument;
                if (textDoc == null) return false;

                // Get the text at the specified line
                var editPoint = textDoc.CreateEditPoint(textDoc.StartPoint);
                editPoint.MoveToLineAndOffset(lineNumber, 1);
                var lineText = editPoint.GetLines(lineNumber, lineNumber + 1);

                // Check if line contains opening brace
                return lineText.Contains("{");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the selected line range from the active document, if any.
        /// </summary>
        /// <param name="dte">The DTE2 service instance.</param>
        /// <returns>A tuple with (startLine, endLine) if multiple lines are selected, otherwise null.</returns>
        public static Tuple<int, int> GetSelectedLineRange(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var activeDocument = dte.ActiveDocument;
                if (activeDocument == null) return null;

                var textDoc = activeDocument.Object("TextDocument") as TextDocument;
                if (textDoc == null) return null;

                var selection = textDoc.Selection;
                if (selection == null || selection.IsEmpty) return null;

                int startLine = selection.TopPoint.Line;
                int endLine = selection.BottomPoint.Line;

                // Return a range if one or more lines are selected
                if (endLine >= startLine)
                {
                    return Tuple.Create(startLine, endLine);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting selected line range: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a breakpoint line should be included when managing breakpoints in a selected range.
        /// Includes the breakpoint if it's in range, or if it's within the buffer and on an opening brace line.
        /// </summary>
        /// <param name="dte">The DTE2 service instance.</param>
        /// <param name="breakpointLine">The line number where the breakpoint is located.</param>
        /// <param name="lineRange">The selected line range (null for entire document).</param>
        /// <param name="lineBuffer">Number of lines to check beyond the range for opening braces.</param>
        /// <returns>True if the breakpoint should be included.</returns>
        public static bool ShouldIncludeBreakpoint(DTE2 dte, int breakpointLine, Tuple<int, int> lineRange, int lineBuffer = 2)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // No range selected = include all breakpoints in document
            if (lineRange == null) return true;

            // Breakpoint is within the selected range
            if (breakpointLine >= lineRange.Item1 && breakpointLine <= lineRange.Item2)
                return true;

            // Breakpoint is beyond the range but within the buffer
            if (breakpointLine > lineRange.Item2 && breakpointLine <= lineRange.Item2 + lineBuffer)
            {
                // Only include if the breakpoint line contains an opening brace
                try
                {
                    var activeDocument = dte.ActiveDocument;
                    if (activeDocument == null) return false;

                    var textDoc = activeDocument.Object("TextDocument") as TextDocument;
                    if (textDoc == null) return false;

                    // Get the text at the breakpoint line
                    var editPoint = textDoc.CreateEditPoint(textDoc.StartPoint);
                    editPoint.MoveToLineAndOffset(breakpointLine, 1);
                    var lineText = editPoint.GetLines(breakpointLine, breakpointLine + 1);

                    // Check if line contains opening brace
                    return lineText.Contains("{");
                }
                catch
                {
                    // If we can't read the line, don't include it
                    return false;
                }
            }

            // Breakpoint is outside the range
            return false;
        }

        /// <summary>
        /// Finds code elements by command type.
        /// </summary>
        /// <param name="locator">The code service instance.</param>
        /// <param name="commandType">The command type to search for.</param>
        /// <param name="lineRange">Optional line range to search within.</param>
        /// <returns>List of found code details.</returns>
        public static List<CodeDetail> FindByCommandType(CodeService locator, CodeDetailType commandType, Tuple<int, int> lineRange)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            switch (commandType)
            {
                case CodeDetailType.Method: return locator.FindMethods("", lineRange);
                case CodeDetailType.PublicMethod: return locator.FindPublicMethods("", lineRange);
                case CodeDetailType.PrivateMethod: return locator.FindPrivateMethods("", lineRange);
                case CodeDetailType.MethodClosingBrace: return locator.FindMethodsClosingBrace("", lineRange);
                case CodeDetailType.Property: return locator.FindProperties("", lineRange);
                case CodeDetailType.VariableAssignment: return locator.FindVariableAssignments("", lineRange);
                case CodeDetailType.Getter: return locator.FindGetters("", lineRange);
                case CodeDetailType.Setter: return locator.FindSetters("", lineRange);
                case CodeDetailType.If: return locator.FindIf("", lineRange);
                case CodeDetailType.IfElse: return locator.FindIfElse("", lineRange);
                case CodeDetailType.Else: return locator.FindElse("", lineRange);
                case CodeDetailType.Switch: return locator.FindSwitch("", lineRange);
                case CodeDetailType.Case: return locator.FindCase("", lineRange);
                case CodeDetailType.For: return locator.FindFor("", lineRange);
                case CodeDetailType.Foreach: return locator.FindForeach("", lineRange);
                case CodeDetailType.While: return locator.FindWhile("", lineRange);
                case CodeDetailType.DoWhile: return locator.FindDoWhile("", lineRange);
                case CodeDetailType.Break: return locator.FindBreak("", lineRange);
                case CodeDetailType.Continue: return locator.FindContinue("", lineRange);
                case CodeDetailType.Try: return locator.FindTry("", lineRange);
                case CodeDetailType.Catch: return locator.FindCatch("", lineRange);
                case CodeDetailType.Finally: return locator.FindFinally("", lineRange);
                case CodeDetailType.Throw: return locator.FindThrow("", lineRange);
                case CodeDetailType.TryCatch: return locator.FindTryCatch("", lineRange);
                case CodeDetailType.Return: return locator.FindReturn("", lineRange);
                case CodeDetailType.Await: return locator.FindAwait("", lineRange);
                case CodeDetailType.Constructor: return locator.FindConstructor("", lineRange);
                case CodeDetailType.ILogger: return locator.FindILogger("", lineRange);
                case CodeDetailType.Lambda: return locator.FindLambda("", lineRange);
                case CodeDetailType.Linq: return locator.FindLinq("", lineRange);
                case CodeDetailType.New: return locator.FindNew("", lineRange);
                case CodeDetailType.Null: return locator.FindNull("", lineRange);
                case CodeDetailType.MethodDownstream: return locator.FindMethodDownstream("", lineRange);
                default: return new List<CodeDetail>();
            }
        }

        /// <summary>
        /// Displays the search results in the output window.
        /// </summary>
        /// <param name="title">The title of the results.</param>
        /// <param name="results">The list of code details to display.</param>
        /// <param name="lineRange">Optional line range that was searched.</param>
        public static void DisplayResults(string title, List<CodeDetail> results, Tuple<int, int> lineRange = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine($"=== Find {title} ===");

                if (lineRange != null)
                {
                    Services.OutputWindow.WriteLine($"Searching in selected range: Lines {lineRange.Item1} - {lineRange.Item2}");
                }

                Services.OutputWindow.WriteLine(",");

                if (results == null || results.Count == 0)
                {
                    Services.OutputWindow.WriteLine($"No {title.ToLower()} found.");
                }
                else
                {
                    Services.OutputWindow.WriteLine($"Found {results.Count} {title.ToLower()}:\n");

                    foreach (var result in results.OrderBy(r => r.LineStart))
                    {
                        Services.OutputWindow.WriteLine($"  Line {result.LineStart,4}, Char {result.CharStart,3}: {result.CodeText}");
                        if (!string.IsNullOrEmpty(result.Context))
                        {
                            Services.OutputWindow.WriteLine($"         Context: {result.Context}");
                        }
                    }
                }

                Services.OutputWindow.WriteLine($"\n=== Analysis Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Error displaying results: {ex.Message}");
            }
        }

        /// <summary>
        /// Recursively parses code elements to find functions/methods.
        /// </summary>
        /// <param name="codeElements">The code elements to parse.</param>
        /// <param name="functions">The list to populate with found functions.</param>
        public static void ParseCodeElements(CodeElements codeElements, List<FunctionInfo> functions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (codeElements == null)
            {
                return;
            }

            foreach (CodeElement codeElement in codeElements)
            {
                try
                {
                    // Check if this is a function/method
                    if (codeElement.Kind == vsCMElement.vsCMElementFunction)
                    {
                        var function = codeElement as CodeFunction;
                        if (function != null)
                        {
                            int lineNumber = function.StartPoint.Line;
                            string functionName = function.Name;
                            string fullName = GetFullName(function);

                            functions.Add(new FunctionInfo
                            {
                                Name = functionName,
                                FullName = fullName,
                                LineNumber = lineNumber,
                                Kind = "Function"
                            });
                        }
                    }
                    // Check for property accessors
                    else if (codeElement.Kind == vsCMElement.vsCMElementProperty)
                    {
                        var property = codeElement as CodeProperty;
                        if (property != null)
                        {
                            // Get getter
                            if (property.Getter != null)
                            {
                                int lineNumber = property.Getter.StartPoint.Line;
                                functions.Add(new FunctionInfo
                                {
                                    Name = $"{property.Name}.get",
                                    FullName = $"{GetParentName(codeElement)}.{property.Name}.get",
                                    LineNumber = lineNumber,
                                    Kind = "Property Getter"
                                });
                            }

                            // Get setter
                            if (property.Setter != null)
                            {
                                int lineNumber = property.Setter.StartPoint.Line;
                                functions.Add(new FunctionInfo
                                {
                                    Name = $"{property.Name}.set",
                                    FullName = $"{GetParentName(codeElement)}.{property.Name}.set",
                                    LineNumber = lineNumber,
                                    Kind = "Property Setter"
                                });
                            }
                        }
                    }

                    // Recursively search nested elements (classes, namespaces, etc.)
                    if (codeElement.Children != null && codeElement.Children.Count > 0)
                    {
                        ParseCodeElements(codeElement.Children, functions);
                    }
                }
                catch (Exception ex)
                {
                    // Some code elements may throw exceptions when accessing properties
                    System.Diagnostics.Debug.WriteLine($"Error parsing code element: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the full name of a function including its parent class/namespace.
        /// </summary>
        /// <param name="function">The function code element.</param>
        /// <returns>The fully qualified function name.</returns>
        public static string GetFullName(CodeFunction function)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                return function.FullName;
            }
            catch
            {
                // Fallback to just the name if FullName is not available
                return function.Name;
            }
        }

        /// <summary>
        /// Gets the parent name (class or namespace) of a code element.
        /// </summary>
        /// <param name="element">The code element.</param>
        /// <returns>The parent name or empty string.</returns>
        public static string GetParentName(CodeElement element)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var parent = element.Collection?.Parent as CodeElement;
                if (parent != null)
                {
                    return parent.Name;
                }
            }
            catch
            {
                // Ignore
            }

            return string.Empty;
        }
    }
}
