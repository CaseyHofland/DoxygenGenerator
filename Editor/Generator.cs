using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using Debug = UnityEngine.Debug;

namespace DoxygenGenerator
{
    public static class Generator
    {
        private const string filesPath = "Packages/com.CaseyDeCoder.doxygengenerator/Editor/Files~";
        private const string collabPath = "Assets/Plugins/DoxygenGenerator/Editor/Files~";

        public static Thread GenerateAsync()
        {
            // Get settings (I find it easier to read this way)
            var doxygenPath = GeneratorSettings.doxygenPath;
            var inputDirectory = GeneratorSettings.inputDirectory;
            var outputDirectory = GeneratorSettings.outputDirectory;
            var project = GeneratorSettings.project;
            var synopsis = GeneratorSettings.synopsis;
            var version = GeneratorSettings.version;
            var useMarkdown = GeneratorSettings.o_MarkdownSupport;//Wether or not to add markdown support
            var useAlnumSorting = GeneratorSettings.o_AlNumSorting;//Should use alnum sorting for all members in classes
            var showReferencesRelation = GeneratorSettings.o_ShowReferencesRelation;//Should show references relation
            var showReferencedByRelation = GeneratorSettings.o_ShowReferencedByRelation;//Should show referenced by relation
            var showUsedFiles = GeneratorSettings.o_ShowUsedFiles;
            var showFiles = GeneratorSettings.o_ShowFiles;
            var showNamespaces = GeneratorSettings.o_ShowNamespaces;
            var hideScopeNames = GeneratorSettings.o_HideScopeNames;
            var hideCompoundReference = GeneratorSettings.o_HideCompoundRefs;
            bool collabMode = !File.Exists(filesPath);//For those that want to collab like Dencho
            // Add the Doxyfile
            var doxyPath = (collabMode ? collabPath : filesPath);
            var doxyFileSource = $"{doxyPath}/Doxyfile";
            var doxyFileDestination = $"{outputDirectory}/Doxyfile";
            File.Copy(doxyFileSource, doxyFileDestination, true);
            // Add doxygen-awesome
            Directory.CreateDirectory($"{outputDirectory}/html");
            //1 - JS Files
            var doxygenJSSource_DarkModeToggle = $"{doxyPath}/doxygen-awesome-darkmode-toggle.js";
            var doxygenJSDestination_DarkModeToggle = $"{outputDirectory}/html/doxygen-awesome-darkmode-toggle.js";
            File.Copy(doxygenJSSource_DarkModeToggle, doxygenJSDestination_DarkModeToggle, true);
            //2
            var doxygenJSSource_FragmentCopy = $"{doxyPath}/doxygen-awesome-fragment-copy-button.js";
            var doxygenJSDesination_FragmentCopy = $"{outputDirectory}/html/doxygen-awesome-fragment-copy-button.js";
            File.Copy(doxygenJSSource_FragmentCopy, doxygenJSDesination_FragmentCopy, true);
            //3
            var doxygenJSSource_ParagraphLink = $"{doxyPath}/doxygen-awesome-paragraph-link.js";
            var doxygenJSDesination_ParagraphLink = $"{outputDirectory}/html/doxygen-awesome-paragraph-link.js";
            File.Copy(doxygenJSSource_ParagraphLink, doxygenJSDesination_ParagraphLink, true);
            //4
            var doxygenJSSource_InteractiveToC = $"{doxyPath}/doxygen-awesome-interactive-toc.js";
            var doxygenJSDesination_InteractiveToC = $"{outputDirectory}/html/doxygen-awesome-interactive-toc.js";
            File.Copy(doxygenJSSource_InteractiveToC, doxygenJSDesination_InteractiveToC, true);
            //5
            var doxygenJSSource_Tabs = $"{doxyPath}/doxygen-awesome-tabs.js";
            var doxygenJSDesination_Tabs = $"{outputDirectory}/html/doxygen-awesome-tabs.js";
            File.Copy(doxygenJSSource_Tabs, doxygenJSDesination_Tabs, true);
            //1 - CSS FIles
            var doxygenCSSSource_Awesome = $"{doxyPath}/doxygen-awesome.css";
            var doxygenCSSDestination_Awesome = $"{outputDirectory}/html/doxygen-awesome.css";
            File.Copy(doxygenCSSSource_Awesome, doxygenCSSDestination_Awesome, true);
            //2
            var doxygenCSSSource_Custom = $"{doxyPath}/doxygen-custom.css";
            var doxygenCSSDestination_Custom = $"{outputDirectory}/html/doxygen-custom.css";
            File.Copy(doxygenCSSSource_Custom, doxygenCSSDestination_Custom, true);
            //3
            var doxygenCSSSource_SidebarOnly = $"{doxyPath}/doxygen-awesome-sidebar-only.css";
            var doxygenCSSDestination_SidebarOnly = $"{outputDirectory}/html/doxygen-awesome-sidebar-only.css";
            File.Copy(doxygenCSSSource_SidebarOnly, doxygenCSSDestination_SidebarOnly, true);
            //4
            var doxygenCSSSource_SideBarOnlyDarkModeToggle = $"{doxyPath}/doxygen-awesome-sidebar-only-darkmode-toggle.css";
            var doxygenCSSDestination_SideBarOnlyDarkModeToggle = $"{outputDirectory}/html/doxygen-awesome-sidebar-only-darkmode-toggle.css";
            File.Copy(doxygenCSSSource_SideBarOnlyDarkModeToggle, doxygenCSSDestination_SideBarOnlyDarkModeToggle, true);
            //Header
            var customHeaderSource = $"{doxyPath}/header.html";
            var customHeaderDestination = $"{outputDirectory}/html/header.html";
            File.Copy(customHeaderSource, customHeaderDestination, true);
            
            // Update Doxyfile parameters
            var doxyFileText = File.ReadAllText(doxyFileDestination);
            var doxyFileStringBuilder = new StringBuilder(doxyFileText);
            //Modify the Base Boxyfile like normal
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_NAME           =", $"PROJECT_NAME           = \"{project}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_BRIEF          =", $"PROJECT_BRIEF          = \"{synopsis}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("PROJECT_NUMBER         =", $"PROJECT_NUMBER         = {version}");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("INPUT                  =", $"INPUT                  = \"{inputDirectory}\"");
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("OUTPUT_DIRECTORY       =", $"OUTPUT_DIRECTORY       = \"{outputDirectory}\"");
            //Custom Header
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("HTML_HEADER            =",string.Format("HTML_HEADER            = {0}", customHeaderDestination));
            //JS string
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("HTML_EXTRA_FILES       = ",
                string.Format("HTML_EXTRA_FILES       = {0} {1} {2} {3} {4}", 
                doxygenJSDestination_DarkModeToggle,
                doxygenJSDesination_FragmentCopy,
                doxygenJSDesination_ParagraphLink,
                doxygenJSDesination_InteractiveToC,
                doxygenJSDesination_Tabs
                ));
            //CSS string
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("HTML_EXTRA_STYLESHEET  = ",
                string.Format("HTML_EXTRA_STYLESHEET  = {0} {1} {2} {3}",
                doxygenCSSDestination_Awesome,
                doxygenCSSDestination_Custom,
                doxygenCSSDestination_SidebarOnly,
                doxygenCSSDestination_SideBarOnlyDarkModeToggle
                ));
            //Update Supported File Formats
            var existingFilePatterns = ExtractFilePatterns(doxyFileText);
            doxyFileStringBuilder = doxyFileStringBuilder.Replace(existingFilePatterns, ConstructFileFormatString(existingFilePatterns, "*.md", useMarkdown));
            //Apply options to Doxyfile
            doxyFileStringBuilder.AppendLine(string.Format("MARKDOWN_SUPPORT       = {0}", useMarkdown?"YES":"NO"));
            doxyFileStringBuilder = doxyFileStringBuilder.Replace("SORT_MEMBER_DOCS       = YES", $"SORT_MEMBER_DOCS       = {string.Format("{0}",useAlnumSorting?"YES":"NO")}");//To sort Members by alnum
            if(showReferencedByRelation)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("REFERENCED_BY_RELATION = NO", $"REFERENCED_BY_RELATION = {string.Format("{0}","YES")}");//
            if(showReferencesRelation)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("REFERENCES_RELATION    = NO", $"REFERENCES_RELATION    = {string.Format("{0}","YES")}");//
            if(!showUsedFiles)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("SHOW_USED_FILES        = YES", $"SHOW_USED_FILES        = {string.Format("{0}","NO")}");//
            if(!showFiles)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("SHOW_FILES             = YES", $"SHOW_FILES             = {string.Format("{0}","NO")}");//
            if(!showNamespaces)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("SHOW_NAMESPACES        = YES", $"SHOW_NAMESPACES        = {string.Format("{0}","NO")}");//
            if(hideScopeNames)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("HIDE_SCOPE_NAMES       = NO", $"HIDE_SCOPE_NAMES       = {string.Format("{0}","YES")}");//
            if(hideCompoundReference)
                doxyFileStringBuilder = doxyFileStringBuilder.Replace("HIDE_COMPOUND_REFERENCE= NO", $"HIDE_COMPOUND_REFERENCE= {string.Format("{0}","YES")}");//

            doxyFileText = doxyFileStringBuilder.ToString();
            File.WriteAllText(doxyFileDestination, doxyFileText);

            // Run doxygen on a new thread
            var doxygenOutput = new DoxygenThreadSafeOutput();
            doxygenOutput.SetStarted();
            var args =  new string[] { doxyFileDestination };//var args =  new string[] { "-u", "-w", "html", "header.html", "delete_me.html", "delete_me.css", doxyFileDestination };
            var doxygen = new DoxygenRunner(doxygenPath, args, doxygenOutput, OnDoxygenFinished);
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
                File.Copy(doxygenCSSSource_Awesome, doxygenCSSDestination_Awesome, true);
                File.Copy(doxygenCSSSource_SidebarOnly, doxygenCSSDestination_SidebarOnly, true);
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



        static string ExtractFilePatterns(string configString)
        {
            StringBuilder filePatternsBuilder = new StringBuilder();
            bool filePatternsSection = false;

            // Split the configuration string into lines
            string[] lines = configString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Check for the start of the FILE_PATTERNS section
                if (line.TrimStart().StartsWith("FILE_PATTERNS"))
                {
                    filePatternsSection = true;
                    filePatternsBuilder.AppendLine(line.TrimEnd());
                    continue;
                }

                // If in the FILE_PATTERNS section and the line contains a file pattern, append it
                if (filePatternsSection && line.Trim().Contains("*"))
                {
                    filePatternsBuilder.AppendLine(line.TrimEnd());
                }
                // If the line does not contain a file pattern, end the section
                else if (filePatternsSection)
                {
                    break;
                }
            }

            return filePatternsBuilder.ToString();
        }

        static string ConstructFileFormatString(string existingFormats, string fileType, bool supported)
        {
            // Split the string into lines and then into file formats
            var formats = new HashSet<string>(existingFormats.Split(new[] { ' ', '\n', '\\', '\r' }, StringSplitOptions.RemoveEmptyEntries));

            // Add or remove the fileType based on the supported flag
            if (supported)
            {
                formats.Add(fileType);
            }
            else
            {
                formats.Remove(fileType);
            }

            // Reconstruct the string
            StringBuilder sb = new StringBuilder();
            sb.Append("FILE_PATTERNS          = ");
            foreach (var format in formats)
            {
                sb.Append(format).Append(" \\\n                         ");
            }

            // Remove the last trailing characters
            if (formats.Count > 0)
            {
                sb.Length -= 27; // Length of " \\\n                         "
            }

            return sb.ToString();
        }
    }
}
