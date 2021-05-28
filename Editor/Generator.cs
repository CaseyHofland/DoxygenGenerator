using System.IO;
using System.Text;
using System.Threading;

using Debug = UnityEngine.Debug;

namespace DoxygenGenerator
{
    public static class Generator
    {
        private const string filesPath = "Packages/com.CaseyDeCoder.doxygengenerator/Editor/Files~";

        public static Thread GenerateAsync()
        {
            // Get settings (I find it easier to read this way)
            var doxygenPath = GeneratorSettings.doxygenPath;
            var inputDirectory = GeneratorSettings.inputDirectory;
            var outputDirectory = GeneratorSettings.outputDirectory;
            var project = GeneratorSettings.project;
            var synopsis = GeneratorSettings.synopsis;
            var version = GeneratorSettings.version;

            // Add the Doxyfile
            var doxyFileSource = $"{filesPath}/Doxyfile";
            var doxyFileDestination = $"{outputDirectory}/Doxyfile";
            File.Copy(doxyFileSource, doxyFileDestination, true);
            
            // Add doxygen-awesome
            Directory.CreateDirectory($"{outputDirectory}/html");

            var doxygenAwesomeSource = $"{filesPath}/doxygen-awesome.css";
            var doxygenAwesomeDestination = $"{outputDirectory}/html/doxygen-awesome.css";
            File.Copy(doxygenAwesomeSource, doxygenAwesomeDestination, true);

            var doxygenAwesomeSidebarOnlySource = $"{filesPath}/doxygen-awesome-sidebar-only.css";
            var doxygenAwesomeSidebarOnlyDestination = $"{outputDirectory}/html/doxygen-awesome-sidebar-only.css";
            File.Copy(doxygenAwesomeSidebarOnlySource, doxygenAwesomeSidebarOnlyDestination, true);

            // Update Doxyfile parameters
            var doxyFileText = File.ReadAllText(doxyFileDestination);
            var doxyFileStringBuilder = new StringBuilder(doxyFileText);

            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_NAME           =", $"PROJECT_NAME           = \"{project}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_BRIEF          =", $"PROJECT_BRIEF          = \"{synopsis}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_NUMBER         =", $"PROJECT_NUMBER         = {version}");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("INPUT                  =", $"INPUT                  = \"{inputDirectory}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("OUTPUT_DIRECTORY       =", $"OUTPUT_DIRECTORY       = \"{outputDirectory}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("HTML_EXTRA_STYLESHEET  =", $"HTML_EXTRA_STYLESHEET  = \"{doxygenAwesomeDestination}\" \"{doxygenAwesomeSidebarOnlyDestination}\"");

            doxyFileText = doxyFileStringBuilder.ToString();
            File.WriteAllText(doxyFileDestination, doxyFileText);

            // Run doxygen on a new thread
            var doxygenOutput = new DoxygenThreadSafeOutput();
            doxygenOutput.SetStarted();
            var doxygen = new DoxygenRunner(doxygenPath, new[] { doxyFileDestination }, doxygenOutput, OnDoxygenFinished);
            var doxygenThread = new Thread(new ThreadStart(doxygen.RunThreadedDoxy));
            doxygenThread.Start();

            return doxygenThread;

            void OnDoxygenFinished(int code)
            {
                if (code != 0)
                {
                    Debug.LogError($"Doxygen finsished with Error: return code {code}. Check the Doxgen Log for Errors and try regenerating your Doxyfile.");
                }

                // Read doxygen-awesome since the files are destroyed in the doxygen process
                File.Copy(doxygenAwesomeSource, doxygenAwesomeDestination, true);
                File.Copy(doxygenAwesomeSidebarOnlySource, doxygenAwesomeSidebarOnlyDestination, true);

                // Create a doxygen log file
                var doxygenLog = doxygenOutput.ReadFullLog();
                var doxygenLogDestination = $"{outputDirectory}/Log.txt";
                if (File.Exists(doxygenLogDestination))
                {
                    File.Delete(doxygenLogDestination);
                }

                File.WriteAllLines(doxygenLogDestination, doxygenLog);
            }
        }
    }
}
