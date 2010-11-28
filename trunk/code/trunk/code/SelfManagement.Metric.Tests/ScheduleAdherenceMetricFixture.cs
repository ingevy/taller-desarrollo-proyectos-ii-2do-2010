namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ScheduleAdherenceMetricFixture
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowCouldNotFindNecessariesFilesWhenSTSOrTTSFilesNotInListMetricSchedAdg()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.HF);
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.QA);
            var newDataFile3 = new Mock<IDataFile>();
            newDataFile3.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.SUMMARY);

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);
            dataFiles.Add(newDataFile3.Object);

            var schedAdgMetric = new ScheduleAdherenceMetric();
            schedAdgMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowFileDatesSTSAndTTSDoNotMatchMetricSchedAdg()
        {
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.STS);
            newDataFile1.Setup(f => f.FileDate).Returns( DateTime.ParseExact("20101013","yyyyMMdd",null) );
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.TTS);
            newDataFile2.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20101014", "yyyyMMdd", null));

            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);

            var schedAdgMetric = new ScheduleAdherenceMetric();
            schedAdgMetric.ProcessFiles(dataFiles);
        }

        [TestMethod]
        public void ShouldCalculateMetricValueForTheAgentsInFileMetricSchedAdg()
        {
            //File STS
            var newDataFile1 = new Mock<IDataFile>();
            newDataFile1.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.STS);
            newDataFile1.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20101013", "yyyyMMdd", null));

            var dataLinesSTS = new List<Dictionary<string, string>>();
            var dic1STS = this.GenerateDictionaryForSTSFile("1,13/10/2010,8:30,14:30,13/10/2010");
            var dic2STS = this.GenerateDictionaryForSTSFile("2,13/10/2010,9:00,15:00,13/10/2010");
            dataLinesSTS.Add(dic1STS);
            dataLinesSTS.Add(dic2STS);
            newDataFile1.Setup(f => f.DataLines).Returns(dataLinesSTS);

            //File TTS
            var newDataFile2 = new Mock<IDataFile>();
            newDataFile2.Setup(f => f.ExternalSystemFile).Returns(ExternalSystemFiles.TTS);
            newDataFile2.Setup(f => f.FileDate).Returns(DateTime.ParseExact("20101013", "yyyyMMdd", null));

            var dataLinesTTS = new List<Dictionary<string, string>>();
            var dic1TTS = this.GenerateDictionaryForTTSFile("1,13/10/2010,8:30,14:24,13/10/2010");
            var dic2TTS = this.GenerateDictionaryForTTSFile("2,13/10/2010,9:43,15:50,13/10/2010");
            dataLinesTTS.Add(dic1TTS);
            dataLinesTTS.Add(dic2TTS);
            newDataFile2.Setup(f => f.DataLines).Returns(dataLinesTTS);


            var dataFiles = new List<IDataFile>();
            dataFiles.Add(newDataFile1.Object);
            dataFiles.Add(newDataFile2.Object);


            var schedAdgMetric = new ScheduleAdherenceMetric();
            schedAdgMetric.ProcessFiles(dataFiles);

            Assert.AreEqual(2, schedAdgMetric.CalculatedValues.Count);

            //STS Dates reg 1
            Int32[] fechaSalidaSTS1 = ScheduleAdherenceMetric.parseFechas    (dic1STS, "fecha Salida",   '/', Convert.ToInt32(dic1STS["legajo"]), "STS");
            Int32[] horarioSalidaSTS1 = ScheduleAdherenceMetric.parseHorarios(dic1STS, "Horario Salida", ':', Convert.ToInt32(dic1STS["legajo"]), "STS");
            DateTime dateTimeSalidaSTS1 = new DateTime(fechaSalidaSTS1[2], fechaSalidaSTS1[1], fechaSalidaSTS1[0], horarioSalidaSTS1[0], horarioSalidaSTS1[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg

            Int32[] fechaEntradaSTS1 = ScheduleAdherenceMetric.parseFechas    (dic1STS, "fecha Entrada",   '/', Convert.ToInt32(dic1STS["legajo"]), "STS");
            Int32[] horarioEntradaSTS1 = ScheduleAdherenceMetric.parseHorarios(dic1STS, "Horario Entrada", ':', Convert.ToInt32(dic1STS["legajo"]), "STS");
            DateTime dateTimeEntradaSTS1 = new DateTime(fechaEntradaSTS1[2], fechaEntradaSTS1[1], fechaEntradaSTS1[0], horarioEntradaSTS1[0], horarioEntradaSTS1[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg

            //TTS Dates reg 1
            Int32[] fechaSalidaTTS1 = ScheduleAdherenceMetric.parseFechas    (dic1TTS, "fecha Salida",   '/', Convert.ToInt32(dic1TTS["legajo"]), "TTS");
            Int32[] horarioSalidaTTS1 = ScheduleAdherenceMetric.parseHorarios(dic1TTS, "Horario Salida", ':', Convert.ToInt32(dic1TTS["legajo"]), "TTS");
            DateTime dateTimeSalidaTTS1 = new DateTime(fechaSalidaTTS1[2], fechaSalidaTTS1[1], fechaSalidaTTS1[0], horarioSalidaTTS1[0], horarioSalidaTTS1[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg

            Int32[] fechaEntradaTTS1 = ScheduleAdherenceMetric.parseFechas    (dic1TTS, "fecha Entrada",   '/', Convert.ToInt32(dic1TTS["legajo"]), "TTS");
            Int32[] horarioEntradaTTS1 = ScheduleAdherenceMetric.parseHorarios(dic1TTS, "Horario Entrada", ':', Convert.ToInt32(dic1TTS["legajo"]), "TTS");
            DateTime dateTimeEntradaTTS1 = new DateTime(fechaEntradaTTS1[2], fechaEntradaTTS1[1], fechaEntradaTTS1[0], horarioEntradaTTS1[0], horarioEntradaTTS1[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg

            //STS Dates reg 2
            Int32[] fechaSalidaSTS2 = ScheduleAdherenceMetric.parseFechas    (dic2STS, "fecha Salida",   '/', Convert.ToInt32(dic2STS["legajo"]), "STS");
            Int32[] horarioSalidaSTS2 = ScheduleAdherenceMetric.parseHorarios(dic2STS, "Horario Salida", ':', Convert.ToInt32(dic2STS["legajo"]), "STS");
            DateTime dateTimeSalidaSTS2 = new DateTime(fechaSalidaSTS2[2], fechaSalidaSTS2[1], fechaSalidaSTS2[0], horarioSalidaSTS2[0], horarioSalidaSTS2[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg

            Int32[] fechaEntradaSTS2 = ScheduleAdherenceMetric.parseFechas    (dic2STS, "fecha Entrada",   '/', Convert.ToInt32(dic2STS["legajo"]), "STS");
            Int32[] horarioEntradaSTS2 = ScheduleAdherenceMetric.parseHorarios(dic2STS, "Horario Entrada", ':', Convert.ToInt32(dic2STS["legajo"]), "STS");
            DateTime dateTimeEntradaSTS2 = new DateTime(fechaEntradaSTS2[2], fechaEntradaSTS2[1], fechaEntradaSTS2[0], horarioEntradaSTS2[0], horarioEntradaSTS2[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg

            //TTS Dates reg 2
            Int32[] fechaSalidaTTS2 = ScheduleAdherenceMetric.parseFechas    (dic2TTS, "fecha Salida",   '/', Convert.ToInt32(dic2TTS["legajo"]), "TTS");
            Int32[] horarioSalidaTTS2 = ScheduleAdherenceMetric.parseHorarios(dic2TTS, "Horario Salida", ':', Convert.ToInt32(dic2TTS["legajo"]), "TTS");
            DateTime dateTimeSalidaTTS2 = new DateTime(fechaSalidaTTS2[2], fechaSalidaTTS2[1], fechaSalidaTTS2[0], horarioSalidaTTS2[0], horarioSalidaTTS2[1], Convert.ToInt32(0)); //anio, mes, dia, hora, minutos, seg

            Int32[] fechaEntradaTTS2 = ScheduleAdherenceMetric.parseFechas    (dic2TTS, "fecha Entrada",   '/', Convert.ToInt32(dic2TTS["legajo"]), "TTS");
            Int32[] horarioEntradaTTS2 = ScheduleAdherenceMetric.parseHorarios(dic2TTS, "Horario Entrada", ':', Convert.ToInt32(dic2TTS["legajo"]), "TTS");
            DateTime dateTimeEntradaTTS2 = new DateTime(fechaEntradaTTS2[2], fechaEntradaTTS2[1], fechaEntradaTTS2[0], horarioEntradaTTS2[0], horarioEntradaTTS2[1], Convert.ToInt32(0));//anio, mes, dia, hora, minutos, seg


            var schedAdg1 = ScheduleAdherenceMetric.CalculateMetricValue(dateTimeEntradaSTS1, dateTimeEntradaTTS1, dateTimeSalidaSTS1, dateTimeSalidaTTS1);
            var schedAdg2 = ScheduleAdherenceMetric.CalculateMetricValue(dateTimeEntradaSTS2, dateTimeEntradaTTS2, dateTimeSalidaSTS2, dateTimeSalidaTTS2);

            Assert.AreEqual(schedAdg1, schedAdgMetric.CalculatedValues[Convert.ToInt32(dic1STS["legajo"])]);
            Assert.AreEqual(schedAdg2, schedAdgMetric.CalculatedValues[Convert.ToInt32(dic2STS["legajo"])]);
            
            newDataFile1.VerifyAll();
            newDataFile2.VerifyAll();
        }

        private Dictionary<string, string> GenerateDictionaryForSTSFile(string csvData)
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
