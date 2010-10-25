namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TimeInAuxStatusMetricFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCouldNotFindNecessariesFilesWhenSummaryOrTTSFilesNotInListMetricAuxTm()
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

            var auxTmMetric = new TimeInAuxStatusMetric();
            auxTmMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowFileDatesSummaryAndTTSDoNotMatchMetricAuxTm()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            newDataFile1.Setup(f => f.FileDate).Returns( DateTime.ParseExact("20101013","yyyyMMdd",null) );
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.TTS);
            newDataFile2.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20101014", "yyyyMMdd", null));

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);

            var auxTmMetric = new TimeInAuxStatusMetric();
            auxTmMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowAgentIDinSummaryFileNotFoundInTTSFileMetricAuxTm()
        {
            //File Summary
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            newDataFile1.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20100908", "yyyyMMdd", null));

            var dataLinesSummary = new List<Dictionary<string, string>>();
            var dic1Summary = this.GenerateDictionaryForSummaryFile("34,9/8/2010,70,200,12,3,330,60,70");
            var dic2Summary = this.GenerateDictionaryForSummaryFile("156,9/8/2010,50,210,8,1,320,40,70");
            dataLinesSummary.Add(dic1Summary);
            dataLinesSummary.Add(dic2Summary);
            newDataFile1.Setup(f => f.DataLines).Returns(dataLinesSummary);
          
            //File TTS
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.TTS);
            newDataFile2.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20100908", "yyyyMMdd", null));

            var dataLinesTTS = new List<Dictionary<string, string>>();
            var dic1TTS = this.GenerateDictionaryForTTSFile("34,9/8/2010,8:30,14:35,9/8/2010");
            var dic2TTS = this.GenerateDictionaryForTTSFile("345,9/8/2010,14:03,20:40,9/8/2010");
            dataLinesTTS.Add(dic1TTS);
            dataLinesTTS.Add(dic2TTS);
            newDataFile2.Setup(f => f.DataLines).Returns(dataLinesTTS);


            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);

            var auxTmMetric = new TimeInAuxStatusMetric();
            auxTmMetric.ProcessFiles(dataFiles);
        }


        [TestMethod]
        public void ShouldCalculateMetricValueForTheAgentsInFileMetricAuxTm()
        {
            //File Summary
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);
            newDataFile1.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20100908", "yyyyMMdd", null));

            var dataLinesSummary = new List<Dictionary<string, string>>();
            var dic1Summary = this.GenerateDictionaryForSummaryFile("34,09/08/2010,70,200,12,3,330,60,70");
            var dic2Summary = this.GenerateDictionaryForSummaryFile("156,09/08/2010,50,210,8,1,320,40,70");
            dataLinesSummary.Add(dic1Summary);
            dataLinesSummary.Add(dic2Summary);
            newDataFile1.Setup(f => f.DataLines).Returns(dataLinesSummary);

            //File TTS
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.TTS);
            newDataFile2.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20100908", "yyyyMMdd", null));

            var dataLinesTTS = new List<Dictionary<string, string>>();
            var dic1TTS = this.GenerateDictionaryForTTSFile("34,09/08/2010,08:30,14:35,09/08/2010");
            var dic2TTS = this.GenerateDictionaryForTTSFile("156,09/08/2010,09:43,15:50,09/08/2010");
            dataLinesTTS.Add(dic1TTS);
            dataLinesTTS.Add(dic2TTS);
            newDataFile2.Setup(f => f.DataLines).Returns(dataLinesTTS);


            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);


            var auxTmMetric = new TimeInAuxStatusMetric();
            auxTmMetric.ProcessFiles(dataFiles);

            Assert.AreEqual(2, auxTmMetric.CalculatedValues.Count);

            var auxTm1 = TimeInAuxStatusMetric.CalculateMetricValue(DateTime.ParseExact(dic1TTS["fecha Salida"], "dd/MM/yyyy", null), DateTime.ParseExact(dic1TTS["Horario Salida"], "HH:mm", null), DateTime.ParseExact(dic1TTS["fecha Entrada"], "dd/MM/yyyy", null), DateTime.ParseExact(dic1TTS["Horario Entrada"], "HH:mm", null), Convert.ToInt32(dic1Summary["Tiempo Loggeado (min)"]));
            var auxTm2 = TimeInAuxStatusMetric.CalculateMetricValue(DateTime.ParseExact(dic2TTS["fecha Salida"], "dd/MM/yyyy", null), DateTime.ParseExact(dic2TTS["Horario Salida"], "HH:mm", null), DateTime.ParseExact(dic2TTS["fecha Entrada"], "dd/MM/yyyy", null), DateTime.ParseExact(dic2TTS["Horario Entrada"], "HH:mm", null), Convert.ToInt32(dic2Summary["Tiempo Loggeado (min)"]));

            Assert.AreEqual(auxTm1, auxTmMetric.CalculatedValues[Convert.ToInt32(dic1Summary["Legajo"])]);
            Assert.AreEqual(auxTm2, auxTmMetric.CalculatedValues[Convert.ToInt32(dic2Summary["Legajo"])]);
            
            newDataFile1.VerifyAll();
            newDataFile2.VerifyAll();
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

        private Dictionary<string, string> GenerateDictionaryForTTSFile(string csvData)
        {
            string csvColumns = "legajo,fecha Entrada,Horario Entrada,Horario Salida,fecha Salida";
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
