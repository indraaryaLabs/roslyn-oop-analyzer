namespace MOOD_Metrics
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelAppName = new Label();
            label1 = new Label();
            textBoxPath = new TextBox();
            buttonBrowse = new Button();
            buttonAnalyze = new Button();
            textBoxOutput = new TextBox();
            textBoxSource = new TextBox();
            buttonhelp = new Button();
            buttoninfo = new Button();
            buttonexport = new Button();
            buttonclear = new Button();
            textBoxSummary = new TextBox();
            SuspendLayout();
            // 
            // labelAppName
            // 
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
            textBoxPath.Location = new Point(220, 67);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(628, 27);
            textBoxPath.TabIndex = 2;
            // 
            // buttonBrowse
            // 
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
            textBoxOutput.Location = new Point(396, 134);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.ScrollBars = ScrollBars.Vertical;
            textBoxOutput.Size = new Size(347, 279);
            textBoxOutput.TabIndex = 5;
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 134);
            textBoxSource.Multiline = true;
            textBoxSource.Name = "textBoxSource";
            textBoxSource.ReadOnly = true;
            textBoxSource.ScrollBars = ScrollBars.Vertical;
            textBoxSource.Size = new Size(347, 279);
            textBoxSource.TabIndex = 6;
            // 
            // buttonhelp
            // 
            buttonhelp.Location = new Point(12, 443);
            buttonhelp.Name = "buttonhelp";
            buttonhelp.Size = new Size(82, 29);
            buttonhelp.TabIndex = 7;
            buttonhelp.Text = "Help";
            buttonhelp.UseVisualStyleBackColor = true;
            buttonhelp.Click += buttonhelp_Click;
            // 
            // buttoninfo
            // 
            buttoninfo.Location = new Point(119, 443);
            buttoninfo.Name = "buttoninfo";
            buttoninfo.Size = new Size(95, 29);
            buttoninfo.TabIndex = 8;
            buttoninfo.Text = "Information";
            buttoninfo.UseVisualStyleBackColor = true;
            buttoninfo.Click += buttoninfo_Click;
            // 
            // buttonexport
            // 
            buttonexport.Location = new Point(1019, 443);
            buttonexport.Name = "buttonexport";
            buttonexport.Size = new Size(94, 29);
            buttonexport.TabIndex = 9;
            buttonexport.Text = "Export";
            buttonexport.UseVisualStyleBackColor = true;
            buttonexport.Click += buttonexport_Click;
            // 
            // buttonclear
            // 
            buttonclear.Location = new Point(237, 443);
            buttonclear.Name = "buttonclear";
            buttonclear.Size = new Size(80, 29);
            buttonclear.TabIndex = 10;
            buttonclear.Text = "Clear";
            buttonclear.UseVisualStyleBackColor = true;
            buttonclear.Click += buttonclear_Click;
            // 
            // textBoxSummary
            // 
            textBoxSummary.Location = new Point(766, 134);
            textBoxSummary.Multiline = true;
            textBoxSummary.Name = "textBoxSummary";
            textBoxSummary.ReadOnly = true;
            textBoxSummary.ScrollBars = ScrollBars.Vertical;
            textBoxSummary.Size = new Size(347, 279);
            textBoxSummary.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1182, 503);
            Controls.Add(textBoxSummary);
            Controls.Add(buttonclear);
            Controls.Add(buttonexport);
            Controls.Add(buttoninfo);
            Controls.Add(buttonhelp);
            Controls.Add(textBoxSource);
            Controls.Add(textBoxOutput);
            Controls.Add(buttonAnalyze);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxPath);
            Controls.Add(label1);
            Controls.Add(labelAppName);
            ForeColor = Color.Black;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
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
    }
}