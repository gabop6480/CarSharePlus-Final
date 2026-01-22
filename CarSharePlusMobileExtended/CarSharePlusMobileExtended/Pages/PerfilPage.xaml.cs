using CarSharePlusShared.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class PerfilPage : ContentPage
    {
        public PerfilPage(PerfilViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // ✅ Conectamos la Page con el ViewModel
        }
    }
}
