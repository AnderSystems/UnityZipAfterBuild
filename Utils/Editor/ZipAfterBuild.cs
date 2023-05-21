using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using System.IO;
using System.Linq;

public class ZipAfterBuild
{
    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        //ToastyNotifications.DisplayNotification("Test", "Test");
        //return;
        //if (!IsPackageInstalled("com.unity.services.push-notifications"))
        //{
        //    if (EditorUtility.DisplayDialog("Install push notifications?", "Push notifications not installed\nDo you want install to enable build finish notification?", "Isntall", "Cancel"))
        //    {
        //        Client.Add("com.unity.services.push-notifications");
        //    }
        //}
    }

    public static bool IsPackageInstalled(string packageId)
    {
        if (!File.Exists("Packages/manifest.json"))
            return false;

        string jsonText = File.ReadAllText("Packages/manifest.json");
        return jsonText.Contains(packageId);
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string path = pathToBuiltProject.Replace("index.html", "");
        string[] splitPath = path.Split(@"/");
        string projectName = splitPath[splitPath.Length-1];

        int dialogResult = EditorUtility.DisplayDialogComplex($"Build {System.DateTime.Now} completed!", $"Your build has been successfully completed and saved in '{pathToBuiltProject}'.",
            "Show in explorer", "Close", "Zip Project");

        //0 = Show in explorer,  Close = 1, Zip = 2

        if (dialogResult == 0)
        {
            //EditorUtility.RevealInFinder(pathToBuiltProject);
        }

        if (dialogResult == 2)
        {
            string FinalPath = path + "/" + projectName + ".zip";
            ZipFile.Zip(path, FinalPath);
            EditorUtility.RevealInFinder(FinalPath);
            //ShowExplorer(pathToBuiltProject);
        }

        Debug.Log(dialogResult);

        //Debug.Log(pathToBuiltProject);
    }
}
