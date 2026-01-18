using BibliotecaAPI.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [PrimeraLetraMayuscula]
        public required string Nombres { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [PrimeraLetraMayuscula]
        public required string Apellidos { get; set; }
        [StringLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        public string? Identificacion { get; set; }
        [Unicode(false)]
        public string? Foto { get; set; }
        public List<AutoresLibros> Libros { get; set; } = [];
    }
}
