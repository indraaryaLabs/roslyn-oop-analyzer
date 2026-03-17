// mood-metrics/MOOD Metrics/Form1.Designer.cs
namespace MOOD_Metrics
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            labelAppName = new Label();
            label1 = new Label();
            textBoxPath = new TextBox();
            buttonBrowse = new Button();
            buttonAnalyze = new Button();
            textBoxOutput = new TextBox();
            textBoxSource = new TextBox();
            textBoxSummary = new TextBox();
            buttonhelp = new Button();
            buttoninfo = new Button();
            buttonexport = new Button();
            buttonclear = new Button();
            checkBoxDebug = new CheckBox();
            buttonToggleFullscreen = new Button();
            flowLayoutPanelLeft = new FlowLayoutPanel();
            flowLayoutPanelRight = new FlowLayoutPanel();
            tableLayoutPanelMain = new TableLayoutPanel();
            flowLayoutPanelLeft.SuspendLayout();
            flowLayoutPanelRight.SuspendLayout();
            tableLayoutPanelMain.SuspendLayout();
            SuspendLayout();
            // 
            // labelAppName
            // 
            labelAppName.Anchor = AnchorStyles.Top;
            labelAppName.AutoSize = true;
            labelAppName.Font = new Font("Yu Gothic UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            labelAppName.Location = new Point(479, 21);
            labelAppName.Name = "labelAppName";
            labelAppName.Size = new Size(229, 28);
            labelAppName.TabIndex = 0;
            labelAppName.Text = "MOOD Metrics Analyzer";
            labelAppName.TextAlign = ContentAlignment.TopCenter;
            labelAppName.Click += labelAppName_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(11, 68);
            label1.Name = "label1";
            label1.Size = new Size(203, 23);
            label1.TabIndex = 1;
            label1.Text = "Choose Source Code File:";
            // 
            // textBoxPath
            // 
            textBoxPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPath.Location = new Point(220, 67);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(628, 27);
            textBoxPath.TabIndex = 2;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonBrowse.Location = new Point(879, 68);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(94, 29);
            buttonBrowse.TabIndex = 3;
            buttonBrowse.Text = "Browse";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // buttonAnalyze
            // 
            buttonAnalyze.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonAnalyze.BackColor = Color.White;
            buttonAnalyze.ForeColor = Color.Black;
            buttonAnalyze.Location = new Point(1005, 68);
            buttonAnalyze.Name = "buttonAnalyze";
            buttonAnalyze.Size = new Size(94, 29);
            buttonAnalyze.TabIndex = 4;
            buttonAnalyze.Text = "Analyze";
            buttonAnalyze.UseVisualStyleBackColor = false;
            buttonAnalyze.Click += buttonAnalyze_Click;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Dock = DockStyle.Fill;
            textBoxOutput.Location = new Point(353, 3);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = ScrollBars.Vertical;
            textBoxOutput.Size = new Size(399, 273);
            textBoxOutput.TabIndex = 1;
            // 
            // textBoxSource
            // 
            textBoxSource.Dock = DockStyle.Fill;
            textBoxSource.Location = new Point(3, 3);
            textBoxSource.Multiline = true;
            textBoxSource.Name = "textBoxSource";
            textBoxSource.ReadOnly = true;
            textBoxSource.ScrollBars = ScrollBars.Vertical;
            textBoxSource.Size = new Size(344, 273);
            textBoxSource.TabIndex = 0;
            // 
            // textBoxSummary
            // 
            textBoxSummary.Dock = DockStyle.Fill;
            textBoxSummary.Location = new Point(758, 3);
            textBoxSummary.Multiline = true;
            textBoxSummary.Name = "textBoxSummary";
            textBoxSummary.ReadOnly = true;
            textBoxSummary.ScrollBars = ScrollBars.Vertical;
            textBoxSummary.Size = new Size(399, 273);
            textBoxSummary.TabIndex = 2;
            // 
            // buttonhelp
            // 
            buttonhelp.Location = new Point(3, 3);
            buttonhelp.Name = "buttonhelp";
            buttonhelp.Size = new Size(82, 29);
            buttonhelp.TabIndex = 0;
            buttonhelp.Text = "Help";
            buttonhelp.UseVisualStyleBackColor = true;
            buttonhelp.Click += buttonhelp_Click;
            // 
            // buttoninfo
            // 
            buttoninfo.Location = new Point(91, 3);
            buttoninfo.Name = "buttoninfo";
            buttoninfo.Size = new Size(95, 29);
            buttoninfo.TabIndex = 1;
            buttoninfo.Text = "Information";
            buttoninfo.UseVisualStyleBackColor = true;
            buttoninfo.Click += buttoninfo_Click;
            // 
            // buttonexport
            // 
            buttonexport.Location = new Point(453, 3);
            buttonexport.Name = "buttonexport";
            buttonexport.Size = new Size(94, 29);
            buttonexport.TabIndex = 2;
            buttonexport.Text = "Export";
            buttonexport.UseVisualStyleBackColor = true;
            buttonexport.Click += buttonexport_Click;
            // 
            // buttonclear
            // 
            buttonclear.Location = new Point(192, 3);
            buttonclear.Name = "buttonclear";
            buttonclear.Size = new Size(80, 29);
            buttonclear.TabIndex = 2;
            buttonclear.Text = "Clear";
            buttonclear.UseVisualStyleBackColor = true;
            buttonclear.Click += buttonclear_Click;
            // 
            // checkBoxDebug
            // 
            checkBoxDebug.AutoSize = true;
            checkBoxDebug.Location = new Point(138, 3);
            checkBoxDebug.Name = "checkBoxDebug";
            checkBoxDebug.Size = new Size(193, 24);
            checkBoxDebug.TabIndex = 0;
            checkBoxDebug.Text = "Tampilkan Debug Metrik";
            checkBoxDebug.TextAlign = ContentAlignment.MiddleCenter;
            checkBoxDebug.UseVisualStyleBackColor = true;
            // 
            // buttonToggleFullscreen
            // 
            buttonToggleFullscreen.Location = new Point(337, 3);
            buttonToggleFullscreen.Name = "buttonToggleFullscreen";
            buttonToggleFullscreen.Size = new Size(110, 29);
            buttonToggleFullscreen.TabIndex = 1;
            buttonToggleFullscreen.Text = "Fullscreen";
            buttonToggleFullscreen.UseVisualStyleBackColor = true;
            buttonToggleFullscreen.Click += buttonToggleFullscreen_Click;
            // 
            // flowLayoutPanelLeft
            // 
            flowLayoutPanelLeft.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            flowLayoutPanelLeft.Controls.Add(buttonhelp);
            flowLayoutPanelLeft.Controls.Add(buttoninfo);
            flowLayoutPanelLeft.Controls.Add(buttonclear);
            flowLayoutPanelLeft.Location = new Point(12, 443);
            flowLayoutPanelLeft.Name = "flowLayoutPanelLeft";
            flowLayoutPanelLeft.Size = new Size(320, 35);
            flowLayoutPanelLeft.TabIndex = 14;
            flowLayoutPanelLeft.WrapContents = false;
            // 
            // flowLayoutPanelRight
            // 
            flowLayoutPanelRight.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            flowLayoutPanelRight.Controls.Add(buttonexport);
            flowLayoutPanelRight.Controls.Add(buttonToggleFullscreen);
            flowLayoutPanelRight.Controls.Add(checkBoxDebug);
            flowLayoutPanelRight.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanelRight.Location = new Point(600, 443);
            flowLayoutPanelRight.Name = "flowLayoutPanelRight";
            flowLayoutPanelRight.Size = new Size(550, 35);
            flowLayoutPanelRight.TabIndex = 15;
            flowLayoutPanelRight.WrapContents = false;
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanelMain.ColumnCount = 3;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanelMain.Controls.Add(textBoxSource, 0, 0);
            tableLayoutPanelMain.Controls.Add(textBoxOutput, 1, 0);
            tableLayoutPanelMain.Controls.Add(textBoxSummary, 2, 0);
            tableLayoutPanelMain.Location = new Point(12, 134);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 1;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Size = new Size(1160, 279);
            tableLayoutPanelMain.TabIndex = 16;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 503);
            Controls.Add(tableLayoutPanelMain);
            Controls.Add(flowLayoutPanelLeft);
            Controls.Add(flowLayoutPanelRight);
            Controls.Add(buttonAnalyze);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxPath);
            Controls.Add(label1);
            Controls.Add(labelAppName);
            ForeColor = Color.Black;
            Name = "Form1";
            Text = "Form1";
            flowLayoutPanelLeft.ResumeLayout(false);
            flowLayoutPanelRight.ResumeLayout(false);
            flowLayoutPanelRight.PerformLayout();
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

            this.progressBarAnalysis = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            //
            // progressBarAnalysis
            //
            this.progressBarAnalysis.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.progressBarAnalysis.Location = new System.Drawing.Point(12, 105); // Sesuaikan posisi
            this.progressBarAnalysis.Name = "progressBarAnalysis";
            this.progressBarAnalysis.Size = new System.Drawing.Size(1160, 23); // Sesuaikan lebar
            this.progressBarAnalysis.Style = System.Windows.Forms.ProgressBarStyle.Marquee; // Membuat animasi "berjalan"
            this.progressBarAnalysis.TabIndex = 17; // Sesuaikan jika sudah ada kontrol lain
            this.progressBarAnalysis.Visible = false; // Sembunyikan secara default
                                                      //
                                                      // Form1
                                                      //
            this.Controls.Add(this.progressBarAnalysis); // Tambahkan ke kontrol form
                                                         // ... (kode yang sudah ada) ...
            this.ResumeLayout(false);
            this.PerformLayout();
        }



        #endregion

        private Label labelAppName;
        private Label label1;
        private TextBox textBoxPath;
        private Button buttonBrowse;
        private Button buttonAnalyze;
        private TextBox textBoxOutput;
        private TextBox textBoxSource;
        private Button buttonhelp;
        private Button buttoninfo;
        private Button buttonexport;
        private Button buttonclear;
        private TextBox textBoxSummary;
        private CheckBox checkBoxDebug;
        private Button buttonToggleFullscreen;
        private FlowLayoutPanel flowLayoutPanelLeft;
        private FlowLayoutPanel flowLayoutPanelRight;
        private TableLayoutPanel tableLayoutPanelMain; // Deklarasi TableLayoutPanel
        private System.Windows.Forms.ProgressBar progressBarAnalysis;
    }
}

