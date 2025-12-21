using TMPro;
using UnityEngine;

public class TimesField : MonoBehaviour
{
  [SerializeField] private TMP_InputField inputField;
  void Start()
  {
    inputField.onEndEdit.AddListener(OnEndEdit);
  }

  public void OnEndEdit(string text)
  {
    InputDataManager.Instance.ExperimentTimesCount = int.Parse(text);
  }
}