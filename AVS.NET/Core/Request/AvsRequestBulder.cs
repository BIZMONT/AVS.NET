using AVS.Core.Request.Metadata;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AVS.Core.Request
{
    public class AvsRequestBulder
    {
        private ByteArrayContent _audioContent;
        private StringContent _jsonMetadataContent;

        public AvsRequestBulder()
        {
        }

        public AvsRequestBulder SetJsonMetadata(AvsRequestMetadata metadata)
        {
            _jsonMetadataContent = new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json");
            _jsonMetadataContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "\"request\"" };

            return this;
        }

        public AvsRequestBulder SetBinaryAudio(byte[] audio)
        {
            _audioContent = CreateAudioContent(audio);

            return this;
        }

        public MultipartFormDataContent Build()
        {
            MultipartFormDataContent _content = new MultipartFormDataContent("123boundary123");

            if (_jsonMetadataContent != null)
            {
                _content.Add(_jsonMetadataContent);
            }

            if (_audioContent != null)
            {
                _content.Add(_audioContent);
            }

            return _content;
        }

        private ByteArrayContent CreateAudioContent(byte[] audio)
        {
            var audioContent = new ByteArrayContent(audio);
            audioContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "\"audio\"" };

            var mediaTypeHeader = new MediaTypeHeaderValue("audio/L16");
            mediaTypeHeader.Parameters.Add(new NameValueHeaderValue("rate", "16000"));
            mediaTypeHeader.Parameters.Add(new NameValueHeaderValue("channels", "1"));

            audioContent.Headers.ContentType = mediaTypeHeader;

            return audioContent;
        }
    }
}
