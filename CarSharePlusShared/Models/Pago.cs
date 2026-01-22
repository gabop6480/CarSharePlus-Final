using System;
using System.ComponentModel.DataAnnotations;

namespace CarSharePlusShared.Models
{
    public class Pago 
    { 
        public int Id { get; set; } 
        [Required(ErrorMessage = "La reserva asociada es obligatoria")] 
        public int ReservaId { get; set; } 
        public Reserva Reserva { get; set; }
        [Required(ErrorMessage = "El monto es obligatorio")] 
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")] public decimal Monto { get; set; } 
        [DataType(DataType.DateTime)] 
        public DateTime FechaPago { get; set; } = DateTime.Now; 
        // inicialización por defecto 
        [Required(ErrorMessage = "El método de pago es obligatorio")] 
        [StringLength(50, ErrorMessage = "El método no puede superar los 50 caracteres")] 
        public string Metodo { get; set; } = "Tarjeta"; 
        [Display(Name = "Pago confirmado")] 
        public bool Confirmado { get; set; } = false; 
    }
}
