using CarSharePlusShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarSharePlusShared.ViewModels
{
    public partial class EditarEvaluacionViewModel : ObservableObject
    {
        [ObservableProperty] private int id;
        [ObservableProperty] private int vehiculoId;
        [ObservableProperty] private int calificacion;
        [ObservableProperty] private string comentario = string.Empty;

        // Eventos que Mobile puede escuchar
        public event EventHandler<string>? OperacionCompletada;
        public event EventHandler? SolicitarCerrar;

        public EditarEvaluacionViewModel(Evaluacion evaluacion)
        {
            Id = evaluacion.Id;
            VehiculoId = evaluacion.VehiculoId;
            Calificacion = evaluacion.Calificacion;
            Comentario = evaluacion.Comentario;
        }

        [RelayCommand]
        private async Task ActualizarEvaluacion()
        {
            if (Calificacion < 1 || Calificacion > 5)
            {
                OperacionCompletada?.Invoke(this, "La calificación debe estar entre 1 y 5.");
                return;
            }

            var evaluacion = new EvaluacionDto
            {
                Id = Id,
                UsuarioId = 1, // Simulado, reemplazar con usuario real
                VehiculoId = VehiculoId,
                Calificacion = Calificacion,
                Comentario = Comentario,
                Fecha = DateTime.Now
            };

            var json = JsonSerializer.Serialize(evaluacion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PutAsync($"https://tuservidor/api/evaluaciones/{Id}", content);

            if (response.IsSuccessStatusCode)
            {
                OperacionCompletada?.Invoke(this, "Evaluación actualizada correctamente.");
                SolicitarCerrar?.Invoke(this, EventArgs.Empty); // Mobile decide si navega atrás
            }
            else
            {
                OperacionCompletada?.Invoke(this, "No se pudo actualizar la evaluación.");
            }
        }
    }
}
