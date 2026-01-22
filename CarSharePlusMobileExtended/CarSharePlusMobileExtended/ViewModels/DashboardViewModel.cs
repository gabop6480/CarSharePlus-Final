using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using CarSharePlusShared.Services;

namespace CarSharePlusMobileExtended.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly DashboardService _dashboardService;

        [ObservableProperty] private int totalReservas;
        [ObservableProperty] private int activas;
        [ObservableProperty] private int finalizadas;
        [ObservableProperty] private int canceladas;
        [ObservableProperty] private double promedioDuracionHoras;

        [ObservableProperty] private Chart chart;

        public DashboardViewModel(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            // Llamamos a cargar datos al iniciar
            Task.Run(CargarDatos);
        }

        [RelayCommand]
        public async Task CargarDatos()
        {
            try
            {
                var stats = await _dashboardService.GetStatsAsync();
                if (stats != null)
                {
                    TotalReservas = stats.TotalReservas;
                    Activas = stats.Activas;
                    Finalizadas = stats.Finalizadas;
                    Canceladas = stats.Canceladas;
                    PromedioDuracionHoras = stats.PromedioDuracionHoras;

                    ActualizarChart();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando dashboard: {ex.Message}");
            }
        }

        private void ActualizarChart()
        {
            // CAMBIO: Si no hay datos, mostramos un gráfico gris para que no quede el hueco en blanco
            if (Activas == 0 && Finalizadas == 0 && Canceladas == 0)
            {
                var emptyEntries = new List<ChartEntry>
                {
                    new ChartEntry(1)
                    {
                        Label = "Sin datos",
                        ValueLabel = "0",
                        Color = SKColor.Parse("#E0E0E0")
                    }
                };
                Chart = new DonutChart { Entries = emptyEntries, HoleRadius = 0.4f, LabelTextSize = 30 };
                return;
            }

            var entries = new List<ChartEntry>();

            if (Activas > 0)
                entries.Add(new ChartEntry(Activas) { Label = "Activas", ValueLabel = Activas.ToString(), Color = SKColor.Parse("#34C759") });

            if (Finalizadas > 0)
                entries.Add(new ChartEntry(Finalizadas) { Label = "Finaliz.", ValueLabel = Finalizadas.ToString(), Color = SKColor.Parse("#007AFF") });

            if (Canceladas > 0)
                entries.Add(new ChartEntry(Canceladas) { Label = "Cancel.", ValueLabel = Canceladas.ToString(), Color = SKColor.Parse("#FF3B30") });

            Chart = new DonutChart { Entries = entries, HoleRadius = 0.4f, LabelTextSize = 32 };
        }
    }
}