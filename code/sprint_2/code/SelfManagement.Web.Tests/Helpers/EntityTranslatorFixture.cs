namespace CallCenter.SelfManagement.Web.Tests.Controllers
{
    using System.Globalization;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.ViewModels;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class EntityTranslatorFixture
    {
        [TestMethod]
        public void ShouldTranslateCampaingToEntity1()
        {
            var model = new CampaingViewModel
            {
                Name = "Campaing Name",
                Description = "Campaing Description",
                BeginDate = "22/01/2010",
                EndDate = "22/01/2011",
                CampaingType = 1,
                OptimalHourlyValue = "5.95",
                ObjectiveHourlyValue = "4.50",
                MinimumHourlyValue = "1.35",
                CustomerName = "Banco Hipotecario"
            };
            var customerId = 5;

            var repository = new Mock<ICampaingRepository>();
            repository.Setup(r => r.RetrieveOrCreateCustomerIdByName(model.CustomerName)).Returns(customerId);

            var entity = model.ToEntity(repository.Object);

            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.Description, entity.Description);
            Assert.AreEqual(model.BeginDate, entity.BeginDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            Assert.IsTrue(entity.EndDate.HasValue);
            Assert.AreEqual(model.EndDate, entity.EndDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            Assert.AreEqual(model.CampaingType, entity.CampaingType);
            Assert.AreEqual(5.95M, entity.OptimalHourlyValue);
            Assert.AreEqual(4.50M, entity.ObjectiveHourlyValue);
            Assert.AreEqual(1.35M, entity.MinimumHourlyValue);
            Assert.AreEqual(customerId, entity.CustomerId);

            repository.VerifyAll();
        }

        [TestMethod]
        public void ShouldTranslateCampaingToEntity2()
        {
            var model = new CampaingViewModel
            {
                Name = "Campaing Name",
                Description = "Campaing Description",
                BeginDate = "22/01/2010",
                CampaingType = 1,
                OptimalHourlyValue = "5.95",
                ObjectiveHourlyValue = "4.50",
                MinimumHourlyValue = "1.35",
                CustomerName = "Banco Hipotecario"
            };
            var customerId = 5;

            var repository = new Mock<ICampaingRepository>();
            repository.Setup(r => r.RetrieveOrCreateCustomerIdByName(model.CustomerName)).Returns(customerId);

            var entity = model.ToEntity(repository.Object);

            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.Description, entity.Description);
            Assert.AreEqual(model.BeginDate, entity.BeginDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            Assert.IsFalse(entity.EndDate.HasValue);
            Assert.IsNull(entity.EndDate);
            Assert.AreEqual(model.CampaingType, entity.CampaingType);
            Assert.AreEqual(5.95M, entity.OptimalHourlyValue);
            Assert.AreEqual(4.50M, entity.ObjectiveHourlyValue);
            Assert.AreEqual(1.35M, entity.MinimumHourlyValue);
            Assert.AreEqual(customerId, entity.CustomerId);

            repository.VerifyAll();
        }

        [TestMethod]
        public void ShouldTranslateCampaingMetricLevelToEntity()
        {
            var model = new CampaingMetricLevelViewModel
            {
                Id = 2,
                OptimalLevel = "5.95",
                ObjectiveLevel = "4.50",
                MinimumLevel = "1.35",
            };
            var campaingId = 1;

            var entity = model.ToEntity(campaingId);

            Assert.AreEqual(campaingId, entity.CampaingId);
            Assert.AreEqual(model.Id, entity.MetricId);
            Assert.AreEqual(5.95, entity.OptimalLevel);
            Assert.AreEqual(4.50, entity.ObjectiveLevel);
            Assert.AreEqual(1.35, entity.MinimumLevel);
            Assert.IsTrue(entity.Enabled);
        }
    }
}
