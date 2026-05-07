using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BreakpointAssistant.Models;
using EnvDTE;

namespace BreakpointAssistant.Services
{
    /// <summary>
    /// Expert mode dialog for batch breakpoint operations.
    /// Allows multi-selection of commands to Enable, Disable, or Remove breakpoints.
    /// </summary>
    public class BreakpointExpertDialog : Form
    {
        private GroupBox _descriptionGroupBox;
        private Label _descriptionLabel;
        private Label _documentLabel;
        private Label _lineSelectionLabel;
        private Label _instructionLabel;
        private ListBox _commandListBox;
        private Button _enableButton;
        private Button _disableButton;
        private Button _removeButton;
        private ToolTip _toolTip;

        private readonly Debugger _debugger;
        private readonly string _documentName;

        public List<CodeDetailType> SelectedCommands { get; private set; }
        public BreakpointState SelectedAction { get; private set; }

        public BreakpointExpertDialog(string documentName, string lineSelection, Debugger debugger)
        {
            _documentName = documentName;
            _debugger = debugger;
            InitializeComponents(documentName, lineSelection);
            PopulateCommandList();
        }

        private void InitializeComponents(string documentName, string lineSelection)
        {
            // Form properties
            Text = "Breakpoint Bulk Actions";
            Width = 500;
            Height = 912;
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Initialize tooltip
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100
            };

            // Document label
            _documentLabel = new Label
            {
                Text = "Document: " + documentName,
                Left = 15,
                Top = 15,
                Width = 460,
                Height = 20
            };
            Controls.Add(_documentLabel);

            // Line selection label
            _lineSelectionLabel = new Label
            {
                Text = "Line Selection: " + lineSelection,
                Left = 15,
                Top = 40,
                Width = 460,
                Height = 20
            };
            Controls.Add(_lineSelectionLabel);

            // Instruction label
            _instructionLabel = new Label
            {
                Text = "Select one or more code patterns below, then choose an action (Hold Ctrl/Shift to multiselect)",
                Left = 15,
                Top = 70,
                Width = 460,
                Height = 20,
                ForeColor = Color.Gray
            };
            Controls.Add(_instructionLabel);

            // Command listbox
            _commandListBox = new ListBox
            {
                Left = 15,
                Top = 95,
                Width = 340,
                Height = 700,
                SelectionMode = SelectionMode.MultiExtended,
                Sorted = false,
                DrawMode = DrawMode.OwnerDrawVariable
            };
            _commandListBox.DrawItem += CommandListBox_DrawItem;
            _commandListBox.MeasureItem += CommandListBox_MeasureItem;
            _commandListBox.SelectedIndexChanged += CommandListBox_SelectedIndexChanged;
            Controls.Add(_commandListBox);

            // Enable button
            _enableButton = new Button
            {
                Text = "Enable",
                Left = 370,
                Top = 95,
                Width = 100,
                Height = 30
            };
            _enableButton.Click += (s, e) => HandleAction(BreakpointState.Enable);
            _toolTip.SetToolTip(_enableButton, "Enable breakpoints for all selected patterns in the line range");
            Controls.Add(_enableButton);

            // Disable button
            _disableButton = new Button
            {
                Text = "Disable",
                Left = 370,
                Top = 135,
                Width = 100,
                Height = 30
            };
            _disableButton.Click += (s, e) => HandleAction(BreakpointState.Disable);
            _toolTip.SetToolTip(_disableButton, "Disable breakpoints for all selected patterns in the line range");
            Controls.Add(_disableButton);

            // Remove button
            _removeButton = new Button
            {
                Text = "Remove",
                Left = 370,
                Top = 175,
                Width = 100,
                Height = 30
            };
            _removeButton.Click += (s, e) => HandleAction(BreakpointState.Remove);
            _toolTip.SetToolTip(_removeButton, "Remove breakpoints for all selected patterns in the line range");
            Controls.Add(_removeButton);

            // Description group box at bottom
            _descriptionGroupBox = new GroupBox
            {
                Text = "Description",
                Left = 15,
                Top = 810,
                Width = 460,
                Height = 60
            };
            Controls.Add(_descriptionGroupBox);

            // Description label inside group box
            _descriptionLabel = new Label
            {
                Text = "Batch set multiple breakpoints for specific code patterns",
                Left = 10,
                Top = 20,
                Width = 440,
                Height = 30
            };
            _descriptionGroupBox.Controls.Add(_descriptionLabel);
        }

        private void PopulateCommandList()
        {
            var orderedTypes = new List<CodeDetailType>
            {
                // Group 1: Methods, Return
                CodeDetailType.Method,
                CodeDetailType.MethodClosingBrace,
                CodeDetailType.MethodDownstream,
                CodeDetailType.PublicMethod,
                CodeDetailType.PrivateMethod,
                CodeDetailType.Return,

                // Group 2: If statements and Switch
                CodeDetailType.If,
                CodeDetailType.IfElse,
                CodeDetailType.Else,
                CodeDetailType.Switch,
                CodeDetailType.Case,

                // Group 3: Loops
                CodeDetailType.For,
                CodeDetailType.Foreach,
                CodeDetailType.While,
                CodeDetailType.DoWhile,
                CodeDetailType.Break,
                CodeDetailType.Continue,

                // Group 4: Exception Handling
                CodeDetailType.Try,
                CodeDetailType.Catch,
                CodeDetailType.Finally,
                CodeDetailType.Throw,

                // Group 5: Properties
                CodeDetailType.VariableAssignment,
                CodeDetailType.Property,
                CodeDetailType.Getter,
                CodeDetailType.Setter,

                // Group 6: Advanced
                CodeDetailType.Await,
                CodeDetailType.Constructor,
                CodeDetailType.ILogger,
                CodeDetailType.Lambda,
                CodeDetailType.Linq,
                CodeDetailType.New,
                CodeDetailType.Null
            };

            bool isFirstGroup = true;
            for (int i = 0; i < orderedTypes.Count; i++)
            {
                var type = orderedTypes[i];

                // Add separator before each new group
                if (i == 6 || i == 11 || i == 17 || i == 21 || i == 25)
                {
                    _commandListBox.Items.Add(new SeparatorItem());
                }

                _commandListBox.Items.Add(new CommandItem
                {
                    Type = type,
                    DisplayName = GetFriendlyName(type)
                });
            }

            _commandListBox.DisplayMember = "DisplayName";
        }

        private void CommandListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            var item = _commandListBox.Items[e.Index];

            if (item is SeparatorItem)
            {
                // Draw just a horizontal line as separator
                using (var pen = new Pen(Color.Gray, 1))
                {
                    int y = e.Bounds.Top + e.Bounds.Height / 2;
                    e.Graphics.DrawLine(pen, e.Bounds.Left + 10, y, e.Bounds.Right - 10, y);
                }
            }
            else if (item is CommandItem commandItem)
            {
                // Draw regular command item
                using (var brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(
                        commandItem.DisplayName,
                        e.Font,
                        brush,
                        e.Bounds.Left + 5,
                        e.Bounds.Top + 2);
                }
                e.DrawFocusRectangle();
            }
        }

        private void CommandListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var item = _commandListBox.Items[e.Index];
            if (item is SeparatorItem)
            {
                e.ItemHeight = 8;
            }
            else
            {
                e.ItemHeight = 20;
            }
        }

        private void CommandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Prevent separator items from being selected
            // Collect indices first to avoid modifying collection during iteration
            var separatorIndices = new List<int>();
            foreach (int index in _commandListBox.SelectedIndices)
            {
                if (_commandListBox.Items[index] is SeparatorItem)
                {
                    separatorIndices.Add(index);
                }
            }

            // Now deselect separator items
            foreach (int index in separatorIndices)
            {
                _commandListBox.SetSelected(index, false);
            }
        }

        private string GetFriendlyName(CodeDetailType type)
        {
            switch (type)
            {
                case CodeDetailType.Method: return "Methods";
                case CodeDetailType.PublicMethod: return "Public Methods";
                case CodeDetailType.PrivateMethod: return "Private Methods";
                case CodeDetailType.MethodClosingBrace: return "Methods Closing Brace";
                case CodeDetailType.MethodDownstream: return "Methods Downstream (Calls)";
                case CodeDetailType.Setter: return "Setters";
                case CodeDetailType.Getter: return "Getters";
                case CodeDetailType.Property: return "Properties";
                case CodeDetailType.VariableAssignment: return "Variable Assignments";
                case CodeDetailType.If: return "If";
                case CodeDetailType.IfElse: return "Else If";
                case CodeDetailType.Else: return "Else";
                case CodeDetailType.Switch: return "Switch";
                case CodeDetailType.Case: return "Case";
                case CodeDetailType.For: return "For";
                case CodeDetailType.Foreach: return "Foreach";
                case CodeDetailType.TryCatch: return "Try-Catch Blocks";
                case CodeDetailType.Try: return "Try";
                case CodeDetailType.Catch: return "Catch";
                case CodeDetailType.Finally: return "Finally";
                case CodeDetailType.Throw: return "Throw";
                case CodeDetailType.While: return "While";
                case CodeDetailType.DoWhile: return "Do-While";
                case CodeDetailType.Break: return "Break";
                case CodeDetailType.Continue: return "Continue";
                case CodeDetailType.Return: return "Return";
                case CodeDetailType.Await: return "Await";
                case CodeDetailType.Constructor: return "Constructors";
                case CodeDetailType.ILogger: return "ILogger";
                case CodeDetailType.Lambda: return "Lambda";
                case CodeDetailType.Linq: return "LINQ Queries";
                case CodeDetailType.New: return "New";
                case CodeDetailType.Null: return "Null";
                default: return type.ToString();
            }
        }

        private void HandleAction(BreakpointState action)
        {
            if (_commandListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one command from the list.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            SelectedCommands = _commandListBox.SelectedItems
                .Cast<object>()
                .OfType<CommandItem>()
                .Select(item => item.Type)
                .ToList();

            SelectedAction = action;

            DialogResult = DialogResult.OK;
            Close();
        }

        public static Tuple<List<CodeDetailType>, BreakpointState, bool> Show(
            string documentName, 
            string lineSelection,
            Debugger debugger)
        {
            using (var dialog = new BreakpointExpertDialog(documentName, lineSelection, debugger))
            {
                var result = dialog.ShowDialog();
                
                if (result == DialogResult.OK)
                {
                    return Tuple.Create(
                        dialog.SelectedCommands, 
                        dialog.SelectedAction,
                        false);
                }
                
                return Tuple.Create<List<CodeDetailType>, BreakpointState, bool>(
                    null, 
                    BreakpointState.Enable,
                    true);
            }
        }

        private class CommandItem
        {
            public CodeDetailType Type { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private class SeparatorItem
        {
        }
    }
}
