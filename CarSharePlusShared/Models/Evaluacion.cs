using System;
using System.ComponentModel.DataAnnotations;

namespace CarSharePlusShared.Models
{
    public class Evaluacion
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public required Usuario Usuario { get; set; }
        public int VehiculoId { get; set; }
        public required Vehiculo Vehiculo { get; set; }
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Calificacion { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string Comentario { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
