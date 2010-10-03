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

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldSaveCampaingMetricLevels()
        {
            var campaingId = 0;
            IList<CampaingMetricLevel> campaingMetricLevelsResult = null;

            using (new TransactionScope())
            {
                var campaing = new Campaing
                {
                    Name = "Test Campaing",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var repository = new CampaingRepository();

                campaingId = repository.CreateCampaing(campaing);

                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 5, OptimalLevel = 45, ObjectiveLevel = 35, MinimumLevel = 25, Enabled = true },
                };
                
                repository.SaveCampaingMetricLevels(campaingMetrics);
                campaingMetricLevelsResult = repository.RetrieveCampaingMetricLevels(campaingId);
            }

            Assert.IsNotNull(campaingMetricLevelsResult);
            Assert.AreEqual(3, campaingMetricLevelsResult.Count);
            Assert.IsTrue(campaingMetricLevelsResult.All(cml => cml.CampaingId == campaingId));

            var sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 4);

            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.AreEqual(60, sampleCampaingMetricLevel.OptimalLevel);
            Assert.AreEqual(50, sampleCampaingMetricLevel.ObjectiveLevel);
            Assert.AreEqual(40, sampleCampaingMetricLevel.MinimumLevel);
            Assert.IsTrue(sampleCampaingMetricLevel.Enabled);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldThrowSavingCampaingMetricLevelsWhenTheyAreNotThree1()
        {
            using (new TransactionScope())
            {
                var campaing = new Campaing
                {
                    Name = "Test Campaing",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var repository = new CampaingRepository();

                var campaingId = repository.CreateCampaing(campaing);
                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                };

                repository.SaveCampaingMetricLevels(campaingMetrics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldThrowSavingCampaingMetricLevelsWhenTheyAreNotThree2()
        {
            using (new TransactionScope())
            {
                var campaing = new Campaing
                {
                    Name = "Test Campaing",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var repository = new CampaingRepository();

                var campaingId = repository.CreateCampaing(campaing);

                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 5, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = false },
                };

                repository.SaveCampaingMetricLevels(campaingMetrics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldThrowSavingCampaingMetricLevelsWhenTheyAreNotThree3()
        {
            using (new TransactionScope())
            {
                var campaing = new Campaing
                {
                    Name = "Test Campaing",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var repository = new CampaingRepository();

                var campaingId = repository.CreateCampaing(campaing);
                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 5, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 6, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                };

                repository.SaveCampaingMetricLevels(campaingMetrics);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldThrowSavingCampaingMetricLevelsFromDifferentCampaings()
        {
            using (new TransactionScope())
            {
                var repository = new CampaingRepository();

                var campaing = new Campaing
                {
                    Name = "Test Campaing 1",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var campaingId1 = repository.CreateCampaing(campaing);

                campaing = new Campaing
                {
                    Name = "Test Campaing 2",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 2
                };
                var campaingId2 = repository.CreateCampaing(campaing);

                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId1, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId2, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId1, MetricId = 5, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                };

                repository.SaveCampaingMetricLevels(campaingMetrics);
            }
        }

        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldUpdateCampaingMetricLevels()
        {
            var campaingId = 0;
            IList<CampaingMetricLevel> campaingMetricLevelsResult = null;

            using (new TransactionScope())
            {
                var campaing = new Campaing
                {
                    Name = "Test Campaing",
                    CampaingType = 0,
                    BeginDate = DateTime.Now,
                    CustomerId = 1
                };
                var repository = new CampaingRepository();

                campaingId = repository.CreateCampaing(campaing);

                // original metrics
                var campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 30, ObjectiveLevel = 20, MinimumLevel = 10, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 4, OptimalLevel = 60, ObjectiveLevel = 50, MinimumLevel = 40, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 5, OptimalLevel = 45, ObjectiveLevel = 35, MinimumLevel = 25, Enabled = true }
                };
                repository.SaveCampaingMetricLevels(campaingMetrics);

                // updated metrics
                campaingMetrics = new List<CampaingMetricLevel>
                {
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 3, OptimalLevel = 100, ObjectiveLevel = 80, MinimumLevel = 60, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 6, OptimalLevel = 10, ObjectiveLevel = 9, MinimumLevel = 8, Enabled = false },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 7, OptimalLevel = 70, ObjectiveLevel = 65, MinimumLevel = 60, Enabled = true },
                    new CampaingMetricLevel { CampaingId = campaingId, MetricId = 8, OptimalLevel = 80, ObjectiveLevel = 75, MinimumLevel = 70, Enabled = true }
                };
                repository.SaveCampaingMetricLevels(campaingMetrics);

                campaingMetricLevelsResult = repository.RetrieveCampaingMetricLevels(campaingId);
            }

            Assert.IsNotNull(campaingMetricLevelsResult);
            Assert.AreEqual(6, campaingMetricLevelsResult.Count);
            Assert.IsTrue(campaingMetricLevelsResult.All(cml => cml.CampaingId == campaingId));

            // Disabled
            var sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 4);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsFalse(sampleCampaingMetricLevel.Enabled);

            sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 5);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsFalse(sampleCampaingMetricLevel.Enabled);
            
            sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 6);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsFalse(sampleCampaingMetricLevel.Enabled);
            Assert.AreEqual(10, sampleCampaingMetricLevel.OptimalLevel);
            Assert.AreEqual(9, sampleCampaingMetricLevel.ObjectiveLevel);
            Assert.AreEqual(8, sampleCampaingMetricLevel.MinimumLevel);

            // Enabled
            sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 3);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsTrue(sampleCampaingMetricLevel.Enabled);
            Assert.AreEqual(100, sampleCampaingMetricLevel.OptimalLevel);
            Assert.AreEqual(80, sampleCampaingMetricLevel.ObjectiveLevel);
            Assert.AreEqual(60, sampleCampaingMetricLevel.MinimumLevel);

            // Enabled
            sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 7);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsTrue(sampleCampaingMetricLevel.Enabled);
            Assert.AreEqual(70, sampleCampaingMetricLevel.OptimalLevel);
            Assert.AreEqual(65, sampleCampaingMetricLevel.ObjectiveLevel);
            Assert.AreEqual(60, sampleCampaingMetricLevel.MinimumLevel);

            // Enabled
            sampleCampaingMetricLevel = campaingMetricLevelsResult.FirstOrDefault(cml => cml.MetricId == 8);
            Assert.IsNotNull(sampleCampaingMetricLevel);
            Assert.IsTrue(sampleCampaingMetricLevel.Enabled);
            Assert.AreEqual(80, sampleCampaingMetricLevel.OptimalLevel);
            Assert.AreEqual(75, sampleCampaingMetricLevel.ObjectiveLevel);
            Assert.AreEqual(70, sampleCampaingMetricLevel.MinimumLevel);
        }
    }
}
