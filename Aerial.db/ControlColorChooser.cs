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
    public partial class ControlColorChooser : Form
    {
        public ControlColorChooser()
        {
            InitializeComponent();
        }

        public Color PickedForeColor
        {
            get { return lblSample.ForeColor; }
        }
        public Color PickedBackColor 
        {
            get { return lblSample.BackColor; }
        }
        
        public void Init(Color ForeColor, Color BackColor)
        {
            lblForeColor.BackColor = lblSample.ForeColor = ForeColor;
            lblBackColor.BackColor = lblSample.BackColor = BackColor;
        }

        private void foreground_Click(object sender, EventArgs e)
        {
            lblForeColor.BackColor = lblSample.ForeColor = ChooseColor(lblForeColor.ForeColor);
        }

        private void background_Click(object sender, EventArgs e)
        {
            lblBackColor.BackColor = lblSample.BackColor = ChooseColor(lblBackColor.ForeColor);
        }


        private Color ChooseColor(Color Color)
        {
            colorDialog1.Color = Color;
            if (colorDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                return colorDialog1.Color;
            return Color;
        }

    }
}
