namespace CodingCode.Services
{
    using System;
    using System.Linq;

    internal static class NonNavTypesChecker
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

        public static bool Check(Type propertyType) => NonNavTypes.Contains(propertyType);

        internal static bool Check(string codeLine) => NonNavTypesNames.Any(codeLine.Contains);
    }
}