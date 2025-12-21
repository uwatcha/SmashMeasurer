using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
  [SerializeField] private Button button;
  void Start()
  {
    button.onClick.AddListener(OnClick);
  }

  public void OnClick()
  {
    InputDataManager.Instance.SaveData();
  }
}