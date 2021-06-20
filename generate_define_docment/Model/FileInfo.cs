using System;
using System.Collections.Generic;
using System.Text;

namespace generate_define_docment.Model
{
    class FIleInfo
    {
        public string PhysicsTableName{get;set;}
        public string LogicalTableName{get;set;}
        public Dictionary<string, string> Columns{get;set;} 
    }
}
