using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace pyWeb_ScaeUPT.Models
{
    [Table("tbestudiante")]
    public class estudianteModel
    {
        [Key]
        public int Id_Estudiante { get; set; }
        public string Id_Persona { get; set; }
        public string Matricula { get; set; }
        public int Semestre { get; set; }
        public string Correo { get; set; }
    }
}
