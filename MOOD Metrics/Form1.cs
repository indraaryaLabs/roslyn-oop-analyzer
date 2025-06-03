// mood-metrics/MOOD Metrics/Form1.cs

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; // Tambahkan ini untuk menggunakan Regex

namespace MOOD_Metrics
{
    public partial class Form1 : Form
    {
        private List<string> csharpFiles = new List<string>(); // Untuk menyimpan path file C# yang dipilih

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
                csharpFiles.Clear(); // Hapus daftar file sebelumnya

                try
                {
                    // Ambil semua file .cs di folder yang dipilih dan subfoldernya
                    string[] files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.cs", SearchOption.AllDirectories);

                    if (files.Length == 0)
                    {
                        MessageBox.Show("Tidak ada file C# ditemukan di folder yang dipilih atau subfoldernya.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxSource.Text = ""; // Kosongkan kotak teks kode sumber
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

            // Inisialisasi daftar untuk mengumpulkan metrik dari semua kelas di semua file
            List<ClassMetric> allClassMetricsMHF = new List<ClassMetric>();
            List<ClassAttributeMetric> allClassMetricsAHF = new List<ClassAttributeMetric>();
            List<ClassInheritanceMetric> allClassMetricsMIF = new List<ClassInheritanceMetric>();
            List<ClassAttributeInheritanceMetric> allClassMetricsAIF = new List<ClassAttributeInheritanceMetric>();
            List<ClassCouplingMetric> allClassMetricsCF = new List<ClassCouplingMetric>();
            List<ClassPolymorphismMetric> allClassMetricsPF = new List<ClassPolymorphismMetric>();

            textBoxOutput.Clear();    // Kosongkan output detail
            textBoxSummary.Clear(); // Kosongkan ringkasan keseluruhan
            textBoxOutput.Text += $"Menganalisis file di: {textBoxPath.Text}{Environment.NewLine}";
            textBoxOutput.Text += $"-----------------------------------------{Environment.NewLine}{Environment.NewLine}";

            // Pass pertama: Kumpulkan semua nama kelas di seluruh sistem untuk analisis global (CF & PF)
            HashSet<string> allClassNamesInSystem = new HashSet<string>();
            foreach (string filePath in csharpFiles)
            {
                try
                {
                    string sourceCode = File.ReadAllText(filePath);
                    // Gunakan Regex untuk deteksi nama kelas yang lebih robust
                    var classMatches = Regex.Matches(sourceCode, @"\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\b");
                    foreach (Match match in classMatches)
                    {
                        string className = match.Groups[1].Value;
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

            // Pass kedua: Lakukan analisis detail untuk setiap file dan akumulasikan metrik global
            foreach (string filePath in csharpFiles)
            {
                try
                {
                    string sourceCode = File.ReadAllText(filePath);
                    string relativePath = Path.GetRelativePath(textBoxPath.Text, filePath);

                    // Panggil fungsi perhitungan metrik untuk file saat ini
                    var fileClassMetricsMHF = CalculateClassMetrics(sourceCode);
                    var fileClassMetricsAHF = CalculateClassMetricsForAHF(sourceCode);
                    var fileClassMetricsMIF = CalculateClassMetricsForMIF(sourceCode);
                    var fileClassMetricsAIF = CalculateClassMetricsForAIF(sourceCode);
                    var fileClassMetricsCF = CalculateCouplingFactor(sourceCode, allClassNamesInSystem);
                    var fileClassMetricsPF = CalculatePolymorphismFactor(sourceCode, allClassNamesInSystem);

                    // Tambahkan metrik file individual ke output dengan struktur yang diperbaiki
                    AppendFileMetricsToOutput(relativePath, fileClassMetricsMHF, fileClassMetricsAHF, fileClassMetricsMIF, fileClassMetricsAIF, fileClassMetricsCF, fileClassMetricsPF);

                    // Tambahkan ke daftar keseluruhan untuk perhitungan metrik tingkat sistem
                    allClassMetricsMHF.AddRange(fileClassMetricsMHF);
                    allClassMetricsAHF.AddRange(fileClassMetricsAHF);
                    allClassMetricsMIF.AddRange(fileClassMetricsMIF);
                    allClassMetricsAIF.AddRange(fileClassMetricsAIF);
                    allClassMetricsCF.AddRange(fileClassMetricsCF);
                    allClassMetricsPF.AddRange(fileClassMetricsPF);

                    textBoxOutput.Text += Environment.NewLine; // Tambahkan pemisah untuk keterbacaan
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error menganalisis {Path.GetFileName(filePath)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxOutput.Text += $"Error menganalisis {Path.GetFileName(filePath)}: {ex.Message}{Environment.NewLine}{Environment.NewLine}";
                }
            }

            // Hitung dan tampilkan metrik tingkat sistem keseluruhan di textBoxSummary
            try
            {
                double mhf = CalculateSystemMHF(allClassMetricsMHF);
                double ahf = CalculateSystemAHF(allClassMetricsAHF);
                double mif = CalculateSystemMIF(allClassMetricsMIF);
                double aif = CalculateSystemAIF(allClassMetricsAIF);
                double cf = CalculateSystemCF(allClassMetricsCF);
                double pf = CalculateSystemPF(allClassMetricsPF);

                textBoxSummary.Text = $"Ringkasan Metrik MOOD Tingkat Sistem Keseluruhan:{Environment.NewLine}{Environment.NewLine}";
                textBoxSummary.Text += $"Method Hiding Factor (MHF): {mhf:P2}{Environment.NewLine}";
                textBoxSummary.Text += $"Attribute Hiding Factor (AHF): {ahf:P2}{Environment.NewLine}";
                textBoxSummary.Text += $"Method Inheritance Factor (MIF): {mif:P2}{Environment.NewLine}";
                textBoxSummary.Text += $"Attribute Inheritance Factor (AIF): {aif:P2}{Environment.NewLine}";
                textBoxSummary.Text += $"Coupling Factor (CF): {cf:P2}{Environment.NewLine}";
                textBoxSummary.Text += $"Polymorphism Factor (PF): {pf:P2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error menghitung metrik sistem keseluruhan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Metode Pembantu untuk Menambahkan Metrik File ke Output ---
        private void AppendFileMetricsToOutput(string filePath,
                                               List<ClassMetric> mhfMetrics,
                                               List<ClassAttributeMetric> ahfMetrics,
                                               List<ClassInheritanceMetric> mifMetrics,
                                               List<ClassAttributeInheritanceMetric> aifMetrics,
                                               List<ClassCouplingMetric> cfMetrics,
                                               List<ClassPolymorphismMetric> pfMetrics)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"--- Detail Analisis untuk: {filePath} ---"); // Header untuk file yang dianalisis
            sb.AppendLine(); // Baris kosong untuk pemisah

            // MHF
            if (mhfMetrics.Any())
            {
                sb.AppendLine("MHF:");
                foreach (var metric in mhfMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    Total Methods = {metric.TotalMethods}");
                    sb.AppendLine($"    Hidden = {metric.HiddenMethods}");
                }
                sb.AppendLine(); // Baris kosong setelah setiap bagian metrik
            }

            // AHF
            if (ahfMetrics.Any())
            {
                sb.AppendLine("AHF:");
                foreach (var metric in ahfMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    Total Attributes = {metric.TotalAttributes}");
                    sb.AppendLine($"    Hidden = {metric.HiddenAttributes}");
                }
                sb.AppendLine();
            }

            // MIF
            if (mifMetrics.Any())
            {
                sb.AppendLine("MIF:");
                foreach (var metric in mifMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    Total Methods = {metric.TotalMethods}");
                    sb.AppendLine($"    Inherited = {metric.InheritedMethods}");
                }
                sb.AppendLine();
            }

            // AIF
            if (aifMetrics.Any())
            {
                sb.AppendLine("AIF:");
                foreach (var metric in aifMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    Total Attributes = {metric.TotalAttributes}");
                    sb.AppendLine($"    Inherited = {metric.InheritedAttributes}");
                }
                sb.AppendLine();
            }

            // CF
            if (cfMetrics.Any())
            {
                sb.AppendLine("CF:");
                foreach (var metric in cfMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    Coupled Classes = {metric.CoupledClasses}");
                }
                sb.AppendLine();
            }

            // PF
            if (pfMetrics.Any())
            {
                sb.AppendLine("PF:");
                foreach (var metric in pfMetrics)
                {
                    sb.AppendLine($"  Kelas: {metric.ClassName}"); // Tambahkan Nama Kelas
                    sb.AppendLine($"    New Methods = {metric.NewMethods}");
                    sb.AppendLine($"    Overriding Methods = {metric.OverridingMethods}");
                    sb.AppendLine($"    Descendants = {metric.DescendantsCount}");
                }
                sb.AppendLine();
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("  Tidak ada metrik yang relevan ditemukan di file ini.");
            }

            textBoxOutput.AppendText(sb.ToString());
        }

        // --- Fungsi Pembantu untuk Analisis Metrik MOOD ---
        // Fungsi-fungsi ini menghitung metrik tingkat sistem
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
            // Hitung nama kelas unik di semua file untuk totalClasses dalam sistem
            int totalClasses = classCouplingMetrics.Select(c => c.ClassName).Distinct().Count();
            int totalCoupledClasses = classCouplingMetrics.Sum(ccm => ccm.CoupledClasses);

            // Rumus CF dari jurnal: sum(is_client) / TC^2
            // Namun, karena kode Anda menggunakan (totalClasses * (totalClasses - 1)), saya akan mengikuti itu.
            return totalClasses > 1 ? (double)totalCoupledClasses / (totalClasses * (totalClasses - 1)) : 0;
        }

        private double CalculateSystemPF(List<ClassPolymorphismMetric> classPolymorphismMetrics)
        {
            int maxPolymorphicSituations = classPolymorphismMetrics.Sum(cpm => cpm.NewMethods * cpm.DescendantsCount);
            int overridingMethods = classPolymorphismMetrics.Sum(cpm => cpm.OverridingMethods);
            return maxPolymorphicSituations > 0 ? (double)overridingMethods / maxPolymorphicSituations : 0;
        }

        // --- Fungsi-fungsi untuk menghitung metrik per kelas ---

        private List<ClassMetric> CalculateClassMetrics(string sourceCode)
        {
            var classMetrics = new List<ClassMetric>();
            // Regex untuk memisahkan kode sumber berdasarkan definisi kelas
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            // Iterasi melalui bagian-bagian yang dihasilkan oleh split
            // Setiap kelas akan memiliki 3 bagian: [kode sebelum kelas], [nama kelas], [body kelas]
            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i]; // Nama kelas ada di grup penangkap pertama Regex
                string classBody = classSplits[i + 1]; // Body kelas setelah nama kelas

                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalMethods = 0;
                int hiddenMethods = 0; // Metode private untuk MHF

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi metode:
                    // Harus mengandung '(' dan ')', tidak diakhiri ';', dan memiliki modifier akses
                    // Memastikan bukan deklarasi kelas itu sendiri
                    bool isMethodCandidate = (
                        (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")) &&
                        trimmedLine.Contains("(") && trimmedLine.Contains(")") &&
                        !trimmedLine.EndsWith(";") &&
                        !trimmedLine.Contains("class ") // Hindari menghitung deklarasi kelas sebagai metode
                    );

                    // Filter properti otomatis yang terlihat seperti metode (misal: public int MyProp { get; set; })
                    if (isMethodCandidate && !Regex.IsMatch(trimmedLine, @"\{\s*(?:get|set|init)\s*;", RegexOptions.IgnoreCase))
                    {
                        totalMethods++;
                        // MHF: Hitung metode 'private' sebagai hidden
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
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i];
                string classBody = classSplits[i + 1];

                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalAttributes = 0;
                int hiddenAttributes = 0; // Atribut private untuk AHF

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi atribut:
                    // Harus mengandung ';', tidak mengandung '(', dan memiliki modifier akses
                    // Memastikan bukan deklarasi kelas
                    bool isAttributeCandidate = (
                        (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")) &&
                        trimmedLine.Contains(";") &&
                        !trimmedLine.Contains("(") && // Bukan metode
                        !trimmedLine.Contains("class ") // Bukan deklarasi kelas
                    );

                    // Filter properti otomatis (misal: public int MyProp { get; set; })
                    // Regex ini mencoba menangkap pola properti otomatis
                    if (isAttributeCandidate && !Regex.IsMatch(trimmedLine, @"\{\s*(?:get|set|init)\s*;", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(trimmedLine, @"\b(?:get|set|init)\s*\b", RegexOptions.IgnoreCase)) // Menangkap properti di baris terpisah
                    {
                        totalAttributes++;
                        // AHF: Hitung atribut 'private' sebagai hidden
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
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i];
                string classBody = classSplits[i + 1];

                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalMethods = 0;
                int inheritedMethods = 0; // Metode override untuk MIF (sesuai Form1.cs asli)

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi metode (sama seperti MHF untuk total metode)
                    bool isMethodCandidate = (
                        (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")) &&
                        trimmedLine.Contains("(") && trimmedLine.Contains(")") &&
                        !trimmedLine.EndsWith(";") &&
                        !trimmedLine.Contains("class ")
                    );

                    if (isMethodCandidate && !Regex.IsMatch(trimmedLine, @"\{\s*(?:get|set|init)\s*;", RegexOptions.IgnoreCase))
                    {
                        totalMethods++;
                        // MIF: Hitung metode 'override' sebagai inherited
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
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i];
                string classBody = classSplits[i + 1];

                if (string.IsNullOrWhiteSpace(className)) continue;

                int totalAttributes = 0;
                int inheritedAttributes = 0; // Atribut protected untuk AIF (sesuai Form1.cs asli)

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi atribut (sama seperti AHF untuk total atribut)
                    bool isAttributeCandidate = (
                        (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")) &&
                        trimmedLine.Contains(";") &&
                        !trimmedLine.Contains("(") &&
                        !trimmedLine.Contains("class ")
                    );

                    if (isAttributeCandidate && !Regex.IsMatch(trimmedLine, @"\{\s*(?:get|set|init)\s*;", RegexOptions.IgnoreCase) &&
                        !Regex.IsMatch(trimmedLine, @"\b(?:get|set|init)\s*\b", RegexOptions.IgnoreCase))
                    {
                         totalAttributes++;
                         // AIF: Hitung atribut 'protected' sebagai inherited
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
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i];
                string classBody = classSplits[i + 1];

                if (string.IsNullOrWhiteSpace(className)) continue;

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                int coupledClasses = 0;
                var usedClasses = new HashSet<string>();

                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi coupling:
                    // Mencari penggunaan 'new' atau '.' (dot operator) yang diikuti nama kelas lain
                    // Mengabaikan 'using' statement dan komentar
                    if ((trimmedLine.Contains("new ") || trimmedLine.Contains(".")) && !trimmedLine.StartsWith("using ") && !trimmedLine.StartsWith("//"))
                    {
                        // Menggunakan Regex untuk mengekstrak kata-kata yang mungkin merupakan nama kelas
                        var words = Regex.Matches(trimmedLine, @"\b[a-zA-Z_][a-zA-Z0-9_]*\b");
                        foreach (Match wordMatch in words)
                        {
                            string word = wordMatch.Value;
                            // Pastikan kata adalah nama kelas yang valid di sistem dan bukan kelas saat ini
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
            var classSplits = Regex.Split(sourceCode, @"(?:\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*[a-zA-Z_][a-zA-Z0-9_,\s]*)?\s*{)");

            var classHierarchy = new Dictionary<string, List<string>>();
            // Bangun hierarki kelas dari sourceCode yang diberikan (file saat ini)
            var classDefinitionMatches = Regex.Matches(sourceCode, @"\b(?:public|private|protected|internal|sealed|abstract)?\s*class\s+([a-zA-Z_][a-zA-Z0-9_]*)\b(?::\s*([a-zA-Z_][a-zA-Z0-9_,\s]*))?");
            foreach (Match match in classDefinitionMatches)
            {
                string currentClassName = match.Groups[1].Value;
                string baseClassPart = match.Groups[2].Value; // Bagian base class
                if (!string.IsNullOrWhiteSpace(baseClassPart))
                {
                    // Ambil nama kelas dasar pertama jika ada beberapa (misal: Class : Base, Interface)
                    string baseClassName = baseClassPart.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    // Pastikan base class ada di daftar kelas yang terdeteksi di seluruh sistem
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


            for (int i = 1; i < classSplits.Length; i += 2)
            {
                string className = classSplits[i];
                string classBody = classSplits[i + 1];

                if (string.IsNullOrWhiteSpace(className)) continue;

                int newMethods = 0;
                int overridingMethods = 0;
                int descendantsCount = 0;

                var lines = classBody.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    // Logika deteksi metode (mirip dengan MHF)
                    bool isMethodCandidate = (
                        (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")) &&
                        trimmedLine.Contains("(") && trimmedLine.Contains(")") &&
                        !trimmedLine.EndsWith(";") &&
                        !trimmedLine.Contains("class ")
                    );

                    if (isMethodCandidate && !Regex.IsMatch(trimmedLine, @"\{\s*(?:get|set|init)\s*;", RegexOptions.IgnoreCase))
                    {
                        // PF: Hitung overridingMethods
                        if (trimmedLine.Contains("override "))
                        {
                            overridingMethods++;
                        }
                        // PF: Hitung newMethods (metode yang bukan override dan bukan virtual)
                        else if (!trimmedLine.Contains("virtual "))
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

        // Fungsi rekursif untuk menghitung jumlah turunan dari sebuah kelas
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


        // --- Struktur Data untuk Metrik Tingkat Kelas ---
        // Kelas-kelas ini digunakan untuk menyimpan metrik per kelas
        private class ClassMetric
        {
            public string ClassName { get; set; }
            public int TotalMethods { get; set; }
            public int HiddenMethods { get; set; } // Metode private
        }

        private class ClassAttributeMetric
        {
            public string ClassName { get; set; }
            public int TotalAttributes { get; set; }
            public int HiddenAttributes { get; set; } // Atribut private
        }

        private class ClassInheritanceMetric
        {
            public string ClassName { get; set; }
            public int TotalMethods { get; set; }
            public int InheritedMethods { get; set; } // Metode override
        }

        private class ClassAttributeInheritanceMetric
        {
            public string ClassName { get; set; }
            public int TotalAttributes { get; set; }
            public int InheritedAttributes { get; set; } // Atribut protected
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

        // --- Event Handler UI (tidak berubah) ---
        private void labelAppName_Click(object sender, EventArgs e) { }

        private void buttonhelp_Click(object sender, EventArgs e)
        {
            string message = "Cara Menggunakan MOOD Metrics Analyzer:\n" +
                             "1. Klik 'Browse' untuk memilih **folder** yang berisi file kode sumber C# (termasuk subfolder).\n" +
                             "2. Klik 'Analyze' untuk mengevaluasi metrik MOOD.\n" +
                             "3. **Ringkasan keseluruhan** metrik MOOD sistem akan muncul di panel 'Overall Summary'.\n" +
                             "4. **Rincian per-file** dengan keterangan folder/path relatif akan muncul di panel 'System-Level Metrics Analysis'.\n" +
                             "\nPenjelasan Metrik MOOD:\n" +
                             "- MHF (Method Hiding Factor): Persentase metode tersembunyi (private).\n" +
                             "- AHF (Attribute Hiding Factor): Persentase atribut tersembunyi (private).\n" +
                             "- MIF (Method Inheritance Factor): Proporsi metode yang diwarisi (metode override).\n" +
                             "- AIF (Attribute Inheritance Factor): Proporsi atribut yang diwarisi (atribut protected).\n" +
                             "- CF (Coupling Factor): Tingkat hubungan antar-kelas di seluruh sistem yang dianalisis.\n" +
                             "- PF (Polymorphism Factor): Tingkat penggunaan polimorfisme dalam sistem.\n" +
                             "\nGunakan hasil ini untuk menilai kualitas desain berorientasi objek Anda!";

            MessageBox.Show(message, "Bantuan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttoninfo_Click(object sender, EventArgs e)
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
            // Gabungkan output dari kedua kotak teks untuk ekspor
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
            textBoxSummary.Clear(); // Kosongkan kotak teks ringkasan baru
            textBoxPath.Text = "";
            csharpFiles.Clear();

            MessageBox.Show("Semua bidang telah dikosongkan.", "Berhasil Dikosongkan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e) { }
        private void textBoxSource_TextChanged(object sender, EventArgs e) { }
        private void textBoxOutput_TextChanged(object sender, EventArgs e) { }
        private void textBoxSummary_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}