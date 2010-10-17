namespace CallCenter.SelfManagement.Data.Tests
{
    using System.Web.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MembershipServiceFixture
    {
        [TestMethod]
        [DeploymentItem("SelfManagement.mdf")]
        public void ShouldCreateAUser()
        {
            //var service = new MembershipService();

            //var result = service.CreateUser("sample_user_1", "password", "sample@user.com");

            //Assert.AreEqual(MembershipCreateStatus.Success, result);
        }
    }
}
