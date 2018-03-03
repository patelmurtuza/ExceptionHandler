using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ExceptionHandler
{
    public sealed class ExceptionWriteLog
    {
        #region Global Variables

        private const string ERR_MESSAGE_CREATION = "Error in message creation";
        private const string ERR_FILE_NAME_GENERATION = "Error in file name generation";
        private string strGlobalErrSourcePath = "";

        private static ExceptionWriteLog m_instance;
        private static object syncRoot = new Object();

        private string _error_fileName = string.Empty, _error_methodName = string.Empty, _error_lineNo = string.Empty, _error_Source = string.Empty, _error_InnerException = string.Empty, _error_Exception = string.Empty;

        #endregion Global Variables

        #region Getter Setter public properties

        public static ExceptionWriteLog Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (m_instance == null)
                        {
                            m_instance = new ExceptionWriteLog();
                        }
                    }
                }

                return m_instance;
            }
        }

        public string Error_FileName
        {
            get { return _error_fileName; }
            set { _error_fileName = value; }
        }

        public string Error_LineNo
        {
            get { return _error_lineNo; }
            set { _error_lineNo = value; }
        }

        public string Error_Source
        {
            get { return _error_Source; }
            set { _error_Source = value; }
        }

        public string Error_MethodName
        {
            get { return _error_methodName; }
            set { _error_methodName = value; }
        }

        public string Error_InnerException
        {
            get { return _error_InnerException; }
            set { _error_InnerException = value; }
        }

        public string Error_Exception
        {
            get { return _error_Exception; }
            set { _error_Exception = value; }
        }

        #endregion Getter Setter public properties


        public ExceptionWriteLog()
        {
            strGlobalErrSourcePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName + "\\Logs";
        }


        public void LogException(ExceptionMessage ExceptionMsg, string sMediaKey, Exception exception)
        {
            try
            {
                if (sMediaKey.ToUpper().Trim().Equals("TXT"))
                    WriteLogFile(exception, ExceptionMsg);
                else if (sMediaKey.ToUpper().Trim().Equals("EV"))
                    MachineLog(exception);
                else if (sMediaKey.ToUpper().Trim().Equals("DB"))
                {
                }

            }
            catch
            {
                throw;
            }
        }


        public void MachineLog(Exception exception)
        {
            try
            {
                if (!EventLog.SourceExists(Error_Source))
                {
                    EventLog.CreateEventSource(Error_Source, "PARIS");
                    Console.WriteLine("PARIS Application Log");
                }
                string message = "**********<DateTime:" + DateTime.Now.ToString() + ">*********\r\n" + "File Name: " + Error_FileName + "\r\n" + "Method Name: " + exception.TargetSite.Name.ToString() + "\r\n\n" + "Line No: " + Error_LineNo + "\r\nException: " + exception.Message.ToString() + "\r\n*******************************************************************************";

                EventLog myLog = new EventLog();
                myLog.Source = Error_Source;

                myLog.WriteEntry(message, EventLogEntryType.Error);
            }
            catch
            {
                throw;
            }
        }

        private void ExtractExceptionInformation(Exception ex)
        {
            try
            {
                #region To Extract FileName

                string strfname = string.Empty, filename = string.Empty;
                if (Convert.ToString(ex.StackTrace) != "")
                {
                    string _strTemp = ex.StackTrace.ToString();
                    string[] strword = _strTemp.Split(':');
                    Error_LineNo = strword[strword.Length - 1];

                    strfname = strword[strword.Length - 2];
                    char[] delimiters = new char[] { '\\' };

                    string[] strfilename = strfname.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    Error_FileName = strfilename[strfilename.Length - 1];

                    Error_Source = Convert.ToString(ex.Source);

                    Error_MethodName = Convert.ToString(ex.TargetSite.Name);

                    Error_InnerException = Convert.ToString(ex.InnerException);

                    Error_Exception = Convert.ToString(ex.Message);
                }

                #endregion To Extract FileName
            }
            catch (Exception ex1)
            {
                ex1.Message.ToString();
                throw;
            }
        }

        public void WriteLogFile(Exception exception, ExceptionMessage ExceptionMsg)
        {
            try
            {
                if (!string.IsNullOrEmpty(exception.Message))
                {
                    if (!Directory.Exists(strGlobalErrSourcePath))
                    {
                        Directory.CreateDirectory(strGlobalErrSourcePath);
                    }
                    string fileName = "Error Log " + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";

                    string FILE_NAME = strGlobalErrSourcePath + "\\" + fileName;
                    string TextLine = string.Empty;
                    if (System.IO.File.Exists(FILE_NAME) == true)
                    {
                        System.IO.StreamReader objReader = new System.IO.StreamReader(FILE_NAME);
                        while (objReader.Peek() != -1)
                        {
                            TextLine = TextLine + objReader.ReadToEnd();
                        }
                        TextLine = TextLine + Convert.ToString(CreateExceptionMessage(exception, ExceptionMsg));
                        objReader.Close();

                        System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME);
                        objWriter.WriteLine(TextLine);
                        objWriter.Close();
                    }
                    else
                    {
                        FileStream file = new FileStream(strGlobalErrSourcePath + "\\" + fileName, FileMode.OpenOrCreate, FileAccess.Write);
                        using (StreamWriter streamWriter = new StreamWriter(file))
                        {
                            streamWriter.WriteLine(CreateExceptionMessage(exception, ExceptionMsg));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw new Exception(ERR_FILE_NAME_GENERATION);
            }
        }


        private StringBuilder CreateExceptionMessage(Exception ex, ExceptionMessage ExceptionMsg)
        {
            StringBuilder objStr = null;

            StringBuilder objStrMessage = null;
            StringBuilder objStrIExp = null;
            StringBuilder objStrSTrace = null;
            StringBuilder objStrSource = null;
            StringBuilder objStrTSite = null;
            StringBuilder objStrTMsg = null;
            string strLineNo = string.Empty;

            try
            {
                ExtractExceptionInformation(ex);

                objStr = new StringBuilder();

                objStrMessage = new StringBuilder();
                objStrIExp = new StringBuilder();
                objStrSTrace = new StringBuilder();
                objStrSource = new StringBuilder();
                objStrTSite = new StringBuilder();
                objStrTMsg = new StringBuilder();

                string errorDateTime = DateTime.Now.ToString("MM-dd-yyyy hh:mm:ss");
                objStrMessage.AppendLine("=========================   ERROR LOG AT Date(MM-dd-yyyy hh:mm:ss) :-  " + errorDateTime + "   =========================");

                objStrIExp.AppendLine("--------------------------------");
                objStrMessage.AppendLine("PARIS Message ==> ");
                objStrMessage.AppendLine(Convert.ToString(ExceptionMsg.LastMessage));
                objStrMessage.AppendLine("--------------------------------");

                objStrIExp.AppendLine("File Name ==> ");
                objStrIExp.AppendLine(Convert.ToString(Error_FileName));
                objStrIExp.AppendLine("--------------------------------");

                objStrIExp.AppendLine("--------------------------------");
                objStrIExp.AppendLine("Method Name ==> ");
                objStrIExp.AppendLine(Convert.ToString(ex.TargetSite.Name.ToString()));
                objStrIExp.AppendLine("--------------------------------");

                objStrIExp.AppendLine("--------------------------------");
                objStrIExp.AppendLine("Line No ==> ");
                objStrIExp.AppendLine(Convert.ToString(Error_LineNo));
                objStrIExp.AppendLine("--------------------------------");

                objStrIExp.AppendLine("--------------------------------");
                objStrMessage.AppendLine("Message ==> ");
                objStrMessage.AppendLine(Convert.ToString(ex.Message));
                objStrMessage.AppendLine("--------------------------------");

                objStrIExp.AppendLine("--------------------------------");
                objStrIExp.AppendLine("Inner Exception ==> ");
                objStrIExp.AppendLine(Convert.ToString(ex.InnerException));
                objStrIExp.AppendLine("--------------------------------");

                objStrSTrace.AppendLine("--------------------------------");
                objStrSTrace.AppendLine("Stack Trace ==> ");
                objStrSTrace.AppendLine(Convert.ToString(ex.StackTrace));
                objStrSTrace.AppendLine("--------------------------------");

                objStrSource.AppendLine("--------------------------------");
                objStrSource.AppendLine("Source ==> ");
                objStrSource.AppendLine(Convert.ToString(ex.Source));

                objStrTSite.AppendLine("==============================================================");

                objStrTMsg.AppendLine("--------------------------------");
                objStrTMsg.AppendLine("PARIS Message History ==>");
                foreach (var item in ExceptionMsg.Messages)
                {
                    objStrTMsg.AppendLine(item);
                }
                objStrTMsg.AppendLine("--------------------------------");
            }
            catch
            {
                throw new Exception(ERR_MESSAGE_CREATION);
            }
            finally
            {
                objStr.Append(objStrMessage);
                objStr.Append(objStrIExp);
                objStr.Append(objStrSTrace);
                objStr.Append(objStrSource);
                objStr.Append(objStrTMsg);
                objStr.Append(objStrTSite);
            }

            return objStr;
        }


        public string DeleteOldExceptionLogs(bool allExceptionlogs)
        {
            string msg = string.Empty;
            try
            {
                if (allExceptionlogs)
                {
                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(strGlobalErrSourcePath);

                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                    {
                        file.Delete();
                        msg = " Exception Log Files Delete successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            return msg;
        }


        public string DeleteOldExceptionLogs(string startDate, string endDate)
        {
            string msg = string.Empty;
            DateTime dtout;
            try
            {
                if (DateTime.TryParse(startDate, out dtout) && DateTime.TryParse(endDate, out dtout))
                {
                    DateTime sdate = Convert.ToDateTime(startDate);
                    DateTime edate = Convert.ToDateTime(endDate);

                    System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(strGlobalErrSourcePath);

                    foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                    {
                        DateTime dt = file.CreationTime.Date;
                        if (dt.Date >= sdate.Date && dt.Date <= edate.Date)
                        {
                            file.Delete();
                            msg = " Exception Log Files Deleted successfully";
                        }
                    }
                }
                else
                {
                    msg = "Date Entered is Not a Valid date";
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
            return msg;
        }


        public string LogToDatabase(Exception ex, string ConnectionString, string DBProvider = "")
        {
            string result = string.Empty;
            if (DBProvider.ToUpper().Equals("ORACLE"))
            {
                //tbd
            }
            else if (DBProvider.ToUpper().Equals("SQL"))
            {
                //tbd
            }
            return result.ToString();
        }

    }
}