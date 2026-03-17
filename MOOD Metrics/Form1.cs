// mood-metrics/MOOD Metrics/Form1.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using MOOD_Metrics;
using System.Threading.Tasks; // Added for async/await

namespace MOOD_Metrics
{
    public partial class Form1 : Form
    {
        private List<string> csharpFiles = new();
        private RoslynMoodAnalyzer analyzer = new();

        private FormWindowState previousWindowState;
        private FormBorderStyle previousFormBorderStyle;
        private bool isFullscreen = false;

        // Deklarasikan paddingBetweenTextBoxes sebagai field kelas
        private const int paddingBetweenTextBoxes = 15; // Jarak antar textbox

        public Form1()
        {
            InitializeComponent();
            previousWindowState = this.WindowState;
            previousFormBorderStyle = this.FormBorderStyle;
            this.Resize += Form1_Resize;
            // Panggil resize pertama kali untuk mengatur tata letak awal
            Form1_Resize(this, EventArgs.Empty);
        }

        private async void buttonAnalyze_Click(object sender, EventArgs e) // Changed to async
        {
            if (!csharpFiles.Any())
            {
                MessageBox.Show("Silakan pilih folder terlebih dahulu.");
                return;
            }

            // Tampilkan progress bar dan nonaktifkan tombol
            progressBarAnalysis.Visible = true; //
            buttonAnalyze.Enabled = false; //
            buttonBrowse.Enabled = false; //
            buttonclear.Enabled = false; //
            buttonexport.Enabled = false; //
            buttonhelp.Enabled = false; //
            buttoninfo.Enabled = false; //
            buttonToggleFullscreen.Enabled = false; //

            textBoxOutput.Clear(); //
            textBoxSummary.Clear(); //

            try
            {
                analyzer = new RoslynMoodAnalyzer();
                // Jalankan analisis di thread terpisah
                await Task.Run(() => analyzer.Analyze(csharpFiles.ToArray())); //

                AppendFileMetricsToOutput(
                    "(Semua File)",
                    (List<ClassMetric>)analyzer.MhfMetrics,
                    (List<ClassAttributeMetric>)analyzer.AhfMetrics,
                    (List<ClassInheritanceMetric>)analyzer.MifMetrics,
                    (List<ClassAttributeInheritanceMetric>)analyzer.AifMetrics,
                    (List<ClassCouplingMetric>)analyzer.CfMetrics,
                    (List<ClassPolymorphismMetric>)analyzer.PfMetrics
                );
            }
            catch (Exception ex) // Added error handling
            {
                MessageBox.Show($"Terjadi kesalahan saat menganalisis: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //
            }
            finally
            {
                // Sembunyikan progress bar dan aktifkan kembali tombol
                progressBarAnalysis.Visible = false; //
                buttonAnalyze.Enabled = true; //
                buttonBrowse.Enabled = true; //
                buttonclear.Enabled = true; //
                buttonexport.Enabled = true; //
                buttonhelp.Enabled = true; //
                buttoninfo.Enabled = true; //
                buttonToggleFullscreen.Enabled = true; //
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderDialog.SelectedPath;
                csharpFiles = Directory.GetFiles(folderDialog.SelectedPath, "*.cs", SearchOption.AllDirectories).ToList();

                StringBuilder sbSource = new StringBuilder();
                sbSource.AppendLine($"Ditemukan {csharpFiles.Count} file C# di '{folderDialog.SelectedPath}' (termasuk subfolder):");
                sbSource.AppendLine();

                foreach (var filePath in csharpFiles)
                {
                    sbSource.AppendLine(Path.GetFileName(filePath));
                }

                sbSource.AppendLine();
                sbSource.AppendLine("Klik 'Analyze' untuk memproses semua file.");
                textBoxSource.Text = sbSource.ToString();
            }
        }

        private void buttonclear_Click(object sender, EventArgs e)
        {
            textBoxSource.Clear();
            textBoxOutput.Clear();
            textBoxSummary.Clear();
            textBoxPath.Text = "";
            csharpFiles.Clear();

            MessageBox.Show("Semua bidang telah dikosongkan.", "Berhasil Dikosongkan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonexport_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Save Analysis Result"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, textBoxSummary.Text);
                MessageBox.Show("Export berhasil disimpan.");
            }
        }

        private void buttonhelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Panduan:\n1. Klik Browse untuk pilih folder berisi file C#.\n2. Klik Analyze untuk melihat hasil MOOD Metrics.\n3. Gunakan tombol lain untuk informasi, ekspor, atau reset.");
        }

        private void buttoninfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MOOD Metrics Analyzer\n" +
                "Versi 1.1.0\n" +
                "Dibuat oleh Indra Arya\n\n" +
                "Menggunakan Roslyn untuk analisis C# OOP Metrics.\n" +
                "Lihat bagian help untuk instruksi penggunaan.\n\n" +
                "Kontak: 21106050002@student.uin-suka.ac.id");
        }

        private void labelAppName_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Selamat datang di MOOD Metrics Analyzer!");
        }

        private void buttonToggleFullscreen_Click(object sender, EventArgs e)
        {
            if (!isFullscreen)
            {
                previousWindowState = this.WindowState;
                previousFormBorderStyle = this.FormBorderStyle;

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                buttonToggleFullscreen.Text = "Windowed";
            }
            else
            {
                this.FormBorderStyle = previousFormBorderStyle;
                this.WindowState = previousWindowState;
                buttonToggleFullscreen.Text = "Fullscreen";
            }
            isFullscreen = !isFullscreen;
            // Panggil Form1_Resize secara manual setelah toggle untuk memastikan tata letak diperbarui
            Form1_Resize(this, EventArgs.Empty);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // Posisikan labelAppName di tengah secara horizontal
            labelAppName.Left = (this.ClientSize.Width - labelAppName.Width) / 2;

            // Sesuaikan posisi Y untuk semua FlowLayoutPanel agar sejajar di bagian bawah
            int bottomControlsTop = this.ClientSize.Height - flowLayoutPanelLeft.Height - 10; // 10 margin dari bawah

            flowLayoutPanelLeft.Top = bottomControlsTop;
            flowLayoutPanelRight.Top = bottomControlsTop;

            // Update posisi progress bar
            progressBarAnalysis.Width = this.ClientSize.Width - (progressBarAnalysis.Left * 2); // Sesuaikan lebar
            progressBarAnalysis.Left = (this.ClientSize.Width - progressBarAnalysis.Width) / 2; // Posisikan di tengah


            // Atur lebar textBoxSource agar tetap
            const int fixedSourceWidth = 347; // Lebar default textBoxSource saat initial design
            textBoxSource.Width = fixedSourceWidth; //

            // Hitung total lebar yang tersedia untuk textBoxOutput dan textBoxSummary
            // Ini adalah lebar total form dikurangi:
            // - textBoxSource.Left (margin kiri awal)
            // - Lebar textBoxSource
            // - paddingBetweenTextBoxes (antara Source dan Output)
            // - paddingBetweenTextBoxes (antara Output dan Summary)
            // - margin kanan yang sama dengan paddingBetweenTextBoxes
            int totalHorizontalPaddingForTextBoxes = textBoxSource.Left + paddingBetweenTextBoxes * 2 + paddingBetweenTextBoxes;
            int availableWidthForOutputAndSummary = this.ClientSize.Width - textBoxSource.Left - textBoxSource.Width - (paddingBetweenTextBoxes * 2);

            // Tentukan lebar yang sama untuk textBoxOutput dan textBoxSummary
            int equalWidthForOutputAndSummary = availableWidthForOutputAndSummary / 2;
            if (equalWidthForOutputAndSummary < 100) equalWidthForOutputAndSummary = 100; // Minimal lebar

            // Sesuaikan tinggi untuk semua textbox di area utama jika form diresize secara vertikal
            int newHeight = this.ClientSize.Height - textBoxSource.Top - (flowLayoutPanelLeft.Height + 20); // 20 adalah margin dari bawah
            if (newHeight < 50) newHeight = 50; // Minimum tinggi

            textBoxSource.Height = newHeight;
            textBoxOutput.Height = newHeight;
            textBoxSummary.Height = newHeight;

            // Atur ulang ukuran dan posisi textBoxOutput
            textBoxOutput.Width = equalWidthForOutputAndSummary;
            textBoxOutput.Left = textBoxSource.Right + paddingBetweenTextBoxes;

            // Atur ulang ukuran dan posisi textBoxSummary
            textBoxSummary.Width = equalWidthForOutputAndSummary;
            textBoxSummary.Left = textBoxOutput.Right + paddingBetweenTextBoxes;

            // Pastikan FlowLayoutPanelRight tetap menempel di kanan
            // Perhitungan ini sudah dilakukan oleh Anchor sebelumnya, tapi ini memastikan konsistensi
            flowLayoutPanelRight.Left = this.ClientSize.Width - flowLayoutPanelRight.Width - 12; // 12 adalah margin kanan
        }

        private void AppendFileMetricsToOutput(string label,
                                               List<ClassMetric> mhfMetrics,
                                               List<ClassAttributeMetric> ahfMetrics,
                                               List<ClassInheritanceMetric> mifMetrics,
                                               List<ClassAttributeInheritanceMetric> aifMetrics,
                                               List<ClassCouplingMetric> cfMetrics,
                                               List<ClassPolymorphismMetric> pfMetrics)
        {
            StringBuilder sbOutput = new();
            StringBuilder sbSummary = new();

            sbOutput.AppendLine($"--- Detail Analisis per Kelas: {label} ---\n");

            if (mhfMetrics.Any())
            {
                sbOutput.AppendLine("--- Method Hiding Factor (MHF) ---");
                foreach (var m in mhfMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Total Methods Dideklarasikan: {m.TotalMethods}");
                    sbOutput.AppendLine($"    Metode Tersembunyi (Kontribusi): {m.HiddenMethods:F2}");
                    sbOutput.AppendLine();
                }
            }

            if (ahfMetrics.Any())
            {
                sbOutput.AppendLine("--- Attribute Hiding Factor (AHF) ---");
                foreach (var m in ahfMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Total Atribut Dideklarasikan: {m.TotalAttributes}");
                    sbOutput.AppendLine($"    Atribut Tersembunyi (Kontribusi): {m.HiddenAttributes:F2}");
                    sbOutput.AppendLine();
                }
            }

            if (mifMetrics.Any())
            {
                sbOutput.AppendLine("--- Method Inheritance Factor (MIF) ---");
                foreach (var m in mifMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Total Metode (Dideklarasikan + Diwarisi): {m.TotalMethods}");
                    sbOutput.AppendLine($"    Metode Diwarisi (dan Dapat Diakses): {m.InheritedMethods}");
                    sbOutput.AppendLine();
                }
            }

            if (aifMetrics.Any())
            {
                sbOutput.AppendLine("--- Attribute Inheritance Factor (AIF) ---");
                foreach (var m in aifMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Total Atribut (Dideklarasikan + Diwarisi): {m.TotalAttributes}");
                    sbOutput.AppendLine($"    Atribut Diwarisi (dan Dapat Diakses): {m.InheritedAttributes}");
                    sbOutput.AppendLine();
                }
            }

            if (cfMetrics.Any())
            {
                sbOutput.AppendLine("--- Coupling Factor (CF) ---");
                foreach (var m in cfMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Jumlah Kelas Terkopel: {m.CoupledClasses}");
                    sbOutput.AppendLine();
                }
            }

            if (pfMetrics.Any())
            {
                sbOutput.AppendLine("--- Polymorphism Factor (PF) ---");
                foreach (var m in pfMetrics)
                {
                    sbOutput.AppendLine($"  Kelas: {m.ClassName}");
                    sbOutput.AppendLine($"    Metode Baru: {m.NewMethods}");
                    sbOutput.AppendLine($"    Metode Overriding: {m.OverridingMethods}");
                    sbOutput.AppendLine($"    Jumlah Keturunan: {m.DescendantsCount}");
                    sbOutput.AppendLine();
                }
            }

            if (checkBoxDebug.Checked)
            {
                sbOutput.AppendLine("\n--- DEBUG INFO (Detail Perhitungan Internal) ---");
                foreach (var m in mhfMetrics)
                    sbOutput.AppendLine($"[DEBUG-MHF] {m.ClassName}: total={m.TotalMethods}, hidden={m.HiddenMethods:F2}");
                foreach (var m in ahfMetrics)
                    sbOutput.AppendLine($"[DEBUG-AHF] {m.ClassName}: total={m.TotalAttributes}, hidden={m.HiddenAttributes:F2}");
                foreach (var m in mifMetrics)
                    sbOutput.AppendLine($"[DEBUG-MIF] {m.ClassName}: total={m.TotalMethods}, inherited={m.InheritedMethods}");
                foreach (var m in aifMetrics)
                    sbOutput.AppendLine($"[DEBUG-AIF] {m.ClassName}: total={m.TotalAttributes}, inherited={m.InheritedAttributes}");
                foreach (var m in cfMetrics)
                    sbOutput.AppendLine($"[DEBUG-CF] {m.ClassName}: coupled={m.CoupledClasses}");
                foreach (var m in pfMetrics)
                    sbOutput.AppendLine($"[DEBUG-PF] {m.ClassName}: new={m.NewMethods}, override={m.OverridingMethods}, descendants={m.DescendantsCount}");
                sbOutput.AppendLine();
            }

            textBoxOutput.Text = sbOutput.ToString();

            double totalDeclaredMethods = analyzer.MhfMetrics.Sum(m => m.TotalMethods);
            double totalHiddenMethods = analyzer.MhfMetrics.Sum(m => m.HiddenMethods);
            double mhf = (totalDeclaredMethods > 0) ? totalHiddenMethods / totalDeclaredMethods : 0.0;

            double totalDeclaredAttributes = analyzer.AhfMetrics.Sum(m => m.TotalAttributes);
            double totalHiddenAttributes = analyzer.AhfMetrics.Sum(m => m.HiddenAttributes);
            double ahf = (totalDeclaredAttributes > 0) ? totalHiddenAttributes / totalDeclaredAttributes : 0.0;

            double totalCallableMethods = analyzer.MifMetrics.Sum(m => m.TotalMethods);
            double totalInheritedMethods = analyzer.MifMetrics.Sum(m => m.InheritedMethods);
            double mif = (totalCallableMethods > 0) ? totalInheritedMethods / totalCallableMethods : 0.0;

            double totalCallableAttributes = analyzer.AifMetrics.Sum(m => m.TotalAttributes);
            double totalInheritedAttributes = analyzer.AifMetrics.Sum(m => m.InheritedAttributes);
            double aif = (totalCallableAttributes > 0) ? totalInheritedAttributes / totalCallableAttributes : 0.0;

            int n = analyzer.CfMetrics.Count;
            double totalCouplingPairs = analyzer.CfMetrics.Sum(c => (double)c.CoupledClasses);
            double cfDenominator = (double)n * n;
            double cf = (cfDenominator > 0) ? totalCouplingPairs / cfDenominator : 0.0;

            double totalOverridingMethods = analyzer.PfMetrics.Sum(p => (double)p.OverridingMethods);
            double pfDenominatorSum = analyzer.PfMetrics.Sum(p => (double)(p.NewMethods + p.OverridingMethods) * p.DescendantsCount);
            double pf = (pfDenominatorSum > 0) ? totalOverridingMethods / pfDenominatorSum : 0.0;

            sbSummary.AppendLine("--- Ringkasan MOOD Metrics Tingkat Sistem ---");
            sbSummary.AppendLine("Hasil ini memberikan gambaran umum tentang kualitas desain berorientasi objek pada sistem Anda.");
            sbSummary.AppendLine();

            sbSummary.AppendLine("1. Method Hiding Factor (MHF)");
            sbSummary.AppendLine("   Mengukur sejauh mana metode-metode dalam sistem tersembunyi (tidak dapat diakses secara publik).");
            sbSummary.AppendLine($"   Formula: (Total Kontribusi Metode Tersembunyi) / (Total Metode yang Dideklarasikan) * 100%");
            sbSummary.AppendLine($"   Perhitungan: ({totalHiddenMethods:F2}) / ({totalDeclaredMethods}) * 100% = {mhf:P2}");
            sbSummary.AppendLine($"   Nilai Akhir: {mhf:P2}");
            sbSummary.AppendLine();

            sbSummary.AppendLine("2. Attribute Hiding Factor (AHF)");
            sbSummary.AppendLine("   Mengukur sejauh mana atribut-atribut dalam sistem tersembunyi (tidak dapat diakses secara publik).");
            sbSummary.AppendLine($"   Formula: (Total Kontribusi Atribut Tersembunyi) / (Total Atribut yang Dideklarasikan) * 100%");
            sbSummary.AppendLine($"   Perhitungan: ({totalHiddenAttributes:F2}) / ({totalDeclaredAttributes}) * 100% = {ahf:P2}");
            sbSummary.AppendLine($"   Nilai Akhir: {ahf:P2}");
            sbSummary.AppendLine();

            sbSummary.AppendLine("3. Method Inheritance Factor (MIF)");
            sbSummary.AppendLine("   Mengukur sejauh mana metode diwarisi dalam sistem (proporsi metode yang diwarisi terhadap total metode yang dapat dipanggil).");
            sbSummary.AppendLine($"   Formula: (Total Metode Diwarisi dan Dapat Diakses) / (Total Metode yang Dapat Dipanggil) * 100%");
            sbSummary.AppendLine($"   Perhitungan: ({totalInheritedMethods}) / ({totalCallableMethods}) = {mif:P2}");
            sbSummary.AppendLine($"   Nilai Akhir: {mif:P2}");
            sbSummary.AppendLine();

            sbSummary.AppendLine("4. Attribute Inheritance Factor (AIF)");
            sbSummary.AppendLine("   Mengukur sejauh mana atribut diwarisi dalam sistem (proporsi atribut yang diwarisi terhadap total atribut yang dapat dipanggil).");
            sbSummary.AppendLine($"   Formula: (Total Atribut Diwarisi dan Dapat Diakses) / (Total Atribut yang Dapat Dipanggil) * 100%");
            sbSummary.AppendLine($"   Perhitungan: ({totalInheritedAttributes}) / ({totalCallableAttributes}) = {aif:P2}");
            sbSummary.AppendLine($"   Nilai Akhir: {aif:P2}");
            sbSummary.AppendLine();

            sbSummary.AppendLine("5. Coupling Factor (CF)");
            sbSummary.AppendLine("   Mengukur tingkat kopling antar kelas dalam sistem, tidak termasuk kopling karena pewarisan.");
            sbSummary.AppendLine($"   Formula: (Jumlah Kelas Terkopel) / ((Total Kelas)^2) * 100%");
            sbSummary.AppendLine($"   Perhitungan: ({totalCouplingPairs}) / ({n}^2) * 100% = ({totalCouplingPairs}) / ({cfDenominator:F0}) * 100% = {cf:P2}");
            sbSummary.AppendLine($"   Nilai Akhir: {cf:P2}");
            sbSummary.AppendLine();

            sbSummary.AppendLine("6. Polymorphism Factor (PF)");
            sbSummary.AppendLine("   Mengukur potensi polimorfisme dalam sistem (proporsi metode overriding terhadap situasi polimorfik maksimum).");
            sbSummary.AppendLine($"   Formula: (Jumlah Total Metode Overriding) / (Jumlah Total [(Metode Baru + Metode Mengganti) * Jumlah Keturunan])");
            sbSummary.AppendLine($"   Perhitungan: ({totalOverridingMethods}) / ({pfDenominatorSum:F2}) = {pf:P2}");
            if (pfDenominatorSum == 0)
            {
                sbSummary.AppendLine("   Nilai Akhir: Tidak Terdefinisi (Undefined) karena penyebut adalah 0.");
            }
            else
            {
                sbSummary.AppendLine($"   Nilai Akhir: {pf:P2}");
            }
            sbSummary.AppendLine();

            textBoxSummary.Text = sbSummary.ToString();
        }
    }

    public class RoslynMoodAnalyzer
    {
        public List<ClassMetric> MhfMetrics = new();
        public List<ClassAttributeMetric> AhfMetrics = new();
        public List<ClassInheritanceMetric> MifMetrics = new();
        public List<ClassAttributeInheritanceMetric> AifMetrics = new();
        public List<ClassCouplingMetric> CfMetrics = new();
        public List<ClassPolymorphismMetric> PfMetrics = new();

        private Dictionary<string, INamedTypeSymbol> classSymbolMap = new();
        private Dictionary<string, List<string>> classDescendants = new();

        public void Analyze(string[] filePaths)
        {
            MhfMetrics.Clear();
            AhfMetrics.Clear();
            MifMetrics.Clear();
            AifMetrics.Clear();
            CfMetrics.Clear();
            PfMetrics.Clear();
            classDescendants.Clear();
            classSymbolMap.Clear();

            var workspace = new AdhocWorkspace();
            var project = workspace.AddProject("MoodProject", LanguageNames.CSharp);

            foreach (var path in filePaths)
            {
                string sourceCode = null;
                try
                {
                    sourceCode = File.ReadAllText(path);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading file {path}: {ex.Message}");
                    continue;
                }
                if (string.IsNullOrEmpty(sourceCode)) continue;

                var doc = project.AddDocument(
                    Path.GetFileName(path),
                    text: SourceText.From(sourceCode),
                    filePath: path
                );
                project = doc.Project;
            }

            var compilation = project.GetCompilationAsync().Result;
            if (compilation == null) return;

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                if (syntaxTree == null) continue;

                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                if (semanticModel == null) continue;

                var root = syntaxTree.GetRoot();
                if (root == null) continue;

                var classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classDecl in classDecls)
                {
                    if (classDecl == null) continue;

                    var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                    if (classSymbol == null) continue;

                    if (classSymbol.IsImplicitlyDeclared || classSymbol.Name.Contains("<") || classSymbol.Name.Contains(">") ||
                        classSymbol.OriginalDefinition.Name.EndsWith("Iterator"))
                    {
                        continue;
                    }
                    if (classDecl.SyntaxTree.FilePath != null && Path.GetFileName(classDecl.SyntaxTree.FilePath).EndsWith(".Designer.cs"))
                    {
                        continue;
                    }
                    if (classSymbol.Name == "Program" && classSymbol.ContainingNamespace.IsGlobalNamespace && classSymbol.GetMembers().Count(m => m is IMethodSymbol && m.Name == "Main") == 1)
                    {
                        continue;
                    }
                    if (classSymbol.GetMembers().Any(m => m is IMethodSymbol method && method.IsStatic && method.IsExtensionMethod))
                    {
                        continue;
                    }
                    if (classSymbol.IsStatic && !classSymbol.GetMembers().Any(m => m is IMethodSymbol method && (method.IsOverride || method.IsVirtual)))
                    {
                        if (classSymbol.Name == "LogService" ||
                            classSymbol.Name == "InfoManager" ||
                            classSymbol.Name == "HttpClientProvider" ||
                            classSymbol.Name == "SystemMonitor" ||
                            classSymbol.Name == "AIAnalysisService")
                        {
                            continue;
                        }
                    }

                    if (classSymbol.TypeKind != TypeKind.Class)
                    {
                        continue;
                    }

                    classSymbolMap[classSymbol.ToDisplayString()] = classSymbol;

                    if (classSymbol.BaseType != null && classSymbol.BaseType.SpecialType != SpecialType.System_Object)
                    {
                        var baseName = classSymbol.BaseType.ToDisplayString();
                        if (!classDescendants.ContainsKey(baseName)) classDescendants[baseName] = new();
                        classDescendants[baseName].Add(classSymbol.ToDisplayString());
                    }
                }
            }

            foreach (var classSymbol in classSymbolMap.Values)
            {
                if (classSymbol == null) continue;

                string className = classSymbol.ToDisplayString();

                int totalMethodsInClass = 0;
                double hiddenMethodsSum = 0;
                int totalAttributesInClass = 0;
                double hiddenAttributesSum = 0;

                int inheritedAndNotOverriddenMethods = 0;
                int inheritedAndNotOverriddenAttributes = 0;

                int newMethods = 0;
                int overridingMethods = 0;

                var coupledClassesSet = new HashSet<string>();

                var currentSyntaxTree = classSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.SyntaxTree;
                if (currentSyntaxTree == null) continue;

                var currentSemanticModel = compilation.GetSemanticModel(currentSyntaxTree);
                if (currentSemanticModel == null) continue;

                var classDeclarationSyntax = currentSyntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                                            .FirstOrDefault(cds => currentSemanticModel.GetDeclaredSymbol(cds)?.Equals(classSymbol, SymbolEqualityComparer.Default) == true);

                if (classDeclarationSyntax == null) continue;

                foreach (var member in classSymbol.GetMembers())
                {
                    if (member == null || member.IsImplicitlyDeclared) continue;

                    if (member is IMethodSymbol method &&
                        method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet &&
                        !method.MethodKind.ToString().Contains("Constructor") &&
                        !method.IsAbstract)
                    {
                        totalMethodsInClass++;

                        if (method.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private)
                        {
                            hiddenMethodsSum += 1.0;
                        }
                        else if (method.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Protected)
                        {
                            int classesNotInheriting = 0;
                            int totalRelevantClasses = classSymbolMap.Count;

                            if (totalRelevantClasses > 1)
                            {
                                foreach (var otherClassEntry in classSymbolMap)
                                {
                                    var otherClass = otherClassEntry.Value;
                                    if (otherClass == null || otherClass.Equals(classSymbol, SymbolEqualityComparer.Default)) continue;

                                    if (!IsDescendantOf(otherClass, classSymbol))
                                    {
                                        classesNotInheriting++;
                                    }
                                }
                                hiddenMethodsSum += (double)classesNotInheriting / (totalRelevantClasses - 1);
                            }
                            else
                            {
                                hiddenMethodsSum += 1.0;
                            }
                        }

                        if (!method.IsStatic)
                        {
                            if (method.IsOverride)
                            {
                                overridingMethods++;
                            }
                            else if (method.OverriddenMethod == null && !method.IsImplementationOfAnyInterfaceMethod())
                            {
                                newMethods++;
                            }
                        }
                    }
                    else if (member is IPropertySymbol prop)
                    {
                        if (!prop.IsStatic)
                        {
                            totalAttributesInClass++;

                            if (prop.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private ||
                                (prop.SetMethod != null && prop.SetMethod.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private) ||
                                (prop.GetAttributes().Any(attr => attr.AttributeClass != null && attr.AttributeClass.Name == "JsonIgnoreAttribute")))
                            {
                                hiddenAttributesSum += 1.0;
                            }
                            else if (prop.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Protected)
                            {
                                int classesNotInheriting = 0;
                                int totalRelevantClasses = classSymbolMap.Count;
                                if (totalRelevantClasses > 1)
                                {
                                    foreach (var otherClassEntry in classSymbolMap)
                                    {
                                        var otherClass = otherClassEntry.Value;
                                        if (otherClass == null || otherClass.Equals(classSymbol, SymbolEqualityComparer.Default)) continue;
                                        if (!IsDescendantOf(otherClass, classSymbol))
                                        {
                                            classesNotInheriting++;
                                        }
                                    }
                                    hiddenAttributesSum += (double)classesNotInheriting / (totalRelevantClasses - 1);
                                }
                                else
                                {
                                    hiddenAttributesSum += 1.0;
                                }
                            }
                        }
                    }
                    else if (member is IFieldSymbol field && !field.IsStatic)
                    {
                        totalAttributesInClass++;

                        if (field.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private)
                        {
                            hiddenAttributesSum += 1.0;
                        }
                        else if (field.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Protected)
                        {
                            int classesNotInheriting = 0;
                            int totalRelevantClasses = classSymbolMap.Count;
                            if (totalRelevantClasses > 1)
                            {
                                foreach (var otherClassEntry in classSymbolMap)
                                {
                                    var otherClass = otherClassEntry.Value;
                                    if (otherClass == null || otherClass.Equals(classSymbol, SymbolEqualityComparer.Default)) continue;
                                    if (!IsDescendantOf(otherClass, classSymbol))
                                    {
                                        classesNotInheriting++;
                                    }
                                }
                                hiddenAttributesSum += (double)classesNotInheriting / (totalRelevantClasses - 1);
                            }
                            else
                            {
                                hiddenAttributesSum += 1.0;
                            }
                        }
                    }
                }

                INamedTypeSymbol currentBaseType = classSymbol.BaseType;
                while (currentBaseType != null && currentBaseType.SpecialType != SpecialType.System_Object)
                {
                    foreach (var baseMember in currentBaseType.GetMembers())
                    {
                        if (baseMember == null || baseMember.IsImplicitlyDeclared) continue;

                        bool isOverriddenOrHidden = false;
                        foreach (var declaredMemberInCurrentClass in classSymbol.GetMembers())
                        {
                            if (declaredMemberInCurrentClass == null) continue;

                            if (declaredMemberInCurrentClass.Name == baseMember.Name)
                            {
                                if (declaredMemberInCurrentClass is IMethodSymbol currentMethod && baseMember is IMethodSymbol baseMethod)
                                {
                                    if (currentMethod.OverriddenMethod?.Equals(baseMethod, SymbolEqualityComparer.Default) == true ||
                                        (baseMethod.IsVirtual && currentMethod.IsVirtual && currentMethod.Name == baseMethod.Name &&
                                         System.Linq.Enumerable.SequenceEqual((IEnumerable<IParameterSymbol>)currentMethod.Parameters, (IEnumerable<IParameterSymbol>)baseMethod.Parameters, SymbolEqualityComparer.Default)) ||
                                         currentMethod.ExplicitInterfaceImplementations.Any())
                                    {
                                        isOverriddenOrHidden = true;
                                        break;
                                    }
                                }
                                else if ((declaredMemberInCurrentClass is IPropertySymbol || declaredMemberInCurrentClass is IFieldSymbol) &&
                                         (baseMember is IPropertySymbol || baseMember is IFieldSymbol))
                                {
                                    isOverriddenOrHidden = true;
                                    break;
                                }
                            }
                        }

                        if (!isOverriddenOrHidden)
                        {
                            if (baseMember.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public ||
                                (baseMember.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Protected && IsDescendantOf(classSymbol, baseMember.ContainingType)))
                            {
                                if (baseMember is IMethodSymbol inheritedMethod &&
                                    inheritedMethod.MethodKind != MethodKind.PropertyGet && inheritedMethod.MethodKind != MethodKind.PropertySet &&
                                    !inheritedMethod.MethodKind.ToString().Contains("Constructor") &&
                                    !inheritedMethod.IsStatic &&
                                    !inheritedMethod.IsAbstract)
                                {
                                    inheritedAndNotOverriddenMethods++;
                                }
                                else if ((baseMember is IPropertySymbol inheritedProp && !inheritedProp.IsStatic) ||
                                         (baseMember is IFieldSymbol inheritedField && !inheritedField.IsStatic))
                                {
                                    inheritedAndNotOverriddenAttributes++;
                                }
                            }
                        }
                    }
                    currentBaseType = currentBaseType.BaseType;
                }

                MifMetrics.Add(new ClassInheritanceMetric
                {
                    ClassName = className,
                    TotalMethods = totalMethodsInClass + inheritedAndNotOverriddenMethods,
                    InheritedMethods = inheritedAndNotOverriddenMethods
                });
                AifMetrics.Add(new ClassAttributeInheritanceMetric
                {
                    ClassName = className,
                    TotalAttributes = totalAttributesInClass + inheritedAndNotOverriddenAttributes,
                    InheritedAttributes = inheritedAndNotOverriddenAttributes
                });
                MhfMetrics.Add(new ClassMetric { ClassName = className, TotalMethods = totalMethodsInClass, HiddenMethods = hiddenMethodsSum });
                AhfMetrics.Add(new ClassAttributeMetric { ClassName = className, TotalAttributes = totalAttributesInClass, HiddenAttributes = hiddenAttributesSum });

                Action<ITypeSymbol> addCoupledType = (typeSymbol) =>
                {
                    if (typeSymbol == null || typeSymbol.IsImplicitlyDeclared || typeSymbol.Name.Contains("<") || typeSymbol.Name.Contains(">")) return;

                    if (typeSymbol is INamedTypeSymbol namedType && namedType.TypeKind == TypeKind.Class)
                    {
                        if (namedType.Equals(classSymbol, SymbolEqualityComparer.Default)) return;

                        if (!classSymbolMap.ContainsKey(namedType.ToDisplayString()))
                        {
                            return;
                        }

                        if (IsDescendantOf(classSymbol, namedType) || IsDescendantOf(namedType, classSymbol))
                        {
                            return;
                        }

                        if (namedType.SpecialType != SpecialType.None) return;

                        coupledClassesSet.Add(namedType.ToDisplayString());
                    }
                };

                foreach (var node in classDeclarationSyntax.DescendantNodesAndSelf())
                {
                    if (node == null) continue;

                    var typeInfo = currentSemanticModel.GetTypeInfo(node);
                    if (typeInfo.Type != null) addCoupledType(typeInfo.Type);

                    var symbolInfo = currentSemanticModel.GetSymbolInfo(node);
                    if (symbolInfo.Symbol != null && symbolInfo.Symbol.ContainingType != null)
                    {
                        addCoupledType(symbolInfo.Symbol.ContainingType);
                    }

                    if (node is ObjectCreationExpressionSyntax objCreation && objCreation.Type != null)
                    {
                        addCoupledType(currentSemanticModel.GetTypeInfo(objCreation.Type).Type);
                    }
                    else if (node is CastExpressionSyntax castExpr && castExpr.Type != null)
                    {
                        addCoupledType(currentSemanticModel.GetTypeInfo(castExpr.Type).Type);
                    }
                    else if (node is MemberAccessExpressionSyntax memberAccess && memberAccess.Expression != null)
                    {
                        addCoupledType(currentSemanticModel.GetTypeInfo(memberAccess.Expression).Type);
                    }
                    else if (node is InvocationExpressionSyntax invocation)
                    {
                        var methodSymbol = currentSemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
                        if (methodSymbol != null && methodSymbol.ContainingType != null)
                        {
                            addCoupledType(methodSymbol.ContainingType);
                        }
                    }
                    else if (node is VariableDeclarationSyntax varDecl && varDecl.Type != null)
                    {
                        addCoupledType(currentSemanticModel.GetTypeInfo(varDecl.Type).Type);
                    }
                    else if (node is ParameterSyntax param && param.Type != null)
                    {
                        addCoupledType(currentSemanticModel.GetTypeInfo(param.Type).Type);
                    }
                }
                CfMetrics.Add(new ClassCouplingMetric { ClassName = className, CoupledClasses = coupledClassesSet.Count });

                PfMetrics.Add(new ClassPolymorphismMetric
                {
                    ClassName = className,
                    NewMethods = newMethods,
                    OverridingMethods = overridingMethods
                });
            }

            foreach (var pf in PfMetrics)
            {
                if (classSymbolMap.TryGetValue(pf.ClassName, out var classSymbol))
                {
                    pf.DescendantsCount = CountDescendants(classSymbol);
                }
            }
        }

        private bool IsDescendantOf(INamedTypeSymbol potentialDescendant, INamedTypeSymbol potentialAncestor)
        {
            if (potentialDescendant == null || potentialAncestor == null || potentialDescendant.Equals(potentialAncestor, SymbolEqualityComparer.Default))
            {
                return false;
            }

            var currentBase = potentialDescendant.BaseType;
            while (currentBase != null && currentBase.SpecialType != SpecialType.System_Object)
            {
                if (currentBase.Equals(potentialAncestor, SymbolEqualityComparer.Default))
                {
                    return true;
                }
                currentBase = currentBase.BaseType;
            }
            return false;
        }

        private int CountDescendants(INamedTypeSymbol classSymbol)
        {
            int count = 0;
            if (classSymbol == null) return 0;

            foreach (var otherClassSymbol in classSymbolMap.Values)
            {
                if (otherClassSymbol == null) continue;
                if (IsDescendantOf(otherClassSymbol, classSymbol))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public static class SymbolExtensions
    {
        public static bool IsPropertyAccessor(this IMethodSymbol method)
        {
            return method.MethodKind == MethodKind.PropertyGet || method.MethodKind == MethodKind.PropertySet;
        }

        public static bool IsImplementationOfAnyInterfaceMethod(this IMethodSymbol method)
        {
            if (method.ExplicitInterfaceImplementations.Any())
            {
                return true;
            }

            foreach (var @interface in method.ContainingType.AllInterfaces)
            {
                foreach (var interfaceMethod in @interface.GetMembers().OfType<IMethodSymbol>())
                {
                    var implementation = method.ContainingType.FindImplementationForInterfaceMember(interfaceMethod);
                    if (implementation != null && implementation.Equals(method, SymbolEqualityComparer.Default))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class ClassMetric
    {
        public string ClassName { get; set; }
        public int TotalMethods { get; set; }
        public double HiddenMethods { get; set; }
    }

    public class ClassAttributeMetric
    {
        public string ClassName { get; set; }
        public int TotalAttributes { get; set; }
        public double HiddenAttributes { get; set; }
    }

    public class ClassInheritanceMetric
    {
        public string ClassName { get; set; }
        public int TotalMethods { get; set; }
        public int InheritedMethods { get; set; }
    }

    public class ClassAttributeInheritanceMetric
    {
        public string ClassName { get; set; }
        public int TotalAttributes { get; set; }
        public int InheritedAttributes { get; set; }
    }

    public class ClassCouplingMetric
    {
        public string ClassName { get; set; }
        public int CoupledClasses { get; set; }
    }

    public class ClassPolymorphismMetric
    {
        public string ClassName { get; set; }
        public int NewMethods { get; set; }
        public int OverridingMethods { get; set; }
        public int DescendantsCount { get; set; }
    }
}