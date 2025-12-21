using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PostProcessBuildiOS
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        // ファイル共有を有効にする
        plist.root.SetBoolean("UIFileSharingEnabled", true);
        // ドキュメントをその場で開く設定を有効にする
        plist.root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);

        File.WriteAllText(plistPath, plist.ToString());
    }
}