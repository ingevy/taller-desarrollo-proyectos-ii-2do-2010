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

        MembershipCreateStatus CreateUser(string userName, string password, string email);

        MembershipCreateStatus CreateUser(int innerUserId, string userName, string password, string email);

        void AddUserToRol(string userName, SelfManagementRoles role);

        void CreateProfile(string userName, string dni, string name, string lastName, decimal? grossSalary, string workday, string status, DateTime? incorporationDate);

        bool ChangePassword(string userName, string oldPassword, string newPassword);

        IList<string> RetrieveAvailableMonthsByUser(int innerUserId);

        Agent RetrieveAgent(string userName);

        Agent RetrieveAgent(int innerUserId);

        Supervisor RetrieveSupervisor(string userName);

        Supervisor RetrieveSupervisor(int innerUserId);

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
