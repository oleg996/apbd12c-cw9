namespace Tutorial9.Services;
using Tutorial9.Model;
public interface IDbService
{
    Task DoSomethingAsync();
    Task ProcedureAsync();
    Task<Boolean> does_product_exists(int id);

    Task<Boolean> does_warehouse_exists(int id);

    Task<int> does_order_exists(int id,DateTime date,int am);

    Task complete_order(WarehouseRequeustDTO warehouseRequeustDTO);
}