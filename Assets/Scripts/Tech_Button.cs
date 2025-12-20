using UnityEngine;
using UnityEngine.UI;

public class Tech_Button : MonoBehaviour
{
    [SerializeField] private TechIDs techID;
    [SerializeField] private Button button;
    private Image buttonImage;
    private Color originalColor;

    void Start()
    {
        buttonImage = button.GetComponent<Image>();
        originalColor = buttonImage.color;
        
        ChangeButtonColorWhite();
        
        // Timerの onStartTimer アクションに登録
        Timer.Instance.onStartTimer += ChangeButtonColorOriginal;
        
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (Timer.Instance.IsRunning)
        {
            InputDataManager.Instance.AddData(techID);
        }
    }

    private void ChangeButtonColorWhite()
    {
        // 灰色にする（アルファ値を下げることで暗く見せる）
        buttonImage.color = new Color(1f, 1f, 1f, 0.5f); // 半透明の白 = 灰色っぽく見える
        // または単純に灰色を指定
        // buttonImage.color = Color.gray;
    }

    private void ChangeButtonColorOriginal()
    {
        buttonImage.color = originalColor;
    }

    private void OnDestroy()
    {
        // イベント登録を解除
        if (Timer.Instance != null)
        {
            Timer.Instance.onStartTimer -= ChangeButtonColorOriginal;
        }
    }
}