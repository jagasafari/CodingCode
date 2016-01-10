using Microsoft.Extensions.Logging;
using CodingCode.ViewModel;
using Common.ProcessExecution.Abstraction;
using CodingCode.Abstraction;
using CodingCode.Model;
using Microsoft.Extensions.OptionsModel;

namespace CodingCode.Services
{
    public class DalGeneratorFactory : IDalGeneratorFactory
    {
        private ILogger<DalGenerator> _logger;
        private IFinishingExecutorFactory _executorFactory;
        private DnxOptions _options;
        private IDataAccessSettingsMapper _mapper;
        public DalGeneratorFactory(ILogger<DalGenerator> logger, IFinishingExecutorFactory executorFactory, 
            IOptions<DnxOptions> options, IDataAccessSettingsMapper mapper)
        {
            _logger = logger;
            _executorFactory = executorFactory;
            _options = options.Value; 
            _mapper = mapper;
        }

        public IDalGenerator Create(DataAccessViewModel dataAccessViewModel, string assemblyName)
        {
            return new DalGenerator(_executorFactory, _logger, _options)
            {
                DataAccessConfiguratios = _mapper.Map(dataAccessViewModel, assemblyName)
            };
        }
    }
}