using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Layout Groupの子要素を現在の位置に固定し、階層から外すユーティリティ
/// </summary>
[RequireComponent(typeof(LayoutGroup))]
public class LayoutGroupUtility : MonoBehaviour
{
    // このスクリプトはRuntimeでも使用できますが、今回はEditor拡張で呼び出します。

    /// <summary>
    /// Layout Groupの子要素を全て、現在のワールド座標の位置を維持したまま、
    /// Layout Groupの親 (通常はGrand Parent) の階層に移動します。
    /// </summary>
    public void DetachChildrenFromLayoutGroup()
    {
        // 1. Layout Groupコンポーネントを取得
        LayoutGroup layoutGroup = GetComponent<LayoutGroup>();
        if (layoutGroup == null)
        {
            Debug.LogError("LayoutGroupコンポーネントが見つかりません。", this);
            return;
        }

        // 2. 移動先の親Transformを決定（通常はLayout Groupの親）
        Transform newParent = layoutGroup.transform.parent;
        if (newParent == null)
        {
            Debug.LogError("Layout Groupの親が存在しないため、移動できません。");
            return;
        }

        // 3. 子要素のRectTransformのリストを作成
        // 実行中に階層を変更するため、事前にリスト化が必要です。
        List<RectTransform> childrenToDetach = new List<RectTransform>();
        
        // Transform.childCount/GetChild(i) を使用して子を取得
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            // UIオブジェクトの子は RectTransform である必要があります
            RectTransform childRect = layoutGroup.transform.GetChild(i) as RectTransform;
            if (childRect != null)
            {
                childrenToDetach.Add(childRect);
            }
        }

        // 4. 各子要素を移動
        foreach (RectTransform childRect in childrenToDetach)
        {
            // SetParent(新しい親, ワールド座標を維持するかどうか)
            // worldPositionStays: true が重要。Canvas上の見た目の位置を維持します。
            childRect.SetParent(newParent, worldPositionStays: true);

            // オプション: Detach後はLayout Groupによる制御が外れるため、
            // アンカーを中央に設定し直すと、以降の操作がやりやすくなる場合があります。
            childRect.anchorMin = new Vector2(0.5f, 0.5f);
            childRect.anchorMax = new Vector2(0.5f, 0.5f);

            // Layout Groupから外れるとスケールが (1, 1, 1) になることが多いですが、
            // 念のためローカルスケールをリセットすることがあります。
            childRect.localScale = Vector3.one; 
        }

        Debug.Log($"{childrenToDetach.Count}個のオブジェクトをLayout Groupの階層から外し、位置を固定しました。", this);

        // 5. Layout Groupコンポーネントを削除（必要であれば）
        // 処理が終わったらLayout Groupは不要になるケースが多いため削除します。
        DestroyImmediate(layoutGroup);
        DestroyImmediate(this); // このユーティリティスクリプト自身も削除
    }
}