using ControleGastos.Api.Models;

namespace ControleGastos.Api.DTOs;

// DTO usado para criar uma pessoa.
// DTO evita receber dados desnecessários do frontend.
public record CriarPessoaDto(string Nome, int Idade);

// DTO usado para criar uma transação.
public record CriarTransacaoDto(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    int PessoaId
);

// DTO usado para exibir os totais de cada pessoa.
public record PessoaTotalDto(
    int PessoaId,
    string Nome,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo
);

// DTO usado para exibir o total geral do sistema.
public record TotalGeralDto(
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal SaldoLiquido
);

// DTO final da consulta de totais.
public record TotaisResponseDto(
    List<PessoaTotalDto> Pessoas,
    TotalGeralDto TotalGeral
);