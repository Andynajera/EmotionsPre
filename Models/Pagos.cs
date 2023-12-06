namespace Classes;

//Clase
public class Pago{

//propiedades


    public int id {get;set;}
  
    public decimal total { get;set; }
    public string name{get;set;}
    public string email{get;set;}
    public bool pagado { get;set; }
    public DateTime date { get;set; }
    public string numCuenta{get;set;}


public string Summary(){
        return  date.ToString("MM/dd/yyyy")+" ";
    }
}