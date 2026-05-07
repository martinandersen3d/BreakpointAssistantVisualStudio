using System;

namespace BreakpointAssistant.Services
{
    /// <summary>
    /// Simple input dialog for getting text input from the user.
    /// Uses Microsoft.VisualBasic.Interaction.InputBox which is available in .NET Framework.
    /// </summary>
    public static class InputDialog
    {
        /// <summary>
        /// Shows a prompt dialog to get text input from the user.
        /// </summary>
        /// <param name="question">The question/prompt to display.</param>
        /// <param name="title">The dialog title.</param>
        /// <param name="defaultValue">The default value to show.</param>
        /// <returns>The user's input, or null/empty if cancelled.</returns>
        public static string ShowPrompt(string question, string title = "Input", string defaultValue = "")
        {
            try
            {
                // Use Microsoft.VisualBasic.Interaction.InputBox
                // This requires adding a reference to Microsoft.VisualBasic
                return Microsoft.VisualBasic.Interaction.InputBox(question, title, defaultValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InputDialog error: {ex.Message}");
                return null;
            }
        }
    }
}
