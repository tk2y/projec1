using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // (他の変数の宣言は変更ありません)
    [Header("移動設定")]
    [Tooltip("プレイヤーの通常の移動速度")]
    [SerializeField] private float moveSpeed = 5.0f;
    [Tooltip("プレイヤーの回転速度")]
    [SerializeField] private float rotationSpeed = 200.0f;
    [Tooltip("Bacum使用中のプレイヤーの移動速度")]
    [SerializeField] private float moveSpeedWhileBacum = 2.5f;

    [Header("ジャンプ設定")]
    [Tooltip("プレイヤーのジャンプ力")]
    [SerializeField] private float jumpForce = 7.0f;
    [Tooltip("地面と判定するタグ")]
    [SerializeField] private string groundTag = "Ground";
    private bool isGrounded;

    [Header("Bacum（吸引機）設定")]
    [Tooltip("出現させるBacumオブジェクトのプレハブ")]
    [SerializeField] private GameObject bacumPrefab;
    [Tooltip("プレイヤーからのBacumの相対的な出現位置 (X:横, Y:高さ, Z:前方)")]
    [SerializeField] private Vector3 bacumOffset = new Vector3(0, 0.5f, 1.5f);
    [Tooltip("プレイヤーの向きに対するBacumの相対的な角度 (X:上下, Y:左右, Z:傾き)")]
    [SerializeField] private Vector3 bacumRotationOffset = Vector3.zero;
    private GameObject currentBacumInstance;

    [Header("カメラ設定")]
    [Tooltip("カメラの追従速度")]
    [SerializeField] private float cameraFollowSpeed = 5.0f;
    [Tooltip("プレイヤーからのカメラの相対的なオフセット (X:横, Y:高さ, Z:後方)")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 3.0f, -5.0f);
    [Tooltip("カメラが注視するプレイヤーの中心からの高さオフセット")]
    [SerializeField] private float cameraLookAtHeightOffset = 1.0f;

    private Rigidbody rb;
    private Camera mainCamera;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("シーンに 'MainCamera' タグのついたカメラが見つかりません。");
        }
    }

    void Update()
    {
        HandleMovementAndRotation();
        if (!Input.GetMouseButton(0) || currentBacumInstance == null)
        {
            HandleJump();
        }
        HandleBacum();
    }

    void LateUpdate()
    {
        HandleBacumFollow();
        if (mainCamera != null)
        {
            HandleCameraFollow();
        }
    }

    private void HandleMovementAndRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
        float currentSpeed = (Input.GetMouseButton(0) && currentBacumInstance != null) ? moveSpeedWhileBacum : moveSpeed;
        Vector3 moveVelocity = transform.forward * verticalInput * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    /// <summary>
    /// ★★★★★ ここを修正しました ★★★★★
    /// BacumのColliderをトリガーに変更して、物理的な衝突をなくします
    /// </summary>
    private void HandleBacum()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (bacumPrefab != null && currentBacumInstance == null)
            {
                Vector3 spawnPosition = transform.TransformPoint(bacumOffset);
                Quaternion spawnRotation = transform.rotation * Quaternion.Euler(bacumRotationOffset);

                currentBacumInstance = Instantiate(bacumPrefab, spawnPosition, spawnRotation);

                // --- ここからが最重要の修正箇所です ---
                // 生成したBacumに付いている全てのColliderを取得して、
                // それらを「トリガーモード」に設定します。
                // これにより、Bacumは物理的な壁ではなくなり、プレイヤーは自由に通り抜けられます。
                Collider[] colliders = currentBacumInstance.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    col.isTrigger = true;
                }
                // --- ここまで ---
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentBacumInstance != null)
            {
                Destroy(currentBacumInstance);
                currentBacumInstance = null;
            }
        }
    }

    private void HandleBacumFollow()
    {
        if (currentBacumInstance != null)
        {
            Vector3 targetPosition = transform.TransformPoint(bacumOffset);
            Quaternion targetRotation = transform.rotation * Quaternion.Euler(bacumRotationOffset);
            currentBacumInstance.transform.position = targetPosition;
            currentBacumInstance.transform.rotation = targetRotation;
        }
    }

    // (HandleCameraFollow と Collision Callbacks は変更なし)
    private void HandleCameraFollow()
    {
        Vector3 targetPosition = transform.position + transform.TransformDirection(cameraOffset);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);
        Vector3 lookAtPoint = transform.position + Vector3.up * cameraLookAtHeightOffset;
        mainCamera.transform.LookAt(lookAtPoint);
    }
    private void OnCollisionEnter(Collision collision) { if (collision.gameObject.CompareTag(groundTag)) isGrounded = true; }
    private void OnCollisionStay(Collision collision) { if (collision.gameObject.CompareTag(groundTag)) isGrounded = true; }
    private void OnCollisionExit(Collision collision) { if (collision.gameObject.CompareTag(groundTag)) isGrounded = false; }
}

//using UnityEngine;

//// Rigidbodyコンポーネントが必須であることを示す属性
//[RequireComponent(typeof(Rigidbody))]
//public class PlayerController : MonoBehaviour
//{
//    [Header("移動設定")]
//    [Tooltip("プレイヤーの移動速度")]
//    [SerializeField] private float moveSpeed = 5.0f;
//    [Tooltip("プレイヤーの回転速度")]
//    [SerializeField] private float rotationSpeed = 200.0f;

//    [Header("ジャンプ設定")]
//    [Tooltip("プレイヤーのジャンプ力")]
//    [SerializeField] private float jumpForce = 7.0f;
//    [Tooltip("地面と判定するタグ")]
//    [SerializeField] private string groundTag = "Ground";
//    private bool isGrounded;

//    [Header("Bacum（吸引機）設定")]
//    [Tooltip("出現させるBacumオブジェクトのプレハブ")]
//    [SerializeField] private GameObject bacumPrefab;
//    [Tooltip("プレイヤーからのBacumの相対的な出現位置 (X:横, Y:高さ, Z:前方)")]
//    [SerializeField] private Vector3 bacumOffset = new Vector3(0, 0.5f, 1.5f);
//    // ★追加: Bacumの角度オフセットを設定する変数
//    [Tooltip("プレイヤーの向きに対するBacumの相対的な角度 (X:上下, Y:左右, Z:傾き)")]
//    [SerializeField] private Vector3 bacumRotationOffset = Vector3.zero;
//    private GameObject currentBacumInstance;

//    [Header("カメラ設定")]
//    [Tooltip("カメラの追従速度")]
//    [SerializeField] private float cameraFollowSpeed = 5.0f;
//    [Tooltip("プレイヤーからのカメラの相対的なオフセット (X:横, Y:高さ, Z:後方)")]
//    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 3.0f, -5.0f);
//    [Tooltip("カメラが注視するプレイヤーの中心からの高さオフセット")]
//    [SerializeField] private float cameraLookAtHeightOffset = 1.0f;

//    private Rigidbody rb;
//    private Camera mainCamera;

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        mainCamera = Camera.main;
//        if (mainCamera == null)
//        {
//            Debug.LogError("シーンに 'MainCamera' タグのついたカメラが見つかりません。");
//        }
//    }

//    void Update()
//    {
//        if (Input.GetMouseButton(0))
//        {
//            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
//        }
//        else
//        {
//            HandleMovementAndRotation();
//            HandleJump();
//        }

//        HandleBacum();
//    }

//    void LateUpdate()
//    {
//        if (mainCamera != null)
//        {
//            HandleCameraFollow();
//        }
//    }

//    private void HandleMovementAndRotation()
//    {
//        float horizontalInput = Input.GetAxis("Horizontal");
//        float verticalInput = Input.GetAxis("Vertical");

//        transform.Rotate(Vector3.up * horizontalInput * rotationSpeed * Time.deltaTime);
//        Vector3 moveVelocity = transform.forward * verticalInput * moveSpeed;
//        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
//    }

//    private void HandleJump()
//    {
//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//        {
//            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
//            isGrounded = false;
//        }
//    }

//    /// <summary>
//    /// ★修正: Bacumの出現・消滅処理
//    /// 角度オフセットを適用するように修正
//    /// </summary>
//    private void HandleBacum()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            if (bacumPrefab != null && currentBacumInstance == null)
//            {
//                // --- 位置の計算 (変更なし) ---
//                Vector3 spawnPosition = transform.position
//                    + transform.right * bacumOffset.x
//                    + transform.up * bacumOffset.y
//                    + transform.forward * bacumOffset.z;

//                // --- 角度の計算 (★ここを修正) ---
//                // 1. プレイヤー自身の回転を取得
//                Quaternion playerRotation = transform.rotation;
//                // 2. インスペクターで設定した角度オフセットをQuaternionに変換
//                Quaternion offsetRotation = Quaternion.Euler(bacumRotationOffset);
//                // 3. プレイヤーの回転にオフセットを適用して最終的な生成角度を計算
//                Quaternion spawnRotation = playerRotation * offsetRotation;

//                // 計算した位置と角度でBacumを生成
//                currentBacumInstance = Instantiate(bacumPrefab, spawnPosition, spawnRotation);
//            }
//        }
//        else if (Input.GetMouseButtonUp(0))
//        {
//            if (currentBacumInstance != null)
//            {
//                Destroy(currentBacumInstance);
//                currentBacumInstance = null;
//            }
//        }
//    }

//    private void HandleCameraFollow()
//    {
//        Vector3 targetPosition = transform.position + transform.TransformDirection(cameraOffset);
//        mainCamera.transform.position = Vector3.Lerp(
//            mainCamera.transform.position,
//            targetPosition,
//            cameraFollowSpeed * Time.deltaTime
//        );

//        Vector3 lookAtPoint = transform.position + Vector3.up * cameraLookAtHeightOffset;
//        mainCamera.transform.LookAt(lookAtPoint);
//    }

//    // --- Collision Callbacks (変更なし) ---
//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.CompareTag(groundTag))
//        {
//            isGrounded = true;
//        }
//    }

//    private void OnCollisionStay(Collision collision)
//    {
//        if (collision.gameObject.CompareTag(groundTag))
//        {
//            isGrounded = true;
//        }
//    }

//    private void OnCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.CompareTag(groundTag))
//        {
//            isGrounded = false;
//        }
//    }
//}