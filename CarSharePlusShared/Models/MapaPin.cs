namespace CarSharePlusShared.Models
{
    public class MapaPin
    {
        public string Label { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Tipo { get; set; } = "Place"; // Ej: Place, SavedPin
    }
}
