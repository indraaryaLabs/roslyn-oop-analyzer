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
                double mif = totalMethods > 0 ? (double)inheritedMethods / (totalMethods + inheritedMethods) : 0;
                double aif = totalAttributes > 0 ? (double)inheritedAttributes / (totalAttributes + inheritedAttributes) : 0;
                double cf = totalMethods > 1 ? (double)couplingRelations / (totalMethods * (totalMethods - 1)) : 0;
                double pf = totalMethods > 0 ? (double)polymorphicMethods / (totalMethods * (inheritedMethods > 0 ? inheritedMethods : 1)) : 0;

                // Formulas and Explanations
                string mhfFormula = "MHF = Hidden Methods / Total Methods";
                string mhfExplanation = $"Hidden Methods: {hiddenMethods}, Total Methods: {totalMethods}";

                string ahfFormula = "AHF = Hidden Attributes / Total Attributes";
                string ahfExplanation = $"Hidden Attributes: {hiddenAttributes}, Total Attributes: {totalAttributes}";

                string mifFormula = "MIF = Inherited Methods / (Total Methods + Inherited Methods)";
                string mifExplanation = $"Inherited Methods: {inheritedMethods}, Total Methods: {totalMethods}";

                string aifFormula = "AIF = Inherited Attributes / (Total Attributes + Inherited Attributes)";
                string aifExplanation = $"Inherited Attributes: {inheritedAttributes}, Total Attributes: {totalAttributes}";

                string cfFormula = "CF = Coupling Relations / (Total Methods * (Total Methods - 1))";
                string cfExplanation = $"Coupling Relations: {couplingRelations}, Total Methods: {totalMethods}";

                string pfFormula = "PF = Polymorphic Methods / (Total Methods * Inherited Methods)";
                string pfExplanation = $"Polymorphic Methods: {polymorphicMethods}, Total Methods: {totalMethods}, Inherited Methods: {inheritedMethods}";

                // Display Metrics with Explanations
                textBoxOutput.Text = $"Metrics Analysis:\n\n" +
                                     $"Method Hiding Factor (MHF): {mhf:P2}\n" +
                                     $"Formula: {mhfFormula}\nExplanation: {mhfExplanation}\n\n" +
                                     $"Attribute Hiding Factor (AHF): {ahf:P2}\n" +
                                     $"Formula: {ahfFormula}\nExplanation: {ahfExplanation}\n\n" +
                                     $"Method Inheritance Factor (MIF): {mif:P2}\n" +
                                     $"Formula: {mifFormula}\nExplanation: {mifExplanation}\n\n" +
                                     $"Attribute Inheritance Factor (AIF): {aif:P2}\n" +
                                     $"Formula: {aifFormula}\nExplanation: {aifExplanation}\n\n" +
                                     $"Coupling Factor (CF): {cf:P2}\n" +
                                     $"Formula: {cfFormula}\nExplanation: {cfExplanation}\n\n" +
                                     $"Polymorphism Factor (PF): {pf:P2}\n" +
                                     $"Formula: {pfFormula}\nExplanation: {pfExplanation}\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during analysis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper Functions for MOOD Metrics Analysis
        private int CountMethods(string sourceCode)
        {
            return sourceCode.Split(new[] { " void ", " int ", " string ", " public ", " private ", " protected " }, StringSplitOptions.None).Length - 1;
        }

        private int CountAttributes(string sourceCode)
        {
            return sourceCode.Split(new[] { ";" }, StringSplitOptions.None).Count(line => line.Contains("public") || line.Contains("private") || line.Contains("protected"));
        }

        private int CountHiddenMethods(string sourceCode)
        {
            return sourceCode.Split(new[] { "private " }, StringSplitOptions.None).Length - 1;
        }

        private int CountHiddenAttributes(string sourceCode)
        {
            return sourceCode.Split(new[] { "private " }, StringSplitOptions.None).Length - 1;
        }

        private int CountInheritedMethods(string sourceCode)
        {
            return sourceCode.Split(new[] { "override " }, StringSplitOptions.None).Length - 1;
        }

        private int CountInheritedAttributes(string sourceCode)
        {
            return 0; // Placeholder: Detect inherited attributes using advanced parsing
        }

        private int CountCouplingRelations(string sourceCode)
        {
            return sourceCode.Split(new[] { "." }, StringSplitOptions.None).Count(line => line.Contains("("));
        }

        private int CountPolymorphicMethods(string sourceCode)
        {
            return sourceCode.Split(new[] { "virtual ", "override " }, StringSplitOptions.None).Length - 1;
        }
    }
}
