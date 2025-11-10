using ProjetoEstagio.Models;

namespace ProjetoEstagio.Repository
{
    public interface ITermoCompromissoRepository
    {
        TermoCompromissoModel Cadastrar(TermoCompromissoModel termo);
        TermoCompromissoModel BuscarPorId(int id);
        TermoCompromissoModel BuscarCompletoPorId(int id); // Para carregar dados relacionados
        TermoCompromissoModel Atualizar(TermoCompromissoModel termo);
        // (Outros métodos como BuscarPorId, Atualizar, etc. virão depois)
    }
}
