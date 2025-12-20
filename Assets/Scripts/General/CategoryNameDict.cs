using System.Collections.Generic;
using UnityEngine;

public class CategoryNameDict : MonoBehaviour
{
    public static readonly Dictionary<Categories, string> Dict = new Dictionary<Categories, string>
    {
        { Categories.Attack, "攻撃" },
        { Categories.Shield, "シールド" },
        { Categories.Dodge, "回避" }
    };
}
