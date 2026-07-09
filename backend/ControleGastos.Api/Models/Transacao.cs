namespace ControleGastos.Api.Models;

public class Transacao
{
    //identificador unico gerado automaticamente
    public int Id { get; set; }

    //descrição da transacao
    public string Descricao { get; set; } = string.Empty;

    //valor da transacao
    public decimal Valor { get; set; }

    //tipo da transacao
    public TipoTransacao Tipo { get; set; }

    //chave estrangeira que liga transacao a pessoa
    public int PessoaId { get; set; }
}