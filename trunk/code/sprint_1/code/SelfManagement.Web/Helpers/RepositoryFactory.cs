namespace CallCenter.SelfManagement.Web.Helpers
{
    using CallCenter.SelfManagement.Data;

    public class RepositoryFactory
    {
        public ICampaingRepository GetCampaingRepository()
        {
            return new CampaingRepository();
        }
    }
}
