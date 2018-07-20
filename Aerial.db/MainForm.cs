using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aerial.db.dal;

namespace Aerial.db {
	public partial class MainForm : Form {
		#region Private Members and properties
		public static string VERSION {
			get { return string.Format("1.00.{0}", Aerial.db.dal.Constants.DALVERSION); }
		}

		private Aerial.db.dal.Pilot _dal = new dal.Pilot(Aerial.db.Properties.Settings.Default.WorkingFolder);
		private Point _pictureBoxSize = new Point();
		private int _fieldGridYPos = 0;
		private int _fieldGridMaxHeight = 0;
		private System.Threading.Timer _timer = null;

		private Image[] __largePictureImages = new Image[0];
		private Image[] _largePictureImages {
			get { return __largePictureImages; }
			set {
				__largePictureImages = (value == null) ? new Image[0] : value;
				_imagePosition = 0;
			}
		}

		private int __imagePosition = 0;
		private int _imagePosition {
			get { return __imagePosition; }
			set {
				if (value < _largePictureImages.Length && value >= 0) {
					__imagePosition = value;
					ChangeLargePicture(_largePictureImages[value]);
				}
				btnPreviousMap.Enabled = __imagePosition > 0;
				btnNextMap.Enabled = (__imagePosition < _largePictureImages.Length - 1);
			}
		}

		private Control[] _basicFunctionalityControls = new Control[0];
		private Label[] _productLabelList = new Label[0];
		private Control[] _textControlsToClear = new Label[0];
		private Control[] _controlsNotToToggleAutomatically = new Control[0];
		private WorkOrder _currentWorkOrder {
			get {
				WorkOrder wo = (WorkOrder)lbWorkOrder.SelectedItem;

				//TODO: Refresh saved values on form.

				return wo;
			}
		}

		private string _applicationPilot {
			get {
				if (lnkPilot.Text == Pilot.NO_PILOT_SELECTED)
					return "";
				return lnkPilot.Text;
			}
			set {
				if (value == "" || !_dal.IsValidPilot(value)) {
					lnkPilot.Text = Pilot.NO_PILOT_SELECTED;
				}
				else {
					lnkPilot.Text = value;
					Aerial.db.Properties.Settings.Default.ApplicationPilot = value;
					Aerial.db.Properties.Settings.Default.Save();
				}
				_dal.SetApplicator(lnkPilot.Text);

				EnableFunctionality(false);
				if (lnkPilot.Text != Pilot.NO_PILOT_SELECTED)
					EnableBasicFunctionality();
				int location = cbPilot.Items.IndexOf(value);
				if (location >= 0) {
					cbPilot.SelectedItem = cbPilot.Items[cbPilot.Items.IndexOf(value)];
					cbPilot_SelectedIndexChanged(null, null);
				}
				lbWorkOrder.SelectedIndex = -1;
			}
		}
		#endregion Private Members and properties

		#region Initialization
		public MainForm() {
			this.InitializeComponent();

			lblVersion.Text = string.Format("Version {0}", VERSION);
			_pictureBoxSize.X = pbPicture.Width;
			_pictureBoxSize.Y = pbPicture.Height;

			_basicFunctionalityControls = new Control[] { lblPilot, cbPilot, lblWorkOrderList, lbWorkOrder, btnHideCompleted,
            //lblTime, lblTemp, lblHQNotes, lblPilotNotes, lblApplicator, 
            lblApplicator, lnkPilot, lblVersion
            };
			_controlsNotToToggleAutomatically = new Control[] { btnConfirmPunchDelete };
			_productLabelList = new Label[] { txtProduct01, txtProduct02, txtProduct03, txtProduct04, txtProduct05, txtProduct06 };
			_textControlsToClear = new Control[] {
                txtProduct01, txtProduct02, txtProduct03, txtProduct04, txtProduct05, txtProduct06,
                txtCreated, txtCustomer, txtProducts, txtTargetPests, txtWorkOrder};

			_controlColor = ControlColors.ControlColors.LoadXMLFromString(Aerial.db.Properties.Settings.Default.ControlColor);
			this.WindowState = FormWindowState.Maximized;
			InitialInitialization();

			pnlCoreControls.BringToFront();
			pnlDetails.BringToFront();
			pnlEnvironment.BringToFront();
			pnlMap.BringToFront();
			pnlDetails.Visible = false;
			pnlEnvironment.Visible = false;
			pnlMap.Visible = false;

			_fieldGridYPos = dgFields.Location.Y;
			_fieldGridMaxHeight = dgFields.Height;

			btnNorth.Tag = CompassDirection.North;
			btnNorthEast.Tag = CompassDirection.NorthEast;
			btnEast.Tag = CompassDirection.East;
			btnSouthEast.Tag = CompassDirection.SouthEast;
			btnSouth.Tag = CompassDirection.South;
			btnSouthWest.Tag = CompassDirection.SouthWest;
			btnWest.Tag = CompassDirection.West;
			btnNorthWest.Tag = CompassDirection.NorthWest;

			txtTemp.Text = "80"; //Init value

			lbWorkOrder.SelectedIndex = -1;

			//Bindings
			lblFieldCount2.DataBindings.Add("Text", lblFieldCount, "Text");
			lblLoadCount2.DataBindings.Add("Text", lblLoadCount, "Text");
			txtAppTotal2.DataBindings.Add("Text", txtAppTotal, "Text");
			txtAppRate2.DataBindings.Add("Text", txtAppRate, "Text");
			txtAppLoad2.DataBindings.Add("Text", txtAppLoad, "Text");
			txtAppAcPerLoad2.DataBindings.Add("Text", txtAppAcPerLoad, "Text");
			txtAppAmtPerLoad2.DataBindings.Add("Text", txtAppAmtPerLoad, "Text");

			//CreateTimer();
		}

		private void CreateTimer(bool CreateIfNull) {
			//Stop the timer
			if (_timer != null)
				_timer.Change(System.Threading.Timeout.Infinite, 30000);

			_callbackOption = _dal.PilotPath + "|" + _applicationPilot;

			//Recreate the timer
			if (_timer != null || CreateIfNull)
				_timer = new System.Threading.Timer(new System.Threading.TimerCallback(tmrThreadingTimer_TimerCallback), _callbackOption, 5, 30000); //30 second callback
		}

		private string _callbackOption = "";

		private delegate void TimerEventFiredDelegate(WorkOrderList List);
		private void TimerEventFired(WorkOrderList List) {
			try {
				//Check for missing work orders ... update as necessary. 
				//Do logic to determine if update is needed...
				if (lbWorkOrder.InvokeRequired) {
					BeginInvoke(new TimerEventFiredDelegate(TimerEventFired), new object[] { List });
				}
				else {
					//Add and update
					foreach (dal.WorkOrder newWO in List) {

						if (
							//Work Order pilot is the pilot's work orders to view
							cbPilot.SelectedIndex >= 0 && newWO.AssignedPilot.ToUpper() == cbPilot.SelectedItem.ToString().ToUpper()

							//State of work order matches what we want to view
							&&
							(!_hideCompleteWorkOrders || _hideCompleteWorkOrders && !newWO.Complete)
							) {
							bool found = false;
							if (_currentWorkOrder != null
								&& newWO.OriginalFilePath == _currentWorkOrder.OriginalFilePath
								&& _applicationPilot.ToUpper() == newWO.AssignedPilot.ToUpper()) {
								found = true;
								if (_currentWorkOrder.UniqueID != newWO.UniqueID) {
										int saveRow = 0;
										if (dgFields.Rows.Count > 0)
											saveRow = dgFields.FirstDisplayedCell.RowIndex;

										LoadWorkOrder(); //Force reload of current work order

										if (saveRow != 0 && saveRow < dgFields.Rows.Count)
											dgFields.FirstDisplayedScrollingRowIndex = saveRow;
									}
							}
							else {
								for (int i = 0; i < lbWorkOrder.Items.Count; i++) {

									if (newWO.OriginalFilePath.ToUpper() == ((dal.WorkOrder)lbWorkOrder.Items[i]).OriginalFilePath.ToUpper()) {
										found = true;
										break;
									}
								}
								if (!found) //Alternative statement, not used for readability: if(i>=lbWorkOrder.Items.Count) 
                                {//Not found...Add it to the list.
									lbWorkOrder.Items.Add(newWO);
								}
							}
						}
					}

					//Remove non matching
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
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.ToString());
			}
		}


		public void tmrThreadingTimer_TimerCallback(object State) {
			//Disable the timer
			_timer.Change(System.Threading.Timeout.Infinite, 30000);

			//Process the data ...
			string[] l = State.ToString().Split('|');
			if (l[0] != "") {
				Pilot p = new Pilot(System.IO.Path.GetDirectoryName(l[0]));
				p.SetPilot(System.IO.Path.GetFileNameWithoutExtension(l[0]));
				WorkOrderList List = new WorkOrderList(p, l[1]);
				foreach (WorkOrder wo in List) {
					int i = 0;
					i++;
				}
				TimerEventFired(List);
			}

			//Enable the timer
			_timer.Change(30000, 30000);
		}


		private void InitialInitialization() {
			Log.LogWriter.WriteSystem("Program started.");
			_applicationPilot = Aerial.db.Properties.Settings.Default.ApplicationPilot;
			cbPilot.Items.AddRange(_dal.GetPilotList(Aerial.db.dal.Pilot.ViewSpecificPilotListFromString(Aerial.db.Properties.Settings.Default.ViewSpecificPilots)));
			if (cbPilot.Items.IndexOf(Aerial.db.Properties.Settings.Default.ApplicationPilot) >= 0)
				cbPilot.SelectedItem = cbPilot.Items[cbPilot.Items.IndexOf(Aerial.db.Properties.Settings.Default.ApplicationPilot)];
			else {
				if (cbPilot.Items.Count > 0)
					cbPilot.SelectedValue = cbPilot.Items[0];
			}

			UpdateColorScheme();
		}
		#endregion Initialization

		#region Event Handlers
		private void lnkPilot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			PilotChooser pc = new PilotChooser();
			pc.Init(_dal.GetPilotList(Aerial.db.dal.Pilot.ViewSpecificPilotListFromString(Aerial.db.Properties.Settings.Default.ViewSpecificPilots)), lnkPilot.Text);
			pc.ShowDialog();
			_applicationPilot = pc.SelectedPilot;
		}

		private void cbPilot_SelectedIndexChanged(object sender, EventArgs e) {
			if (cbPilot.SelectedIndex >= 0) {
				DateTime startTime = DateTime.Now;
				_dal.SetPilot(cbPilot.SelectedItem.ToString());
				CreateTimer(false);
				PopulateWorkOrders(true);
				Log.LogWriter.WriteSystem(string.Format("Took {0} ms to load {1} records for pilot {2}", DateTime.Now.Subtract(startTime).TotalMilliseconds, lbWorkOrder.Items.Count, _dal.PilotName));
			}
		}

		#region Graphics
		private void cbPilot_DrawItem(object sender, DrawItemEventArgs e) {
			int index = e.Index >= 0 ? e.Index : 0;
			Brush brush = Brushes.Black;
			e.DrawBackground();

			Color fore = Color.Black; //Will be overridden
			Color back = Color.Black; //Will be overridden
			if (System.Convert.ToBoolean(e.State & System.Windows.Forms.DrawItemState.Selected | e.State & DrawItemState.ComboBoxEdit)) {//Item selected?
				fore = Aerial.db.Properties.Settings.Default.SelectedForeColor;
				back = Aerial.db.Properties.Settings.Default.SelectedBackColor;
			}
			else {
				fore = Aerial.db.Properties.Settings.Default.ControlForeColor;
				back = Aerial.db.Properties.Settings.Default.ControlBackColor;
			}

			e.Graphics.DrawRectangle(new Pen(back), e.Bounds);
			e.Graphics.FillRectangle(new SolidBrush(back), e.Bounds);

			e.Graphics.DrawString(cbPilot.Items[index].ToString(), e.Font, new SolidBrush(fore), e.Bounds, StringFormat.GenericDefault);
			//e.DrawFocusRectangle();
		}
		#endregion Graphics

		private void btnPunchClock_Click(object sender, EventArgs e) {
			DateTime now = DateTime.Now;
			DateTime PunchTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0); //Strip anything beyond a minute
			_clockPunchList.Add(PunchTime);
			RefreshClockPunchList();
			_currentWorkOrder.AddClockPunch(_applicationPilot, PunchTime);
		}

		private void btnCopyAll_Click(object sender, EventArgs e) {
			bool found = false;
			foreach (WorkOrder wo in lbWorkOrder.Items) {
				string[] files = wo.ShapeFiles;
				if (files == null || files.Length == 0) {
					//MessageBox.Show("Unable to copy Shape files.\r\nNo shape files found.", "Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else {
					//Find drive ...
					for (int i = (int)'D'; i <= (int)'Z'; i++) {
						if (System.IO.Directory.Exists(string.Format("{0}:\\", (char)i))) {
							foreach (string file in files)
								try {
									System.IO.File.Copy(file, string.Format("{0}:\\{1}", (char)i, System.IO.Path.GetFileName(file)), true);
									found = true;
								}
								catch { }
						}
					}
					//else
					//{
					//    MessageBox.Show("The shape files copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					//}
				}
			}
			if (!found) {
				MessageBox.Show("No external drives found.\r\nShape files were not copied.", "Drive not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
				MessageBox.Show("The shape files copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void lblVersion_MouseUp(object sender, MouseEventArgs e) {
			//if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				CustomizeColors cc = new CustomizeColors();
				cc.ShowDialog();
				UpdateColorScheme();
			}
		}

		private void txtPilotNotes_TextValueChanged(object sender, EventArgs e) {
			if (_currentWorkOrder != null)
				_currentWorkOrder.SetPilotNotes(_applicationPilot, txtPilotNotes.Text);
		}

		#endregion Event Handlers

		#region UI Manipulation
		#region Colors
		private void UpdateColorScheme() {
			UpdateColorScheme(this);
			for (int i = 0; i < dgFields.Rows.Count; i++)
				UpdateGridFont(i);
			UpdateWindDirectionControls();
			SetPanelButtonProperties();
			SetControlColors();
		}
		private void UpdateColorScheme(Control Control) {
			if (Control.Controls != null)
				foreach (Control c in Control.Controls)
					UpdateColorScheme(c);
			Control.BackColor = Aerial.db.Properties.Settings.Default.ControlBackColor;
			Control.ForeColor = Aerial.db.Properties.Settings.Default.ControlForeColor;

			if (Control.GetType() == typeof(System.Windows.Forms.DataGridView)) {
				((System.Windows.Forms.DataGridView)Control).BackgroundColor = Control.BackColor;
				//((System.Windows.Forms.DataGridView)Control).Color = Control.BackColor;
				((System.Windows.Forms.DataGridView)Control).RowsDefaultCellStyle.BackColor = Control.BackColor;
				((System.Windows.Forms.DataGridView)Control).RowsDefaultCellStyle.ForeColor = Control.ForeColor;
				((System.Windows.Forms.DataGridView)Control).RowsDefaultCellStyle.SelectionBackColor = Aerial.db.Properties.Settings.Default.SelectedBackColor;
				((System.Windows.Forms.DataGridView)Control).RowsDefaultCellStyle.SelectionForeColor = Aerial.db.Properties.Settings.Default.SelectedForeColor;
				//TODO: Grid does not refresh color scheme if it is already being displayed.
			}
			else if (Control.GetType() == typeof(AerialListBox)) {
				((AerialListBox)Control).BorderColor = Aerial.db.Properties.Settings.Default.BorderColor;
				//((AerialListBox)Contorl).Color
			}
			else if (Control.GetType() == typeof(LinkLabel)) {
				((System.Windows.Forms.LinkLabel)Control).ActiveLinkColor = Aerial.db.Properties.Settings.Default.ControlForeColor;
				((System.Windows.Forms.LinkLabel)Control).LinkColor = Aerial.db.Properties.Settings.Default.ControlForeColor;
			}
			else if (Control.GetType() == typeof(AerialTextBox)) {
				((AerialTextBox)Control).BorderColor = Aerial.db.Properties.Settings.Default.BorderColor;
			}
			else if (Control.GetType() == typeof(AerialWind)) {
				((AerialWind)Control).BackColor = Aerial.db.Properties.Settings.Default.ControlBackColor;
				((AerialWind)Control).ForeColor = Aerial.db.Properties.Settings.Default.ControlForeColor;
				((AerialWind)Control).CompassColor = Aerial.db.Properties.Settings.Default.SelectedBackColor;
			}
			else if (Control.GetType() == typeof(Aerial.db.AerialButton)) {
				((AerialButton)Control).BackColor = Aerial.db.Properties.Settings.Default.ButtonBackColor;
				((AerialButton)Control).ForeColor = Aerial.db.Properties.Settings.Default.ButtonForeColor;
			}

		}

		ControlColors.ControlColors _controlColor = null;
		private void SetControlColors() {
			//Load XML
			if (_controlColor.Control != null) {
				foreach (ControlColors.Control c in _controlColor.Control) {
					SetControlColors(this, c);
				}
			}
		}

		private bool SetControlColors(Control Control, ControlColors.Control ColorScheme) {
			if (GetControlHash(Control) == ColorScheme.ID) {
				Control.ForeColor = System.Drawing.Color.FromArgb(ColorScheme.ForeR, ColorScheme.ForeG, ColorScheme.ForeB);
				Control.BackColor = System.Drawing.Color.FromArgb(ColorScheme.BackR, ColorScheme.BackG, ColorScheme.BackB);
				return true;
			}
			else if (Control.Controls != null) {
				bool retVal = false;
				foreach (Control c in Control.Controls) {
					retVal = SetControlColors(c, ColorScheme);
					if (retVal == true)
						return true;
				}
			}
			return false;
		}

		private string GetControlHash(Control Control) {
			return string.Format("{0}:{1}", Control.Text, (Control.Tag == null) ? "null" : Control.Tag.ToString());
		}

		#endregion Colors
		private void EnableFunctionality(bool Enable) {
			foreach (Control c in this.Controls)
				EnableFunctionality(Enable, c);
			lblApplicator.Visible = true; //This is always enabled.
			lnkPilot.Visible = true; //This is always enabled.
		}

		private void EnableFunctionality(bool Enable, Control Control) {
			foreach (Control ignoreControl in _controlsNotToToggleAutomatically) {
				if (ignoreControl.Equals(Control))
					return;
			}
			//Control.Enabled = Enable;
			if (Control.GetType() != typeof(Panel))
				Control.Visible = Enable;
			if (Control.Controls != null) {
				foreach (Control c in Control.Controls)
					EnableFunctionality(Enable, c);
			}
		}


		private void EnableBasicFunctionality() {
			foreach (Control c in _basicFunctionalityControls)
				c.Visible = true;

			foreach (Control c in _textControlsToClear)
				c.Text = "";
			pbPicture.Image = null;
			dgFields.DataSource = null;
			lblSelectWorkOrder.Visible = true;
			lblSelectWorkOrder.BringToFront();
		}
		#endregion UI Manipulation

		private void LoadWorkOrder() {
			if (lbWorkOrder.SelectedIndex >= 0) {
				EnableFunctionality(true);
				lblSelectWorkOrder.Visible = false;

				WorkOrder wo = _currentWorkOrder; //Make a copy of the work order so we do not refresh it every time we access it.
				wo.RefreshWorkOrder();

				txtWorkOrder.Text = string.Format("Work Order {0}", wo.WorkOrderNumber);
				txtWorkOrderComplete.Visible = wo.Complete;

				txtCreated.Text = string.Format("Created On: {0}", wo.WorkOrderDate);
				txtCustomer.Text = string.Format("Customer: {0}", wo.Customer);
				txtTargetPests.Text = string.Format("Target Pests: {0}", wo.TargetPests);
				txtProducts.Text = "Products:";
				for (int i = 0; i < _productLabelList.Length; i++) {
					_productLabelList[i].Text = wo.ProductList.Length >= i + 1 ? wo.ProductList[i].Name : "";
				}

				//Fields
				//Grab last index
				int lastRowIndex, lastRowCount;
				if (dgFields.SelectedCells.Count > 0)
					lastRowIndex = dgFields.SelectedCells[0].RowIndex;
				else
					lastRowIndex = 0;
				lastRowCount = dgFields.Rows.Count;

				dgFields.Rows.Clear();
				foreach (Aerial.db.dal.WorkOrderParsed.Fields f in wo.Fields)
					dgFields.Rows.Add(f.Complete, f.Name, f.LatLong, f.Area, (f.CompletedBy.Length > 0) ? f.CompletedBy[0].ToString().ToUpper() : "");
				int totalWidth = 0;
				int visibleColumnCount = 0;
				dgFields.Columns["colLatLong"].Visible = false;
				foreach (System.Windows.Forms.DataGridViewColumn dgvc in dgFields.Columns) {
					if (dgvc.Visible) {
						visibleColumnCount++;
						dgvc.Width = dgvc.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, false);
						totalWidth += dgvc.Width;
					}
				}
				int widthLeft = dgFields.Width - totalWidth;
				foreach (System.Windows.Forms.DataGridViewColumn dgvc in dgFields.Columns) {
					//dgvc.Width = dgvc.Width + ((widthLeft / visibleColumnCount) - 1);//Integeger division truncates/rounds down. This is preferred. Minus one due to border width.
					dgvc.Width = dgvc.Width + ((widthLeft / visibleColumnCount) - 1 - 15);//Integeger division truncates/rounds down. This is preferred. Minus one due to border width. Minus 15 due to scroll bars
					//totalWidth += dgvc.Width; //We could do something with the remaining pixels left due to rounding error if needed...
				}
				int height = 0;
				foreach (DataGridViewRow dgvr in dgFields.Rows) {
					dgvr.Height += 10; //Make it a little taller...
					UpdateGridFont(dgvr.Index); //Update the font.
					height += dgvr.Height;
				}

				//Tweak the grid height and position
				height += dgFields.Rows.Count * 2;
				if (height > _fieldGridMaxHeight)
					height = _fieldGridMaxHeight;
				dgFields.Height = height;
				dgFields.Top = _fieldGridYPos + (_fieldGridMaxHeight - height);

				//Select the last selected row
				if(lastRowCount == dgFields.Rows.Count && lastRowIndex < dgFields.Rows.Count)
					dgFields.CurrentCell = dgFields.Rows[lastRowIndex].Cells["colFieldName"];


				_largePictureImages = wo.Images;
				if (_largePictureImages.Length > 0) {
					pbPicture.Image = (_largePictureImages.Length == 0) ? null : _largePictureImages[0];

					//Resize preview picture box as appropraite
					//Get Ratio:
					if (pbPicture.Image != null) {
						double ratio = pbPicture.Image.Width / (pbPicture.Image.Height + 0.0);
						pbPicture.Width = _pictureBoxSize.X;
						pbPicture.Height = _pictureBoxSize.Y;
						if (ratio > 0) {
							//ratio = pictureBox1.Image.Height / (pictureBox1.Image.Width + 0.0);
							pbPicture.Height = (int)(pbPicture.Height / ratio); // (int)(ratio * pictureBox1.Height);
							//pictureBox1.Width = (int)((pictureBox1.Image.Height / (pictureBox1.Image.Width + 0.0)) * pictureBox1.Width);
						}
						else {
							pbPicture.Width = (int)(ratio * pbPicture.Width);
							//pictureBox1.Height = (int)(ratio * pictureBox1.Height);
						}

						if (pbPicture.Height > _pictureBoxSize.Y) {
							ratio = pbPicture.Width / (pbPicture.Height + 0.0);
							pbPicture.Height = _pictureBoxSize.Y;
							pbPicture.Width = (int)(pbPicture.Width * ratio);
						}
					}
				}

				//Load Clock Punches
				_clockPunchList.Clear();
				foreach (DateTime dt in wo.ClockPunches)
					_clockPunchList.Add(dt);
				RefreshClockPunchList();

				//Load Loads
				lbLoads.Items.Clear();
				lbLoads2.Items.Clear();
				DateTime[] times = wo.LoadTimes.ToArray();
				System.Array.Sort(times);
				foreach (DateTime dt in times) {
					lbLoads.Items.Insert(0, dt.ToShortTimeString());
					lbLoads2.Items.Insert(0, dt.ToShortTimeString());
				}

				//Load notes
				txtAdditionalNotes.Text = txtAdditionalNotes2.Text = wo.HQNotes;
				txtPilotNotes.Text = wo.PilotNotes;
				txtTemp.Text = wo.Temperature.ToString();

				//aerialWind1.WindSpeed = wo.WindSpeed;
				//aerialWind1.SetWindDirection(wo.WindDirection);
				SetWindSpeed(wo.WindSpeed);
				SetWindDirection(wo.WindDirection);

				UpdateFieldTotals();

				UpdateApplicationLoads();
			}
			else {
				//Clear the work order
				EnableFunctionality(false);
				EnableBasicFunctionality();
				lbClock.Items.Clear();
				lbLoads.Items.Clear();
				lbLoads2.Items.Clear();
				txtAdditionalNotes.Text = txtAdditionalNotes2.Text = txtPilotNotes.Text = "";
				lblSelectWorkOrder.Visible = true;
			}
		}

		private void UpdateApplicationLoads() {
			WorkOrder wo = _currentWorkOrder;
			pnlApplicationDetail.Visible = wo.ShowApplicationInformaion;
			txtAppTotal.Text = string.Format("{0} ac", wo.ApplicationTotal);
			txtAppRate.Text = string.Format("{0} {1}/ac", wo.ApplicationRate, wo.ApplicationUnitOfMeasure);
			txtAppLoad.Text = string.Format("{0}", wo.ApplicationLoads);
			txtAppAcPerLoad.Text = string.Format("{0} ac", wo.ApplicationAcresPerLoad);
			txtAppAmtPerLoad.Text = string.Format("{0} {1}", wo.ApplicationAmountPerLoad, wo.ApplicationUnitOfMeasure);

			UpdateApplicationLoadCount();
		}

		private void UpdateApplicationLoadCount() {
			lblLoadCount.Text = string.Format("Load Count: {0} / {1}", lbLoads.Items.Count, txtAppLoad.Text);
		}

		#region Clock
		#region Private Members
		private System.Collections.Generic.List<DateTime> _clockPunchList = new List<DateTime>();
		private Timer _confirmDeleteTimer = null;
		#endregion Private Members
		#region Event Handlers
		private void btnDeletePunch_Click(object sender, EventArgs e) {
			if (lbClock.SelectedIndex >= 0) {
				btnConfirmPunchDelete.Visible = true;
				_confirmDeleteTimer = new Timer();
				_confirmDeleteTimer.Interval = 10000; //10 seconds
				_confirmDeleteTimer.Tick += new EventHandler(_confirmDeleteTimer_Tick);
				_confirmDeleteTimer.Enabled = true;
			}
		}

		void _confirmDeleteTimer_Tick(object sender, EventArgs e) {
			((Timer)sender).Enabled = btnConfirmPunchDelete.Visible = false;
		}

		private void btnConfirmPunchDelete_Click(object sender, EventArgs e) {
			//Get the punch date time / time...
			object o = lbClock.SelectedItem;
			if (o != null) {
				try {
					DateTime stampToRemove = System.Convert.ToDateTime(o.ToString().Substring(5));
					_clockPunchList.Remove(stampToRemove);
					_currentWorkOrder.DeleteClockPunch(_applicationPilot, stampToRemove);
					RefreshClockPunchList();
				}
				catch { }
			}
			btnConfirmPunchDelete.Visible = false;
		}
		#endregion Event Handlers
		#region Logic
		private void RefreshClockPunchList() {
			lbClock.Items.Clear();
			_clockPunchList.Sort(); //Resort the list ... just in case (should not be needed)
			for (int i = 0; i < _clockPunchList.Count; i++) {
				lbClock.Items.Add(
				string.Format("{0} {1}",
					(i % 2 == 0) ? "Start" : "Stop",
					_clockPunchList[i]
					));
			}
		}
		#endregion Logic
		#endregion Clock

		#region UI - Basic Navigation Controls
		#region UI - Basic Navigation Controls - Event Handlers
		private void btnEnvironment_Click(object sender, EventArgs e) {
			ShowHidePanel(pnlEnvironment);
		}

		private void btnDetails_Click(object sender, EventArgs e) {
			ShowHidePanel(pnlDetails);
		}

		private void btnMap_Click(object sender, EventArgs e) {
			ShowHidePanel(pnlMap);
		}

		private void control_MouseUp(object sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Right) {
				ControlColorChooser ccc = new ControlColorChooser();
				ccc.Init(((Control)sender).ForeColor, ((Control)sender).BackColor);
				if (ccc.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					_controlColor.SetControlColor(GetControlHash((Control)sender), ccc.PickedForeColor, ccc.PickedBackColor);

					Aerial.db.Properties.Settings.Default.ControlColor = _controlColor.XmlAsString();
					Aerial.db.Properties.Settings.Default.Save();
					((Control)sender).ForeColor = ccc.PickedForeColor;
					((Control)sender).BackColor = ccc.PickedBackColor;
				}
			}
		}
		#endregion UI - Basic Navigation Controls - Event Handlers
		#region UI - Basic Navigation Controls - Logic
		private void ShowHidePanel(Panel Panel) {
			foreach (Panel p in new Panel[] { pnlEnvironment, pnlDetails, pnlMap }) {
				if (p.Equals(Panel)) {
					p.Visible = !p.Visible;
				}
				else
					p.Visible = false;
			}
			SetPanelButtonProperties();
		}


		private void SetPanelButtonProperties() {
			SetControlColor((Control)btnDetails, pnlDetails.Visible);
			SetControlColor((Control)btnEnvironment, pnlEnvironment.Visible);
			SetControlColor((Control)btnMap, pnlMap.Visible);
		}
		#endregion UI - Basic Navigation Controls - Logic
		#endregion UI - Basic Navigation Controls

		#region Environment Controls
		#region Environment Controls - Event Handlers
		private void btnTempMinus5_Click(object sender, EventArgs e) {
			SetTemp(-5);
		}

		private void btnTempMinus1_Click(object sender, EventArgs e) {
			SetTemp(-1);
		}

		private void btnTempPlus1_Click(object sender, EventArgs e) {
			SetTemp(1);
		}

		private void btnTempPlus5_Click(object sender, EventArgs e) {
			SetTemp(5);
		}

		private void btnWindDirection_Click(object sender, EventArgs e) {
			SetWindDirection(((Control)sender).Tag.ToString());
		}

		private void txtWindSpeed_TextChanged(object sender, EventArgs e) {
			_currentWorkOrder.SetWindSpeed(_applicationPilot, System.Convert.ToInt32(txtWindSpeed.Text));
		}

		private void btnWindSpeed_Clicked(object sender, EventArgs e) {
			try {
				SetWindSpeed(System.Convert.ToInt32(txtWindSpeed.Text) + System.Convert.ToInt32(((Control)sender).Text));
			}
			catch {
				SetWindSpeed(0);
			}
		}

		private void txtTemp_TextValueChanged(object sender, EventArgs e) {
			try {
				if (_currentWorkOrder != null)
					_currentWorkOrder.SetTemperature(_applicationPilot, System.Convert.ToInt32(txtTemp.Text));
			}
			catch { }
		}
		#endregion Environment Controls - Event Handlers
		#region Environment Controls - Logic
		private void SetWindSpeed(int Speed) {
			if (Speed < 0)
				Speed = 0;
			txtWindSpeed.Text = Speed.ToString();
		}

		public enum CompassDirection { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };
		private void SetWindDirection(string Direction) {
			string currentWindDirection = _currentWorkOrder.WindDirection;

			CompassDirection WindDirection = CompassDirection.North;

			Direction = Direction.ToUpper();
			switch (Direction) {
				case "NORTH":
				case "N":
					WindDirection = CompassDirection.North;
					break;
				case "NORTHEAST":
				case "NE":
					WindDirection = CompassDirection.NorthEast;
					break;
				case "EAST":
				case "E":
					WindDirection = CompassDirection.East;
					break;
				case "SOUTHEAST":
				case "SE":
					WindDirection = CompassDirection.SouthEast;
					break;
				case "SOUTH":
				case "S":
					WindDirection = CompassDirection.South;
					break;
				case "SOUTHWEST":
				case "SW":
					WindDirection = CompassDirection.SouthWest;
					break;
				case "WEST":
				case "W":
					WindDirection = CompassDirection.West;
					break;
				case "NORTHWEST":
				case "NW":
					WindDirection = CompassDirection.NorthWest;
					break;
				default:
					break;
			}
			if (currentWindDirection.ToUpper() != WindDirection.ToString().ToUpper())
				_currentWorkOrder.SetWindDirection(_applicationPilot, WindDirection.ToString());
			UpdateWindDirectionControls(WindDirection.ToString());
		}

		private void UpdateWindDirectionControls() {
			WorkOrder wo = _currentWorkOrder;
			if (wo != null)
				UpdateWindDirectionControls(wo.WindDirection);
		}

		private void UpdateWindDirectionControls(string Direction) {
			Direction = Direction.ToUpper();
			foreach (Button b in new Button[] { btnNorth, btnNorthEast, btnEast, btnSouthEast, btnSouth, btnSouthWest, btnWest, btnNorthWest }) {
				SetControlColor((Control)b, b.Tag.ToString().ToUpper() == Direction);
			}

		}

		private void SetTemp(int AdjustBy) {
			try {
				txtTemp.Text = (System.Convert.ToInt32(txtTemp.Text) + AdjustBy).ToString();
			}
			catch {
				txtTemp.Text = "80";
			}

		}
		#endregion Environment Controls - Logic
		#endregion Environment Controls

		#region Image Control
		#region Image Control - Event Handlers
		private void btnNextMap_Click(object sender, EventArgs e) {
			_imagePosition++;
		}

		private void btnPreviousMap_Click(object sender, EventArgs e) {
			_imagePosition--;
		}

		private void btnZoomOut_Click(object sender, EventArgs e) {
			AdjustPictureSize(-0.5);
		}

		private void btnZoomIn_Click(object sender, EventArgs e) {
			AdjustPictureSize(0.5);
		}


		private Point _panStartPoint = new Point();
		private void pbLargeMap_MouseDown(object sender, MouseEventArgs e) {
			_panStartPoint = new Point(e.X, e.Y);
		}

		private void pbLargeMap_MouseMove(object sender, MouseEventArgs e) {
			if (e.Button == System.Windows.Forms.MouseButtons.Left) {
				int x = _panStartPoint.X - e.X;
				int y = _panStartPoint.Y - e.Y;
				pnlLargePictureHolder.AutoScrollPosition = new Point(x - pnlLargePictureHolder.AutoScrollPosition.X, y - pnlLargePictureHolder.AutoScrollPosition.Y);
			}
		}

		private void pbLargeMap_DoubleClick(object sender, EventArgs e) {
			AdjustPictureSize(0.5);
		}
		#endregion Image Control - Event Handlers
		#region Image Control - Logic
		private double _imageZoom = 0;
		private double _initialImageZoom = 0;
		private void ChangeLargePicture(Image Image) {
			pbLargeMap.Image = Image;
			if (pbLargeMap.Image != null) {
				//TODO size for basic init
				_initialImageZoom = _imageZoom = 0;
				double ratio1 = pnlLargePictureHolder.Width / ((double)pbLargeMap.Image.Width);
				double ratio2 = pnlLargePictureHolder.Height / ((double)pbLargeMap.Image.Height);
				_initialImageZoom = (ratio1 < ratio2) ? ratio1 : ratio2;
				AdjustPictureSize(0);
			}

		}

		private void AdjustPictureSize(double Step) {
			_imageZoom += Step;
			if (_imageZoom < _initialImageZoom) //Prevent zooming out too far
				_imageZoom = _initialImageZoom;
			if (pbLargeMap.Image != null) {
				pbLargeMap.Width = (int)(pbLargeMap.Image.Width * _imageZoom);
				pbLargeMap.Height = (int)(pbLargeMap.Image.Height * _imageZoom);
			}
		}
		#endregion Image Control - Logic
		#endregion Image Control

		#region Work Order List
		#region Work Order List - Event Handlers
		private void btnHideCompleted_Click(object sender, EventArgs e) {
			ToggleHideCompleted();
		}

		private void lbWorkOrder_SelectedIndexChanged(object sender, EventArgs e) {
			LoadWorkOrder();
		}

		private void btnWOComplete_Click(object sender, EventArgs e) {
			//Set the work order state and set the "Complete" label as visible accordingly.
			txtWorkOrderComplete.Visible = _currentWorkOrder.SetWorkOrderComplete(_dal.RootPath, _applicationPilot, !_currentWorkOrder.Complete);

			//Refresh the list box
			//lbWorkOrder.Invalidate();

			if (_hideCompleteWorkOrders && lbWorkOrder.SelectedIndex >= 0)
				lbWorkOrder.Items.RemoveAt(lbWorkOrder.SelectedIndex);

			lbWorkOrder.Refresh();

		}
		#endregion Work Order List - Event Handlers
		#region Work Order List - Logic
		private void PopulateWorkOrders(bool QuickLoad) {
			lbWorkOrder.Items.Clear();
			foreach (WorkOrder wo in (QuickLoad) ? _dal.QuickWorkOrderList : _dal.WorkOrders)
				lbWorkOrder.Items.Add(wo);
			LoadWorkOrder(); //Force update of the work order
		}

		private bool _hideCompleteWorkOrders = false;
		private void ToggleHideCompleted() {
			if (!_hideCompleteWorkOrders) {
				_hideCompleteWorkOrders = true; //We are now hiding completed work orders

				//Remove the work orders from the list
				if (lbWorkOrder.Items.Count > 0) {
					for (int i = 0; i < lbWorkOrder.Items.Count; i++) {
						if (((WorkOrder)lbWorkOrder.Items[i]).Complete) {
							lbWorkOrder.Items.RemoveAt(i);
							i--;
						}
					}
				}
			}
			else {
				_hideCompleteWorkOrders = false;
				//We are now showing completed work orders

				object o = lbWorkOrder.SelectedItem;
				PopulateWorkOrders(true);
				lbWorkOrder.SelectedItem = o;
			}
		}
		#endregion Work Order List - Logic
		#endregion Work Order List

		#region Field
		#region Field - Event Handlers
		private void btnFieldUp_Click(object sender, EventArgs e) {
			NavigateField(-1);
		}

		private void btnFieldDown_Click(object sender, EventArgs e) {
			NavigateField(1);
		}

		private void NavigateField(int Position) {
			int rowIndex = dgFields.CurrentCell.RowIndex + Position;
			if (rowIndex >= 0 && rowIndex < dgFields.Rows.Count)
				dgFields.CurrentCell = dgFields.Rows[rowIndex].Cells["colFieldName"];
		}

		private void btnToggleField_Click(object sender, EventArgs e) {
			if (dgFields.SelectedRows != null) {
				foreach (DataGridViewRow dgvr in dgFields.SelectedRows) {
					bool valueToBe = _currentWorkOrder.SetFieldComplete(
						_applicationPilot,
						dgvr.Cells["colFieldName"].Value.ToString(),
						dgvr.Cells["colLatLong"].Value.ToString(),
						!(bool)dgvr.Cells["colCompleted"].Value,
						_dal.RootPath);
					dgvr.Cells["colCompleted"].Value = valueToBe;
					dgvr.Cells["colPilot"].Value = (valueToBe) ? _applicationPilot[0].ToString() : "";
					UpdateGridFont(dgvr.Index);
				}
				UpdateFieldTotals();
			}
		}
		#endregion Field - Event Handlers
		#region Field - Logic
		private void UpdateGridFont(int rowIndex) {
			bool strikeThrough = (bool)dgFields.Rows[rowIndex].Cells["colCompleted"].Value;
			for (int i = 0; i < dgFields.Columns.Count; i++) {
				if (dgFields.Rows[rowIndex].Cells[i].Style.Font == null)
					dgFields.Rows[rowIndex].Cells[i].Style.Font = dgFields.DefaultCellStyle.Font;
				dgFields.Rows[rowIndex].Cells[i].Style.Font = new Font(dgFields.Rows[rowIndex].Cells[i].Style.Font.FontFamily, dgFields.Rows[rowIndex].Cells[i].Style.Font.Size, (strikeThrough) ? FontStyle.Strikeout : FontStyle.Regular);
				dgFields.Rows[rowIndex].Cells[i].Style.ForeColor = (strikeThrough) ? Aerial.db.Properties.Settings.Default.FinishedForeColor : Aerial.db.Properties.Settings.Default.ControlForeColor;
			}
		}

		private void UpdateFieldTotals() {
			decimal complete, remaining, total;
			int fieldCount, fieldsCompleted;
			complete = remaining = total = 0;
			fieldCount = fieldsCompleted = 0;
			Aerial.db.dal.WorkOrderParsed.Fields[] fields = _currentWorkOrder.Fields;
			if (fields != null) {
				decimal area = 0;
				foreach (Aerial.db.dal.WorkOrderParsed.Fields field in _currentWorkOrder.Fields) {
					try {
						area = System.Convert.ToDecimal(field.Area);
					}
					catch {
						area = 0;
					}
					total += area;
					fieldCount++;
					if (field.Complete) {
						complete += area;
						fieldsCompleted++;
					}
					else
						remaining += area;
				}
			}
			lblAcresComplete.Text = string.Format("{0:f2}", complete);
			lblAcresRemaining.Text = string.Format("{0:f2}", remaining);
			;
			lblAcresTotal.Text = string.Format("{0:f2}", total);
			;
			lblFieldCount.Text = string.Format("Field Count: {0} / {1}", fieldsCompleted, fieldCount);
		}
		#endregion Field - Logic
		#endregion Field


		private void SetControlColor(Control Control, bool Selected) {
			if (Selected) {
				Control.BackColor = Properties.Settings.Default.SelectedBackColor;
				Control.ForeColor = Properties.Settings.Default.SelectedForeColor;
			}
			else {
				if (Control.GetType() == typeof(AerialButton)) {
					Control.BackColor = Properties.Settings.Default.ButtonBackColor;
					Control.ForeColor = Properties.Settings.Default.ButtonForeColor;
				}
				else {
					Control.BackColor = Properties.Settings.Default.ControlBackColor;
					Control.ForeColor = Properties.Settings.Default.ControlForeColor;
				}
			}
		}

		private void btnLoadComplete_Click(object sender, EventArgs e) {
			AddLoadTime();
		}

		private void AddLoadTime() {
			DateTime now = DateTime.Now;
			lbLoads.Items.Insert(0, now.ToShortTimeString());
			lbLoads2.Items.Insert(0, now.ToShortTimeString());
			_currentWorkOrder.AddLoadTime(_applicationPilot, now);
			UpdateApplicationLoadCount();
		}

		private void btnCopyField_Click(object sender, EventArgs e) {
			string[] files = _currentWorkOrder.ShapeFiles;
			if (files == null || files.Length == 0) {
				MessageBox.Show("Unable to copy Shape files.\r\nNo shape files found.", "Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else {
				//Find drive ...
				bool found = false;
				for (int i = (int)'D'; i <= (int)'Z'; i++) {
					if (System.IO.Directory.Exists(string.Format("{0}:\\", (char)i))) {
						foreach (string file in files)
							try {
								System.IO.File.Copy(file, string.Format("{0}:\\{1}", (char)i, System.IO.Path.GetFileName(file)), true);
								found = true;
							}
							catch { }
					}
				}
				if (!found) {
					MessageBox.Show("No external drives found.\r\nShape files were not copied.", "Drive not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else {
					MessageBox.Show("The shape files copied successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
		}

		private void MainForm_Shown(object sender, EventArgs e) {
			CreateTimer(true);
		}
	}
}