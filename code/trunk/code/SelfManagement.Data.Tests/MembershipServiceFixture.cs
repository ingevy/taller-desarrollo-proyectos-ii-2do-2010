namespace CallCenter.SelfManagement.Data.Tests
{
    using System.Globalization;
    using System.Web.Security;
    using CallCenter.SelfManagement.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MembershipServiceFixture
    {
        private int userCount = 0;

        [TestMethod]
        public void ShouldCheckIfUserExists1()
        {
            var service = new MembershipService();

            var result = service.ExistsUser("john.doe");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ShouldCheckIfUserExists2()
        {
            var service = new MembershipService();

            var result = service.ExistsUser("none-existing-user");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldCheckIfUserExists3()
        {
            var service = new MembershipService();

            var result = service.ExistsUser("");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShouldCreateAUser()
        {
            var service = new MembershipService();
            var userName = this.GetNextUserName();

            while (service.ExistsUser(userName))
            {
                userName = this.GetNextUserName();
            }            

            var result = service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@tests.com", userName));

            Assert.AreEqual(MembershipCreateStatus.Success, result);
        }

        [TestMethod]
        public void ShouldAddUserToRolAgent()
        {
            var service = new MembershipService();
            var userName = this.GetNextUserName();

            while (service.ExistsUser(userName))
            {
                userName = this.GetNextUserName();
            }

            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@tests.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);

            var agent = service.GetAgent(userName);
            var supervisor = service.GetSupervisor(userName);

            Assert.IsNotNull(agent);
            Assert.IsNull(supervisor);

            Assert.AreEqual(userName, agent.UserName);
        }

        [TestMethod]
        public void ShouldAddUserToRolSupervisor()
        {
            var service = new MembershipService();
            var userName = this.GetNextUserName();

            while (service.ExistsUser(userName))
            {
                userName = this.GetNextUserName();
            }

            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@tests.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Supervisor);

            var agent = service.GetAgent(userName);
            var supervisor = service.GetSupervisor(userName);

            Assert.IsNull(agent);
            Assert.IsNotNull(supervisor);

            Assert.AreEqual(userName, supervisor.UserName);
        }

        private string GetNextUserName()
        {
            this.userCount++;

            return string.Format(CultureInfo.InvariantCulture, "sample_user_{0}", this.userCount);
        }
    }
}
