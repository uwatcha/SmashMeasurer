using UnityEngine;
using UnityEngine.UI;

public class UndoButton : MonoBehaviour
{
    [SerializeField] private Button button;
    void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        InputDataManager.Instance.RemoveLastData();
    }
}