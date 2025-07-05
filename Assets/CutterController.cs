using UnityEngine;
using System.Linq;

public class CutterController : MonoBehaviour
{
    [Header("索敵設定")]
    [Tooltip("キャラクターを検知する範囲")]
    [SerializeField] private float detectionRadius = 10f;
    [Tooltip("ターゲットの方向を向く速さ")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("索敵対象とするオブジェクトのタグ")]
    [SerializeField] private string[] targetTags = { "Player", "Cuuerp", "Swordp" };

    [Header("攻撃設定")]
    [Tooltip("攻撃に使う武器のCutterWeaponControllerをアタッチ")]
    [SerializeField] private CutterWeaponController cutterWeapon;
    [Tooltip("武器が飛んでいく距離")]
    [SerializeField] private float attackDistance = 10f;
    [Tooltip("武器が目的地に到達するまでの時間（秒）")]
    [SerializeField] private float timeToReachTarget = 1f;
    [Tooltip("次の攻撃までのクールタイム（秒）")]
    [SerializeField] private float attackCooldown = 4f;

    private Transform currentTarget = null;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    void Update()
    {
        // 攻撃中でなければ、常に最も近いターゲットを探す
        if (!isAttacking)
        {
            FindClosestTarget();
        }

        // クールダウンタイマーを減らす
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // ターゲットがいて、攻撃可能状態なら攻撃を開始する
        if (currentTarget != null && !isAttacking && cooldownTimer <= 0)
        {
            StartAttack();
        }
        // ターゲットがいて、攻撃中でない場合はターゲットの方向を向く
        else if (currentTarget != null && !isAttacking)
        {
            RotateTowardsTarget();
        }
    }

    void StartAttack()
    {
        Debug.Log("攻撃開始！");
        isAttacking = true;
        cooldownTimer = attackCooldown; // クールダウンタイマーをリセット

        // 武器を投げる目標地点を計算
        Vector3 targetPosition = transform.position + transform.forward * attackDistance;

        // 武器に攻撃開始を命令
        // 最後の `() => { isAttacking = false; }` は、武器が戻ってきたら実行される処理
        // これにより、攻撃が完了したことをCutter本体が知ることができる
        cutterWeapon.Throw(targetPosition, transform, timeToReachTarget, () => {
            Debug.Log("攻撃終了。");
            isAttacking = false;
        });
    }

    void FindClosestTarget()
    {
        currentTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        Collider[] collidersInRadius = Physics.OverlapSphere(currentPosition, detectionRadius);

        foreach (Collider col in collidersInRadius)
        {
            if (targetTags.Contains(col.tag))
            {
                float sqrDistanceToTarget = (col.transform.position - currentPosition).sqrMagnitude;
                if (sqrDistanceToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = sqrDistanceToTarget;
                    currentTarget = col.transform;
                }
            }
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 攻撃方向を視覚的に表示
        Gizmos.color = Color.red;
        Vector3 attackTargetPos = transform.position + transform.forward * attackDistance;
        Gizmos.DrawLine(transform.position, attackTargetPos);
        Gizmos.DrawWireSphere(attackTargetPos, 0.5f);
    }
}