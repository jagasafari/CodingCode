namespace CodingCode.Web.Contracts
{
    using ViewModels;

    public interface ICodeFounderFactory 
    {
        ICodeFounder Create(SearchedCodeViewModel model);
    }
}