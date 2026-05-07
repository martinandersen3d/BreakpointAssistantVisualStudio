using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace BreakpointAssistant.Commands
{
    internal static class CommandHandlers
    {
        private static DTE2 _dte;

        public static void Initialize(DTE2 dte)
        {
            _dte = dte;
        }

        public static void OnListFunctionLineNumbers(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                Services.OutputWindow.WriteLine("Command invoked: List Function Line Numbers");
                if (_dte == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: DTE service is not available.");
                    return;
                }
                ExtensionCommands.ListFunctionLineNumbers(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Command execution failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"OnListFunctionLineNumbers exception: {ex}");
            }
        }

        public static void OnFindMethods(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindMethods(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Methods failed: {ex.Message}");
            }
        }

        public static void OnFindPublicMethods(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindPublicMethods(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Public Methods failed: {ex.Message}");
            }
        }

        public static void OnFindPrivateMethods(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindPrivateMethods(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Private Methods failed: {ex.Message}");
            }
        }

        public static void OnFindProperties(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindProperties(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Properties failed: {ex.Message}");
            }
        }

        public static void OnFindVariableAssignments(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindVariableAssignments(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Variable Assignments failed: {ex.Message}");
            }
        }

        public static void OnFindGetters(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindGetters(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Getters failed: {ex.Message}");
            }
        }

        public static void OnFindSetters(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindSetters(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Setters failed: {ex.Message}");
            }
        }

        public static void OnFindIf(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindIf(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find If failed: {ex.Message}");
            }
        }

        public static void OnFindIfElse(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindIfElse(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find If-Else failed: {ex.Message}");
            }
        }

        public static void OnFindElse(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindElse(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Else failed: {ex.Message}");
            }
        }

        public static void OnFindSwitch(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindSwitch(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Switch failed: {ex.Message}");
            }
        }

        public static void OnFindCase(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindCase(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Case failed: {ex.Message}");
            }
        }

        public static void OnFindFor(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindFor(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find For failed: {ex.Message}");
            }
        }

        public static void OnFindForeach(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindForeach(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Foreach failed: {ex.Message}");
            }
        }

        public static void OnFindWhile(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindWhile(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find While failed: {ex.Message}");
            }
        }

        public static void OnFindDoWhile(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindDoWhile(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Do-While failed: {ex.Message}");
            }
        }

        public static void OnFindBreak(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindBreak(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Break failed: {ex.Message}");
            }
        }

        public static void OnFindContinue(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindContinue(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Continue failed: {ex.Message}");
            }
        }

        public static void OnFindTry(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindTry(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Try failed: {ex.Message}");
            }
        }

        public static void OnFindCatch(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindCatch(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Catch failed: {ex.Message}");
            }
        }

        public static void OnFindFinally(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindFinally(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Finally failed: {ex.Message}");
            }
        }

        public static void OnFindThrow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindThrow(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Throw failed: {ex.Message}");
            }
        }

        public static void OnFindTryCatch(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindTryCatch(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Try-Catch failed: {ex.Message}");
            }
        }

        public static void OnFindReturn(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindReturn(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Return failed: {ex.Message}");
            }
        }

        public static void OnFindAwait(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindAwait(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Await failed: {ex.Message}");
            }
        }

        public static void OnFindConstructor(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindConstructor(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Constructor failed: {ex.Message}");
            }
        }

        public static void OnFindILogger(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindILogger(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find ILogger failed: {ex.Message}");
            }
        }

        public static void OnFindLambda(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindLambda(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Lambda failed: {ex.Message}");
            }
        }

        public static void OnFindLinq(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindLinq(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Linq failed: {ex.Message}");
            }
        }

        public static void OnFindNew(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindNew(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find New failed: {ex.Message}");
            }
        }

        public static void OnFindNull(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindNull(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Null failed: {ex.Message}");
            }
        }

        public static void OnFindMethodsClosingBrace(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindMethodsClosingBrace(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Methods Closing Brace failed: {ex.Message}");
            }
        }

        public static void OnFindMethodDownstream(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindMethodDownstream(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Method Downstream failed: {ex.Message}");
            }
        }

        public static void OnFindText(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.FindText(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Find Text failed: {ex.Message}");
            }
        }

        public static void OnExpertMode(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.ShowExpertMode(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Expert Mode failed: {ex.Message}");
            }
        }

        public static void OnEnableBreakpoints(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.EnableBreakpoints(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Enable Breakpoints failed: {ex.Message}");
            }
        }

        public static void OnDisableBreakpoints(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.DisableBreakpoints(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Disable Breakpoints failed: {ex.Message}");
            }
        }

        public static void OnDeleteBreakpoints(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_dte == null) return;
                ExtensionCommands.DeleteBreakpoints(_dte);
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"Delete Breakpoints failed: {ex.Message}");
            }
        }
    }
}
