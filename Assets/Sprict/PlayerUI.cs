using UnityEngine;
using UnityEngine.UI;

public class SharedHealthUIController : MonoBehaviour
{
    // ================== このスクリプトをどこからでも簡単に呼び出せるようにする設定 ==================
    public static SharedHealthUIController Instance { get; private set; }

    void Awake()
    {
        // シングルトンパターン：このスクリプトのインスタンスが他に無ければ自分を登録し、
        // すでに存在するならこのオブジェクトを破棄する。
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // =========================================================================================


    [Header("HPバーの見た目設定")]
    [SerializeField]
    private Color healthBarColor = new Color(0.2f, 1f, 0.2f, 1f); // HPバーの色（デフォルトは緑）
    [SerializeField]
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f); // HPバーの背景色
    [SerializeField]
    private Vector2 barSize = new Vector2(300f, 30f); // HPバーのサイズ
    [SerializeField]
    private Vector2 barPosition = new Vector2(0f, -50f); // HPバーの画面上での位置（上部中央からのオフセット）

    // 内部で利用する変数
    private Image healthBarFillImage;    // HPバーの наполнение (fill) 部分のImageコンポーネント
    private GameObject gameOverPanel;    // ゲームオーバーパネルのGameObject

    // ゲーム開始時に一度だけ呼ばれる処理
    void Start()
    {
        // GameOverタグを持つパネルを探して非表示にする
        gameOverPanel = GameObject.FindWithTag("GameOver");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // ゲーム開始時は非表示
        }
        else
        {
            Debug.LogError("エラー: 'GameOver' タグを持つUIパネルが見つかりません。");
        }

        // スクリプトからHPバーを自動生成する
        CreateHealthBarUI();
    }

    /// <summary>
    /// 外部のスクリプトから呼び出して、HPバーの表示を更新する
    /// </summary>
    /// <param name="currentHealth">現在のHP</param>
    /// <param name="maxHealth">最大HP</param>
    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        // HPバーのFill Imageが存在しない場合は何もしない
        if (healthBarFillImage == null) return;

        // 最大HPが0だと0除算エラーになるためチェック
        if (maxHealth <= 0)
        {
            healthBarFillImage.fillAmount = 0;
        }
        else
        {
            // ImageのfillAmountプロパティをHPの割合（0.0～1.0）で更新する
            healthBarFillImage.fillAmount = currentHealth / maxHealth;
        }

        // HPが0以下になったらゲームオーバー処理を呼ぶ
        if (currentHealth <= 0)
        {
            ShowGameOver();
        }
    }

    /// <summary>
    /// ゲームオーバーパネルを表示する
    /// </summary>
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// スクリプトからHPバーのUIを動的に生成する
    /// </summary>
    private void CreateHealthBarUI()
    {
        // 'PlayerUI' タグを持つCanvasを探す
        GameObject canvasObject = GameObject.FindWithTag("PlayerUI");
        if (canvasObject == null)
        {
            Debug.LogError("エラー: 'PlayerUI' タグを持つCanvasが見つかりません。");
            return;
        }

        // --- HPバーの背景を作成 ---
        GameObject backgroundObject = new GameObject("HealthBar_Background");
        backgroundObject.transform.SetParent(canvasObject.transform, false);

        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = backgroundColor;

        RectTransform bgRect = backgroundObject.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 1f);
        bgRect.anchorMax = new Vector2(0.5f, 1f);
        bgRect.pivot = new Vector2(0.5f, 1f);
        bgRect.sizeDelta = barSize;
        bgRect.anchoredPosition = barPosition;

        // --- HPバーのフィル（中身）を作成 ---
        GameObject fillObject = new GameObject("HealthBar_Fill");
        fillObject.transform.SetParent(backgroundObject.transform, false);

        healthBarFillImage = fillObject.AddComponent<Image>();
        healthBarFillImage.color = healthBarColor;
        healthBarFillImage.type = Image.Type.Filled;
        healthBarFillImage.fillMethod = Image.FillMethod.Horizontal;
        healthBarFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;

        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
    }
}