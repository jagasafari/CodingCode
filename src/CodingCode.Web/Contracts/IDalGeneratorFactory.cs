namespace CodingCode.Web.Contracts
{
    using Logic;
    using ViewModel;

    public interface IDalGeneratorFactory
    {
        DalInfoViewModel DalInfoViewModel { get; set; }
        DalGenerator Create();
    }
}