using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aerial.db {
	public partial class AerialTextBox : UserControl {

		#region Event Handling
		public delegate void TextChangedEvent(object sender, EventArgs e);
		public event TextChangedEvent TextValueChanged;
		#endregion Event Handling

		private int _padding = 1;
		public AerialTextBox() {
			InitializeComponent();
			textBox1.Location = new Point(_padding, _padding);
			textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}

		private System.Drawing.Color _borderColor = System.Drawing.SystemColors.Control;
		public System.Drawing.Color BorderColor {
			get { return _borderColor; }
			set {
				_borderColor = value;
				panel1.BackColor = _borderColor;
			}
		}

		override public string Text {
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}

		public bool Multiline {
			get { return textBox1.Multiline; }
			set { textBox1.Multiline = value; }
		}

		public bool ReadOnly {
			get { return textBox1.ReadOnly; }
			set { textBox1.ReadOnly = value; }
		}

		public void SelectAll() {
			textBox1.SelectAll();
		}

		public HorizontalAlignment TextAlign {
			get { return textBox1.TextAlign; }
			set { textBox1.TextAlign = value; }
		}

		private void panel1_Resize(object sender, EventArgs e) {
			textBox1.Width = panel1.Width - _padding * 4;
			textBox1.Height = panel1.Height - _padding * 4;
		}

		private void textBox1_TextChanged(object sender, EventArgs e) {
			if (TextValueChanged != null)
				TextValueChanged(this, e);
		}

		private void textBox1_FontChanged(object sender, EventArgs e) {
			textBox1.Font = this.Font;
		}
	}
}