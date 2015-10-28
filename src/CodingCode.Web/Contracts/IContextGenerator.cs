namespace CodingCode.Web.Contracts
{
    using System.Threading.Tasks;
    using ViewModels;

    public interface IContextGenerator
    {
        Task<object> GenerateAsync(DalInfoViewModel dalInfo, string assemblyName);
    }
}