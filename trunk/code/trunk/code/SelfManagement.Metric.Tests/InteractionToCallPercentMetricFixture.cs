namespace CallCenter.SelfManagement.Metric.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Moq;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using System.Collections.Generic;

    [TestClass]
    public class InteractionToCallPercentMetricFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCouldNotFindNecessaryFileWhenSummaryFileNotInList()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.HF);
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.QA);
            var newDataFile3 = new Mock<IDataFile>();
            newDataFile3.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.STS);

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);
            dataFiles.Add(newDataFile3.Object);

            var ictMetric = new InteractionToCallPercentMetric(new DateTime());
            ictMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowFileDateDoesNotMatchMetricDateWhenDateInDataFileDiffersFromDateInMetric()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            newDataFile1.Setup(f => f.FileDate).Returns(new DateTime(2010, 10, 2));

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);

            var ictMetric = new InteractionToCallPercentMetric(new DateTime(2010,10,3));
            ictMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        public void ShouldCalculateMetricValueForTheAgentsInFile()
        {
            var dataLines = new List<Dictionary<string, string>>();

            var dic1 = this.GenerateDictionaryForSummaryFile("34,9/8/2010,70,200,12,3,330,60,70");
            var dic2 = this.GenerateDictionaryForSummaryFile("156,9/8/2010,50,210,8,1,320,40,70");
            var dic3 = this.GenerateDictionaryForSummaryFile("345,9/8/2010,67,240,4,0,320,20,60");

            dataLines.Add(dic1);
            dataLines.Add(dic2);
            dataLines.Add(dic3);

            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            newDataFile1.Setup(f => f.DataLines).Returns(dataLines);

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);

            var ictMetric = new InteractionToCallPercentMetric(new DateTime());
            ictMetric.ProcessFiles(dataFiles);

            Assert.AreEqual(3, ictMetric.CalculatedValues.Count);

            var ict1 = (Convert.ToInt32(dic1["Cantidad Llamadas"]) - Convert.ToInt32(dic1["Cantidad Llamadas Transferidas"])) / Convert.ToInt32(dic1["Cantidad Llamadas"]);
            var ict2 = (Convert.ToInt32(dic2["Cantidad Llamadas"]) - Convert.ToInt32(dic2["Cantidad Llamadas Transferidas"])) / Convert.ToInt32(dic2["Cantidad Llamadas"]);
            var ict3 = (Convert.ToInt32(dic3["Cantidad Llamadas"]) - Convert.ToInt32(dic3["Cantidad Llamadas Transferidas"])) / Convert.ToInt32(dic3["Cantidad Llamadas"]);

            Assert.AreEqual(ict1, ictMetric.CalculatedValues[Convert.ToInt32(dic1["Legajo"])]);
            Assert.AreEqual(ict2, ictMetric.CalculatedValues[Convert.ToInt32(dic2["Legajo"])]);
            Assert.AreEqual(ict3, ictMetric.CalculatedValues[Convert.ToInt32(dic3["Legajo"])]);
        }

        private Dictionary<string, string> GenerateDictionaryForSummaryFile(string csvData)
        {
            string csvColumns = "Legajo,Date,Cantidad Llamadas,Tiempo InCall (min),Tiempo en espera (min),Cantidad Llamadas Transferidas,Tiempo Loggeado (min),Tiempo Ready for Call (min),Tiempo en after call work (min)";
            string[] columns = csvColumns.Split(',');
            string[] values = csvData.Split(',');

            var dic = new Dictionary<string, string>();
            for (var i = 0; i < columns.Length; i++)
            {
                dic.Add(columns[i], values[i]);
            }

            return dic;
        }
    }
}
