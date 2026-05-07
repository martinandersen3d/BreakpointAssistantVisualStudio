namespace BreakpointAssistant.Helpers
{
public static partial class CommandHelper
    {
        /// <summary>
        /// Helper class to store function information.
        /// </summary>
        public class FunctionInfo
        {
            public string Name { get; set; }
            public string FullName { get; set; }
            public int LineNumber { get; set; }
            public string Kind { get; set; }
        }
    }
}
