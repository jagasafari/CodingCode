namespace CodingCode.Logic
{
    using System;
    using System.Collections.Generic;

    public class DbContextWrapper
    {
        public DbContextWrapper()
        {
            DbContextDictionary = new Dictionary<string, dynamic>();
        }

        private Dictionary<string, dynamic> DbContextDictionary { get; }

        public dynamic this[ string index ]
        {
            get { return DbContextDictionary[index]; }
            set
            {
                if ( DbContextDictionary.ContainsKey( index ) )
                    throw new Exception(
                        $"Database context with the name {index} already exists" );
                DbContextDictionary[index] = value;
            }
        }

        public bool Exists( string assemblyName )
        {
            return DbContextDictionary.ContainsKey( assemblyName );
        }
    }
}