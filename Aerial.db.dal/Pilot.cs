using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal
{
    public class Pilot
    {
        #region Private Members and variables
        private string _rootPath = "";
		public string RootPath {
			get { return _rootPath; }
		}
        #endregion Private Members and variables

        #region Public Members and variables
        public const string NO_PILOT_SELECTED = "No Pilot - Click To Change";
        #endregion Public Members and variables


        public Pilot(string RootPath)
        {
            _rootPath = RootPath;
            Log.LogWriter = new Logger(_rootPath, _applicatorName);
            //WorkOrders = new WorkOrderList(null, _applicatorName);
        }

        public string[] GetPilotList(string[] SpecificPilots) {
            System.Collections.Generic.List<string> glist = new List<string>();
            try {
                string[] list = System.IO.Directory.GetDirectories(_rootPath);
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = System.IO.Path.GetFileName(list[i]);
					if (!list[i].StartsWith(".")) {
						if (SpecificPilots == null || ViewPilot(SpecificPilots, list[i]))
							glist.Add(list[i]);
					}
                }
                return glist.ToArray();
            }
            catch {
                return new string[] {"Error - Path Not Found"};
            }
        }

		/// <summary>
		/// Check to see if the user wants to view this particular pilot.
		/// </summary>
		/// <param name="SpecificPilots">A list of pilots to view</param>
		/// <param name="Pilot">The pilot we are checking to see if we want to view.</param>
		/// <returns>True if the pilot should be listed, false otherwise.</returns>
		private bool ViewPilot(string[] SpecificPilots, string Pilot) {
			if (SpecificPilots == null || SpecificPilots.Length == 0)
				return true;
			else {
				//We have a list to check
				Pilot = Pilot.ToUpper();
				foreach (string s in SpecificPilots) {
					if (s.ToUpper() == Pilot)
						return true;
				}
				return false;
			}
		}

		public static string[] ViewSpecificPilotListFromString(string PilotList) {
			string[] list = PilotList.ToUpper().Split(new char[] {',', ';'}, StringSplitOptions.RemoveEmptyEntries);
			if (list.Length == 0)
				list = null;
			return list;
		}

        public bool IsValidPilot(string Pilot)
        {
            return Pilot.Trim() != string.Empty && System.IO.Directory.Exists(string.Format("{0}\\{1}", _rootPath, Pilot));
        }

        private string _pilotName = "";
        public void SetPilot(string PilotName)
        {
            if (IsValidPilot(PilotName))
            {
                _pilotName = PilotName;
            }
            else
                _pilotName = NO_PILOT_SELECTED;
            Log.LogWriter.WriteActivity(string.Format("Set pilot to {0}", PilotName));
            //_workOrders = new WorkOrderList(this, _applicatorName);
            _workOrders = null;
        }

        private string _applicatorName = "";
        public void SetApplicator(string Applicator)
        {
            _applicatorName = Applicator;
            Log.LogWriter = new Logger(_rootPath, _applicatorName);
            Log.LogWriter.WriteActivity(string.Format("Set applicator to {0}", Applicator));
        }

        public string PilotName
        {
            get
            {
                return _pilotName;
            }
        }
        public string PilotPath
        {
            get
            {
                if (IsValidPilot(this.PilotName))
                {
                    return string.Format("{0}\\{1}", _rootPath, PilotName);
                }
                return "";
            }
        }
        
        private WorkOrderList _workOrders = null;
        public WorkOrderList WorkOrders
        {
            get
            {
                if (_workOrders == null)
                    _workOrders = new WorkOrderList(this, this._applicatorName);
                return _workOrders;
            }
        }

        public WorkOrderList QuickWorkOrderList
        {
            get
            {
                return new WorkOrderList(this, this._applicatorName, true);
            }
        }
    }
}
