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
            SuspendLayout();
            // 
            // labelAppName
            // 
            labelAppName.AutoSize = true;
            labelAppName.Location = new Point(305, 9);
            labelAppName.Name = "labelAppName";
            labelAppName.Size = new Size(168, 20);
            labelAppName.TabIndex = 0;
            labelAppName.Text = "MOOD Metrics Analyzer";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 68);
            label1.Name = "label1";
            label1.Size = new Size(176, 20);
            label1.TabIndex = 1;
            label1.Text = "Choose Source Code File:";
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(223, 65);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.Size = new Size(311, 27);
            textBoxPath.TabIndex = 2;
            // 
            // buttonBrowse
            // 
            buttonBrowse.Location = new Point(564, 68);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new Size(94, 29);
            buttonBrowse.TabIndex = 3;
            buttonBrowse.Text = "Browse";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // buttonAnalyze
            // 
            buttonAnalyze.Location = new Point(683, 68);
            buttonAnalyze.Name = "buttonAnalyze";
            buttonAnalyze.Size = new Size(94, 29);
            buttonAnalyze.TabIndex = 4;
            buttonAnalyze.Text = "Analyze";
            buttonAnalyze.UseVisualStyleBackColor = true;
            buttonAnalyze.Click += buttonAnalyze_Click;
            // 
            // textBoxOutput
            // 
            textBoxOutput.Location = new Point(396, 134);
            textBoxOutput.Multiline = true;
            textBoxOutput.Name = "textBoxOutput";
            textBoxOutput.ReadOnly = true;
            textBoxOutput.Size = new Size(347, 279);
            textBoxOutput.TabIndex = 5;
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 134);
            textBoxSource.Multiline = true;
            textBoxSource.Name = "textBoxSource";
            textBoxSource.ReadOnly = true;
            textBoxSource.Size = new Size(347, 279);
            textBoxSource.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBoxSource);
            Controls.Add(textBoxOutput);
            Controls.Add(buttonAnalyze);
            Controls.Add(buttonBrowse);
            Controls.Add(textBoxPath);
            Controls.Add(label1);
            Controls.Add(labelAppName);
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
    }
}