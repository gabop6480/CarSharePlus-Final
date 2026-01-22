using System;

namespace CarSharePlusShared.Models
{
    public class EvaluacionDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
