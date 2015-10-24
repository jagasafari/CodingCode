namespace UITest
{
    using System;
    using System.Linq;
    using System.Xml;

    public class TokenRetriever
    {
        public string ActionUrl { get; set; }
        public string HtmlContent { get; set; }

        public string RetrieveAntiForgeryToken()
        {
            var formStartIndex = HtmlContent.IndexOf("<form",
                StringComparison.OrdinalIgnoreCase);
            var formEndIndex = HtmlContent.IndexOf("</form>", 
                StringComparison.OrdinalIgnoreCase);

            if(formStartIndex == -1 || formEndIndex == -1)
                throw new Exception("No form found!");

            formEndIndex = formEndIndex + "</form>".Length;

            var htmlDocument = new XmlDocument();
            htmlDocument.LoadXml(HtmlContent.Substring(formStartIndex,
                formEndIndex - formStartIndex));

            if(
                ! htmlDocument.DocumentElement.Attributes
                    .Cast<XmlAttribute>().Any(IsActionUrl))
                throw new Exception("No token found!");

            return 
                htmlDocument.GetElementsByTagName("input")
                    .Cast<XmlNode>()
                    .Where(IsToken)
                    .Single().Attributes["value"].Value;

        }

        private bool IsActionUrl(XmlAttribute attribute)
        {
            return string.Compare(attribute.Name, "action",
                StringComparison.OrdinalIgnoreCase) == 0 &&
                   attribute.Value.EndsWith(
                       ActionUrl, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsToken(XmlNode input)
        {
            return input.Attributes["name"]?.Value ==
                   "__RequestVerificationToken" &&
                   input.Attributes["type"].Value == "hidden";
        }
    }
}