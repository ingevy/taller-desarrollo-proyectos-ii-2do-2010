namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Globalization;
    using System.Web.Profile;
    using System.Web.Security;

    public class MembershipService : IMembershipService
    {
        private readonly MembershipProvider provider;

        public MembershipService()
            : this(Membership.Provider)
        {
        }

        public MembershipService(MembershipProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentException("Value cannot be null or empty.", "provider");
            }

            this.provider = provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return this.provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Value cannot be null or empty.", "password");
            }

            return provider.ValidateUser(userName, password);
        }

        public void CreateProfile(string userName, string dni, string name, string lastName, decimal? grossSalary, string workday, string status, DateTime? incorporationDate)
        {
            var profileBase = ProfileBase.Create(userName, true);

            profileBase.SetPropertyValue("DNI", dni);
            profileBase.SetPropertyValue("Name", name);
            profileBase.SetPropertyValue("LastName", lastName);
            profileBase.SetPropertyValue("GrossSalary", grossSalary);
            profileBase.SetPropertyValue("Workday", workday);
            profileBase.SetPropertyValue("Status", status);

            if (incorporationDate.HasValue)
            {
                profileBase.SetPropertyValue("IncorporationDate", incorporationDate);
            }

            profileBase.Save();
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Value cannot be null or empty.", "password");
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Value cannot be null or empty.", "email");
            }

            MembershipCreateStatus status;
            this.provider.CreateUser(userName, password, email, null, null, true, null, out status);

            return status;
        }

        public void AddUserToRol(string userName, string role)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(userName);
            }

            if (Roles.RoleExists(role))
            {
                throw new ArgumentException("role", string.Format(CultureInfo.InvariantCulture, "El rol {0} no existe.", role));
            }
            
            Roles.AddUserToRole(userName, role);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "userName");
            }

            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Value cannot be null or empty.", "newPassword");
            }

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                var currentUser = this.provider.GetUser(userName, true /* userIsOnline */);

                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }
}
