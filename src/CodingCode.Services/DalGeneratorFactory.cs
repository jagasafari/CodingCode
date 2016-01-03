using Microsoft.Extensions.Logging;
using CodingCode.ViewModel;
using Common.ProcessExecution.Abstraction;
using CodingCode.Abstraction;

namespace CodingCode.Services{
    public class DalGeneratorFactory : IDalGeneratorFactory 
    {
        private ILogger<DalGenerator> _logger;
        private IFinishingExecutorFactory _executorFactory;
        private ModelsProvider _modelProvider;
        public DalGeneratorFactory(ILogger<DalGenerator> logger, 
                IFinishingExecutorFactory executorFactory,
                ModelsProvider modelProvider)
        {
            _logger = logger;
            _executorFactory = executorFactory;
            _modelProvider = modelProvider;
        }
        
        public IDalGenerator Create(DataAccessViewModel dalInfo, string assemblyName, string appBasePath)
        {
            return new DalGenerator(_executorFactory, _logger){
                DataAccessSettings = _modelProvider.DataAccessSettings(dalInfo, assemblyName, appBasePath)
                };
        }
    }
}