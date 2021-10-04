using System.Collections.Generic;
using System.Linq;

namespace Mantel.Programming.Tasks.Models
{
    public class Response<T>
    {
        public Response()
        {
            ErrorMessages = new List<Error>();
        }
        
        public T Data { get; set; }
        
        public bool IsSuccessful => ErrorMessages.Any() == false;

        public IList<Error> ErrorMessages { get; set; }
    }
}