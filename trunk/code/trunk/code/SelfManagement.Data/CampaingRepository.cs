namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;

    public class CampaingRepository : ICampaingRepository
    {
        public bool ExistsAgent(int agentId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from a in ctx.Agents
                            where a.InnerUserId == agentId
                            select a;

                return (query.ToList().Count > 0);
            }
        }

        public bool ExistsSupervisor(int supervisorId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from s in ctx.Supervisors
                            where s.InnerUserId == supervisorId
                            select s;

                return (query.ToList().Count > 0);
            }
        }

        public bool ExistsCampaing(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from c in ctx.Campaings
                            where c.Id == campaingId
                            select c;

                return (query.ToList().Count > 0);
            }
        }

        public IList<Campaing> RetrieveAllCampaings(int pageSize, int pageNumber)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings
                        .Include("Customer")
                        .OrderBy(c => c.Id)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
            }
        }

        public int CountAllCampaings()
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings.Count();
            }
        }

        public IList<Metric> RetrieveAvailableMetrics()
        {
            using (var ctx = new SelfManagementEntities())
            {
                var query = from m in ctx.Metrics
                            select m;

                return query.ToList();
            }
        }

        public IList<Customer> RetrieveCustomersByName(string customerName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Customers
                    .FullTextSearch(customerName)
                    .ToList();
            }
        }

        public Campaing RetrieveCurrentCampaingByUserId(int innerUserId)
        {
            var today = DateTime.Now;

            using (var ctx = new SelfManagementEntities())
            {
                var campingUser = ctx.CampaingUsers
                    .Include("Campaing")
                    .Where(cu => cu.InnerUserId == innerUserId)
                    .FirstOrDefault(cu => (cu.BeginDate <= today) && (cu.EndDate >= today));

                return campingUser != null ? campingUser.Campaing : null;
            }
        }

        public IList<Campaing> RetrieveCampaingsByUserId(int innerUserId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.CampaingUsers
                    .Include("Campaing")
                    .Where(cu => cu.InnerUserId == innerUserId)
                    .Select(cu => cu.Campaing)
                    .ToList();
            }
        }

        public IList<Supervisor> RetrieveAvailableSupervisors(DateTime beginDate, DateTime? endDate = null)
        {
            using (var ctx = new SelfManagementEntities())
            {
                IQueryable<Supervisor> query;

                if (endDate.HasValue)
                {
                    var finalDate = endDate.Value;

                    query = from s in ctx.Supervisors
                            where ctx.CampaingUsers
                                    .Where(cu => cu.InnerUserId == s.InnerUserId)
                                    .All(cu => ((beginDate < cu.BeginDate) || (beginDate > cu.EndDate)) && ((finalDate < cu.BeginDate) || (finalDate > cu.EndDate)))
                            select s;
                }
                else
                {
                    query = from s in ctx.Supervisors
                            where ctx.CampaingUsers
                                    .Where(cu => cu.InnerUserId == s.InnerUserId)
                                    .All(cu => beginDate > cu.EndDate)
                            select s;
                }

                return query.ToList();
            }
        }

        public IList<string> RetrieveAvailableMonthsByCampaing(int campaingId)
        {
            var campaing = this.RetrieveCampaingById(campaingId);

            if (campaing != null)
            {
                return GetMonthsList(campaing.BeginDate, campaing.EndDate.HasValue ? campaing.EndDate.Value : DateTime.Now);
            }

            throw new ArgumentException("campaingId");
        }

        public Campaing RetrieveCampaingById(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Campaings
                    .Where(c => c.Id == campaingId)
                    .FirstOrDefault();
            }
        }

        public IList<Campaing> RetrieveCampaingsByUserIdAndDate(int innerUserId, DateTime date)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.CampaingUsers
                    .Include("Campaing")
                    .Where(cu => cu.InnerUserId == innerUserId)
                    .Where(cu => ((cu.BeginDate <= date) && (cu.EndDate >= date)) || ((cu.BeginDate.Year == date.Year) && (cu.BeginDate.Month == date.Month)) || ((cu.EndDate.Year == date.Year) && (cu.EndDate.Month == date.Month)))
                    .Select(cu => cu.Campaing)
                    .ToList();
            }
        }

        public IList<CampaingMetricLevel> RetrieveCampaingMetricLevels(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.CampaingMetricLevels
                    .Include("Metric")
                    .Where(cml => cml.CampaingId == campaingId)
                    .ToList();
            }
        }

        public IList<Supervisor> RetrieveCampaingSupervisors(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                    .Where(s => ctx.CampaingUsers.Any(cu => (cu.InnerUserId == s.InnerUserId) && (cu.CampaingId == campaingId)))
                    .ToList();
            }
        }

        public int CountCampaingSupervisors(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Supervisors
                    .Where(s => ctx.CampaingUsers.Any(cu => (cu.InnerUserId == s.InnerUserId) && (cu.CampaingId == campaingId)))
                    .Count();
            }
        }

        public IList<Agent> RetrieveCampaingAgents(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                    .Where(a => ctx.CampaingUsers.Any(cu => (cu.InnerUserId == a.InnerUserId) && (cu.CampaingId == campaingId)))
                    .ToList();
            }
        }

        public int CountCampaingAgents(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                return ctx.Agents
                    .Where(a => ctx.CampaingUsers.Any(cu => (cu.InnerUserId == a.InnerUserId) && (cu.CampaingId == campaingId)))
                    .Count();
            }
        }

        public IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                if (!ctx.Supervisors.Any(s => s.InnerUserId == supervisorId))
                {
                    throw new ArgumentException("Solo se pueden agentes asignados a un Supervisor.", "supervisorId");
                }

                return ctx.Agents
                    .Where(a => ctx.SupervisorAgents.Any(sa => (sa.AgentId == a.InnerUserId) && (sa.SupervisorId == supervisorId)))
                    .ToList();
            }
        }

        public int RetrieveOrCreateCustomerIdByName(string customerName)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var customer = ctx.Customers
                    .Where(c => c.Name == customerName)
                    .FirstOrDefault();

                if (customer == null)
                {
                    customer = new Customer { Name = customerName };
                    ctx.Customers.AddObject(customer);
                    ctx.SaveChanges();
                }

                return customer.Id;
            }
        }

        public void EndCampaing(int campaingId)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var campaing = ctx.Campaings
                               .FirstOrDefault(c => c.Id == campaingId);
                var originalEndDate = campaing.EndDate.HasValue ? campaing.EndDate.Value.Date : DateTime.MaxValue.Date;
                
                campaing.EndDate = DateTime.Now;
                var campaingUsers = ctx.CampaingUsers
                                    .Where(cu => (cu.CampaingId == campaingId) && (cu.EndDate.Year == originalEndDate.Year) && (cu.EndDate.Month == originalEndDate.Month) && (cu.EndDate.Day == originalEndDate.Day));

                foreach (var campaingUser in campaingUsers)
                {
                    campaingUser.EndDate = campaing.EndDate.Value;
                }

                ctx.SaveChanges();
            }
        }

        public void AddAgent(int campaingId, Agent agent)
        {
            using (var ctx = new SelfManagementEntities())
            {
                var campaing = ctx.Campaings
                               .Where(c => c.Id == campaingId)
                               .FirstOrDefault();

                if (campaing != null)
                {
                    var incorporationDate = DateTime.Parse(agent.IncorporationDate, new CultureInfo("es-AR",false));
                    var startDate = (campaing.BeginDate < incorporationDate) ? incorporationDate : campaing.BeginDate;
                    var campaingUser = new CampaingUser { CampaingId = campaing.Id, InnerUserId = agent.InnerUserId, BeginDate = startDate, EndDate = campaing.EndDate.GetValueOrDefault(DateTime.MaxValue) };
                    campaing.CampaingUsers.Add(campaingUser);
                }

                ctx.SaveChanges();
            }
        }

        public int CreateCampaing(Campaing campaing)
        {
            using (var ctx = new SelfManagementEntities())
            {
                ctx.Campaings.AddObject(campaing);
                ctx.SaveChanges();

                return campaing.Id;
            }
        }

        public void SaveCampaingMetricLevels(IEnumerable<CampaingMetricLevel> campaingMetricLevels)
        {
            if (campaingMetricLevels.Where(cml => cml.Enabled).Count() != 3)
            {
                throw new ArgumentException("Por cada campaña se requieren tres métricas obligatoriamente.", "campaingMetricLevels");
            }

            var campaingId = campaingMetricLevels.FirstOrDefault().CampaingId;
            if (!campaingMetricLevels.All(cml => cml.CampaingId == campaingId))
            {
                throw new ArgumentException("Todas las metricas deben pertenecer a una misma campaña.", "campaingMetricLevels");
            }

            using (var ctx = new SelfManagementEntities())
            {
                foreach (var campaingMetricLevel in campaingMetricLevels)
                {
                    var original = ctx.CampaingMetricLevels
                        .Where(cml => (cml.CampaingId == campaingMetricLevel.CampaingId) && (cml.MetricId == campaingMetricLevel.MetricId))
                        .FirstOrDefault();

                    if (original != null)
                    {
                        original.OptimalLevel = campaingMetricLevel.OptimalLevel;
                        original.ObjectiveLevel = campaingMetricLevel.ObjectiveLevel;
                        original.MinimumLevel = campaingMetricLevel.MinimumLevel;
                        original.Enabled = campaingMetricLevel.Enabled;
                    }
                    else
                    {
                        ctx.CampaingMetricLevels.AddObject(campaingMetricLevel);
                    }
                }

                var metricIds = campaingMetricLevels.Select(cml => cml.MetricId).ToList();
                var campaingMetricLevelsToDisable = ctx.CampaingMetricLevels
                    .Where(cml => cml.CampaingId == campaingId)
                    .Where(cml => !metricIds.Contains(cml.MetricId));

                foreach (var campaingMetricLevel in campaingMetricLevelsToDisable)
                {
                    campaingMetricLevel.Enabled = false;
                }                    

                ctx.SaveChanges();
            }
        }

        public void SaveCampaingSupervisors(IEnumerable<CampaingUser> campaingSupervisors)
        {
            var campaingId = campaingSupervisors.FirstOrDefault().CampaingId;
            if (!campaingSupervisors.All(cu => cu.CampaingId == campaingId))
            {
                throw new ArgumentException("Todas los supervisores deben pertenecer a una misma campaña.", "campaingSupervisors");
            }

            using (var ctx = new SelfManagementEntities())
            {
                var supervisorIds = campaingSupervisors.Select(cu => cu.InnerUserId).ToList();

                if (!supervisorIds.All(supervisorId => ctx.Supervisors.Any(s => s.InnerUserId == supervisorId)))
                {
                    throw new ArgumentException("Solo se pueden asignar usarios que tengan rol de Supervisor.", "campaingSupervisors");
                }
                
                foreach (var supervisor in campaingSupervisors)
                {
                    ctx.CampaingUsers.AddObject(supervisor);
                }

                foreach (var agent in ctx.Agents.Where(a => a.SupervisorId.HasValue && supervisorIds.Contains(a.SupervisorId.Value)))
                {
                    var supervisor = campaingSupervisors.FirstOrDefault(cu => cu.InnerUserId == agent.SupervisorId);

                    ctx.CampaingUsers.AddObject(new CampaingUser { CampaingId = campaingId, InnerUserId = agent.InnerUserId, BeginDate = supervisor.BeginDate, EndDate = supervisor.EndDate });
                }

                ctx.SaveChanges();
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
    }
}
