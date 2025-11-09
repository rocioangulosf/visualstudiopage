using System;
using System.ComponentModel.DataAnnotations;

namespace Escuela.Web.Models
{
    public class Estudiante
    {
        [Key]
        public int IdEstudiante { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(200), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(18, 40, ErrorMessage = "La edad debe estar entre 18 y 40")]
        public int Edad { get; set; }

        public DateTime Creado { get; set; }
    }
}
