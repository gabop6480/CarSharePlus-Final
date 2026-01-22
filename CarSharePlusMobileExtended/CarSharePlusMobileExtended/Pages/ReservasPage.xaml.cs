using CarSharePlusShared.ViewModels;  // ✅ ahora apunta a Shared

namespace CarSharePlusMobileExtended.Pages
{
    public partial class ReservasPage : ContentPage
    {
        public ReservasPage(ReservasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
