using System.IO;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

namespace Fumiki.Editor.BuildExtension
{
    public static class GameFrameworkConfigs
    {
        [BuildSettingsConfigPath]
        public static string BuildSettingsConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/BuildSettings.xml"));

        [ResourceCollectionConfigPath]
        public static string ResourceCollectionConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceCollection.xml"));

        [ResourceEditorConfigPath]
        public static string ResourceEditorConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceEditor.xml"));

        [ResourceBuilderConfigPath]
        public static string ResourceBuilderConfig = GameFramework.Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "Res/Configs/ResourceBuilder.xml"));
    }
}

