using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleanup
{
    public static class SignatureExtensions
    {
        public static IEnumerable<Signature> ApplyFix(this IEnumerable<Signature> source, Fix fix) {
            return source.Select(s => (fix.Condition(s) ? fix.GetFixed(s) : s));
        }
    }
}
