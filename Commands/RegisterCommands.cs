using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace BreakpointAssistant.Commands
{
    /// <summary>
    /// Handles registration of all extension commands with Visual Studio.
    /// </summary>
    public static class RegisterCommands
    {
        private static AsyncPackage _package;
        private static DTE2 _dte;

        /// <summary>
        /// Initializes all commands for the extension.
        /// </summary>
        /// <param name="package">The package instance.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            // Switch to the UI thread for command registration
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            try
            {
                // Get the command service
                IMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
                if (commandService == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Could not get IMenuCommandService");
                    return;
                }

                // Get DTE service
                _dte = await package.GetServiceAsync(typeof(EnvDTE.DTE)) as DTE2;
                if (_dte == null)
                {
                    Services.OutputWindow.WriteLine("ERROR: Could not get DTE2 service");
                }

                CommandHandlers.Initialize(_dte);
                Services.OutputWindow.WriteLine("Services acquired successfully");

                // Register all commands
                RegisterListFunctionLineNumbersCommand(commandService);
                RegisterFindMethodsCommand(commandService);
                RegisterFindPublicMethodsCommand(commandService);
                RegisterFindPrivateMethodsCommand(commandService);
                RegisterFindMethodsClosingBraceCommand(commandService);
                RegisterFindMethodDownstreamCommand(commandService);
                RegisterFindVariableAssignmentsCommand(commandService);
                RegisterFindPropertiesCommand(commandService);
                RegisterFindGettersCommand(commandService);
                RegisterFindSettersCommand(commandService);
                RegisterFindIfCommand(commandService);
                RegisterFindIfElseCommand(commandService);
                RegisterFindElseCommand(commandService);
                RegisterFindSwitchCommand(commandService);
                RegisterFindCaseCommand(commandService);
                RegisterFindForCommand(commandService);
                RegisterFindForeachCommand(commandService);
                RegisterFindWhileCommand(commandService);
                RegisterFindDoWhileCommand(commandService);
                RegisterFindBreakCommand(commandService);
                RegisterFindContinueCommand(commandService);
                RegisterFindTryCommand(commandService);
                RegisterFindCatchCommand(commandService);
                RegisterFindFinallyCommand(commandService);
                RegisterFindThrowCommand(commandService);
                RegisterFindTryCatchCommand(commandService);
                RegisterFindReturnCommand(commandService);
                RegisterFindAwaitCommand(commandService);
                RegisterFindConstructorCommand(commandService);
                RegisterFindILoggerCommand(commandService);
                RegisterFindLambdaCommand(commandService);
                RegisterFindLinqCommand(commandService);
                RegisterFindNewCommand(commandService);
                RegisterFindNullCommand(commandService);
                RegisterFindTextCommand(commandService);
                RegisterExpertModeCommand(commandService);
                RegisterEnableBreakpointsCommand(commandService);
                RegisterDisableBreakpointsCommand(commandService);
                RegisterDeleteBreakpointsCommand(commandService);

                Services.OutputWindow.WriteLine("All commands registered successfully");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"ERROR during initialization: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"RegisterCommands exception: {ex}");
            }
        }

        /// <summary>
        /// Registers the "List Function Line Numbers" command.
        /// </summary>
        /// <param name="commandService">The command service.</param>
        private static void RegisterListFunctionLineNumbersCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var commandId = new CommandID(CommandIds.CommandSet, CommandIds.ListFunctionLineNumbers);
                var menuCommand = new OleMenuCommand(CommandHandlers.OnListFunctionLineNumbers, commandId);
                commandService.AddCommand(menuCommand);

                Services.OutputWindow.WriteLine("Registered: List Function Line Numbers");
            }
            catch (Exception ex)
            {
                Services.OutputWindow.WriteLine($"ERROR registering command: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"RegisterListFunctionLineNumbersCommand exception: {ex}");
            }
        }

        private static void RegisterFindMethodsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindMethods);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindMethods, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Methods");
        }

        private static void RegisterFindPublicMethodsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindPublicMethods);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindPublicMethods, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Public Methods");
        }

        private static void RegisterFindPrivateMethodsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindPrivateMethods);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindPrivateMethods, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Private Methods");
        }

        private static void RegisterFindMethodsClosingBraceCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindMethodsClosingBrace);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindMethodsClosingBrace, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Methods Closing Brace");
        }

        private static void RegisterFindMethodDownstreamCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindMethodDownstream);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindMethodDownstream, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Method Downstream");
        }

        private static void RegisterFindPropertiesCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindProperties);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindProperties, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Properties");
        }

        private static void RegisterFindVariableAssignmentsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindVariableAssignments);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindVariableAssignments, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Variable Assignments");
        }

        private static void RegisterFindGettersCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindGetters);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindGetters, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Getters");
        }

        private static void RegisterFindSettersCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindSetters);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindSetters, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Setters");
        }

        private static void RegisterFindIfCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindIf);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindIf, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find If");
        }

        private static void RegisterFindIfElseCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindIfElse);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindIfElse, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find If-Else");
        }

        private static void RegisterFindElseCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindElse);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindElse, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Else");
        }

        private static void RegisterFindSwitchCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindSwitch);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindSwitch, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Switch");
        }

        private static void RegisterFindCaseCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindCase);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindCase, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Case");
        }

        private static void RegisterFindForCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindFor);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindFor, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find For");
        }

        private static void RegisterFindForeachCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindForeach);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindForeach, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Foreach");
        }

        private static void RegisterFindWhileCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindWhile);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindWhile, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find While");
        }

        private static void RegisterFindDoWhileCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindDoWhile);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindDoWhile, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Do-While");
        }

        private static void RegisterFindBreakCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindBreak);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindBreak, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Break");
        }

        private static void RegisterFindContinueCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindContinue);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindContinue, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Continue");
        }

        private static void RegisterFindTryCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindTry);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindTry, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Try");
        }

        private static void RegisterFindCatchCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindCatch);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindCatch, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Catch");
        }

        private static void RegisterFindFinallyCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindFinally);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindFinally, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Finally");
        }

        private static void RegisterFindThrowCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindThrow);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindThrow, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Throw");
        }

        private static void RegisterFindTryCatchCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindTryCatch);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindTryCatch, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Try-Catch");
        }

        private static void RegisterFindReturnCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindReturn);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindReturn, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Return");
        }

        private static void RegisterFindAwaitCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindAwait);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindAwait, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Await");
        }

        private static void RegisterFindConstructorCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindConstructor);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindConstructor, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Constructor");
        }

        private static void RegisterFindILoggerCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindILogger);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindILogger, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find ILogger");
        }

        private static void RegisterFindLambdaCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindLambda);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindLambda, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Lambda");
        }

        private static void RegisterFindLinqCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindLinq);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindLinq, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Linq");
        }

        private static void RegisterFindNewCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindNew);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindNew, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find New");
        }

        private static void RegisterFindNullCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindNull);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindNull, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Null");
        }

        private static void RegisterFindTextCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.FindText);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnFindText, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Find Text");
        }

        private static void RegisterExpertModeCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.ExpertMode);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnExpertMode, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Expert Mode");
        }

        private static void RegisterEnableBreakpointsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.EnableBreakpoints);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnEnableBreakpoints, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Enable Breakpoints");
        }

        private static void RegisterDisableBreakpointsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.DisableBreakpoints);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnDisableBreakpoints, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Disable Breakpoints");
        }

        private static void RegisterDeleteBreakpointsCommand(IMenuCommandService commandService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commandId = new CommandID(CommandIds.CommandSet, CommandIds.DeleteBreakpoints);
            var menuCommand = new OleMenuCommand(CommandHandlers.OnDeleteBreakpoints, commandId);
            commandService.AddCommand(menuCommand);
            Services.OutputWindow.WriteLine("Registered: Delete Breakpoints");
        }
    }
}
