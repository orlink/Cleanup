using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Cleanup
{
    public class Reader
    {
        public static IEnumerable<Signature>Read(string filePath, char delimiter, int headerRowCount, bool withIdColumn, Encoding encoding)
        {
            if (withIdColumn)
                return File.ReadAllLines(filePath, encoding).Skip(headerRowCount)
                            .Select(l => l.Split(delimiter).Take(10).ToArray())
                            .Select(i => new Signature { Id = i[0], PageNo = i[1], EGN = i[2], FirstName = i[3], MiddleName = i[4], LastName = i[5], City = i[6], LocationUnitName = i[7], LocationUnitNo = i[8] });
            else
                return File.ReadAllLines(filePath, encoding).Skip(headerRowCount)
                            .Select(l => l.Split(delimiter).Take(10).ToArray())
                            .Select(i => new Signature { PageNo = i[0], EGN = i[1], FirstName = i[2], MiddleName = i[3], LastName = i[4], City = i[5], LocationUnitName = i[6], LocationUnitNo = i[7]});
        }
    }
}
