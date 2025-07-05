using UnityEngine;
using System.Linq;

public class SwordHomingBehavior : MonoBehaviour
{
    [Header("�^�[�Q�b�g�ݒ�")]
    [Tooltip("���G�ΏۂƂ���I�u�W�F�N�g�̃^�O�𕡐��w��ł��܂��B")]
    public string[] targetTags = { "Player", "Cuuerp", "Swordp" };

    [Header("����ݒ�")]
    [Tooltip("�����ړ����鑬�x")]
    public float speed = 10f;
    [Tooltip("�^�[�Q�b�g�����m����͈́i���a�j")]
    public float detectionRadius = 15f;
    [Tooltip("�^�[�Q�b�g�ɂ��̋����܂ŋ߂Â�����U�����J�n���܂�")]
    public float attackDistance = 2.0f;

    [Header("�U���ݒ�")]
    [Tooltip("�U����������I�u�W�F�N�g�B'SwordWeapon'�^�O�����Ă���K�v������܂��B")]
    public Transform swordWeaponTransform; // GameObject����Transform�ɕύX
    [Tooltip("�U�����ԁi�b�j")]
    public float attackDuration = 2.0f;
    [Tooltip("�U�����ɑO�i���鋗��")]
    public float attackMoveDistance = 1.0f;
    [Tooltip("����U��p�x�i��: 90�x�Ȃ�Б�45�x���U��j")]
    public float swingAngle = 90f;
    [Tooltip("���̍U���܂ł̃N�[���^�C���i�b�j")]
    public float cooldownTime = 5.0f;

    // �����Ŏg�p����ϐ�
    private Vector3 originalPosition;
    private Quaternion originalWeaponRotation; // ����̏�����]��ۑ�
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

        // SwordWeapon�I�u�W�F�N�g�̏���
        if (swordWeaponTransform == null)
        {
            swordWeaponTransform = FindChildWithTag(transform, "SwordWeapon");
            if (swordWeaponTransform != null)
            {
                Debug.Log("'SwordWeapon'�^�O�����q�I�u�W�F�N�g�������I�Ɍ����܂���: " + swordWeaponTransform.name);
            }
        }

        if (swordWeaponTransform != null)
        {
            swordWeaponCollider = swordWeaponTransform.GetComponent<Collider>();
            originalWeaponRotation = swordWeaponTransform.localRotation; // ���[�J����]���L��
            if (swordWeaponCollider != null)
            {
                swordWeaponCollider.enabled = false;
            }
            else
            {
                Debug.LogError("swordWeaponTransform �� Collider ������܂���I", this);
            }
        }
        else
        {
            Debug.LogWarning("swordWeaponTransform ���ݒ肳��Ă��܂���B�U���͍s���܂���B", this);
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
        // �U���J�n���_�ł̃^�[�Q�b�g����������
        transform.LookAt(currentTarget);
        Vector3 attackDirection = transform.forward; // �����̐��ʕ���
        attackMoveTargetPosition = attackStartPosition + attackDirection * attackMoveDistance;

        currentTarget = null;

        if (swordWeaponCollider != null)
        {
            swordWeaponCollider.enabled = true;
        }
        Debug.Log("�U���J�n�I");
    }

    private void PerformAttack()
    {
        timer += Time.deltaTime;

        // �i�s�x���v�Z (0 -> 1 -> 0 �̂悤�ȓ���)
        float progress = timer / attackDuration;

        // 1. �O�i����
        transform.position = Vector3.Lerp(attackStartPosition, attackMoveTargetPosition, progress);

        // 2. �����U�鏈���i���U��j
        // Mathf.Sin���g���āA0 -> 1 -> -1 -> 0 �̂悤�Ȏ����I�ȓ��������
        // progress * 2 - 1 �Ŕ͈͂� [-1, 1] �ɂ���
        float swingValue = Mathf.Sin(progress * Mathf.PI); // 0����n�܂�1�ōő�ɂȂ�2��0�ɖ߂锼�~�̓���

        // Y������ɉ�]������N�H�[�^�j�I�����v�Z
        Quaternion swingRotation = Quaternion.Euler(0, swingValue * (swingAngle / 2), 0);

        // ������]�ɍ������ēK�p
        swordWeaponTransform.localRotation = originalWeaponRotation * swingRotation;


        // �U���I������
        if (progress >= 1.0f)
        {
            if (swordWeaponCollider != null)
            {
                swordWeaponCollider.enabled = false;
            }
            // ����̉�]�����ɖ߂�
            swordWeaponTransform.localRotation = originalWeaponRotation;
            Debug.Log("�U���I���B�A�҂��܂��B");
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
        Debug.Log("�N�[���_�E���J�n�B");
    }

    private void HandleCooldown()
    {
        timer += Time.deltaTime;
        if (timer >= cooldownTime)
        {
            currentState = SwordState.Idle;
            Debug.Log("�N�[���_�E���I���B���G���ĊJ���܂��B");
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
            // ����ɑ��I�u�W�F�N�g���T���ꍇ
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