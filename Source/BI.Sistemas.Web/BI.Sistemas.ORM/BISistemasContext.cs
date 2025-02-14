using BI.Sistemas.Domain;
using BI.Sistemas.Domain.Novo;
using Microsoft.EntityFrameworkCore;

namespace BI.Sistemas.Context
{
    public class BISistemasContext : DbContext
    {
        private readonly string _connectionString = @"Server=CON-SNOTE093;Database=BI_SISTEMASDEVS;Trusted_Connection=True;TrustServerCertificate=True;";

        public BISistemasContext(DbContextOptions<BISistemasContext> options)
        : base(options) { }

        public BISistemasContext() { }

        public BISistemasContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<Movidesk> Movidesks { get; set; }
        public DbSet<Ponto> Pontos { get; set; }
        public DbSet<TMetric> TMetrics { get; set; }
        public DbSet<HE> HorasExtras { get; set; }
        public DbSet<EvolucaoSLA> EvolucaoSLA { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            optionsBuilder.LogTo(Console.WriteLine);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colaborador>(entity =>
            {
                entity.ToTable("COLABORADOR");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.Nome)
                    .HasColumnName("NOME")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Cargo)
                    .HasColumnName("CARGO")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.CargaHoraria)
                    .HasColumnName("CARGAHORARIA");

                entity.Property(e => e.Inicio)
                    .HasColumnName("INICIO");

                entity.Property(e => e.Termino)
                    .HasColumnName("TERMINO");

                entity.Property(e => e.Time)
                    .HasColumnName("TIME")
                    .HasMaxLength(100);

                entity.Property(e => e.Foto)
                    .HasColumnName("FOTO");

                entity.Property(e => e.UserTMetric)
                    .HasColumnName("USER_TMETRIC")
                    .HasMaxLength(255);

                entity.Property(e => e.Matricula)
                    .HasColumnName("MATRICULA")
                    .IsRequired()
                    .HasMaxLength(7);

                entity.HasIndex(e => e.Matricula)
                    .IsUnique();
            });

            modelBuilder.Entity<TMetric>(entity =>
            {
                entity.ToTable("TMETRIC");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.Data)
                    .HasColumnName("DATA")
                    .IsRequired();

                entity.Property(e => e.Usuario)
                    .HasColumnName("USUARIO")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Inicio)
                    .HasColumnName("INICIO")
                    .IsRequired();

                entity.Property(e => e.Termino)
                    .HasColumnName("TERMINO")
                    .IsRequired();

                entity.Property(e => e.Duracao)
                    .HasColumnName("DURACAO")
                    .IsRequired();

                entity.Property(e => e.DevopsTask)
                    .HasColumnName("DEVOPSTASK");

                entity.Property(e => e.DataCarga)
                    .HasColumnName("CARGA");

                entity.Property(e => e.ColaboradorId)
                    .HasColumnName("COLABORADOR");

                entity.HasOne(e => e.Colaborador)
                    .WithOne()
                    .HasForeignKey<TMetric>(c => c.ColaboradorId);

                entity.Property(e => e.PeriodoId)
                    .HasColumnName("PERIODO");

                entity.HasOne(e => e.Periodo)
                    .WithOne()
                    .HasForeignKey<TMetric>(c => c.PeriodoId);

                entity.Property(e => e.Tipo)
                    .HasColumnName("TIPO")
                    .HasMaxLength(100);

                entity.Property(e => e.ParentType)
                    .HasMaxLength(25)
                    .HasColumnName("PARENT_TYPE");

                entity.Property(e => e.ParentTitulo)
                    .HasColumnName("PARENT_TITULO")
                    .HasMaxLength(1000);

            });

            modelBuilder.Entity<Periodo>(entity =>
            {
                entity.ToTable("PERIODO");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.Data)
                    .HasColumnName("DATA")
                    .IsRequired();

                entity.Property(e => e.Inicio)
                    .HasColumnName("DATAINICIO")
                    .IsRequired();

                entity.Property(e => e.Termino)
                    .HasColumnName("DATATERMINO")
                    .IsRequired();
            });

            modelBuilder.Entity<Ponto>(entity =>
            {
                entity.ToTable("PONTO");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.Horas)
                    .HasColumnName("HORAS")
                    .IsRequired();

                entity.Property(e => e.Tipo)
                    .HasColumnName("TIPO")
                    .IsRequired();

                entity.Property(e => e.ColaboradorId)
                    .HasColumnName("COLABORADOR");

                entity.HasOne(e => e.Colaborador)
                    .WithOne()
                    .HasForeignKey<Ponto>(c => c.ColaboradorId);

                entity.Property(e => e.PeriodoId)
                    .HasColumnName("PERIODO");

            });

            modelBuilder.Entity<HE>(entity =>
            {
                entity.ToTable("HE");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.ColaboradorId)
                .HasColumnName("COLABORADOR")
                    .IsRequired();

                entity.HasOne(e => e.Colaborador)
                    .WithMany()
                    .HasForeignKey(e => e.ColaboradorId);

                entity.Property(e => e.Horas)
                    .HasColumnName("HORAS")
                    .IsRequired();

                entity.Property(e => e.Data)
                    .HasColumnName("DATA")
                    .IsRequired();

                entity.Property(e => e.PeriodoId)
                .HasColumnName("PERIODO")
                    .IsRequired();

                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoId);
            });

            modelBuilder.Entity<EvolucaoSLA>(entity =>
            {
                entity.ToTable("EVOLUCAO_SLA");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.ColaboradorId)
                .HasColumnName("COLABORADOR")
                    .IsRequired();

                entity.HasOne(e => e.Colaborador)
                    .WithMany()
                    .HasForeignKey(e => e.ColaboradorId);

                entity.Property(e => e.DentroDoPrazo)
                    .HasColumnName("NoPrazo")
                    .IsRequired();

                entity.Property(e => e.ForaDoPrazo)
                    .HasColumnName("ForaDoPrazo")
                    .IsRequired();

                entity.Property(e => e.Data)
                    .HasColumnName("DATA")
                    .IsRequired();

                entity.Property(e => e.PeriodoId)
                .HasColumnName("PERIODO")
                    .IsRequired();

                entity.HasOne(e => e.Periodo)
                    .WithMany()
                    .HasForeignKey(e => e.PeriodoId);
            });

            CreateNovoModulo(modelBuilder);
        }

        private void CreateNovoModulo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BI.Sistemas.Domain.Novo.Geracao>(entity =>
            {
                entity.ToTable("Geracao");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.Data)
                    .HasColumnName("Data_Geral")
                    .IsRequired();

                entity.Property(e => e.DataFim)
                    .HasColumnName("Data_Fim")
                    .IsRequired();

                entity.Property(e => e.DataInicio)
                    .HasColumnName("Data_Inicio");

                entity.HasMany(x => x.Times).WithOne(y => y.Geracao);

            });
            modelBuilder.Entity<BI.Sistemas.Domain.Novo.GeracaoTime>(entity =>
            {
                entity.ToTable("Geracao_Time");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.GeracaoId)
                    .HasColumnName("GERACAO")
                    .IsRequired();

                entity.Property(e => e.ConclusaoCards)
                    .HasColumnName("Conclusao_Card")
                    .IsRequired();

                entity.Property(e => e.SpEntregues)
                    .HasColumnName("SP_ENTREGUE")
                    .IsRequired();

                entity.Property(e => e.BacklogTotal)
                   .HasColumnName("BACKLOG_TOTAL")
                   .IsRequired();

                entity.Property(e => e.BacklogNovo)
                   .HasColumnName("BACKLOG_NOVO")
                   .IsRequired();

                entity.Property(e => e.Wip)
                   .HasColumnName("WIP")
                   .IsRequired();

                entity.Property(e => e.Bug)
                   .HasColumnName("BUG")
                   .IsRequired();

                entity.Property(e => e.LeadTime)
                  .HasColumnName("LEADTIME")
                  .IsRequired();

                entity.Property(e => e.CicleTime)
                  .HasColumnName("CICLETIME")
                  .IsRequired();

                entity.HasOne(x => x.Geracao)
                    .WithMany(y => y.Times)
                    .HasForeignKey(c => c.GeracaoId);

            });

            modelBuilder.Entity<BI.Sistemas.Domain.Novo.Movidesk>(entity =>
            {
                entity.ToTable("MOVIDESK");

                entity.HasKey(e => e.Id)
                    .HasName("ID");

                entity.Property(e => e.IndicadorSLA)
                    .HasColumnName("INDICADORSLA");

                entity.Property(e => e.PeriodoId)
                    .HasColumnName("PERIODO");

                entity.HasOne(e => e.Periodo)
                    .WithOne()
                    .HasForeignKey<Movidesk>(c => c.PeriodoId);

                entity.Property(e => e.Numero)
                    .HasColumnName("NUMERO")
                    .IsRequired();

                entity.Property(e => e.DataAbertura)
                    .HasColumnName("DATAABERTURA")
                    .IsRequired();

                entity.Property(e => e.DataVencimento)
                    .HasColumnName("DATAFECHAMENTO");

                entity.Property(e => e.DataFechamento)
                    .HasColumnName("DATAVENCIMENTO");

                entity.Property(e => e.Assunto)
                    .HasColumnName("ASSUNTO");

                entity.Property(e => e.Pessoa)
                    .HasColumnName("PESSOA");

                entity.Property(e => e.ResponsavelId)
                    .HasColumnName("RESPONSAVEL");

                entity.HasOne(e => e.Responsavel)
                    .WithOne()
                    .HasForeignKey<Movidesk>(c => c.ResponsavelId);

                entity.Property(e => e.ResponsavelChamado)
                    .HasColumnName("RESPONSAVELCHAMADO");

                entity.Property(e => e.Servico)
                    .HasColumnName("SERVICO");

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Time)
                    .HasColumnName("TIME");

                entity.Navigation(c => c.Periodo).AutoInclude();
                entity.Navigation(c => c.Responsavel).AutoInclude();

            });
        }
    }
}
