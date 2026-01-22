using System.ComponentModel.DataAnnotations;
using CarSharePlusShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CarSharePlusShared.ViewModels
{
    public partial class VehiculoViewModel : ObservableValidator
    {
        [ObservableProperty]
        private int id;

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(50, ErrorMessage = "La marca no puede superar los 50 caracteres")]
        [ObservableProperty]
        private string marca = string.Empty;

        partial void OnMarcaChanging(string value) => ValidateProperty(value, nameof(Marca));

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(50, ErrorMessage = "El modelo no puede superar los 50 caracteres")]
        [ObservableProperty]
        private string modelo = string.Empty;

        partial void OnModeloChanging(string value) => ValidateProperty(value, nameof(Modelo));

        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        [ObservableProperty]
        private int anio;

        partial void OnAnioChanging(int value) => ValidateProperty(value, nameof(Anio));

        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(10, ErrorMessage = "La placa no puede superar los 10 caracteres")]
        [ObservableProperty]
        private string placa = string.Empty;

        partial void OnPlacaChanging(string value) => ValidateProperty(value, nameof(Placa));

        [Required(ErrorMessage = "La transmisión es obligatoria")]
        [ObservableProperty]
        private TipoTransmision transmision;

        partial void OnTransmisionChanging(TipoTransmision value) => ValidateProperty(value, nameof(Transmision));

        [Required(ErrorMessage = "El tipo de energía es obligatorio")]
        [ObservableProperty]
        private TipoEnergia energia;

        partial void OnEnergiaChanging(TipoEnergia value) => ValidateProperty(value, nameof(Energia));

        [ObservableProperty]
        private bool disponible;

        // UsuarioId solo se usa para mostrar información, no para asignar en ToModel
        [Display(Name = "Usuario asignado")]
        [ObservableProperty]
        private int? usuarioId;

        // Propiedades informativas
        [ObservableProperty]
        private string? usuarioNombre;

        [ObservableProperty]
        private string? usuarioCorreo;

        [Range(0, 10000, ErrorMessage = "La autonomía debe ser mayor o igual a 0")]
        [ObservableProperty]
        private int autonomiaKm;

        [Range(0.0001, double.MaxValue, ErrorMessage = "El consumo debe ser positivo")]
        [ObservableProperty]
        private double consumoPorKm;

        [Range(0.01, 1000, ErrorMessage = "La tarifa por hora debe ser positiva")]
        [Display(Name = "Tarifa por hora")]
        [ObservableProperty]
        private decimal tarifaPorHora;

        // ===== UBICACIÓN =====

        [Range(-90, 90, ErrorMessage = "Latitud inválida")]
        [ObservableProperty]
        private double latitud;

        [Range(-180, 180, ErrorMessage = "Longitud inválida")]
        [ObservableProperty]
        private double longitud;


        public static VehiculoViewModel FromModel(Vehiculo v)
        {
            var vm = new VehiculoViewModel
            {
                Id = v.Id,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                Placa = v.Placa,
                Transmision = v.Transmision,
                Energia = v.Energia,
                Disponible = v.Disponible,
                UsuarioId = v.UsuarioId, // solo informativo
                UsuarioNombre = v.Usuario?.Nombre,
                UsuarioCorreo = v.Usuario?.Correo,
                AutonomiaKm = v.AutonomiaKm,
                ConsumoPorKm = v.ConsumoPorKm,
                TarifaPorHora = v.TarifaPorHora,
                Latitud = v.Latitud,
                Longitud = v.Longitud
            };
            vm.ValidateAllProperties();
            return vm;
        }

        public Vehiculo ToModel(int usuarioId) => new Vehiculo
        {
            Id = Id,
            Marca = Marca,
            Modelo = Modelo,
            Anio = Anio,
            Placa = Placa,
            Transmision = Transmision,
            Energia = Energia,
            Disponible = Disponible,
            AutonomiaKm = AutonomiaKm,
            ConsumoPorKm = ConsumoPorKm,
            TarifaPorHora = TarifaPorHora,

            // 🔥 CLAVE
            UsuarioId = usuarioId,

            // valores base seguros
            Latitud = Latitud,
            Longitud = Longitud
        };

    }
}
