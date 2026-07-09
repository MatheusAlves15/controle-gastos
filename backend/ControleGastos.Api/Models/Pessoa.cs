namespace ControleGastos.Api.Models;

public class Pessoa
{
    //identificador automatico
    public int Id { get; set; }

    //nome da pessoa
    public string Nome { get; set; } = string.Empty;

    //idade do usuario
    public int Idade { get; set; }

    //transações vinculadas ao usuario
    //quando deletar o usuario as transacoes sao deletadas junto
    public List<Transacao> Transacoes { get; set; } = new();
}