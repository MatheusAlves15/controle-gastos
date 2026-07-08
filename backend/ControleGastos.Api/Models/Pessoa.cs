namespace ControleGastos.Api.Models;

public class Pessoa
{
    // Identificador único gerado automaticamente pelo banco.
    public int Id { get; set; }

    // Nome da pessoa cadastrada.
    public string Nome { get; set; } = string.Empty;

    // Idade usada para validar a regra:
    // menores de 18 anos só podem ter despesas cadastradas.
    public int Idade { get; set; }

    // Lista de transações vinculadas à pessoa.
    // Ao deletar uma pessoa, as transações dela também serão apagadas.
    public List<Transacao> Transacoes { get; set; } = new();
}