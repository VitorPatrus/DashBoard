﻿namespace BI.Sistemas.Domain
{
    public class Colaborador: Entity
    {
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Email { get; set; }
        public int CargaHoraria { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public string Time { get; set; }
        public byte[]? Foto { get; set; }
        public string UserTMetric { get; set; }
    }
}