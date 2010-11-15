namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.IO;

    [TestClass]
    public class DataFileFixture
    {
        [TestMethod]
        public void ShouldCreateDataFileWithDateAndTypeAndPathTakenFromPath()
        {
            var df1 = new DataFile("TTS_20101101.csv");
            var df2 = new DataFile("STS_20101101.csv");
            var df3 = new DataFile("Summary_20101101.csv");
            var df4 = new DataFile("QA_20101101.csv");
            var df5 = new DataFile("HF System.csv");

            var dateExpected = new DateTime(2010, 11, 1);

            Assert.AreEqual("TTS_20101101.csv", df1.FilePath);
            Assert.AreEqual("STS_20101101.csv", df2.FilePath);
            Assert.AreEqual("Summary_20101101.csv", df3.FilePath);
            Assert.AreEqual("QA_20101101.csv", df4.FilePath);
            Assert.AreEqual("HF System.csv", df5.FilePath);

            Assert.AreEqual(dateExpected, df1.FileDate);
            Assert.AreEqual(dateExpected, df2.FileDate);
            Assert.AreEqual(dateExpected, df3.FileDate);
            Assert.AreEqual(dateExpected, df4.FileDate);
            Assert.AreEqual(DateTime.Now.Date, df5.FileDate);

            Assert.AreEqual(ExternalSystemFiles.TTS, df1.ExternalSystemFile);
            Assert.AreEqual(ExternalSystemFiles.STS, df2.ExternalSystemFile);
            Assert.AreEqual(ExternalSystemFiles.SUMMARY, df3.ExternalSystemFile);
            Assert.AreEqual(ExternalSystemFiles.QA, df4.ExternalSystemFile);
            Assert.AreEqual(ExternalSystemFiles.HF, df5.ExternalSystemFile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfFileNameNotSupported()
        {
            DataFile.GetDataFileType("TTZ_20101102.csv");
        }

        [TestMethod]
        public void ShouldReturnDictionaryWithColumnValuePairsFromFile()
        {
            using (var sw = new StreamWriter("TTS_20100930.csv",false))
            {
                sw.Write("legajo,fecha Entrada,Horario Entrada,Horario Salida,fecha Salida");
                sw.Write(Environment.NewLine);
                sw.Write("1,14/10/2010,9:30,15:30,14/10/2010");
                sw.Write(Environment.NewLine);
                sw.Close();
            }

            var df1 = new DataFile("TTS_20100930.csv");

            var dataLines = df1.DataLines;

            foreach (var line in dataLines)
            {
                Assert.AreEqual("1", line["legajo"]);
                Assert.AreEqual("14/10/2010", line["fecha Entrada"]);
                Assert.AreEqual("9:30", line["Horario Entrada"]);
                Assert.AreEqual("15:30", line["Horario Salida"]);
                Assert.AreEqual("14/10/2010", line["fecha Salida"]);
            }

            File.Delete("TTS_20100930.csv");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowArgumentExceptionIfFileNotExists()
        {
            var df1 = new DataFile("TTS_20100929.csv");

            var dataLines = df1.DataLines;
        }
    }
}
