﻿namespace CodingCode.Web.Contracts
{
    using System;
    using ViewModel;

    public interface IQueryRequestMapper
    {
        TableViewModel MapToViewModel(Type randomType, dynamic dbContext);
    }
}