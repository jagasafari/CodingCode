namespace CodingCode.Abstraction{
    using CodingCode.ViewModel;
    using CodingCode.Model;
   public interface IDataAccessSettingsMapper{
       DataAccessConfigurations Map(DataAccessViewModel dataAccessViewModel, string assemblyName);
   }
}