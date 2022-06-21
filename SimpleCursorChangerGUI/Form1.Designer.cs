namespace SimpleCursorChangerGUI
{
    partial class MainForm
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
            this.cbCursorSize = new System.Windows.Forms.ComboBox();
            this.lCursorSize = new System.Windows.Forms.Label();
            this.lPath = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnPath = new System.Windows.Forms.Button();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbCursorSize
            // 
            this.cbCursorSize.FormattingEnabled = true;
            this.cbCursorSize.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.cbCursorSize.Location = new System.Drawing.Point(125, 47);
            this.cbCursorSize.Name = "cbCursorSize";
            this.cbCursorSize.Size = new System.Drawing.Size(51, 28);
            this.cbCursorSize.TabIndex = 23;
            this.cbCursorSize.Text = "1";
            // 
            // lCursorSize
            // 
            this.lCursorSize.AutoSize = true;
            this.lCursorSize.Location = new System.Drawing.Point(13, 50);
            this.lCursorSize.Name = "lCursorSize";
            this.lCursorSize.Size = new System.Drawing.Size(85, 20);
            this.lCursorSize.TabIndex = 22;
            this.lCursorSize.Text = "Cursor Size";
            // 
            // lPath
            // 
            this.lPath.AutoSize = true;
            this.lPath.Location = new System.Drawing.Point(13, 15);
            this.lPath.Name = "lPath";
            this.lPath.Size = new System.Drawing.Size(40, 20);
            this.lPath.TabIndex = 21;
            this.lPath.Text = "Path";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(261, 47);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(73, 29);
            this.btnApply.TabIndex = 20;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(261, 15);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(73, 29);
            this.btnPath.TabIndex = 19;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(80, 12);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(175, 27);
            this.tbPath.TabIndex = 18;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(182, 46);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(73, 29);
            this.btnReset.TabIndex = 24;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 90);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.cbCursorSize);
            this.Controls.Add(this.lCursorSize);
            this.Controls.Add(this.lPath);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.tbPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "SimpleCursorChanger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbCursorSize;
        private System.Windows.Forms.Label lCursorSize;
        private System.Windows.Forms.Label lPath;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btnReset;
    }
}
