using Http2.Hpack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AVS.Core.Http2
{
    internal struct Http2ResponseMessage
    {
        private HttpStatusCode _statusCode;

        public Http2ResponseMessage(IEnumerable<HeaderField> headers, byte[] data)
        {
            Headers = headers;
            Data = data;

            var code = Headers.First(header => header.Name.Equals("statuscode", StringComparison.InvariantCultureIgnoreCase)).Value;
            _statusCode = Enum.Parse<HttpStatusCode>(code);
        }

        public HttpStatusCode StatusCode
        {
            get
            {

                return _statusCode;
            }
        }
        public IEnumerable<HeaderField> Headers { get; private set; }
        public byte[] Data { get; private set; }
    }
}
