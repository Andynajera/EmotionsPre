namespace Classes;

//Clase
 public class Usuario{
    
//Propiedades

    
        public int id { get; set; }
        public string name { get; set; }
        public string apellidos { get; set; }
        public string email { get; set; }
        public string descripcion { get; set; } 
        public bool esSocio { get; set; }
        public DateTime dateInscription { get; set; }

          public string Summary()
        {
            return dateInscription.ToString("dd/MM/yyyy HH:mm:ss");
        }
    
//Cambia las propiedades a string con el toString
/*    public string Summary(){
 return  DateInscription.ToString("MM/dd/yyyy")+" ";    }*/
 }


 