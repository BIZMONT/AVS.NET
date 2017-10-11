using AVS.Enums;
using System;

namespace AVS.Core.Request.Metadata.Context
{
    public class PlaybackState : ContextItem
    {
        public PlaybackState(string token, long offsetInMilliseconds, PlayerActivity playerActivity)
        {
            Header.Namespace = "AudioPlayer";
            Header.Name = "PlaybackState";

            Payload.Add("token", token);
            Payload.Add("offsetInMilliseconds", offsetInMilliseconds);
            Payload.Add("playerActivity", Enum.GetName(playerActivity.GetType(), playerActivity));
        }
    }
}
