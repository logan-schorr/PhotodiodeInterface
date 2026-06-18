using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotdiodeInterface.services
{
    internal class FileManager
    {
        private StreamWriter textWriter;
        private BinaryWriter binaryWriter;
        private StreamWriter statisticsWriter;

        private bool useTextFile;

        public string SessionFolder { get; private set; }

        public void CreateSession(
            string outputRootFolder,
            bool useTextFile,
            NationalInstruments.DAQmx.Task task)
        {
            this.useTextFile = useTextFile;

            string timestamp =
                DateTime.Now.ToString("yyyy-MM-dd_HHmmss");

            SessionFolder =
                Path.Combine(outputRootFolder, timestamp);

            Directory.CreateDirectory(SessionFolder);

            string waveformFile =
                Path.Combine(
                    SessionFolder,
                    useTextFile ?
                    "acquisitionData.txt" :
                    "acquisitionData.bin");

            string statisticsFile =
                Path.Combine(
                    SessionFolder,
                    "processStatistics.csv");

            FileStream fs =
                new FileStream(
                    waveformFile,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.Read);

            if (useTextFile)
                textWriter = new StreamWriter(fs);
            else
                binaryWriter = new BinaryWriter(fs);

            statisticsWriter =
                new StreamWriter(statisticsFile);

            WriteHeader(task);
        }

        private void WriteHeader(NationalInstruments.DAQmx.Task task)
        {
            if (useTextFile)
            {
                foreach (AIChannel channel in task.AIChannels)
                {
                    textWriter.Write(channel.PhysicalName);
                    textWriter.Write('\t');
                }

                textWriter.WriteLine();
            }

            statisticsWriter.WriteLine(
                "UtcTimestamp,OADate,ElapsedSeconds,MeanVoltage,MinVoltage,MaxVoltage,RMSVoltage");
        }

        public void WriteWaveform(double[,] data)
        {
            int channels = data.GetLength(0);
            int samples = data.GetLength(1);

            if (useTextFile)
            {
                for (int i = 0; i < samples; i++)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        textWriter.Write(
                            data[ch, i].ToString("E6"));

                        if (ch < channels - 1)
                            textWriter.Write('\t');
                    }

                    textWriter.WriteLine();
                }

                textWriter.Flush();
            }
            else
            {
                for (int i = 0; i < samples; i++)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        binaryWriter.Write(data[ch, i]);
                    }
                }

                binaryWriter.Flush();
            }
        }

        public void WriteStatistics(
            string csvLine)
        {
            statisticsWriter.WriteLine(csvLine);
            statisticsWriter.Flush();
        }

        public void Dispose()
        {
            textWriter?.Dispose();
            binaryWriter?.Dispose();
            statisticsWriter?.Dispose();
        }
    }
}
