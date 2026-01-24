using Microsoft.Maui.Controls;
using CarSharePlusShared.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class ReservarVehiculoPage : ContentPage
    {
        // El constructor SOLO debe recibir el VehiculosViewModel
        public ReservarVehiculoPage(VehiculosViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}