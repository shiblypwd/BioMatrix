using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BioMetrixCore
{
    public class Post
    {
        public int id { get; set; }
        public int attendanceCardId { get; set; }

        public DateTime entryTime { get; set; }
        public string machineName { get; set; }
        public string verifyMode    { get; set; }
        public int inOutStatus { get; set; }
    }
}
