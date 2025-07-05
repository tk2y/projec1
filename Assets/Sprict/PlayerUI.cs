using UnityEngine;
using UnityEngine.UI;

public class SharedHealthUIController : MonoBehaviour
{
    // ================== ���̃X�N���v�g���ǂ�����ł��ȒP�ɌĂяo����悤�ɂ���ݒ� ==================
    public static SharedHealthUIController Instance { get; private set; }

    void Awake()
    {
        // �V���O���g���p�^�[���F���̃X�N���v�g�̃C���X�^���X�����ɖ�����Ύ�����o�^���A
        // ���łɑ��݂���Ȃ炱�̃I�u�W�F�N�g��j������B
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


    [Header("HP�o�[�̌����ڐݒ�")]
    [SerializeField]
    private Color healthBarColor = new Color(0.2f, 1f, 0.2f, 1f); // HP�o�[�̐F�i�f�t�H���g�͗΁j
    [SerializeField]
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f); // HP�o�[�̔w�i�F
    [SerializeField]
    private Vector2 barSize = new Vector2(300f, 30f); // HP�o�[�̃T�C�Y
    [SerializeField]
    private Vector2 barPosition = new Vector2(0f, -50f); // HP�o�[�̉�ʏ�ł̈ʒu�i�㕔��������̃I�t�Z�b�g�j

    // �����ŗ��p����ϐ�
    private Image healthBarFillImage;    // HP�o�[�� �~�p�����|�~�u�~�y�u (fill) ������Image�R���|�[�l���g
    private GameObject gameOverPanel;    // �Q�[���I�[�o�[�p�l����GameObject

    // �Q�[���J�n���Ɉ�x�����Ă΂�鏈��
    void Start()
    {
        // GameOver�^�O�����p�l����T���Ĕ�\���ɂ���
        gameOverPanel = GameObject.FindWithTag("GameOver");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // �Q�[���J�n���͔�\��
        }
        else
        {
            Debug.LogError("�G���[: 'GameOver' �^�O������UI�p�l����������܂���B");
        }

        // �X�N���v�g����HP�o�[��������������
        CreateHealthBarUI();
    }

    /// <summary>
    /// �O���̃X�N���v�g����Ăяo���āAHP�o�[�̕\�����X�V����
    /// </summary>
    /// <param name="currentHealth">���݂�HP</param>
    /// <param name="maxHealth">�ő�HP</param>
    public void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        // HP�o�[��Fill Image�����݂��Ȃ��ꍇ�͉������Ȃ�
        if (healthBarFillImage == null) return;

        // �ő�HP��0����0���Z�G���[�ɂȂ邽�߃`�F�b�N
        if (maxHealth <= 0)
        {
            healthBarFillImage.fillAmount = 0;
        }
        else
        {
            // Image��fillAmount�v���p�e�B��HP�̊����i0.0�`1.0�j�ōX�V����
            healthBarFillImage.fillAmount = currentHealth / maxHealth;
        }

        // HP��0�ȉ��ɂȂ�����Q�[���I�[�o�[�������Ă�
        if (currentHealth <= 0)
        {
            ShowGameOver();
        }
    }

    /// <summary>
    /// �Q�[���I�[�o�[�p�l����\������
    /// </summary>
    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// �X�N���v�g����HP�o�[��UI�𓮓I�ɐ�������
    /// </summary>
    private void CreateHealthBarUI()
    {
        // 'PlayerUI' �^�O������Canvas��T��
        GameObject canvasObject = GameObject.FindWithTag("PlayerUI");
        if (canvasObject == null)
        {
            Debug.LogError("�G���[: 'PlayerUI' �^�O������Canvas��������܂���B");
            return;
        }

        // --- HP�o�[�̔w�i���쐬 ---
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

        // --- HP�o�[�̃t�B���i���g�j���쐬 ---
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