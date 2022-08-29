using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BioMetrixCore
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static void writeToFile(string content)
        {
            string path = @"E:\Gate\out.txt";
            File.AppendAllText(path, content+"\n");
        }

        public static void writeToFileWithoutNL(string content)
        {
            string path = @"E:\Gate\out.txt";
            File.AppendAllText(path, content);
        }

        [STAThread]
        static void Main()
        {
            
            writeToFile("Started+++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Master());
        }
    }
}
