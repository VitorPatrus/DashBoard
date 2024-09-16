namespace BI.Sistemas.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }

    public abstract class EntityPeriodo: Entity
    {
        public Guid? PeriodoId { get; set; }
        public virtual Periodo? Periodo { get; set; }
    }
}
