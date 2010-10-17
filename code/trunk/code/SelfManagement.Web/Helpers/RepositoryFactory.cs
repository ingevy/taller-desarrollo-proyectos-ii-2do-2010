namespace CallCenter.SelfManagement.Web.Helpers
{
    using CallCenter.SelfManagement.Data;
    using CallCenter.SelfManagement.Web.Services;

    public class RepositoryFactory
    {
        public ICampaingRepository GetCampaingRepository()
        {
            return new CampaingRepository();
        }

        public IMembershipService GetMembershipService()
        {
            return new MembershipService();
        }

        public IFormsAuthenticationService GetFormsAuthenticationService()
        {
            return new FormsAuthenticationService();
        }
    }
}
