using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aerial.db
{
    public partial class CustomizeColors : Form
    {
        public CustomizeColors()
        {
            InitializeComponent();

            //Set Colors
            lblBorderColor.BackColor = Aerial.db.Properties.Settings.Default.BorderColor;
            lblBackground.BackColor = Aerial.db.Properties.Settings.Default.ControlBackColor;
            lblForegroundColor.BackColor = Aerial.db.Properties.Settings.Default.ControlForeColor;

            lblSelectedItemForegroundColor.BackColor = Aerial.db.Properties.Settings.Default.SelectedForeColor;
            lblSelectedItemColor.BackColor = Aerial.db.Properties.Settings.Default.SelectedBackColor;

            lblButtonForegroundColor.BackColor = Aerial.db.Properties.Settings.Default.ButtonForeColor;
            lblButtonBackgroundColor.BackColor = Aerial.db.Properties.Settings.Default.ButtonBackColor;

            lblFinishedItemColor.BackColor = Aerial.db.Properties.Settings.Default.FinishedForeColor;
        }

        private void lblFinishedItemColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblFinishedItemColor);
        }

        private void lblSelectedItemForegroundColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblSelectedItemForegroundColor);
        }

        private void lblBorderColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblBorderColor);
        }

        private void lblSelectedItemColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblSelectedItemColor);
        }

        private void lblForegroundColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblForegroundColor);
        }

        private void lblBackground_Click(object sender, EventArgs e)
        {
            ChooseColor(lblBackground);
        }

        private void lblButtonForegroundColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblButtonForegroundColor);
        }

        private void lblButtonBackgroundColor_Click(object sender, EventArgs e)
        {
            ChooseColor(lblButtonBackgroundColor);
        }


        private void ChooseColor(Control Control)
        {
            colorDialog1.Color = Control.BackColor;
            if (colorDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                Control.BackColor = colorDialog1.Color;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //Save Colors
            Aerial.db.Properties.Settings.Default.BorderColor = lblBorderColor.BackColor;
            Aerial.db.Properties.Settings.Default.ControlBackColor = lblBackground.BackColor;
            Aerial.db.Properties.Settings.Default.ControlForeColor = lblForegroundColor.BackColor;
            Aerial.db.Properties.Settings.Default.SelectedForeColor = lblSelectedItemForegroundColor.BackColor;
            Aerial.db.Properties.Settings.Default.SelectedBackColor = lblSelectedItemColor.BackColor;
            Aerial.db.Properties.Settings.Default.ButtonForeColor = lblButtonForegroundColor.BackColor;
            Aerial.db.Properties.Settings.Default.ButtonBackColor = lblButtonBackgroundColor.BackColor;
            Aerial.db.Properties.Settings.Default.FinishedForeColor = lblFinishedItemColor.BackColor;

            Aerial.db.Properties.Settings.Default.Save();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void CustomizeColors_Resize(object sender, EventArgs e)
        {
            panel1.Left = (this.ClientSize.Width - panel1.Width) / 2;
            panel1.Top = (this.ClientSize.Height - panel1.Height) / 2;
        }
    }
}
