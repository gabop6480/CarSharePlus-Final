using CarSharePlusShared.Models;
using CarSharePlusShared.Services; // Necesario para usar EvaluacionService
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarSharePlusShared.ViewModels
{
    public partial class EvaluacionesViewModel : ObservableObject
    {
        private readonly EvaluacionService _evaluacionService;

        [ObservableProperty]
        private ObservableCollection<Evaluacion> evaluaciones = new();

        // Eventos para comunicar a la vista (Page)
        public event EventHandler<string>? OperacionCompletada;
        public event EventHandler<Evaluacion>? SolicitarEdicion;
        public event EventHandler? SolicitarAgregar;

        // Constructor con Inyección de Dependencias
        public EvaluacionesViewModel(EvaluacionService evaluacionService)
        {
            _evaluacionService = evaluacionService;

            // Enlazamos la colección del servicio a la del ViewModel
            Evaluaciones = _evaluacionService.Evaluaciones;

            // Cargar datos al iniciar
            Task.Run(CargarEvaluaciones);
        }

        [RelayCommand]
        public async Task CargarEvaluaciones()
        {
            try
            {
                await _evaluacionService.CargarEvaluacionesAsync();
                OperacionCompletada?.Invoke(this, "Evaluaciones actualizadas.");
            }
            catch (Exception ex)
            {
                OperacionCompletada?.Invoke(this, $"Error al cargar: {ex.Message}");
            }
        }

        [RelayCommand]
        private Task AgregarEvaluacion()
        {
            SolicitarAgregar?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task EditarEvaluacion(Evaluacion evaluacion)
        {
            if (evaluacion == null) return Task.CompletedTask;
            SolicitarEdicion?.Invoke(this, evaluacion);
            return Task.CompletedTask;
        }

        // Método auxiliar para guardar desde la vista de agregar
        public async Task<bool> GuardarNuevaEvaluacion(Evaluacion nueva)
        {
            return await _evaluacionService.AgregarEvaluacionAsync(nueva);
        }
    }
}