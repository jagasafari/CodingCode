namespace CodingCode.Abstraction
{
    using CodingCode.ViewModel;
    public interface IDalGeneratorFactory
    {
        IDalGenerator Create(DataAccessViewModel dalInfo, string assemblyName, string appBasePath);
    }
}