#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class PostBuildProcessor : MonoBehaviour
{
    public static void OnPostprocessBuildiOS (string exportPath)
    {
    }

	[PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);
        string target = proj.TargetGuidByName("Unity-iPhone");

        proj.AddFrameworkToProject(target, "AssetsLibrary.framework", false);
        File.WriteAllText(projPath, proj.WriteToString());
    }
}
#endif