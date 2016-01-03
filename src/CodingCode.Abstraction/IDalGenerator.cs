namespace CodingCode.Abstraction
{
    using System;
    using System.Threading.Tasks;

    public interface IDalGenerator : IDisposable
    {
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