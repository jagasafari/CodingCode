namespace UITest
{
    using System.Collections.Generic;

    public class TestDatabase
    {
        public IEnumerable<string> GetTableNames()
        {
            string[] tableNames =
            {
                "Categories", "CustomerCustomerDemo",
                "CustomerDemographics",
                "Customers", "EmployeeTerritories", "Employees",
                "Order_Details", "Orders", "Products", "Region",
                "Shippers", "Suppliers", "Territories"
            };
            return tableNames;
        }
    }
}