using Http2;
using Http2.Hpack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AVS.Core.Http2
{
    internal struct Http2Response
    {
        private HttpStatusCode _statusCode;

        public Http2Response(IStream stream, IDictionary<string, string> headers, byte[] data)
        {
            Headers = headers;
            Data = data;
            Stream = stream;

            var code = Headers[":status"];
            _statusCode = Enum.Parse<HttpStatusCode>(code);
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
        }

        public IStream Stream { get; private set; }

        public IDictionary<string, string> Headers { get; private set; }

        public byte[] Data { get; private set; }

        public bool IsSuccessStatusCode
        {
            get
            {
                return _statusCode >= HttpStatusCode.OK && _statusCode < HttpStatusCode.MultipleChoices;
            }
        }
    }
}
