using CarSharePlusMobileExtended.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class DashboardPage : ContentPage
    {
        // El constructor recibe el ViewModel automáticamente gracias a MauiProgram.cs
        public DashboardPage(DashboardViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}