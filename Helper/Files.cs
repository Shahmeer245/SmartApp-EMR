using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Files
    {
        public static void SaveToCSV(string serviceName, string source, DateTime startTime, DateTime endTime)
        {
            string csvHeader = "serviceName,source,startTime,endTime,duration";
                        
            string csvFilename = "C:\\Logs\\PerformanceLog.csv";

            string csv = string.Format("{0},{1},{2},{3},{4}", serviceName, source, startTime, endTime, (endTime - startTime).Seconds);

            string existingContents;
            if(!File.Exists(csvFilename))
            {
                FileStream stream = File.Create(csvFilename);

                stream.Close();
            }

            using (StreamReader sr = new StreamReader(csvFilename))
            {
                existingContents = sr.ReadToEnd();
            }

            using (StreamWriter writetext = File.AppendText(csvFilename))
            {
                if (!existingContents.Contains(csvHeader))
                {
                    writetext.WriteLine(csvHeader);
                }
                writetext.WriteLine(csv);
            }
        }
    }
}
