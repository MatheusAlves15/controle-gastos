namespace ControleGastos.Api.Models;

//enum para limitar o tipo da transacao
//para evitar que qualquer texto aleatorio entre no banco
public enum TipoTransacao
{
    Despesa = 1,
    Receita = 2
}