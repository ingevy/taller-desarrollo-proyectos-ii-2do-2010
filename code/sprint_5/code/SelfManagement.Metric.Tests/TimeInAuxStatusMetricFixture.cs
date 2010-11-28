namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using CallCenter.SelfManagement.Metric.Helpers;

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
        [ExpectedException(typeof(MetricException))]
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
            var dic2TTS = this.GenerateDictionaryForTTSFile("156,9/8/2010,9:43,15:50,9/8/2010");
            dataLinesTTS.Add(dic1TTS);
            dataLinesTTS.Add(dic2TTS);
            newDataFile2.Setup(f => f.DataLines).Returns(dataLinesTTS);


            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);


            var auxTmMetric = new TimeInAuxStatusMetric();
            auxTmMetric.ProcessFiles(dataFiles);

            Assert.AreEqual(2, auxTmMetric.CalculatedValues.Count);

            //Dates reg 1
            Int32[] fechaSalida1 = TimeInAuxStatusMetric.parseFechas    (dic1TTS, "fecha Salida",   '/', Convert.ToInt32(dic1TTS["legajo"]));
            Int32[] horarioSalida1 = TimeInAuxStatusMetric.parseHorarios(dic1TTS, "Horario Salida", ':', Convert.ToInt32(dic1TTS["legajo"]));
            DateTime dateTimeSalida1 = new DateTime(fechaSalida1[2], fechaSalida1[1], fechaSalida1[0], horarioSalida1[0], horarioSalida1[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg
            
            Int32[] fechaEntrada1 = TimeInAuxStatusMetric.parseFechas    (dic1TTS, "fecha Entrada",   '/', Convert.ToInt32(dic1TTS["legajo"]));
            Int32[] horarioEntrada1 = TimeInAuxStatusMetric.parseHorarios(dic1TTS, "Horario Entrada", ':', Convert.ToInt32(dic1TTS["legajo"]));
            DateTime dateTimeEntrada1 = new DateTime(fechaEntrada1[2], fechaEntrada1[1], fechaEntrada1[0], horarioEntrada1[0], horarioEntrada1[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg

            //Dates reg 2
            Int32[] fechaSalida2 = TimeInAuxStatusMetric.parseFechas    (dic2TTS, "fecha Salida",   '/', Convert.ToInt32(dic2TTS["legajo"]));
            Int32[] horarioSalida2 = TimeInAuxStatusMetric.parseHorarios(dic2TTS, "Horario Salida", ':', Convert.ToInt32(dic2TTS["legajo"]));
            DateTime dateTimeSalida2 = new DateTime(fechaSalida2[2], fechaSalida2[1], fechaSalida2[0], horarioSalida2[0], horarioSalida2[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg
            
            Int32[] fechaEntrada2 = TimeInAuxStatusMetric.parseFechas    (dic2TTS, "fecha Entrada",   '/', Convert.ToInt32(dic2TTS["legajo"]));
            Int32[] horarioEntrada2 = TimeInAuxStatusMetric.parseHorarios(dic2TTS, "Horario Entrada", ':', Convert.ToInt32(dic2TTS["legajo"]));
            DateTime dateTimeEntrada2 = new DateTime(fechaEntrada2[2], fechaEntrada2[1], fechaEntrada2[0], horarioEntrada2[0], horarioEntrada2[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg


            var auxTm1 = TimeInAuxStatusMetric.CalculateMetricValue(dateTimeSalida1, dateTimeEntrada1, Convert.ToInt32(dic1Summary["Tiempo Loggeado (min)"]));
            var auxTm2 = TimeInAuxStatusMetric.CalculateMetricValue(dateTimeSalida2, dateTimeEntrada2, Convert.ToInt32(dic2Summary["Tiempo Loggeado (min)"]));

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
