using Microsoft.Extensions.Logging;
using CodingCode.Contracts;
using Common.ProcessExecution;
using CodingCode.ViewModel;

namespace CodingCode.Services{
    public class DalGeneratorFactory{
        private ILogger<DalGenerator> _logger;
        private ProcessProviderServices _processProviderServices;
        private ModelsProvider _modelProvider;
        public DalGeneratorFactory(ILogger<DalGenerator> logger, 
                ProcessProviderServices processProviderServices,
                ModelsProvider modelProvider)
        {
            _logger = logger;
            _processProviderServices = processProviderServices;
            _modelProvider = modelProvider;
        }
        
        public IDalGenerator Create(DataAccessViewModel dalInfo, string assemblyName, string appBasePath)
        {
            return new DalGenerator(_processProviderServices, _logger){
                DataAccessSettings = _modelProvider.DataAccessSettings(dalInfo, assemblyName, appBasePath)
                };
        }
    }
}