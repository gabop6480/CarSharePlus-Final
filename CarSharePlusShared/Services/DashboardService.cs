using System.Threading.Tasks;

namespace CarSharePlusShared.Services
{
    // Clase simple para transportar los datos (DTO)
    public class DashboardStats
    {
        public int TotalReservas { get; set; }
        public int Activas { get; set; }
        public int Finalizadas { get; set; }
        public int Canceladas { get; set; }
        public double PromedioDuracionHoras { get; set; }
    }

    public class DashboardService
    {
        // Constructor vacío compatible con la inyección
        public DashboardService(HttpClient client) { }

        public async Task<DashboardStats> GetStatsAsync()
        {
            // Simulamos una pequeña carga
            await Task.Delay(300);

            // DATOS FALSOS PARA QUE LA GRÁFICA SE VEA BIEN SIEMPRE
            return new DashboardStats
            {
                TotalReservas = 150,
                Activas = 12,        // Verde
                Finalizadas = 120,   // Azul
                Canceladas = 18,     // Rojo
                PromedioDuracionHoras = 5.5
            };
        }
    }
}