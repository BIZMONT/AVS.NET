using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.Core.Request.Metadata.Context
{
    public class RecognizerState : ContextItem
    {
        public RecognizerState()
        {
            Header.Namespace = "SpeechRecognizer";
            Header.Name = "RecognizerState";
            Payload.Add("wakeword", "ALEXA");
        }
    }
}
