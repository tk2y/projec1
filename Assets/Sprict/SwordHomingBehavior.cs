using UnityEngine;
using System.Linq;

public class SwordHomingBehavior : MonoBehaviour
{
    [Header("ターゲット設定")]
    [Tooltip("索敵対象とするオブジェクトのタグを複数指定できます。")]
    public string[] targetTags = { "Player", "Cuuerp", "Swordp" };

    [Header("動作設定")]
    [Tooltip("剣が移動する速度")]
    public float speed = 10f;
    [Tooltip("ターゲットを検知する範囲（半径）")]
    public float detectionRadius = 15f;
    [Tooltip("ターゲットにこの距離まで近づいたら攻撃を開始します")]
    public float attackDistance = 2.0f;

    [Header("攻撃設定")]
    [Tooltip("攻撃判定を持つオブジェクト。'SwordWeapon'タグがついている必要があります。")]
    public Transform swordWeaponTransform; // GameObjectからTransformに変更
    [Tooltip("攻撃時間（秒）")]
    public float attackDuration = 2.0f;
    [Tooltip("攻撃時に前進する距離")]
    public float attackMoveDistance = 1.0f;
    [Tooltip("剣を振る角度（例: 90度なら片側45度ずつ振る）")]
    public float swingAngle = 90f;
    [Tooltip("次の攻撃までのクールタイム（秒）")]
    public float cooldownTime = 5.0f;

    // 内部で使用する変数
    private Vector3 originalPosition;
    private Quaternion originalWeaponRotation; // 武器の初期回転を保存
    private Transform currentTarget;
    private SwordState currentState;
    private float timer;
    private Vector3 attackStartPosition;
    private Vector3 attackMoveTargetPosition;
    private Collider swordWeaponCollider;

    private enum SwordState
    {
        Idle, Approaching, Attacking, Returning, Cooldown
    }

    void Start()
    {
        originalPosition = transform.position;

        // SwordWeaponオブジェクトの準備
        if (swordWeaponTransform == null)
        {
            swordWeaponTransform = FindChildWithTag(transform, "SwordWeapon");
            if (swordWeaponTransform != null)
            {
                Debug.Log("'SwordWeapon'タグを持つ子オブジェクトを自動的に見つけました: " + swordWeaponTransform.name);
            }
        }

        if (swordWeaponTransform != null)
        {
            swordWeaponCollider = swordWeaponTransform.GetComponent<Collider>();
            originalWeaponRotation = swordWeaponTransform.localRotation; // ローカル回転を記憶
            if (swordWeaponCollider != null)
            {
                swordWeaponCollider.enabled = false;
            }
            else
            {
                Debug.LogError("swordWeaponTransform に Collider がありません！", this);
            }
        }
        else
        {
            Debug.LogWarning("swordWeaponTransform が設定されていません。攻撃は行われません。", this);
        }

        currentState = SwordState.Idle;
    }

    void Update()
    {
        switch (currentState)
        {
            case SwordState.Idle:
                FindClosestTarget();
                break;
            case SwordState.Approaching:
                ApproachTarget();
                break;
            case SwordState.Attacking:
                PerformAttack();
                break;
            case SwordState.Returning:
                ReturnToOrigin();
                break;
            case SwordState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    private void FindClosestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var col in colliders)
        {
            if (targetTags.Contains(col.tag))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = col.transform;
                }
            }
        }

        if (closestTarget != null)
        {
            currentTarget = closestTarget;
            currentState = SwordState.Approaching;
        }
    }

    private void ApproachTarget()
    {
        if (currentTarget == null)
        {
            currentState = SwordState.Returning;
            return;
        }

        transform.LookAt(currentTarget);
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= attackDistance)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        if (swordWeaponTransform == null || currentTarget == null)
        {
            currentState = SwordState.Returning;
            return;
        }

        currentState = SwordState.Attacking;
        timer = 0f;
        attackStartPosition = transform.position;
        // 攻撃開始時点でのターゲット方向を向く
        transform.LookAt(currentTarget);
        Vector3 attackDirection = transform.forward; // 自分の正面方向
        attackMoveTargetPosition = attackStartPosition + attackDirection * attackMoveDistance;

        currentTarget = null;

        if (swordWeaponCollider != null)
        {
            swordWeaponCollider.enabled = true;
        }
        Debug.Log("攻撃開始！");
    }

    private void PerformAttack()
    {
        timer += Time.deltaTime;

        // 進行度を計算 (0 -> 1 -> 0 のような動き)
        float progress = timer / attackDuration;

        // 1. 前進処理
        transform.position = Vector3.Lerp(attackStartPosition, attackMoveTargetPosition, progress);

        // 2. 武器を振る処理（横振り）
        // Mathf.Sinを使って、0 -> 1 -> -1 -> 0 のような周期的な動きを作る
        // progress * 2 - 1 で範囲を [-1, 1] にする
        float swingValue = Mathf.Sin(progress * Mathf.PI); // 0から始まり1で最大になり2で0に戻る半円の動き

        // Y軸周りに回転させるクォータニオンを計算
        Quaternion swingRotation = Quaternion.Euler(0, swingValue * (swingAngle / 2), 0);

        // 初期回転に合成して適用
        swordWeaponTransform.localRotation = originalWeaponRotation * swingRotation;


        // 攻撃終了判定
        if (progress >= 1.0f)
        {
            if (swordWeaponCollider != null)
            {
                swordWeaponCollider.enabled = false;
            }
            // 武器の回転を元に戻す
            swordWeaponTransform.localRotation = originalWeaponRotation;
            Debug.Log("攻撃終了。帰還します。");
            currentState = SwordState.Returning;
        }
    }

    private void ReturnToOrigin()
    {
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
        float distanceToOrigin = Vector3.Distance(transform.position, originalPosition);

        if (distanceToOrigin < 0.1f)
        {
            transform.position = originalPosition;
            StartCooldown();
        }
    }

    private void StartCooldown()
    {
        currentState = SwordState.Cooldown;
        timer = 0f;
        Debug.Log("クールダウン開始。");
    }

    private void HandleCooldown()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTime)
        {
            currentState = SwordState.Idle;
            Debug.Log("クールダウン終了。索敵を再開します。");
        }
    }

    private Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
            // さらに孫オブジェクトも探す場合
            // Transform result = FindChildWithTag(child, tag);
            // if (result != null) return result;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}