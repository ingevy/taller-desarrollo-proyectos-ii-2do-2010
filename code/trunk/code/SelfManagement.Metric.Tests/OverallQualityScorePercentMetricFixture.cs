namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class OverallQualityScorePercentMetricFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCouldNotFindNecessaryFileWhenSummaryFileNotInListMetricQaScore()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.HF);
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            var newDataFile3 = new Mock<IDataFile>();
            newDataFile3.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.STS);

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);
            dataFiles.Add(newDataFile3.Object);

            var qaScoreMetric = new OverallQualityScorePercentMetric();
            qaScoreMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        public void ShouldCalculateMetricValueForTheAgentsInFileMetricQaScore()
        {
            var dataLines = new List<Dictionary<string, string>>();

            var dic1 = this.GenerateDictionaryForQAFile("34,9/8/2010,3,30,27");
            var dic2 = this.GenerateDictionaryForQAFile("156,9/8/2010,2,20,18");
            var dic3 = this.GenerateDictionaryForQAFile("345,9/8/2010,4,40,38");

            dataLines.Add(dic1);
            dataLines.Add(dic2);
            dataLines.Add(dic3);

            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.QA);
            newDataFile1.Setup(f => f.DataLines).Returns(dataLines);

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);

            var qaScoreMetric = new OverallQualityScorePercentMetric();
            qaScoreMetric.ProcessFiles(dataFiles);

            Assert.AreEqual(3, qaScoreMetric.CalculatedValues.Count);

            var qaScore1 = OverallQualityScorePercentMetric.CalculateMetricValue(Convert.ToInt32(dic1["Cantidad Puntos logrados"]), Convert.ToInt32(dic1["Cant de Puntos posibles"]));
            var qaScore2 = OverallQualityScorePercentMetric.CalculateMetricValue(Convert.ToInt32(dic2["Cantidad Puntos logrados"]), Convert.ToInt32(dic2["Cant de Puntos posibles"]));
            var qaScore3 = OverallQualityScorePercentMetric.CalculateMetricValue(Convert.ToInt32(dic3["Cantidad Puntos logrados"]), Convert.ToInt32(dic3["Cant de Puntos posibles"]));

            Assert.AreEqual(qaScore1, qaScoreMetric.CalculatedValues[Convert.ToInt32(dic1["legajo"])]);
            Assert.AreEqual(qaScore2, qaScoreMetric.CalculatedValues[Convert.ToInt32(dic2["legajo"])]);
            Assert.AreEqual(qaScore3, qaScoreMetric.CalculatedValues[Convert.ToInt32(dic3["legajo"])]);

            newDataFile1.VerifyAll();
        }

        private Dictionary<string, string> GenerateDictionaryForQAFile(string csvData)
        {
            string csvColumns = "legajo,Fecha,Cant Evaluaciones,Cant de Puntos posibles,Cantidad Puntos logrados";
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
