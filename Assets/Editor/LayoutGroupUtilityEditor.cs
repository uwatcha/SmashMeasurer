using UnityEditor;
using UnityEngine;

// 対象とするスクリプトを指定
[CustomEditor(typeof(LayoutGroupUtility))]
public class LayoutGroupUtilityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 元のインスペクター表示 (通常は何も表示されない)
        DrawDefaultInspector();

        // ターゲットスクリプトの参照を取得
        LayoutGroupUtility utility = (LayoutGroupUtility)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("この機能は、Layout Groupが計算した現在の子要素の位置・サイズをCanvas上の見た目そのままに固定し、階層から外します。", MessageType.Info);
        EditorGUILayout.Space(5);

        // ボタンの表示
        if (GUILayout.Button("現在の位置を固定してLayout Groupから外す (Detach)", GUILayout.Height(30)))
        {
            // Undoの記録 (安全のため)
            Undo.RecordObject(utility.gameObject, "Detach Children from Layout Group");

            // 処理実行
            utility.DetachChildrenFromLayoutGroup();
        }
    }
}