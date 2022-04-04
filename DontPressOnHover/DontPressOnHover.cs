using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using System;

namespace DontPressOnHover
{
    [PluginName("Dont Press On Hover")]
    public class DontPressOnHover : IPositionedPipelineElement<IDeviceReport>
    {
        public IDeviceReport Hover_Distance(IDeviceReport input)
        {
            if (input is ITabletReport tabletReport and IProximityReport proximityReport)
            {
                if (proximityReport.HoverDistance > Hover_max)
                {
                    tabletReport.Pressure = 0;
                    return input;
                }
            }
            return input;
        }

        public IDeviceReport Near_Proximity(IDeviceReport input)
        {
            return input;
        }

        public IDeviceReport Pressure_Cutoff(IDeviceReport input)
        {
            return input;
        }

        public event Action<IDeviceReport> Emit;

        public void Consume(IDeviceReport value)
        {
            var report = Pressure_Cutoff(value);
            report = Near_Proximity(report);
            report = Hover_Distance(report);

            Emit?.Invoke(report);
        }

        public PipelinePosition Position => PipelinePosition.PreTransform;

        [Property("Maximum hover distance before liftoff"), DefaultPropertyValue(63f), ToolTip
            ("The maximum HoverDistance where input for the mouse buttons is sent.\n\n" +
            "(HoverDistance can be found in the tablet debugger for supported tablets.)")]
        public float Hover_max { set; get; }
    }
}