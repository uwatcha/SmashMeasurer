using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartEndButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI buttonText;
    const string START_BUTTON_TEXT = "始";
    const string END_BUTTON_TEXT = "終";
    private float pressTime = 0f;
    private bool isPressed = false;
    private const float HOLD_DURATION = 1f;

    void Update()
    {
        UpdateButtonText();

        if (isPressed && Timer.Instance.IsRunning)
        {
            pressTime += Time.deltaTime;
            if (pressTime >= HOLD_DURATION)
            {
                OnLongClick();
                isPressed = false;
                pressTime = 0f;
            }
        }
    }

    void UpdateButtonText()
    {
        if (Timer.Instance.IsRunning)
        {
            buttonText.text = END_BUTTON_TEXT;
        }
        else
        {
            buttonText.text = START_BUTTON_TEXT;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Timer.Instance.IsRunning && isPressed)
        {
            OnClick();
        }
        isPressed = false;
        pressTime = 0f;
    }

    public void OnClick()
    {
        Timer.Instance.StartTimer();
    }

    public void OnLongClick()
    {
        Timer.Instance.StopTimer();
        SceneManager.LoadScene("ChartScene");
    }
}