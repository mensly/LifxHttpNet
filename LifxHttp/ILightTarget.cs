using LifxHttp.Enums;
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

        Task<ResponseType> TogglePower(double duration = LifxClient.DEFAULT_DURATION);
        Task<ResponseType> SetPower(PowerState powerState, double duration = LifxClient.DEFAULT_DURATION);
        Task<ResponseType> SetColor(LifxColor color, double duration = LifxClient.DEFAULT_DURATION, bool powerOn = LifxClient.DEFAULT_POWER_ON);
        Task<ResponseType> SetState(PowerState powerState, LifxColor color, double brightness, double duration = LifxClient.DEFAULT_DURATION, double infrared = LifxClient.DEFAULT_INFRARED);
        Task<ResponseType> PulseEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON);
        Task<ResponseType> BreatheEffect(LifxColor color, double period, double cycles, LifxColor fromColor = LifxClient.DEFAULT_FROM_COLOR, bool persist = LifxClient.DEFAULT_PERSIST, bool powerOn = LifxClient.DEFAULT_POWER_ON, double peak = LifxClient.DEFAULT_PEAK);
        Task<ResponseType> Cycle(List<LightState> states, LightState defaults, Direction direction = LifxClient.DEFAULT_DIRECTION);
    }
}
