namespace CallCenter.SelfManagement.Web.Controllers
{
    using System.Web.Mvc;
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Helpers;
    using CallCenter.SelfManagement.Web.Services;
    using CallCenter.SelfManagement.Web.ViewModels;

    [HandleError]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new RepositoryFactory().GetFormsAuthenticationService(), new RepositoryFactory().GetMembershipService())
        {
        }

        public AccountController(IFormsAuthenticationService formsService, IMembershipService membershipService)
        {
            this.FormsService = formsService;
            this.MembershipService = membershipService;
        }

        public IFormsAuthenticationService FormsService { get; set; }

        public IMembershipService MembershipService { get; set; }

        public ActionResult LogOn()
        {
            return this.View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    this.FormsService.SignIn(model.UserName, model.RememberMe);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    this.ModelState.AddModelError("LogOnViewModel", "El nombre de usuario o contraseña es inválido.");
                }
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        public ActionResult LogOff()
        {
            this.FormsService.SignOut();

            return this.RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            this.ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                if (this.MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return this.RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    this.ModelState.AddModelError("ChangePasswordViewModel", "La contraseña actual es incorrecta o la nueva contraseña es inválida.");
                }
            }

            // If we got this far, something failed, redisplay form
            this.ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            
            return this.View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return this.View();
        }
    }
}
