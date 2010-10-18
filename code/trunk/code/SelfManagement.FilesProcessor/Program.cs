namespace CallCenter.SelfManagement.FilesProcessor
{
    using System;
    using CallCenter.SelfManagement.Data;

    public class Program
    {
        public static void Main(string[] args)
        {
            var processor = new FilesProcessor(new MetricsRepository());

            processor.ProcessMetrics();

            Console.WriteLine("El procesamiento de las Métricas finalizó. Presione un tecla para finalizar...");
            Console.ReadKey();
        }
    }
}
