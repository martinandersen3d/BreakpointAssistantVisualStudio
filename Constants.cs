using System;

namespace BreakpointAssistant
{
    /// <summary>
    /// Contains all constant values used throughout the extension.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The package GUID for Breakpoint Assistant.
        /// Must match the GUID in BreakpointAssistantPackage and source.extension.vsixmanifest.
        /// </summary>
        public const string PackageGuidString = "22b2b6ca-7aa6-433d-b541-c079257991be";

        /// <summary>
        /// The command set GUID for all Breakpoint Assistant commands.
        /// Must match the GUID in VSCommandTable.vsct and RegisterCommands.cs.
        /// </summary>
        public const string CommandSetGuidString = "A8B9C7D6-E5F4-3A2B-1C0D-9E8F7A6B5C4D";

        /// <summary>
        /// The GUID for the Output window pane.
        /// </summary>
        public const string OutputWindowGuidString = "B8A7E7D3-9F4C-4E1A-8B3D-5F2E9C4A6B1D";

        /// <summary>
        /// Command IDs - must match VSCommandTable.vsct.
        /// </summary>
        public static class CommandIds
        {
            public const int ListFunctionLineNumbers = 0x0100;
        }

        /// <summary>
        /// Menu and group IDs - must match VSCommandTable.vsct.
        /// </summary>
        public static class MenuIds
        {
            public const int BreakpointAssistantMenuGroup = 0x1020;
            public const int BreakpointAssistantSubMenu = 0x1100;
            public const int BreakpointAssistantSubMenuGroup = 0x1150;
        }

        /// <summary>
        /// Extension metadata.
        /// </summary>
        public const string ExtensionName = "Breakpoint Assistant";
        public const string OutputWindowPaneName = "Breakpoint Assistant";
    }
}
