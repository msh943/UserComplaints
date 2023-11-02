using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Exception
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode;

        public bool IsSuccess = true;

        public List<string>? Errors;

        public object? Result;
    }
}
