namespace CallCenter.SelfManagement.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class CampaingRepositoryFixture
    {
        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldRetrieveCustomersByName()
        {
            IList<Customer> result = null;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                result = repository.RetrieveCustomersByName("banco");
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Any(c => c.Name == "Banco Hipotecario"));
            Assert.IsTrue(result.Any(c => c.Name == "Banco Nación"));
            Assert.IsTrue(result.Any(c => c.Name == "Banco Provincia"));
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldRetrieveCustomerByName()
        {
            var customerName = "Banco Hipotecario";
            var result = 0;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                result = repository.RetrieveOrCreateCustomerIdByName(customerName);
            }

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldCreateCustomerIfDoesNotExist()
        {
            var customerName = "Este Cliente No Existe";
            var customerId = 0;     
            IList<Customer> result = null;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                customerId = repository.RetrieveOrCreateCustomerIdByName(customerName);
                result = repository.RetrieveCustomersByName(customerName);
            }

            Assert.IsTrue(customerId > 7);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(customerName, result.FirstOrDefault().Name);
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldRetrieveAvailableMetrics()
        {
            IList<Metric> result = null;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();
                
                result = repository.RetrieveAvailableMetrics();
            }

            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Count);

            var sampleMetric = result.FirstOrDefault(m => m.Id == 10);

            Assert.AreEqual("AVG_TALK_TM", sampleMetric.MetricName);
            Assert.AreEqual("Average Talk Time (seconds/call)", sampleMetric.ShortDescription);
            Assert.AreEqual("The average time the agent spent talking to a customer. Sec/call.", sampleMetric.LondDescription);
            Assert.AreEqual(1, sampleMetric.Format);
            Assert.AreEqual("CallCenter.SelfManagement.Metric.AverageTalkTimeNumberMetric, SelfManagement.Metric", sampleMetric.CLRType);
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldCreateNewCampaingWithAllFields()
        {
            var campaing = new Campaing
            {
                Name = "Test Campaing",
                Description = "Test Description",
                CampaingType = 0,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(15),
                CustomerId = 1
            };
            Campaing campaingResult = null;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                var id = repository.CreateCampaing(campaing);
                campaingResult = repository.RetrieveCampaingById(id);
            }

            Assert.IsNotNull(campaingResult);
            Assert.AreEqual(campaing.Name, campaingResult.Name);
            Assert.AreEqual(campaing.Description, campaingResult.Description);
            Assert.AreEqual(campaing.CampaingType, campaingResult.CampaingType);
            Assert.AreEqual(campaing.CustomerId, campaingResult.CustomerId);
            Assert.AreEqual(campaing.BeginDate.Year, campaingResult.BeginDate.Year);
            Assert.AreEqual(campaing.BeginDate.Month, campaingResult.BeginDate.Month);
            Assert.AreEqual(campaing.BeginDate.Day, campaingResult.BeginDate.Day);
            Assert.AreEqual(campaing.BeginDate.Hour, campaingResult.BeginDate.Hour);
            Assert.AreEqual(campaing.BeginDate.Minute, campaingResult.BeginDate.Minute);
            Assert.IsTrue(campaingResult.EndDate.HasValue);
            Assert.AreEqual(campaing.EndDate.Value.Year, campaingResult.EndDate.Value.Year);
            Assert.AreEqual(campaing.EndDate.Value.Month, campaingResult.EndDate.Value.Month);
            Assert.AreEqual(campaing.EndDate.Value.Day, campaingResult.EndDate.Value.Day);
            Assert.AreEqual(campaing.EndDate.Value.Hour, campaingResult.EndDate.Value.Hour);
            Assert.AreEqual(campaing.EndDate.Value.Minute, campaingResult.EndDate.Value.Minute);
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldCreateNewCampaingWithOnlyRequiredFields()
        {
            var campaing = new Campaing
            {
                Name = "Test Campaing",
                CampaingType = 0,
                BeginDate = DateTime.Now,
                CustomerId = 1
            };
            Campaing campaingResult = null;

            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                var id = repository.CreateCampaing(campaing);
                campaingResult = repository.RetrieveCampaingById(id);
            }

            Assert.IsNotNull(campaingResult);
            Assert.AreEqual(campaing.Name, campaingResult.Name);
            Assert.AreEqual(campaing.CampaingType, campaingResult.CampaingType);
            Assert.AreEqual(campaing.CustomerId, campaingResult.CustomerId);
            Assert.AreEqual(campaing.BeginDate.Year, campaingResult.BeginDate.Year);
            Assert.AreEqual(campaing.BeginDate.Month, campaingResult.BeginDate.Month);
            Assert.AreEqual(campaing.BeginDate.Day, campaingResult.BeginDate.Day);
            Assert.AreEqual(campaing.BeginDate.Hour, campaingResult.BeginDate.Hour);
            Assert.AreEqual(campaing.BeginDate.Minute, campaingResult.BeginDate.Minute);
            Assert.IsFalse(campaingResult.EndDate.HasValue);
            Assert.IsTrue(string.IsNullOrWhiteSpace(campaingResult.Description));
        }
    }
}
