namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using CallCenter.SelfManagement.Data;

    class Program
    {
        static void Main(string[] args)
        {
            var processor = new FilesProcessor(new MetricsRepository());
            processor.Process();
            Console.Read();
        }
    }
}
