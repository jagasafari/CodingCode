namespace CodingCode.Web.Logic
{
    using Contracts;
    using ViewModels;

    public class CodeFounderFactory:ICodeFounderFactory
    {
        public ICodeFounder Create(SearchedCodeViewModel model)
        {
            var keywords = model.Keywords.Split(',');
            var numKeywords = keywords.Length;
            var searchedPath = model.SearchedPath;
            var fileExtensionPattern = model.FileExtensionPattern;
            return new CodeFounder(numKeywords, keywords,searchedPath,fileExtensionPattern);
        }
    }
}