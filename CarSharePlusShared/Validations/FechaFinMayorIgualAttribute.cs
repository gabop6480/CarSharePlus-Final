using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSharePlusShared.Validations 
{ 
    public class FechaFinMayorIgualAttribute : ValidationAttribute 
    { 
        private readonly string _fechaInicioProperty; 
        public FechaFinMayorIgualAttribute(string fechaInicioProperty) 
        { 
            _fechaInicioProperty = fechaInicioProperty; 
            ErrorMessage = "La fecha de fin debe ser mayor o igual a la fecha de inicio."; 
        } 
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext) 
        { 
            var fechaFin = value as DateTime?; 
            var fechaInicioProp = validationContext.ObjectType.GetProperty(_fechaInicioProperty); 
            if (fechaInicioProp == null) return new ValidationResult($"Propiedad {_fechaInicioProperty} no encontrada.");
            var fechaInicio = fechaInicioProp.GetValue(validationContext.ObjectInstance) as DateTime?; 
            if (fechaFin.HasValue && fechaInicio.HasValue && fechaFin.Value < fechaInicio.Value) 
            { 
                return new ValidationResult(ErrorMessage); 
            } 
            return ValidationResult.Success!; 
        } 
    } 
}