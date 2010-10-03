namespace Callcenter.SelfManagement.Metric.Interfaces
{
    using System;

    public interface IDataFile
    {
        string GetExternalSystemName();

        DateTime GetFileDate();

        string GetFilePath();
    }
}
