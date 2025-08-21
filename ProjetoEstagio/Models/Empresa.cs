using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ProjetoEstagio.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string Segmento { get; set; }
        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Representante { get; set; }
        public string Funcao { get; set; }
        public string CPF { get; set; }
        public string Departamento { get; set; }
        public string telefone { get; set; }
        public string cep { get; set; }
        public string Supervisor { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
    }
}
