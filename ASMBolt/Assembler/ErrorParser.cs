using ASMBolt.Assembler;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASMBolt.Assembler
{
    public class ErrorParser
    {
        public List<AssemblerError> ParseErrors(string errorLog, AssemblerType assemblerType, string filePath)
        {
            var errors = new List<AssemblerError>();

            if (string.IsNullOrEmpty(errorLog))
            {
                return errors;
            }

            string[] errorLines = errorLog.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (assemblerType == AssemblerType.MASM)
            {
                // Example MASM error format: filename(linenumber) : error ErrorCode: Error message
                // Example: mycode.asm(10) : error A2004: symbol must be defined
                var masmErrorRegex = new Regex(@"^(.*?)\((\d+)\)\s*:\s*error\s+([A-Z]\d+):\s*(.*)$");
                foreach (string line in errorLines)
                {
                    var match = masmErrorRegex.Match(line);
                    if (match.Success)
                    {
                        if (string.IsNullOrEmpty(filePath) || match.Groups[1].Value.ToLower() == Path.GetFileName(filePath).ToLower())
                        {
                            errors.Add(new AssemblerError(match.Groups[1].Value, int.Parse(match.Groups[2].Value), match.Groups[3].Value, match.Groups[4].Value));
                        }
                    }
                }
            }
            else if (assemblerType == AssemblerType.NASM)
            {
                // Example NASM error format: filename:linenumber: error: Error message
                // Example: mycode.asm:10: error: symbol `undefined_symbol' not defined
                var nasmErrorRegex = new Regex(@"^(.*?):(\d+):\s*error:\s*(.*)$");
                foreach (string line in errorLines)
                {
                    var match = nasmErrorRegex.Match(line);
                    if (match.Success)
                    {
                        if (string.IsNullOrEmpty(filePath) || match.Groups[1].Value.ToLower() == Path.GetFileName(filePath).ToLower())
                        {
                            errors.Add(new AssemblerError(match.Groups[1].Value, int.Parse(match.Groups[2].Value), "NASM", match.Groups[3].Value));
                        }
                    }
                }
            }

            return errors;
        }
    }

    public class AssemblerError
    {
        public string File { get; }
        public int LineNumber { get; }
        public string Code { get; } // Or source of the error (e.g., "MASM", "NASM")
        public string Message { get; }

        public AssemblerError(string file, int lineNumber, string code, string message)
        {
            File = file;
            LineNumber = lineNumber;
            Code = code;
            Message = message;
        }
    }
}
