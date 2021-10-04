using System.Threading.Tasks;
using System.Collections.Generic;
using Mantel.Programming.Tasks.Models;

namespace Mantel.Programming.Tasks.Services.Interfaces
{
    public interface ILogFileAnalyserService
    {
        public Task<Response<bool>> ProcessFileAsync(string filename);
        public Response<IList<KeyValuePair<string, int>>> FindMostActiveAddresses(int count);
        public Response<IList<KeyValuePair<string, int>>> FindMostVisitedUrls(int count);
        public int CountUniqueIpAddresses();
    }
}