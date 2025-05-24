using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic; // Added for List and HashSet
using System.Text; // Added for StringBuilder

namespace MOOD_Metrics
{
    public partial class Form1 : Form
    {
        private List<string> csharpFiles = new List<string>(); // To store paths of selected C# files

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Pilih folder yang berisi file Kode Sumber C#"
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowserDialog.SelectedPath;
                csharpFiles.Clear(); // Clear previous file list

                try
                {
                    string[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.cs", SearchOption.AllDirectories);

                    if (files.Length == 0)
                    {
                        MessageBox.Show("Tidak ada file C# ditemukan di folder yang dipilih atau subfoldernya.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxSource.Text = ""; // Clear source code text box
                        return;
                    }

                    csharpFiles.AddRange(files);

                    if (csharpFiles.Any())
                    {
                        textBoxSource.Text = $"Ditemukan {csharpFiles.Count} file C# di '{folderBrowserDialog.SelectedPath}' (termasuk subfolder):{Environment.NewLine}{Environment.NewLine}" +
                                             string.Join(Environment.NewLine, csharpFiles.Select(Path.GetFileName)) +
                                             $"{Environment.NewLine}{Environment.NewLine}Klik 'Analyze' untuk memproses semua file.";
                    }
                    else
                    {
                        textBoxSource.Text = "Tidak ada file C# yang dipilih.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error membaca folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxSource.Text = "";
                }
            }
        }

        private void buttonAnalyze_Click(object sender, EventArgs e)
        {
            if (!csharpFiles.Any())
            {
                MessageBox.Show("Silakan pilih folder dengan file C# terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Aggregate metrics across all files for overall summary
            List<ClassMetric> allClassMetricsMHF = new List<ClassMetric>();
            List<ClassAttributeMetric> allClassMetricsAHF = new List<ClassAttributeMetric>();
            List<ClassInheritanceMetric> allClassMetricsMIF = new List<ClassInheritanceMetric>();
            List<ClassAttributeInheritanceMetric> allClassMetricsAIF = new List<ClassAttributeInheritanceMetric>();
            List<ClassCouplingMetric> allClassMetricsCF = new List<ClassCouplingMetric>();
            List<ClassPolymorphismMetric> allClassMetricsPF = new List<ClassPolymorphismMetric>();

            textBoxOutput.Clear();    // Clear detailed output
            textBoxSummary.Clear(); // Clear overall summary
            textBoxOutput.Text += $"Menganalisis file di: {textBoxPath.Text}{Environment.NewLine}";
            textBoxOutput.Text += $"-----------------------------------------{Environment.NewLine}{Environment.NewLine}";

            // First pass: Collect all class names in the entire system for global analysis (CF & PF)
            HashSet<string> allClassNamesInSystem = new HashSet<string>();
            foreach (string filePath in csharpFiles)
            {
                try
                {
                    string sourceCode = File.ReadAllText(filePath);
                    var classesInFile = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);
                    foreach (var clsDef in classesInFile.Skip(1))
                    {
                        string className = clsDef.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(className))
                        {
                            allClassNamesInSystem.Add(className);
                        }
                    }
                }
                catch (Exception ex)
                {
                    textBoxOutput.Text += $"Peringatan: Tidak dapat membaca kelas dari {Path.GetFileName(filePath)} untuk CF/PF global: {ex.Message}{Environment.NewLine}";
                }
            }

            // Second pass: Perform detailed analysis for each file and accumulate global metrics
            foreach (string filePath in csharpFiles)
            {
                try
                {
                    string sourceCode = File.ReadAllText(filePath);
                    string relativePath = Path.GetRelativePath(textBoxPath.Text, filePath);

                    textBoxOutput.Text += $"--- Detail Analisis untuk: {relativePath} ---{Environment.NewLine}";

                    var fileClassMetricsMHF = CalculateClassMetrics(sourceCode);
                    var fileClassMetricsAHF = CalculateClassMetricsForAHF(sourceCode);
                    var fileClassMetricsMIF = CalculateClassMetricsForMIF(sourceCode);
                    var fileClassMetricsAIF = CalculateClassMetricsForAIF(sourceCode);
                    var fileClassMetricsCF = CalculateCouplingFactor(sourceCode, allClassNamesInSystem);
                    var fileClassMetricsPF = CalculatePolymorphismFactor(sourceCode, allClassNamesInSystem);

                    // Append individual file metrics to output
                    AppendFileMetricsToOutput(relativePath, fileClassMetricsMHF, fileClassMetricsAHF, fileClassMetricsMIF, fileClassMetricsAIF, fileClassMetricsCF, fileClassMetricsPF);

                    // Add to overall lists
                    allClassMetricsMHF.AddRange(fileClassMetricsMHF);
                    allClassMetricsAHF.AddRange(fileClassMetricsAHF);
                    allClassMetricsMIF.AddRange(fileClassMetricsMIF);
                    allClassMetricsAIF.AddRange(fileClassMetricsAIF);
                    allClassMetricsCF.AddRange(fileClassMetricsCF);
                    allClassMetricsPF.AddRange(fileClassMetricsPF);

                    textBoxOutput.Text += Environment.NewLine; // Add a separator for readability
                }
                catch (Exception ex)
                {
                    textBoxOutput.Text += $"Error menganalisis {Path.GetFileName(filePath)}: {ex.Message}{Environment.NewLine}{Environment.NewLine}";
                }
            }

            // Calculate and display overall system-level metrics in textBoxSummary
            try
            {
                double mhf = CalculateSystemMHF(allClassMetricsMHF);
                double ahf = CalculateSystemAHF(allClassMetricsAHF);
                double mif = CalculateSystemMIF(allClassMetricsMIF);
                double aif = CalculateSystemAIF(allClassMetricsAIF);
                double cf = CalculateSystemCF(allClassMetricsCF);
                double pf = CalculateSystemPF(allClassMetricsPF);

                textBoxSummary.Text = $"Ringkasan Metrik MOOD Tingkat Sistem Keseluruhan:{Environment.NewLine}{Environment.NewLine}";
                textBoxSummary.Text += $"Method Hiding Factor (MHF): {mhf:P2}\n{Environment.NewLine}";
                textBoxSummary.Text += $"Attribute Hiding Factor (AHF): {ahf:P2}\n{Environment.NewLine}";
                textBoxSummary.Text += $"Method Inheritance Factor (MIF): {mif:P2}\n {Environment.NewLine}";
                textBoxSummary.Text += $"Attribute Inheritance Factor (AIF): {aif:P2}\n{Environment.NewLine}";
                textBoxSummary.Text += $"Coupling Factor (CF): {cf:P2}\n {Environment.NewLine}";
                textBoxSummary.Text += $"Polymorphism Factor (PF): {pf:P2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menghitung metrik sistem keseluruhan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- New Helper Method to Append File Metrics to Output ---
        private void AppendFileMetricsToOutput(string filePath,
                                               List<ClassMetric> mhfMetrics,
                                               List<ClassAttributeMetric> ahfMetrics,
                                               List<ClassInheritanceMetric> mifMetrics,
                                               List<ClassAttributeInheritanceMetric> aifMetrics,
                                               List<ClassCouplingMetric> cfMetrics,
                                               List<ClassPolymorphismMetric> pfMetrics)
        {
            StringBuilder sb = new StringBuilder();

            if (mhfMetrics.Any())
            {
                sb.AppendLine("MHF:");
                foreach (var metric in mhfMetrics)
                {
                    sb.AppendLine($"     Total Methods = {metric.TotalMethods}");
                    sb.AppendLine($"     Hidden = {metric.HiddenMethods}{Environment.NewLine}");
                }
            }

            if (ahfMetrics.Any())
            {
                sb.AppendLine("AHF:");
                foreach (var metric in ahfMetrics)
                {
                    sb.AppendLine($"     Total Attributes = {metric.TotalAttributes}");
                    sb.AppendLine($"     Hidden = {metric.HiddenAttributes}{Environment.NewLine}");
                }
            }

            if (mifMetrics.Any())
            {
                sb.AppendLine("MIF:");
                foreach (var metric in mifMetrics)
                {
                    sb.AppendLine($"     Total Methods = {metric.TotalMethods}");
                    sb.AppendLine($"     Inherited = {metric.InheritedMethods} {Environment.NewLine}");
                }
            }

            if (aifMetrics.Any())
            {
                sb.AppendLine("AIF:");
                foreach (var metric in aifMetrics)
                {
                    sb.AppendLine($"     Total Attributes = {metric.TotalAttributes}");
                    sb.AppendLine($"     Inherited = {metric.InheritedAttributes}{Environment.NewLine}");
                }
            }

            if (cfMetrics.Any())
            {
                sb.AppendLine("CF:");
                foreach (var metric in cfMetrics)
                {
                    sb.AppendLine($"     Coupled Classes = {metric.CoupledClasses} {Environment.NewLine}");
                }
            }

            if (pfMetrics.Any())
            {
                sb.AppendLine("PF:");
                foreach (var metric in pfMetrics)
                {
                    sb.AppendLine($"     New Methods = {metric.NewMethods}");
                    sb.AppendLine($"     Overriding Methods = {metric.OverridingMethods}");
                    sb.AppendLine($"     Descendants = {metric.DescendantsCount}");
                }
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("  Tidak ada metrik yang relevan ditemukan di file ini.");
            }

            textBoxOutput.AppendText(sb.ToString());
        }

        // --- Helper Functions for MOOD Metrics Analysis ---
        // These remain largely the same, as they calculate metrics for a given source code snippet (which will be a single file's content)

        private double CalculateSystemMHF(List<ClassMetric> classMetrics)
        {
            int totalMethods = classMetrics.Sum(cm => cm.TotalMethods);
            int hiddenMethods = classMetrics.Sum(cm => cm.HiddenMethods);
            return totalMethods > 0 ? (double)hiddenMethods / totalMethods : 0;
        }

        private double CalculateSystemAHF(List<ClassAttributeMetric> classAttributeMetrics)
        {
            int totalAttributes = classAttributeMetrics.Sum(cam => cam.TotalAttributes);
            int hiddenAttributes = classAttributeMetrics.Sum(cam => cam.HiddenAttributes);
            return totalAttributes > 0 ? (double)hiddenAttributes / totalAttributes : 0;
        }

        private double CalculateSystemMIF(List<ClassInheritanceMetric> classInheritanceMetrics)
        {
            int totalMethods = classInheritanceMetrics.Sum(cim => cim.TotalMethods);
            int inheritedMethods = classInheritanceMetrics.Sum(cim => cim.InheritedMethods);
            return totalMethods > 0 ? (double)inheritedMethods / totalMethods : 0;
        }

        private double CalculateSystemAIF(List<ClassAttributeInheritanceMetric> classAttributeInheritanceMetrics)
        {
            int totalAttributes = classAttributeInheritanceMetrics.Sum(caim => caim.TotalAttributes);
            int inheritedAttributes = classAttributeInheritanceMetrics.Sum(caim => caim.InheritedAttributes);
            return totalAttributes > 0 ? (double)inheritedAttributes / totalAttributes : 0;
        }

        private double CalculateSystemCF(List<ClassCouplingMetric> classCouplingMetrics)
        {
            // Count unique class names across all files for totalClasses in the system
            int totalClasses = classCouplingMetrics.Select(c => c.ClassName).Distinct().Count();
            int totalCoupledClasses = classCouplingMetrics.Sum(ccm => ccm.CoupledClasses);

            return totalClasses > 1 ? (double)totalCoupledClasses / (totalClasses * (totalClasses - 1)) : 0;
        }

        private double CalculateSystemPF(List<ClassPolymorphismMetric> classPolymorphismMetrics)
        {
            int maxPolymorphicSituations = classPolymorphismMetrics.Sum(cpm => cpm.NewMethods * cpm.DescendantsCount);
            int overridingMethods = classPolymorphismMetrics.Sum(cpm => cpm.OverridingMethods);
            return maxPolymorphicSituations > 0 ? (double)overridingMethods / maxPolymorphicSituations : 0;
        }

        private List<ClassMetric> CalculateClassMetrics(string sourceCode)
        {
            var classMetrics = new List<ClassMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalMethods = 0;
                int hiddenMethods = 0;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if ((trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ")) &&
                        trimmedLine.Contains("(") && trimmedLine.Contains(")") && !trimmedLine.EndsWith(";"))
                    {
                        totalMethods++;
                        if (trimmedLine.Contains("private "))
                        {
                            hiddenMethods++;
                        }
                    }
                }
                classMetrics.Add(new ClassMetric
                {
                    ClassName = className,
                    TotalMethods = totalMethods,
                    HiddenMethods = hiddenMethods
                });
            }
            return classMetrics;
        }

        private List<ClassAttributeMetric> CalculateClassMetricsForAHF(string sourceCode)
        {
            var classAttributeMetrics = new List<ClassAttributeMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalAttributes = 0;
                int hiddenAttributes = 0;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if ((trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ")) &&
                        trimmedLine.Contains(";") && !trimmedLine.Contains("(") && !trimmedLine.Contains("class "))
                    {
                        totalAttributes++;
                        if (trimmedLine.Contains("private "))
                        {
                            hiddenAttributes++;
                        }
                    }
                }
                classAttributeMetrics.Add(new ClassAttributeMetric
                {
                    ClassName = className,
                    TotalAttributes = totalAttributes,
                    HiddenAttributes = hiddenAttributes
                });
            }
            return classAttributeMetrics;
        }

        private List<ClassInheritanceMetric> CalculateClassMetricsForMIF(string sourceCode)
        {
            var classInheritanceMetrics = new List<ClassInheritanceMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalMethods = 0;
                int inheritedMethods = 0;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.Contains("(") && trimmedLine.Contains(")") && !trimmedLine.EndsWith(";"))
                    {
                        totalMethods++;
                        if (trimmedLine.Contains("override "))
                        {
                            inheritedMethods++;
                        }
                    }
                }
                classInheritanceMetrics.Add(new ClassInheritanceMetric
                {
                    ClassName = className,
                    TotalMethods = totalMethods,
                    InheritedMethods = inheritedMethods
                });
            }
            return classInheritanceMetrics;
        }

        private List<ClassAttributeInheritanceMetric> CalculateClassMetricsForAIF(string sourceCode)
        {
            var classAttributeInheritanceMetrics = new List<ClassAttributeInheritanceMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalAttributes = 0;
                int inheritedAttributes = 0;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if ((trimmedLine.Contains("public ") || trimmedLine.Contains("protected ")) &&
                        trimmedLine.Contains(";") && !trimmedLine.Contains("(") && !trimmedLine.Contains("class "))
                    {
                        totalAttributes++;
                        if (trimmedLine.Contains("protected "))
                        {
                            inheritedAttributes++;
                        }
                    }
                }
                classAttributeInheritanceMetrics.Add(new ClassAttributeInheritanceMetric
                {
                    ClassName = className,
                    TotalAttributes = totalAttributes,
                    InheritedAttributes = inheritedAttributes
                });
            }
            return classAttributeInheritanceMetrics;
        }

        private List<ClassCouplingMetric> CalculateCouplingFactor(string sourceCode, HashSet<string> allClassNamesInSystem)
        {
            var classCouplingMetrics = new List<ClassCouplingMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                int coupledClasses = 0;
                var usedClasses = new HashSet<string>();

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if ((trimmedLine.Contains("new ") || trimmedLine.Contains(".")) && !trimmedLine.StartsWith("using "))
                    {
                        var words = trimmedLine.Split(new char[] { ' ', '.', '(', ')', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var word in words)
                        {
                            if (allClassNamesInSystem.Contains(word) && word != className && !usedClasses.Contains(word))
                            {
                                usedClasses.Add(word);
                                coupledClasses++;
                            }
                        }
                    }
                }
                classCouplingMetrics.Add(new ClassCouplingMetric
                {
                    ClassName = className,
                    CoupledClasses = coupledClasses
                });
            }
            return classCouplingMetrics;
        }

        private List<ClassPolymorphismMetric> CalculatePolymorphismFactor(string sourceCode, HashSet<string> allClassNamesInSystem)
        {
            var classPolymorphismMetrics = new List<ClassPolymorphismMetric>();
            var classes = sourceCode.Split(new[] { "class " }, StringSplitOptions.None);

            var classHierarchy = new Dictionary<string, List<string>>();
            foreach (var clsDef in classes.Skip(1))
            {
                string classDefinitionLine = clsDef.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(classDefinitionLine)) continue;

                string currentClassName = classDefinitionLine.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(currentClassName)) continue;

                if (classDefinitionLine.Contains(":"))
                {
                    string baseClassPart = classDefinitionLine.Split(':').Last().Trim();
                    string baseClassName = baseClassPart.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(baseClassName) && allClassNamesInSystem.Contains(baseClassName))
                    {
                        if (!classHierarchy.ContainsKey(baseClassName))
                        {
                            classHierarchy[baseClassName] = new List<string>();
                        }
                        classHierarchy[baseClassName].Add(currentClassName);
                    }
                }
            }

            foreach (var cls in classes.Skip(1))
            {
                string className = cls.Split(new[] { ' ', '{' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(className)) continue;

                int newMethods = 0;
                int overridingMethods = 0;
                int descendantsCount = 0;

                var lines = cls.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.Contains("(") && trimmedLine.Contains(")") && !trimmedLine.EndsWith(";"))
                    {
                        if (trimmedLine.Contains("override "))
                        {
                            overridingMethods++;
                        }
                        else if (!trimmedLine.Contains("virtual ")) // Count methods that are not virtual or overridden as 'new'
                        {
                            newMethods++;
                        }
                    }
                }

                descendantsCount = CountDescendants(className, classHierarchy);

                classPolymorphismMetrics.Add(new ClassPolymorphismMetric
                {
                    ClassName = className,
                    NewMethods = newMethods,
                    OverridingMethods = overridingMethods,
                    DescendantsCount = descendantsCount
                });
            }

            return classPolymorphismMetrics;
        }

        private int CountDescendants(string className, Dictionary<string, List<string>> classHierarchy)
        {
            int count = 0;
            if (classHierarchy.ContainsKey(className))
            {
                foreach (var descendant in classHierarchy[className])
                {
                    count++;
                    count += CountDescendants(descendant, classHierarchy);
                }
            }
            return count;
        }


        // --- Data Structures for Class-Level Metrics ---
        private class ClassMetric
        {
            public string ClassName { get; set; }
            public int TotalMethods { get; set; }
            public int HiddenMethods { get; set; }
        }

        private class ClassAttributeMetric
        {
            public string ClassName { get; set; }
            public int TotalAttributes { get; set; }
            public int HiddenAttributes { get; set; }
        }

        private class ClassInheritanceMetric
        {
            public string ClassName { get; set; }
            public int TotalMethods { get; set; }
            public int InheritedMethods { get; set; }
        }

        private class ClassAttributeInheritanceMetric
        {
            public string ClassName { get; set; }
            public int TotalAttributes { get; set; }
            public int InheritedAttributes { get; set; }
        }

        private class ClassCouplingMetric
        {
            public string ClassName { get; set; }
            public int CoupledClasses { get; set; }
        }

        private class ClassPolymorphismMetric
        {
            public string ClassName { get; set; }
            public int NewMethods { get; set; }
            public int OverridingMethods { get; set; }
            public int DescendantsCount { get; set; }
        }

        // --- UI Event Handlers (unchanged from your second snippet) ---
        private void labelAppName_Click(object sender, EventArgs e) { }

        private void buttonhelp_Click(object sender, EventArgs e)
        {
            string message = "Cara Menggunakan MOOD Metrics Analyzer:\n" +
                             "1. Klik 'Browse' untuk memilih **folder** yang berisi file kode sumber C# (termasuk subfolder).\n" +
                             "2. Klik 'Analyze' untuk mengevaluasi metrik MOOD.\n" +
                             "3. **Ringkasan keseluruhan** metrik MOOD sistem akan muncul di panel 'Overall Summary'.\n" +
                             "4. **Rincian per-file** dengan keterangan folder/path relatif akan muncul di panel 'System-Level Metrics Analysis'.\n" +
                             "\nPenjelasan Metrik MOOD:\n" +
                             "- MHF (Method Hiding Factor): Persentase metode tersembunyi (private/protected).\n" +
                             "- AHF (Attribute Hiding Factor): Persentase atribut tersembunyi (private/protected).\n" +
                             "- MIF (Method Inheritance Factor): Proporsi metode yang diwarisi dari kelas dasar.\n" +
                             "- AIF (Attribute Inheritance Factor): Proporsi atribut yang diwarisi dari kelas dasar.\n" +
                             "- CF (Coupling Factor): Tingkat hubungan antar-kelas di seluruh sistem yang dianalisis.\n" +
                             "- PF (Polymorphism Factor): Tingkat penggunaan polimorfisme dalam sistem.\n" +
                             "\nGunakan hasil ini untuk menilai kualitas desain berorientasi objek Anda!";

            MessageBox.Show(message, "Bantuan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttoninfo_Click(object sender, EventArgs e) // This is likely for the "Information" button
        {
            string information = "MOOD Metrics Analyzer\n" +
                                 "Versi: 1.0.0\n" +
                                 "Dikembangkan oleh: Indra Arya\n\n" +
                                 "Aplikasi ini mengevaluasi kualitas desain berorientasi objek menggunakan metrik MOOD. " +
                                 "Lihat bagian Bantuan untuk instruksi penggunaan.\n\n" +
                                 "Kontak: 21106050002@student.uin-suka.ac.id";
            MessageBox.Show(information, "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonexport_Click(object sender, EventArgs e)
        {
            // Combine output from both textboxes for export
            string overallSummary = textBoxSummary.Text;
            string detailedOutput = textBoxOutput.Text;

            string combinedResult = $"Overall MOOD Metrics Summary:{Environment.NewLine}" +
                                    $"=========================================={Environment.NewLine}" +
                                    overallSummary +
                                    $"{Environment.NewLine}{Environment.NewLine}" +
                                    $"Detailed Per-File Analysis:{Environment.NewLine}" +
                                    $"=========================================={Environment.NewLine}" +
                                    detailedOutput;

            if (string.IsNullOrWhiteSpace(combinedResult) || combinedResult.Contains("No C# files selected."))
            {
                MessageBox.Show("Tidak ada data analisis untuk diekspor. Harap lakukan analisis terlebih dahulu.", "Kesalahan Ekspor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv",
                Title = "Simpan Analisis Metrik MOOD",
                FileName = "MOOD_Metrics_Analysis"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, combinedResult);
                    MessageBox.Show("Analisis berhasil diekspor!", "Ekspor Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Terjadi kesalahan saat mengekspor: {ex.Message}", "Kesalahan Ekspor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonclear_Click(object sender, EventArgs e)
        {
            textBoxSource.Clear();
            textBoxOutput.Clear();
            textBoxSummary.Clear(); // Clear the new summary textbox
            textBoxPath.Text = "";
            csharpFiles.Clear();

            MessageBox.Show("Semua bidang telah dikosongkan.", "Berhasil Dikosongkan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e) { }
        private void textBoxSource_TextChanged(object sender, EventArgs e) { }
        private void textBoxOutput_TextChanged(object sender, EventArgs e) { }
        private void textBoxSummary_TextChanged(object sender, EventArgs e) { } // New event handler for textBoxSummary
        private void label1_Click(object sender, EventArgs e) { }
    }
}