using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //config da tabela pessoa
        modelBuilder.Entity<Pessoa>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Idade)
                .IsRequired();

            //quando uma pessoa for deletada todas as atividades vinculadas sao deletadas junto
            entity.HasMany(p => p.Transacoes)
                .WithOne()
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        //config da tabela pessoa transacao
        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Descricao)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(t => t.Valor)
                .IsRequired();

            //salva enum como texto no banco
            //despesa ou receita
            entity.Property(t => t.Tipo)
                .HasConversion<string>()
                .IsRequired();
        });
    }
}