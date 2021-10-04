using System.Collections.Generic;
using Mantel.Programming.Tasks.Models;

namespace Mantel.Programming.Tasks.Repository
{
    /// <summary>
    /// This is the Data Store Interface and should Ideally by in a separate project
    /// away from the implementation
    /// </summary>
    public interface IDataStore
    {
        public IEnumerable<LogDataRaw> GetLogDataRaws();
        public void AddLogData(IList<LogDataRaw> dataRaws);
    }
}