namespace ControleGastos.Api.Models;

// Enum usado para limitar o tipo da transação.
// Isso evita salvar qualquer texto aleatório no banco.
public enum TipoTransacao
{
    Despesa = 1,
    Receita = 2
}