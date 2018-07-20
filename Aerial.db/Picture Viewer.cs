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
    public partial class Picture_Viewer : Form
    {
        private double pictureMultiplier = 1.0;

        private int __imagePosition = 0;
        private int _imagePosition {
            get { return __imagePosition; }
            set
            {
                if (value < _imageList.Length && value >= 0)
                {
                    __imagePosition = value;
                    this.pictureBox1.Image = _imageList[value];
                    pictureMultiplier = 1.0;
                    AdjustPictureSize(0);
                }
                btnPrevious.Enabled = __imagePosition > 0;
                btnNext.Enabled = (__imagePosition < _imageList.Length - 1);

            }
        }
        private Image[] __imageList = null;
        private Image[] _imageList
        {
            get { return __imageList; }
            set
            {
                __imageList = value;
                _imagePosition = 0;
                btnNext.Enabled = btnPrevious.Enabled = __imageList.Length > 1;
            }
        }
        
        
        public Picture_Viewer()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        public void Init(System.Drawing.Image Image)
        {
            _imageList = new Image[1];
            _imageList[0] = Image;
            _imagePosition = 0;
            //_imageList[0] = Image;
            //this.pictureBox1.Image = _imageList[0];
            //AdjustPictureSize(0);
        }

        public void Init(System.Drawing.Image[] Images)
        {
            _imageList = Images;
            _imagePosition = 0;
        }

        private void AdjustPictureSize(double Step)
        {
            pictureMultiplier += Step;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Width = (int)(pictureBox1.Image.Width * pictureMultiplier);
                pictureBox1.Height = (int)(pictureBox1.Image.Height * pictureMultiplier);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            AdjustPictureSize(0.5);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            AdjustPictureSize(-0.5);
        }

        private Point _panStartPoint = new Point();
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _panStartPoint = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int x = _panStartPoint.X - e.X;
                int y = _panStartPoint.Y - e.Y;
                panel1.AutoScrollPosition = new Point(x - panel1.AutoScrollPosition.X, y - panel1.AutoScrollPosition.Y);
            }
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AdjustPictureSize(0.5);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            _imagePosition++;
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            _imagePosition--;
        }
    }
}
