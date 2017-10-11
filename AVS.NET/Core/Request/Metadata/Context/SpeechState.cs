using AVS.Enums;
using System;

namespace AVS.Core.Request.Metadata.Context
{
    public class SpeechState : ContextItem
    {
        public SpeechState(string token, long offsetInMilliseconds, PlayerActivity playerActivity)
        {
            Header.Namespace = "SpeechSynthesizer";
            Header.Name = "SpeechState";

            Payload.Add("token", token);
            Payload.Add("offsetInMilliseconds", offsetInMilliseconds);
            Payload.Add("playerActivity", Enum.GetName(playerActivity.GetType(), playerActivity));
        }
    }
}
