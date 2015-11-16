namespace CodingCode.IntegrationTest.Helpers
{
    using System;
    using System.Linq;
    using System.Xml;

    public class TokenRetriever
    {
        public static string RetrieveAntiForgeryToken(string htmlContent)
        {
            var formStartIndex = htmlContent.IndexOf("<form",
                StringComparison.OrdinalIgnoreCase);
            var formEndIndex = htmlContent.IndexOf("</form>", 
                StringComparison.OrdinalIgnoreCase);

            if(formStartIndex == -1 || formEndIndex == -1)
                throw new Exception("No form found!");

            formEndIndex = formEndIndex + "</form>".Length;

            var htmlDocument = new XmlDocument();
            htmlDocument.LoadXml(htmlContent.Substring(formStartIndex,
                formEndIndex - formStartIndex));

            return
                htmlDocument.GetElementsByTagName("input")
                    .Cast<XmlNode>()
                    .Where(IsToken)
                    .Single().Attributes["value"].Value;

        }

        private static bool IsToken(XmlNode input)=>
            input.Attributes["name"]?.Value ==
                   "__RequestVerificationToken" &&
                   input.Attributes["type"].Value == "hidden";
    }
}