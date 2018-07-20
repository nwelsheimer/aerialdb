using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aerial.db.WOP {
	public partial class Form1 : Form {
		#region Private members
		private Aerial.db.dal.Pilot _dal = new Aerial.db.dal.Pilot(Aerial.db.WOP.Properties.Settings.Default.WorkingFolder);
		System.Collections.Generic.List<Aerial.db.dal.WorkOrder> _workOrderList = new List<dal.WorkOrder>();
		enum StatusState { All, Open, WIP, Complete }
		bool __isDirty = false;
		bool _isDirty {
			get { return __isDirty; }
			set { __isDirty = btnSave.Enabled = btnCancel.Enabled = value; }
		}
		dal.WorkOrder __currentWorkOrder = null;
		dal.WorkOrder _currentWorkOrder {
			get { return __currentWorkOrder; }
			set {
				__currentWorkOrder = value;
				btnExport.Enabled = __currentWorkOrder != null && __currentWorkOrder.IsValid();
			}
		}

		System.Threading.Timer _timer = null;
		private const int _timerCallbackDelayInMs = 15000;
		private delegate void TimerEventFiredDelegate(System.Collections.Generic.List<Aerial.db.dal.WorkOrder> List, bool ForceRefresh);
		private delegate void ProgressBarFiredDelegate(bool Visiable);

		private enum UpdateStatus { Yes, No, Unknown }
		#endregion Private members

		#region Public members
		public static string VERSION  {
			get { return string.Format("1.04.{0}", Aerial.db.dal.Constants.DALVERSION); }
		}

		#endregion Public members

		#region Initialization
		public Form1() {
			InitializeComponent();
			foreach (string State in Enum.GetNames(typeof(StatusState)))
				lbStatus.Items.Add(State.Replace('_', ' '));
			lbStatus.SelectedIndex = 0;
			//RefreshWorkOrders();

			cbAutoRefresh.Checked = Aerial.db.WOP.Properties.Settings.Default.AutoRefresh;
			btnRefresh.Enabled = !Aerial.db.WOP.Properties.Settings.Default.AutoRefresh;

			lblVersion.Text = string.Format("Version: {0}", VERSION);

			//Create the refresh thread timer
			//_timer = new System.Threading.Timer(new System.Threading.TimerCallback(tmrThreadingTimer_TimerCallback), null, _timerCallbackDelayInMs, _timerCallbackDelayInMs);
			_timer = new System.Threading.Timer(new System.Threading.TimerCallback(tmrThreadingTimer_TimerCallback), null, 10, 10); //Fire the timer right away...
		}
		#endregion Initialization

		#region Logic
		private System.Collections.Generic.List<Aerial.db.dal.WorkOrder> GetWorkOrderList(bool QuickList) {
			System.Collections.Generic.List<Aerial.db.dal.WorkOrder> wol = new List<dal.WorkOrder>();

			if (lbPilot.InvokeRequired) {
				lbPilot.Invoke((MethodInvoker)delegate {
					if (!lbPilot.Items.Contains("All"))
						lbPilot.Items.Add("All");
				});

			}
			else {
				if (!lbPilot.Items.Contains("All"))
					lbPilot.Items.Add("All");
			}

			foreach (string pilot in _dal.GetPilotList(Aerial.db.dal.Pilot.ViewSpecificPilotListFromString(Aerial.db.WOP.Properties.Settings.Default.ViewSpecificPilots))) {
				_dal.SetPilot(pilot);
				//Old Code to be replaced with a broken out if statement. This is for error handling.
				//foreach (Aerial.db.dal.WorkOrder wo in (QuickList) ? _dal.QuickWorkOrderList : _dal.WorkOrders)
				//	wol.Add(wo);
				if (lbPilot.InvokeRequired) {
					lbPilot.Invoke((MethodInvoker)delegate {
						if (!lbPilot.Items.Contains(pilot))
							lbPilot.Items.Add(pilot);
					});
				}
				else {
					if (!lbPilot.Items.Contains(pilot))
						lbPilot.Items.Add(pilot);
				}
				if (QuickList) {
					for (int i = 0; i < _dal.QuickWorkOrderList.Count; i++) {
						try {
							wol.Add(_dal.QuickWorkOrderList[i]);
						}
						catch (Exception ex) {
							Aerial.db.dal.Email.SendEmail(ex.ToString(), VERSION);
							MessageBox.Show(string.Format("Unable to process file {0}. Please try again or recreate the work order.", _dal.QuickWorkOrderList[i].OriginalFilePath), "Unable To Process", MessageBoxButtons.OK);
						}
					}
				}
				else {
					try {
						//Validates we can access the work orders.
						if (_dal.WorkOrders.Count == 0)
							;
					}
					catch(Exception ex) {
						MessageBox.Show(string.Format("Unable to open the Working Folder path. Please confirm that \"{0}\" exists and is accessible.", _dal.PilotPath), "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Error);
						throw new Exception(string.Format("Unable to open the working folder path. Additional details:\r\n{0}", ex.ToString()));
					}
					for (int i = 0; i < _dal.WorkOrders.Count; i++) {
						try {
							dal.WorkOrder wo = _dal.WorkOrders[i];
							if (wo != null)
								wol.Add(wo);
						}
						catch (Exception ex) {
							Aerial.db.dal.Email.SendEmail(ex.ToString(), VERSION);
							MessageBox.Show(string.Format("Unable to process file {0}. Please try again or recreate the work order.", _dal.WorkOrders[i].OriginalFilePath), "Unable To Process", MessageBoxButtons.OK);
						}
					}
				}
			}
			return wol;
		}

		public void RefreshWorkOrders() {
			if (this.InvokeRequired) {
				this.Invoke((MethodInvoker)delegate {
					//RefreshWorkOrdersInvoked();
				});
			}
			else
				RefreshWorkOrdersInvoked();

		}

		public void RefreshWorkOrdersInvoked() {
			SaveBeforeContinue();
			if (!Aerial.db.WOP.Properties.Settings.Default.AutoRefresh) {
				System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(RefreshWorkOrdersForThreads));
				t.Start();
			}
			else {
				//Get Pilot List

				//Grab snapshot of item before working on it...Due to threading...
				object o = null;

				o = lbPilot.SelectedItem;
				string item2 = o == null ? "" : o.ToString();

				o = lbWorkOrder.SelectedItem;
				//lbWorkOrder.Items.Clear();

				string item3 = o == null ? "" : o.ToString();

				_workOrderList.Clear();
				//lbPilot.Items.Clear();
				//foreach (string pilot in _dal.GetPilotList(Aerial.db.dal.Pilot.ViewSpecificPilotListFromString(Aerial.db.WOP.Properties.Settings.Default.ViewSpecificPilots)))
				//    if (!lbPilot.Items.Contains(pilot))
				//        lbPilot.Items.Add(pilot);
				//if (!lbPilot.Items.Contains("All"))
				//lbPilot.Items.Add("All");

				_workOrderList = GetWorkOrderList(false);

				btnRefresh.Enabled = !Aerial.db.WOP.Properties.Settings.Default.AutoRefresh;

				ApplyWorkOrderListFilter();


				//Restore the previous values
				int i = 0;
				for (i = 0; i < lbPilot.Items.Count; i++) {
					if (item2 == lbPilot.Items[i].ToString()) {
						lbPilot.SelectedItem = lbPilot.Items[i];
						break;
					}
				}
				if (i >= lbPilot.Items.Count && lbPilot.Items.Count > 0)
					lbPilot.SelectedIndex = 0;

				for (i = 0; i < lbWorkOrder.Items.Count; i++) {
					if (item3 == lbWorkOrder.Items[i].ToString()) {
						lbWorkOrder.SelectedItem = lbWorkOrder.Items[i];
						break;
					}
				}
				if (i >= lbWorkOrder.Items.Count && lbWorkOrder.Items.Count > 0)
					lbWorkOrder.SelectedIndex = 0;
			}
		}

		private void ApplyWorkOrderListFilter() {
			if (lbWorkOrder.InvokeRequired) {
				lbWorkOrder.Invoke((MethodInvoker)delegate {
					lbWorkOrder.Items.Clear();
				});
			}
			else
				lbWorkOrder.Items.Clear();

			for (int i = 0; i < _workOrderList.Count; i++)
				ApplyWorkOrderListFilter(_workOrderList[i]);
			LoadWorkOrder();
		}

		private void ApplyWorkOrderListFilter(dal.WorkOrder WorkOrder) {
			if (WorkOrder == null)
				return;
			StatusState currentState = StatusState.All;
			try {
				currentState = (StatusState)Enum.Parse(typeof(StatusState), lbStatus.SelectedItem.ToString());
			}
			catch { }

			string selectedPilot = "ALL";
			try {
				selectedPilot = lbPilot.SelectedItem.ToString();
			}
			catch { }
			selectedPilot = selectedPilot.ToUpper();

			if (
				(currentState == StatusState.All)
				||
				(WorkOrder.Complete && currentState == StatusState.Complete)
				||
				(WorkOrder.Started && currentState == StatusState.WIP)
				||
				(!WorkOrder.Started && currentState == StatusState.Open)
			) {
				if (
					selectedPilot == "ALL"
					|| WorkOrder.AssignedPilot.ToUpper() == selectedPilot) {

					bool found = false;
					foreach (dal.WorkOrder wo in lbWorkOrder.Items) {
						if (wo.OriginalFilePath.ToUpper() == WorkOrder.OriginalFilePath.ToUpper()) {
							found = true;
							break;
						}
					}
					if (!found) {
						if (lbWorkOrder.InvokeRequired) {
							lbWorkOrder.Invoke((MethodInvoker)delegate {
								lbWorkOrder.Items.Add(WorkOrder);
							});
						}
						else {
							lbWorkOrder.Items.Add(WorkOrder);
						}
					}
				}
			}
		}

		private void SaveBeforeContinue() {
			if (_isDirty) {
				if (DialogResult.Yes == MessageBox.Show("Would you like to save your changes?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) {
					Save();
				}
				_isDirty = false;
			}
		}

		private void Save() {
			for (int i = 0; i < clbProducts.Items.Count; i++) {
				_currentWorkOrder.SetProductFromCustomer(clbProducts.Items[i].ToString(), clbProducts.GetItemChecked(i), false);
			}

			_currentWorkOrder.SetHQNotes(txtAdditionalNotes.Text, false);
			_currentWorkOrder.SetApplicationDetails(
				System.Convert.ToDecimal(txtApplicationAcresPerLoad.Text),
				System.Convert.ToDecimal(txtApplicationAmountPerLoad.Text),
				System.Convert.ToDecimal(txtApplicationLoads.Text),
				System.Convert.ToDecimal(txtApplicationRate.Text),
				System.Convert.ToDecimal(txtApplicationTotal.Text),
				(rbGallons.Checked) ? rbGallons.Tag.ToString() : rbPounds.Tag.ToString(),
				false
				);
			_isDirty = false;
			LoadWorkOrder();
		}

		private void LoadWorkOrderInvoked() {
			SaveBeforeContinue();
			Aerial.db.dal.WorkOrder wo = new dal.WorkOrder("", "");
			if (lbWorkOrder.SelectedItem != null)
				wo = (Aerial.db.dal.WorkOrder)lbWorkOrder.SelectedItem;
			wo.RefreshWorkOrder();
			txtCustomer.Text = wo.Customer;
			txtPilot.Text = wo.AssignedPilot;
			txtCreatedOn.Text = wo.WorkOrderDate;
			txtTargetPests.Text = wo.TargetPests;

			clbProducts.Items.Clear();
			foreach (dal.WorkOrderParsed.Product product in wo.ProductList) {
				clbProducts.Items.Add(product.Name, product.CustomerSupplied);
			}

			txtApplicationAcresPerLoad.Text = wo.ApplicationAcresPerLoad.ToString();
			txtApplicationAmountPerLoad.Text = wo.ApplicationAmountPerLoad.ToString();
			txtApplicationLoads.Text = wo.ApplicationLoads.ToString();
			txtApplicationRate.Text = wo.ApplicationRate.ToString();
			txtApplicationTotal.Text = wo.ApplicationTotal.ToString();
			rbPounds.Checked = wo.ApplicationUnitOfMeasure == rbPounds.Tag.ToString();
			rbGallons.Checked = wo.ApplicationUnitOfMeasure == rbGallons.Tag.ToString();

			txtAdditionalNotes.Text = wo.HQNotes;

			//Load the data grid
			dgFields.Rows.Clear();
			foreach (Aerial.db.dal.WorkOrderParsed.Fields f in wo.Fields)
				dgFields.Rows.Add(f.Complete, f.Name, f.LatLong, f.Area, (f.CompletedBy.Length > 0) ? f.CompletedBy.ToUpper() : "", (f.CompletedDate == Aerial.db.dal.Constants.INVALID_DATE) ? "Never" : f.CompletedDate.ToString());
			dgFields.Columns["colLatLong"].Visible = false;
			foreach (System.Windows.Forms.DataGridViewColumn dgvc in dgFields.Columns) {
				if (dgvc.Visible) {
					dgvc.Width = dgvc.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, false);
				}
			}

			//Update the status
			if (wo.Complete)
				txtStatus.Text = "Status: Complete";
			else if (wo.Started)
				txtStatus.Text = "Status: Started";
			else
				txtStatus.Text = "Status: Open";

			_currentWorkOrder = wo;
			_isDirty = false;
			btnRefresh.Enabled = !Aerial.db.WOP.Properties.Settings.Default.AutoRefresh;
		}

		private void LoadWorkOrder() {
			if (this.InvokeRequired) {
				this.Invoke((MethodInvoker)delegate {
					LoadWorkOrderInvoked();
				});
			}
			else
				LoadWorkOrderInvoked();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="FileName"></param>
		/// <returns></returns>
		private bool OkToSaveFile(string FileName) {
			DialogResult dr = System.Windows.Forms.DialogResult.Yes;
			if (System.IO.File.Exists(FileName)) {
				dr = MessageBox.Show(string.Format("Destination file {0} exists.\r\nWould you like to overwrite it?", FileName), "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
			}
			return dr == System.Windows.Forms.DialogResult.Yes;
		}

		private void Export() {
			if (!_currentWorkOrder.Complete) {
				if (DialogResult.Yes !=
					MessageBox.Show("It appears this work order is not complete.\r\nAre you sure you wish to invoice this work order?", "Continue to Invoice", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
					)
					return;
			}
			this.Cursor = Cursors.WaitCursor;
			if (DialogResult.Yes == MessageBox.Show("Exporting will remove the work order from the pilots and will no longer be monitored.\r\nWould you like to continue?", "Continue with Export", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)) {
				SaveBeforeContinue();
				System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
				sfd.FileName = string.Format("{0} - {1}", (_currentWorkOrder.Customer == "") ? "Unknown" : _currentWorkOrder.Customer, _currentWorkOrder.WorkOrderNumber);
				sfd.ShowDialog();

				string detailFile = string.Format("{0}\\{1} - Detail{2}",
					System.IO.Path.GetDirectoryName(sfd.FileName),
					System.IO.Path.GetFileNameWithoutExtension(sfd.FileName),
					".doc");

				string summaryFile = string.Format("{0}\\{1} - Summary{2}",
					System.IO.Path.GetDirectoryName(sfd.FileName),
					System.IO.Path.GetFileNameWithoutExtension(sfd.FileName),
					".doc");


				string imageFile = string.Format("{0}\\{1} - Image",
					System.IO.Path.GetDirectoryName(sfd.FileName),
					System.IO.Path.GetFileNameWithoutExtension(sfd.FileName));

				string workOrderFile = string.Format("{0}\\{1} - Work Order{2}",
					System.IO.Path.GetDirectoryName(sfd.FileName),
					System.IO.Path.GetFileNameWithoutExtension(sfd.FileName),
					System.IO.Path.GetExtension(_currentWorkOrder.OriginalFilePath));

				if (!OkToSaveFile(workOrderFile) || !OkToSaveFile(detailFile) || !OkToSaveFile(summaryFile)) {
					MessageBox.Show("The files were not saved.\r\nSave operation was cancelled by the user.", "Not Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
					this.Cursor = Cursors.Default;
					return;
				}

				bool saveError = false;
				try {
					//Delete destination files, if they exist...
					try {
						SetAttributes(detailFile);
						System.IO.File.Delete(detailFile);
					}
					catch { } //(Exception Exception) { MessageBox.Show(Exception.ToString()); }
					try {
						SetAttributes(workOrderFile);
						System.IO.File.Delete(workOrderFile);
					}
					catch { } //(Exception Exception) { MessageBox.Show(Exception.ToString()); }
					try {
						SetAttributes(summaryFile);
						System.IO.File.Delete(workOrderFile);
					}
					catch { } //(Exception Exception) { MessageBox.Show(Exception.ToString()); }

					//Save Word Document of the log
					SaveWordDocument(detailFile, true);
					SaveWordDocument(summaryFile, false);

					//Save the original work order
					System.IO.File.Copy(_currentWorkOrder.OriginalFilePath,
						workOrderFile, true); //Delete handled by RemoveCurrentWorkOrder()

					//Save all the images.
					string[] files = _currentWorkOrder.ImageFileList;
					if (files.Length == 1) {
						//Copy the one image
						System.IO.File.Copy(files[0], string.Format("{0}{1}", imageFile, System.IO.Path.GetExtension(files[0])));
					}
					else if (files.Length > 1) {
						for (int i = 0; i < files.Length; i++) {
							System.IO.File.Copy(files[i], string.Format("{0}-{1}{2}", imageFile, i + 1, System.IO.Path.GetExtension(files[i])));

						}
					}
				}
				catch {
					saveError = true;
					MessageBox.Show("There was a problem saving the documents.\r\nThe original source files have been deleted or there is a network connection problem.\r\nPlease try again.",
						"Error Saving",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}

				if (!saveError) {
					RemoveCurrentWorkOrder();
				}
			}
			this.Cursor = Cursors.Default;
		}

		private void SetAttributes(string FileName) {
			System.IO.File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
		}

		private void DeleteRecursive(string Path) {
			foreach (string dir in System.IO.Directory.GetDirectories(Path))
				DeleteRecursive(Path);
			int retryCount = 5;
			foreach (string file in System.IO.Directory.GetFiles(Path)) {
				retryCount = 5;
				while (System.IO.File.Exists(file) && retryCount > 0) {
					try {
						SetAttributes(file);
						System.IO.File.Delete(file);
					}
					catch { }
					retryCount--;
					if (System.IO.File.Exists(file))
						System.Threading.Thread.Sleep(1000);
				}
			}

			retryCount = 5;
			while (System.IO.Directory.Exists(Path) && retryCount > 0) {
				try { System.IO.Directory.Delete(Path); }
				catch { }
				retryCount--;
				if (System.IO.Directory.Exists(Path))
					System.Threading.Thread.Sleep(1000);
			}

		}

		private void CopyFiles(string SourceFolder, string Destination) {
			foreach (string file in System.IO.Directory.GetFiles(SourceFolder))
				try { System.IO.File.Copy(file, string.Format("{0}\\{1}", Destination, System.IO.Path.GetFileName(file))); }
				catch { }
		}

		private void BackupData(string WorkOrder, string WorkOrderPath) {
			string destination = string.Format("{0}\\ADB WOP\\{1}",
			System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
			DateTime.Now.Ticks);
			System.IO.Directory.CreateDirectory(destination);
			System.IO.File.Copy(WorkOrder, string.Format("{0}\\{1}", destination, System.IO.Path.GetFileName(WorkOrder)));
			CopyFiles(WorkOrderPath, destination);
		}

		private void RemoveCurrentWorkOrder() {
			//Remove from GUI
			_workOrderList.Remove(_currentWorkOrder);

			string workingDirectory, originalPath;
			workingDirectory = _currentWorkOrder.WorkingDirectory;
			originalPath = _currentWorkOrder.OriginalFilePath;

			BackupData(originalPath, workingDirectory);

			_currentWorkOrder = null;

			//Remove from file system
			DeleteRecursive(workingDirectory);
			int retryCount = 5;
			while (System.IO.File.Exists(originalPath) && retryCount > 0) {
				try {
					SetAttributes(originalPath);
					System.IO.File.Delete(originalPath);
				}
				catch { }
				retryCount--;
				if (System.IO.File.Exists(originalPath))
					System.Threading.Thread.Sleep(1000);
			}
			if (System.IO.File.Exists(originalPath))
				MessageBox.Show(string.Format("Unable to delete {0}. Please delete it manually.", originalPath), "Error Deleting File", MessageBoxButtons.OK, MessageBoxIcon.Error);


			ApplyWorkOrderListFilter();
		}

		string[] _pilotColorChoices = new string[] { "aqua", "fuchsia", "yellow", "red", "blue" };

		System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> _pilotColors = new List<KeyValuePair<string, string>>();

		private string GetPilotColor(string Pilot) {
			Pilot = Pilot.ToUpper();
			foreach (KeyValuePair<string, string> kvp in _pilotColors) {
				if (kvp.Key == Pilot)
					return kvp.Value;
			}
			//Not found ...
			string color = _pilotColorChoices[_pilotColors.Count % _pilotColorChoices.Length];
			_pilotColors.Add(new KeyValuePair<string, string>(Pilot, color));
			return color;
		}

		private void SaveWordDocument(string FileDestination, bool DetailReport) {
			string fieldColor = "lime";


			//build the content for the dynamic Word document
			//in HTML alongwith some Office specific style properties. 
			StringBuilder strBody = new StringBuilder();
			strBody.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>");
			strBody.Append("<head><title></title>");
			//The setting specifies document's view after it is downloaded as Print
			//instead of the default Web Layout
			strBody.Append("<!--[if gte mso 9]>" +
							 "<xml>" +
							 "<w:WordDocument>" +
							 "<w:View>Print</w:View>" +
							 "<w:Zoom>90</w:Zoom>" +
							 "<w:DoNotOptimizeForBrowser/>" +
							 "</w:WordDocument>" +
							 "</xml>" +
							 "<![endif]-->");

			strBody.Append("<style>" +
							"<!-- /* Style Definitions */" +
							"@page Section1" +
							"   {size:8.5in 11.0in; " +
							"   margin:1.0in 1.25in 1.0in 1.25in ; " +
							"   mso-header-margin:.5in; " +
							"   mso-footer-margin:.5in; mso-paper-source:0;}" +
							" div.Section1" +
							"   {page:Section1;}" +
							"-->" +
						   "</style></head>");

			strBody.Append("<body lang=EN-US style='tab-interval:.5in'>" +
							"<div class=Section1>");
			strBody.Append(string.Format("<p><b>Work Order:</b> {0}<br/><b>Date:</b> {1}</p>", _currentWorkOrder.WorkOrderNumber, _currentWorkOrder.WorkOrderDate.ToString()));

			string tab = "&nbsp;&nbsp;&nbsp;";

			if (DetailReport) {
				strBody.Append("<p><b>Customer Supplied Products:</b><br/>");
				bool found = false;
				foreach (dal.WorkOrderParsed.Product product in _currentWorkOrder.ProductList) {
					if (product.CustomerSupplied) {
						strBody.Append(string.Format("{0}{1}<br/>", tab, product.Name));
						found = true;
					}
				}
				if (!found)
					strBody.Append("None");
				strBody.Append("</p>");
			}
			strBody.Append("<p><b>Application Times:</b><br/>");

			DateTime lastPunch = dal.Constants.INVALID_DATE;

			foreach (dal.WorkOrderEntry.WorkOrderEntry woe in _currentWorkOrder.PilotEntries) {
				if (woe.ClockPunches != null && woe.ClockPunches.Length > 0) {
					strBody.Append(string.Format("{0}<span style='background:{2};mso-highlight:{2}'><b>{1}</b></span><br/>", tab, woe.PilotName, GetPilotColor(woe.PilotName)));
					DateTime[] punches = new DateTime[woe.ClockPunches.Length];
					for (int i = 0; i < punches.Length; i++) {
						punches[i] = woe.ClockPunches[i].Date;
					}
					Array.Sort<DateTime>(punches);
					for (int i = 0; i < punches.Length; i++) {
						strBody.Append(string.Format("{0}{0}{1}:  {2}<br/>", tab, (i % 2 == 0) ? "Start" : "Stop", punches[i].ToString()));
					}
					if (punches[punches.Length - 1] > lastPunch)
						lastPunch = punches[punches.Length - 1];
				}
			}
			strBody.Append("</p>");

			//Fields
			strBody.Append("<p><b>Fields:</b><br/>");
			dal.WorkOrderEntry.Environment lastEnvironment = null;
			foreach (dal.WorkOrderParsed.Fields field in _currentWorkOrder.Fields) {
				string completedBy = field.CompletedBy == "" ? _currentWorkOrder.AssignedPilot : field.CompletedBy;
				strBody.Append(string.Format("{0}<b>Field:</b> <span style='background:{6};mso-highlight:{6}'>{1}</span> ({2}) {3} acres<br/>{0}Completed On {4} by <span 'background:{7};mso-highlight:{7}'>{5}</span><br/>", tab, field.Name, field.LatLong, field.Area,
					field.CompletedDate == dal.Constants.INVALID_DATE ? lastPunch : field.CompletedDate,
					completedBy,
					fieldColor,
					GetPilotColor(completedBy)
					));
				bool fieldFound = false;
				foreach (dal.WorkOrderEntry.WorkOrderEntry pwoe in _currentWorkOrder.PilotEntries) {
					if (pwoe.Fields != null) {
						foreach (dal.WorkOrderEntry.Fields pilotField in pwoe.Fields) {
							if (pilotField.LatLong.ToUpper() == field.LatLong.ToUpper()) {
								strBody.Append(string.Format("{0}{0}<b>Pilot:</b> <span style='background:{6};mso-highlight:{6}'>{1}</span><br/>{0}{0}{0}<b>Time Completed:</b> {2}<br/>{0}{0}{0}<b>Wind Speed:</b> {3}<br/>{0}{0}{0}<b>Wind Direction:</b> {4}<br/>{0}{0}{0}<b>Temperature:</b> {5}<br/>",
									tab,
									pwoe.PilotName,
									pilotField.Stamp.DateRecorded.ToString(),
									pilotField.Environment.WindSpeed,
									pilotField.Environment.WindDirection,
									pilotField.Environment.Temperature,
									GetPilotColor(pwoe.PilotName)));
								fieldFound = true;
								break;
							}
							if (lastEnvironment == null || lastEnvironment.Stamp.DateRecorded < pilotField.Environment.Stamp.DateRecorded)
								lastEnvironment = pilotField.Environment;
						}
					}
				}
				if (!fieldFound) {

					strBody.Append(string.Format("{0}{0}<b>Pilot:</b> <span style='background:{6};mso-highlight:{6}'>{1}</span><br/>{0}{0}{0}<b>Time Completed:</b> {2}<br/>{0}{0}{0}<b>Wind Speed:</b> {3}<br/>{0}{0}{0}<b>Wind Direction:</b> {4}<br/>{0}{0}{0}<b>Temperature:</b> {5}<br/>",
						tab,
						_currentWorkOrder.AssignedPilot,
						lastPunch,
						lastEnvironment == null ? "Not Recorded" : lastEnvironment.WindSpeed.ToString(),
						lastEnvironment == null ? "Not Recorded" : lastEnvironment.WindDirection.ToString(),
						lastEnvironment == null ? "Not Recorded" : lastEnvironment.Temperature.ToString(),
						GetPilotColor(_currentWorkOrder.AssignedPilot)
						));
				}
			}
			strBody.Append("</p>");
			if (DetailReport) {
				//Application rates
				strBody.Append("<b>Application Rates:</b><br/>");
				strBody.Append(string.Format("<table><tr><td>{0}Total:</td><td>{1} ac</td></tr><tr><td>{0}Rate:</td><td>{2} {3}/ac</td></tr><tr><td>{0}Loads:</td><td>{4}</td></tr><tr><td>{0}Acres/Load:</td><td>{5} ac</td></tr><tr><td>{0}Amount/Load:</td><td>{6} {3}</td></tr></table>",
					tab, _currentWorkOrder.ApplicationTotal, _currentWorkOrder.ApplicationRate, _currentWorkOrder.ApplicationUnitOfMeasure, _currentWorkOrder.ApplicationLoads, _currentWorkOrder.ApplicationAcresPerLoad, _currentWorkOrder.ApplicationAmountPerLoad));
				strBody.Append("<br/>");

				//Notes
				strBody.Append("<p><b>Notes:</b><br/><pre>");
				strBody.Append(_currentWorkOrder.HQNotes);
				strBody.Append("</pre></p>");
			}
			strBody.Append("</div></body></html>");

			System.IO.File.WriteAllText(FileDestination, strBody.ToString());
		}
		#endregion Logic

		#region Event Handlers
		private void listBoxStatus_SelectedIndexChanged(object sender, EventArgs e) {
			ApplyWorkOrderListFilter();
		}

		private void lbPilot_SelectedIndexChanged(object sender, EventArgs e) {
			ApplyWorkOrderListFilter();
		}

		private void lbWorkOrder_SelectedIndexChanged(object sender, EventArgs e) {
			LoadWorkOrder();
		}

		private void clbProducts_ItemCheck(object sender, ItemCheckEventArgs e) {
			_isDirty = true;
		}

		private void rb_CheckedChanged(object sender, EventArgs e) {
			_isDirty = true;
			RadioButton c = (RadioButton)sender;
			if (c.Checked) {
				txtUofM.Text = c.Tag.ToString();
				txtUofMPerAc.Text = string.Format("{0}/ac", c.Tag.ToString());

			}
		}

		private void btnRefresh_Click(object sender, EventArgs e) {
			RefreshWorkOrders();
		}

		private void btnExport_Click(object sender, EventArgs e) {
			Export();
		}

		private void txt_TextChanged(object sender, EventArgs e) {
			_isDirty = true;
		}

		private void ProgressBarFired(bool Visible) {
			if (pbProcessing.InvokeRequired) {
				BeginInvoke(new ProgressBarFiredDelegate(ProgressBarFired), new object[] { Visible });
			}
			else {
				pbProcessing.Visible = Visible;
			}
		}

		private void TimerEventFired(System.Collections.Generic.List<Aerial.db.dal.WorkOrder> List, bool ForceRefresh) {
			if (Aerial.db.WOP.Properties.Settings.Default.AutoRefresh || ForceRefresh) {
				//Check for missing work orders ... update as necessary. 
				if (lbWorkOrder.InvokeRequired) {
					BeginInvoke(new TimerEventFiredDelegate(TimerEventFired), new object[] { List, ForceRefresh });
				}
				else {
					//Add and update
					foreach (dal.WorkOrder newWO in List) {
						bool found = false;
						if (_currentWorkOrder != null
							&& newWO.OriginalFilePath == _currentWorkOrder.OriginalFilePath) {
							//&& _currentWorkOrder.UniqueID != newWO.UniqueID) {
							//_currentWorkOrder.RefreshWorkOrder(); -- This is handled within LoadWorkOrder()
							//LoadWorkOrder(); //Force reload of current work order
							found = true;
							if (_currentWorkOrder.UniqueID != newWO.UniqueID) {
								if (_isDirty)
									btnRefresh.Enabled = true;
								else {
									int saveRow = 0;
									if (dgFields.Rows.Count > 0)
										saveRow = dgFields.FirstDisplayedCell.RowIndex;

									LoadWorkOrder(); //Force reload of current work order

									if (saveRow != 0 && saveRow < dgFields.Rows.Count)
										dgFields.FirstDisplayedScrollingRowIndex = saveRow;
								}
								break; //foreach(dal.WorkOrder newWO in List)
							}
							//else -- Work Order is up-to-date
						}
						else {
							for (int i = 0; i < _workOrderList.Count; i++) {
								if (newWO.OriginalFilePath.ToUpper() == _workOrderList[i].OriginalFilePath.ToUpper()) {
									//Check to see if the status has changed...If so, we need to refresh it.
									if (newWO.Complete != _workOrderList[i].Complete
										|| newWO.Started != _workOrderList[i].Started) {
										_workOrderList[i].RefreshWorkOrder();
										ApplyWorkOrderListFilter(_workOrderList[i]);
									}
									if (_workOrderList[i].QuickLoad) {
										_workOrderList[i].QuickLoad = false;
										_workOrderList[i].RefreshWorkOrder();
										ApplyWorkOrderListFilter(_workOrderList[i]);
									}
									found = true;
									break;
								}
							}
							if (!found) //Alternative statement, not used for readability: if(i>=lbWorkOrder.Items.Count) 
                        {//Not found...Add it to the list.
								_workOrderList.Add(newWO);
								ApplyWorkOrderListFilter(newWO);
							}
						}



					}

					//Remove non matching in the list box
					for (int i = lbWorkOrder.Items.Count - 1; i >= 0; i--) {
						bool found = false;
						foreach (dal.WorkOrder newWO in List) {
							if (newWO.OriginalFilePath.ToUpper() == ((dal.WorkOrder)lbWorkOrder.Items[i]).OriginalFilePath.ToUpper()) {
								found = true;
								break;
							}
						}
						if (!found) {
							lbWorkOrder.Items.RemoveAt(i);
						}
					}

					//Remove from the master list
					for (int i = _workOrderList.Count - 1; i >= 0; i--) {
						bool found = false;
						foreach (dal.WorkOrder newWO in List) {
							if (newWO.OriginalFilePath.ToUpper() == _workOrderList[i].OriginalFilePath.ToUpper()) {
								found = true;
								break;
							}
						}
						if (!found) {
							_workOrderList.RemoveAt(i);
						}
					}
				}
			}
		}

		public void RefreshWorkOrdersForThreads() {
			ProgressBarFired(true);
			TimerEventFired(GetWorkOrderList(false), true);
			ProgressBarFired(false);
		}

		private bool _firstProcessing = true;
		public void tmrThreadingTimer_TimerCallback(object State) {
			if (_firstProcessing) {
				_firstProcessing = false;
				RefreshWorkOrders();
			}

			if (Aerial.db.WOP.Properties.Settings.Default.AutoRefresh) {
				//Disable the timer
				_timer.Change(System.Threading.Timeout.Infinite, _timerCallbackDelayInMs);

				//Process the data ...
				ProgressBarFired(true);
				TimerEventFired(GetWorkOrderList(false), false);
				ProgressBarFired(false);

				//Enable the timer
				_timer.Change(_timerCallbackDelayInMs, _timerCallbackDelayInMs);
				_firstProcessing = false;
			}
		}

		private void btnSave_Click(object sender, EventArgs e) {
			Save();
		}

		private void btnCancel_Click(object sender, EventArgs e) {
			if (DialogResult.Yes == MessageBox.Show("Cancelling your edit will lose any changes you have made. Are you sure you would like to continue?", "Lose Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)) {
				_isDirty = false;
				LoadWorkOrder();
			}
		}

		private void cbAutoRefresh_CheckedChanged(object sender, EventArgs e) {
			Aerial.db.WOP.Properties.Settings.Default.AutoRefresh = cbAutoRefresh.Checked;
			Aerial.db.WOP.Properties.Settings.Default.Save();

			//We only enable the button when disabling AutoRefresh
			//If we go from AutoRefresh disabled to enabled, we are 
			//unsure of the state of the current workorder, so we
			//by default will leave refresh enabled.
			if (!Aerial.db.WOP.Properties.Settings.Default.AutoRefresh)
				btnRefresh.Enabled = true;
		}
		#endregion Event Handlers
	}
}