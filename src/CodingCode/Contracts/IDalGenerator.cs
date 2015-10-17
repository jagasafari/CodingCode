namespace CodingCode.Contracts
{
    using System;

    public interface IDalGenerator : IDisposable
    {
        new void Dispose();
        void CreateDalDirectory();
        void CopyProjectJson();
        void Restore();
        void Scaffold();
        void CodeContext();
        void CodeEntities();
        void Build();
        dynamic InstantiateDbContext();
    }
}