namespace BI.Sistemas.API.View
{
    public class TMetricLancamento
    {
        public DateTime day { get; set; }
        public DateTime startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string? user { get; set; }
        public string? issueId { get; set; }
        public float duration { get; set; }
    }
}
