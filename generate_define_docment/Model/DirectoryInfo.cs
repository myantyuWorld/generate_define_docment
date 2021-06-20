using System;
using System.Collections.Generic;
using System.Text;

namespace generate_define_docment.Model
{
    class DirectoryInfo : AppBase
    {
        public string TargetDirectoryPath { get; set; }
        public string[] Files { get; set; }
        public List<FIleInfo> FileInfos { get; set; }
    }
}
