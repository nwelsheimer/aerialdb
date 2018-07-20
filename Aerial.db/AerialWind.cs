using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aerial.db
{
    public partial class AerialWind : UserControl
    {
        #region Event Handling
        public delegate void WindSpeedChangedEvent(object sender, EventArgs e);
        public event WindSpeedChangedEvent WindSpeedChanged;

        public delegate void WindDirectionChangedEvent(object sender, EventArgs e);
        public event WindDirectionChangedEvent WindDirectionChanged;
        #endregion Event Handling

        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        private float _innerCirclePercent = 0.9f;
        public enum CompassDirection { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

        private CompassDirection _windDirection = CompassDirection.North;
        public CompassDirection WindDirection
        {
            get { return _windDirection; }
            set
            {
                _windDirection = value;
                this.Invalidate(); //Force Redraw
            }
        }

        public void SetWindDirection(string Direction)
        {
            Direction = Direction.ToUpper();
            switch (Direction)
            {
                case "NORTH": 
                case "N" :
                    WindDirection = CompassDirection.North;
                    break;
                case "NORTHEAST":
                case "NE":
                    WindDirection = CompassDirection.NorthEast;
                    break;
                case "EAST":
                case "E":
                    WindDirection = CompassDirection.East;
                    break;
                case "SOUTHEAST":
                case "SE":
                    WindDirection = CompassDirection.SouthEast;
                    break;
                case "SOUTH":
                case "S":
                    WindDirection = CompassDirection.South;
                    break;
                case "SOUTHWEST":
                case "SW":
                    WindDirection = CompassDirection.SouthWest;
                    break;
                case "WEST":
                case "W":
                    WindDirection = CompassDirection.West;
                    break;
                case "NORTHWEST":
                case "NW":
                    WindDirection = CompassDirection.NorthWest;
                    break;
                default :
                    break;
            }
        }

        public int WindSpeed
        {
            get
            {
                try
                {
                    return System.Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                textBox1.Text = value.ToString();
            }
        }

        public AerialWind()
        {
            InitializeComponent();
            this.SetStyle(
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer |
                System.Windows.Forms.ControlStyles.ResizeRedraw |
                System.Windows.Forms.ControlStyles.UserPaint,
                true);

        }

        public Color CompassColor = Color.Red;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            e.Graphics.FillEllipse(new SolidBrush(this.ForeColor), 0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);
            
            e.Graphics.FillEllipse(new SolidBrush(this.BackColor),
                e.ClipRectangle.Width * (1-_innerCirclePercent),
                e.ClipRectangle.Height * (1-_innerCirclePercent),
                e.ClipRectangle.Width - (e.ClipRectangle.Width * (1 - _innerCirclePercent) * 2),
                e.ClipRectangle.Height - (e.ClipRectangle.Height * (1 - _innerCirclePercent) * 2));

            switch (_windDirection)
            {
                case CompassDirection.North:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), NorthBox);
                    break;
                case CompassDirection.NorthEast:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), NorthEastBox);
                    break;
                case CompassDirection.East:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), EastBox);
                    break;
                case CompassDirection.SouthEast:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), SouthEastBox);
                    break;
                case CompassDirection.South:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), SouthBox);
                    break;
                case CompassDirection.SouthWest:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), SouthWestBox);
                    break;
                case CompassDirection.West:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), WestBox);
                    break;
                case CompassDirection.NorthWest:
                    e.Graphics.FillEllipse(new SolidBrush(CompassColor), NorthWestBox);
                    break;
                default: break;
            }

            //TODO: Handle a size other than 175x175
            //Top Arrow
            e.Graphics.DrawLine(new Pen(this.ForeColor, 5), new Point(60, 65), new Point(88, 45));
            e.Graphics.DrawLine(new Pen(this.ForeColor, 5), new Point(114, 65), new Point(86, 45));

            //Bottom Arrow
            e.Graphics.DrawLine(new Pen(this.ForeColor, 5), new Point(60, 109), new Point(88, 129));
            e.Graphics.DrawLine(new Pen(this.ForeColor, 5), new Point(114, 109), new Point(86, 129));
        }

        #region Direction Draw and Click Boxes
        public Rectangle NorthBox
        {
            get {
                return new Rectangle(MidPoint.X - DirectionCircleSize, 0, DirectionCircleSize * 2, DirectionCircleSize * 2); 
            }
        }

        public Rectangle NorthEastBox
        {
            get
            {
                return new Rectangle((int)(MidPoint.X + OuterCircleRadius * Math.Cos(45 * Math.PI / 180) + 5) - DirectionCircleSize * 2, (int)(MidPoint.Y - OuterCircleRadius * Math.Sin(45 * Math.PI / 180) - 5), DirectionCircleSize * 2, DirectionCircleSize * 2);
            }
        }

        public Rectangle EastBox
        {
            get
            {
                //Normalized Rectangle
                return new Rectangle(this.ClientRectangle.Width - DirectionCircleSize * 2, MidPoint.Y - DirectionCircleSize, DirectionCircleSize * 2, DirectionCircleSize * 2);
                //return new Rectangle(this.ClientRectangle.Width, MidPoint.Y - DirectionCircleSize, DirectionCircleSize * -2, DirectionCircleSize * 2);
            }
        }

        public Rectangle SouthEastBox
        {
            get
            {
                return new Rectangle((int)(MidPoint.X + OuterCircleRadius * Math.Cos(-45 * Math.PI / 180) + 5 - DirectionCircleSize * 2), (int)(MidPoint.Y - OuterCircleRadius * Math.Sin(-45 * Math.PI / 180) + 5 - DirectionCircleSize * 2), DirectionCircleSize * 2, DirectionCircleSize * 2);
            }
        }

        public Rectangle SouthBox
        {
            get { return new Rectangle(MidPoint.X - DirectionCircleSize, this.ClientRectangle.Height - DirectionCircleSize * 2, DirectionCircleSize * 2, DirectionCircleSize * 2); }
        }

        public Rectangle SouthWestBox {
            get {
                return new Rectangle((int)(MidPoint.X + OuterCircleRadius * Math.Cos(-135 * Math.PI / 180) - 5), (int)(MidPoint.Y - OuterCircleRadius * Math.Sin(-135 * Math.PI / 180) + 5 - DirectionCircleSize * 2), DirectionCircleSize * 2, DirectionCircleSize * 2);
            }
        }

        public Rectangle WestBox
        {
            get
            {
                return new Rectangle(0, MidPoint.Y + DirectionCircleSize - DirectionCircleSize * 2, DirectionCircleSize * 2, DirectionCircleSize * 2);
            }
        }

        public Rectangle NorthWestBox
        {
            get
            {
                return new Rectangle((int)(MidPoint.X + OuterCircleRadius * Math.Cos(135 * Math.PI / 180) - 5), (int)(MidPoint.Y - OuterCircleRadius * Math.Sin(135 * Math.PI / 180) - 5), DirectionCircleSize * 2, DirectionCircleSize * 2);
            }
        }

        public int DirectionCircleSize
        {
            get
            {
                return (int)(this.ClientSize.Width * (1 - _innerCirclePercent));
            }
        }

        public Point MidPoint
        {
            get
            {
                return new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2); //Integer division
            }
        }

        public int OuterCircleRadius
        {
            get
            {
                return this.ClientSize.Width / 2;
            }
        }
        #endregion Direction Draw and Click Boxes

        private void AerialWind_MouseUp(object sender, MouseEventArgs e)
        {
            CompassDirection OriginalWindDirection = this.WindDirection;
            if (NorthBox.Contains(e.Location))
                this.WindDirection = CompassDirection.North;
            else if (NorthEastBox.Contains(e.Location))
                this.WindDirection = CompassDirection.NorthEast;
            else if (EastBox.Contains(e.Location))
                this.WindDirection = CompassDirection.East;
            else if (SouthEastBox.Contains(e.Location))
                this.WindDirection = CompassDirection.SouthEast;
            else if (SouthBox.Contains(e.Location))
                this.WindDirection = CompassDirection.South;
            else if (SouthWestBox.Contains(e.Location))
                this.WindDirection = CompassDirection.SouthWest;
            else if (WestBox.Contains(e.Location))
                this.WindDirection = CompassDirection.West;
            else if (NorthWestBox.Contains(e.Location))
                this.WindDirection = CompassDirection.NorthWest;
            else if (new Rectangle(60, 45, 54, 20).Contains(e.Location)) //Top Arrow
            {
                try
                {
                    textBox1.Text = (System.Convert.ToInt32(textBox1.Text) + 1).ToString();
                }
                catch
                {
                    textBox1.Text = "5";
                }
            }
            else if (new Rectangle(60, 109, 54, 20).Contains(e.Location)) //Bottom Arrow
            {
                try
                {
                    int val = (System.Convert.ToInt32(textBox1.Text) - 1);
                    if (val < 0)
                        val = 0;
                    textBox1.Text = val.ToString();
                }
                catch
                {
                    textBox1.Text = "5";
                }
            }
            if (OriginalWindDirection != this.WindDirection)
                FireDirectionChangedEvent();
        }

        private void AerialWind_EnabledChanged(object sender, EventArgs e)
        {
            this.Visible = this.Enabled;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (WindSpeedChanged != null)
                WindSpeedChanged(this, e);
        }

        private void FireDirectionChangedEvent()
        {
            if (WindDirectionChanged != null)
                WindDirectionChanged(this, new EventArgs());
        }
    }
}
