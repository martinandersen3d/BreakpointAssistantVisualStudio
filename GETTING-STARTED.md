# BreakpointStudio - Getting Started Guide

Welcome to **BreakpointStudio**! This Visual Studio extension supercharges your debugging workflow by automatically setting breakpoints on specific code patterns in your C# files.

---

## 🚀 Quick Start

### Accessing BreakpointStudio

There are **two ways** to access BreakpointStudio features:

1. **Extensions Menu**: Go to **Extensions** → **Breakpoint Assistant**
2. **Context Menu**: Right-click in the code editor → **Breakpoint Assistant**

### Basic Workflow

1. Open a C# code file
2. **(Optional)** Select a range of code to limit the scope
3. Choose a feature from the **Breakpoint Assistant** menu
4. View results in the **Breakpoint Assistant** Output window

> 💡 **Tip**: To view the Output window, press `Ctrl+Alt+O` and select **"Breakpoint Assistant"** from the dropdown.

---

## 🎯 Key Features

### 1. Breakpoint Expert Mode (Bulk Actions)

The most powerful feature! Opens an interactive dialog where you can:
- Select **multiple code patterns** at once (hold Ctrl/Shift to multiselect)
- Choose an action: **Enable**, **Disable**, or **Remove** breakpoints
- Apply changes to all selected patterns in one operation

**Use Case**: Quickly debug multiple related patterns like all try-catch blocks and exception throws.

### 2. Find Text and Set Breakpoints

Search for specific text in your code and automatically place breakpoints on matching lines.

**Use Case**: Set breakpoints on all lines containing "TODO" or a specific variable name.

---

## 📍 Breakpoint Patterns

### Methods & Functions
- **Methods** - First line of all method declarations
- **Public/Private Methods** - Target specific method visibility
- **Methods Closing Brace** - Track method exits
- **Methods Downstream** - Method invocations/calls
- **Constructors** - Constructor declarations
- **Return** - All return statements

### Conditional Logic
- **If/Else If/Else** - Conditional branches
- **Switch/Case** - Switch statement logic

### Loops
- **For/Foreach** - Loop declarations
- **While/Do-While** - Loop conditions
- **Break/Continue** - Loop control statements

### Exception Handling
- **Try/Catch/Finally** - Exception blocks
- **Throw** - Exception throw statements

### Properties & Variables
- **Properties** - Property declarations
- **Getters/Setters** - Property accessors
- **Variable Assignments** - All assignment statements

### Advanced Patterns
- **Await** - Async/await expressions
- **ILogger** - Logging statements
- **Lambda** - Lambda expressions
- **LINQ Queries** - LINQ expressions
- **New** - Object instantiation
- **Null** - Null checks and assignments

---

## 🔧 Breakpoint Management

Manage existing breakpoints in your document or selection:

- **Enable Breakpoints** - Activate all breakpoints in scope
- **Disable Breakpoints** - Deactivate without removing
- **Delete Breakpoints** - Remove all breakpoints in scope

---

## 💡 Tips & Best Practices

### Work with Selections
Select a range of code before invoking a feature to limit the scope. This is especially useful for large files.

### Use Expert Mode for Efficiency
When debugging complex scenarios, use **Breakpoint Expert Mode** to set multiple patterns at once instead of running individual commands.

### Review the Output Window
All operations are logged to the **Breakpoint Assistant** Output window, showing exactly which lines had breakpoints set.

### Common Debugging Scenarios

**Scenario 1: Debug a method flow**
- Use **Methods** + **Return** + **If** to track all decision points

**Scenario 2: Track async operations**
- Use **Await** + **Try/Catch** to debug async/await patterns

**Scenario 3: Monitor exception handling**
- Use **Try/Catch/Finally** + **Throw** to see exception flow

**Scenario 4: Debug loops**
- Use **For/Foreach/While** + **Break/Continue** to track loop execution

---

## 🎓 Example Walkthrough

Let's debug a problematic method:

1. Open your C# file containing the method
2. Select the method's code block
3. Right-click → **Breakpoint Assistant** → **Breakpoint Expert Mode**
4. In the dialog, select:
   - **Methods**
   - **If**
   - **Return**
5. Click **Enable**
6. Start debugging - breakpoints are now set at all key decision points!

---

## 📺 Output Window

All results appear in the **Breakpoint Assistant** pane of the Output window:
- Number of matches found
- Line numbers where breakpoints were set
- Any errors or warnings

**To open**: `View` → `Output` (or `Ctrl+Alt+O`), then select **"Breakpoint Assistant"** from the dropdown.

---

## ⌨️ Keyboard Shortcuts

No default shortcuts are assigned, but you can customize them:
1. Go to **Tools** → **Options** → **Environment** → **Keyboard**
2. Search for "BreakpointAssistant"
3. Assign your preferred shortcuts

---

## 🎉 You're Ready!

You now have everything you need to start using BreakpointStudio. Happy debugging! 🐛

For issues or feature requests, visit the GitHub repository.
   - Update `Models/CodeDetailType.cs` with new pattern type
   - Add detection logic in `Services/CodeService.cs`
   - Register command in `VSCommandTable.vsct`

2. **Modify UI:**
   - Edit `Services/BreakpointExpertDialog.cs` for the expert dialog
   - Update `VSCommandTable.vsct` for menu structure

3. **Debug:**
   - Set breakpoints in your code
   - Press **F5** and trigger commands in the experimental instance
   - Use Debug > Attach to Process if needed

## Building for Release

1. Set build configuration to **Release**
2. Build the project (Ctrl+Shift+B)
3. Find the `.vsix` file in `bin/Release/`
4. Double-click to install or distribute

## Need Help?

- Check the [README.md](README.md) for feature documentation
- Review existing code patterns in `Services/CodeService.cs`
- Visual Studio Output window shows extension logs
