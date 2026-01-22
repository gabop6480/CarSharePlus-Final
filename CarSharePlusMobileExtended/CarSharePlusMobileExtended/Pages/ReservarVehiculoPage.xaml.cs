using Microsoft.Maui.Controls;
using CarSharePlusShared.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class ReservarVehiculoPage : ContentPage
    {
        // CAMBIO: Inyectamos el ViewModel de la LISTA (plural)
        public ReservarVehiculoPage(VehiculosViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}