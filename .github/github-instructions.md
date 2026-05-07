# Breakpoint Assistant - Visual Studio Extension

## Project Overview
This is a Visual Studio extension (VSIX) targeting Visual Studio 2022 (17.0+) built on .NET Framework 4.7.2. The extension provides code analysis and breakpoint management tools accessible through the Tools menu.

## Project Structure

```
BreakpointAssistant/
??? BreakpointAssistantPackage.cs          # Main package entry point
??? source.extension.vsixmanifest        # VSIX manifest
??? Commands/                            # Command implementations
?   ??? RegisterCommands.cs              # Command registration and initialization
?   ??? ExtensionCommands.cs             # Command functionality implementations
??? Services/                            # Helper services
?   ??? OutputWindow.cs                  # Output window logging service
??? Resources/                           # Icons, images, and resources
??? Properties/
    ??? AssemblyInfo.cs                  # Assembly metadata
```

## Key Components

### 1. BreakpointAssistantPackage.cs
- **Location**: Root directory
- **Purpose**: Main package class that inherits from `AsyncPackage`
- **Responsibilities**:
  - Package initialization
  - Service registration
  - Call `RegisterCommands.Initialize()` in `InitializeAsync()`
- **Key Attributes**:
  - `[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]`
  - `[Guid(BreakpointAssistantPackage.PackageGuidString)]`
  - `[ProvideMenuResource("Menus.ctmenu", 1)]` for menu integration

### 2. Commands/RegisterCommands.cs
- **Location**: `Commands/` folder
- **Purpose**: Centralized command registration
- **Responsibilities**:
  - Initialize all commands during package load
  - Register command handlers with VS command service
  - Create menu command bindings
- **Pattern**:
  ```csharp
  public static class RegisterCommands
  {
      public static async Task InitializeAsync(AsyncPackage package)
      {
          await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
          var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
          
          // Register each command here
      }
  }
  ```

### 3. Commands/ExtensionCommands.cs
- **Location**: `Commands/` folder
- **Purpose**: Implements all command functionality
- **Responsibilities**:
  - Contains the actual logic for each command
  - Interacts with DTE (Development Tools Environment) API
  - Uses OutputWindow service for logging
- **Pattern**: Each command should be a static method that can be invoked by command handlers
- **Key APIs to use**:
  - `DTE2` for accessing active document
  - `TextDocument` for document content
  - `CodeModel` and `FileCodeModel` for parsing code structure

### 4. Services/OutputWindow.cs
- **Location**: `Services/` folder
- **Purpose**: Provides logging capabilities to Visual Studio Output window
- **Requirements**:
  - Create a custom output pane named "Breakpoint Assistant"
  - Provide methods: `Initialize()`, `WriteLine()`, `Clear()`, `Activate()`
  - Should be thread-safe and switch to UI thread when needed
- **Pattern**:
  ```csharp
  public static class OutputWindow
  {
      private static IVsOutputWindowPane _pane;
      
      public static void Initialize(IServiceProvider serviceProvider) { }
      public static void WriteLine(string message) { }
      public static void Clear() { }
      public static void Activate() { }
  }
  ```

## Commands to Implement

### Tools Menu Structure
- **Menu Path**: Tools > Breakpoint Assistant > [Commands]
- **Menu GUID**: Define in a constants class or vsct file
- **Commands**:
  1. **List Function Line Numbers** - Lists all line numbers where functions are defined in the active document

## Command Implementation Guide

### List Function Line Numbers Command
- **ID**: CommandId should be unique (e.g., 0x0100)
- **Functionality**:
  1. Get the active document from DTE
  2. Parse the document using `FileCodeModel`
  3. Iterate through `CodeElements` to find functions/methods
  4. Extract line numbers using `CodeElement.StartPoint.Line`
  5. Output results to "Breakpoint Assistant" output pane
- **Output Format**: `Function: {FunctionName} at line {LineNumber}`

## Coding Standards

### Thread Safety
- Always use `ThreadHelper.ThrowIfNotOnUIThread()` when accessing VS services
- Use `await JoinableTaskFactory.SwitchToMainThreadAsync()` for async operations
- Never block UI thread with synchronous operations

### Error Handling
- Wrap command logic in try-catch blocks
- Log errors to the Output window
- Show user-friendly messages via `VsShellUtilities.ShowMessageBox()`
- Never let exceptions crash the extension

### Naming Conventions
- Use PascalCase for classes, methods, and properties
- Use descriptive names (e.g., `ListFunctionLineNumbers` not `ListFunc`)
- Command IDs should be constants with clear names

### Dependencies
- Use Microsoft.VisualStudio.SDK for VS integration
- Use Microsoft.VisualStudio.Shell.15.0 for package infrastructure
- Use EnvDTE and EnvDTE80 for DTE automation
- Minimize external dependencies

## Service Usage Patterns

### Getting DTE Service
```csharp
var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
```

### Getting Active Document
```csharp
ThreadHelper.ThrowIfNotOnUIThread();
var activeDoc = dte.ActiveDocument;
if (activeDoc == null) return;
```

### Getting Command Service
```csharp
var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
```

## Menu Integration

### VSCT File (if used)
- Define command group GUID
- Define command IDs as sequential integers
- Place commands under Tools menu
- Create submenu "Breakpoint Assistant"
- Add commands to submenu

### Alternative: Code-based Registration
- Use `MenuCommand` class to create commands dynamically
- Use `CommandID` with proper GUID and ID
- Add commands to `OleMenuCommandService`

## Best Practices

1. **Initialization**: All service initialization should happen in `InitializeAsync()`
2. **Lazy Loading**: Use `ProvideAutoLoad` attributes carefully to avoid impacting startup time
3. **Logging**: Always log command execution start/end to Output window
4. **Validation**: Check for null references (active document, text selection, etc.)
5. **Performance**: Cache services when possible, don't query repeatedly
6. **User Feedback**: Provide clear messages about what the command is doing
7. **Async/Await**: Use async patterns throughout, switch to UI thread only when needed

## Common Pitfalls to Avoid

- Don't access DTE on background threads without switching to UI thread
- Don't create multiple output panes with the same name
- Don't forget to null-check active document
- Don't hardcode file paths or assume file extensions
- Don't block the UI thread with long-running operations

## Testing Checklist

- [ ] Commands appear in Tools > Breakpoint Assistant menu
- [ ] Output window pane "Breakpoint Assistant" is created
- [ ] Active document detection works
- [ ] Function parsing works for C#, VB, and other languages
- [ ] Error messages appear in output window
- [ ] No exceptions thrown for edge cases (no document, empty file, etc.)

## Extension Metadata

- **Package GUID**: `22b2b6ca-7aa6-433d-b541-c079257991be`
- **Target Framework**: .NET Framework 4.7.2
- **VS Version**: 17.0 - 18.0 (Visual Studio 2022)
- **Architecture**: amd64

## File Creation Order

When implementing new features, create files in this order:
1. `Services/OutputWindow.cs` - Logging infrastructure first
2. `Commands/ExtensionCommands.cs` - Command logic
3. `Commands/RegisterCommands.cs` - Command registration
4. Update `BreakpointAssistantPackage.cs` - Wire everything together

## Code Generation Guidelines

When generating code for this extension:
- Use `Microsoft.VisualStudio.Shell` namespace for package infrastructure
- Use `EnvDTE` and `EnvDTE80` for IDE automation
- Use `Microsoft.VisualStudio.Shell.Interop` for low-level VS services
- Always include proper using statements
- Add XML documentation comments to public methods
- Include error handling in all command handlers
- Log all important operations to the Output window
- Use async/await patterns consistently
- Follow Visual Studio SDK best practices for command implementation
