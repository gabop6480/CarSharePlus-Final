using Microsoft.Maui.Controls;
using CarSharePlusShared.ViewModels;
using CarSharePlusShared.Models;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class ReservarVehiculoPage : ContentPage
    {
        private readonly ReservaViewModel _viewModel;

        public ReservarVehiculoPage(ReservaViewModel viewModel, Reserva reserva)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Cargar la reserva directamente
            _viewModel.CargarReservaExistente(reserva);
        }
    }
}
