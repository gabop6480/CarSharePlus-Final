using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CarSharePlusShared.Models;
using CarSharePlusShared.Services;

namespace CarSharePlusShared.ViewModels
{
    public partial class PagosViewModel : ObservableObject
    {
        private readonly IPagoService _pagoService;

        [ObservableProperty]
        private ObservableCollection<PagoConReservaViewModel> pagos = new();

        [ObservableProperty]
        private bool estaOcupado;

        public event EventHandler<string>? OperacionCompletada;

        public PagosViewModel(IPagoService pagoService)
        {
            _pagoService = pagoService;
            CargarPagos();
        }

        private void CargarPagos()
        {
            Pagos.Clear();
            foreach (var pago in _pagoService.Pagos)
            {
                Pagos.Add(PagoConReservaViewModel.FromModel(pago));
            }
        }

        [RelayCommand]
        private async Task RegistrarAsync(int reservaId)
        {
            if (EstaOcupado) return;

            try
            {
                EstaOcupado = true;

                if (await _pagoService.ExistePagoParaReserva(reservaId))
                {
                    OperacionCompletada?.Invoke(this, "Ya existe un pago para esta reserva.");
                    return;
                }

                var monto = await _pagoService.CalcularMontoPorReserva(reservaId);

                if (monto <= 0)
                {
                    OperacionCompletada?.Invoke(this, "No se pudo calcular el monto.");
                    return;
                }

                var nuevoPago = new Pago
                {
                    ReservaId = reservaId,
                    Monto = monto,
                    Metodo = "Tarjeta",
                    FechaPago = DateTime.Now,
                    Confirmado = false
                };

                var exito = await _pagoService.RegistrarPago(nuevoPago);

                if (exito)
                {
                    Pagos.Add(PagoConReservaViewModel.FromModel(nuevoPago));
                    OperacionCompletada?.Invoke(this, "Pago registrado correctamente.");
                }
                else
                {
                    OperacionCompletada?.Invoke(this, "No se pudo registrar el pago.");
                }
            }
            catch (Exception ex)
            {
                OperacionCompletada?.Invoke(this, "Error: " + ex.Message);
            }
            finally
            {
                EstaOcupado = false;
            }
        }

        [RelayCommand]
        private async Task ConfirmarAsync(int pagoId)
        {
            var exito = await _pagoService.ConfirmarPago(pagoId);

            if (exito)
            {
                CargarPagos();
                OperacionCompletada?.Invoke(this, "Pago confirmado.");
            }
        }

        [RelayCommand]
        private void Refrescar() => CargarPagos();
    }

    // 🔹 Clase auxiliar interna para mostrar Pago + info de Reserva
    public class PagoConReservaViewModel
    {
        public int PagoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public bool Confirmado { get; set; }

        public int ReservaId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string VehiculoPlaca { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public Pago PagoOriginal { get; set; } = new();

        public static PagoConReservaViewModel FromModel(Pago pago)
        {
            return new PagoConReservaViewModel
            {
                PagoId = pago.Id,
                Monto = pago.Monto,
                FechaPago = pago.FechaPago,
                Metodo = pago.Metodo,
                Confirmado = pago.Confirmado,
                ReservaId = pago.ReservaId,
                UsuarioNombre = pago.Reserva?.Usuario?.Nombre ?? string.Empty,
                VehiculoPlaca = pago.Reserva?.Vehiculo?.Placa ?? string.Empty,
                FechaInicio = pago.Reserva?.FechaInicio,
                FechaFin = pago.Reserva?.FechaFin,
                PagoOriginal = pago
            };
        }
    }
}
