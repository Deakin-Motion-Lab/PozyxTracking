using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pozyx_HeightData
{
    static class FILE_IO
    {
        private static string baseFilePath = @"C:\Projects\Pozyx\Error Analysis\Logs\";

        public static void WriteLog(string data)
        {
            string filePath = baseFilePath + "Positioning_Log.csv";
  
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw;

            try
            {
                sw = new StreamWriter(fs);
                sw.WriteLine("X,Y,Z");
                sw.Write(data);
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
