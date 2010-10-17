namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Globalization;
    using System.Linq;
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

        public bool ExistsUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            var user = this.provider.GetUser(userName, false);

            return user != null;
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

        public void AddUserToRol(string userName, SelfManagementRoles role)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(userName);
            }

            var roleName = role.ToString();
            if (!Roles.RoleExists(roleName))
            {
                throw new ArgumentException("role", string.Format(CultureInfo.InvariantCulture, "El rol {0} no existe.", roleName));
            }

            Roles.AddUserToRole(userName, roleName);
        }

        public void CreateProfile(string userName, string dni, string name, string lastName, decimal? grossSalary, string workday, string status, DateTime? incorporationDate)
        {
            var profileBase = ProfileBase.Create(userName, true);

            profileBase.SetPropertyValue("DNI", dni);
            profileBase.SetPropertyValue("Names", name);
            profileBase.SetPropertyValue("Surname", lastName);
            profileBase.SetPropertyValue("Workday", workday);
            profileBase.SetPropertyValue("Status", status);

            if (grossSalary.HasValue)
            {
                profileBase.SetPropertyValue("GrossSalary", grossSalary.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (incorporationDate.HasValue)
            {
                profileBase.SetPropertyValue("IncorporationDate", incorporationDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            }

            profileBase.Save();
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

        public Agent GetAgent(string userName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                    .Where(a => a.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }
        }

        public Supervisor GetSupervisor(string userName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                    .Where(s => s.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
            }
        }
    }
}
