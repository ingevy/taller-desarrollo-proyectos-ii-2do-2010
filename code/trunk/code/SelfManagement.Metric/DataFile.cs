namespace CallCenter.SelfManagement.Metric
{
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    class DataFile : IDataFile
    {
        private ExternalSystemFiles externalSystemFile;
        private DateTime fileDate;
        private string filePath;

        public DataFile(ExternalSystemFiles externalSystemFile, DateTime fileDate, string filePath)
        {
            this.externalSystemFile = externalSystemFile;
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
