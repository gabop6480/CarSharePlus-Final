using System;
using System.ComponentModel.DataAnnotations;
using CarSharePlusShared.Models;
using CarSharePlusShared.Validations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CarSharePlusShared.ViewModels
{
    public partial class ReservaViewModel : ObservableValidator
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un usuario")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un vehículo")]
        [Display(Name = "Vehículo")]
        public int VehiculoId { get; set; }
        public string? VehiculoPlaca { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [FechaHoyHoraMayor]
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [FechaFinMayorIgual("FechaInicio")]
        [Display(Name = "Fecha de fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Ubicación de inicio")]
        public string? UbicacionInicio { get; set; }

        [Display(Name = "Ubicación de fin")]
        public string? UbicacionFin { get; set; }

        [Display(Name = "Estado de la reserva")]
        public EstadoReserva Estado { get; set; }

        [Display(Name = "Horas alquiladas")]
        public double HorasAlquiladas => (FechaFin - FechaInicio).TotalHours;

        [Display(Name = "Tarifa por hora")]
        public decimal TarifaPorHora { get; set; }

        [Display(Name = "Monto de Pago")]
        public decimal MontoPago { get; set; }

        [Display(Name = "Monto a mostrar")]
        public decimal MontoMostrar => Math.Round(MontoPago, 2);

        private decimal CalcularMontoInterno()
        {
            var horas = (FechaFin - FechaInicio).TotalHours;
            if (horas <= 0 || TarifaPorHora <= 0) return 0m;
            return (decimal)horas * TarifaPorHora;
        }

        // ⭐ Nueva propiedad: Calificación de la reserva
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        [Display(Name = "Calificación")]
        public int? Calificacion { get; set; }

        // ⭐ Nueva propiedad: Comentario opcional
        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres")]
        [Display(Name = "Comentario")]
        public string? Comentario { get; set; }

        // Métodos de mapeo
        public static ReservaViewModel FromModel(Reserva r) => new ReservaViewModel
        {
            Id = r.Id,
            UsuarioId = r.UsuarioId,
            UsuarioNombre = r.Usuario != null ? r.Usuario.Nombre : $"Usuario #{r.UsuarioId}",
            VehiculoId = r.VehiculoId,
            VehiculoPlaca = r.Vehiculo != null ? r.Vehiculo.Placa : $"Vehículo #{r.VehiculoId}",
            FechaInicio = r.FechaInicio,
            FechaFin = r.FechaFin,
            UbicacionInicio = r.UbicacionInicio ?? "",
            UbicacionFin = r.UbicacionFin ?? "",
            Estado = r.Estado,
            TarifaPorHora = r.Vehiculo != null ? r.Vehiculo.TarifaPorHora : 0m,
            MontoPago = r.MontoPago,  // decimal no nullable, ok
            Calificacion = r.Calificacion.HasValue ? r.Calificacion.Value : 0,  // ✅
            Comentario = r.Comentario ?? "" // ✅
        };






        public Reserva ToModel() => new Reserva
        {
            Id = Id,
            UsuarioId = UsuarioId,
            VehiculoId = VehiculoId,
            FechaInicio = FechaInicio,
            FechaFin = FechaFin,
            UbicacionInicio = UbicacionInicio,
            UbicacionFin = UbicacionFin,
            Estado = Estado,

            // ✅ YA NO nullable
            MontoPago = MontoPago,

            Calificacion = Calificacion,
            Comentario = Comentario
        };


        // 🔹 Nuevo método para cargar una reserva existente
        public void CargarReservaExistente(Reserva reserva)
        {
            if (reserva == null) return;

            Id = reserva.Id;
            UsuarioId = reserva.UsuarioId;
            UsuarioNombre = reserva.Usuario?.Nombre;
            VehiculoId = reserva.VehiculoId;
            VehiculoPlaca = reserva.Vehiculo?.Placa;
            FechaInicio = reserva.FechaInicio;
            FechaFin = reserva.FechaFin;
            UbicacionInicio = reserva.UbicacionInicio;
            UbicacionFin = reserva.UbicacionFin;
            Estado = reserva.Estado;
            TarifaPorHora = reserva.Vehiculo?.TarifaPorHora ?? 0m;
            MontoPago = reserva.MontoPago;
            Calificacion = reserva.Calificacion;
            Comentario = reserva.Comentario;
        }
    }
}
