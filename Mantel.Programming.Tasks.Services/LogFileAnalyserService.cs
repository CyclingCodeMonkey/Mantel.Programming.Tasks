using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mantel.Programming.Tasks.Models;
using Mantel.Programming.Tasks.Repository;
using Mantel.Programming.Tasks.Services.Interfaces;

namespace Mantel.Programming.Tasks.Services
{
    public class LogFileAnalyserService : ILogFileAnalyserService
    {
        private readonly IFileHandlingService _fileHandlingService;
        private readonly IDataStore _dataStore;

        public LogFileAnalyserService(IFileHandlingService fileHandlingService, IDataStore dataStore)
        {
            _fileHandlingService = fileHandlingService;
            _dataStore = dataStore;
        }

        public async Task<Response<bool>> ProcessFileAsync(string filename)
        {
            var response = new Response<bool>();
            try
            {
                var result = await _fileHandlingService.ReadLogFileAsync(filename);

                // inject and do validation here

                if (result.Any())
                    _dataStore.AddLogData(result);
                response.Data = result.Any();
            }
            catch (Exception exception)
            {
                response.ErrorMessages = new List<Error>
                {
                    new Error
                    {
                        ErrorMessage = exception.Message
                    }
                };
            }
            return response;
        }

        public Response<IList<KeyValuePair<string, int>>> FindMostActiveAddresses(int count)
        {
            if (count <= 0)
                count = 0;

            var sortedIpAddresses = SortedIpAddressesByCount();
            var response = new Response<IList<KeyValuePair<string, int>>>
            {
                Data = sortedIpAddresses.Take(count).ToList()
            };

            return response;
        }

        private IEnumerable<KeyValuePair<string, int>> SortedIpAddressesByCount()
        {
            var ipAddresses = new Dictionary<string, int>();
            foreach (var dataRaw in _dataStore.GetLogDataRaws())
            {
                if (ipAddresses.ContainsKey(dataRaw.IpAddress))
                {
                    ipAddresses[dataRaw.IpAddress] += 1;
                }
                else
                {
                    ipAddresses.Add(dataRaw.IpAddress, 1);
                }
            }

            var sortedIpAddresses =
                (from entry in ipAddresses orderby entry.Value descending select entry);
            return sortedIpAddresses;
        }

        public Response<IList<KeyValuePair<string, int>>> FindMostVisitedUrls(int count)
        {
            if (count <= 0)
                count = 0;

            var urls = new Dictionary<string, int>();
            foreach (var dataRaw in _dataStore.GetLogDataRaws())
            {
                var key = dataRaw.Url.ToLower();
                if (urls.ContainsKey(key))
                {
                    urls[key] += 1;
                }
                else
                {
                    urls.Add(key, 1);
                }
            }
            var sortedUrls =
                (from entry in urls orderby entry.Value descending select entry).Take(count).ToList();
            return new Response<IList<KeyValuePair<string, int>>>
            {
                Data = sortedUrls
            };
        }

        public int CountUniqueIpAddresses()
        {
            var sortedIpAddresses = SortedIpAddressesByCount();
            return sortedIpAddresses.Count();
        }
    }
}