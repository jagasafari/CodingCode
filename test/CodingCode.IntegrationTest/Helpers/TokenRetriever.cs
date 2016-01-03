namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Linq;
    using System.Xml;

    public class TokenRetriever : ITokenRetriever
    {
        public string RetrieveAntiForgeryToken(string htmlContent)
        {
            var formStartIndex = htmlContent.IndexOf("<form", StringComparison.OrdinalIgnoreCase);
            var formEndIndex = htmlContent.IndexOf("</form>", StringComparison.OrdinalIgnoreCase) + "</form>".Length;

            if(formStartIndex == -1) throw new Exception("No form found!");

            var htmlDocument = new XmlDocument();
            htmlDocument.LoadXml(htmlContent.Substring(formStartIndex, formEndIndex - formStartIndex));

            return
                htmlDocument.GetElementsByTagName("input")
                    .Cast<XmlNode>()
                    .Where(IsToken)
                    .Single()
                    .Attributes["value"]
                    .Value;
        }

        private bool IsToken(XmlNode input) => 
            input.IsAttribute("name", "__RequestVerificationToken") 
            && 
            input.IsAttribute("type", "hidden");
                   
    }
    
    public static class XmlNodeExtensions
    {
        public static bool IsAttribute(this XmlNode node, string key, string value) =>
            node.Attributes[key]?.Value == value;
    }
}