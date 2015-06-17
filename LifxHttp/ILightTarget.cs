using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifxHttp
{
    public interface ILightTarget<ResponseType>
    {
        string Id { get; }
        string Label { get; }

        bool IsOn { get; }

        Task<ResponseType> TogglePower();
        Task<ResponseType> SetPower(bool powerState, float duration = LifxClient.DEFAULT_DURATION);
        Task<ResponseType> SetColor(LifxColor color, float duration = LifxClient.DEFAULT_DURATION, bool powerOn = LifxClient.DEFAULT_POWER_ON);
    }
}
