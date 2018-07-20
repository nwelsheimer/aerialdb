namespace Aerial.db
{
    partial class ControlColorChooser
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.lblForeColor = new System.Windows.Forms.Label();
            this.lblSample = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(26, 198);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(183, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(26, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 38);
            this.label1.TabIndex = 2;
            this.label1.Text = "Foreground Color";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Click += new System.EventHandler(this.foreground_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(26, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 38);
            this.label2.TabIndex = 3;
            this.label2.Text = "Background Color";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.Click += new System.EventHandler(this.background_Click);
            // 
            // lblBackColor
            // 
            this.lblBackColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBackColor.Location = new System.Drawing.Point(144, 51);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(112, 38);
            this.lblBackColor.TabIndex = 5;
            this.lblBackColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBackColor.Click += new System.EventHandler(this.background_Click);
            // 
            // lblForeColor
            // 
            this.lblForeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblForeColor.Location = new System.Drawing.Point(144, 13);
            this.lblForeColor.Name = "lblForeColor";
            this.lblForeColor.Size = new System.Drawing.Size(112, 38);
            this.lblForeColor.TabIndex = 4;
            this.lblForeColor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblForeColor.Click += new System.EventHandler(this.foreground_Click);
            // 
            // lblSample
            // 
            this.lblSample.Location = new System.Drawing.Point(86, 112);
            this.lblSample.Name = "lblSample";
            this.lblSample.Size = new System.Drawing.Size(112, 38);
            this.lblSample.TabIndex = 6;
            this.lblSample.Text = "Sample";
            this.lblSample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ControlColorChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lblSample);
            this.Controls.Add(this.lblBackColor);
            this.Controls.Add(this.lblForeColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "ControlColorChooser";
            this.Text = "ControlColorChooser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblBackColor;
        private System.Windows.Forms.Label lblForeColor;
        private System.Windows.Forms.Label lblSample;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}