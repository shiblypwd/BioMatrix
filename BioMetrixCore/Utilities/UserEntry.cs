using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    public class UserEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; } 
        public string DataStr { get; set; } 
        public DateTime EntryTime { get; set; }

        public UserEntry(int id, string dataStr, DateTime entryTime)
        {
            Id=id;
            DataStr=dataStr;
            EntryTime=entryTime;
        }

        public UserEntry(int id, string name, string designation, string dataStr, DateTime entryTime)
        {
            Id = id;
            Name = name;
            this.Designation = designation;
            DataStr = dataStr;
            EntryTime = entryTime;
        }

        public UserEntry(int id, string name, string designation)
        {
            Id = id;
            Name = name;
            this.Designation = designation;
        }
    }
}
