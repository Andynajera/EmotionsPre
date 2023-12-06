//Clase
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Classes;

public class Preset{
    
//Propiedades
    public int id { get; set; }
    public string? titulo {get;set;}
    public string? descripcion {get;set;}
    public int precio {get;set;}
    public bool descuento{get;set;}
    public  int precioDescuento{get;set;}
    public DateTime  fechaReserva{get;set;}


 }