using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal.WorkOrderEntry
{
    public partial class WorkOrderEntry
    {
        #region Load / Save
        public string SourceFile = "";

		public void SaveXML(string Destination) {

			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Destination)))
				try {
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Destination));
				}
				catch { }
			
			int retryCount = WorkOrder.SaveRetryCount;
			while (retryCount > 0) {
				System.IO.TextWriter writer = null;
				try {
					writer = new System.IO.StreamWriter(Destination);
					System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Aerial.db.dal.WorkOrderEntry.WorkOrderEntry));
					serializer.Serialize(writer, this);
					retryCount = -1;
				}
				catch {
					retryCount--;
					if (retryCount > 0)
						System.Threading.Thread.Sleep(WorkOrder.SaveRetryDelay);
				}
				finally {
					if (writer != null)
						writer.Close();
				}
			}
		}


        public void SaveXML()
        {
            SaveXML(SourceFile);
        }

        public static WorkOrderEntry LoadXMLFromFile(string Source)
        {
            System.IO.TextReader reader = null;
            WorkOrderEntry woe = new WorkOrderEntry();
            try
            {
                reader = new System.IO.StreamReader(Source);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(WorkOrderEntry));
                woe = (WorkOrderEntry)serializer.Deserialize(reader);
            }
            catch
            {
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            woe.SourceFile = Source;
            return woe;
        }
        #endregion Load / Save

        #region Helper Functions
        public string PilotName
        {
            get
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(SourceFile).ToUpper();
                if (name.Length > 1)
                    name = string.Format("{0}{1}", name[0], name.Substring(1).ToLower());
                return name;
            }
        }

        static public void VerifyStamp(ref RecordStamp Stamp)
        {
            if (Stamp == null)
            {
                Stamp = new RecordStamp();
                Stamp.DateRecorded = new DateTime(1900, 1, 1);
                Stamp.Pilot = "";
            }
        }
        
        public void AddClockPunch(string Applicator, DateTime ClockPunch)
        {
            ClockPunches[] clockList = this.ClockPunches;
            if (clockList == null)
                clockList = new ClockPunches[0];
            Array.Resize<ClockPunches>(ref clockList, clockList.Length + 1);
            clockList[clockList.Length - 1] = new ClockPunches();
            clockList[clockList.Length - 1].Date = ClockPunch;
            clockList[clockList.Length - 1].Stamp = new RecordStamp();
            clockList[clockList.Length - 1].Stamp.DateRecorded = DateTime.Now;
            clockList[clockList.Length - 1].Stamp.Pilot = Applicator;
            this.ClockPunches = clockList;
        }

        public void AddLoadTime(string Applicator, DateTime Time)
        {
            DateTime[] loadList = this.Loads;
            if (loadList == null)
                loadList = new DateTime[0];
            Array.Resize<DateTime>(ref loadList, loadList.Length + 1);
            loadList[loadList.Length - 1] = Time;
            this.Loads = loadList;
        }

        public void DeleteClockPunch(DateTime ClockPunch)
        {
            ClockPunches[] clockList = this.ClockPunches;
            if (clockList == null)
                clockList = new ClockPunches[0];
            int i = 0;
            for (i = 0; i < clockList.Length; i++)
            {
                if (clockList[i].Date == ClockPunch)
                {
                    clockList = clockList.Where((val, idx) => idx != i).ToArray();
                    break;
                }
            }
            this.ClockPunches = clockList;
        }

        public void SetFieldComplete(string Applicator, string Name, string LatLong, bool Complete,
            int Temperature, string WindDirection, int WindSpeed)
        {
            int i = 0;
            for (i = 0; this.Fields != null && i < this.Fields.Length; i++)
            {
                if (Fields[i].Name.ToUpper() == Name.ToUpper()
                    && Fields[i].LatLong.ToUpper() == LatLong.ToUpper())
                {
                    //Field was found
                    RecordStamp genericStamp = new RecordStamp();
                    genericStamp.DateRecorded = DateTime.Now;
                    genericStamp.Pilot = Applicator;
                    
                    Fields[i].Completed = Complete;
                    Fields[i].Stamp = genericStamp;
                    //if (Fields[i].Stamp == null)
                    //    Fields[i].Stamp = new RecordStamp();
                    //Fields[i].Stamp.DateRecorded = DateTime.Now;
                    //Fields[i].Stamp.Pilot = Applicator;
                    if(Fields[i].Environment == null)
                        Fields[i].Environment = new Environment();
                    Fields[i].Environment.Temperature = Temperature;
                    Fields[i].Environment.WindDirection = WindDirection;
                    Fields[i].Environment.WindSpeed = WindSpeed;
                    Fields[i].Environment.Stamp = genericStamp;
                    
                    break;
                }
            }

            if (this.Fields == null || i >= this.Fields.Length)
            {//Field not found. Add it to the list.
                Fields[] fields = this.Fields;
                if (fields == null)
                    fields = new Fields[0];

                Array.Resize<Fields>(ref fields, fields.Length + 1);
                fields[fields.Length - 1] = new Fields();
                fields[fields.Length - 1].Completed = Complete;
                fields[fields.Length - 1].Name = Name;
                fields[fields.Length - 1].LatLong = LatLong;
                fields[fields.Length - 1].Stamp = new RecordStamp();
                fields[fields.Length - 1].Stamp.DateRecorded = DateTime.Now;
                fields[fields.Length - 1].Stamp.Pilot = Applicator;
                fields[fields.Length - 1].Environment = new Environment();
                fields[fields.Length - 1].Environment.Stamp = fields[fields.Length - 1].Stamp;
                fields[fields.Length - 1].Environment.Temperature = Temperature;
                fields[fields.Length - 1].Environment.WindDirection = WindDirection;
                fields[fields.Length - 1].Environment.WindSpeed = WindSpeed;

                this.Fields = fields;
            }
        }
        #endregion Helper Functions
    }
}