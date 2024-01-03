using UnityEditor;

namespace DoxygenGenerator
{
    public static class GeneratorSettings
    {
        public static string doxygenPath
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(doxygenPath)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(doxygenPath)}", value);
        }

        public static string inputDirectory
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(inputDirectory)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(inputDirectory)}", value);
        }

        public static string outputDirectory
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(outputDirectory)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(outputDirectory)}", value);
        }

        public static string project
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(project)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(project)}", value);
        }

        public static string synopsis
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(synopsis)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(synopsis)}", value);
        }

        public static string version
        {
            get => EditorPrefs.GetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(version)}", string.Empty);
            set => EditorPrefs.SetString($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(version)}", value);
        }

        public static bool o_MarkdownSupport
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_MarkdownSupport)}", true);
            set=> EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_MarkdownSupport)}", value);
        }

        public static bool o_AlNumSorting
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_AlNumSorting)}", false);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_AlNumSorting)}", value);
        }
        
        public static bool o_ShowReferencesRelation
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowReferencesRelation)}", false);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowReferencesRelation)}", value);
        }

        public static bool o_ShowReferencedByRelation
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowReferencedByRelation)}", false);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowReferencedByRelation)}", value);
        }
        
        public static bool o_ShowUsedFiles
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowUsedFiles)}", true);
            set=> EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowUsedFiles)}", value);
        }

        public static bool o_ShowFiles
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowFiles)}", true);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowFiles)}", value);
        }

        public static bool o_ShowNamespaces
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowNamespaces)}", true);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_ShowNamespaces)}", value);
        }

        public static bool o_HideScopeNames
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_HideScopeNames)}", false);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_HideScopeNames)}", value);
        }

        public static bool o_HideCompoundRefs
        {
            get => EditorPrefs.GetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_HideCompoundRefs)}", false);
            set => EditorPrefs.SetBool($"{nameof(DoxygenGenerator)}.{nameof(GeneratorSettings)}.{nameof(o_HideCompoundRefs)}", value);
        }
    }
}
