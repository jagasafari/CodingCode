namespace CodingCode.Services
{
    using System;
    using CodingCode.Abstraction;
    using Microsoft.Data.Entity;

    public class RandomTablePicker : IRandomTablePicker
    {
        public Type GetRandomTable(DbContext ctx)
        {
            var types = ctx.GetType().Assembly.GetTypes();
            
            while (true)
            {
                var randomIndex = new Random().Next(types.Length);
                if(IsEntityType(types[randomIndex].Name)) return types[randomIndex];
            }
        }
        
        private static bool IsEntityType(string typeName){
           return !typeName.Contains("Context") && !typeName.Contains("<>"); 
        }
    }
}