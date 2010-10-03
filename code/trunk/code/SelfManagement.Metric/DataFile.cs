namespace CallCenter.SelfManagement.Metric
{
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    class DataFile : IDataFile
    {
        private string externalSystemName;
        private DateTime fileDate;
        private string filePath;

        public DataFile(string externalSystemName, DateTime fileDate, string filePath)
        {
            this.externalSystemName = externalSystemName;
            this.fileDate = fileDate;
            this.filePath = filePath;
        }

        public string ExternalSystemName
        {
            get { return this.externalSystemName; }
        }

        public System.DateTime FileDate
        {
            get { return this.fileDate; }
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        public List<string[]> DataLines
        {
            get
            {
                List<string[]> parsedData = new List<string[]>();

                try
                {
                    using (StreamReader readFile = new StreamReader(this.FilePath))
                    {
                        string line;
                        string[] row;

                        while ((line = readFile.ReadLine()) != null)
                        {
                            row = line.Split(',');
                            parsedData.Add(row);
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
