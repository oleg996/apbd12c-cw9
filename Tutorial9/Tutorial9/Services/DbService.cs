using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial9.Model;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;
    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task DoSomethingAsync()
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        // BEGIN TRANSACTION
        try
        {
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 1);
            command.Parameters.AddWithValue("@Name", "Animal1");
        
            await command.ExecuteNonQueryAsync();
        
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO Animal VALUES (@IdAnimal, @Name);";
            command.Parameters.AddWithValue("@IdAnimal", 2);
            command.Parameters.AddWithValue("@Name", "Animal2");
        
            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            
            await transaction.RollbackAsync();
            throw;
        }
        // END TRANSACTION
    }

    public async Task ProcedureAsync()
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "NazwaProcedury";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@Id", 2);
        
        await command.ExecuteNonQueryAsync();
        
    }

    public async Task<Boolean> does_product_exists(int id){

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand("select count(*) from Product p where p.IdProduct  = @id",connection);
        

        command.Parameters.Add("@id",SqlDbType.Int).Value = id;
    
        await connection.OpenAsync();

        using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) == 1)
                        return true;


                }
            }



        return false;
    }
    public async Task<Boolean> does_warehouse_exists(int id){

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand("select count(*) from Warehouse w where w.idWarehouse = @id",connection);
        

        command.Parameters.Add("@id",SqlDbType.Int).Value = id;
    
        await connection.OpenAsync();

        using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (reader.GetInt32(0) == 1)
                        return true;


                }
            }



        return false;
    }

    public async Task<int> does_order_exists(int id,DateTime date,int am){





        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand("select IdOrder from [Order] o  where o.IdProduct  = @id and o.FulfilledAt is null and o.CreatedAt < @dt and o.Amount = @am",connection);
        

        command.Parameters.Add("@id",SqlDbType.Int).Value = id;
        command.Parameters.Add("@dt",SqlDbType.DateTime).Value = date;
        command.Parameters.Add("@am",SqlDbType.Int).Value = am;
    
        await connection.OpenAsync();

        using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);


                }
            }



        return -1;
    }


    
    public async Task<int> complete_order(WarehouseRequeustDTO warehouseRequeustDTO)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;


        int orederid = await does_order_exists(warehouseRequeustDTO.IdProduct,warehouseRequeustDTO.CreatedAt,warehouseRequeustDTO.Amount);
        int id = -1;
        try
        {
            command.CommandText = "Insert into Product_Warehouse  (IdWarehouse, IdProduct,IdOrder,Amount,Price,CreatedAt) Values(@idw,@idp,@ido,@am, CAST((select Price from Product p  where p.IdProduct = @idp) AS INTEGER) * @am , GETDATE())	";
            command.Parameters.AddWithValue("@idw", warehouseRequeustDTO.IdWarehouce);
            command.Parameters.AddWithValue("@idp", warehouseRequeustDTO.IdProduct);
            command.Parameters.AddWithValue("@ido", orederid);
            command.Parameters.AddWithValue("@am", warehouseRequeustDTO.Amount);
        
            await command.ExecuteNonQueryAsync();
        
            command.Parameters.Clear();
            command.CommandText = "UPDATE [Order] SET FulfilledAt = GETDATE()  WHERE IdProduct  = @id and FulfilledAt is null and CreatedAt < @dt and Amount = @am";
            command.Parameters.AddWithValue("@id", warehouseRequeustDTO.IdProduct);
            command.Parameters.AddWithValue("@dt", warehouseRequeustDTO.CreatedAt);
            command.Parameters.AddWithValue("@am", warehouseRequeustDTO.Amount);
        
            await command.ExecuteNonQueryAsync();



            command.Parameters.Clear();
            command.CommandText = "select @@identity";
           
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    id = (int)reader.GetDecimal(0);


                }
            }




            
            await transaction.CommitAsync();


        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        return id;
    }



    public async Task<int> compleate_order_with_procedure(WarehouseRequeustDTO warehouseRequeustDTO){

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        command.CommandText = "AddProductToWarehouse";
        command.CommandType = CommandType.StoredProcedure;
        
        command.Parameters.AddWithValue("@IdProduct", warehouseRequeustDTO.IdProduct);

        command.Parameters.AddWithValue("@IdWarehouse", warehouseRequeustDTO.IdWarehouce);

        command.Parameters.AddWithValue("@Amount", warehouseRequeustDTO.Amount);

        command.Parameters.AddWithValue("@CreatedAt", warehouseRequeustDTO.CreatedAt);


        
         using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    return (int)reader.GetDecimal(0);


                }
            }



        return -1;


    }

   
}