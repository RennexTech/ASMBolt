using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Assembler
{
    public class AssemblerInfo
    {
        public string Name { get; }
        public string FilePath { get; }
        public string Version { get; }
        public bool Found { get; }

        // Specific properties for MASM and NASM for easier access
        public string MASMPath { get; internal set; }
        public bool MASMFound { get; internal set; }
        public string MASMVersion { get; internal set; }

        public string NASMPath { get; internal set; }
        public bool NASMFound { get; internal set; }
        public string NASMVersion { get; internal set; }

        public AssemblerInfo(string name, string filePath, string version, bool found)
        {
            Name = name;
            FilePath = filePath;
            Version = version;
            Found = found;
        }

        // Default constructor for the DetectAssemblers return type
        public AssemblerInfo() { }
    }
}