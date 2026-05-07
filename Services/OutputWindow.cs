using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace BreakpointAssistant.Services
{
    /// <summary>
    /// Provides logging capabilities to the Visual Studio Output window under "Breakpoint Assistant" pane.
    /// </summary>
    public static class OutputWindow
    {
        private static IVsOutputWindowPane _pane;
        private static IServiceProvider _serviceProvider;
        private static Guid _paneGuid = new Guid("B8A7E7D3-9F4C-4E1A-8B3D-5F2E9C4A6B1D");
        private const string PaneName = "Breakpoint Assistant";

        /// <summary>
        /// Initializes the output window pane.
        /// </summary>
        /// <param name="serviceProvider">The service provider from the package.</param>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            try
            {
                var outputWindow = _serviceProvider.GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                if (outputWindow == null)
                {
                    return;
                }

                // Try to get existing pane
                Guid paneGuid = _paneGuid;
                var hr = outputWindow.GetPane(ref paneGuid, out _pane);
                
                // If pane doesn't exist, create it
                if (hr != VSConstants.S_OK || _pane == null)
                {
                    paneGuid = _paneGuid;
                    outputWindow.CreatePane(ref paneGuid, PaneName, 1, 1);
                    paneGuid = _paneGuid;
                    outputWindow.GetPane(ref paneGuid, out _pane);
                }

                WriteLine("Breakpoint Assistant initialized successfully.");
            }
            catch (Exception ex)
            {
                // Fail silently if output window cannot be created
                System.Diagnostics.Debug.WriteLine($"Failed to initialize output window: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes a line of text to the output window.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public static void WriteLine(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_pane == null)
            {
                return;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                _pane.OutputString($"[{timestamp}] {message}\r\n");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to write to output window: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all content from the output window pane.
        /// </summary>
        public static void Clear()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_pane == null)
            {
                return;
            }

            try
            {
                _pane.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to clear output window: {ex.Message}");
            }
        }

        /// <summary>
        /// Activates and brings focus to the output window pane.
        /// </summary>
        public static void Activate()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_pane == null)
            {
                return;
            }

            try
            {
                _pane.Activate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to activate output window: {ex.Message}");
            }
        }
    }
}
