using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace DoxygenGenerator
{
    public class GeneratorWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private Thread doxygenThread;
        private string generateButtonName = "Generate";

        private bool canGenerate => File.Exists(doxygenPath)
            && Directory.Exists(inputDirectory)
            && Directory.Exists(outputDirectory)
            && doxygenThread == null;

        #region Settings
        private string doxygenPath
        {
            get => GeneratorSettings.doxygenPath;
            set => GeneratorSettings.doxygenPath = value;
        }

        private string inputDirectory
        {
            get => GeneratorSettings.inputDirectory;
            set => GeneratorSettings.inputDirectory = value;
        }

        private string outputDirectory
        {
            get => GeneratorSettings.outputDirectory;
            set => GeneratorSettings.outputDirectory = value;
        }

        private string project
        {
            get => GeneratorSettings.project;
            set => GeneratorSettings.project = value;
        }

        private string synopsis
        {
            get => GeneratorSettings.synopsis;
            set => GeneratorSettings.synopsis = value;
        }

        private string version
        {
            get => GeneratorSettings.version;
            set => GeneratorSettings.version = value;
        }
        #endregion

        [MenuItem("Window/Doxygen Generator")]
        public static void Initialize()
        {
            var window = GetWindow<GeneratorWindow>("Doxygen Generator");
            window.minSize = new Vector2(420, 245);
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Select your doxygen install location
            DoxygenInstallPathGUI();

            // Setup the directories
            SetupTheDirectoriesGUI();

            // Set your project settings
            ProjectSettingsGUI();

            // Generate the API
            DocumentationGUI();

            EditorGUILayout.EndScrollView();
        }

        private void DoxygenInstallPathGUI()
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUILayout.Label("Doxygen Install Path", EditorStyles.boldLabel);

            // Doxygen not selected error
            if (!File.Exists(doxygenPath))
            {
                doxygenPath = default;
                EditorGUILayout.HelpBox("No doxygen install path is selected. Please install Doxygen and select it below.", MessageType.Error, true);
                if (GUILayout.Button("Download Doxygen", GUILayout.MaxWidth(150)))
                {
                    Application.OpenURL("https://www.doxygen.nl/download.html");
                }
            }

            // Doxygen Path
            EditorGUILayout.BeginHorizontal();
            doxygenPath = EditorGUILayout.DelayedTextField("doxygen.exe", doxygenPath);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                doxygenPath = EditorUtility.OpenFilePanel("Select your doxygen.exe", string.Empty, string.Empty);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SetupTheDirectoriesGUI()
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUILayout.Label("Setup the Directories", EditorStyles.boldLabel);

            // Input not selected error
            if (!Directory.Exists(inputDirectory))
            {
                inputDirectory = default;
                EditorGUILayout.HelpBox("No input directory selected. Please select a directory you would like your API to be generated from.", MessageType.Error, true);
            }

            // Input Directory
            EditorGUILayout.BeginHorizontal();
            inputDirectory = EditorGUILayout.DelayedTextField("Input Directory", inputDirectory);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                inputDirectory = EditorUtility.OpenFolderPanel("Select your Input Directory", string.Empty, string.Empty);
            }
            EditorGUILayout.EndHorizontal();

            // Output not selected error
            if (!Directory.Exists(outputDirectory))
            {
                outputDirectory = default;
                EditorGUILayout.HelpBox("No output directory selected. Please select a directory you would like your API to be generated to.", MessageType.Error, true);
            }

            // Output Directory
            EditorGUILayout.BeginHorizontal();
            outputDirectory = EditorGUILayout.DelayedTextField("Output Directory", outputDirectory);
            if (GUILayout.Button("...", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                outputDirectory = EditorUtility.OpenFolderPanel("Select your Output Directory", string.Empty, string.Empty);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ProjectSettingsGUI()
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUILayout.Label("Project Settings", EditorStyles.boldLabel);
            project = EditorGUILayout.TextField("Name", project);
            synopsis = EditorGUILayout.TextField("Synopsis", synopsis);
            version = EditorGUILayout.TextField("Version", version);
        }

        private void DocumentationGUI()
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            GUILayout.Label("Documentation", EditorStyles.boldLabel);

            // Update the doxygen thread
            if(doxygenThread != null)
            {
                switch (doxygenThread.ThreadState)
                {
                    case ThreadState.Aborted:
                    case ThreadState.Stopped:
                        doxygenThread = null;
                        generateButtonName = "Generate";
                        break;
                }
            }

            // Generate Button
            EditorGUI.BeginDisabledGroup(!canGenerate);
            if (GUILayout.Button(generateButtonName, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                doxygenThread = Generator.GenerateAsync();
                generateButtonName = "Generating...";
            }
            EditorGUI.EndDisabledGroup();

            // Open Button
            EditorGUI.BeginDisabledGroup(!Directory.Exists(outputDirectory) || doxygenThread != null);
            if (GUILayout.Button("Open", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                System.Diagnostics.Process.Start(outputDirectory);
            }
            EditorGUI.EndDisabledGroup();

            // View Log Button
            var logPath = $"{outputDirectory}/Log.txt";
            EditorGUI.BeginDisabledGroup(!File.Exists(logPath) || doxygenThread != null);
            if (GUILayout.Button("View Log", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL($"File://{logPath}");
            }
            EditorGUI.EndDisabledGroup();

            // Browse Button
            var browsePath = $"{outputDirectory}/html/annotated.html";
            EditorGUI.BeginDisabledGroup(!File.Exists(browsePath) || doxygenThread != null);
            if (GUILayout.Button("Browse", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                Application.OpenURL($"File://{browsePath}");
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
