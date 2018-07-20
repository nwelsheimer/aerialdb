using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.ControlColors
{
    public partial class ControlColors
    {
        #region Load / Save
        public string XmlAsString()
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ControlColors));
                serializer.Serialize(writer, this);
            }
            catch { }
            return writer.ToString();
        }

        public void SaveXML(string Destination)
        {
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Destination)))
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Destination));
                }
                catch { }
            System.IO.TextWriter writer = null;
            try
            {
                writer = new System.IO.StreamWriter(Destination);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ControlColors));
                serializer.Serialize(writer, this);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static ControlColors LoadXMLFromString(string String)
        {
            System.IO.StringReader reader = new System.IO.StringReader(String);
            ControlColors wop = new ControlColors();
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ControlColors));
                wop = (ControlColors)serializer.Deserialize(reader);
            }
            catch {
            }
            finally
            {
            }
            return wop;
        }

        public static ControlColors LoadXMLFromFile(string Source)
        {
            System.IO.TextReader reader = null;
            ControlColors wop = new ControlColors();
            try
            {
                reader = new System.IO.StreamReader(Source);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ControlColors));
                wop = (ControlColors)serializer.Deserialize(reader);
            }
            catch { }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return wop;
        }
        #endregion Load / Save

        #region Helper Functions
        public void SetControlColor(string ID, System.Drawing.Color ForeColor, System.Drawing.Color BackColor)
        {
            if (this.Control != null)
            {
                for (int i = 0; i < this.Control.Length; i++)
                {
                    if (this.Control[i].ID == ID)
                    {
                        this.Control[i].ForeR = ForeColor.R;
                        this.Control[i].ForeG = ForeColor.G;
                        this.Control[i].ForeB = ForeColor.B;
                        this.Control[i].BackR = BackColor.R;
                        this.Control[i].BackG = BackColor.G;
                        this.Control[i].BackB = BackColor.B;
                        return;
                    }
                }
            }

            Control[] controls = this.Control;
            if (controls == null)
                controls = new Control[0];
            Array.Resize<Control>(ref controls, controls.Length + 1);
            controls[controls.Length - 1] = new Control();
            controls[controls.Length - 1].ID = ID;
            controls[controls.Length - 1].ForeR = ForeColor.R;
            controls[controls.Length - 1].ForeG = ForeColor.G;
            controls[controls.Length - 1].ForeB = ForeColor.B;
            controls[controls.Length - 1].BackR = BackColor.R;
            controls[controls.Length - 1].BackG = BackColor.G;
            controls[controls.Length - 1].BackB = BackColor.B;

            this.Control = controls;
        }
        #endregion Helper Functions
    }

}
