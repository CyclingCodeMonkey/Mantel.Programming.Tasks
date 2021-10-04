using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mantel.Programming.Tasks.Models;
using Mantel.Programming.Tasks.Services.Interfaces;

namespace Mantel.Programming.Tasks.Services
{
    public class FileHandlingService : IFileHandlingService
    {
        public async Task<IList<LogDataRaw>> ReadLogFileAsync(string filename)
        {
            // check that the file exists
            if (!File.Exists(filename))
            {
                throw new ArgumentException($"File ({filename}), does not exist.");
            }

            // using File.ReadLines is slightly better on large files, you can start enumerating
            // the collection of strings before the whole collection is returned
            var fileLines = await File.ReadAllLinesAsync(filename);
            var dataRaws = new List<LogDataRaw>();
            foreach (var fileLine in fileLines)
            {
                var columns = fileLine.Split(' ');
                var reading = new LogDataRaw
                {
                    IpAddress = columns[0],
                    UserIdentifier = CleanInput(columns[1]),
                    Username = CleanInput(columns[2]),
                    DateTime = CleanInput(columns[3]),
                    Offset = CleanInput(columns[4]),
                    HttpVerb = CleanInput(columns[5]),
                    Url = columns[6],
                    Protocol = CleanInput(columns[7]),
                    HttpStatusCode = CleanInput(columns[8]),
                    Size = CleanInput(columns[9]),
                    UserAgent = BuildUserAgent(columns)
                };
                dataRaws.Add(reading);
            }
            return dataRaws;
        }

        private static string CleanInput(string rawValue)
        {
            return rawValue?.Replace("-", string.Empty)?
                .Replace(@"""", string.Empty)?
                .Replace("[", string.Empty)?
                .Replace("]", string.Empty);
        }

        private static string BuildUserAgent(string[] columns)
        {
            var userAgent = new StringBuilder();
            for (var i = 11; i < columns.Length; i++)
            {
                userAgent.Append(columns[i]);
                userAgent.Append(' ');
            }

            return userAgent.ToString().Trim();
        }
    }
}