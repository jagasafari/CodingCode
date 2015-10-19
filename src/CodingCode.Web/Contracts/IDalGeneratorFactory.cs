namespace CodingCode.Web.Contracts
{
    using Logic;
    using ViewModels;

    public interface IDalGeneratorFactory
    {
        DalInfoViewModel DalInfoViewModel { get; set; }
        DalGenerator Create();
    }
}