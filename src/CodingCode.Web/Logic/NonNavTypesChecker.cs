namespace CodingCode.Web.Logic
{
    using System;
    using System.Linq;

    internal class NonNavTypesChecker
    {
        public static Type[] NonNavTypes =
        {
            typeof(int), typeof(int?), typeof(string),
            typeof(decimal), typeof(decimal?),
            typeof(double), typeof(double?), typeof(bool),
            typeof(bool?), typeof(float), typeof(float?),
            typeof(DateTime),
            typeof(DateTime?)
        };

        public static string[] NonNavTypesNames =
        {
            "int", "int?", "string",
            "decimal", "decimal?",
            "double", "double?", "bool",
            "bool?", "float", "float?",
            "DateTime","DateTime?"
        };

        public static bool Check(Type propertyType)
        {
            return NonNavTypes.Contains(propertyType);
        }

        internal static bool Check(string codeLine)
        {
            return NonNavTypesNames.Any(codeLine.Contains);
        }
    }
}