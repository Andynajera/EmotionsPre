using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classes
{
    public class OrderPro
    {
        [Key]
        public int id { get; set; }

        // Otras propiedades de la orden...
        public String? name{get;set;}
        public String? email{get;set;}
        public DateTime? fechaEnvio { get; set; }

        // Propiedades de clave foránea
        public int? userId { get; set; }
        public int? pagoId { get; set; }
        public int? presetId { get; set; }
        public int? eventoId { get; set; }

        // Propiedades de navegación
        [ForeignKey("userId")]
        public Usuario? user { get; set; }

        [ForeignKey("pagoId")]
        public Pago? pago { get; set; }

        [ForeignKey("presetId")]
        public Preset? preset { get; set; }

        [ForeignKey("eventoId")]
        public Evento? evento { get; set; }

        // Propiedad Total específica de la orden
        public decimal total { get; set; }
        
    }
}
