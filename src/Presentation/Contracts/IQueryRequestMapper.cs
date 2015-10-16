namespace Presentation.Contracts
{
    using System;
    using Model;

    public interface IQueryRequestMapper
    {
        TableViewModel MapToViewModel(Type randomType, dynamic dbContext);
    }
}