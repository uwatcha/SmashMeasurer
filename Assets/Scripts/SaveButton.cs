using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
  [SerializeField] private Button button;
  [SerializeField] private TextMeshProUGUI buttonText;
  void Start()
  {
    button.onClick.AddListener(OnClick);
  }

  public void OnClick()
  {
    InputDataManager.Instance.SaveData();
    buttonText.text = "Saved!!";
  }
}