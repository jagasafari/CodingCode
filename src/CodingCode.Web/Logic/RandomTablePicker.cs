namespace CodingCode.Web.Logic
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Microsoft.Data.Entity;

    public class RandomTablePicker : IRandomTablePicker
    {
        public Type GetRandomTable(DbContext ctx)
        {
            var types = ctx.GetType().Assembly.GetTypes();
            var names = new List<Type>();
            foreach(var t in types)
            {
                var nam = t.Name;
                if(nam.Contains("Context")) continue;
                if(nam.Contains("<")) continue;
                names.Add(t);
            }
            var random = new Random();
            var next = random.Next(names.Count);
            return names[next];
        }
    }
}