namespace CallCenter.SelfManagement.Web.Tests.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Web.Controllers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HomeControllerFixture
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            ViewDataDictionary viewData = result.ViewData;
            Assert.AreEqual("¡Bienvenido al sistema SelfManagement!", viewData["WelcomeMessage"]);
        }
    }
}
