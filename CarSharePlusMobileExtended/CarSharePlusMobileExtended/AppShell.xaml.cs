using CarSharePlusMobileExtended.Pages;

namespace CarSharePlusMobileExtended
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Registar rutas para la navegación
            Routing.RegisterRoute("dashboard", typeof(DashboardPage)); 
            Routing.RegisterRoute("mapas", typeof(MapasPage)); 
            Routing.RegisterRoute("evaluaciones", typeof(EvaluacionesPage)); 
            Routing.RegisterRoute("agregar-evaluacion", typeof(AgregarEvaluacionPage)); 
            Routing.RegisterRoute("editar-evaluacion", typeof(EditarEvaluacionPage)); 
            Routing.RegisterRoute("pagos", typeof(PagosPage)); 
            Routing.RegisterRoute("perfil", typeof(PerfilPage)); 
            Routing.RegisterRoute("reservar", typeof(ReservarVehiculoPage));
            Routing.RegisterRoute("reservas", typeof(ReservasPage)); 
            Routing.RegisterRoute("recomendaciones", typeof(RecomendacionesPage));
        }
    }
}
