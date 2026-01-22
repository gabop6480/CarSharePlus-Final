using CarSharePlusShared.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class AgregarEvaluacionPage : ContentPage
    {
        public AgregarEvaluacionPage(AgregarEvaluacionViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // ✅ Conectamos la Page con el ViewModel
        }
    }
}