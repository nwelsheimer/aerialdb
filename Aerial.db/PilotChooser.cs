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
    public partial class PilotChooser : Form
    {
        public PilotChooser()
        {
            InitializeComponent();
        }

        public void Init(string[] PilotList, string SelectedPilot)
        {
            lbPilots.Items.Clear();
            lbPilots.Items.AddRange(PilotList);
            if(lbPilots.Items.IndexOf(SelectedPilot) >= 0)
                lbPilots.SetSelected(lbPilots.Items.IndexOf(SelectedPilot), true);
        }
        
        public string SelectedPilot {
            get
            {
                if (lbPilots.SelectedIndex >= 0)
                    return lbPilots.SelectedItem.ToString();
                return "";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}