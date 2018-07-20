using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal.WorkOrderParsed
{
    public partial class WorkOrderParsed
    {
        #region Load / Save
        public void SaveXML(string Destination)
        {
            if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Destination)))
                try {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Destination));
                } catch { }

			int retryCount = WorkOrder.SaveRetryCount;
			while (retryCount > 0) {
				System.IO.TextWriter writer = null;
				try {
					writer = new System.IO.StreamWriter(Destination);
					System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Aerial.db.dal.WorkOrderParsed.WorkOrderParsed));
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

        public static WorkOrderParsed LoadXMLFromFile(string Source)
        {
            System.IO.TextReader reader = null;
            WorkOrderParsed wop = new WorkOrderParsed();
            try
            {
				reader = new System.IO.StreamReader(Source);
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(WorkOrderParsed));
                wop = (WorkOrderParsed)serializer.Deserialize(reader);
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
        public void AddProduct(string Product)
        {
            Aerial.db.dal.WorkOrderParsed.Product[] productList = this.Products;
            if (productList == null)
                productList = new Aerial.db.dal.WorkOrderParsed.Product[0];
            Array.Resize<Aerial.db.dal.WorkOrderParsed.Product>(ref productList, productList.Length + 1);
            productList[productList.Length - 1] = new Product();
            productList[productList.Length - 1].CustomerSupplied = false;
            productList[productList.Length - 1].Name = Product;
            this.Products = productList;
        }

        public void AddField(string Name, string LatLong, string Area)
        {
            Fields[] fields = this.Fields;
            if (fields == null)
                fields = new Fields[0];
            Array.Resize<Fields>(ref fields, fields.Length + 1);
            fields[fields.Length - 1] = new Fields();
            fields[fields.Length - 1].Name = Name;
            fields[fields.Length - 1].LatLong = LatLong;
            fields[fields.Length - 1].Area = Area;
            this.Fields = fields;
        }
        #endregion Helper Functions
    }

    public partial class Fields
    {
        private bool _complete = false;
        public bool Complete { get { return _complete; } }
        private string _completedBy = "";
        public string CompletedBy { get { return _completedBy; } }
        private DateTime _completeDate = Aerial.db.dal.Constants.INVALID_DATE;
        public DateTime CompletedDate { get { return _completeDate; } }
        public bool SetComplete(bool Complete, DateTime DateRecorded, string CompletedBy) {
            if (DateRecorded > _completeDate)
            {
                _complete = Complete;
                _completeDate = DateRecorded;
                CompletedBy = CompletedBy.ToUpper();
                if (CompletedBy.Length > 1)
                    CompletedBy = string.Format("{0}{1}", CompletedBy[0], CompletedBy.Substring(1).ToLower());
                _completedBy = CompletedBy;
            }
            return _complete;
        }
    }
}