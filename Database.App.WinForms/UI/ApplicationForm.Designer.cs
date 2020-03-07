namespace Database.App.WinForms.UI
{
    partial class ApplicationForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sqlTextBox = new System.Windows.Forms.RichTextBox();
            this.validateButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.RichTextBox();
            this.clearOutputTextAreaButton = new System.Windows.Forms.Button();
            this.loadFileButton = new System.Windows.Forms.Button();
            this.settingsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonSaveDOM = new System.Windows.Forms.Button();
            this.refreshSchemaButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sqlTextBox
            // 
            this.sqlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sqlTextBox.Location = new System.Drawing.Point(12, 12);
            this.sqlTextBox.Name = "sqlTextBox";
            this.sqlTextBox.Size = new System.Drawing.Size(797, 613);
            this.sqlTextBox.TabIndex = 0;
            this.sqlTextBox.Text = "";
            this.sqlTextBox.WordWrap = false;
            // 
            // validateButton
            // 
            this.validateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.validateButton.Location = new System.Drawing.Point(1256, 602);
            this.validateButton.Name = "validateButton";
            this.validateButton.Size = new System.Drawing.Size(75, 23);
            this.validateButton.TabIndex = 1;
            this.validateButton.Text = "Validate";
            this.validateButton.UseVisualStyleBackColor = true;
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(815, 264);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(516, 332);
            this.outputTextBox.TabIndex = 3;
            this.outputTextBox.Text = "";
            // 
            // clearOutputTextAreaButton
            // 
            this.clearOutputTextAreaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearOutputTextAreaButton.Location = new System.Drawing.Point(1175, 602);
            this.clearOutputTextAreaButton.Name = "clearOutputTextAreaButton";
            this.clearOutputTextAreaButton.Size = new System.Drawing.Size(75, 23);
            this.clearOutputTextAreaButton.TabIndex = 4;
            this.clearOutputTextAreaButton.Text = "Clear output";
            this.clearOutputTextAreaButton.UseVisualStyleBackColor = true;
            this.clearOutputTextAreaButton.Click += new System.EventHandler(this.clearOutputTextAreaButton_Click);
            // 
            // loadFileButton
            // 
            this.loadFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadFileButton.Location = new System.Drawing.Point(815, 602);
            this.loadFileButton.Name = "loadFileButton";
            this.loadFileButton.Size = new System.Drawing.Size(75, 23);
            this.loadFileButton.TabIndex = 5;
            this.loadFileButton.Text = "Load file";
            this.loadFileButton.UseVisualStyleBackColor = true;
            this.loadFileButton.Click += new System.EventHandler(this.resetScriptButton_Click);
            // 
            // settingsPanel
            // 
            this.settingsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanel.AutoScroll = true;
            this.settingsPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.settingsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.settingsPanel.Location = new System.Drawing.Point(815, 12);
            this.settingsPanel.Name = "settingsPanel";
            this.settingsPanel.Size = new System.Drawing.Size(516, 246);
            this.settingsPanel.TabIndex = 6;
            // 
            // buttonSaveDOM
            // 
            this.buttonSaveDOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveDOM.Location = new System.Drawing.Point(896, 602);
            this.buttonSaveDOM.Name = "buttonSaveDOM";
            this.buttonSaveDOM.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveDOM.TabIndex = 7;
            this.buttonSaveDOM.Text = "Save DOM";
            this.buttonSaveDOM.UseVisualStyleBackColor = true;
            this.buttonSaveDOM.Click += new System.EventHandler(this.buttonViewDOM_Click);
            // 
            // refreshSchemaButton
            // 
            this.refreshSchemaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshSchemaButton.Location = new System.Drawing.Point(978, 602);
            this.refreshSchemaButton.Name = "refreshSchemaButton";
            this.refreshSchemaButton.Size = new System.Drawing.Size(102, 23);
            this.refreshSchemaButton.TabIndex = 8;
            this.refreshSchemaButton.Text = "Refresh schema";
            this.refreshSchemaButton.UseVisualStyleBackColor = true;
            this.refreshSchemaButton.Click += new System.EventHandler(this.refreshSchemaButton_Click);
            // 
            // ApplicationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1343, 637);
            this.Controls.Add(this.refreshSchemaButton);
            this.Controls.Add(this.buttonSaveDOM);
            this.Controls.Add(this.settingsPanel);
            this.Controls.Add(this.loadFileButton);
            this.Controls.Add(this.clearOutputTextAreaButton);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.sqlTextBox);
            this.Name = "ApplicationForm";
            this.Text = "Database.Validator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox sqlTextBox;
        private System.Windows.Forms.Button validateButton;
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.Button clearOutputTextAreaButton;
        private System.Windows.Forms.Button loadFileButton;
        private System.Windows.Forms.FlowLayoutPanel settingsPanel;
        private System.Windows.Forms.Button buttonSaveDOM;
        private System.Windows.Forms.Button refreshSchemaButton;
    }
}

