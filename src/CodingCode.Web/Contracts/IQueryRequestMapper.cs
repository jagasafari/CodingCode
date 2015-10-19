namespace CodingCode.Web.Contracts
{
    using System;
    using ViewModels;

    public interface IQueryRequestMapper
    {
        TableViewModel MapToViewModel(Type randomType, dynamic dbContext);
    }
}