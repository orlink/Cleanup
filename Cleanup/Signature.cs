using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cleanup
{
    public class Signature
    {
        public string Id { get; set; }
        public string PageNo { get; set; }
        public string EGN { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string LocationUnitName { get; set; }
        public string LocationUnitNo { get; set; }
        public string I { get; set; }
        public string J { get; set; }
        public string FixedEgn { get; set; }
        public string Error { get; set; }

        public Signature Original { get; set; }

        public string ToString(string separator = "\t")
        {
            return Id + separator + PageNo + separator + EGN.Trim() + separator + FirstName.Trim() + separator + MiddleName.Trim() + separator + LastName.Trim() + separator + City.Trim() + separator + LocationUnitName.Trim() + separator + LocationUnitNo + (Original != null ? separator + (Error != null ? Error : "") + separator + Original.ToString() : "");
        }

        public static bool checkdate(int month, int day, int year)
        {
            if (year >= 1 && year <= 32767)
            {
                if (month >= 1 && month <= 12)
                {
                    if (day >= 1 && day <= DateTime.DaysInMonth(year, month))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int[] egnWeights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

        public static bool IsEGNDOBValid(string egn) // this version considers valid only those born in the 20th century
        {
            int year, mon, day;
            if (!(int.TryParse(egn.Substring(0, 2), out year)
               && int.TryParse(egn.Substring(2, 2), out mon)
               && int.TryParse(egn.Substring(4, 2), out day)
               ))
                return false;

            if (mon > 40 && year <= 25) // custom
            {
                if (!checkdate(mon - 40, day, year + 2000)) return false;
            }
            else
                if (mon > 20)
                {
                    return false;
                    //if (!checkdate(mon - 20, day, year + 1800)) return false; 
                }
                else
                {
                    if (!checkdate(mon, day, year + 1900)) return false;
                }

            return true;
        }

        public static bool IsEGNValid(string egn){

            if (egn.Length != 10)
                return false;

            double egnNumber;
            int year, mon, day, checksum;

            if (!(double.TryParse(egn.Trim(), out egnNumber)
                && int.TryParse(egn.Substring(0, 2), out year)
                && int.TryParse(egn.Substring(2, 2), out mon)
                && int.TryParse(egn.Substring(4, 2), out day)
                && int.TryParse(egn.Substring(9, 1), out checksum)))
                return false;

                if (mon > 40) {
                    if (!checkdate(mon-40, day, year+2000)) return false;
                } else
                if (mon > 20) {
                    if (!checkdate(mon-20, day, year+1800)) return false;
                } else {
                    if (!checkdate(mon, day, year+1900)) return false;
                }

                var egnsum = 0;
                for (var i=0;i<9;i++)
                    try
                    {
                        egnsum += int.Parse(egn.Substring(i,1)) * egnWeights[i];
                    }
                    catch (Exception)
                    {
                    }

                var valid_checksum = egnsum % 11;
                if (valid_checksum == 10)
                    valid_checksum = 0;
                if (checksum == valid_checksum)
                    return true;

            return false;

        }

        public static double LevenshteinDistance(string s, string t)
        {
            return LevenshteinDistance(s, t, 1.0, 1.0, 1.0);
        }

        /// <summary>
        /// Compute the Levenshtein distance between two strings.
        /// </summary>
        public static double LevenshteinDistance(string s, string t, double substitution_cost, double insertion_cost, double deletion_cost, Func<char, char, double> calculateSubstitutionCost = null)
        {
            int n = s.Length;
            int m = t.Length;
            double[,] d = new double[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m * insertion_cost;
            }

            if (m == 0)
            {
                return n * deletion_cost;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = insertion_cost * (i++)) ;

            for (int j = 0; j <= m; d[0, j] = deletion_cost * (j++)) ;

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    double substition_distance = d[i - 1, j - 1] + (s[i - 1] != t[j - 1] ? calculateSubstitutionCost != null ? calculateSubstitutionCost(s[i - 1], t[j - 1]) : substitution_cost : 0);

                    double deletion_distance = d[i - 1, j] + deletion_cost;

                    double insertion_distance = d[i, j - 1] + insertion_cost;

                    d[i, j] = Math.Min(substition_distance,
                                        Math.Min(insertion_distance, deletion_distance));
                }
            }
            // Step 7
            return d[n, m];
        }

    }
}
