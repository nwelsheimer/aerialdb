﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;

namespace Aerial.db.dal
{
  public class WorkOrder
  {

    #region Private Members and variables
    private static string _growerPath = "Growers";
    private bool _complete = false;
    private DateTime _completeDate = Aerial.db.dal.Constants.INVALID_DATE;

    public bool Complete
    {
      get { return _complete; }
    }

    public bool Started
    {
      get
      {
        for (int i = 0; i < Fields.Length; i++)
        {
          if (Fields[i].Complete)
            return true;
        }
        return false;
      }
    }

    private bool _fastOpenOnly = false;
    public bool QuickLoad
    {
      get { return _fastOpenOnly; }
      set
      {
        _fastOpenOnly = false;
      }
    }

    private bool _loaded = false;
    public bool OriginalFilePathValid
    {
      get
      {
        return _originalFilePath != "" && System.IO.File.Exists(_originalFilePath);
      }
    }
    private string __originalFilePath = "";
    private string _originalFilePath
    {
      get { return __originalFilePath; }
      set
      {
        __originalFilePath = value;
        RefreshWorkOrder();
      }
    }
    public string OriginalFilePath
    {
      get { return _originalFilePath; }
    }

    public string AssignedPilot
    {
      get
      {
        try
        {
          return System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(_originalFilePath));
        }
        catch
        {
          return "";
        }
      }
    }

    private string _applicator = "";

    public string WorkingDirectory
    {
      get
      {
        string dir = "";
        if (OriginalFilePathValid)
        {
          try
          {
            dir = string.Format("{0}\\{1}", System.IO.Path.GetDirectoryName(_originalFilePath), System.IO.Path.GetFileNameWithoutExtension(_originalFilePath));
          }
          catch (Exception ex)
          {
            throw new Exception(string.Format("Original File Path::{0}::\r\n{1}", _originalFilePath, ex.ToString()));
          }
          if (!System.IO.Directory.Exists(dir))
            try
            {
              System.IO.Directory.CreateDirectory(dir);
            }
            catch { }
        }

        return dir;
      }
    }

    private const string PARSED_FILENAME = "parsed.xml";
    private const string EXTRA_FILENAME = "extra.xml";
    private const string SAVED_IMAGE_FILE_TEMPLATE = "image.png";
    private const string IMAGE_FILTER = "*.png";
    private string GetImageFileName(int position)
    {
      return string.Format("{0}\\{1}{2}.png", WorkingDirectory, System.IO.Path.GetFileNameWithoutExtension(SAVED_IMAGE_FILE_TEMPLATE), position, System.IO.Path.GetExtension(SAVED_IMAGE_FILE_TEMPLATE));
    }

    public string[] ShapeFiles
    {
      get
      {
        //return System.IO.Directory.GetFiles(WorkingDirectory, "*.shp");
        System.Collections.Generic.List<string> files = new List<string>();

        if (this.Fields != null)
        {
          foreach (WorkOrderParsed.Fields field in this.Fields)
          {
            files.AddRange(GetFieldShapes(field.Name, System.IO.Path.GetDirectoryName(WorkingDirectory)));
          }
        }
        return files.ToArray();

        //return System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(WorkingDirectory), "*.shp");
      }
    }

    private string[] GetFieldShapes(string FieldName, string SourcePath)
    {
      //Get field numbers ...
      string num = "";
      for (int i = 0; i < FieldName.Length; i++)
      {
        if (FieldName[i] >= '0' && FieldName[i] <= '9')
          num = string.Format("{0}{1}", num, FieldName[i]);
        if (FieldName[i] == ':' && num != "")
          break;
      }
      return System.IO.Directory.GetFiles(SourcePath, string.Format("*{0}.shp", num));
    }

    private string _parsedXMLFile
    {
      get
      {
        return string.Format("{0}\\{1}", WorkingDirectory, PARSED_FILENAME);
      }
    }


    public string[] ImageFileList
    {
      get { return System.IO.Directory.GetFiles(WorkingDirectory, IMAGE_FILTER); }
    }
    public System.Drawing.Image[] Images
    {
      get
      {
        string[] files = ImageFileList;
        System.Drawing.Image[] images = new System.Drawing.Image[files.Length];
        Array.Sort(files);
        for (int i = 0; i < files.Length; i++)
        {
          try
          {
            System.IO.FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read);
            images[i] = System.Drawing.Image.FromStream(fs);
            fs.Close();
          }
          catch { }
        }
        return images;
      }
    }
    private DateTime _piloteNotesDate = Aerial.db.dal.Constants.INVALID_DATE;
    private string _pilotNotes = "";
    public string PilotNotes
    {
      get { return _pilotNotes; }
    }

    private string _hqNotes = "";
    public string HQNotes
    {
      get { return _hqNotes; }
    }

    private System.Collections.Generic.List<dal.WorkOrderEntry.WorkOrderEntry> _pilotEntries = new List<WorkOrderEntry.WorkOrderEntry>();
    public System.Collections.Generic.List<dal.WorkOrderEntry.WorkOrderEntry> PilotEntries { get { return _pilotEntries; } }

    public string WorkOrderNumber
    {
      get
      {
        return _workOrderXML.Number == null ? "" : _workOrderXML.Number;
      }
    }

    public string Customer
    {
      get
      {
        return _workOrderXML.Customer == null ? "" : _workOrderXML.Customer;
      }
    }
    public string WorkOrderDate { get { return _workOrderXML.Date; } }
    public string TargetPests { get { return _workOrderXML.TargetPests; } }
    public WorkOrderParsed.Product[] ProductList
    {
      get
      {
        if (_workOrderXML.Products == null)
          return new WorkOrderParsed.Product[0];
        return _workOrderXML.Products;
      }
    }
    public WorkOrderParsed.Fields[] Fields
    {
      get
      {
        if (_workOrderXML.Fields == null)
          return new WorkOrderParsed.Fields[0];
        return _workOrderXML.Fields;
      }
    }

    public decimal _applicationTotal = 0;
    public decimal ApplicationTotal { get { return _applicationTotal; } }


    public decimal _applicationRate = 0;
    public decimal ApplicationRate { get { return _applicationRate; } }

    public string _applicationUnitOfMeasure = "";
    public string ApplicationUnitOfMeasure { get { return _applicationUnitOfMeasure; } }
    public decimal _applicationLoads = 0;
    public decimal ApplicationLoads { get { return _applicationLoads; } }
    public decimal _applicationAcresPerLoad = 0;
    public decimal ApplicationAcresPerLoad { get { return _applicationAcresPerLoad; } }
    public decimal _applicationAmountPerLoad = 0;
    public decimal ApplicationAmountPerLoad { get { return _applicationAmountPerLoad; } }

    public bool ShowApplicationInformaion
    {
      get
      {
        return ApplicationTotal != 0
            //|| ApplicationUnitOfMeasure != ""
            || ApplicationLoads != 0
            || ApplicationAcresPerLoad != 0
            || ApplicationAmountPerLoad != 0;
      }
    }

    private WorkOrderParsed.WorkOrderParsed _workOrderXML = new WorkOrderParsed.WorkOrderParsed();

    /// <summary>
    /// RTF formatting codes to detect pictures/images
    /// </summary>
    private string[] RTF_IMAGE_CODES = new string[] {
            @"\pict", @"\emfblip", @"\pngblip", @"\jpegblip",
            @"\macpict", @"\pmmetafile", @"\wmetafile",
            @"\dibitmap", @"\wbitmap", @"\wbmbitspixel",
            @"\wbmplanes", @"\wbmwidthbytes", @"\picw",
            @"\pich", @"\picwgoal", @"\pichgoal",
            @"\picscalex", @"\picscaley", @"\picscaled",
            @"\piccropt", @"\piccropb", @"\piccropr",
            @"\piccropl", @"\picbmp", @"\picbpp"
        };

    public string _pictureRawData = "";
    private string _UniqueID = "";
    public string UniqueID
    {
      get { return _UniqueID; }
    }
    #endregion Private Members and variables

    #region Public Members and Variables

    public static int SaveRetryMaxDelay = 5000; //In MS
    public static int SaveRetryDelay = 200; //In MS
    public static int SaveRetryCount = SaveRetryMaxDelay / SaveRetryDelay;
    #endregion Public Members and Variables

    #region Initialization
    public WorkOrder(string FilePath, string Applicator)
    {
      Init(FilePath, Applicator, false);
    }

    public WorkOrder(string FilePath, string Applicator, bool FastOpenOnly)
    {
      Init(FilePath, Applicator, FastOpenOnly);
    }

    private void Init(string FilePath, string Applicator, bool FastOpenOnly)
    {
      _fastOpenOnly = FastOpenOnly;
      _applicator = Applicator;
      if (System.IO.File.Exists(FilePath))
      {
        if (System.IO.File.Exists(string.Format("{0}\\{1}",
        System.IO.Path.GetDirectoryName(FilePath),
        System.IO.Path.GetFileNameWithoutExtension(FilePath))))
        {
          //Invalid work order -- A file already exists that would be the working directory
          Log.LogWriter.WriteSystem(string.Format("Invalid work order. The predicted working directory is consumed by a file.\r\nWork order: {0}", FilePath));
        }
        else
          _originalFilePath = FilePath;
      }
    }
    #endregion Initialization

    #region Simple Function Calls
    public bool IsValid() { return _loaded; }

    private string GetApplicatorXMLFile(string Applicator) { return string.Format("{0}\\{1}.xml", WorkingDirectory, Applicator.ToLower()); }

    private string GetExtraXMLFile() { return string.Format("{0}\\{1}", WorkingDirectory, EXTRA_FILENAME); }

    public override string ToString() { return (WorkOrderNumber == "") ? "Not Available" : WorkOrderNumber; }

    private WorkOrderEntry.WorkOrderEntry GetWorkOrderEntry(string Applicator) { return WorkOrderEntry.WorkOrderEntry.LoadXMLFromFile(GetApplicatorXMLFile(Applicator)); }
    private WorkOrderExtra.WorkOrderExtra GetWorkOrderExtra() { return WorkOrderExtra.WorkOrderExtra.LoadXMLFromFile(GetExtraXMLFile()); }
    #endregion Simple Function Calls

    #region Set Data Values
    /// <summary>
    /// Sets the Work Order Complete field and updates the pilot XML record
    /// </summary>
    /// <param name="Applicator"></param>
    /// <param name="Complete"></param>
    public bool SetWorkOrderComplete(string RootPath, string Applicator, bool Complete)
    {
      //Update field data
      foreach (WorkOrderParsed.Fields f in this.Fields)
      {
        if (!f.Complete)
        {
          AdjustCustomerFieldStatus(RootPath, Complete, f.Name, DateTime.Now);
        }
      }
      //Update Pilot Log Data
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      woe.Completed = new WorkOrderEntry.Completed();
      woe.Completed.Complete = Complete;
      woe.Completed.Stamp = new WorkOrderEntry.RecordStamp();
      woe.Completed.Stamp.DateRecorded = DateTime.Now;
      woe.Completed.Stamp.Pilot = Applicator;
      woe.SaveXML();
      _complete = Complete;
      _completeDate = DateTime.Now;
      return _complete;
    }

    /// <summary>
    /// Sets the work order complete field but does not update the pilot records. Use this when reading the XML data.
    /// </summary>
    /// <param name="Complete"></param>
    /// <param name="DateRecorded"></param>
    public void SetWorkOrderComplete(bool Complete, DateTime DateRecorded)
    {
      if (DateRecorded > _completeDate)
      {
        _completeDate = DateRecorded;
        _complete = Complete;
      }
    }

    public void SetEnvironment(string Applicator, int Temperature, string WindDirection, int WindSpeed)
    {
      //Update Pilot Log Data
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      if (woe.Environment == null)
        woe.Environment = new WorkOrderEntry.Environment();
      if (woe.Environment.Stamp == null)
        woe.Environment.Stamp = new WorkOrderEntry.RecordStamp();

      _environmentDate = DateTime.Now;
      _temperature = Temperature;
      _windDirection = WindDirection;
      _windSpeed = WindSpeed;

      woe.Environment.Temperature = _temperature;
      woe.Environment.WindDirection = _windDirection;
      woe.Environment.WindSpeed = _windSpeed;

      woe.Environment.Stamp.Pilot = Applicator;
      woe.Environment.Stamp.DateRecorded = _environmentDate;
      woe.SaveXML();
    }

    public void SetEnvironment(int Temperature, string WindDirection, int WindSpeed, DateTime DateRecorded)
    {
      if (DateRecorded > _environmentDate)
      {
        _environmentDate = DateRecorded;
        _temperature = Temperature;
        _windDirection = WindDirection;
        _windSpeed = WindSpeed;
      }
    }

    public void SetTemperature(string Applicator, int Temperature)
    {
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      if (woe.Environment == null)
        woe.Environment = new WorkOrderEntry.Environment();
      if (woe.Environment.Stamp == null)
        woe.Environment.Stamp = new WorkOrderEntry.RecordStamp();

      _environmentDate = DateTime.Now;
      _temperature = Temperature;

      woe.Environment.Temperature = _temperature;
      woe.Environment.WindDirection = _windDirection;
      woe.Environment.WindSpeed = _windSpeed;

      woe.Environment.Stamp.Pilot = Applicator;
      woe.Environment.Stamp.DateRecorded = _environmentDate;
      woe.SaveXML();
    }

    public void SetWindSpeed(string Applicator, int WindSpeed)
    {
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      if (woe.Environment == null)
        woe.Environment = new WorkOrderEntry.Environment();
      if (woe.Environment.Stamp == null)
        woe.Environment.Stamp = new WorkOrderEntry.RecordStamp();

      _environmentDate = DateTime.Now;
      _windSpeed = WindSpeed;

      woe.Environment.Temperature = _temperature;
      woe.Environment.WindDirection = _windDirection;
      woe.Environment.WindSpeed = _windSpeed;

      woe.Environment.Stamp.Pilot = Applicator;
      woe.Environment.Stamp.DateRecorded = _environmentDate;
      woe.SaveXML();
    }

    public void SetWindDirection(string Applicator, string WindDirection)
    {
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      if (woe.Environment == null)
        woe.Environment = new WorkOrderEntry.Environment();
      if (woe.Environment.Stamp == null)
        woe.Environment.Stamp = new WorkOrderEntry.RecordStamp();

      _environmentDate = DateTime.Now;
      _windDirection = WindDirection;

      woe.Environment.Stamp.Pilot = Applicator;
      woe.Environment.Stamp.DateRecorded = _environmentDate;
      woe.SaveXML();
    }

    public void SetHQNotes(string Notes, bool FromXML)
    {
      _hqNotes = Notes;
      if (!FromXML)
      {
        WorkOrderExtra.WorkOrderExtra woe = GetWorkOrderExtra();
        woe.HQNotes = Notes;
        woe.SaveXML();
      }
    }

    public void SetApplicationDetails(decimal AcresPerLoad, decimal AmountPerLoad, decimal Loads, decimal Rate, decimal Total, string UnitOfMeasure, bool FromXML)
    {
      _applicationAcresPerLoad = AcresPerLoad;
      _applicationAmountPerLoad = AmountPerLoad;
      _applicationLoads = Loads;
      _applicationRate = Rate;
      _applicationTotal = Total;
      _applicationUnitOfMeasure = UnitOfMeasure;

      if (!FromXML)
      {
        WorkOrderExtra.WorkOrderExtra woe = WorkOrderExtra.WorkOrderExtra.LoadXMLFromFile(GetExtraXMLFile());
        woe.ApplicationAcresPerLoad = AcresPerLoad;
        woe.ApplicationAmountPerLoad = AmountPerLoad;
        woe.ApplicationLoads = Loads;
        woe.ApplicationRate = Rate;
        woe.ApplicationTotal = Total;
        woe.ApplicationUnitOfMeasure = UnitOfMeasure;
        woe.SaveXML();
      }

    }

    public void SetPilotNotes(string Applicator, string Notes)
    {
      SetPilotNotes(Applicator, Notes, false);
    }

    public void SetPilotNotes(string Applicator, string Notes, bool FromXML)
    {
      _pilotNotes = Notes;
      if (!FromXML)
      {
        WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
        woe.PilotNotes = Notes;
        woe.SaveXML();
      }
    }

    public void SetProductFromCustomer(string Product, bool FromCustomer, bool FromXML)
    {
      foreach (WorkOrderParsed.Product p in ProductList)
      {
        if (p.Name.ToUpper() == Product.ToUpper())
        {
          if (p.CustomerSupplied == FromCustomer)
            return; //Nothing to do. the values are the same.
          p.CustomerSupplied = FromCustomer;
          break;
        }
      }
      if (!FromXML)
      {
        WorkOrderExtra.WorkOrderExtra woe = GetWorkOrderExtra();
        woe.SetProductState(Product, FromCustomer);
        woe.SaveXML();
      }

    }
    #endregion Set Data Values

    #region Loads
    private System.Collections.Generic.List<DateTime> _loadTimes = new List<DateTime>();
    public System.Collections.Generic.List<DateTime> LoadTimes
    {
      get { return _loadTimes; }
    }
    public void AddLoadTime(string Applicator, DateTime Time)
    {
      AddLoadTime(Applicator, Time, false);
    }

    public void AddLoadTime(string Applicator, DateTime Time, bool FromXML)
    {
      _loadTimes.Add(Time);
      if (!FromXML)
      {
        WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
        woe.AddLoadTime(Applicator, Time);
        woe.SaveXML();
      }
    }
    #endregion Loads

    #region Clock Punches
    private System.Collections.Generic.List<DateTime> _clockPunches = new List<DateTime>();
    public System.Collections.Generic.List<DateTime> ClockPunches
    {
      get { return _clockPunches; }
    }

    public void AddClockPunch(string Applicator, DateTime Punch)
    {
      AddClockPunch(Applicator, Punch, false);
    }

    public void AddClockPunch(string Applicator, DateTime Punch, bool FromXML)
    {
      _clockPunches.Add(Punch);
      if (!FromXML)
      {
        WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
        woe.AddClockPunch(Applicator, Punch);
        woe.SaveXML();
      }
    }

    public void DeleteClockPunch(string Applicator, DateTime Punch)
    {
      _clockPunches.Remove(Punch);
      //Update the XML
      WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
      woe.DeleteClockPunch(Punch);

      woe.SaveXML();

    }
    #endregion Clock Punches

    #region Helper Functions
    public void ClearPilotOnlyData()
    {
      _clockPunches.Clear();
      _loadTimes.Clear();
    }
    #endregion Helper Functions

    #region Field Control
    public bool SetFieldComplete(string Applicator, string Name, string LatLong, bool Complete, string RootPath)
    {
      return SetFieldComplete(Applicator, Name, LatLong, Complete, DateTime.Now, false, RootPath);
    }

    public bool SetFieldComplete(string Applicator, string Name, string LatLong, bool Complete, DateTime DateRecorded, bool FromXML, string RootPath)
    {
      for (int i = 0; i < Fields.Length; i++)
      {
        if (Fields[i].Name.ToUpper() == Name.ToUpper()
            && Fields[i].LatLong.ToUpper() == LatLong.ToUpper())
        {
          //Field was found
          bool retVal = Fields[i].SetComplete(Complete, DateRecorded, Applicator);
          if (!FromXML)
          {
            WorkOrderEntry.WorkOrderEntry woe = GetWorkOrderEntry(Applicator);
            woe.SetFieldComplete(Applicator, Name, LatLong, Complete, _temperature, _windDirection, _windSpeed);
            woe.SaveXML();
            AdjustCustomerFieldStatus(RootPath, Complete, Name, DateRecorded);
          }
          break;
        }
      }
      return Complete;
    }

    private static string _dateTimeFormat = "yyyy-MM-dd     HHmm";

    private void AdjustCustomerFieldStatus(string RootPath, bool Complete, string FieldName, DateTime DateRecorded)
    {
      string destinationPath = string.Format("{0}\\{1}\\{2}", RootPath, FileSupport.MakeValidDirectoryName(_growerPath), FileSupport.MakeValidDirectoryName(this.Customer));
      string uniqueID = string.Format("{0} ({1})", FileSupport.MakeValidFileName(FieldName), this.WorkOrderNumber);
      string destinationFile = string.Format("{0}\\{1}     {2:" + _dateTimeFormat + "}.txt", destinationPath, uniqueID, DateRecorded);
      if (!Complete)
      {
        try
        {
          foreach (string file in System.IO.Directory.GetFiles(destinationPath, string.Format("{0}*.txt", uniqueID)))
          {
            if (System.IO.File.Exists(file))
              System.IO.File.Delete(file);
          }
        }
        catch (Exception ex)
        {
          Aerial.db.dal.Email.SendEmail(ex.ToString(), Aerial.db.dal.Constants.DALONLYVERSION);
        }
      }
      else
      {
        //Create the file
        string body = "";
        for (int i = 0; i < ProductList.Length; i++)
          body = string.Format("{0}{1}\r\n", body, ProductList[i].Name);
        try
        {
          System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destinationFile));
          System.IO.File.WriteAllText(destinationFile, body);
        }
        catch (Exception ex)
        {
          Aerial.db.dal.Email.SendEmail(ex.ToString(), Aerial.db.dal.Constants.DALONLYVERSION);
        }
      }
    }

    public static void CleanCustomerFieldStatus(string RootPath, int Hours)
    {
      string path = string.Format("{0}\\{1}", RootPath, FileSupport.MakeValidDirectoryName(_growerPath));
      if (System.IO.Directory.Exists(path))
      {
        foreach (string file in System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
          try
          {
            //Parse the filename...
            string clean = System.IO.Path.GetFileNameWithoutExtension(file);
            clean = clean.Substring(clean.Length - _dateTimeFormat.Length);
            TimeSpan ts = DateTime.Now.Subtract(DateTime.ParseExact(clean, _dateTimeFormat, null));
            if (ts.TotalHours > Hours)
              System.IO.File.Delete(file);
          }
          catch (Exception ex)
          {
            Aerial.db.dal.Email.SendEmail(ex.ToString(), Aerial.db.dal.Constants.DALONLYVERSION);
          }
        }
      }
    }
    #endregion Field Control

    #region Environmnent
    private DateTime _environmentDate = Aerial.db.dal.Constants.INVALID_DATE;
    private int _temperature = 80;
    private string _windDirection = "North";
    private int _windSpeed = 0;

    public int Temperature
    {
      get { return _temperature; }
    }

    public string WindDirection
    {
      get { return _windDirection; }
    }

    public int WindSpeed
    {
      get { return _windSpeed; }
    }
    #endregion Environmnent

    public bool RefreshWorkOrder()
    {
      try
      {
        if (!OriginalFilePathValid)
          return false;
        DateTime startTime = DateTime.Now;
        bool refreshed = false;

        FileStream fs = null;
        try
        {
          fs = new FileStream(_originalFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
          if (fs.Length == 0) //File length is zero. Nothing to do. It is most likely in the process of being written/created.
            return false;
        }
        catch
        {
          return false; //Work order removed. Not sure how to flag this!
        }
        StreamReader sr = new StreamReader(fs);
        string MD5 = ComputeMD5(sr);
        //bool loadedFromXML = false;
        if (_workOrderXML.HashCode != MD5)
        {
          //Check XML - This may have been updated from another source
          WorkOrderParsed.WorkOrderParsed wop = WorkOrderParsed.WorkOrderParsed.LoadXMLFromFile(_parsedXMLFile);
          if (wop.HashCode == MD5)
          {
            _workOrderXML = wop;
            //loadedFromXML = true;
            _loaded = true;
          }
          else if (OriginalFilePath.ToLower().EndsWith(".rtf"))  //We only handle RTF if we are not in fast mode only
          {
            #region RTF Logic
            //We parse the RTF data.
            _workOrderXML.HashCode = MD5;
            refreshed = true;

            _workOrderXML.Number = "";
            _workOrderXML.Date = "";
            _workOrderXML.TargetPests = "";
            _workOrderXML.Products = new WorkOrderParsed.Product[0];
            _workOrderXML.Customer = "";

            //Continue to parse.
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            int pos1 = 0;
            int pos2 = 0;
            int pos3 = 0;
            int pos4 = 0;
            int pos5 = 0;
            int pos6 = 0;
            bool inPicture = false;
            bool inPictureData = false;
            bool pictureComplete = false;
            bool inProducts = false;
            bool inTargetPests = false;
            bool productComplete = false;
            int filePosition = 0;
            bool firstLine = true;
            if (!_fastOpenOnly)
            {
              _loaded = true;
              do
              {
                string line = sr.ReadLine();
                if (line == null)
                {
                  Aerial.db.dal.Email.SendEmail(
                      string.Format("Line read was null for file {0}.\r\nMD5: {1}\r\nReader Length/Position/CanRead: {2}/{3}/{4}",
                          _originalFilePath,
                          _workOrderXML.HashCode,
                          sr.BaseStream.Length,
                          sr.BaseStream.Position,
                          sr.BaseStream.CanRead),
                          Aerial.db.dal.Constants.DALONLYVERSION);
                  return false;
                  //throw new Exception(
                  //    string.Format(
                  //        "Line read was null for file {0}.\r\nMD5: {1}\r\nReader Length/Position/CanRead: {2}/{3}/{4}", 
                  //        _originalFilePath,
                  //        _workOrderXML.HashCode,
                  //        sr.BaseStream.Length,
                  //        sr.BaseStream.Position,
                  //        sr.BaseStream.CanRead
                  //        )
                  //    );
                }
                if (firstLine)
                {
                  //pos1 = line.LastIndexOf('\t');
                  //if (pos1 >= 0) {
                  //    pos2 = line.IndexOf('-', pos1);
                  //    if (pos2 < 0)
                  //        pos2 = line.IndexOf('\\', pos1);
                  //    if (pos2 >= 0)
                  //        _workOrderXML.Customer = line.Substring(pos1 + 1, pos2 - pos1 - 1).Trim();
                  //}
                  firstLine = false;
                }
                if (_workOrderXML.Customer == "")
                {
                  if (line.IndexOf(@"\pard\tx560\f1\fs18") >= 0)
                  {
                    pos1 = line.IndexOf("\t");
                    if (pos1 >= 0)
                    {
                      pos1++;
                      pos2 = line.IndexOf(@"\par", pos1);
                      if (pos2 >= 0)
                      {
                        _workOrderXML.Customer = line.Substring(pos1, pos2 - pos1).Trim();
                      }
                    }

                  }
                }
                filePosition += line.Length + 2;
                if (line.Contains('\\') || !pictureComplete)
                {
                  #region Logic
                  if (!inPicture && !inProducts)
                  {
                    if (_workOrderXML.Number == "")
                    {
                      pos1 = line.IndexOf("Work Order:", StringComparison.OrdinalIgnoreCase);
                      if (pos1 >= 0)
                      {
                        string match2 = "\\b0";
                        pos2 = line.IndexOf(match2, pos1 + 1);
                        if (pos2 >= 0)
                        {
                          pos2 += match2.Length;
                          pos3 = line.IndexOf('\\', pos2);
                          if (pos3 > pos2)
                            _workOrderXML.Number = line.Substring(pos2, pos3 - pos2).Trim();
                        }
                      }
                    }
                    if (_workOrderXML.Date == "")
                    {
                      pos1 = line.IndexOf("Date:", StringComparison.OrdinalIgnoreCase);
                      if (pos1 >= 0)
                      {
                        string match2 = "\\b0";
                        pos2 = line.IndexOf(match2, pos1 + 1);
                        if (pos2 >= 0)
                        {
                          pos2 += match2.Length;
                          pos3 = line.IndexOf('\\', pos2);
                          if (pos3 > pos2)
                            _workOrderXML.Date = line.Substring(pos2, pos3 - pos2).Trim();
                        }

                      }
                    }
                    if (_workOrderXML.TargetPests == "")
                    {
                      if (!inTargetPests)
                      {
                        inTargetPests = line.IndexOf("Target Pests", StringComparison.OrdinalIgnoreCase) >= 0;
                      }
                      else
                      { //This assumes pests are not on the same line
                        string match1 = "\\f0\\fs16";
                        pos1 = line.IndexOf(match1);
                        if (pos1 >= 0)
                        {
                          pos1 += match1.Length;
                          string match2 = "\\par";
                          //if (pos1 + match1.Length < line.Length) { //It could be possible that pos1 + match1.Length will exceed the end of the line.
                          pos2 = line.IndexOf(match2, pos1);
                          if (pos2 >= 0)
                          {
                            //pos2++;
                            //pos3 = line.IndexOf(@"\par", pos2);
                            string pests = line.Substring(pos1, pos2 - pos1).Trim();
                            _workOrderXML.TargetPests = (pests == "") ? "Not Specified" : pests;

                            //if (pos3 < 0)
                            //    pos3 = line.IndexOf('\t', pos2);
                            //if (pos3 > pos2)
                            //    _workOrderXML.TargetPests = line.Substring(pos2, pos3 - pos2);
                          }
                          //}
                        }
                      }
                    }
                  }
                  //                        else
                  //                        {
                  #region Picture Data
                  if (!pictureComplete)
                  {
                    pos1 = line.IndexOf(@"\pict", StringComparison.OrdinalIgnoreCase);
                    if (pos1 >= 0)
                    {
                      inPicture = true;
                      line = line.Substring(pos1);
                    }
                    if (inPicture)
                    {
                      //eat escape codes
                      if (!inPictureData)
                      {
                        bool found = false;
                        string t = line.ToLower();
                        foreach (string token in RTF_IMAGE_CODES)
                        {
                          if (t.Contains(token))
                          {
                            found = true;
                            break;
                          }
                        }
                        if (!found)
                        {
                          inPictureData = true;
                          sr.BaseStream.Seek(filePosition, SeekOrigin.Begin);
                          //long length = FindByteCountToChar(sr.BaseStream);
                          //if (length >= 0)
                          {
                            //char[] cStream = new char[length];
                            //sr.Read(cStream, (int)sr.BaseStream.Position, cStream.Length);
                            //sr.Read(cStream, 0, cStream.Length);
                            StringWriter sw = new StringWriter();
                            sw.Write(line);
                            //sw.Write(cStream, 0, cStream.Length);
                            while (sr.BaseStream.Position <= sr.BaseStream.Length)
                            {
                              char c = (char)sr.BaseStream.ReadByte();
                              if (c != '}')
                                sw.Write(c);
                              else
                                break;
                            }
                            //while (Stream.Position <= Stream.Length && Stream.ReadByte() != (int)'}') ;

                            //_pictureRawData = string.Format("{0}{1}", line, cStream);
                            _pictureRawData = sw.ToString();
                            //Write file.
                            SavePicture(_pictureRawData);
                            pictureComplete = true;
                            inPicture = false;
                            //filePosition = filePosition + _pictureRawData.Length - (line.Length + 2); //Update the file position
                            filePosition = (int)sr.BaseStream.Position; //Update the file position
                          }
                        }
                      }
                    }//(inPicture)
                  }
                  #endregion Picture Data
                  if (!productComplete && !inPicture)
                  {
                    if (!inProducts)
                    {
                      pos1 = line.IndexOf("Products", StringComparison.OrdinalIgnoreCase);
                      if (pos1 >= 0)
                        inProducts = true;
                    }
                    else
                    { //We ignore the line that contains "Products"
                      if (line.IndexOf("CropZone", StringComparison.OrdinalIgnoreCase) >= 0)
                        //inProducts = false;
                        productComplete = true;
                      else if (line.IndexOf("Name/EPA", StringComparison.OrdinalIgnoreCase) <= 0)
                      {
                        if (line.Contains(@"\pard\tx740\tqr\tx4028\tqr\tx4748\tqr\tx6368\tqr\tx7538\tqr\tx9608\tx1"))
                        {
                          pos2 = line.IndexOf('\t', 0);
                          if (pos2 >= 0)
                          {
                            pos2++;
                            pos3 = line.IndexOf('\t', pos2);
                            if (pos3 > pos2)
                            {
                              _workOrderXML.AddProduct(line.Substring(pos2, pos3 - pos2).Trim('/').Trim());
                            }
                          }
                        }
                      }
                    }
                  }
                  //if (productComplete)
                  //{
                  //Detect fields ...
                  //System.Text.RegularExpressions.Regex.Match(line, @"*'*\"*N'
                  if (System.Text.RegularExpressions.Regex.Match(line, ".*\'.*\".*N.*\'.*\".*W", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success
                      ||
                      line.Contains(@"\tx650\tx2899\tx4429\tx5869\tqr\tx9158\tqr\tx10238\tx1") //This is a dirty hack. Andersen Brothers was not properly displaying Lat/Long in the work order.
                                                                                               //This is an identified "fingerprint" for field lines.
                      )
                  {

                    pos1 = line.IndexOf("\t");
                    if (pos1 >= 0)
                    {
                      pos1 = line.IndexOf("\t", pos1 + 1);
                      if (pos1 >= 0)
                      {
                        pos1++;
                        pos2 = line.IndexOf("\t", pos1);
                        if (pos2 >= 0)
                        {
                          pos2++;
                          pos3 = line.IndexOf("\t", pos2);
                          if (pos3 >= 0)
                          {
                            pos3++;
                            pos4 = line.IndexOf("\t", pos3); //Eat another tab
                            if (pos4 >= 0)
                            {
                              pos4++;
                              pos5 = line.IndexOf("\t", pos4);
                              if (pos5 >= 0)
                              {
                                pos5++;
                                pos6 = line.IndexOf("\t", pos5);
                                if (pos6 >= 0)
                                {
                                  //pos4 = line.Substring(0, pos5).LastIndexOf("\t");
                                  //pos4 = line.IndexOf("\t", pos3);
                                  _workOrderXML.AddField(
                                      string.Format("{0}:{1}", line.Substring(pos1, pos2 - pos1).Trim(), line.Substring(pos2, pos3 - pos2).Trim()),

                                      //line.Substring(pos1, pos2 - pos1).Trim(),
                                      line.Substring(pos4, pos5 - pos4).Trim(),
                                      line.Substring(pos5, pos6 - pos5).Trim()
                                      );
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                  #endregion Logic
                }
              } while (!sr.EndOfStream);

              //Save the parsed data
              _workOrderXML.SaveXML(_parsedXMLFile);
            }
            #endregion RTF Logic
          }
          else if (OriginalFilePath.ToLower().EndsWith(".pdf"))
          {
            #region PDF Logic
            //Extract text
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(OriginalFilePath);
            string textLocation = string.Empty;
            string textSimple = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
              textLocation += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy());
              textSimple += iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy());
            }

            //BEGIN Common items to clear (Same in PDF and RTF -- Refactor)
            _workOrderXML.HashCode = MD5;
            refreshed = true;

            _workOrderXML.Number = "";


            _workOrderXML.TargetPests = "";
            _workOrderXML.Products = new WorkOrderParsed.Product[0];
            _workOrderXML.Customer = "";
            //END Common items to clear

            Match match = null; //RegEx match object to work against

            //Seeking and to populate:
            match = System.Text.RegularExpressions.Regex.Match(textLocation, "[a-zA-Z ]*\n");
            if (match.Success)
            {
              _workOrderXML.Customer = textLocation.Substring(match.Index, match.Length).Trim();
            }
            match = System.Text.RegularExpressions.Regex.Match(textLocation, "/[0-9]{4}\n[a-zA-Z0-9 ]*[0-9]+\n");
            if (match.Success)
            {
              _workOrderXML.Number = textLocation.Substring(match.Index + 5, match.Length - 5).Trim();
            }

            match = System.Text.RegularExpressions.Regex.Match(textLocation, "\n[0-9]+/[0-9]+/[0-9][0-9][0-9][0-9]\n");
            if (match.Success)
            {
              _workOrderXML.Date = textLocation.Substring(match.Index, match.Length).Trim(); //Remove new lice constants
            }

            //Rip out the header -- The date is the last thing standing...
            string startOfNonHeaderToMatch = "\nApplication Date & Time";
            System.Text.RegularExpressions.Match nonHeader = System.Text.RegularExpressions.Regex.Match(textLocation, startOfNonHeaderToMatch);
            if (nonHeader.Success)
              textLocation = textLocation.Replace(textLocation.Substring(0, nonHeader.Index), "");
            else //Operate against the Date match
              textLocation = textLocation.Replace(textLocation.Substring(0, match.Index + match.Length - 1), ""); //-1 as we do not want to rip out the new line character


            _workOrderXML.TargetPests = "Unknown"; //For the PDF reports, we do nothing here.

            #region Products
            //Works best against textSimple
            #region textLocation method
            //string productData = "";
            //match = System.Text.RegularExpressions.Regex.Match(textLocation, "\nProducts\n.*\nFields", System.Text.RegularExpressions.RegexOptions.Singleline); //Force single line mode -- it forces the consumption of new line chars
            //if (match.Success) {
            //	System.Text.RegularExpressions.Match localMatch = match;
            //	productData = textLocation.Substring(match.Index, match.Length);
            //	string originalCopyOfProductData = productData;
            //	while (localMatch.Success) { //First pass is always true from the productData gathering if() statement
            //		localMatch = System.Text.RegularExpressions.Regex.Match(productData, "\n.*[0-9] %.*\n");
            //		if (localMatch.Success) { //We have a new product to work against
            //			string productDetail = productData.Substring(localMatch.Index, localMatch.Length).Trim();
            //			_workOrderXML.AddProduct(productDetail.Substring(0, System.Text.RegularExpressions.Regex.Match(productDetail, "%").Index - 4).Trim()); //-3 to eat "100 "
            //			productData = productData.Substring(localMatch.Index + localMatch.Length - 1); //Remove the processed product, but do not remove the trailing newline char
            //		}
            //		//else false, we will exit the while loop
            //	}

            //	if (_workOrderXML.Products.Count() == 0) {
            //		localMatch = match; //Reset the match

            //		while (localMatch.Success) { //First pass is always true from the productData gathering if() statement
            //			localMatch = System.Text.RegularExpressions.Regex.Match(productData, "\n.*[0-9] .*\n");
            //			if (localMatch.Success) { //We have a new product to work against
            //				string productDetail = productData.Substring(localMatch.Index, localMatch.Length).Trim();

            //				//Default index
            //				int bestMatchIndex = System.Text.RegularExpressions.Regex.Match(productDetail, "[0-9]").Index;

            //				//If we can find a space before the first decimal point, this is a better match
            //				int decimalIndex = productDetail.IndexOf('.');
            //				if (decimalIndex > 0) { //It could be zero, but that would be the first char in the string
            //					int spaceIndex = productDetail.LastIndexOf(' ', decimalIndex);
            //					if (spaceIndex > 0) //It could be zero, but that would be the first char in the string
            //						if(spaceIndex > bestMatchIndex)
            //							bestMatchIndex = spaceIndex;
            //				}
            //				string product = productDetail.Substring(0, bestMatchIndex).Trim();

            //				string aerial = " AERIAL";
            //				if (product.ToUpper().EndsWith(aerial))
            //					product = product.Substring(0, product.Length - aerial.Length);

            //				_workOrderXML.AddProduct(product);
            //				productData = productData.Substring(localMatch.Index + localMatch.Length - 1); //Remove the processed product, but do not remove the trailing newline char
            //			}
            //			//else false, we will exit the while loop
            //		}
            //	}
            //}
            #endregion textLocation method

            #region textSimple method
            string productData = "";
            string productHeader = "App Method App Area Rate / ac. Total";
            match = System.Text.RegularExpressions.Regex.Match(textLocation, $"{productHeader}.*\nFields", System.Text.RegularExpressions.RegexOptions.Singleline); //Force single line mode -- it forces the consumption of new line chars
            if (!match.Success)
              match = System.Text.RegularExpressions.Regex.Match(textSimple, $"{productHeader}.*\n", System.Text.RegularExpressions.RegexOptions.Singleline); //Force single line mode -- it forces the consumption of new line chars
            if (match.Success)
            {
              System.Text.RegularExpressions.Match localMatch = match;
              productData = textLocation.Substring(match.Index + productHeader.Length, match.Length - productHeader.Length);
              string originalCopyOfProductData = productData;

              //							string[] separators = ;
              string[] productList = productData.Split(new string[] { "\n" }, StringSplitOptions.None);
              Regex regex = new Regex("\n.*ac");
              //foreach (string product in productList) {
              foreach (Match product in regex.Matches(productData))
              {
                /*if (product.EndsWith("\n")) {
                    string productToAdd = product;
                    //Valid product
                    string toSearch = "PHI(d)";
                    if (product.IndexOf(toSearch) > 0) {
                        //Make sure this is not a trailing entry -- Identified by "\nFields\n"
                        if (product.Contains("\nFields\n"))
                            break;
                        productToAdd  = product.Substring(product.IndexOf(toSearch) + toSearch.Length);
                        //Remove \n##\n
                        localMatch = System.Text.RegularExpressions.Regex.Match(productToAdd, "\n[0-9]*\n");
                        if (localMatch.Success && localMatch.Index < System.Text.RegularExpressions.Regex.Match(productToAdd, "\n[A-z][A-z]*.*\n").Index) //Numbers must exist before any letters
                            productToAdd = productToAdd.Substring(localMatch.Index + localMatch.Length);
                        else {
                            //Eat from beginning to next new line char
                            toSearch = "\n";
                            int index = productToAdd.IndexOf(toSearch);
                            if (index > 0) {
                                productToAdd = productToAdd.Substring(index + toSearch.Length);
                            }
                        }
                    }
                    productToAdd = productToAdd.Replace("*", ""); //Clean up
                    productToAdd = productToAdd.Replace("  ", " "); //Clean up
                    productToAdd = productToAdd.Replace("\n", " "); //Clean up
                    
                string productToAdd = product.Value.ToString();
                _workOrderXML.AddProduct(productToAdd.Trim());
              }*/
                string productToAdd = product.Value.ToString();
                _workOrderXML.AddProduct(productToAdd.Trim());
              }

            //while (localMatch.Success) { //First pass is always true from the productData gathering if() statement

            //	localMatch = System.Text.RegularExpressions.Regex.Match(productData, "\n.* ac. .*\n");
            //	if (localMatch.Success) { //We have a new product to work against
            //		string productDetail = productData.Substring(0, localMatch.Index).Trim();

            //		//Default index
            //		int bestMatchIndex = productDetail.Length;

            //		//Look for a number in the productDetail
            //		System.Text.RegularExpressions.Match productMatch = System.Text.RegularExpressions.Regex.Match(productDetail, "[0-9]");
            //		if (productMatch.Success)
            //			bestMatchIndex = productMatch.Index;

            //		//If we can find a space before the first decimal point, this is a better match
            //		int decimalIndex = productDetail.IndexOf('.');
            //		if (decimalIndex > 0) { //It could be zero, but that would be the first char in the string
            //			int spaceIndex = productDetail.LastIndexOf(' ', decimalIndex);
            //			if (spaceIndex > 0) //It could be zero, but that would be the first char in the string
            //				if (spaceIndex > bestMatchIndex)
            //					bestMatchIndex = spaceIndex;
            //		}
            //		string product = productDetail.Substring(0, bestMatchIndex).Trim();

            //		string aerial = " AERIAL";
            //		if (product.ToUpper().EndsWith(aerial))
            //			product = product.Substring(0, product.Length - aerial.Length);

            //		_workOrderXML.AddProduct(product);
            //		productData = productData.Substring(localMatch.Index + localMatch.Length - 1); //Remove the processed product, but do not remove the trailing newline char
            //	}
            //	//else false, we will exit the while loop
            //}
          }
          #endregion textSimple method

          #endregion Products
          //Fields
          string fieldData = "";
          match = System.Text.RegularExpressions.Regex.Match(textLocation, "\nFarm Fields Crop Zones Crop CENTER LAT/LONG WO. Area Actual Area\n.*\nSignature Date", System.Text.RegularExpressions.RegexOptions.Singleline);  //Force single line mode -- it forces the consumption of new line chars
          if (match.Success)
          {
            fieldData = textLocation.Substring(match.Index, match.Length);
            while (match.Success)
            { //First pass is always true from the fieldData gathering if() statement
              match = System.Text.RegularExpressions.Regex.Match(fieldData, "\n.*[0-9][0-9]\n"); //Grab the next set of field data
              if (match.Success == true)
              {
                string[] fieldDetail = fieldData.Substring(match.Index, match.Length).Trim().Split(' ');
                if (fieldDetail.Length >= 5) //Anything less is invalid
                  _workOrderXML.AddField(/*fieldDetail[fieldDetail.Length - 6]*/ string.Join(" ", fieldDetail, 0, fieldDetail.Length - 5), string.Format("{0} {1}", fieldDetail[fieldDetail.Length - 3], fieldDetail[fieldDetail.Length - 2]), fieldDetail[fieldDetail.Length - 1]);
                else //Flag possible error
                  _workOrderXML.AddField("INVALID", "CALL HQ", "");
                fieldData = fieldData.Substring(match.Index + match.Length - 1); //Remove the processed field, but do not remove the trailing newline char
              }
              //else false, we will exit the while loop
            }
          }

          List<System.Drawing.Image> images = new List<System.Drawing.Image>();
          for (int i = 1; i <= reader.NumberOfPages; i++)
          { //reader.GetPageN is NOT zero index based.
            images.AddRange(GetPDFImages(reader.GetPageN(i), reader));
          }

          //Close the PDF reader
          reader.Close();

          List<string> hashList = new List<string>();

          for (int i = 0; i < images.Count; i++)
          {
            string destFileName = GetImageFileName(i);
            images[i].Save(destFileName, System.Drawing.Imaging.ImageFormat.Png);
            string hash = ComputeMD5(destFileName);
            if (hashList.Contains(hash))
              System.IO.File.Delete(destFileName); //Duplicated image. Delete it.
            else
              hashList.Add(hash); //New image. Add its fingerprint to the list
          }

          //Save the parsed data
          _workOrderXML.SaveXML(_parsedXMLFile);
          #endregion PDF Logic
        }
      }

                sr.Close();
      //this._UniqueID = string.Format("{0}{1}", loadedFromXML, MD5);
      this._UniqueID = MD5;
      LoadSavedData();
      return refreshed;
    }
			catch (Exception ex) {
				//Unhandled error.
				//Log.LogWriter.WriteSystem(string.Format("Unable to process {0}. See Stack trace as follows: {1}", _originalFilePath, ex.ToString()));
				return false;
			}
}

#region Picture Helper Functions
private static List<System.Drawing.Image> GetPDFImages(PdfDictionary dict, iTextSharp.text.pdf.PdfReader reader)
{
  List<System.Drawing.Image> images = new List<System.Drawing.Image>();
  PdfDictionary res = (PdfDictionary)(PdfReader.GetPdfObject(dict.Get(PdfName.RESOURCES)));
  PdfDictionary xobj = (PdfDictionary)(PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT)));

  if (xobj != null)
  {
    foreach (PdfName name in xobj.Keys)
    {
      PdfObject obj = xobj.Get(name);
      if (obj.IsIndirect())
      {
        PdfDictionary tg = (PdfDictionary)(PdfReader.GetPdfObject(obj));
        PdfName subtype = (PdfName)(PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE)));
        if (PdfName.IMAGE.Equals(subtype))
        {
          int xrefIdx = ((PRIndirectReference)obj).Number;
          PdfObject pdfObj = reader.GetPdfObject(xrefIdx);
          PdfStream str = (PdfStream)(pdfObj);

          iTextSharp.text.pdf.parser.PdfImageObject pdfImage =
              new iTextSharp.text.pdf.parser.PdfImageObject((PRStream)str);
          System.Drawing.Image img = pdfImage.GetDrawingImage();

          images.Add(img);
        }
        else if (PdfName.FORM.Equals(subtype) || PdfName.GROUP.Equals(subtype))
        {
          images.AddRange(GetPDFImages(tg, reader)); //Some objects contain other objects. Recursive call.
        }
      }
    }
  }
  return images;
}

public static System.Drawing.Bitmap Crop(System.Drawing.Bitmap bmp)
{
  int w = bmp.Width;
  int h = bmp.Height;

  Func<int, bool> allWhiteRow = row =>
  {
    for (int i = 0; i < w; ++i)
      if (bmp.GetPixel(i, row).R != 255)
        return false;
    return true;
  };

  Func<int, bool> allWhiteColumn = col =>
  {
    for (int i = 0; i < h; ++i)
      if (bmp.GetPixel(col, i).R != 255)
        return false;
    return true;
  };

  int topmost = 0;
  for (int row = 0; row < h; ++row)
  {
    if (allWhiteRow(row))
      topmost = row;
    else
      break;
  }

  int bottommost = 0;
  for (int row = h - 1; row >= 0; --row)
  {
    if (allWhiteRow(row))
      bottommost = row;
    else
      break;
  }

  int leftmost = 0, rightmost = 0;
  for (int col = 0; col < w; ++col)
  {
    if (allWhiteColumn(col))
      leftmost = col;
    else
      break;
  }

  for (int col = w - 1; col >= 0; --col)
  {
    if (allWhiteColumn(col))
      rightmost = col;
    else
      break;
  }

  if (rightmost == 0)
    rightmost = w; // As reached left
  if (bottommost == 0)
    bottommost = h; // As reached top.

  int croppedWidth = rightmost - leftmost;
  int croppedHeight = bottommost - topmost;

  if (croppedWidth == 0) // No border on left or right
  {
    leftmost = 0;
    croppedWidth = w;
  }

  if (croppedHeight == 0) // No border on top or bottom
  {
    topmost = 0;
    croppedHeight = h;
  }

  try
  {
    var target = new System.Drawing.Bitmap(croppedWidth, croppedHeight);
    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
    {
      g.DrawImage(bmp,
        new System.Drawing.RectangleF(0, 0, croppedWidth, croppedHeight),
        new System.Drawing.RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
        System.Drawing.GraphicsUnit.Pixel);
    }
    return target;
  }
  catch (Exception ex)
  {
    throw new Exception(
      string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
      ex);
  }
}

//private long FindByteCountToChar(Stream Stream)
//{
//    long originalPosition = Stream.Position;
//    long byteCount = -1;

//    while(Stream.Position <= Stream.Length && Stream.ReadByte() != (int)'}');

//    if (Stream.Position <= Stream.Length)
//        byteCount = Stream.Position - originalPosition - 1;

//    //Stream.Position = originalPosition;
//    Stream.Seek(originalPosition, SeekOrigin.Begin);
//    return byteCount;
//}

public static byte[] ConvertHexStringToByteArray(string hexString)
{
  if (hexString.Length % 2 != 0)
  {
    //hexString = string.Format("{0}0", hexString);
    throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
  }

  byte[] HexAsBytes = new byte[hexString.Length / 2];
  for (int index = 0; index * 2 < hexString.Length; index++)
  {
    string byteValue = "";
    try
    {
      byteValue = hexString.Substring(index * 2, 2);
    }
    catch (Exception ex)
    {
      int i = 0;
      i++;
    }
    HexAsBytes[index] = byte.Parse(byteValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
  }

  return HexAsBytes;
}

private void SavePicture(string Data)
{
  byte[] array = ConvertHexStringToByteArray(Data.Trim().Replace("\r\n", ""));
  System.IO.MemoryStream ms = new MemoryStream();
  //System.IO.StreamWriter sw = new StreamWriter(ms);
  //sw.Write(array, );
  ms.Write(array, 0, array.Length);
  //sw.Flush();
  ms.Seek(0, 0);
  System.Drawing.Image i = System.Drawing.Image.FromStream(ms);
  ms = new MemoryStream();
  i.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
  ms.Seek(0, 0);
  System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms);
  bmp = Crop(bmp);
  bmp.Save(GetImageFileName(0), System.Drawing.Imaging.ImageFormat.Png);

  //i.Save(_imageFile, System.Drawing.Imaging.ImageFormat.Png);

  //System.IO.FileStream fs = new System.IO.FileStream(_imageFile, System.IO.FileMode.Create);
  //fs.Write(array, 0, array.Length);
  //fs.Close();
}
#endregion Picture Helper Functions

private string ComputeMD5(string FileName)
{
  StreamReader sr = new StreamReader(FileName);
  string MD5 = ComputeMD5(sr);
  sr.Close();
  return MD5;
}

private string ComputeMD5(StreamReader Stream)
{
  Stream.BaseStream.Seek(0, SeekOrigin.Begin);
  return Convert.ToBase64String(System.Security.Cryptography.MD5.Create().ComputeHash(Stream.BaseStream));
}

private void LoadSavedData()
{
  //Clear the pilot only data (clock punchs and loads)
  ClearPilotOnlyData();
  _pilotEntries.Clear();

  foreach (string s in System.IO.Directory.GetFiles(WorkingDirectory, "*.xml"))
  {
    this._UniqueID = string.Format("{0}{1}", _UniqueID, ComputeMD5(s));
    if (System.IO.Path.GetFileName(s).ToUpper() != PARSED_FILENAME.ToUpper())
    {
      if (System.IO.Path.GetFileName(s).ToUpper() == EXTRA_FILENAME.ToUpper())
      {
        WorkOrderExtra.WorkOrderExtra woe = WorkOrderExtra.WorkOrderExtra.LoadXMLFromFile(s);
        if (woe.Products != null)
        {
          foreach (WorkOrderExtra.Product p in woe.Products)
          {
            SetProductFromCustomer(p.Name, p.CustomerSupplied, true);
          }
        }
        SetHQNotes(woe.HQNotes, true);
        SetApplicationDetails(woe.ApplicationAcresPerLoad, woe.ApplicationAmountPerLoad, woe.ApplicationLoads, woe.ApplicationRate, woe.ApplicationTotal, woe.ApplicationUnitOfMeasure, true);
      }
      else
      {
        //Apply values
        WorkOrderEntry.WorkOrderEntry woe = WorkOrderEntry.WorkOrderEntry.LoadXMLFromFile(s);
        _pilotEntries.Add(woe);
        string fileApplicator = System.IO.Path.GetFileNameWithoutExtension(s);

        if (woe.ClockPunches != null)
        {
          foreach (WorkOrderEntry.ClockPunches cp in woe.ClockPunches)
          {
            if (cp.Stamp.Pilot.ToLower() == _applicator.ToLower())
              AddClockPunch(_applicator, cp.Date, true);
          }
        }
        if (woe.Loads != null && _applicator.ToLower() == fileApplicator)
        {//Only load the current applicators load times.
          foreach (DateTime dt in woe.Loads)
            AddLoadTime(fileApplicator, dt, true);
        }
        if (woe.Completed != null && woe.Completed.Stamp != null)
        {
          SetWorkOrderComplete(woe.Completed.Complete, woe.Completed.Stamp.DateRecorded);
        }
        if (woe.Environment != null)
        {
          if (woe.Environment.Stamp != null)
            SetEnvironment(woe.Environment.Temperature, woe.Environment.WindDirection, woe.Environment.WindSpeed, woe.Environment.Stamp.DateRecorded);
        }
        if (woe.Fields != null)
        {
          foreach (WorkOrderEntry.Fields field in woe.Fields)
          {
            if (field.Stamp != null)
            {
              SetFieldComplete(fileApplicator, field.Name, field.LatLong, field.Completed, field.Stamp.DateRecorded, true, "");
            }
          }

        }
        if (woe.PilotNotes != null)
        {
          if (fileApplicator == _applicator.ToLower())
            SetPilotNotes(_applicator, woe.PilotNotes, true);
        }
      }
    }
  }
}
	}
}