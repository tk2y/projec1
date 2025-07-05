using UnityEngine;
using System.Collections;
using System.Collections.Generic; // HashSet���g�����߂ɕK�v
using System; // Action���g�����߂ɕK�v

public class CutterWeaponController : MonoBehaviour
{
    [Tooltip("���̕��킪�_���[�W��^����Ώۂ̃^�O")]
    [SerializeField] private string[] hitTargetTags = { "Player", "Cuuerp", "Swordp" };

    // 1��̍U���Ńq�b�g�����I�u�W�F�N�g���L�^���A���i�q�b�g��h�����߂̃��X�g
    private HashSet<Collider> hitObjects;

    void Awake()
    {
        // �ŏ��͔�\���ɂ��Ă���
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ����𓊂��鏈�����J�n����
    /// </summary>
    /// <param name="targetPosition">�ړI�n�̃��[���h���W</param>
    /// <param name="owner">������(Cutter)��Transform</param>
    /// <param name="duration">�ړI�n�ɓ��B����܂ł̎���</param>
    /// <param name="onAttackFinish">�U���i�����j�����������Ƃ��ɌĂ΂�鏈��</param>
    public void Throw(Vector3 targetPosition, Transform owner, float duration, Action onAttackFinish)
    {
        // �U���J�n���Ƀq�b�g���X�g��������
        hitObjects = new HashSet<Collider>();

        // �����Cutter�̈ʒu�ɕ\������
        transform.position = owner.position;
        gameObject.SetActive(true);

        // �ړ��V�[�P���X�̃R���[�`�����J�n
        StartCoroutine(MoveSequence(targetPosition, owner, duration, onAttackFinish));
    }

    /// <summary>
    /// ����̉����ړ��𐧌䂷��R���[�`��
    /// </summary>
    private IEnumerator MoveSequence(Vector3 targetPosition, Transform owner, float duration, Action onAttackFinish)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // --- 1. �ړI�n�֌������i�s���j ---
        while (elapsedTime < duration)
        {
            // Lerp���g���Ċ��炩�Ɉړ�
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 1�t���[���҂�
        }
        transform.position = targetPosition; // �덷���Ȃ������߁A�҂����荇�킹��

        // --- 2. Cutter�̌��֖߂�i�A��j ---
        elapsedTime = 0f;
        Vector3 returnStartPosition = transform.position;
        while (elapsedTime < duration)
        {
            // Cutter�������\�����l�����A��ɍŐV��owner.position���Q�Ƃ���
            transform.position = Vector3.Lerp(returnStartPosition, owner.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 1�t���[���҂�
        }

        // --- 3. �U���I������ ---
        gameObject.SetActive(false); // ������\���ɂ���
        onAttackFinish?.Invoke();    // CutterController�ɍU���������������Ƃ�ʒm����
    }

    /// <summary>
    /// �����蔻�� (Trigger)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // ���ɂ��̍U���Ńq�b�g�ς݂̃I�u�W�F�N�g�͖�������
        if (hitObjects.Contains(other))
        {
            return;
        }

        // �U���Ώۂ̃^�O���ǂ������`�F�b�N
        bool isTarget = false;
        foreach (string tag in hitTargetTags)
        {
            if (other.CompareTag(tag))
            {
                isTarget = true;
                break;
            }
        }

        if (isTarget)
        {
            Debug.Log(other.name + " �ɍU�����q�b�g�I");
            hitObjects.Add(other); // �q�b�g���X�g�ɒǉ����āA�����G�ւ̑��i�q�b�g��h��

            // �����Ƀ_���[�W��^���鏈���Ȃǂ��L�q���܂�
            // ��: other.GetComponent<PlayerHealth>()?.TakeDamage(10);
        }
    }
}