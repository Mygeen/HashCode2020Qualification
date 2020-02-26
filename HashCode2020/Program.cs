using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace HackerRankProblems.Solutions
{
    public class SampleSolution
    {
        public static void SimulateBookScanning(StreamWriter output, int[] bookScores, int[][] booksByLibrary, int[] signUpDurations, int[] maxScansPerDayByLib, int totalDays)
        {
            var libraryScores = new LibraryScore[booksByLibrary.Length];
            for (var i = 0; i < libraryScores.Length; i++)
            {
                libraryScores[i] = new LibraryScore
                {
                    Index = i,
                    Score = booksByLibrary[i].Sum(x => (long)bookScores[x]) * maxScansPerDayByLib[i] / signUpDurations[i]
                };
                booksByLibrary[i] = booksByLibrary[i].OrderByDescending(x => bookScores[x]).ToArray();
            }
            libraryScores = libraryScores.OrderByDescending(x => x.Score).ToArray();
            var currentSigningLibrary = -1;
            var remainingSignUpDays = 0;
            var libraryCount = signUpDurations.Length;
            var signedUpLibraries = new List<LibraryBookStatus>();
            var isBookScanned = new bool[bookScores.Length];
            for (var day = 0; day < totalDays; day++)
            {
                PickALibraryToSignUpIfItIsTime(signUpDurations, totalDays, libraryScores, ref currentSigningLibrary, ref remainingSignUpDays, libraryCount, signedUpLibraries, day);
                ScanBooksForTheDay(booksByLibrary, maxScansPerDayByLib, signedUpLibraries, isBookScanned);
                remainingSignUpDays--;
            }
            //Filtering out libraries that did not send anybook for scanning
            signedUpLibraries = signedUpLibraries.Where(x => x.CurrentBookIndex + 1 - x.SkippedBooks.Count > 0).ToList();
            OutputSolution(output, booksByLibrary, signedUpLibraries);
        }
        private static void OutputSolution(StreamWriter output, int[][] booksByLibrary, List<LibraryBookStatus> signedUpLibraries)
        {
            output.WriteLine(signedUpLibraries.Count);
            for (int libIndex = 0; libIndex < signedUpLibraries.Count; libIndex++)
            {
                var library = signedUpLibraries[libIndex];
                output.WriteLine($"{library.Index} {library.CurrentBookIndex + 1 - library.SkippedBooks.Count}");
                for (int bookIndex = 0; bookIndex < library.CurrentBookIndex + 1; bookIndex++)
                {
                    var actualIndex = booksByLibrary[library.Index][bookIndex];
                    if (!library.SkippedBooks.Contains(actualIndex))
                    {
                        output.Write($"{booksByLibrary[library.Index][bookIndex]} ");
                    }
                }
                output.WriteLine();
            }
        }
        private static void PickALibraryToSignUpIfItIsTime(int[] signUpDurations, int totalDays, LibraryScore[] libraryScores, ref int currentSigningLibrary, ref int remainingSignUpDays, int libraryCount, List<LibraryBookStatus> signedUpLibraries, int day)
        {
            if (remainingSignUpDays <= 0 && currentSigningLibrary < libraryCount)
            {
                if (currentSigningLibrary > -1)
                {
                    signedUpLibraries.Add(
                        new LibraryBookStatus()
                        {
                            CurrentBookIndex = -1,
                            Index = libraryScores[currentSigningLibrary].Index
                        });
                }
                currentSigningLibrary++;
                for (; currentSigningLibrary < libraryCount && signUpDurations[currentSigningLibrary] + day > totalDays; currentSigningLibrary++)
                {
                }
                if (currentSigningLibrary < libraryCount)
                {
                    remainingSignUpDays = signUpDurations[libraryScores[currentSigningLibrary].Index];
                }
            }
        }
        private static void ScanBooksForTheDay(int[][] booksByLibrary, int[] maxScansPerDayByLib, List<LibraryBookStatus> signedUpLibraries, bool[] isBookScanned)
        {
            for (int lib = 0; lib < signedUpLibraries.Count; lib++)
            {
                var library = signedUpLibraries[lib];
                if (library.CurrentBookIndex < booksByLibrary[library.Index].Length)
                {
                    int unscannedBookCounter = 0;
                    int bb = library.CurrentBookIndex + 1;
                    for (;
                        bb < booksByLibrary[library.Index].Length &&
                        unscannedBookCounter < maxScansPerDayByLib[library.Index];
                        bb++)
                    {
                        if (isBookScanned[booksByLibrary[library.Index][bb]])
                        {
                            library.SkippedBooks.Add(booksByLibrary[library.Index][bb]);
                        }
                        else
                        {
                            unscannedBookCounter++;
                        }
                    }
                    var newIndex = bb - 1;
                    for (int b = library.CurrentBookIndex + 1; b <= newIndex; b++)
                        isBookScanned[booksByLibrary[library.Index][b]] = true;
                    library.CurrentBookIndex = newIndex;
                }
            }
        }
        public static void Main(string[] args)
        {
            var file = new StreamReader("input.txt");
            var numbers = file.ReadLine()?.Split(" ").Select(int.Parse).ToArray();
            var libraryCount = numbers[1];
            var totalDays = numbers[2];
            var bookScores = file.ReadLine()?.Split(" ").Select(int.Parse).ToArray();
            var booksByLibraries = new int[libraryCount][];
            var signupDurations = new int[libraryCount];
            var maxScanPerDayByLibrary = new int[libraryCount];
            for (int libIndex = 0; libIndex < libraryCount; libIndex++)
            {
                var libNumbers = file.ReadLine()?.Split(" ").Select(int.Parse).ToArray();
                booksByLibraries[libIndex] = file.ReadLine()?.Split(" ").Select(int.Parse).ToArray(); ;
                signupDurations[libIndex] = libNumbers[1];
                maxScanPerDayByLibrary[libIndex] = libNumbers[2];
            }
            using (var outputFile = new StreamWriter("output.txt"))
            {
                SimulateBookScanning(outputFile, bookScores, booksByLibraries, signupDurations, maxScanPerDayByLibrary, totalDays);
            }
        }
    }
    public class LibraryScore
    {
        public int Index;
        public long Score;
    }
    public class LibraryBookStatus
    {
        public int Index;
        public int CurrentBookIndex;
        public List<int> SkippedBooks = new List<int>();
    }
}