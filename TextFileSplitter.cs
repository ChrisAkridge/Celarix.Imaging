using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using NLog;
using LongFile = Pri.LongPath.File;
using LongFileInfo = Pri.LongPath.FileInfo;
using LongPath = Pri.LongPath.Path;
using LongDirectoryInfo = Pri.LongPath.DirectoryInfo;

namespace Celarix.IO.FileAnalysis.Analysis
{
    /* internal */ public static class TextFileSplitter
    {
        private const int LineWidth = 80;
        private const int PageWidth = 80;
        private const int PageHeight = 50;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static bool TryCreatePagesForTextFile(string filePath)
        {
            if (!Utilities.Utilities.IsTextFile(filePath))
            {
                logger.Info($"{filePath} is not a text file... probably");

                return false;
            }

            var lines = LongFile
                .ReadAllLines(filePath)
                .Select(ReplaceTabsWithSpaces)
                .ToArray();
            
            logger.Info($"File {filePath} has {lines.Length} lines");

            var lineMarginWidth = CalculateLineMarginWidth(lines.Length);
            var printedLines = lines
                .Select((l, i) => LineToPrintedLine(l, lineMarginWidth, i))
                .SelectMany(l => l)
                .ToList();

            lines = null;
            
            var pageCount = Math.Max(printedLines.Count / 49, 1);
            var pageCountWidth = CalculatePageCountWidth(pageCount);
            var pageLabelWidth = CalculatePageLabelWidth(pageCountWidth);
            var maxFileNameWidth = CalculateMaxFileNameWidth(pageLabelWidth);
            var linesForEachPage = printedLines.Batch(49);
            var pageBuilder = new StringBuilder();
            var currentPageNumber = 1;

            foreach (var linesForPage in linesForEachPage)
            {
                var pageLabel = GeneratePageLabel(pageCountWidth, currentPageNumber, pageCount);
                var fileLabel = GenerateFileLabel(filePath, maxFileNameWidth);
                pageBuilder.AppendLine($"{fileLabel} {pageLabel}");

                foreach (var line in linesForPage)
                {
                    pageBuilder.AppendLine(line);
                }
                
                SavePage(filePath, currentPageNumber - 1, pageCountWidth, pageBuilder.ToString());
                pageBuilder.Clear();
                currentPageNumber += 1;

                if (currentPageNumber % 10 == 0)
                {
                    logger.Info($"Saved page {currentPageNumber} of {pageCount} for {filePath}");
                }
            }

            return true;
        }

        private static IEnumerable<string> GetLinesFromFiles(IEnumerable<string> filePaths) =>
            filePaths.SelectMany(filePath => LongFile.ReadAllLines(filePath));

        private static string ReplaceTabsWithSpaces(string line) => line.Replace("\t", "    ");
        private static int CalculateLineMarginWidth(int lineCount) => lineCount.ToString().Length + 1;
        private static int CalculatePageCountWidth(int pageCount) => pageCount.ToString().Length;
        private static int CalculatePageLabelWidth(int pageCountWidth) => (pageCountWidth * 2) + 9;
        private static int CalculateMaxFileNameWidth(int pageLabelWidth) => 79 - pageLabelWidth;
        
        private static IEnumerable<string> LineToPrintedLine(string line, int lineMarginWidth, int lineNumber)
        {
            var lineBuilder = new StringBuilder();
            var lineWords = line.Split(' ');
            
            lineBuilder.Append((lineNumber + 1).ToString().PadRight(lineMarginWidth, ' '));

            foreach (var lineWord in lineWords.Select(w => w + " "))
            {
                if (lineWord.Length > LineWidth - lineMarginWidth)
                {
                    // This word is long! Write until the line is 79 characters,
                    // then write a -, then the rest of the word on the next line(s).
                    var longWordStack = new Queue<char>(lineWord);

                    while (longWordStack.Count > 0)
                    {
                        while (lineBuilder.Length < (LineWidth - lineMarginWidth - 1) && longWordStack.Count > 0)
                        {
                            lineBuilder.Append(longWordStack.Dequeue());
                        }

                        string resultLine;

                        if (longWordStack.Count != 0)
                        {
                            lineBuilder.Append('-');
                            resultLine = lineBuilder.ToString();
                        }
                        else { resultLine = lineBuilder.ToString(); }

                        lineBuilder.Clear();
                        lineBuilder.Append(new string(' ', lineMarginWidth));

                        yield return resultLine;
                    }
                }
                else if (lineBuilder.Length + lineWord.Length > LineWidth)
                {
                    // This word will exceed the width of the line. Return what
                    // we've got so far.
                    yield return lineBuilder.ToString();
                    
                    lineBuilder.Clear();
                    lineBuilder.Append(new string(' ', lineMarginWidth));
                    lineBuilder.Append(lineWord);

                    continue;
                }
                else { lineBuilder.Append(lineWord); }

                if (lineBuilder.Length > LineWidth) { System.Diagnostics.Debugger.Break(); }
            }

            yield return lineBuilder.ToString();
        }

        private static string GeneratePageLabel(int pageCountWidth, int pageNumber, int pageCount) =>
            $"Page {pageNumber.ToString().PadLeft(pageCountWidth, ' ')} of {pageCount}";

        private static string GenerateFileLabel(string filePath, int maxFileNameWidth) =>
            filePath.Length <= maxFileNameWidth
                ? filePath
                : "..." + filePath.Substring(filePath.Length - (maxFileNameWidth - 3));

        private static void SavePage(string filePath, int pageNumber, int pageCountWidth, string page)
        {
            var path = LongPath.Combine(Utilities.Utilities.GetTextFilePagesFolderPath(filePath),
                $"{pageNumber.ToString().PadLeft(pageCountWidth, '0')}.txt");
            new LongDirectoryInfo(LongPath.GetDirectoryName(path)).Create();
            
            LongFile.WriteAllText(path, page);
        }
    }
}
