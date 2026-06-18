using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotdiodeInterface.services
{
    internal class AcquisitionSettings
    {
        public string PhysicalChannel { get; set; }

        public double MinimumVoltage { get; set; }
        public double MaximumVoltage { get; set; }

        public double SampleRate { get; set; }
        public int SamplesPerRead { get; set; }

        public bool UseTextFile { get; set; }

        public string OutputFolder { get; set; }

        public string TriggerSource { get; set; }

        public DigitalLevelPauseTriggerCondition PauseCondition { get; set; }
    }
}
