using ProjetoEstagio.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Login é de preenchimento Obrigatório.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O campo E-mail é de preenchimento Obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A seleção do Perfil é Obrigatória.")]
        public Perfil? Perfil { get; set; }

        [Required(ErrorMessage = "O campo Senha é de preenchimento Obrigatório.")]
        public string Senha { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }


        public bool SenhaValida(string senha) 
        {
            return Senha == senha;
        }
    }
}
