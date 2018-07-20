using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aerial.db.dal {
	public class FileSupport {
		static public string MakeValidFileName(string FileName) {
			string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
			string invalidReStr = string.Format("[{0}]", invalidChars);
			return System.Text.RegularExpressions.Regex.Replace(FileName, invalidReStr, " ");
		}
		
		static public string MakeValidDirectoryName(string DirectoryName) {
			string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidPathChars()));
			string invalidReStr = string.Format("[{0}]", invalidChars);
			return System.Text.RegularExpressions.Regex.Replace(DirectoryName, invalidReStr, " ");
		}
	}
}
