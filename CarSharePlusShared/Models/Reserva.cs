using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSharePlusShared.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; } // Nullable: puede no estar cargado

        [Required(ErrorMessage = "El vehículo es obligatorio")]
        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; } // Nullable: puede no estar cargado

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(Reserva), nameof(ValidarFechas))]
        public DateTime FechaFin { get; set; }

        [StringLength(200, ErrorMessage = "La ubicación no puede superar los 200 caracteres")]
        public string? UbicacionInicio { get; set; } = ""; // Valor por defecto para evitar NULL

        [StringLength(200, ErrorMessage = "La ubicación no puede superar los 200 caracteres")]
        public string? UbicacionFin { get; set; } = ""; // Valor por defecto

        [Display(Name = "Monto del pago")]
        public decimal MontoPago { get; set; } = 0m; // Valor por defecto 0

        [Display(Name = "Estado de la reserva")]
        public EstadoReserva Estado { get; set; } = EstadoReserva.Pendiente;

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

        // Calificación nullable (puede ser NULL en la BD)
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public int? Calificacion { get; set; }

        // Comentario opcional
        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres")]
        public string? Comentario { get; set; } = ""; // Valor por defecto para evitar NULL

        // 🔧 Propiedades calculadas
        public string RangoFechas => $"{FechaInicio:dd/MM/yyyy HH:mm} - {FechaFin:dd/MM/yyyy HH:mm}";

        public string DescripcionVehiculo => Vehiculo != null
            ? $"{Vehiculo.Marca} {Vehiculo.Modelo} ({Vehiculo.Placa})"
            : "Vehículo no asignado";

        public double DuracionHoras => (FechaFin - FechaInicio).TotalHours;

        // Validación personalizada 
        public static ValidationResult? ValidarFechas(DateTime fechaFin, ValidationContext context)
        {
            var instance = context.ObjectInstance as Reserva;
            if (instance != null && fechaFin <= instance.FechaInicio)
            {
                return new ValidationResult("La fecha de fin debe ser posterior a la fecha de inicio.");
            }
            return ValidationResult.Success;
        }
    }

    public enum EstadoReserva
    {
        Pendiente,
        Activa,
        Finalizada,
        Cancelada
    }
}
