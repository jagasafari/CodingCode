namespace CodingCode.Abstraction
{
    using ViewModel;

    public interface IQueryRequestMapper
    {
        TableViewModel MapToViewModel<T>(dynamic dbContext) where T:class;
    }
}