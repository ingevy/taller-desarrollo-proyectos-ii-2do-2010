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
    }
}
