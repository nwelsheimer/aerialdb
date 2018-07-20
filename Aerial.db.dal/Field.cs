using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal
{
    public class Field
    {
        private string _name = "";
        private string _latLong = "";
        private string _area = "";
        private bool _complete = false;
        private DateTime _completeDate = Aerial.db.dal.Constants.INVALID_DATE;

        public string Name { get { return _name; } }
        public string LatLong { get { return _latLong; } }
        public string Area { get { return _area; } }
        public bool Complete { get { return _complete; } }

        public Field(string Name, string LatLong, string Area)
        {
            Init(Name, LatLong, Area, false, Aerial.db.dal.Constants.INVALID_DATE);
        }

        public Field(string Name, string LatLong, decimal Area)
        {
            Init(Name, LatLong, Area.ToString(), false, Aerial.db.dal.Constants.INVALID_DATE);
        }

        public Field(string Name, string LatLong, string Area, bool Complete, DateTime DateRecorded)
        {
            Init(Name, LatLong, Area, Complete, DateRecorded);
        }

        public Field(string Name, string LatLong, decimal Area, bool Complete, DateTime DateRecorded)
        {
            Init(Name, LatLong, Area.ToString(), Complete, DateRecorded);
        }

        private void Init(string Name, string LatLong, string Area, bool Complete, DateTime DateRecorded)
        {
            _name = Name;
            _latLong = LatLong;
            _area = Area;
            _complete = Complete;
            _completeDate = DateRecorded;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", _name, _latLong, _area);
        }

    }
}
