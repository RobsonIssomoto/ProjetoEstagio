using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.Validation
{
    // 1. Crie a classe herdando de ValidationAttribute
    public class ValidarCPFAttribute : ValidationAttribute
    {
        // 2. Sobrescreva o método IsValid
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Se o valor for nulo ou vazio, deixa o [Required] cuidar disso
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var cpf = value.ToString();

            // 3. Limpa a formatação (pontos e traços)
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            // 4. Verifica o tamanho
            if (cpf.Length != 11)
            {
                return new ValidationResult("O CPF deve conter 11 dígitos.");
            }

            // 5. Verifica se todos os dígitos são iguais (ex: 111.111.111-11)
            if (new string(cpf[0], 11) == cpf)
            {
                return new ValidationResult("CPF inválido.");
            }

            // 6. Lógica de Validação (Cálculo dos Dígitos Verificadores)
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            // 7. Compara o dígito calculado com o dígito informado
            if (cpf.EndsWith(digito))
            {
                return ValidationResult.Success; // Válido!
            }
            else
            {
                return new ValidationResult("CPF inválido."); // Inválido!
            }
        }
    }
}