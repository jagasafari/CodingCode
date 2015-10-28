namespace CodingCode.Web.Contracts
{
    using ViewModels;

    public interface ICodeFounder
    {
        string[] GetMachingFiles(SearchedCodeViewModel searchedCode);
    }
}