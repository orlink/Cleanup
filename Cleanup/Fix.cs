using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cleanup
{
    public class Fix
    {
        public string Name { get; set; }
        public Func<Signature, bool> Condition { get; set; }
        public Func<Signature, Signature> GetFixed { get; set; }

        public static List<Fix> Fixes = new List<Fix> { 
            new Fix { 
                Name="Male with female middle name", 
                Condition = s => Signature.IsEGNValid(s.EGN) && (int.Parse(s.EGN.Substring(s.EGN.Length - 2, 1)) % 2 == 0 && ((s.MiddleName.EndsWith("ва") || s.MiddleName.EndsWith("ска")) && (s.LastName.EndsWith("ски") || s.LastName.EndsWith("в")))), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName.TrimEnd('а'), LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Male with female middle name", Original = s}
            },
            new Fix { 
                Name="Male with female last name", 
                Condition = s => Signature.IsEGNValid(s.EGN) && (int.Parse(s.EGN.Substring(s.EGN.Length - 2, 1)) % 2 == 0 && ((s.LastName.EndsWith("ва") || s.LastName.EndsWith("ска")) && (s.MiddleName.EndsWith("ски") || s.MiddleName.EndsWith("в")))), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName.TrimEnd('а'), City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Male with female last name", Original = s}
            },
            new Fix { 
                Name="Female with male middle name", 
                Condition = s => Signature.IsEGNValid(s.EGN) && (int.Parse(s.EGN.Substring(s.EGN.Length - 2, 1)) % 2 == 1 && ((s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("ска") || s.LastName.EndsWith("ва")))), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName + "а", LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Female with male middle name", Original = s}
            },
            new Fix { 
                Name="Female with male last name", 
                Condition = s => Signature.IsEGNValid(s.EGN) && (int.Parse(s.EGN.Substring(s.EGN.Length - 2, 1)) % 2 == 1 && ((s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && (s.MiddleName.EndsWith("ска") || s.MiddleName.EndsWith("ва")))), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName + "а", City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Female with male last name", Original = s}
            },
            new Fix { 
                Name="11-digit EGN", 
                Condition = s => s.EGN.Length == 11 && Signature.IsEGNValid(Fix11DigitEgn(s.EGN)), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix11DigitEgn(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "11-digit EGN", Original = s}
            },
            new Fix { 
                Name="Female with male EGN", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && s.MiddleName.EndsWith("а") && s.LastName.EndsWith("а") && int.Parse(s.EGN.Substring(8,1))%2 == 0
                && Signature.IsEGNValid(FixEgnForFemale(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = FixEgnForFemale (s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Female with invalid male EGN", Original = s}
            },
            new Fix { 
                Name="Male with female EGN", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && (s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && int.Parse(s.EGN.Substring(8,1))%2 == 1
                && Signature.IsEGNValid(FixEgnForMale(s.EGN)), 
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = FixEgnForMale (s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Male with ivalid female EGN", Original = s}
            },
            new Fix { // ix: 7
                Name="Male with female first name", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && Signature.IsEGNValid(s.EGN) && s.FirstName.EndsWith("а") && !maleNames.Contains(s.FirstName)  && (s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && int.Parse(s.EGN.Substring(8,1))%2 == 0,
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = s.EGN, FirstName = FixFirstNameForMale(s.FirstName), MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Male with female first name", Original = s}
            },
            new Fix { 
                Name="1st digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN) && Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn1stDigit(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "1st digit fix", Original = s}
            },
            new Fix { 
                Name="Invalid DOB", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && !Signature.IsEGNDOBValid(s.EGN) && Signature.IsEGNValid(Fix.FixEgnDOB(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgnDOB(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "DOB fix", Original = s}
            },
            new Fix { 
                Name="7th digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN) 
                                && Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn7thDigit(s.EGN, s.City), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "7th digit fix", Original = s}
            },
            new Fix { 
                Name="8th digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN)
                                && !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 1))  
                                && Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn8thDigit(s.EGN, s.City), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "8th digit fix", Original = s}
            },
            new Fix { 
                Name="9th digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN)
                                && !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN)) && !Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 1))
                                && Signature.IsEGNValid(Fix.FixEgn9thDigit(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn9thDigit(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "9th digit fix", Original = s}
            },
            new Fix { 
                Name="6th digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN)
                                && !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN)) && !Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 1)) && !Signature.IsEGNValid(Fix.FixEgn9thDigit(s.EGN, 1)) 
                                && Signature.IsEGNValid(Fix.FixEgn6thDigit(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn6thDigit(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "6th digit fix", Original = s}
            },
            new Fix { 
                Name="10th digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN)
                                && !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN)) && !Signature.IsEGNValid(Fix.FixEgn2ndDigit(s.EGN)) 
                                && Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn10thDigit(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "10th digit fix", Original = s}
            },
            new Fix { 
                Name="2nd digit fix", 
                Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && Signature.IsEGNDOBValid(s.EGN)
                                && !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN)) && !Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 1)) 
                                 && Signature.IsEGNValid(Fix.FixEgn2ndDigit(s.EGN)),
                GetFixed = s => new Signature {Id = s.Id, PageNo = s.PageNo, EGN = Fix.FixEgn2ndDigit(s.EGN), FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "2nd digit fix", Original = s}
            },

            //new Fix { 
            //    Name="Semicolon", 
            //    Condition = s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && Signature.IsEGNValid(s.EGN) && s.FirstName.EndsWith("а") && !maleNames.Contains(s.FirstName)  && (s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && int.Parse(s.EGN.Substring(8,1))%2 == 0,
            //    GetFixed = s => new Signature {PageNo = s.PageNo, EGN = s.EGN, FirstName = FixFirstNameForMale(s.FirstName), MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo, I = s.I, J = s.J, Error= "Semicolon", Original = s}
            //},
            //new Fix { 
            //    Name="Mixed alphabets", 
            //    Condition = s => (Regex.IsMatch(s.FirstName, @"[а-яА-Я]") && Regex.IsMatch(s.FirstName, @"[a-zA-Z]"))
            //                    || (Regex.IsMatch(s.MiddleName, @"[а-яА-Я]") && Regex.IsMatch(s.MiddleName, @"[a-zA-Z]"))
            //                    || (Regex.IsMatch(s.LastName, @"[а-яА-Я]") && Regex.IsMatch(s.LastName, @"[a-zA-Z]")), 
            //    GetFixed = s => new Signature {PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo}
            //},
            //new Fix { 
            //    Name="Latin alphabets", 
            //    Condition = s => (!Regex.IsMatch(s.FirstName, @"[а-яА-Я]") && Regex.IsMatch(s.FirstName, @"[a-zA-Z]"))
            //                    || (!Regex.IsMatch(s.MiddleName, @"[а-яА-Я]") && Regex.IsMatch(s.MiddleName, @"[a-zA-Z]"))
            //                    || (!Regex.IsMatch(s.LastName, @"[а-яА-Я]") && Regex.IsMatch(s.LastName, @"[a-zA-Z]")), 
            //    GetFixed = s => new Signature {PageNo = s.PageNo, EGN = s.EGN, FirstName = s.FirstName, MiddleName = s.MiddleName, LastName = s.LastName, City = s.City, LocationUnitName = s.LocationUnitName, LocationUnitNo = s.LocationUnitNo}
            //},

        };

        public static Dictionary<int, int[]> likelySubstitutions = new Dictionary<int, int[]> {
            {0, new int[] {6,1}},
            {1, new int[] {7,0,9,4}},
            {2, new int[] {3,1}},
            {3, new int[] {8,5,2,9}},
            {4, new int[] {2,1,9,3,6,0}},
            {5, new int[] {3,6,8,0}},
            {6, new int[] {8,0,5}},
            {7, new int[] {4,1,8,3}},
            {8, new int[] {3,5,6,0,9}},
            {9, new int[] {8,5,3,1,0,4}},
        };

        public static Dictionary<int, int[]> Digit2Map = new Dictionary<int, int[]> {
            {0, new int[] {6}},
            {1, new int[] {}},
            {2, new int[] {5}},
            {3, new int[] {9}},
            {4, new int[] {1}},
            {5, new int[] {6}},
            {6, new int[] {5,0}},
            {7, new int[] {}},
            {8, new int[] {5,6,0,3}},
            {9, new int[] {3,4,6}},
        };

        public static Dictionary<int, int[]> Digit8Map = new Dictionary<int, int[]> {
            {0, new int[] {6, 9}},
            {1, new int[] {0}},
            {2, new int[] {5}},
            {3, new int[] {8,5,2,0}},
            {4, new int[] {1,2}},
            {5, new int[] {3,6}},
            {6, new int[] {8,5,9}},
            {7, new int[] {4,8,6}},
            {8, new int[] {5,9,3}},
            {9, new int[] {8,3,4,6}},
        };

        public static Dictionary<int, int[]> Digit9Map = new Dictionary<int, int[]> {
            {0, new int[] {6}},
            {1, new int[] {}},
            {2, new int[] {0}},
            {3, new int[] {}},
            {4, new int[] {2}},
            {5, new int[] {3,1}},
            {6, new int[] {8}},
            {7, new int[] {}},
            {8, new int[] {0}},
            {9, new int[] {5}},
        };

        public static Dictionary<int, int[]> Digit10Map = new Dictionary<int, int[]> {
            {0, new int[] {}},
            {1, new int[] {}},
            {2, new int[] {7,3}},
            {3, new int[] {}},
            {4, new int[] {3}},
            {5, new int[] {3,8}},
            {6, new int[] {0}},
            {7, new int[] {4}},
            {8, new int[] {}},
            {9, new int[] {4,5,6}},
        };

        public static Dictionary<string, int[]> regions = new Dictionary<string, int[]> { 
           {"Благоевград", new int[] {0, 43}},   /* от 000 до 043 */ 
           {"Бургас", new int[] {44, 93}},  /* от 044 до 093 */ 
           {"Варна", new int[] {93, 139}}, /* от 094 до 139 */ 
           {"Велико Търново",new int[]{140, 169}}, /* от 140 до 169 */ 
           {"Видин", new int[] {170, 183}}, /* от 170 до 183 */ 
           {"Враца", new int[] {184, 217}}, /* от 184 до 217 */ 
           {"Габрово", new int[] {218, 233}}, /* от 218 до 233 */ 
           {"Кърджали", new int[] {234, 281}}, /* от 234 до 281 */ 
           {"Кюстендил", new int[] {282, 301}}, /* от 282 до 301 */ 
           {"Ловеч", new int[] {302, 319}}, /* от 302 до 319 */ 
           {"Монтана", new int[] {320, 341}}, /* от 320 до 341 */ 
           {"Пазарджик", new int[] {342, 377}}, /* от 342 до 377 */ 
           {"Перник", new int[] {378, 395}}, /* от 378 до 395 */ 
           {"Плевен", new int[] {396, 435}}, /* от 396 до 435 */ 
           {"Пловдив", new int[] {436, 501}}, /* от 436 до 501 */ 
           {"Разград", new int[] {502, 527}}, /* от 502 до 527 */ 
           {"Русе", new int[] {528, 555}}, /* от 528 до 555 */ 
           {"Силистра", new int[] {556, 575}}, /* от 556 до 575 */ 
           {"Сливен", new int[] {576, 601}}, /* от 576 до 601 */ 
           {"Смолян", new int[] {602, 623}}, /* от 602 до 623 */ 
           {"София - град", new int[] {624, 721}}, /* от 624 до 721 */ 
           {"София - окръг", new int[] {722, 751}}, /* от 722 до 751 */ 
           {"Стара Загора", new int[] {752, 789}}, /* от 752 до 789 */ 
           {"Добрич (Толбухин)", new int[] {790, 821}}, /* от 790 до 821 */ 
           {"Търговище", new int[] {822, 843}}, /* от 822 до 843 */ 
           {"Хасково", new int[] {844, 871}}, /* от 844 до 871 */ 
           {"Шумен", new int[] {872, 903}}, /* от 872 до 903 */ 
           {"Ямбол", new int[] {904, 925}}, /* от 904 до 925 */ 
           {"Друг/Неизвестен", new int[] {926, 999}} /* от 926 до 999 -*/
       };

        public static ILookup<string, CityCode> City7thDigit;

        public static ILookup<string, CityCode> City78thDigits;

        public static string NormalizeCity(string city)
        {
            return Regex.Replace(city.Trim(), @"^(гр|с|гт|гс|грю)[., \-]+| общ\..*| обл\..*", "").Replace("Г.О.", "Горна Оряховица").Replace("Г. Оряховица", "Горна Оряховица").Trim().ToLower();
        }
        public static void CalculateStatistics(string filePath, char separator, bool withIdColumn, Encoding encoding)
        {
            var signatures = Reader.Read(filePath, '\t', 0, withIdColumn, encoding);
            var signaturesCityEGN = signatures.Where(s => Signature.IsEGNValid(s.EGN) && !String.IsNullOrWhiteSpace(s.City))
                .Select(s => new { 
                    City = NormalizeCity(s.City) , 
                    EGN = s.EGN 
                }).ToList();

            Fix.City7thDigit = signaturesCityEGN
                .GroupBy(s => new { City = s.City, Region = s.EGN.Substring(6, 1) })
                .Select(g => new CityCode
                {
                    City = g.Key.City,
                    Code = g.Key.Region,
                    Count = g.Count()
                }
                ).Where(i => i.Count > 0 //&& i.City != "гр. София"
                ).OrderByDescending(i => i.Count)
                            //.Sum(i => i.Count);
                .ToLookup(c => c.City, c => c);

            Fix.City78thDigits = signaturesCityEGN
               .GroupBy(s => new { City = s.City, Region = s.EGN.Substring(6, 2) })
               .Select(g => new CityCode
               {
                   City = g.Key.City,
                   Code = g.Key.Region,
                   Count = g.Count()
               }
               ).Where(i => i.Count > 0 //&& i.City != "гр. София"
               ).OrderByDescending(i => i.Count)
                            //.Sum(i => i.Count);
               .ToLookup(c => c.City, c => c);
        }

        public static string FixEgn7thDigit(string egn, string city, int previous = 1000)
        {
            var cityKey = NormalizeCity(city);
            if (City7thDigit[cityKey].Count() > 0)
            {
                var digit = egn.Substring(6, 1);

                var codes = City7thDigit[cityKey].Where((c, i) => i < 2 || (c.Count > 2000 && i < 3) || (c.Count > 20 && i < 3 && c.Count > 0.05 * City7thDigit[cityKey].First().Count)).ToArray();

                if (!codes.Any(c => c.Code == digit))
                {
                    foreach (var code in codes)
                    {
                        var newEgn = egn.Substring(0, 6) + code.Code + egn.Substring(7);
                        // check if new egn is valid and that 7th and 8th digit together match one of the common city codes
                        var city78codes = City78thDigits[cityKey].Where(c => c.Code.StartsWith(code.Code));
                        var top78code = city78codes.First();
                        if (Signature.IsEGNValid(newEgn)
                            && city78codes.Where((c, i) => (i < 4 && c.Count > 0.3 * top78code.Count) 
                                || (code.Code == "6" && cityKey == "софия" && i < 5))
                            .Any(c => c.Code == newEgn.Substring(6, 2)))
                        {
                            return newEgn;
                        }
                    }
                    if (codes.Count() == 1 && codes.First().Count < 10 && Signature.IsEGNValid(egn.Substring(0, 6) + codes.First().Code + egn.Substring(7)))
                    {
                        return egn.Substring(0, 6) + codes.First().Code + egn.Substring(7);
                    }
                }
            }

            return egn;
        }

        public static string FixEgn8thDigit(string egn, string city, int previous = 1000)
        {
            var cityKey = NormalizeCity(city);
            if (City78thDigits[cityKey].Count() > 0)
            {
                var leadingDigit = egn.Substring(6,1);
                var digit = egn.Substring(7, 1);

                var top7Code = City7thDigit[cityKey].First();
                if (!City7thDigit[cityKey].Any(c => c.Code == leadingDigit && ((c.Count > 0.06 * top7Code.Count) || (cityKey == "софия" && c.Count > 0.04 * top7Code.Count)))) // try only those where the leading digit is common for the city
                    return egn;

                var topCodes = City78thDigits[cityKey].Where((c, i) => c.Code.StartsWith(leadingDigit)).Take(2);
                var okCodes = City78thDigits[cityKey].Where((c, i) => c.Code.StartsWith(leadingDigit))
                    .Where(c => c.Count > 0.08 * topCodes.First().Count).ToArray();

                if (!okCodes.Any(c => c.Code.EndsWith(digit)))
                {
                    var newEgn = topCodes.Select(c => egn.Substring(0, 6) + c.Code + egn.Substring(8)).FirstOrDefault(e => Signature.IsEGNValid(e));
                    if (newEgn != null)
                        return newEgn;
                }
                else
                {
                    var newEgn = okCodes.Where(c => c.Count > 0.25 * topCodes.First().Count && c.Count > 5)
                        .Where(c => (Digit8Map[int.Parse(digit)][0] == int.Parse(c.Code.Substring(1, 1))) 
                            //|| (Digit8Map[int.Parse(digit)].Length > 1 && Digit8Map[int.Parse(digit)][1] == int.Parse(c.Code.Substring(1, 1)))
                         )
                        .Select(c => egn.Substring(0, 6) + c.Code + egn.Substring(8))
                        .FirstOrDefault(e => Signature.IsEGNValid(e));
                    if (newEgn != null)
                        return newEgn;

                }
            };

            return egn;
        }

        public static string FixEgn1stDigit(string egn)
        {
            string newEgn;

            if (egn.StartsWith("1"))
            {
                foreach (var i in new string[] { "7", "9" })
                {
                    newEgn = i + egn.Substring(1);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            if (egn.StartsWith("2"))
            {
                foreach (var i in new string[] { "8" })
                {
                    newEgn = i + egn.Substring(1);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            if (egn.StartsWith("3"))
            {
                newEgn = egn.Substring(1,1) + egn.Substring(0,1) + egn.Substring(2); //switch first and second digit
                if (Signature.IsEGNValid(newEgn))
                    return newEgn;

                foreach (var i in new string[] { "5" })
                {
                    newEgn = i + egn.Substring(1);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            if (egn.StartsWith("99"))
            {
                foreach (var i in new string[] { "8" })
                {
                    newEgn = i + i + egn.Substring(2);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            return egn;
        }

        public static string FixEgn2ndDigit(string egn)
        {
            return FixEgnDigit(egn, 1, Digit2Map, 2);
        }

        public static string FixEgnSeventhDigitStatic(string egn, string city, int previous = 1000)
        {
            string newEgn;
            var cityKey = city.Replace("гр. ", "").Replace("гр ", "");
            if (regions.ContainsKey(cityKey))
            {
                var digit = int.Parse(egn.Substring(6,1));

                var first = Math.Floor((regions[cityKey][0] / 10.0));
                var last = Math.Floor((regions[cityKey][1] / 10.0));


                if (digit != first && digit != last)
                {
                    newEgn = egn.Substring(0, 6) + first.ToString() + egn.Substring(7);
                    if (Signature.IsEGNValid(newEgn))
                    {
                        return newEgn;
                    }
                    newEgn = egn.Substring(0, 6) + last.ToString() + egn.Substring(7);
                    if (Signature.IsEGNValid(newEgn))
                    {
                        return newEgn;
                    }
                }
            };

            return egn;
        }

        public static string FixEgnEigthDigit(string egn, string city, int previous = 1000)
        {
            string newEgn;
            var cityKey = city.Replace("гр. ", "").Replace("гр ", "");
            if (regions.ContainsKey(cityKey))
            {
                var digit = int.Parse(egn.Substring(6, 1));

                var first = Math.Floor((regions[cityKey][0]%100 / 10.0));
                var last = Math.Floor((regions[cityKey][1]%100 / 10.0));

                if (digit < first || digit > last)
                {
                    newEgn = egn.Substring(0, 6) + first.ToString() + egn.Substring(7);
                    if (Signature.IsEGNValid(newEgn))
                    {
                        return newEgn;
                    }
                    newEgn = egn.Substring(0, 6) + last.ToString() + egn.Substring(7);
                    if (Signature.IsEGNValid(newEgn))
                    {
                        return newEgn;
                    }
                }
            };

            return egn;
        }

        public static string FixEgn6thDigit(string egn)
        {
            string newEgn;

            newEgn = egn.Substring(0, 5) + egn.Substring(5, 1).Replace("3", "5").Replace("8","9") + egn.Substring(6);
            if (Signature.IsEGNValid(newEgn))
                return newEgn;

            newEgn = egn.Substring(0, 5) + egn.Substring(5, 1).Replace("8", "6").Replace("9", "5") + egn.Substring(6);
            if (Signature.IsEGNValid(newEgn))
                return newEgn;

            return egn;
        }

        public static string FixEgn5thDigit(string egn)
        {
            string newEgn;

            newEgn = egn.Substring(0, 4) + egn.Substring(4,1).Replace("1","2") + egn.Substring(5);
            if (Signature.IsEGNValid(newEgn))
                return newEgn;

            return egn;
        }

        public static string FixEgnDOB(string egn)
        {
            string newEgn;
            int year, mon, day;
            var firstDayDigits = new int[] {2,1,0};

            if (!(int.TryParse(egn.Substring(0, 2), out year)
               && int.TryParse(egn.Substring(2, 2), out mon)
               && int.TryParse(egn.Substring(4, 2), out day)
               ))
                return egn;

            year += 1900; // only people born before 2000

            if (!Signature.checkdate(mon, day, year))  // invalid date
            {
                if (mon > 12 && mon <= 31 && Signature.checkdate(day, mon, year)) // try switching month and day digits
                {
                    newEgn = egn.Substring(0, 2) + egn.Substring(4, 2) + egn.Substring(2, 2) + egn.Substring(6);
                    if (Signature.IsEGNValid(newEgn))
                    {
                        return newEgn;
                    }
                }
                var map = Fix.likelySubstitutions;

                if (mon >= 20 && day >=31) //try to fix both first month digit and first day digit
                {
                    foreach (var i in new string[] { "1", "0" })
                    {
                        if (day > 31) // also first digit of day is wrong
                        {
                            foreach (var j in new string[] { "2", "1", "0", "3" })
                            {
                                newEgn = egn.Substring(0, 2) + i + egn.Substring(3, 1) + j + egn.Substring(5);
                                if (Signature.IsEGNValid(newEgn))
                                    return newEgn;
                            }
                        }
                    }
                }
                else if (mon >= 20 && day <= 31) //try to fix first month digit
                {
                    foreach (var i in new string[] { "1", "0" })
                    {
                        newEgn = egn.Substring(0, 2) + i + egn.Substring(3);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon > 12 && mon < 20 && day <= 31) // try to fix second month digit
                {
                    foreach (var i in new string[] { "0", "1", "2" })
                    {
                        newEgn = egn.Substring(0, 3) + i + egn.Substring(4);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon == 0 && day <= 31) //try to fix second month digit
                {
                    foreach (var i in new string[] { "8", "6", "9", "1" })
                    {
                        newEgn = egn.Substring(0, 3) + i + egn.Substring(4);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon == 2 && DateTime.DaysInMonth(year, mon) == 28 && day == 29) // try to fix second day digit
                {
                    foreach (var i in new string[] { "8", "4", "0", "5" })
                    {
                        newEgn = egn.Substring(0, 5) + i + egn.Substring(6);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon == 2 && DateTime.DaysInMonth(year, mon) == 29 && day == 28) // try to fix second day digit
                {
                    foreach (var i in new string[] { "9", "6", "5", "3" })
                    {
                        newEgn = egn.Substring(0, 5) + i + egn.Substring(6);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon > 0 && mon <= 12 && DateTime.DaysInMonth(year, mon) == 30 && day == 31) // try to fix second day digit
                {
                    newEgn = egn.Substring(0, 5) + "0" + egn.Substring(6);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
                else if (mon > 0 && mon <= 12 && DateTime.DaysInMonth(year, mon) < day) // try to fix first day digit
                {
                    foreach (var i in new string[] {"2","1","0","3"})
                    {
                        newEgn = egn.Substring(0, 4) + i + egn.Substring(5);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (mon > 0 && mon <= 12 && day == 0) // try to fix first day digit
                {
                    foreach (var i in new string[] { "3", "1", "2"})
                    {
                        newEgn = egn.Substring(0, 4) + i + egn.Substring(5);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
                else if (year < 1920 && year >= 1900) // try to fix first year digit
                {
                    newEgn = FixEgnDigit(egn, 0, map, 4);
                    if (newEgn != egn)
                        return newEgn;
                }
                else if (year == 1999)
                {
                    newEgn = FixEgnDigit(egn, 1, map, 6);
                    if (newEgn != egn)
                        return newEgn;
                }
            }

            return egn;
        }

        public static string FixEgn9thDigit(string egn, int tries = 2)
        {
            string newEgn;

            if (egn.Length == 10)
            {
                int digit;
                if (int.TryParse(egn.Substring(8, 1), out digit))
                {
                    var map = Fix.Digit9Map;

                    for (int i = 0; i < Math.Min(tries, map[digit].Length); i++)
                    {
                        if (digit % 2 == map[digit][i] % 2) {
                            newEgn = egn.Substring(0, 8) + map[digit][i].ToString() + egn.Substring(9);
                            if (Signature.IsEGNValid(newEgn))
                                return newEgn;
                        }
                    }
                }
            }

            return egn;
        }

        public static string FixEgn10thDigit(string egn, int tries = 2)
        {
            return FixEgnDigit(egn, 9, Fix.Digit10Map, tries);
        }

        public static string FixEgnSwapLastTwoDigits(string egn, int tries = 2)
        {
            var newEgn = egn.Substring(0, 8) + egn.Substring(9, 1) + egn.Substring(8, 1);
            if (Signature.IsEGNValid(newEgn))
            {
                return newEgn;
            }
            return egn;
        }

        public static string FixEgnDigit(string egn, int pos, Dictionary<int, int[]> map, int tries)
        {
            string newEgn;

            if (egn.Length == 10)
            {
                int digit;
                if (int.TryParse(egn.Substring(pos, 1), out digit))
                {
                    for (int i = 0; i < Math.Min(tries, map[digit].Length); i++)
                    {
                        newEgn = egn.Substring(0, pos) + map[digit][i].ToString() + (pos < 9 ? egn.Substring(pos + 1) : "");
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }
            }

            return egn;
        }

        public static string Fix11DigitEgn(string egn)
        {
            if (egn.Length == 11)
            {
                for (var i = 0; i < 9; i++)
                {
                    if (Signature.IsEGNValid(egn.Substring(0, i) + egn.Substring(i + 1)))
                        return egn.Substring(0, i) + egn.Substring(i + 1);
                }
            }

            return egn;
        }

        public static string FixEgnForFemale(string egn)
        {
            string newEgn;

            if (egn.Length == 10)
            {
                if (egn.Substring(8, 1) == "8")
                {
                    for (var i = 9; i >= 1; i = i - 2)
                    {
                        newEgn = egn.Substring(0, 8) + i.ToString() + egn.Substring(9);
                        if (Signature.IsEGNValid(newEgn))
                            return newEgn;
                    }
                }

                for (var i = 1; i <= 9; i = i + 2)
                {
                    newEgn = egn.Substring(0, 8) + i.ToString() + egn.Substring(9);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            return egn;
        }

        public static string FixEgnForMale(string egn)
        {
            string newEgn;

            if (egn.Length == 10)
            {
                for (var i = 8; i >= 0; i = i - 2)
                {
                    newEgn = egn.Substring(0, 8) + i.ToString() + egn.Substring(9);
                    if (Signature.IsEGNValid(newEgn))
                        return newEgn;
                }
            }

            return egn;
        }

        public static string[] maleNames = new string[] { "Никола", "никола", "Коста", "Сава", "Тома", "Муса", "Мустафа", "Серьожа", "Миша", "Славката", "Караджа", "Ивица", "Автула", "Риза", "Реза", "Абдула", "Абдулла", "Никита", "Гриша", "Илга", "Альоша", "Емурла", "Лука", "Дука", "Мусба" };

        public static string FixFirstNameForMale(string name)
        {
            return name.Replace("Антоанета", "Антони")
                        .Replace("Марианна", "Мариан")
                        .Replace("Василка", "Васил")
                        .Replace("Цветанка", "Цветан")
                        .Replace("Иванка", "Иван")
                        .Replace("Пенка", "Пенко")
                        .Replace("Тодорка", "Тодор")
                        .Replace("Василка", "Васил")
                        .Replace("Стоянка", "Стоян")
                        .Replace("Йорданка", "Йордан")
                        .Replace("Пламенка", "Пламен")
                        .Replace("Света", "Свети")
                        .Replace("Цана", "Цанко")
                        .Replace("Цветанка", "Цветан")
                        .Replace("Величка", "Величко")
                        .Replace("Господинка", "Господин")
                        .Replace("Добринка", "Добрин")
                        .Replace("Здравка", "Здравко")
                        .Replace("Кирилка", "Кирил")
                        .Replace("Василка", "Васил")
                        .Replace("Дафинка", "Дафин")
                        .Replace("Свободка", "Свободко")
                        .Replace("Атанаска", "Атанас")
                        .Replace("Стоянка", "Стоян")
                        .Replace("Антонка", "Антон")
                        .Replace("Костадинка", "Костадин")
                        .Replace("Генка", "Генко")
                        .Replace("Цветанка", "Цветан")
                        .Replace("Цона", "Цона")
                        .Replace("Деница", "Деница")
                        .TrimEnd('а');
        }
    }
}
