namespace CodingCode.Common
{
    using System;

    public static class Check
    {
        public static T NotNull< T >( T value )
        {
            if(ReferenceEquals( value, null )) throw new ArgumentException();
            return value;
        }
    }
}
