using ControleGastos.Api.Models;

namespace ControleGastos.Api.DTOs;

//dto para criar uma pessoa
public record CriarPessoaDto(string Nome, int Idade);

//dto usado para criar uma transacao
public record CriarTransacaoDto(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    int PessoaId
);

//dto para exibir os totais de cada pessoa
public record PessoaTotalDto(
    int PessoaId,
    string Nome,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo
);

//dto para exibir o total geral do sistema
public record TotalGeralDto(
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal SaldoLiquido
);

//dto final da consulta de totais
public record TotaisResponseDto(
    List<PessoaTotalDto> Pessoas,
    TotalGeralDto TotalGeral
);