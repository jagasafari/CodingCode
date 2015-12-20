namespace CodingCode.Contracts
{
    using System.Threading.Tasks;
    using ViewModel;

    public interface IContextGenerator
    {
        Task<object> GenerateAsync(DataAccessViewModel dataAccessViewModel, string assemblyName);
    }
}