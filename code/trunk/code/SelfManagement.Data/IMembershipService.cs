namespace CallCenter.SelfManagement.Data
{
    using System;
    using System.Web.Security;

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);

        MembershipCreateStatus CreateUser(string userName, string password, string email);

        void CreateProfile(string userName, string dni, string name, string lastName, decimal? grossSalary, string workday, string status, DateTime? incorporationDate);

        void AddUserToRol(string userName, string role);

        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }
}
