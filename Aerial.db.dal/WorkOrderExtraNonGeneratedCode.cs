using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal.WorkOrderExtra
{
    public partial class WorkOrderExtra
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
					System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Aerial.db.dal.WorkOrderExtra.WorkOrderExtra));
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

        public static WorkOrderExtra LoadXMLFromFile(string Source)
        {
            System.IO.TextReader reader = null;
            WorkOrderExtra woe = new WorkOrderExtra();
            try
            {
                reader = new System.IO.StreamReader(Source);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(WorkOrderExtra));
                woe = (WorkOrderExtra)serializer.Deserialize(reader);
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
        //static public void VerifyStamp(ref RecordStamp Stamp)
        //{
        //    if (Stamp == null)
        //    {
        //        Stamp = new RecordStamp();
        //        Stamp.DateRecorded = new DateTime(1900, 1, 1);
        //        Stamp.Pilot = "";
        //    }
        //}
        
        //public void AddClockPunch(string Applicator, DateTime ClockPunch)
        //{
        //    ClockPunches[] clockList = this.ClockPunches;
        //    if (clockList == null)
        //        clockList = new ClockPunches[0];
        //    Array.Resize<ClockPunches>(ref clockList, clockList.Length + 1);
        //    clockList[clockList.Length - 1] = new ClockPunches();
        //    clockList[clockList.Length - 1].Date = ClockPunch;
        //    clockList[clockList.Length - 1].Stamp = new RecordStamp();
        //    clockList[clockList.Length - 1].Stamp.DateRecorded = DateTime.Now;
        //    clockList[clockList.Length - 1].Stamp.Pilot = Applicator;
        //    this.ClockPunches = clockList;
        //}

        //public void AddLoadTime(string Applicator, DateTime Time)
        //{
        //    DateTime[] loadList = this.Loads;
        //    if (loadList == null)
        //        loadList = new DateTime[0];
        //    Array.Resize<DateTime>(ref loadList, loadList.Length + 1);
        //    loadList[loadList.Length - 1] = Time;
        //    this.Loads = loadList;
        //}

        //public void DeleteClockPunch(DateTime ClockPunch)
        //{
        //    ClockPunches[] clockList = this.ClockPunches;
        //    if (clockList == null)
        //        clockList = new ClockPunches[0];
        //    int i = 0;
        //    for (i = 0; i < clockList.Length; i++)
        //    {
        //        if (clockList[i].Date == ClockPunch)
        //        {
        //            clockList = clockList.Where((val, idx) => idx != i).ToArray();
        //            break;
        //        }
        //    }
        //    this.ClockPunches = clockList;
        //}

        public void SetProductState(string Product, bool CustomerSupplied)
        {
            int i = 0;
            for (i = 0; this.Products != null && i < this.Products.Length; i++)
            {
                if (Products[i].Name.ToUpper() == Product.ToUpper())
                {
                    Products[i].CustomerSupplied = CustomerSupplied;
                    break;
                }
            }
            if (this.Products == null || i >= this.Products.Length)
            {//Product not found. Add it to the list
                Product[] products = this.Products;
                if (products == null)
                    products = new Product[0];
                Array.Resize<Product>(ref products, products.Length + 1);
                products[products.Length - 1] = new Product();
                products[products.Length - 1].Name = Product;
                products[products.Length - 1].CustomerSupplied = CustomerSupplied;

                this.Products = products;
            }
        }
        #endregion Helper Functions
    }
}
