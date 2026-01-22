namespace CarSharePlusMobileExtended
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnDashboardClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("dashboard");

        private async void OnMapasClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("mapas");

        private async void OnEvaluacionesClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("evaluaciones");

        private async void OnAgregarEvaluacionClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("agregar-evaluacion");

        private async void OnEditarEvaluacionClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("editar-evaluacion");

        private async void OnPagosClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("pagos");

        private async void OnPerfilClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("perfil");

        private async void OnReservarClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("reservar");

        private async void OnReservasClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("reservas");

        private async void OnRecomendacionesClicked(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("recomendaciones");
    }
}
