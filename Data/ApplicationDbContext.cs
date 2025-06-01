using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tcc_in305b.Models;

namespace tcc_in305b.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Treinador> Treinadores { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Treino> Treinos { get; set; }
        public DbSet<TreinoPlayer> TreinoPlayers { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<TreinoGrupo> TreinoGrupos { get; set; }
        public DbSet<Estrategia> Estrategias { get; set; }
        public DbSet<EstrategiaGrupo> EstrategiaGrupos { get; set; }
        public DbSet<ToDoTreinador> ToDoTreinadores { get; set; }
        public DbSet<TreinoTipo> TreinoTipos { get; set; }
        public DbSet<Selecao> Selecoes { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<HistoricoPlayerGrupo> HistoricoPlayerGrupos { get; set; }
        public DbSet<Analise> Analises { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<AnalisePlayer> AnalisePlayers { get; set; }
        public DbSet<AnaliseGrupo> AnaliseGrupos { get; set; }
        public DbSet<AnalisePlayerGrupo> AnalisePlayerGrupos { get; set; }
        public DbSet<AnaliseNota> AnaliseNotas { get; set; }
        public DbSet<Fase> Fases { get; set; }
        public DbSet<FaseTreinador> FaseTreinadores { get; set; }
        public DbSet<SelecaoPlayer> SelecaoPlayers { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<EquipePartida> EquipePartidas { get; set; }
        public DbSet<Chaveamento> Chaveamentos { get; set; }
        public DbSet<Torneio> Torneios { get; set; }
        public DbSet<EquipeTorneio> EquipeTorneios { get; set; }
        public DbSet<AnaliseSelecao> AnaliseSelecoes { get; set; }
        public DbSet<PlayerHistorico> PlayerHistoricos { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity <Player>()
                .HasOne(p => p.User)
                .WithOne() 
                .HasForeignKey<Player>(p => p.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Grupo)
                .WithMany(g => g.Players) 
                .HasForeignKey(p => p.GrupoId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Player>()
               .HasOne(p => p.Treinador)
               .WithMany() 
               .HasForeignKey(p => p.TreinadorId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Fase)
                .WithMany(f => f.Players)
                .HasForeignKey(p => p.FaseId);
            
            modelBuilder.Entity<Treinador>()
                .HasOne(p => p.User)
                .WithOne() 
                .HasForeignKey<Treinador>(p => p.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<Admin>()
                .HasOne(p => p.User)
                .WithOne() 
                .HasForeignKey<Admin>(p => p.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<Treino>()
                .HasOne(a => a.Treinador)
                .WithMany() 
                .HasForeignKey(a => a.TreinadorId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<TreinoPlayer>()
                .HasKey(ap => new { ap.TreinoId, ap.PlayerId }); 

            modelBuilder.Entity<TreinoPlayer>()
                .HasOne(ap => ap.Treino)
                .WithMany(a => a.TreinoPlayers)
                .HasForeignKey(ap => ap.TreinoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TreinoPlayer>()
                .HasOne(ap => ap.Player)
                .WithMany()
                .HasForeignKey(ap => ap.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Grupo>()
                .HasOne(g => g.Treinador)
                .WithMany() 
                .HasForeignKey(g => g.TreinadorId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<TreinoGrupo>()
                .HasKey(ag => new { ag.TreinoId, ag.GrupoId }); 

            modelBuilder.Entity<TreinoGrupo>()
                .HasOne(ag => ag.Treino)
                .WithMany(a => a.TreinoGrupos)
                .HasForeignKey(ag => ag.TreinoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TreinoGrupo>()
                .HasOne(ag => ag.Grupo)
                .WithMany()
                .HasForeignKey(ag => ag.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Estrategia>()
                .HasOne(e => e.Treinador)
                .WithMany() 
                .HasForeignKey(e => e.TreinadorId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EstrategiaGrupo>()
                .HasKey(eg => new { eg.EstrategiaId, eg.GrupoId }); 

            modelBuilder.Entity<EstrategiaGrupo>()
                .HasOne(eg => eg.Estrategia)
                .WithMany(e => e.EstrategiaGrupos)
                .HasForeignKey(eg => eg.EstrategiaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EstrategiaGrupo>()
                .HasOne(eg => eg.Grupo)
                .WithMany()
                .HasForeignKey(eg => eg.GrupoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Treino>()
                .Property(a => a.Data)
                .HasColumnType("date");  

            modelBuilder.Entity<Treino>()
                .Property(a => a.HorarioInicio)
                .HasColumnType("time");  

            modelBuilder.Entity<Treino>()
                .Property(a => a.HorarioFim)
                .HasColumnType("time");  

            
            modelBuilder.Entity<Treino>()
                .HasOne(a => a.Estrategia)
                .WithMany() 
                .HasForeignKey(a => a.EstrategiaId);

            
            modelBuilder.Entity<ToDoTreinador>()
                .HasOne(t => t.Treinador)
                .WithMany(t => t.ToDoTreinadores)
                .HasForeignKey(t => t.TreinadorId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<TreinoTipo>()
                .HasOne(t => t.Treinador)
                .WithMany(t => t.TreinoTipos)
                .HasForeignKey(at => at.TreinadorId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<Treino>()
                .HasOne(a => a.TreinoTipo)
                .WithMany(at => at.Treinos) 
                .HasForeignKey(a => a.TreinoTipoId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<HistoricoPlayerGrupo>()
                .HasKey(h => new { h.PlayerId, h.GrupoId, h.Data });

            
            modelBuilder.Entity<HistoricoPlayerGrupo>()
                .HasOne(h => h.Player)
                .WithMany(p => p.HistoricoPlayerGrupos)
                .HasForeignKey(h => h.PlayerId);

            
            modelBuilder.Entity<HistoricoPlayerGrupo>()
                .HasOne(h => h.Grupo)
                .WithMany(g => g.HistoricoPlayerGrupos)
                .HasForeignKey(h => h.GrupoId);



            modelBuilder.Entity<Equipe>()
               .HasOne(e => e.Treinador)
               .WithMany(t => t.Equipes)
               .HasForeignKey(e => e.TreinadorId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
              .HasOne(p => p.Equipe)
              .WithMany(e => e.Players)
              .HasForeignKey(p => p.EquipeId)
              .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Selecao>()
               .HasOne(s => s.Treinador)
               .WithMany(t => t.Selecoes)
               .HasForeignKey(s => s.TreinadorId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Analise>()
               .HasOne(a => a.Treinador)
               .WithMany(t => t.Analises)
               .HasForeignKey(a => a.TreinadorId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Nota>()
               .HasOne(n => n.Treinador)
               .WithMany(t => t.Notas)
               .HasForeignKey(n => n.TreinadorId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AnaliseNota>()
                .HasKey(an => new { an.AnaliseId, an.NotaId});


            modelBuilder.Entity<AnaliseNota>()
                .HasOne(an => an.Analise)
                .WithMany(a => a.AnaliseNotas)
                .HasForeignKey(an => an.AnaliseId);


            modelBuilder.Entity<AnaliseNota>()
                .HasOne(an => an.Nota)
                .WithMany(n => n.AnaliseNotas)
                .HasForeignKey(an => an.NotaId);

            modelBuilder.Entity<AnalisePlayer>()
                .HasKey(ap => new { ap.AnaliseId, ap.PlayerId });


            modelBuilder.Entity<AnalisePlayer>()
                .HasOne(ap => ap.Analise)
                .WithMany(a => a.AnalisePlayers)
                .HasForeignKey(ap => ap.AnaliseId);


            modelBuilder.Entity<AnalisePlayer>()
                .HasOne(ap => ap.Player)
                .WithMany(p => p.AnalisePlayers)
                .HasForeignKey(ap => ap.PlayerId);

            modelBuilder.Entity<AnaliseGrupo>()
                .HasKey(ag => new { ag.AnaliseId, ag.GrupoId });


            modelBuilder.Entity<AnaliseGrupo>()
                .HasOne(ag => ag.Analise)
                .WithMany(a => a.AnaliseGrupos)
                .HasForeignKey(ag => ag.AnaliseId);


            modelBuilder.Entity<AnaliseGrupo>()
                .HasOne(ag => ag.Grupo)
                .WithMany(p => p.AnaliseGrupos)
                .HasForeignKey(ag => ag.GrupoId);

            modelBuilder.Entity<AnalisePlayerGrupo>()
                .HasKey(apg => new { apg.AnaliseId, apg.PlayerId, apg.GrupoId });


            modelBuilder.Entity<AnalisePlayer>()
                .HasOne(apg => apg.Analise)
                .WithMany(a => a.AnalisePlayers)
                .HasForeignKey(apg => apg.AnaliseId);


            modelBuilder.Entity<AnalisePlayer>()
                .HasOne(apg => apg.Player)
                .WithMany(p => p.AnalisePlayers)
                .HasForeignKey(apg => apg.PlayerId);

            modelBuilder.Entity<AnalisePlayerGrupo>()
                .HasOne(apg => apg.Grupo)
                .WithMany(g => g.AnalisePlayerGrupos)
                .HasForeignKey(apg => apg.GrupoId);
            

            modelBuilder.Entity<FaseTreinador>()
                .HasKey(ft => new { ft.FaseId, ft.TreinadorId }); 

            modelBuilder.Entity<FaseTreinador>()
                .HasOne(ft => ft.Fase)
                .WithMany(f => f.FaseTreinadores)
                .HasForeignKey(ft => ft.FaseId);

            modelBuilder.Entity<FaseTreinador>()
                .HasOne(ft => ft.Treinador)
                .WithMany(t => t.FaseTreinadores)
                .HasForeignKey(ft => ft.TreinadorId);

            modelBuilder.Entity<SelecaoPlayer>()
                .HasKey(sp => new { sp.SelecaoId, sp.PlayerId });


            modelBuilder.Entity<SelecaoPlayer>()
                .HasOne(sp => sp.Selecao)
                .WithMany(s => s.SelecaoPlayers)
                .HasForeignKey(sp => sp.SelecaoId);


            modelBuilder.Entity<SelecaoPlayer>()
                .HasOne(sp => sp.Player)
                .WithMany(p => p.SelecaoPlayers)
                .HasForeignKey(sp => sp.PlayerId);

            modelBuilder.Entity<Round>()
                .HasOne(r => r.Selecao)
                .WithMany(s => s.Rounds)
                .HasForeignKey(r => r.SelecaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.Round)
                .WithMany(r => r.Partidas)
                .HasForeignKey(p => p.RoundId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<EquipePartida>()
                .HasKey(ep => new { ep.EquipeId, ep.PartidaId });


            modelBuilder.Entity<EquipePartida>()
                .HasOne(ep => ep.Equipe)
                .WithMany(e => e.EquipePartidas)
                .HasForeignKey(ep => ep.EquipeId);


            modelBuilder.Entity<EquipePartida>()
                .HasOne(ep => ep.Partida)
                .WithMany(p => p.EquipePartidas)
                .HasForeignKey(ep => ep.PartidaId);

           

            modelBuilder.Entity<Equipe>()
                .HasOne(e => e.Selecao)
                .WithMany(s => s.Equipes)
                .HasForeignKey(e => e.SelecaoId);


            modelBuilder.Entity<Chaveamento>()
               .HasOne(c => c.Torneio)
               .WithMany(t => t.Chaveamentos)
               .HasForeignKey(c => c.TorneioId);

            modelBuilder.Entity<Chaveamento>()
               .HasOne(c => c.Anterior)
               .WithMany(c => c.Proximos)
               .HasForeignKey(c => c.AnteriorId)
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<EquipeTorneio>()
                .HasKey(et => new { et.EquipeId, et.TorneioId });


            modelBuilder.Entity<EquipeTorneio>()
                .HasOne(et => et.Equipe)
                .WithMany(e => e.EquipeTorneios)
                .HasForeignKey(et => et.EquipeId);


            modelBuilder.Entity<EquipeTorneio>()
                .HasOne(et => et.Torneio)
                .WithMany(t => t.EquipeTorneios)
                .HasForeignKey(et => et.TorneioId);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.Chaveamento)
                .WithMany(c => c.Partidas)
                .HasForeignKey(p => p.ChaveamentoId);

            modelBuilder.Entity<AnaliseSelecao>()
                .HasKey(ase => new { ase.AnaliseId, ase.SelecaoId });


            modelBuilder.Entity<AnaliseSelecao>()
                .HasOne(ase => ase.Analise)
                .WithMany(ase => ase.AnaliseSelecoes)
                .HasForeignKey(ase => ase.AnaliseId);


            modelBuilder.Entity<AnaliseSelecao>()
                .HasOne(ase => ase.Selecao)
                .WithMany(ase => ase.AnaliseSelecoes)
                .HasForeignKey(ase => ase.SelecaoId);

            modelBuilder.Entity<Torneio>()
                .HasOne(t => t.Treinador)
                .WithMany(tre => tre.Torneios)
                .HasForeignKey(t => t.TreinadorId);

            modelBuilder.Entity<PlayerHistorico>()
                .HasOne(h => h.Player)
                .WithMany(p => p.PlayerHistoricos)
                .HasForeignKey(h => h.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
