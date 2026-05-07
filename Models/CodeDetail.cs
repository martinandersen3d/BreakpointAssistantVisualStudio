namespace BreakpointAssistant.Models
{
    /// <summary>
    /// Represents the location and type of a code element.
    /// </summary>
    public class CodeDetail
    {
        /// <summary>
        /// Gets or sets the line number where the code element starts (1-based).
        /// </summary>
        public int LineStart { get; set; }

        /// <summary>
        /// Gets or sets the character position on the line where the code element starts (1-based).
        /// </summary>
        public int CharStart { get; set; }

        /// <summary>
        /// Gets or sets the type of code element.
        /// </summary>
        public CodeDetailType CodeType { get; set; }

        /// <summary>
        /// Gets or sets the matched code text.
        /// </summary>
        public string CodeText { get; set; }

        /// <summary>
        /// Gets or sets additional context information (e.g., method name, variable name).
        /// </summary>
        public string Context { get; set; }
    }
}
