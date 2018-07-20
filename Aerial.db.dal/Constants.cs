using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal
{
    public class Constants
    {
        public static DateTime INVALID_DATE = new DateTime(1900, 1, 1);
		public static string DALVERSION = "09";
		public static string DALONLYVERSION {
			get { return string.Format("Aerial.db.dal.{0}", DALVERSION); }
		}
    }
}
