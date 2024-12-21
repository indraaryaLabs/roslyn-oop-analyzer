using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MOOD_Metrics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            // Open File Dialog to Select C# Source File
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                Title = "Select a C# Source Code File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = openFileDialog.FileName; // Display file path

                try
                {
                    // Read and display source code
                    string sourceCode = File.ReadAllText(openFileDialog.FileName);
                    textBoxSource.Text = sourceCode;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            string sourceCode = textBoxSource.Text;
            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                MessageBox.Show("Please select and load a source code file first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform MOOD Metrics Analysis
            try
            {
                // Example analysis: Counts and dummy metrics
                int totalMethods = CountMethods(sourceCode);
                int totalAttributes = CountAttributes(sourceCode);
                int hiddenMethods = CountHiddenMethods(sourceCode);
                int hiddenAttributes = CountHiddenAttributes(sourceCode);
                int inheritedMethods = CountInheritedMethods(sourceCode);
                int inheritedAttributes = CountInheritedAttributes(sourceCode);
                int couplingRelations = CountCouplingRelations(sourceCode);
                int polymorphicMethods = CountPolymorphicMethods(sourceCode);

                // Calculate Metrics
                double mhf = totalMethods > 0 ? (double)hiddenMethods / totalMethods : 0;
                double ahf = totalAttributes > 0 ? (double)hiddenAttributes / totalAttributes : 0;
                double mif = totalMethods > 0 ? (double)inheritedMethods / totalMethods : 0;
                double aif = totalAttributes > 0 ? (double)inheritedAttributes / totalAttributes : 0;
                double cf = totalMethods > 1 ? (double)couplingRelations / (totalMethods * (totalMethods - 1)) : 0;
                double pf = totalMethods > 0 ? (double)polymorphicMethods / totalMethods : 0;

                // Display Metrics
                textBoxOutput.Text = $"Metrics Analysis:\n\n" +
                                     $"Method Hiding Factor (MHF): {mhf:P2}\n" +
                                     $"Attribute Hiding Factor (AHF): {ahf:P2}\n" +
                                     $"Method Inheritance Factor (MIF): {mif:P2}\n" +
                                     $"Attribute Inheritance Factor (AIF): {aif:P2}\n" +
                                     $"Coupling Factor (CF): {cf:P2}\n" +
                                     $"Polymorphism Factor (PF): {pf:P2}\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during analysis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Example Helper Functions for Analysis
        private int CountMethods(string sourceCode)
        {
            // Count methods by identifying "method-like" patterns
            return sourceCode.Split(new[] { "void ", "int ", "string ", "public ", "private ", "protected " }, StringSplitOptions.None).Length - 1;
        }

        private int CountAttributes(string sourceCode)
        {
            // Count attributes by identifying field declarations
            return sourceCode.Split(new[] { ";" }, StringSplitOptions.None).Count(line => line.Contains("public") || line.Contains("private") || line.Contains("protected"));
        }

        private int CountHiddenMethods(string sourceCode)
        {
            // Count methods declared as private
            return sourceCode.Split(new[] { "private " }, StringSplitOptions.None).Length - 1;
        }

        private int CountHiddenAttributes(string sourceCode)
        {
            // Count attributes declared as private
            return sourceCode.Split(new[] { "private " }, StringSplitOptions.None).Length - 1;
        }

        private int CountInheritedMethods(string sourceCode)
        {
            // Placeholder: Advanced parsing needed to detect inherited methods
            return 0;
        }

        private int CountInheritedAttributes(string sourceCode)
        {
            // Placeholder: Advanced parsing needed to detect inherited attributes
            return 0;
        }

        private int CountCouplingRelations(string sourceCode)
        {
            // Placeholder: Coupling calculation logic needed
            return 0;
        }

        private int CountPolymorphicMethods(string sourceCode)
        {
            // Placeholder: Polymorphic method detection logic needed
            return 0;
        }
    }
}
