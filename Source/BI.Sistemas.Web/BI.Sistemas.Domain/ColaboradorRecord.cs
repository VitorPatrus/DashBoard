using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain
{
    public class ColaboradorRecord
    {
        public string Colaborador { get; set; }
        public TimeSpan Banco { get; set; }
    }
    public sealed class ColaboradorRecordMap : ClassMap<ColaboradorRecord>
    {
        public ColaboradorRecordMap()
        {
            Map(m => m.Colaborador).Name("Colaborador");
            Map(m => m.Banco).Name("Banco").TypeConverter<TimeSpanConverter>();

        }
    }
    public class TimeSpanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (TimeSpan.TryParse(text, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
            {
                return timeSpan;
            }
            return TimeSpan.Zero;
        }
    }
}
