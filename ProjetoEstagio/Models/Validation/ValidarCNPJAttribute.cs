using System.ComponentModel.DataAnnotations;

namespace ProjetoEstagio.Models.Validation
{
    public class ValidarCNPJAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var cnpj = value.ToString();

            // Limpa formatação
            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            // Verifica tamanho
            if (cnpj.Length != 14)
            {
                return new ValidationResult("O CNPJ deve conter 14 dígitos.");
            }

            // Verifica dígitos iguais
            if (new string(cnpj[0], 14) == cnpj)
            {
                return new ValidationResult("CNPJ inválido.");
            }

            // Lógica de Validação
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            if (cnpj.EndsWith(digito))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("CNPJ inválido.");
            }
        }
    }
}