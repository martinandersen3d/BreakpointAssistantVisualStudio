using System;

namespace BreakpointAssistant.Commands
{
    /// <summary>
    /// Contains all command IDs and the command set GUID for the extension.
    /// </summary>
    public static class CommandIds
    {
        // Command Set GUID - must match VSCommandTable.vsct
        public static readonly Guid CommandSet = new Guid("A8B9C7D6-E5F4-3A2B-1C0D-9E8F7A6B5C4D");

        // Code Analysis Commands
        public const int ListFunctionLineNumbers = 0x0100;

        // Method Search Commands
        public const int FindMethods = 0x0101;
        public const int FindPublicMethods = 0x010E;
        public const int FindPrivateMethods = 0x010F;
        public const int FindMethodsClosingBrace = 0x011F;
        public const int FindMethodDownstream = 0x0125;
        public const int FindConstructor = 0x0119;

        // Property Search Commands
        public const int FindVariableAssignments = 0x0126;
        public const int FindProperties = 0x0102;
        public const int FindGetters = 0x0103;
        public const int FindSetters = 0x0104;

        // Control Flow Commands
        public const int FindIf = 0x0105;
        public const int FindIfElse = 0x0106;
        public const int FindElse = 0x0107;
        public const int FindSwitch = 0x0110;
        public const int FindCase = 0x0111;

        // Loop Commands
        public const int FindFor = 0x0108;
        public const int FindForeach = 0x0109;
        public const int FindWhile = 0x010A;
        public const int FindDoWhile = 0x010B;
        public const int FindBreak = 0x0112;
        public const int FindContinue = 0x0113;

        // Exception Handling Commands
        public const int FindTryCatch = 0x010C;
        public const int FindTry = 0x0114;
        public const int FindCatch = 0x0115;
        public const int FindFinally = 0x0116;
        public const int FindThrow = 0x0117;

        // Advanced Language Features
        public const int FindReturn = 0x010D;
        public const int FindAwait = 0x0118;
        public const int FindILogger = 0x011A;
        public const int FindLambda = 0x011B;
        public const int FindLinq = 0x011C;
        public const int FindNew = 0x011D;
        public const int FindNull = 0x011E;

        // Specialized Commands
        public const int FindText = 0x0120;
        public const int ExpertMode = 0x0121;

        // Breakpoint Management Commands
        public const int EnableBreakpoints = 0x0122;
        public const int DisableBreakpoints = 0x0123;
        public const int DeleteBreakpoints = 0x0124;
    }
}
