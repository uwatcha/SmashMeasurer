using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumField : MonoBehaviour
{
  [SerializeField] private TMP_InputField inputField;
  void Start()
  {
    inputField.onEndEdit.AddListener(OnEndEdit);
  }

  public void OnEndEdit(string text)
  {
    InputDataManager.Instance.StudentID = text;
  }
}