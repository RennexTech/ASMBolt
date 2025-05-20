using ASMBolt.Assembler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMBolt.Assembler
{
    public enum AssemblerType
    {
        MASM,
        NASM,
        FASM, // Future support
        GAS   // Future support
    }

    public class AssemblerService
    {
        public async Task<AssemblerInfo> DetectAssemblers()
        {
            var masmInfo = await DetectAssembler("ml64.exe", "MASM"); // Assuming x64 MASM by default
            if (!masmInfo.Found)
            {
                masmInfo = await DetectAssembler("ml.exe", "MASM");   // Check for x86 MASM
            }

            var nasmInfo = await DetectAssembler("nasm.exe", "NASM");

            // Future: Add detection for FASM and GAS if needed

            return new AssemblerInfo
            {
                MASMPath = masmInfo.FilePath,
                MASMFound = masmInfo.Found,
                MASMVersion = masmInfo.Version,
                NASMPath = nasmInfo.FilePath,
                NASMFound = nasmInfo.Found,
                NASMVersion = nasmInfo.Version
                // Future: Add FASM and GAS info here
            };
        }

        private async Task<AssemblerInfo> DetectAssembler(string executableName, string assemblerName)
        {
            string pathFromEnvironment = FindExecutableInPath(executableName);
            if (!string.IsNullOrEmpty(pathFromEnvironment))
            {
                string version = await GetAssemblerVersion(pathFromEnvironment, assemblerName);
                return new AssemblerInfo(assemblerName, pathFromEnvironment, version, true);
            }

            // Future: Check known installation locations if not in PATH

            return new AssemblerInfo(assemblerName, null, null, false);
        }

        private string FindExecutableInPath(string executableName)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            if (path != null)
            {
                string[] paths = path.Split(Path.PathSeparator);
                foreach (string dir in paths)
                {
                    string fullPath = Path.Combine(dir, executableName);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
            }
            return null;
        }

        private async Task<string> GetAssemblerVersion(string executablePath, string assemblerName)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = executablePath,
                        Arguments = assemblerName.ToLower() == "masm" ? "/?" : "--version", // Adjust arguments as needed
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    // Basic version extraction - might need more robust parsing
                    if (assemblerName.ToLower() == "masm" && output.Contains("Microsoft (R) Macro Assembler"))
                    {
                        var versionLine = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
                        return versionLine.Split(' ')[5]; // Crude extraction, needs refinement
                    }
                    else if (assemblerName.ToLower() == "nasm" && output.ToLower().Contains("nasm"))
                    {
                        var versionLine = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0];
                        return versionLine.Split(' ')[2]; // Crude extraction, needs refinement
                    }
                    return "N/A";
                }
            }
            catch
            {
                return "N/A";
            }
        }

        public async Task<CompilationResult> CompileFile(string filePath, AssemblerType assemblerType, bool linkExecutable = true, string outputDirectory = null)
        {
            var result = new CompilationResult { Success = false };
            string outputFileName = Path.GetFileNameWithoutExtension(filePath);
            string objectFilePath = Path.Combine(outputDirectory ?? Path.GetDirectoryName(filePath), outputFileName + (assemblerType == AssemblerType.MASM ? ".obj" : ".o"));
            string executableFilePath = Path.Combine(outputDirectory ?? Path.GetDirectoryName(filePath), outputFileName + ".exe");

            try
            {
                using (var process = new Process())
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.CreateNoWindow = true;

                    if (assemblerType == AssemblerType.MASM)
                    {
                        string masmPath = await GetMasmPath();
                        if (string.IsNullOrEmpty(masmPath))
                        {
                            result.ErrorText = "MASM executable not found.";
                            return result;
                        }
                        startInfo.FileName = masmPath;
                        startInfo.Arguments = $"/c /coff \"{filePath}\""; // /c for assemble only, /coff for object format
                        if (linkExecutable)
                        {
                            startInfo.Arguments += $" /link /subsystem:console"; // Basic console subsystem linking
                            startInfo.Arguments += $" /out:\"{executableFilePath}\"";
                            result.OutputFilePath = executableFilePath;
                        }
                        else
                        {
                            result.OutputFilePath = objectFilePath;
                        }
                    }
                    else if (assemblerType == AssemblerType.NASM)
                    {
                        string nasmPath = await GetNasmPath();
                        if (string.IsNullOrEmpty(nasmPath))
                        {
                            result.ErrorText = "NASM executable not found.";
                            return result;
                        }
                        startInfo.FileName = nasmPath;
                        startInfo.Arguments = $"-f win64 -o \"{objectFilePath}\" \"{filePath}\""; // Assuming 64-bit Windows
                        if (linkExecutable)
                        {
                            // NASM doesn't link directly, typically uses a separate linker (like link.exe for MSVC)
                            // For simplicity in this initial version, we might skip auto-linking for NASM or provide a basic example.
                            // Let's just output the object file for now if linking is requested for NASM in this basic version.
                            result.OutputFilePath = executableFilePath; // Indicate where the exe *would* be
                            result.OutputText += "Note: Auto-linking for NASM is not implemented in this basic version. Object file created.";
                            linkExecutable = false; // Prevent further linking attempts in this basic version
                        }
                        else
                        {
                            result.OutputFilePath = objectFilePath;
                        }
                    }
                    else
                    {
                        result.ErrorText = $"Unsupported assembler type: {assemblerType}";
                        return result;
                    }

                    process.StartInfo = startInfo;
                    process.Start();

                    result.OutputText = await process.StandardOutput.ReadToEndAsync();
                    result.ErrorText = await process.StandardError.ReadToEndAsync();

                    await process.WaitForExitAsync();
                    result.Success = process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorText = $"An error occurred during compilation: {ex.Message}";
            }

            return result;
        }

        private async Task<string> GetMasmPath()
        {
            var info = await DetectAssemblers();
            return info.MASMPath;
        }

        private async Task<string> GetNasmPath()
        {
            var info = await DetectAssemblers();
            return info.NASMPath;
        }
    }
}
