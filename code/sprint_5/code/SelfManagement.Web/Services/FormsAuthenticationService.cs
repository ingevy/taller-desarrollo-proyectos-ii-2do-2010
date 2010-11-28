namespace CallCenter.SelfManagement.Web.Services
{
    using System;
    using System.Web.Security;

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);

        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}