using UnityEngine;
using UnityEngine.UI;

public class Tech_Button : MonoBehaviour
{
    [SerializeField] private int techID;
    [SerializeField] private Button button;

    void Start()
    {
        button.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
    }
}