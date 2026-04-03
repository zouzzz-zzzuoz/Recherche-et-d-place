namespace Recherche_et_déplace
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
            versionLabel = new Label();
            searchButton = new Button();
            pathLabel = new Label();
            comboBoxColumn = new ComboBox();
            comboBoxRow = new ComboBox();
            inputTextBox = new TextBox();
            actionButton = new Button();
            SuspendLayout();
            // 
            // searchButton
            // 
            searchButton.Location = new Point(12, 12);
            searchButton.Name = "searchButton";
            searchButton.Size = new Size(100, 30);
            searchButton.TabIndex = 0;
            searchButton.Text = "Recherche";
            searchButton.UseVisualStyleBackColor = true;
            // 
            // pathLabel
            // 
            pathLabel.Location = new Point(12, 50);
            pathLabel.Name = "pathLabel";
            pathLabel.Size = new Size(760, 23);
            pathLabel.TabIndex = 1;
            pathLabel.Text = string.Empty;
            // 
            // comboBoxColumn
            // 
            columnLabel = new Label();
            columnLabel.Location = new Point(12, 60);
            columnLabel.Name = "columnLabel";
            columnLabel.Size = new Size(60, 20);
            columnLabel.Text = "Colonne :";
            comboBoxColumn.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxColumn.Location = new Point(12, 80);
            comboBoxColumn.Name = "comboBoxColumn";
            comboBoxColumn.Size = new Size(80, 23);
            comboBoxColumn.TabIndex = 2;
            for (char c = 'A'; c <= 'Z'; c++) comboBoxColumn.Items.Add(c.ToString());
            comboBoxColumn.SelectedIndex = 0;
            // 
            // comboBoxRow
            // 
            rowLabel = new Label();
            rowLabel.Location = new Point(110, 60);
            rowLabel.Name = "rowLabel";
            rowLabel.Size = new Size(60, 20);
            rowLabel.Text = "Ligne :";
            comboBoxRow.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxRow.Location = new Point(110, 80);
            comboBoxRow.Name = "comboBoxRow";
            comboBoxRow.Size = new Size(80, 23);
            comboBoxRow.TabIndex = 3;
            for (int i = 1; i <= 20; i++) comboBoxRow.Items.Add(i.ToString());
            comboBoxRow.SelectedIndex = 0;
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(12, 115);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(300, 23);
            inputTextBox.TabIndex = 4;
            // 
            // actionButton
            // 
            actionButton.Location = new Point(12, 150);
            actionButton.Name = "actionButton";
            actionButton.Size = new Size(100, 30);
            actionButton.TabIndex = 5;
            actionButton.Text = "Action";
            actionButton.UseVisualStyleBackColor = true;
            actionButton.Enabled = false;
            // 
            // versionLabel
            // 
            versionLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            versionLabel.AutoSize = true;
            versionLabel.Location = new Point(680, 426);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(0, 15);
            versionLabel.TabIndex = 6;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(searchButton);
            Controls.Add(pathLabel);
            Controls.Add(columnLabel);
            Controls.Add(comboBoxColumn);
            Controls.Add(rowLabel);
            Controls.Add(comboBoxRow);
            Controls.Add(inputTextBox);
            Controls.Add(actionButton);
            Controls.Add(versionLabel);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
