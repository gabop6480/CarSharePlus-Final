using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSharePlusShared.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres")]
        public string Correo { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres")]
        [RegularExpression(@"^\+?\d{7,20}$", ErrorMessage = "Formato de teléfono inválido (ej: +593987654321)")]
        public string Telefono { get; set; }

        // Seguridad
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [StringLength(20)]
        public string Rol { get; set; } = "User"; 

        // Relación: un usuario puede tener varios vehículos, reservas, evaluaciones y solicitudes
        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }
}
