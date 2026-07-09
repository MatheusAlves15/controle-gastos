using System.Text.Json.Serialization;
using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//configuracao do sqlite
//o arquivo gastos.db sera criado na pasta do backend
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=gastos.db");
});

//configuracao para o frontend conseguir acessar a api
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCors("ReactApp");

//cria o banco caso ele ainda nao exista.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

//endpoint para testar se a API esta no ar
app.MapGet("/", () => "API de Controle de Gastos está funcionando!");


//lista pessoas cadastradas
app.MapGet("/pessoas", async (AppDbContext db) =>
{
    var pessoas = await db.Pessoas
        .AsNoTracking()
        .OrderBy(p => p.Nome)
        .ToListAsync();

    return Results.Ok(pessoas);
});

//cria nova pessoa
app.MapPost("/pessoas", async (CriarPessoaDto dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Nome))
        return Results.BadRequest("O nome da pessoa é obrigatório.");

    if (dto.Idade < 0)
        return Results.BadRequest("A idade não pode ser negativa.");

    var pessoa = new Pessoa
    {
        Nome = dto.Nome.Trim(),
        Idade = dto.Idade
    };

    db.Pessoas.Add(pessoa);
    await db.SaveChangesAsync();

    return Results.Created($"/pessoas/{pessoa.Id}", pessoa);
});

//deleta pessoa e transacoes vinculadas
app.MapDelete("/pessoas/{id:int}", async (int id, AppDbContext db) =>
{
    var pessoa = await db.Pessoas.FindAsync(id);

    if (pessoa is null)
        return Results.NotFound("Pessoa não encontrada.");

    db.Pessoas.Remove(pessoa);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


//lista todas as transacoes cadastradas
app.MapGet("/transacoes", async (AppDbContext db) =>
{
    var transacoes = await db.Transacoes
        .AsNoTracking()
        .Join(
            db.Pessoas.AsNoTracking(),
            transacao => transacao.PessoaId,
            pessoa => pessoa.Id,
            (transacao, pessoa) => new
            {
                transacao.Id,
                transacao.Descricao,
                transacao.Valor,
                transacao.Tipo,
                transacao.PessoaId,
                PessoaNome = pessoa.Nome
            }
        )
        .OrderByDescending(t => t.Id)
        .ToListAsync();

    return Results.Ok(transacoes);
});

//cria uma nova transacao
app.MapPost("/transacoes", async (CriarTransacaoDto dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Descricao))
        return Results.BadRequest("A descrição da transação é obrigatória");

    if (dto.Valor <= 0)
        return Results.BadRequest("O valor da transação deve ser maior que zero");

    var pessoa = await db.Pessoas.FindAsync(dto.PessoaId);

    if (pessoa is null)
        return Results.BadRequest("A pessoa informada não existe");

    //pessoas menores de idade so podem ter despesas
    if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
        return Results.BadRequest("Menores de idade só podem ter despesas cadastradas");

    var transacao = new Transacao
    {
        Descricao = dto.Descricao.Trim(),
        Valor = dto.Valor,
        Tipo = dto.Tipo,
        PessoaId = dto.PessoaId
    };

    db.Transacoes.Add(transacao);
    await db.SaveChangesAsync();

    return Results.Created($"/transacoes/{transacao.Id}", transacao);
});

//consulta os totais por pessoa e geral
app.MapGet("/totais", async (AppDbContext db) =>
{
    var pessoas = await db.Pessoas
        .Include(p => p.Transacoes)
        .AsNoTracking()
        .OrderBy(p => p.Nome)
        .ToListAsync();

    var totaisPorPessoa = pessoas.Select(p =>
    {
        var totalReceitas = p.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Receita)
            .Sum(t => t.Valor);

        var totalDespesas = p.Transacoes
            .Where(t => t.Tipo == TipoTransacao.Despesa)
            .Sum(t => t.Valor);

        var saldo = totalReceitas - totalDespesas;

        return new PessoaTotalDto(
            p.Id,
            p.Nome,
            totalReceitas,
            totalDespesas,
            saldo
        );
    }).ToList();

    var totalGeralReceitas = totaisPorPessoa.Sum(t => t.TotalReceitas);
    var totalGeralDespesas = totaisPorPessoa.Sum(t => t.TotalDespesas);
    var saldoLiquido = totalGeralReceitas - totalGeralDespesas;

    var resposta = new TotaisResponseDto(
        totaisPorPessoa,
        new TotalGeralDto(
            totalGeralReceitas,
            totalGeralDespesas,
            saldoLiquido
        )
    );

    return Results.Ok(resposta);
});

//api rodando em porta fixa para facilitar o uso
app.Run("http://localhost:5000");