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
            CargarDatosCommand.Execute(null);
        }

        [RelayCommand]
        public async Task CargarDatos()
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

        private void ActualizarChart()
        {
            var entries = new List<ChartEntry>
            {
                new ChartEntry(Activas) { Label = "Activas", ValueLabel = Activas.ToString(), Color = SKColor.Parse("#34C759") },
                new ChartEntry(Finalizadas) { Label = "Finalizadas", ValueLabel = Finalizadas.ToString(), Color = SKColor.Parse("#007AFF") },
                new ChartEntry(Canceladas) { Label = "Canceladas", ValueLabel = Canceladas.ToString(), Color = SKColor.Parse("#FF3B30") }
            };

            Chart = new DonutChart { Entries = entries, HoleRadius = 0.4f, LabelTextSize = 32 };
        }
    }
}