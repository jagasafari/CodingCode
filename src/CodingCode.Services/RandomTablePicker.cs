﻿namespace CodingCode.Services
{
    using System;
    using System.Collections.Generic;
    using CodingCode.Abstraction;
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
            var next = new Random()
                .Next(names.Count);
            return names[next];
        }
    }
}