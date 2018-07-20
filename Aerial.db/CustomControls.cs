using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Aerial.db
{
    public class AerialListBox : System.Windows.Forms.ListBox
    {
        public AerialListBox()
        {
            InitializeComponent();
            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        }

        private System.Drawing.Color _borderColor = System.Drawing.SystemColors.Control;
        public System.Drawing.Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                InvokePaint(this, new System.Windows.Forms.PaintEventArgs(this.CreateGraphics(), this.Bounds));
            }
        }

        private void InitializeComponent()
        {
            this.SetStyle(
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
                System.Windows.Forms.ControlStyles.ResizeRedraw |
                System.Windows.Forms.ControlStyles.UserPaint,
                true);
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
        }

        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                bool strikeOut = false;
                if (this.Items[e.Index].GetType() == typeof(dal.WorkOrder))
                    strikeOut = ((dal.WorkOrder)this.Items[e.Index]).Complete;
                string text = this.Items[e.Index].ToString();
                bool complete = text.EndsWith("--") || strikeOut;
                if (complete)
                    text = text.Trim('-');

                Color fore = Color.Black; //Will be overridden
                Color back = Color.Black; //Will be overridden
                if (System.Convert.ToBoolean(e.State & System.Windows.Forms.DrawItemState.Selected))
                {//Item selected?
                    fore = Aerial.db.Properties.Settings.Default.SelectedForeColor;
                    back = Aerial.db.Properties.Settings.Default.SelectedBackColor;
                }
                else
                {
                    if (complete)
                        fore = Aerial.db.Properties.Settings.Default.FinishedForeColor;
                    else
                        fore = Aerial.db.Properties.Settings.Default.ControlForeColor;

                    back = Aerial.db.Properties.Settings.Default.ControlBackColor;
                }

                //Draw Background
                e.Graphics.DrawRectangle(new Pen(back), e.Bounds);
                e.Graphics.FillRectangle(new SolidBrush(back), e.Bounds);

                //Draw Text

                Brush brush = new SolidBrush(fore);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;
                Font f = e.Font;
                if (complete)
                    f = new Font(e.Font, FontStyle.Strikeout);
                e.Graphics.DrawString(text,
                    f, brush, e.Bounds, sf);
            }
            System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, this.BorderColor, System.Windows.Forms.ButtonBorderStyle.Solid);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Region iRegion = new System.Drawing.Region(e.ClipRectangle);
            e.Graphics.FillRegion(new System.Drawing.SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((this.SelectionMode == System.Windows.Forms.SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == System.Windows.Forms.SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == System.Windows.Forms.SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new System.Windows.Forms.DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                System.Windows.Forms.DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new System.Windows.Forms.DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                System.Windows.Forms.DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, base.ClientRectangle, this.BorderColor, System.Windows.Forms.ButtonBorderStyle.Solid);
            base.OnPaint(e);
        }
    }
    public class AerialButton : System.Windows.Forms.Button
    {
        public AerialButton()
        {
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        }
    }
}