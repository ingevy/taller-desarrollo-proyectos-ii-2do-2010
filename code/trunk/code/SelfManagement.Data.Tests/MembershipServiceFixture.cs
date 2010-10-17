namespace CallCenter.SelfManagement.Data.Tests
{
    using System.Globalization;
    using System.Web.Security;
    using CallCenter.SelfManagement.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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

            // Supervisors
            var supervisorUserName = userName;
            service.CreateUser(supervisorUserName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(supervisorUserName, SelfManagementRoles.Supervisor);
            service.CreateProfile(supervisorUserName, "30325134", "Jose Luis", "Lopez", null, "FTE", "activo", new DateTime(2007, 1, 9));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Supervisor);
            service.CreateProfile(userName, "30357963", "Martin", "Ramirez", 2500M, "FTE", "activo", null);

            // Agents
            var agentUserName = this.GetNextUserName();
            service.CreateUser(agentUserName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(agentUserName, SelfManagementRoles.Agent);
            service.CreateProfile(agentUserName, "30345235", "Juan", "Perez", 2000M, "PTE", "activo", new DateTime(2009, 2, 19));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "32877122", "Jose", "Flores", 2000M, "PTE", "activo", new DateTime(2009, 2, 13));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "34877111", "Lorena", "Garcia", 2000M, "PTE", "activo", new DateTime(2009, 2, 13));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "29111333", "Matias", "Gonzales", 2000M, "PTE", "activo", new DateTime(2009, 2, 13));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "24129333", "Juana", "Liorna", 2000M, "PTE", "activo", new DateTime(2009, 4, 15));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "28723090", "Lorena", "Gomez", 1900M, "FTE", "activo", new DateTime(2009, 4, 15));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "31474235", "Martin", "Martinez", 1800M, "PTE", "activo", new DateTime(2009, 6, 30));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "29456789", "Gonzalo", "Farias", 1800M, "PTE", "activo", new DateTime(2009, 6, 30));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "32564224", "Violeta", "Rivero", 1800M, "PTE", "activo", new DateTime(2009, 6, 30));

            userName = this.GetNextUserName();
            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
            service.AddUserToRol(userName, SelfManagementRoles.Agent);
            service.CreateProfile(userName, "26050210", "Carlos", "Abate", 1700M, "PTE", "activo", new DateTime(2009, 8, 1));

            var supervisor = service.GetSupervisor(supervisorUserName);           
            var agent = service.GetAgent(agentUserName);

            Assert.IsNotNull(supervisor);
            Assert.IsNotNull(agent);

            Assert.AreEqual(supervisorUserName, supervisor.UserName);
            Assert.AreEqual("30325134", supervisor.DNI);
            Assert.AreEqual("Jose Luis", supervisor.Name);
            Assert.AreEqual("Lopez", supervisor.LastName);
            Assert.AreEqual("", supervisor.GrossSalary);
            Assert.AreEqual("FTE", supervisor.Workday);
            Assert.AreEqual("activo", supervisor.Status);
            Assert.AreEqual("09/01/2007", supervisor.IncorporationDate);

            Assert.AreEqual(agentUserName, agent.UserName);
            Assert.AreEqual("30345235", agent.DNI);
            Assert.AreEqual("Juan", agent.Name);
            Assert.AreEqual("Perez", agent.LastName);
            Assert.AreEqual("2000", agent.GrossSalary);
            Assert.AreEqual("PTE", agent.Workday);
            Assert.AreEqual("activo", agent.Status);
            Assert.AreEqual("19/02/2009", agent.IncorporationDate);
        }

        [TestMethod]
        public void ShouldAddUserProfile()
        {
            var service = new MembershipService();
            var userName = this.GetNextUserName();

            while (service.ExistsUser(userName))
            {
                userName = this.GetNextUserName();
            }

            service.CreateUser(userName, "password", string.Format(CultureInfo.InvariantCulture, "{0}@callcenter.com", userName));
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
