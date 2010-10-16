namespace CallCenter.SelfManagement.FilesProcessor
{
    using CallCenter.SelfManagement.Data;

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new FilesProcessor(new MetricsRepository());
            processor.Process();
        }
    }
}
