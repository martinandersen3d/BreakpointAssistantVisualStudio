using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using BreakpointAssistant.Services;
using BreakpointAssistant.Models;
using BreakpointAssistant.Helpers;

namespace BreakpointAssistant.Commands
{
    /// <summary>
    /// Contains the implementation of all extension commands.
    /// </summary>
    public static class ExtensionCommands
    {
        private static DTE2 _cachedDte;

        /// <summary>
        /// Lists all function line numbers in the active document.
        /// </summary>
        /// <param name="dte">The DTE2 service instance.</param>
        public static void ListFunctionLineNumbers(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                _cachedDte = dte;
                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine("=== List Function Line Numbers ===");

                // Get the active document
                Document activeDocument = dte.ActiveDocument;
                if (activeDocument == null)
                {
                    Services.OutputWindow.WriteLine("Error: No active document found. Please open a code file.");
                    return;
                }

                Services.OutputWindow.WriteLine($"Analyzing: {activeDocument.Name}");

                // Get the ProjectItem for the active document
                ProjectItem projectItem = activeDocument.ProjectItem;
                if (projectItem == null)
                {
                    Services.OutputWindow.WriteLine("Error: Could not retrieve project item for active document.");
                    return;
                }

                // Get the FileCodeModel
                FileCodeModel fileCodeModel = projectItem.FileCodeModel;
                if (fileCodeModel == null)
                {
                    Services.OutputWindow.WriteLine("Error: File does not support code model. This may not be a supported code file.");
                    return;
                }

                // Parse and list all functions
                var functions = new List<CommandHelper.FunctionInfo>();
                CommandHelper.ParseCodeElements(fileCodeModel.CodeElements, functions);

                if (functions.Count == 0)
                {
                    Services.OutputWindow.WriteLine("No functions found in the active document.");
                }
                else
                {
                    Services.OutputWindow.WriteLine($"\nFound {functions.Count} function(s):\n");
                    
                    foreach (var func in functions.OrderBy(f => f.LineNumber))
                    {
                        Services.OutputWindow.WriteLine($"  Line {func.LineNumber,4}: {func.FullName}");
                    }
                }

                Services.OutputWindow.WriteLine("\n=== Analysis Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ListFunctionLineNumbers exception: {ex}");
            }
        }

        /// <summary>
        /// Finds and lists all methods in the active document.
        /// </summary>
        public static void FindMethods(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindMethods("", lineRange);
            CommandHelper.DisplayResults("Methods", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all variable assignments in the active document.
        /// </summary>
        public static void FindVariableAssignments(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindVariableAssignments("", lineRange);
            CommandHelper.DisplayResults("Variable Assignments", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all properties in the active document.
        /// </summary>
        public static void FindProperties(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindProperties("", lineRange);
            CommandHelper.DisplayResults("Properties", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all property getters in the active document.
        /// </summary>
        public static void FindGetters(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindGetters("", lineRange);
            CommandHelper.DisplayResults("Property Getters", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all property setters in the active document.
        /// </summary>
        public static void FindSetters(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindSetters("", lineRange);
            CommandHelper.DisplayResults("Property Setters", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all if statements in the active document.
        /// </summary>
        public static void FindIf(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindIf("", lineRange);
            CommandHelper.DisplayResults("If Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all if-else statements in the active document.
        /// </summary>
        public static void FindIfElse(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindIfElse("", lineRange);
            CommandHelper.DisplayResults("If-Else Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all else statements in the active document.
        /// </summary>
        public static void FindElse(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindElse("", lineRange);
            CommandHelper.DisplayResults("Else Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all for loops in the active document.
        /// </summary>
        public static void FindFor(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindFor("", lineRange);
            CommandHelper.DisplayResults("For Loops", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all foreach loops in the active document.
        /// </summary>
        public static void FindForeach(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindForeach("", lineRange);
            CommandHelper.DisplayResults("Foreach Loops", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all while loops in the active document.
        /// </summary>
        public static void FindWhile(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindWhile("", lineRange);
            CommandHelper.DisplayResults("While Loops", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all do-while loops in the active document.
        /// </summary>
        public static void FindDoWhile(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindDoWhile("", lineRange);
            CommandHelper.DisplayResults("Do-While Loops", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all try-catch blocks in the active document.
        /// </summary>
        public static void FindTryCatch(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindTryCatch("", lineRange);
            CommandHelper.DisplayResults("Try-Catch Blocks", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all return statements in the active document.
        /// </summary>
        public static void FindReturn(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindReturn("", lineRange);
            CommandHelper.DisplayResults("Return Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all public methods in the active document.
        /// </summary>
        public static void FindPublicMethods(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindPublicMethods("", lineRange);
            CommandHelper.DisplayResults("Public Methods", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all private methods in the active document.
        /// </summary>
        public static void FindPrivateMethods(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindPrivateMethods("", lineRange);
            CommandHelper.DisplayResults("Private Methods", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all method closing braces in the active document.
        /// </summary>
        public static void FindMethodsClosingBrace(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindMethodsClosingBrace("", lineRange);
            CommandHelper.DisplayResults("Method Closing Braces", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all switch statements in the active document.
        /// </summary>
        public static void FindSwitch(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindSwitch("", lineRange);
            CommandHelper.DisplayResults("Switch Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all case statements in the active document.
        /// </summary>
        public static void FindCase(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindCase("", lineRange);
            CommandHelper.DisplayResults("Case Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all break statements in the active document.
        /// </summary>
        public static void FindBreak(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindBreak("", lineRange);
            CommandHelper.DisplayResults("Break Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all continue statements in the active document.
        /// </summary>
        public static void FindContinue(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindContinue("", lineRange);
            CommandHelper.DisplayResults("Continue Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all try blocks in the active document.
        /// </summary>
        public static void FindTry(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindTry("", lineRange);
            CommandHelper.DisplayResults("Try Blocks", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all catch blocks in the active document.
        /// </summary>
        public static void FindCatch(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindCatch("", lineRange);
            CommandHelper.DisplayResults("Catch Blocks", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all finally blocks in the active document.
        /// </summary>
        public static void FindFinally(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindFinally("", lineRange);
            CommandHelper.DisplayResults("Finally Blocks", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all throw statements in the active document.
        /// </summary>
        public static void FindThrow(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindThrow("", lineRange);
            CommandHelper.DisplayResults("Throw Statements", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all await expressions in the active document.
        /// </summary>
        public static void FindAwait(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindAwait("", lineRange);
            CommandHelper.DisplayResults("Await Expressions", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all constructors in the active document.
        /// </summary>
        public static void FindConstructor(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindConstructor("", lineRange);
            CommandHelper.DisplayResults("Constructors", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all ILogger references in the active document.
        /// </summary>
        public static void FindILogger(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindILogger("", lineRange);
            CommandHelper.DisplayResults("ILogger References", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all lambda expressions in the active document.
        /// </summary>
        public static void FindLambda(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindLambda("", lineRange);
            CommandHelper.DisplayResults("Lambda Expressions", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all LINQ queries in the active document.
        /// </summary>
        public static void FindLinq(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindLinq("", lineRange);
            CommandHelper.DisplayResults("LINQ Queries", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all 'new' keyword usage in the active document.
        /// </summary>
        public static void FindNew(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindNew("", lineRange);
            CommandHelper.DisplayResults("New Keywords", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all null references and checks in the active document.
        /// </summary>
        public static void FindNull(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindNull("", lineRange);
            CommandHelper.DisplayResults("Null References", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all custom method calls (downstream) in the active document.
        /// Excludes Microsoft, System, and framework methods.
        /// </summary>
        public static void FindMethodDownstream(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            var lineRange = CommandHelper.GetSelectedLineRange(dte);
            var locator = new CodeService(dte);
            var results = locator.FindMethodDownstream("", lineRange);
            CommandHelper.DisplayResults("Method Calls (Custom Methods Only)", results, lineRange);
            CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
        }

        /// <summary>
        /// Finds and lists all occurrences of custom text in the active document.
        /// </summary>
        public static void FindText(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;
            
            try
            {
                // Show input dialog to get search text from user
                var searchText = Services.InputDialog.ShowPrompt(
                    "Enter the text to search for:", 
                    "Find Text and Set Breakpoints", 
                    "");

                if (string.IsNullOrEmpty(searchText))
                {
                    Services.OutputWindow.WriteLine("Find Text: Operation cancelled or no text entered.");
                    return;
                }

                Services.OutputWindow.WriteLine($"Searching for text: '{searchText}'");
                
                var lineRange = CommandHelper.GetSelectedLineRange(dte);
                var locator = new CodeService(dte);
                var results = locator.FindCustomText(searchText, lineRange);
                CommandHelper.DisplayResults($"Text '{searchText}'", results, lineRange);
                CommandHelper.SetBreakpointState(dte, results, BreakpointState.Enable);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Text failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"FindText exception: {ex}");
            }
        }

        /// <summary>
        /// Shows the expert mode dialog for batch breakpoint operations.
        /// </summary>
        public static void ShowExpertMode(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;

            try
            {
                var activeDoc = dte.ActiveDocument;
                if (activeDoc == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: No active document.");
                    return;
                }

                var debugger = dte.Debugger;
                if (debugger == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Debugger service not available.");
                    return;
                }

                var documentName = System.IO.Path.GetFileName(activeDoc.FullName);
                var lineRange = CommandHelper.GetSelectedLineRange(dte);
                var lineSelection = lineRange != null 
                    ? lineRange.Item1 + "-" + lineRange.Item2
                    : "Entire Document";

                // Show the expert dialog
                var result = Services.BreakpointExpertDialog.Show(documentName, lineSelection, debugger);
                var commands = result.Item1;
                var action = result.Item2;
                var cancelled = result.Item3;

                if (cancelled || commands == null || commands.Count == 0)
                {
                    Services.OutputWindow.WriteLine("Expert mode cancelled.");
                    return;
                }

                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine("=== Expert Mode - Batch Operation ===");
                Services.OutputWindow.WriteLine("Document: " + documentName);
                Services.OutputWindow.WriteLine("Line Selection: " + lineSelection);
                Services.OutputWindow.WriteLine("Action: " + action);
                Services.OutputWindow.WriteLine("Selected Commands: " + commands.Count);
                Services.OutputWindow.WriteLine("");

                var locator = new CodeService(dte);
                var allResults = new List<CodeDetail>();

                // Execute search for each selected command
                foreach (var commandType in commands)
                {
                    Services.OutputWindow.WriteLine("Processing: " + commandType + "...");
                    var results = CommandHelper.FindByCommandType(locator, commandType, lineRange);
                    
                    if (results != null && results.Count > 0)
                    {
                        allResults.AddRange(results);
                        Services.OutputWindow.WriteLine("  Found " + results.Count + " locations");
                    }
                    else
                    {
                        Services.OutputWindow.WriteLine("  No matches found");
                    }
                }

                if (allResults.Count > 0)
                {
                    Services.OutputWindow.WriteLine("\nTotal locations found: " + allResults.Count);
                    Services.OutputWindow.WriteLine("Setting breakpoint state to: " + action + "\n");
                    
                    // Set breakpoint state without group
                    CommandHelper.SetBreakpointState(dte, allResults, action);
                }
                else
                {
                    Services.OutputWindow.WriteLine("\nNo locations found for the selected commands.");
                }

                Services.OutputWindow.WriteLine("\n=== Expert Mode Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine("Expert Mode failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("ShowExpertMode exception: " + ex);
            }
        }

        /// <summary>
        /// Enables all breakpoints in the active document or selected line range.
        /// </summary>
        public static void EnableBreakpoints(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;

            try
            {
                var debugger = dte.Debugger;
                if (debugger == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Debugger service not available.");
                    return;
                }

                var activeDoc = dte.ActiveDocument;
                if (activeDoc == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: No active document.");
                    return;
                }

                string fileName = activeDoc.FullName;
                var lineRange = CommandHelper.GetSelectedLineRange(dte);
                
                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine("=== Enable Breakpoints ===");
                Services.OutputWindow.WriteLine("Document: " + System.IO.Path.GetFileName(fileName));
                
                if (lineRange != null)
                {
                    Services.OutputWindow.WriteLine("Line Range: " + lineRange.Item1 + "-" + lineRange.Item2);
                }
                else
                {
                    Services.OutputWindow.WriteLine("Scope: Entire Document");
                }
                
                Services.OutputWindow.WriteLine("");

                int enabledCount = 0;
                foreach (Breakpoint bp in debugger.Breakpoints)
                {
                    if (bp.File.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check if breakpoint is in selected range
                        if (lineRange == null || (bp.FileLine >= lineRange.Item1 && bp.FileLine <= lineRange.Item2))
                        {
                            bp.Enabled = true;
                            Services.OutputWindow.WriteLine("  Enabled breakpoint at Line " + bp.FileLine);
                            enabledCount++;
                        }
                    }
                }

                Services.OutputWindow.WriteLine("\nEnabled " + enabledCount + " breakpoint(s)");
                Services.OutputWindow.WriteLine("=== Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine("Enable Breakpoints failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("EnableBreakpoints exception: " + ex);
            }
        }

        /// <summary>
        /// Disables all breakpoints in the active document or selected line range.
        /// </summary>
        public static void DisableBreakpoints(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;

            try
            {
                var debugger = dte.Debugger;
                if (debugger == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Debugger service not available.");
                    return;
                }

                var activeDoc = dte.ActiveDocument;
                if (activeDoc == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: No active document.");
                    return;
                }

                string fileName = activeDoc.FullName;
                var lineRange = CommandHelper.GetSelectedLineRange(dte);
                
                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine("=== Disable Breakpoints ===");
                Services.OutputWindow.WriteLine("Document: " + System.IO.Path.GetFileName(fileName));
                
                if (lineRange != null)
                {
                    Services.OutputWindow.WriteLine("Line Range: " + lineRange.Item1 + "-" + lineRange.Item2);
                }
                else
                {
                    Services.OutputWindow.WriteLine("Scope: Entire Document");
                }
                
                Services.OutputWindow.WriteLine("");

                int disabledCount = 0;
                foreach (Breakpoint bp in debugger.Breakpoints)
                {
                    if (bp.File.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check if breakpoint should be included (handles VS moved breakpoints to opening braces)
                        if (CommandHelper.ShouldIncludeBreakpoint(dte, bp.FileLine, lineRange))
                        {
                            bp.Enabled = false;
                            Services.OutputWindow.WriteLine("  Disabled breakpoint at Line " + bp.FileLine);
                            disabledCount++;
                        }
                    }
                }

                Services.OutputWindow.WriteLine("\nDisabled " + disabledCount + " breakpoint(s)");
                Services.OutputWindow.WriteLine("=== Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine("Disable Breakpoints failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DisableBreakpoints exception: " + ex);
            }
        }

        /// <summary>
        /// Deletes all breakpoints in the active document or selected line range.
        /// </summary>
        public static void DeleteBreakpoints(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _cachedDte = dte;

            try
            {
                var debugger = dte.Debugger;
                if (debugger == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Debugger service not available.");
                    return;
                }

                var activeDoc = dte.ActiveDocument;
                if (activeDoc == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: No active document.");
                    return;
                }

                string fileName = activeDoc.FullName;
                var lineRange = CommandHelper.GetSelectedLineRange(dte);
                
                Services.OutputWindow.Clear();
                Services.OutputWindow.Activate();
                Services.OutputWindow.WriteLine("=== Delete Breakpoints ===");
                Services.OutputWindow.WriteLine("Document: " + System.IO.Path.GetFileName(fileName));
                
                if (lineRange != null)
                {
                    Services.OutputWindow.WriteLine("Line Range: " + lineRange.Item1 + "-" + lineRange.Item2);
                }
                else
                {
                    Services.OutputWindow.WriteLine("Scope: Entire Document");
                }
                
                Services.OutputWindow.WriteLine("");

                // Collect breakpoints to delete first (can't delete while iterating)
                var breakpointsToDelete = new List<Breakpoint>();

                foreach (Breakpoint bp in debugger.Breakpoints)
                {
                    if (bp.File.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Check if breakpoint should be included (handles VS moved breakpoints to opening braces)
                        if (CommandHelper.ShouldIncludeBreakpoint(dte, bp.FileLine, lineRange))
                        {
                            breakpointsToDelete.Add(bp);
                        }
                    }
                }

                // Now delete them
                int deletedCount = 0;
                foreach (var bp in breakpointsToDelete)
                {
                    int line = bp.FileLine;
                    bp.Delete();
                    Services.OutputWindow.WriteLine("  Deleted breakpoint at Line " + line);
                    deletedCount++;
                }

                Services.OutputWindow.WriteLine("\nDeleted " + deletedCount + " breakpoint(s)");
                Services.OutputWindow.WriteLine("=== Complete ===");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine("Delete Breakpoints failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("DeleteBreakpoints exception: " + ex);
            }
        }
    }
}
