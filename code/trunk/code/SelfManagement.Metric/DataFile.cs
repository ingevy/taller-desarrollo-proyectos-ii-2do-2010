namespace CallCenter.SelfManagement.Metric
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using CallCenter.SelfManagement.Metric.Interfaces;

    public class DataFile : IDataFile
    {
        private ExternalSystemFiles externalSystemFile;
        private DateTime fileDate;
        private string filePath;

        public static ExternalSystemFiles GetDataFileTypeByFileName(string fileName)
        {
            var dateStrRegex = "[1-9][0-9]{3}[0-1][[0-9][0-3][0-9]";
            var summaryRegex = new Regex("^Summary_" + dateStrRegex + ".csv$");
            var ttsRegex = new Regex("^TTS_" + dateStrRegex + ".csv$");
            var qaRegex = new Regex("^QA_" + dateStrRegex + ".csv$");
            var stsRegex = new Regex("^STS_[Enero|Febrero|Marzo|Abril|Mayo|Junio|Julio|Agosto|Septiembre|Octubre|Noviembre|Diciembre][1-9][0-9]{3}.csv$");

            if (summaryRegex.IsMatch(fileName)) { return ExternalSystemFiles.SUMMARY; }
            if (ttsRegex.IsMatch(fileName)) { return ExternalSystemFiles.TTS; }
            if (qaRegex.IsMatch(fileName)) { return ExternalSystemFiles.QA; }
            if (stsRegex.IsMatch(fileName)) { return ExternalSystemFiles.STS; }

            throw new System.ArgumentException("Unexpected file name");
        }

        public DataFile(DateTime fileDate, string filePath)
        {
            this.externalSystemFile = DataFile.GetDataFileTypeByFileName(Path.GetFileName(filePath));
            this.fileDate = fileDate;
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
                    throw new ApplicationException(e.Message);
                }

                return parsedData;
            }
        }
    }
}
