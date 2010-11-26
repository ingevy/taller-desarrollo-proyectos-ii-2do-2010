namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Collections.Generic;
    using System.Web.Security;

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);

        bool ExistsUser(string userName);

        bool ExistsUser(int innerUserId);

        MembershipCreateStatus CreateUser(string userName, string password, string email);

        MembershipCreateStatus CreateUser(int innerUserId, string userName, string password, string email);

        void AddUserToRol(string userName, SelfManagementRoles role);

        void CreateProfile(string userName, string dni, string name, string lastName, decimal? grossSalary, string workday, string status, DateTime? incorporationDate);

        bool ChangePassword(string userName, string oldPassword, string newPassword);

        IList<string> RetrieveAvailableMonthsByUser(int innerUserId);

        Agent RetrieveAgent(string userName);

        Agent RetrieveAgent(int innerUserId);

        IList<Agent> RetrieveAgentsBySupervisorId(int supervisorId, int pageSize, int pageNumber);

        int CountAgentsBySupervisorId(int supervisorId);

        IList<Agent> RetrieveAgentsByCampaingId(int campaingId, int pageSize, int pageNumber);

        int CountAgentsByCampaingId(int campaingId);

        IList<Agent> SearchAgents(string searchCriteria);

        IList<Agent> SearchAgentsBySupervisorId(int supervisorId, string searchCriteria);

        IList<Agent> RetrieveAllAgents(int pageSize, int pageNumber);

        int CountAllAgents();
        
        Supervisor RetrieveSupervisor(string userName);

        Supervisor RetrieveSupervisor(int innerUserId);

        IList<Supervisor> SearchSupervisors(string searchCriteria);

        IList<Supervisor> RetrieveSupervisorsByCampaingId(int campaingId, int pageSize, int pageNumber);

        int CountSupervisorsByCampaingId(int campaingId);

        IList<Supervisor> RetrieveAllSupervisors(int pageSize, int pageNumber);

        int CountAllSupervisors();

        MonthlySchedule RetrieveMonthlySchedule(int innerUserId, DateTime date);

        int RetrieveInnerUserIdByUserName(string userName);
    }

    public enum SelfManagementRoles
    {
        AccountManager,
        Agent,
        ITManager,
        Supervisor
    }
}
