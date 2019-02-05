namespace Diff.Net
{
    partial class CompareOptionsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox typeGroup;
            System.Windows.Forms.GroupBox optionsGroup;
            this.compareBinary = new System.Windows.Forms.RadioButton();
            this.compareXml = new System.Windows.Forms.RadioButton();
            this.compareText = new System.Windows.Forms.RadioButton();
            this.compareAuto = new System.Windows.Forms.RadioButton();
            this.binaryFootprintLength = new System.Windows.Forms.NumericUpDown();
            this.binaryLabel = new System.Windows.Forms.Label();
            this.ignoreXmlWhitespace = new System.Windows.Forms.CheckBox();
            this.ignoreTextWhitespace = new System.Windows.Forms.CheckBox();
            this.ignoreCase = new System.Windows.Forms.CheckBox();
            typeGroup = new System.Windows.Forms.GroupBox();
            optionsGroup = new System.Windows.Forms.GroupBox();
            typeGroup.SuspendLayout();
            optionsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.binaryFootprintLength)).BeginInit();
            this.SuspendLayout();
            // 
            // typeGroup
            // 
            typeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            typeGroup.Controls.Add(this.compareBinary);
            typeGroup.Controls.Add(this.compareXml);
            typeGroup.Controls.Add(this.compareText);
            typeGroup.Controls.Add(this.compareAuto);
            typeGroup.Location = new System.Drawing.Point(4, 0);
            typeGroup.Name = "typeGroup";
            typeGroup.Size = new System.Drawing.Size(80, 128);
            typeGroup.TabIndex = 0;
            typeGroup.TabStop = false;
            typeGroup.Text = "Compare";
            // 
            // compareBinary
            // 
            this.compareBinary.AutoSize = true;
            this.compareBinary.Location = new System.Drawing.Point(12, 96);
            this.compareBinary.Name = "compareBinary";
            this.compareBinary.Size = new System.Drawing.Size(54, 17);
            this.compareBinary.TabIndex = 3;
            this.compareBinary.TabStop = true;
            this.compareBinary.Text = "Binary";
            this.compareBinary.UseVisualStyleBackColor = true;
            // 
            // compareXml
            // 
            this.compareXml.AutoSize = true;
            this.compareXml.Location = new System.Drawing.Point(12, 72);
            this.compareXml.Name = "compareXml";
            this.compareXml.Size = new System.Drawing.Size(47, 17);
            this.compareXml.TabIndex = 2;
            this.compareXml.TabStop = true;
            this.compareXml.Text = "XML";
            this.compareXml.UseVisualStyleBackColor = true;
            // 
            // compareText
            // 
            this.compareText.AutoSize = true;
            this.compareText.Location = new System.Drawing.Point(12, 48);
            this.compareText.Name = "compareText";
            this.compareText.Size = new System.Drawing.Size(46, 17);
            this.compareText.TabIndex = 1;
            this.compareText.TabStop = true;
            this.compareText.Text = "Text";
            this.compareText.UseVisualStyleBackColor = true;
            // 
            // compareAuto
            // 
            this.compareAuto.AutoSize = true;
            this.compareAuto.Location = new System.Drawing.Point(12, 24);
            this.compareAuto.Name = "compareAuto";
            this.compareAuto.Size = new System.Drawing.Size(47, 17);
            this.compareAuto.TabIndex = 0;
            this.compareAuto.TabStop = true;
            this.compareAuto.Text = "Auto";
            this.compareAuto.UseVisualStyleBackColor = true;
            // 
            // optionsGroup
            // 
            optionsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            optionsGroup.Controls.Add(this.binaryFootprintLength);
            optionsGroup.Controls.Add(this.binaryLabel);
            optionsGroup.Controls.Add(this.ignoreXmlWhitespace);
            optionsGroup.Controls.Add(this.ignoreTextWhitespace);
            optionsGroup.Controls.Add(this.ignoreCase);
            optionsGroup.Location = new System.Drawing.Point(92, 0);
            optionsGroup.Name = "optionsGroup";
            optionsGroup.Size = new System.Drawing.Size(276, 128);
            optionsGroup.TabIndex = 1;
            optionsGroup.TabStop = false;
            optionsGroup.Text = "Options";
            // 
            // binaryFootprintLength
            // 
            this.binaryFootprintLength.Location = new System.Drawing.Point(140, 96);
            this.binaryFootprintLength.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.binaryFootprintLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.binaryFootprintLength.Name = "binaryFootprintLength";
            this.binaryFootprintLength.Size = new System.Drawing.Size(48, 20);
            this.binaryFootprintLength.TabIndex = 4;
            this.binaryFootprintLength.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // binaryLabel
            // 
            this.binaryLabel.AutoSize = true;
            this.binaryLabel.Location = new System.Drawing.Point(10, 98);
            this.binaryLabel.Name = "binaryLabel";
            this.binaryLabel.Size = new System.Drawing.Size(119, 13);
            this.binaryLabel.TabIndex = 3;
            this.binaryLabel.Text = "Binary Footprint Length:";
            // 
            // ignoreXmlWhitespace
            // 
            this.ignoreXmlWhitespace.AutoSize = true;
            this.ignoreXmlWhitespace.Location = new System.Drawing.Point(12, 72);
            this.ignoreXmlWhitespace.Name = "ignoreXmlWhitespace";
            this.ignoreXmlWhitespace.Size = new System.Drawing.Size(246, 17);
            this.ignoreXmlWhitespace.TabIndex = 2;
            this.ignoreXmlWhitespace.Text = "Ignore Insignificant Whitespace Nodes In XML";
            this.ignoreXmlWhitespace.UseVisualStyleBackColor = true;
            // 
            // ignoreTextWhitespace
            // 
            this.ignoreTextWhitespace.AutoSize = true;
            this.ignoreTextWhitespace.Location = new System.Drawing.Point(12, 48);
            this.ignoreTextWhitespace.Name = "ignoreTextWhitespace";
            this.ignoreTextWhitespace.Size = new System.Drawing.Size(252, 17);
            this.ignoreTextWhitespace.TabIndex = 1;
            this.ignoreTextWhitespace.Text = "Ignore Leading And Trailing Whitespace In Text";
            this.ignoreTextWhitespace.UseVisualStyleBackColor = true;
            // 
            // ignoreCase
            // 
            this.ignoreCase.AutoSize = true;
            this.ignoreCase.Location = new System.Drawing.Point(12, 24);
            this.ignoreCase.Name = "ignoreCase";
            this.ignoreCase.Size = new System.Drawing.Size(119, 17);
            this.ignoreCase.TabIndex = 0;
            this.ignoreCase.Text = "Ignore Case In Text";
            this.ignoreCase.UseVisualStyleBackColor = true;
            // 
            // CompareOptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(optionsGroup);
            this.Controls.Add(typeGroup);
            this.Name = "CompareOptionsControl";
            this.Size = new System.Drawing.Size(373, 133);
            this.Load += new System.EventHandler(this.CompareOptionsControl_Load);
            typeGroup.ResumeLayout(false);
            typeGroup.PerformLayout();
            optionsGroup.ResumeLayout(false);
            optionsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.binaryFootprintLength)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton compareBinary;
        private System.Windows.Forms.RadioButton compareXml;
        private System.Windows.Forms.RadioButton compareText;
        private System.Windows.Forms.RadioButton compareAuto;
        private System.Windows.Forms.NumericUpDown binaryFootprintLength;
        private System.Windows.Forms.CheckBox ignoreXmlWhitespace;
        private System.Windows.Forms.CheckBox ignoreTextWhitespace;
        private System.Windows.Forms.CheckBox ignoreCase;
        private System.Windows.Forms.Label binaryLabel;
    }
}
