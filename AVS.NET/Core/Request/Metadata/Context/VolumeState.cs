using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.Core.Request.Metadata.Context
{
    public class VolumeState : ContextItem
    {
        public VolumeState(int volume, bool muted)
        {
            Header.Namespace = "Speaker";
            Header.Name = "VolumeState";

            if (volume < 0 && volume > 100)
            {
                throw new ArgumentOutOfRangeException("Volume value must be between 0 and 100");
            }

            Payload.Add("volume", volume);
            Payload.Add("muted", muted);
        }
    }
}
