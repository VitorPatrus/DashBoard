using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Extensions
{
    public static class GuidExtensions
    {
        public static bool EqualsGuid(this Guid? objA, Guid? objB)
        {
            if (objA == null || objB == null) return false;
            return EqualsGuid(objA.Value, objB.Value.ToString());
        }

        public static bool EqualsGuid(this Guid objA, Guid? objB)
        {
            if (objB == null) return false;
            return EqualsGuid(objA, objB.Value.ToString());
        }

        public static bool EqualsGuid(this Guid objA, string? guidId)
        {
            if (string.IsNullOrEmpty(guidId)) return false;
            return objA.ToString().Equals(guidId, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
