namespace CallCenter.SelfManagement.Metric.Tests
{
    using System;
    using System.Collections.Generic;
    using CallCenter.SelfManagement.Metric.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DataFileFixture
    {
        [TestMethod]
        public void ShouldCreateDataFileWithDateAndTypeTakenFromPath()
        {
            var df1 = new DataFile("TTS_20101101.csv");
            var df2 = new DataFile("STS_20101101.csv");
            var df3 = new DataFile("Summary_20101101.csv");
            var df4 = new DataFile("QA_20101101.csv");
            var df5 = new DataFile("HF System.csv");

            var dateExpected = new DateTime(2010, 11, 1);

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
            var df5 = new DataFile("TTD_20101101.csv");
        }
    }
}
