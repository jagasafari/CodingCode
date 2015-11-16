namespace CodingCode.Services
{
    using Contracts;
    using Model;
    using ProcessExecution;

    public class CodingCodeProviderServices
    {
        public IDalGenerator DalGenerator(
            DataAccessSettings dataAccessSettings) =>
                new DalGenerator
                {
                    DataAccessSettings = dataAccessSettings,
                    ProcessProviderServices = new ProcessProviderServices()
                };
    }
}