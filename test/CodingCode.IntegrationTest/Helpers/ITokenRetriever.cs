namespace CodingCode.IntegrationTest.Helpers
{
    public interface ITokenRetriever {
        string RetrieveAntiForgeryToken(string htmlContent);
    }
}