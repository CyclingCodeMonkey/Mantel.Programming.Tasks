using System.Collections.Generic;
using Mantel.Programming.Tasks.Models;

namespace Mantel.Programming.Tasks.Repository
{
    /// <summary>
    /// This is the fake data store
    /// </summary>
    public class DataStore : IDataStore
    {
        private IList<LogDataRaw> _dataLog;

        public DataStore()
        {
            _dataLog = new List<LogDataRaw>();
        }

        public IEnumerable<LogDataRaw> GetLogDataRaws()
        {
            return _dataLog;
        }

        public void AddLogData(IList<LogDataRaw> dataRaws)
        {
            foreach (var dataRaw in dataRaws)
            {
                _dataLog.Add(dataRaw);
            }
        }
    }
}