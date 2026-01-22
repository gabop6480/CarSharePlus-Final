using CarSharePlusShared.ViewModels;

namespace CarSharePlusMobileExtended.Pages
{
    public partial class EvaluacionesPage : ContentPage
    {
        public EvaluacionesPage(EvaluacionesViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // ✅ Conectamos la Page con el ViewModel
        }
    }
}
