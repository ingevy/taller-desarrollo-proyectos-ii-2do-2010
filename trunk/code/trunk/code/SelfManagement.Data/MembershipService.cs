namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
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

        public MembershipCreateStatus CreateUser(int innerUserId, string userName, string password, string email)
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
            this.SetInnerUserId(userName, innerUserId);

            return status;
        }

        private void SetInnerUserId(string userName, int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var user = ctx.aspnet_Users
                           .FirstOrDefault(a => a.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

                user.InnerUserId = innerUserId;

                ctx.SaveChanges();
            }
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

        public IList<string> RetrieveAvailableMonthsByUser(int innerUserId)
        {
            var today = DateTime.Now;
            var agent = this.RetrieveAgent(innerUserId);

            if (agent != null)
            {
                return GetMonthsList(DateTime.ParseExact(agent.IncorporationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None), today);            
            }

            var supervisor = this.RetrieveSupervisor(innerUserId);

            if (supervisor != null)
            {
                return GetMonthsList(DateTime.ParseExact(supervisor.IncorporationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None), today);
            }

            throw new ArgumentException("innerUserId");
        }

        public Agent RetrieveAgent(string userName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                    .FirstOrDefault(a => a.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public Agent RetrieveAgent(int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                    .FirstOrDefault(a => a.InnerUserId == innerUserId);
            }
        }

        public Supervisor RetrieveSupervisor(string userName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                    .FirstOrDefault(s => s.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public Supervisor RetrieveSupervisor(int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                    .FirstOrDefault(s => s.InnerUserId == innerUserId);
            }
        }

        public MonthlySchedule RetrieveMonthlySchedule(int innerUserId, DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.MonthlySchedules
                    .FirstOrDefault(ms => (ms.InnerUserId == innerUserId) && (ms.Year == date.Year) && (ms.Month == date.Month));               
            }
        }

        public int RetrieveInnerUserIdByUserName(string userName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var user = ctx.aspnet_Users
                    .FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

                if (user == null)
                {
                    throw new ArgumentException("userName", "El nombre de usuario no existe.");
                }

                return user.InnerUserId;
            }
        }

        private static IList<string> GetMonthsList(DateTime from, DateTime to)
        {
            var months = new List<string>();

            while ((from.Date <= to.Date) || ((from.Date.Year <= to.Date.Year) && (from.Date.Month <= to.Date.Month)))
            {
                months.Add(string.Format(CultureInfo.InvariantCulture, "{0}-{1}", from.Year, from.Month.ToString("D2", CultureInfo.InvariantCulture)));
                from = from.AddMonths(1);
            }

            return months;
        }

        public IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId, int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Supervisors.Any(s => s.InnerUserId == supervisorId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "El identificador de usario {0} no corresponde a un rol Supervisor.",
                            supervisorId),
                        "supervisorId");
                }

                return ctx.Agents
                        .Where(a => ctx.SupervisorAgents.Any(sa => (sa.SupervisorId == supervisorId) && (sa.AgentId == a.InnerUserId)))
                        .OrderBy(a => a.InnerUserId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public IList<Agent> RetrieveAgentsByCampaingId(int campaingId, int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Campaings.Any(c => c.CustomerId == campaingId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "No se encontró una campaña con el identificador {0}.",
                            campaingId),
                        "campaingId");
                }

                return ctx.Agents
                        .Where(a => ctx.CampaingUsers.Any(ca => (ca.CampaingId == campaingId) && (ca.InnerUserId == a.InnerUserId)))
                        .OrderBy(a => a.InnerUserId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public IList<Agent> SearchAgents(string searchCriteria)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                        .Where(a => a.DNI.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.UserName.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.UserName.Contains(searchCriteria)
                                    || a.Workday.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.Workday.Contains(searchCriteria)
                                    || a.Status.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.Status.Contains(searchCriteria)
                                    || a.Name.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.Name.Contains(searchCriteria)
                                    || a.LastName.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.LastName.Contains(searchCriteria)
                                    || a.IncorporationDate.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                    || a.IncorporationDate.Contains(searchCriteria)
                                    || a.GrossSalary.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(a => a.InnerUserId)
                        .ToList();
            }
        }

        public IList<Agent> SearchAgentsBySupervisorId(int supervisorId, string searchCriteria)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                        .Where(a => (a.SupervisorId == supervisorId) 
                                     && (a.DNI.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.UserName.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.UserName.Contains(searchCriteria)
                                         || a.Workday.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.Workday.Contains(searchCriteria)
                                         || a.Status.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.Status.Contains(searchCriteria)
                                         || a.Name.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.Name.Contains(searchCriteria)
                                         || a.LastName.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.LastName.Contains(searchCriteria)
                                         || a.IncorporationDate.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)
                                         || a.IncorporationDate.Contains(searchCriteria)
                                         || a.GrossSalary.Equals(searchCriteria, StringComparison.OrdinalIgnoreCase)))
                        .OrderBy(a => a.InnerUserId)
                        .ToList();
            }
        }

        public IList<Agent> RetrieveAllAgents(int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                        .OrderBy(a => a.InnerUserId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public IList<Supervisor> RetrieveSupervisorsByCampaingId(int campaingId, int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Campaings.Any(c => c.CustomerId == campaingId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "No se encontró una campaña con el identificador {0}.",
                            campaingId),
                        "campaingId");
                }

                return ctx.Supervisors
                        .Where(s => ctx.CampaingUsers.Any(ca => (ca.CampaingId == campaingId) && (ca.InnerUserId == s.InnerUserId)))
                        .OrderBy(s => s.InnerUserId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public IList<Supervisor> RetrieveAllSupervisors(int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                        .OrderBy(s => s.InnerUserId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public int CountAgentsBySupervisorId(int supervisorId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Supervisors.Any(s => s.InnerUserId == supervisorId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "El identificador de usario {0} no corresponde a un rol Supervisor.",
                            supervisorId),
                        "supervisorId");
                }

                return ctx.Agents
                        .Where(a => ctx.SupervisorAgents.Any(sa => (sa.SupervisorId == supervisorId) && (sa.AgentId == a.InnerUserId)))
                        .Count();
            }
        }

        public int CountAgentsByCampaingId(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Campaings.Any(c => c.CustomerId == campaingId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "No se encontró una campaña con el identificador {0}.",
                            campaingId),
                        "campaingId");
                }

                return ctx.Agents
                        .Where(a => ctx.CampaingUsers.Any(ca => (ca.CampaingId == campaingId) && (ca.InnerUserId == a.InnerUserId)))
                        .Count();
            }
        }

        public int CountAllAgents()
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents.Count();
            }
        }

        public int CountSupervisorsByCampaingId(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Campaings.Any(c => c.CustomerId == campaingId))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "No se encontró una campaña con el identificador {0}.",
                            campaingId),
                        "campaingId");
                }

                return ctx.Supervisors
                        .Where(s => ctx.CampaingUsers.Any(ca => (ca.CampaingId == campaingId) && (ca.InnerUserId == s.InnerUserId)))
                        .Count();
            }
        }

        public int CountAllSupervisors()
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors.Count();
            }
        }
    }
}
