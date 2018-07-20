namespace Aerial.db.WOP
{
    partial class Form1
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.lbPilot = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lbWorkOrder = new System.Windows.Forms.ListBox();
			this.lblTargetPests = new System.Windows.Forms.Label();
			this.lblProducts = new System.Windows.Forms.Label();
			this.lblCreatedOn = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lblAdditionalNotes = new System.Windows.Forms.Label();
			this.txtAdditionalNotes = new System.Windows.Forms.TextBox();
			this.txtApplicationTotal = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.txtApplicationRate = new System.Windows.Forms.TextBox();
			this.rbGallons = new System.Windows.Forms.RadioButton();
			this.label13 = new System.Windows.Forms.Label();
			this.rbPounds = new System.Windows.Forms.RadioButton();
			this.txtApplicationLoads = new System.Windows.Forms.TextBox();
			this.txtApplicationAcresPerLoad = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.txtApplicationAmountPerLoad = new System.Windows.Forms.TextBox();
			this.dgFields = new System.Windows.Forms.DataGridView();
			this.colCompleted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.colFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colLatLong = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colPilot = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colLastUpdated = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label16 = new System.Windows.Forms.Label();
			this.lbStatus = new System.Windows.Forms.ListBox();
			this.txtStatus = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnExport = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtUofMPerAc = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.txtUofM = new System.Windows.Forms.Label();
			this.txtCreatedOn = new System.Windows.Forms.Label();
			this.txtTargetPests = new System.Windows.Forms.Label();
			this.txtPilot = new System.Windows.Forms.Label();
			this.lblPilot = new System.Windows.Forms.Label();
			this.clbProducts = new System.Windows.Forms.CheckedListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtCustomer = new System.Windows.Forms.Label();
			this.cbAutoRefresh = new System.Windows.Forms.CheckBox();
			this.pbProcessing = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.dgFields)).BeginInit();
			this.SuspendLayout();
			// 
			// lbPilot
			// 
			this.lbPilot.FormattingEnabled = true;
			this.lbPilot.Location = new System.Drawing.Point(98, 44);
			this.lbPilot.Name = "lbPilot";
			this.lbPilot.Size = new System.Drawing.Size(69, 108);
			this.lbPilot.Sorted = true;
			this.lbPilot.TabIndex = 3;
			this.lbPilot.SelectedIndexChanged += new System.EventHandler(this.lbPilot_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(98, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Pilot:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(183, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Work Order:";
			// 
			// lbWorkOrder
			// 
			this.lbWorkOrder.FormattingEnabled = true;
			this.lbWorkOrder.Location = new System.Drawing.Point(186, 44);
			this.lbWorkOrder.Name = "lbWorkOrder";
			this.lbWorkOrder.Size = new System.Drawing.Size(69, 108);
			this.lbWorkOrder.Sorted = true;
			this.lbWorkOrder.TabIndex = 5;
			this.lbWorkOrder.SelectedIndexChanged += new System.EventHandler(this.lbWorkOrder_SelectedIndexChanged);
			// 
			// lblTargetPests
			// 
			this.lblTargetPests.AutoSize = true;
			this.lblTargetPests.Location = new System.Drawing.Point(272, 69);
			this.lblTargetPests.Name = "lblTargetPests";
			this.lblTargetPests.Size = new System.Drawing.Size(70, 13);
			this.lblTargetPests.TabIndex = 10;
			this.lblTargetPests.Text = "Target Pests:";
			// 
			// lblProducts
			// 
			this.lblProducts.AutoSize = true;
			this.lblProducts.Location = new System.Drawing.Point(272, 82);
			this.lblProducts.Name = "lblProducts";
			this.lblProducts.Size = new System.Drawing.Size(52, 13);
			this.lblProducts.TabIndex = 12;
			this.lblProducts.Text = "Products:";
			// 
			// lblCreatedOn
			// 
			this.lblCreatedOn.AutoSize = true;
			this.lblCreatedOn.Location = new System.Drawing.Point(272, 56);
			this.lblCreatedOn.Name = "lblCreatedOn";
			this.lblCreatedOn.Size = new System.Drawing.Size(64, 13);
			this.lblCreatedOn.TabIndex = 8;
			this.lblCreatedOn.Text = "Created On:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(12, 189);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(37, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "Fields:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(291, 430);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(66, 13);
			this.label11.TabIndex = 29;
			this.label11.Text = "Acres/Load:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(291, 404);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(39, 13);
			this.label10.TabIndex = 27;
			this.label10.Text = "Loads:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(291, 378);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(33, 13);
			this.label9.TabIndex = 25;
			this.label9.Text = "Rate:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(9, 378);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(34, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "Total:";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(10, 355);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 13);
			this.label7.TabIndex = 18;
			this.label7.Text = "Application Rates";
			// 
			// lblAdditionalNotes
			// 
			this.lblAdditionalNotes.AutoSize = true;
			this.lblAdditionalNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblAdditionalNotes.Location = new System.Drawing.Point(8, 484);
			this.lblAdditionalNotes.Name = "lblAdditionalNotes";
			this.lblAdditionalNotes.Size = new System.Drawing.Size(84, 13);
			this.lblAdditionalNotes.TabIndex = 36;
			this.lblAdditionalNotes.Text = "Additional Notes";
			// 
			// txtAdditionalNotes
			// 
			this.txtAdditionalNotes.Location = new System.Drawing.Point(11, 500);
			this.txtAdditionalNotes.Multiline = true;
			this.txtAdditionalNotes.Name = "txtAdditionalNotes";
			this.txtAdditionalNotes.Size = new System.Drawing.Size(520, 75);
			this.txtAdditionalNotes.TabIndex = 37;
			this.txtAdditionalNotes.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// txtApplicationTotal
			// 
			this.txtApplicationTotal.Location = new System.Drawing.Point(106, 378);
			this.txtApplicationTotal.Name = "txtApplicationTotal";
			this.txtApplicationTotal.Size = new System.Drawing.Size(100, 20);
			this.txtApplicationTotal.TabIndex = 20;
			this.txtApplicationTotal.Text = "0";
			this.txtApplicationTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtApplicationTotal.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(212, 381);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(33, 13);
			this.label12.TabIndex = 21;
			this.label12.Text = "acres";
			// 
			// txtApplicationRate
			// 
			this.txtApplicationRate.Location = new System.Drawing.Point(387, 375);
			this.txtApplicationRate.Name = "txtApplicationRate";
			this.txtApplicationRate.Size = new System.Drawing.Size(100, 20);
			this.txtApplicationRate.TabIndex = 26;
			this.txtApplicationRate.Text = "0";
			this.txtApplicationRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtApplicationRate.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// rbGallons
			// 
			this.rbGallons.AutoSize = true;
			this.rbGallons.Checked = true;
			this.rbGallons.Location = new System.Drawing.Point(106, 404);
			this.rbGallons.Name = "rbGallons";
			this.rbGallons.Size = new System.Drawing.Size(60, 17);
			this.rbGallons.TabIndex = 23;
			this.rbGallons.TabStop = true;
			this.rbGallons.Tag = "gal";
			this.rbGallons.Text = "Gallons";
			this.rbGallons.UseVisualStyleBackColor = true;
			this.rbGallons.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(10, 406);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(85, 13);
			this.label13.TabIndex = 22;
			this.label13.Text = "Unit of Measure:";
			// 
			// rbPounds
			// 
			this.rbPounds.AutoSize = true;
			this.rbPounds.Location = new System.Drawing.Point(106, 425);
			this.rbPounds.Name = "rbPounds";
			this.rbPounds.Size = new System.Drawing.Size(61, 17);
			this.rbPounds.TabIndex = 24;
			this.rbPounds.TabStop = true;
			this.rbPounds.Tag = "lbs";
			this.rbPounds.Text = "Pounds";
			this.rbPounds.UseVisualStyleBackColor = true;
			this.rbPounds.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// txtApplicationLoads
			// 
			this.txtApplicationLoads.Location = new System.Drawing.Point(387, 401);
			this.txtApplicationLoads.Name = "txtApplicationLoads";
			this.txtApplicationLoads.Size = new System.Drawing.Size(100, 20);
			this.txtApplicationLoads.TabIndex = 28;
			this.txtApplicationLoads.Text = "0";
			this.txtApplicationLoads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtApplicationLoads.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// txtApplicationAcresPerLoad
			// 
			this.txtApplicationAcresPerLoad.Location = new System.Drawing.Point(387, 427);
			this.txtApplicationAcresPerLoad.Name = "txtApplicationAcresPerLoad";
			this.txtApplicationAcresPerLoad.Size = new System.Drawing.Size(100, 20);
			this.txtApplicationAcresPerLoad.TabIndex = 30;
			this.txtApplicationAcresPerLoad.Text = "0";
			this.txtApplicationAcresPerLoad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtApplicationAcresPerLoad.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(291, 456);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(75, 13);
			this.label15.TabIndex = 31;
			this.label15.Text = "Amount/Load:";
			// 
			// txtApplicationAmountPerLoad
			// 
			this.txtApplicationAmountPerLoad.Location = new System.Drawing.Point(387, 453);
			this.txtApplicationAmountPerLoad.Name = "txtApplicationAmountPerLoad";
			this.txtApplicationAmountPerLoad.Size = new System.Drawing.Size(100, 20);
			this.txtApplicationAmountPerLoad.TabIndex = 32;
			this.txtApplicationAmountPerLoad.Text = "0";
			this.txtApplicationAmountPerLoad.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtApplicationAmountPerLoad.TextChanged += new System.EventHandler(this.txt_TextChanged);
			// 
			// dgFields
			// 
			this.dgFields.AllowUserToAddRows = false;
			this.dgFields.AllowUserToDeleteRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCompleted,
            this.colFieldName,
            this.colLatLong,
            this.colArea,
            this.colPilot,
            this.colLastUpdated});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgFields.DefaultCellStyle = dataGridViewCellStyle2;
			this.dgFields.Location = new System.Drawing.Point(12, 205);
			this.dgFields.Name = "dgFields";
			this.dgFields.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgFields.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgFields.RowHeadersVisible = false;
			this.dgFields.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgFields.ShowEditingIcon = false;
			this.dgFields.Size = new System.Drawing.Size(520, 137);
			this.dgFields.TabIndex = 17;
			// 
			// colCompleted
			// 
			this.colCompleted.HeaderText = "Completed";
			this.colCompleted.Name = "colCompleted";
			this.colCompleted.ReadOnly = true;
			this.colCompleted.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colCompleted.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// colFieldName
			// 
			this.colFieldName.HeaderText = "Name";
			this.colFieldName.Name = "colFieldName";
			this.colFieldName.ReadOnly = true;
			// 
			// colLatLong
			// 
			this.colLatLong.HeaderText = "LatLong";
			this.colLatLong.Name = "colLatLong";
			this.colLatLong.ReadOnly = true;
			// 
			// colArea
			// 
			this.colArea.HeaderText = "Area";
			this.colArea.Name = "colArea";
			this.colArea.ReadOnly = true;
			// 
			// colPilot
			// 
			this.colPilot.HeaderText = "Pilot";
			this.colPilot.Name = "colPilot";
			// 
			// colLastUpdated
			// 
			this.colLastUpdated.HeaderText = "Last Updated";
			this.colLastUpdated.Name = "colLastUpdated";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(9, 28);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(40, 13);
			this.label16.TabIndex = 0;
			this.label16.Text = "Status:";
			// 
			// lbStatus
			// 
			this.lbStatus.FormattingEnabled = true;
			this.lbStatus.Location = new System.Drawing.Point(9, 44);
			this.lbStatus.Name = "lbStatus";
			this.lbStatus.Size = new System.Drawing.Size(69, 108);
			this.lbStatus.TabIndex = 1;
			this.lbStatus.SelectedIndexChanged += new System.EventHandler(this.listBoxStatus_SelectedIndexChanged);
			// 
			// txtStatus
			// 
			this.txtStatus.AutoSize = true;
			this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStatus.Location = new System.Drawing.Point(210, 164);
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.Size = new System.Drawing.Size(176, 25);
			this.txtStatus.TabIndex = 15;
			this.txtStatus.Text = "Status: Complete";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(11, 592);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(98, 23);
			this.btnSave.TabIndex = 38;
			this.btnSave.Text = "Save Changes";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnExport
			// 
			this.btnExport.Location = new System.Drawing.Point(424, 592);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(107, 23);
			this.btnExport.TabIndex = 40;
			this.btnExport.Text = "Export for Invoicing";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(146, 592);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(98, 23);
			this.btnCancel.TabIndex = 39;
			this.btnCancel.Text = "Cancel Changes";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtUofMPerAc
			// 
			this.txtUofMPerAc.AutoSize = true;
			this.txtUofMPerAc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtUofMPerAc.Location = new System.Drawing.Point(493, 378);
			this.txtUofMPerAc.Name = "txtUofMPerAc";
			this.txtUofMPerAc.Size = new System.Drawing.Size(38, 13);
			this.txtUofMPerAc.TabIndex = 33;
			this.txtUofMPerAc.Text = "gal/ac";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label19.Location = new System.Drawing.Point(493, 430);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(33, 13);
			this.label19.TabIndex = 34;
			this.label19.Text = "acres";
			// 
			// txtUofM
			// 
			this.txtUofM.AutoSize = true;
			this.txtUofM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtUofM.Location = new System.Drawing.Point(493, 460);
			this.txtUofM.Name = "txtUofM";
			this.txtUofM.Size = new System.Drawing.Size(21, 13);
			this.txtUofM.TabIndex = 35;
			this.txtUofM.Text = "gal";
			// 
			// txtCreatedOn
			// 
			this.txtCreatedOn.AutoSize = true;
			this.txtCreatedOn.Location = new System.Drawing.Point(348, 56);
			this.txtCreatedOn.Name = "txtCreatedOn";
			this.txtCreatedOn.Size = new System.Drawing.Size(0, 13);
			this.txtCreatedOn.TabIndex = 9;
			// 
			// txtTargetPests
			// 
			this.txtTargetPests.AutoSize = true;
			this.txtTargetPests.Location = new System.Drawing.Point(348, 69);
			this.txtTargetPests.Name = "txtTargetPests";
			this.txtTargetPests.Size = new System.Drawing.Size(0, 13);
			this.txtTargetPests.TabIndex = 11;
			// 
			// txtPilot
			// 
			this.txtPilot.AutoSize = true;
			this.txtPilot.Location = new System.Drawing.Point(348, 43);
			this.txtPilot.Name = "txtPilot";
			this.txtPilot.Size = new System.Drawing.Size(0, 13);
			this.txtPilot.TabIndex = 7;
			// 
			// lblPilot
			// 
			this.lblPilot.AutoSize = true;
			this.lblPilot.Location = new System.Drawing.Point(272, 43);
			this.lblPilot.Name = "lblPilot";
			this.lblPilot.Size = new System.Drawing.Size(30, 13);
			this.lblPilot.TabIndex = 6;
			this.lblPilot.Text = "Pilot:";
			// 
			// clbProducts
			// 
			this.clbProducts.FormattingEnabled = true;
			this.clbProducts.Location = new System.Drawing.Point(351, 85);
			this.clbProducts.Name = "clbProducts";
			this.clbProducts.Size = new System.Drawing.Size(181, 64);
			this.clbProducts.TabIndex = 13;
			this.clbProducts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbProducts_ItemCheck);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(275, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(257, 15);
			this.label3.TabIndex = 14;
			this.label3.Text = "*Items checked are customer supplied.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Location = new System.Drawing.Point(427, 5);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(107, 23);
			this.btnRefresh.TabIndex = 41;
			this.btnRefresh.Text = "Refresh";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
			// 
			// lblVersion
			// 
			this.lblVersion.Location = new System.Drawing.Point(427, 31);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(104, 16);
			this.lblVersion.TabIndex = 42;
			this.lblVersion.Text = "Version:";
			this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(272, 30);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 13);
			this.label5.TabIndex = 43;
			this.label5.Text = "Customer:";
			// 
			// txtCustomer
			// 
			this.txtCustomer.AutoSize = true;
			this.txtCustomer.Location = new System.Drawing.Point(348, 30);
			this.txtCustomer.Name = "txtCustomer";
			this.txtCustomer.Size = new System.Drawing.Size(0, 13);
			this.txtCustomer.TabIndex = 44;
			// 
			// cbAutoRefresh
			// 
			this.cbAutoRefresh.AutoSize = true;
			this.cbAutoRefresh.Location = new System.Drawing.Point(323, 9);
			this.cbAutoRefresh.Name = "cbAutoRefresh";
			this.cbAutoRefresh.Size = new System.Drawing.Size(88, 17);
			this.cbAutoRefresh.TabIndex = 45;
			this.cbAutoRefresh.Text = "Auto Refresh";
			this.cbAutoRefresh.UseVisualStyleBackColor = true;
			this.cbAutoRefresh.CheckedChanged += new System.EventHandler(this.cbAutoRefresh_CheckedChanged);
			// 
			// pbProcessing
			// 
			this.pbProcessing.Location = new System.Drawing.Point(11, 621);
			this.pbProcessing.Name = "pbProcessing";
			this.pbProcessing.Size = new System.Drawing.Size(520, 13);
			this.pbProcessing.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.pbProcessing.TabIndex = 46;
			this.pbProcessing.Visible = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(546, 646);
			this.Controls.Add(this.pbProcessing);
			this.Controls.Add(this.cbAutoRefresh);
			this.Controls.Add(this.txtCustomer);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.clbProducts);
			this.Controls.Add(this.txtPilot);
			this.Controls.Add(this.lblPilot);
			this.Controls.Add(this.txtTargetPests);
			this.Controls.Add(this.txtCreatedOn);
			this.Controls.Add(this.txtUofM);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.txtUofMPerAc);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnExport);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.lblAdditionalNotes);
			this.Controls.Add(this.txtStatus);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.lbStatus);
			this.Controls.Add(this.lblCreatedOn);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.dgFields);
			this.Controls.Add(this.lblProducts);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.lblTargetPests);
			this.Controls.Add(this.lbWorkOrder);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtAdditionalNotes);
			this.Controls.Add(this.lbPilot);
			this.Controls.Add(this.rbPounds);
			this.Controls.Add(this.txtApplicationTotal);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.txtApplicationAmountPerLoad);
			this.Controls.Add(this.txtApplicationLoads);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.rbGallons);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.txtApplicationAcresPerLoad);
			this.Controls.Add(this.txtApplicationRate);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.Text = "Work Order Processing";
			((System.ComponentModel.ISupportInitialize)(this.dgFields)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbPilot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbWorkOrder;
        private System.Windows.Forms.Label lblTargetPests;
        private System.Windows.Forms.Label lblProducts;
        private System.Windows.Forms.Label lblCreatedOn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblAdditionalNotes;
        private System.Windows.Forms.TextBox txtAdditionalNotes;
        private System.Windows.Forms.TextBox txtApplicationTotal;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtApplicationRate;
        private System.Windows.Forms.RadioButton rbGallons;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton rbPounds;
        private System.Windows.Forms.TextBox txtApplicationLoads;
        private System.Windows.Forms.TextBox txtApplicationAcresPerLoad;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtApplicationAmountPerLoad;
        private System.Windows.Forms.DataGridView dgFields;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ListBox lbStatus;
        private System.Windows.Forms.Label txtStatus;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label txtUofMPerAc;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label txtUofM;
        private System.Windows.Forms.Label txtCreatedOn;
        private System.Windows.Forms.Label txtTargetPests;
        private System.Windows.Forms.Label txtPilot;
        private System.Windows.Forms.Label lblPilot;
        private System.Windows.Forms.CheckedListBox clbProducts;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colCompleted;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLatLong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPilot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastUpdated;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label txtCustomer;
		private System.Windows.Forms.CheckBox cbAutoRefresh;
		private System.Windows.Forms.ProgressBar pbProcessing;
    }
}

