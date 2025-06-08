using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace pyWeb_ScaeUPT.Models
{

    [Table("tbtoken")]

    public class tokenModel
    {
        [Key]
        public int Id_codigoqr { get; set; }
        public string DNI_token { get; set; }
        public string Token { get; set; } //aca se envia ell codigo encriptado a descrifrar en desktop
    }
}
