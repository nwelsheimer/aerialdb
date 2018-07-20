using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal {
	public class WorkOrderList : IEnumerable<WorkOrder> {
		private Pilot _pilot = null;
		private string _applicator = "";
		private bool _quickList = false;
		public WorkOrderList(Pilot Pilot, string Applicator) {
			_pilot = Pilot;
			_applicator = Applicator;
		}

		public WorkOrderList(Pilot Pilot, string Applicator, bool QuickList) {
			_pilot = Pilot;
			_applicator = Applicator;
			_quickList = QuickList;
		}

		private System.Collections.Generic.List<WorkOrder> __workOrderList = null;
		private System.Collections.Generic.List<WorkOrder> _workOrderList {
			get {
				if (__workOrderList == null)
					RefreshWorkOrders();
				return __workOrderList;
			}
		}

		#region IEnumerable Implementation
		public IEnumerator<WorkOrder> GetEnumerator() {
			return _workOrderList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _workOrderList.GetEnumerator();
		}
		#endregion IEnumerable Implementation

		#region Implementation
		public WorkOrder this[int index] {
			get {
				try { //Try/catch to fix: Message: System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
					return _workOrderList[index];
				}
				catch { return null; }
			}
			set {
				_workOrderList[index] = value;
			}
		}

		public int Count {
			get { return _workOrderList.Count; }
		}

		private void RefreshWorkOrders() {
			__workOrderList = new List<WorkOrder>();
			List<string> files = new List<string>(System.IO.Directory.GetFiles(_pilot.PilotPath, "*.rtf"));
			files.AddRange(System.IO.Directory.GetFiles(_pilot.PilotPath, "*.pdf")); //2015 Added support for PDFs
			foreach (string s in files) {
				if (System.IO.File.Exists(s) &&
					(System.IO.File.GetAttributes(s) == System.IO.FileAttributes.Normal || System.IO.File.GetAttributes(s) == System.IO.FileAttributes.Archive)) {

					WorkOrder wo = new WorkOrder(s, _applicator, _quickList);
					if (wo.IsValid())
						__workOrderList.Add(wo);
				}
			}
		}
		#endregion Implementation
	}
}