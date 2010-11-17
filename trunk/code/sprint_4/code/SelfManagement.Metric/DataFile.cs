namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System.Globalization;

    public class DataFile : IDataFile
    {
        private ExternalSystemFiles externalSystemFile;
        private DateTime fileDate;
        private string filePath;

        public static ExternalSystemFiles GetDataFileType(string fileName)
        {
            var dateStrRegex = "[1-9][0-9]{3}[0-1][[0-9][0-3][0-9]";
            var summaryRegex = new Regex("^Summary_" + dateStrRegex + ".csv$");
            var ttsRegex = new Regex("^TTS_" + dateStrRegex + ".csv$");
            var qaRegex = new Regex("^QA_" + dateStrRegex + ".csv$");
            //var stsRegex = new Regex("^STS_(Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre)[1-9][0-9]{3}.csv$");
            var stsRegex = new Regex("^STS_" + dateStrRegex + ".csv$");
            var hfRegex = new Regex("^HF System.csv$");

            if (summaryRegex.IsMatch(fileName)) { return ExternalSystemFiles.SUMMARY; }
            if (ttsRegex.IsMatch(fileName)) { return ExternalSystemFiles.TTS; }
            if (qaRegex.IsMatch(fileName)) { return ExternalSystemFiles.QA; }
            if (stsRegex.IsMatch(fileName)) { return ExternalSystemFiles.STS; }
            if (hfRegex.IsMatch(fileName)) { return ExternalSystemFiles.HF; }

            throw new System.ArgumentException("Unexpected file name");
        }

        public static DateTime GetDataFileDate(ExternalSystemFiles type, string fileName)
        {
            DateTime fileDate;

            switch (type)
            {
                case ExternalSystemFiles.QA:
                    fileDate = DateTime.ParseExact(fileName.Substring(3, 8),"yyyyMMdd",null);
                    break;
                case ExternalSystemFiles.TTS:
                    fileDate = DateTime.ParseExact(fileName.Substring(4, 8), "yyyyMMdd", null);
                    break;
                case ExternalSystemFiles.SUMMARY:
                    fileDate = DateTime.ParseExact(fileName.Substring(8, 8), "yyyyMMdd", null);
                    break;
                /*case ExternalSystemFiles.STS:
                    fileDate = DateTime.ParseExact("01" + fileName.Substring(4, fileName.IndexOf(".") - 4), "ddMMMMyyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                    break;*/
                case ExternalSystemFiles.STS:
                    fileDate = DateTime.ParseExact(fileName.Substring(4, 8), "yyyyMMdd", null);
                    break;
                case ExternalSystemFiles.HF:
                    fileDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
                    break;
                default:
                    fileDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    break;
            }

            return fileDate;
        }

        public DataFile(string filePath)
        {
            this.externalSystemFile = DataFile.GetDataFileType(Path.GetFileName(filePath));
            this.fileDate = DataFile.GetDataFileDate(this.externalSystemFile, Path.GetFileName(filePath));
            this.filePath = filePath;
        }

        public ExternalSystemFiles ExternalSystemFile
        {
            get { return this.externalSystemFile; }
        }

        public System.DateTime FileDate
        {
            get { return this.fileDate; }
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        public IList<Dictionary<string,string>> DataLines
        {
            get
            {
                IList<Dictionary<string, string>> parsedData = new List<Dictionary<string,string>>();

                try
                {
                    using (StreamReader readFile = new StreamReader(this.FilePath))
                    {
                        string line;
                        string[] columns = null;
                        string[] row;
                        bool isFirstLine = true;

                        while ((line = readFile.ReadLine()) != null)
                        {
                            if (isFirstLine)
                            {
                                columns = line.Split(',');
                                isFirstLine = false;
                            }
                            else
                            {
                                row = line.Split(',');
                                var lineValues = new Dictionary<string, string>();
                                for (var i = 0; i < row.Length; i++)
                                {
                                    lineValues.Add(columns[i], row[i]);
                                }
                                parsedData.Add(lineValues);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.Message);
                }

                return parsedData;
            }
        }
    }
}
