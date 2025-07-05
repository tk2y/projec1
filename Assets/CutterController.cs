using UnityEngine;
using System.Linq;

public class CutterController : MonoBehaviour
{
    [Header("���G�ݒ�")]
    [Tooltip("�L�����N�^�[�����m����͈�")]
    [SerializeField] private float detectionRadius = 10f;
    [Tooltip("�^�[�Q�b�g�̕�������������")]
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("���G�ΏۂƂ���I�u�W�F�N�g�̃^�O")]
    [SerializeField] private string[] targetTags = { "Player", "Cuuerp", "Swordp" };

    [Header("�U���ݒ�")]
    [Tooltip("�U���Ɏg�������CutterWeaponController���A�^�b�`")]
    [SerializeField] private CutterWeaponController cutterWeapon;
    [Tooltip("���킪���ł�������")]
    [SerializeField] private float attackDistance = 10f;
    [Tooltip("���킪�ړI�n�ɓ��B����܂ł̎��ԁi�b�j")]
    [SerializeField] private float timeToReachTarget = 1f;
    [Tooltip("���̍U���܂ł̃N�[���^�C���i�b�j")]
    [SerializeField] private float attackCooldown = 4f;

    private Transform currentTarget = null;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;

    void Update()
    {
        // �U�����łȂ���΁A��ɍł��߂��^�[�Q�b�g��T��
        if (!isAttacking)
        {
            FindClosestTarget();
        }

        // �N�[���_�E���^�C�}�[�����炷
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // �^�[�Q�b�g�����āA�U���\��ԂȂ�U�����J�n����
        if (currentTarget != null && !isAttacking && cooldownTimer <= 0)
        {
            StartAttack();
        }
        // �^�[�Q�b�g�����āA�U�����łȂ��ꍇ�̓^�[�Q�b�g�̕���������
        else if (currentTarget != null && !isAttacking)
        {
            RotateTowardsTarget();
        }
    }

    void StartAttack()
    {
        Debug.Log("�U���J�n�I");
        isAttacking = true;
        cooldownTimer = attackCooldown; // �N�[���_�E���^�C�}�[�����Z�b�g

        // ����𓊂���ڕW�n�_���v�Z
        Vector3 targetPosition = transform.position + transform.forward * attackDistance;

        // ����ɍU���J�n�𖽗�
        // �Ō�� `() => { isAttacking = false; }` �́A���킪�߂��Ă�������s����鏈��
        // ����ɂ��A�U���������������Ƃ�Cutter�{�̂��m�邱�Ƃ��ł���
        cutterWeapon.Throw(targetPosition, transform, timeToReachTarget, () => {
            Debug.Log("�U���I���B");
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

        // �U�����������o�I�ɕ\��
        Gizmos.color = Color.red;
        Vector3 attackTargetPos = transform.position + transform.forward * attackDistance;
        Gizmos.DrawLine(transform.position, attackTargetPos);
        Gizmos.DrawWireSphere(attackTargetPos, 0.5f);
    }
}