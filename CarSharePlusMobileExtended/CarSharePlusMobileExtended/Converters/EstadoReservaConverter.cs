using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using CarSharePlusShared.Models;

namespace CarSharePlusMobileExtended.Converters
{
    public class EstadoReservaConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EstadoReserva estado)
            {
                return estado switch
                {
                    EstadoReserva.Activa => Color.FromArgb("#008000"), // Verde
                    EstadoReserva.Cancelada => Color.FromArgb("#FF0000"), // Rojo
                    EstadoReserva.Finalizada => Color.FromArgb("#808080"), // Gris
                    _ => Color.FromArgb("#000000") // Negro
                };
            }
            return Color.FromArgb("#000000");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
