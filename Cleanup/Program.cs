using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Cleanup
{
    class Program
    {
        //static void test()
        //{
        //    string inputFilePath, outputFilePath;
        //    var signatures = File.ReadAllLines(inputFilePath)...;
        //    var fixedSignatures = signatures.ApplyFix(Fixes[0]).ApplyFix(Fixes[1]);
        //    File.WriteAllLines(outputFilePath, fixedSignatures.Select(s => s.ToString()));
        //}
        static void Main(string[] args)
        {
            //int ix = File.ReadAllText(@"C:\Kredor\Source\Referendum\BLANKA Cloud 410k-new2-fixed.csv").IndexOf("\r\n", 10000);
            //var filePath = @"C:\Kredor\Source\Referendum\EgnErrorSample.txt";
            var filePath = @"C:\Kredor\Source\Referendum\BLANKA_FINAL-2.txt"; //BLANKA Cloud 410k-new2-trimmed.csv";
            //var lines = File.ReadAllLines(filePath, Encoding.GetEncoding("windows-1251"));
            //var signs = File.ReadAllLines(filePath, Encoding.GetEncoding("windows-1251")).Skip(2).Select(l => l.Split('\t').Take(8).ToArray()).Where(i => i.Length < 8).ToList();
            
            var signatures = Reader.Read(filePath, '\t', 0, true, Encoding.GetEncoding("windows-1251")).ToList();

            //var common = signatures.Where(s => true || Signature.IsEGNValid(s.EGN) 
            //    //&& !String.IsNullOrWhiteSpace(s.City)
            //    ).Select(s => new { City = Fix.NormalizeCity(s.City), EGN = s.EGN })
            //    .GroupBy(s => new { City = s.City })
            //    .Select(g => new
            //    {
            //        City = g.Key.City,
            //        Count = g.Count()
            //    }
            //    ).Where(i => i.Count == 1
            //    ).OrderBy(c => c.City).ThenByDescending(i => i.Count)
            //    //.Sum(i => i.Count);
            //    .ToList();

            //Fix.City78thDigits = common.ToLookup(c => c.City, c => c);

            int temp;

            Fix.CalculateStatistics(filePath, '\t', true, Encoding.GetEncoding("windows-1251"));

            var tabDelimitedCommon = String.Join("\r\n", Fix.City78thDigits["Кръстава"]
                //.Where(c => c.Code.StartsWith("7"))
                .Select(c => c.City + "\t" + c.Code + "\t" + c.Count));

            filePath = @"C:\Kredor\Source\Referendum\EgnErrorSample.txt";
            var signaturesSample = Reader.Read(filePath, '\t', 0, false, Encoding.GetEncoding("windows-1251")).ToList();
            var isvalid = Signature.IsEGNValid(Fix.FixEgnDOB("7509004031"));
            //var signaturesEGN = signatures.Skip(0).Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && Signature.IsEGNDOBValid(s.EGN) && !Signature.IsEGNValid(s.EGN)
            var signaturesEGN = signaturesSample.Skip(0).Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) 
                && Signature.IsEGNDOBValid(s.EGN)
                //&& !Signature.IsEGNDOBValid(Fix.FixEgnDOB(s.EGN)) 
                //&& !Signature.IsEGNValid(Fix.FixEgn1stDigit(s.EGN))
                //&& !Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 1)) 
                //&& !Signature.IsEGNValid(Fix.FixEgn9thDigit(s.EGN))
                && Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City))
                //&& !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City))
                //&& Signature.IsEGNValid(Fix.FixEgn10thDigit(s.EGN, 3)) 
                //&& !Signature.IsEGNValid(Fix.FixEgn2ndDigit(s.EGN))
               //&& Signature.IsEGNValid(Fix.FixEgn6thDigit(s.EGN))

                //&& !Signature.IsEGNValid(Fix.FixEgnSwapLastTwoDigits(s.EGN)) //&& !Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)) && !Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City))
            //var signaturesEGN = signatures.Skip(0).Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && Signature.IsEGNDOBValid(s.EGN) && !Signature.IsEGNValid(Fix.FixEgnLastDigit(s.EGN,1)) && !Signature.IsEGNValid(s.EGN)
            //var signaturesEGN = signatures.Skip(0).Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNDOBValid(s.EGN) //&& Signature.IsEGNValid(FixEgnForMale(s.EGN))
            //var signaturesEGN = signatures.Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) //&& Signature.IsEGNValid(FixEgnForMale(s.EGN))
            //var signaturesEGN = signatures.Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && (int.Parse(s.EGN.Substring(4, 2)) > 31) && !Signature.IsEGNValid(s.EGN) && s.MiddleName.EndsWith("а") && s.LastName.EndsWith("а") && int.Parse(s.EGN.Substring(8, 1)) % 2 == 1 // && Signature.IsEGNValid(FixEgnForFemale(s.EGN))
            //var signaturesEGN = signatures.Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && Signature.IsEGNValid(s.EGN) && s.FirstName.EndsWith("а") && !maleNames.Contains(s.FirstName)  && (s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && int.Parse(s.EGN.Substring(8,1))%2 == 0 //&& Signature.IsEGNValid(FixEgnForMale(s.EGN))
            //var signaturesEGN = signatures.Where(s => s.EGN.Length == 9 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) //&& int.Parse(s.EGN.Substring(8,1))%2 == 1 && s.LastName.EndsWith("а") //                                                         && !Signature.IsEGNValid(s.EGN) && (s.MiddleName.EndsWith("в") || s.MiddleName.EndsWith("ски")) && (s.LastName.EndsWith("в") || s.LastName.EndsWith("ски")) && int.Parse(s.EGN.Substring(8, 1)) % 2 == 1 && Signature.IsEGNValid(FixEgnForMale(s.EGN))
            ).Skip(0).Take(1000)
            //.Where(s => Signature.IsEGNValid(Fix.FixEgnSecondToLastDigit(s.EGN)))
            //.Where(s => Signature.IsEGNValid(Fix.FixEgnLastDigit(s.EGN,2)))
            //.Where(s => Signature.IsEGNValid(Fix.FixEgn7thDigit(s.EGN, s.City)))
            //.Where(s => Signature.IsEGNValid(Fix.FixEgn8thDigit(s.EGN, s.City)))
            //.Where(s => Signature.IsEGNValid(Fix.FixEgn2ndDigit(s.EGN)))
            //.Where(s => Signature.IsEGNValid(Fix.FixEgnSwapLastTwoDigits(s.EGN)))
            .ToList();
            var tabDelimited = String.Join("\r\n", signaturesEGN.Select(s =>
                //Fix.FixEgn8thDigit(s.EGN, s.City) 
                //Fix.FixEgn1stDigit(s.EGN)
                //Fix.FixEgn2ndDigit(s.EGN)
                Fix.FixEgn7thDigit(s.EGN, s.City)
                    //Fix.FixEgn10thDigit(s.EGN, 2)
                    //Fix.FixEgn9thDigit(s.EGN)
                    //Fix.FixEgn6thDigit(s.EGN)
                + '\t' + s.ToString()).ToArray()); //..Aggregate("", (r, s) => r + s.ToString() + "\r\n");

            //File.WriteAllLines(@"C:\Kredor\Source\Referendum\BLANKA Cloud 410k-new2-trimmed.csv", signatures.Select(s => s.ToString()).ToArray());

            //var invalidEGN1st = signatures.Where(s => Signature.IsEGNValid(s.EGN) && (s.EGN.StartsWith("2") || s.EGN.StartsWith("1")))// || s.EGN.StartsWith("3")) )
            //    //.OrderBy(s => s.EGN.Substring(0,1))
            //    .Count();

            var h = 1;
            var fixedSignatures = signatures.ApplyFix(Fix.Fixes[0]).ApplyFix(Fix.Fixes[1]).ApplyFix(Fix.Fixes[2]).ApplyFix(Fix.Fixes[3]).ApplyFix(Fix.Fixes[4]).ApplyFix(Fix.Fixes[5]).ApplyFix(Fix.Fixes[6])
                .ApplyFix(Fix.Fixes[7])
                .ApplyFix(Fix.Fixes[8]).ApplyFix(Fix.Fixes[9]).ApplyFix(Fix.Fixes[10]).ApplyFix(Fix.Fixes[11])
                .ApplyFix(Fix.Fixes[12]).ApplyFix(Fix.Fixes[13]).ApplyFix(Fix.Fixes[14]).ApplyFix(Fix.Fixes[15])
                .ToList();

            var invalidEGN = signatures.Where(s => !Signature.IsEGNValid(s.EGN)).Count();
            var invalidDOB = signatures.Where(s =>  s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && !Signature.IsEGNDOBValid(s.EGN)).Count();
            var invalidDOBinFixed = fixedSignatures.Where(s => s.EGN.Length == 10 && Regex.IsMatch(s.EGN, "^[0-9]*$") && !Signature.IsEGNValid(s.EGN) && !Signature.IsEGNDOBValid(s.EGN)).ToList();

            var fixes = fixedSignatures.Where(s => s.Original != null).ToList();
            var fixesEgn = fixedSignatures.Where(s => s.Original == null ? false : s.EGN != s.Original.EGN).ToList();
            var notfixedEgn = fixedSignatures.Where(s => s.Original == null ? true : s.EGN == s.Original.EGN).ToList();
            var tabDelimited1 = String.Join("\r\n", invalidDOBinFixed.Select(s => s.ToString()).ToArray());

            var outputFilePath = @"C:\Kredor\Source\Referendum\BLANKA_FINAL-2-fixed-w.txt";
            var separator = "\t";
            //File.WriteAllLines(outputFilePath, fixedSignatures.Select((s,i) => (i-1).ToString() + separator + s.ToString(separator)).ToArray(), Encoding.GetEncoding("windows-1251"));File.WriteAllLines(outputFilePath, fixedSignatures.Select((s,i) => (i-1).ToString() + separator + s.ToString(separator)).ToArray(), Encoding.GetEncoding("windows-1251"));
            File.WriteAllLines(outputFilePath, fixedSignatures.Select((s, i) => s.ToString(separator)).ToArray(), Encoding.GetEncoding("windows-1251"));
            var d = 1;
            double inttemp;




            //FixSignatures(signatures);

            //var common = signatures.Where(s => !double.TryParse(s.FirstName, out inttemp) && Signature.IsEGNValid(s.EGN))
            //                .GroupBy(s => s.FirstName)
            //                .Select(g => new
            //                {
            //                    FirstName = g.Key,
            //                    Count = g.Count(),
            //                    Sex = int.Parse(g.First().EGN.Substring(8,1))%2,
            //                    All = g.ToList()
            //                }
            //                ).Where(i => i.Count > 5
            //    //&& (i.FirstFirstName != i.LastFirstName || i.FirstLastName != i.LastLastName)
            //                    ).OrderByDescending(i => i.Count)
            //    //.Sum(i => i.Count);
            //                .ToList();

            //var rare = signatures.Where(s => !double.TryParse(s.FirstName, out inttemp) && Signature.IsEGNValid(s.EGN))
            //                .GroupBy(s => s.FirstName)
            //                .Select(g => new
            //                {
            //                    FirstName = g.Key,
            //                    Count = g.Count(),
            //                    Sex = int.Parse(g.First().EGN.Substring(8, 1)) % 2,
            //                    First = g.First(),
            //                    All = g.ToList()
            //                }
            //                ).Where(i => i.Count == 1
            //    //&& (i.FirstFirstName != i.LastFirstName || i.FirstLastName != i.LastLastName)
            //                    ).OrderByDescending(i => i.Count)
            //    //.Sum(i => i.Count);
            //                .ToList();

            //var misspelled = rare.Select(n => new
            //{
            //    Suspect = n.FirstName,
            //    Suggestion = common.Where(c => c.Sex == n.Sex && Signature.LevenshteinDistance(c.FirstName, n.FirstName) == 1 && c.FirstName.Length >= 5).Select(c => c.FirstName).FirstOrDefault(),
            //    n.First
            //}).Where(c => c.Suggestion != null).ToList();
            var withSpacesegn = signatures.Where(s => s.EGN.EndsWith(" ")).ToList();

            var withSpaces = signatures.Where(s => s.FirstName.StartsWith(" ") || s.FirstName.EndsWith(" ") || s.MiddleName.StartsWith(" ") || s.MiddleName.EndsWith(" ") || s.LastName.EndsWith(" ") || s.City.EndsWith(" ") || s.LocationUnitName.EndsWith(" ")).ToList();

            var commonMiddle = signatures.Where(s => !double.TryParse(s.MiddleName, out inttemp) && Signature.IsEGNValid(s.EGN))
            .GroupBy(s => s.MiddleName)
            .Select(g => new
            {
                MiddleName = g.Key,
                Count = g.Count(),
                Sex = int.Parse(g.First().EGN.Substring(8, 1)) % 2,
                All = g.ToList()
            }
            ).Where(i => i.Count > 10
                        //&& (i.FirstFirstName != i.LastFirstName || i.FirstMiddleName != i.LastMiddleName)
                ).OrderByDescending(i => i.Count)
            //.Sum(i => i.Count);
            .ToList();

            var rareMiddle = signatures.Where(s => !double.TryParse(s.MiddleName, out inttemp) && Signature.IsEGNValid(s.EGN))
                            .GroupBy(s => s.MiddleName)
                            .Select(g => new
                            {
                                MiddleName = g.Key,
                                Count = g.Count(),
                                Sex = int.Parse(g.First().EGN.Substring(8, 1)) % 2,
                                First = g.First(),
                                All = g.ToList()
                            }
            ).Where(i => i.Count == 1
            ).OrderByDescending(i => i.Count)
                //.Sum(i => i.Count);
            .ToList();

            var misspelledMiddle = rareMiddle.Select(n => new
            {
                Suspect = n.MiddleName,
                Suggestion = commonMiddle.Where(c => c.Sex == n.Sex && Signature.LevenshteinDistance(c.MiddleName, n.MiddleName, 1, 1, 1) == 1).Select(c => c.MiddleName).FirstOrDefault(),
                n.First
            }).Where(c => c.Suggestion != null).ToList();

            var misspelledTabDelimited = String.Join("\n", misspelledMiddle.Select(m => m.First.PageNo + '\t' + m.Suspect + '\t' + m.Suggestion));         

           var commonLast = signatures.Where(s => !double.TryParse(s.LastName, out inttemp) && Signature.IsEGNValid(s.EGN))
                .GroupBy(s => s.LastName)
                .Select(g => new
                {
                    LastName = g.Key,
                    Count = g.Count(),
                    Sex = int.Parse(g.First().EGN.Substring(8, 1)) % 2,
                    All = g.ToList()
                }
                ).Where(i => i.Count > 10
                //&& (i.FirstFirstName != i.LastFirstName || i.FirstLastName != i.LastLastName)
                    ).OrderByDescending(i => i.Count)
                //.Sum(i => i.Count);
               .ToList();

            //var rareLast = signatures.Where(s => !double.TryParse(s.LastName, out inttemp) && Signature.IsEGNValid(s.EGN))
            //                .GroupBy(s => s.LastName)
            //                .Select(g => new
            //                {
            //                    LastName = g.Key,
            //                    Count = g.Count(),
            //                    Sex = int.Parse(g.First().EGN.Substring(8, 1)) % 2,
            //                    First = g.First(),
            //                    All = g.ToList()
            //                }
            //                ).Where(i => i.Count == 1
            //    //&& (i.FirstFirstName != i.LastFirstName || i.FirstLastName != i.LastLastName)
            //                    ).OrderByDescending(i => i.Count)
            //    //.Sum(i => i.Count);
            //                .ToList();

            //var misspelledLast = rareLast.Select(n => new
            //{
            //    Suspect = n.LastName,
            //    Suggestion = commonLast.Where(c => c.Sex == n.Sex && Signature.LevenshteinDistance(c.LastName, n.LastName, 3, 1, 1) == 1 && c.LastName.Length >= 5).Select(c => c.LastName).FirstOrDefault(),
            //    n.First
            //}).Where(c => c.Suggestion != null).ToList();
            //var signaturesO = signatures.GroupBy(s => s.EGN)
            //    .Select(g => new
            //    {
            //        EGN = g.Key,
            //        Count = g.Count(),
            //        FirstFirstName = g.First().FirstName,
            //        LastFirstName = g.Last().FirstName,
            //        FirstLastName = g.Last().LastName,
            //        LastLastName = g.Last().LastName,
            //        All = g.ToList()
            //    }
            //    ).Where(i => i.Count > 1
            //        && i.EGN != ""
            //        && (i.FirstFirstName != i.LastFirstName)
            //    //&& (i.FirstFirstName != i.LastFirstName || i.FirstLastName != i.LastLastName)
            //        ).OrderByDescending(i => i.Count)
            //    //.Sum(i => i.Count);
            //    .ToList();

            //FixMaleNames(signatures);
        }


        static string ToTabDelimited(IEnumerable<Signature> singatures)
        {
            return singatures.Aggregate("", (r, s) => s.ToString() + "\r\n");
        }
    }
}
