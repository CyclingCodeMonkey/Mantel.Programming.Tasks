using System.Collections.Generic;
using System.Threading.Tasks;
using Mantel.Programming.Tasks.Models;

namespace Mantel.Programming.Tasks.Services.Interfaces
{
    public interface IFileHandlingService
    {
        Task<IList<LogDataRaw>> ReadLogFileAsync(string filename);
    }
}