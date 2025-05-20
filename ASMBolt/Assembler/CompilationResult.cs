using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Assembler
{
    public class CompilationResult
    {
        public bool Success { get; set; }
        public string OutputText { get; set; }
        public string ErrorText { get; set; }
        public string OutputFilePath { get; set; }
    }
}
