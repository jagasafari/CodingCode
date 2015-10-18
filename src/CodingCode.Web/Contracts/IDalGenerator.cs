namespace CodingCode.Web.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IDalGenerator : IDisposable
    {
        new void Dispose();
        void CreateDalDirectory();
        void CopyProjectJson();
        Task RestoreAsync();
        Task ScaffoldAsync();
        void CodeContext();
        Task CodeEntitiesAsync();
        Task BuildAsync();
        dynamic InstantiateDbContext();
    }
}