using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;

namespace DoxygenGenerator
{
    /// <summary>
    ///  This class spawns and runs Doxygen in a separate thread, and could serve as an example of how to create 
    ///  plugins for unity that call a command line application and then get the data back into Unity safely.	 
    /// </summary>
    public class DoxygenRunner
    {
        DoxygenThreadSafeOutput SafeOutput;
        public Action<int> onCompleteCallBack;
        List<string> DoxyLog = new List<string>();
        public string EXE = null;
        public string[] Args;
        static string WorkingFolder;

        public DoxygenRunner(string exepath, string[] args, DoxygenThreadSafeOutput safeoutput, Action<int> callback)
        {
            EXE = exepath;
            Args = args;
            SafeOutput = safeoutput;
            onCompleteCallBack = callback;
            WorkingFolder = FileUtil.GetUniqueTempPathInProject();
            Directory.CreateDirectory(WorkingFolder);
        }

        public void updateOuputString(string output)
        {
            SafeOutput.WriteLine(output);
            DoxyLog.Add(output);
        }

        public void RunThreadedDoxy()
        {
            int ReturnCode = Run(updateOuputString, null, EXE, Args);
            SafeOutput.WriteFullLog(DoxyLog);
            SafeOutput.SetFinished();
            onCompleteCallBack(ReturnCode);
        }

        /// <summary>
        /// Runs the specified executable with the provided arguments and returns the process' exit code.
        /// </summary>
        /// <param name="output">Recieves the output of either std/err or std/out</param>
        /// <param name="input">Provides the line-by-line input that will be written to std/in, null for empty</param>
        /// <param name="exe">The executable to run, may be unqualified or contain environment variables</param>
        /// <param name="args">The list of unescaped arguments to provide to the executable</param>
        /// <returns>Returns process' exit code after the program exits</returns>
        /// <exception cref="FileNotFoundException">Raised when the exe was not found</exception>
        /// <exception cref="ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'
        public static int Run(Action<string> output, TextReader input, string exe, params string[] args)
        {
            if (String.IsNullOrEmpty(exe))
                throw new FileNotFoundException();
            if (output == null)
                throw new ArgumentNullException("output");

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            psi.ErrorDialog = false;
            psi.WorkingDirectory = WorkingFolder;
            psi.FileName = FindExePath(exe);
            psi.Arguments = EscapeArguments(args);

            using (Process process = Process.Start(psi))
            using (ManualResetEvent mreOut = new ManualResetEvent(false),
                   mreErr = new ManualResetEvent(false))
            {
                process.OutputDataReceived += (o, e) => { if (e.Data == null) mreOut.Set(); else output(e.Data); };
                process.BeginOutputReadLine();
                process.ErrorDataReceived += (o, e) => { if (e.Data == null) mreErr.Set(); else output(e.Data); };
                process.BeginErrorReadLine();

                string line;
                while (input != null && null != (line = input.ReadLine()))
                {
                    process.StandardInput.WriteLine(line);
                }

                process.StandardInput.Close();
                process.WaitForExit();

                mreOut.WaitOne();
                mreErr.WaitOne();

                return process.ExitCode;
            }
        }

        /// <summary>
        /// Quotes all arguments that contain whitespace, or begin with a quote and returns a single
        /// argument string for use with Process.Start().
        /// </summary>
        /// <param name="args">A list of strings for arguments, may not contain null, '\0', '\r', or '\n'</param>
        /// <returns>The combined list of escaped/quoted strings</returns>
        /// <exception cref="ArgumentNullException">Raised when one of the arguments is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Raised if an argument contains '\0', '\r', or '\n'</exception>
        public static string EscapeArguments(params string[] args)
        {
            StringBuilder arguments = new StringBuilder();
            Regex invalidChar = new Regex("[\x00\x0a\x0d]");//  these can not be escaped
            Regex needsQuotes = new Regex(@"\s|""");//          contains whitespace or two quote characters
            Regex escapeQuote = new Regex(@"(\\*)(""|$)");//    one or more '\' followed with a quote or end of string
            for (int carg = 0; args != null && carg < args.Length; carg++)
            {
                if (args[carg] == null)
                {
                    throw new ArgumentNullException("args[" + carg + "]");
                }
                if (invalidChar.IsMatch(args[carg]))
                {
                    throw new ArgumentOutOfRangeException("args[" + carg + "]");
                }
                if (args[carg] == String.Empty)
                {
                    arguments.Append("\"\"");
                }
                else if (!needsQuotes.IsMatch(args[carg]))
                {
                    arguments.Append(args[carg]);
                }
                else
                {
                    arguments.Append('"');
                    arguments.Append(escapeQuote.Replace(args[carg], m =>
                                                         m.Groups[1].Value + m.Groups[1].Value +
                                                         (m.Groups[2].Value == "\"" ? "\\\"" : "")
                                                        ));
                    arguments.Append('"');
                }
                if (carg + 1 < args.Length)
                    arguments.Append(' ');
            }
            return arguments.ToString();
        }


        /// <summary>
        /// Expands environment variables and, if unqualified, locates the exe in the working directory
        /// or the evironment's path.
        /// </summary>
        /// <param name="exe">The name of the executable file</param>
        /// <returns>The fully-qualified path to the file</returns>
        /// <exception cref="FileNotFoundException">Raised when the exe was not found</exception>
        public static string FindExePath(string exe)
        {
            exe = Environment.ExpandEnvironmentVariables(exe);
            if (!File.Exists(exe))
            {
                if (Path.GetDirectoryName(exe) == String.Empty)
                {
                    foreach (string test in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
                    {
                        string path = test.Trim();
                        if (!String.IsNullOrEmpty(path) && File.Exists(path = Path.Combine(path, exe)))
                            return Path.GetFullPath(path);
                    }
                }
                throw new FileNotFoundException(new FileNotFoundException().Message, exe);
            }
            return Path.GetFullPath(exe);
        }
    }
}
