using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Tutorial9.Model;
using Tutorial9.Services;

namespace Tutorial9.Controllers
{
    [Route("api/warehouce")]
    [ApiController]
    public class WarehouceController : ControllerBase
    {
        private readonly IConfiguration  _confyguration;

        private readonly IDbService  _dbservice;

        public WarehouceController(IConfiguration configuration,IDbService Dbservice){
            _confyguration = configuration;
            _dbservice = Dbservice;
        }



        [HttpPost]
        public async Task<IActionResult> addItem(WarehouseRequeustDTO warehouseRequeustDTO){
            if(!await _dbservice.does_product_exists(warehouseRequeustDTO.IdProduct))
                return NotFound("product not found");

            if(!await _dbservice.does_warehouse_exists(warehouseRequeustDTO.IdWarehouce))
                return NotFound("warehouse not found");

            if(warehouseRequeustDTO.Amount <= 0)
                return  BadRequest("Ammount cannot be <= 0");

            if( await _dbservice.does_order_exists(warehouseRequeustDTO.IdProduct,warehouseRequeustDTO.CreatedAt,warehouseRequeustDTO.Amount) == -1)
                return NotFound("order not found(or was compleated)");


            int id = await _dbservice.complete_order(warehouseRequeustDTO);




            return Ok(id);
        }
        
        [HttpPost("proc")]
        public async Task<IActionResult> addItem_with_proc(WarehouseRequeustDTO warehouseRequeustDTO){
           
            Console.WriteLine(warehouseRequeustDTO);

            try
            {
            int id = await _dbservice.compleate_order_with_procedure(warehouseRequeustDTO);
            return Ok(id);
            }
            catch (SqlException e){
                return NotFound(e.Message);
            }


            
        }



    }
}