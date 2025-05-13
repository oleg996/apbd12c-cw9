namespace Tutorial9.Services;

public interface IDbService
{
    Task DoSomethingAsync();
    Task ProcedureAsync();
    Task<Boolean> does_product_exists(int id);

    Task<Boolean> does_warehouse_exists(int id);
}