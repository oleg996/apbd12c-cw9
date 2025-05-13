using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class WarehouseRequeustDTO
{
    public required int IdProduct { get; set; }

    public required int IdWarehouce { get; set; }

    public required int Amount{get; set;}

    public required DateTime CreatedAt {get; set;}

}