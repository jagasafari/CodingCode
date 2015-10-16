namespace Presentation.Contracts
{
    using Controllers;
    using Logic;

    public interface IDalGeneratorFactory
    {
        DalInfo DalInfo { get; set; }
        DalGenerator Create();
    }
}