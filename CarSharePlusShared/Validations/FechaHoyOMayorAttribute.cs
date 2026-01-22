using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSharePlusShared.Validations 
{ 
    public class FechaHoyHoraMayorAttribute : ValidationAttribute 
    { 
        public FechaHoyHoraMayorAttribute() 
        { 
            ErrorMessage = "La fecha de inicio debe ser igual o posterior a la fecha y hora actual."; 
        } 
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext) 
        { 
            var fechaInicio = value as DateTime?; 
            if (fechaInicio.HasValue && fechaInicio.Value < DateTime.Now) 
            { 
                return new ValidationResult(ErrorMessage); 
            } 
            return ValidationResult.Success!; 
        } 
    } 
}
