namespace Presentation.Contracts
{
    using System;
    using Microsoft.Data.Entity;

    public interface IRandomTablePicker
    {
        Type GetRandomTable( DbContext ctx );
    }
}