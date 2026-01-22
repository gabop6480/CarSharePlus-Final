using System;
using System.ComponentModel.DataAnnotations;

namespace CarSharePlusShared.Models
{
    public class VehiculoRecomendado
    {
        [Required]
        public string Placa { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "El promedio debe estar entre 0 y 5")]
        public double PromedioCalificacion { get; set; }

        public TipoTransmision Transmision { get; set; }
        public TipoEnergia TipoEnergia { get; set; }

        [Range(0, 2000, ErrorMessage = "La autonomía debe estar entre 0 y 2000 km")]
        public int Autonomia { get; set; }

        [Range(0, 50, ErrorMessage = "El consumo debe estar entre 0 y 50 por km")]
        public double Consumo { get; set; }
    }
}
