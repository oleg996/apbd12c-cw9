using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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



            return Ok(_confyguration.GetConnectionString("default"));
        }



    }
}