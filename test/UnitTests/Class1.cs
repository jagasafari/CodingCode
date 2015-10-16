namespace UnitTests
{
    using Presentation.Contracts;
    using Presentation.Logic;
    using Xunit;

    public class Class1
    {
        [Theory]
        [InlineData(
            @"Data Source=localhost\DEV2008R2;Initial Catalog=Reserves;Integrated Security=True",
            "localhost_DEV2008R2_Reserves" )]
        [InlineData(
            @"Server=DELL\SQLEXPRESS;Database=Northwind;Trusted_Connection=True;MultipleActiveResultSets=true",
            "DELL_SQLEXPRESS_Northwind" )]
        public void ContextNameTest( string connection,
            string expectedContextName )
        {
            var contextName =
                DalGenerator.GenerateAssemblyName( connection );
            Assert.Equal( expectedContextName, contextName );
        }
    }
}