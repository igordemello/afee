using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tcc_in305b.Models;

public class HistoricoPlayerGrupo
{
    [Key, Column(Order = 0)]
    public int PlayerId { get; set; }

    [Key, Column(Order = 1)]
    public int GrupoId { get; set; }

    [Key, Column(Order = 2)]
    public DateTime Data { get; set; }

    public string MotivoDeTroca { get; set; }

    
    public Player Player { get; set; }
    public Grupo Grupo { get; set; }
}
