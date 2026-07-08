namespace ControleGastos.Api.Models;

public class Transacao
{
    // Identificador único gerado automaticamente pelo banco.
    public int Id { get; set; }

    // Descrição da transação, exemplo: "Conta de luz".
    public string Descricao { get; set; } = string.Empty;

    // Valor da transação.
    public decimal Valor { get; set; }

    // Tipo da transação: Despesa ou Receita.
    public TipoTransacao Tipo { get; set; }

    // Chave estrangeira que liga a transação a uma pessoa.
    public int PessoaId { get; set; }
}