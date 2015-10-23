namespace CodingCode.Web.Contracts
{
    using Logic;
    using ViewModels;

    public interface IDalGeneratorFactory
    {
        DalInfoViewModel DalInfoViewModel { get; set; }
        string ApplicationBasePath { get; set; }
        DalGenerator Create();
    }
}