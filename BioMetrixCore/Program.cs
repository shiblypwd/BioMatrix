using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using BioMetrixCore.Utilities;
using System.Linq;
using System.Text.RegularExpressions;

namespace BioMetrixCore
{

    static class Program
    {
       
        [STAThread]
        static void Main()
        {
            //getTodayInData();
            new LocalSMS().processLocalSMS();            
        }        
    }
}
