using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player, CutterP, SwordP�̗̑͂����L���A����̃^�O�����I�u�W�F�N�g����̃_���[�W���Ǘ����܂��B
/// ���̃X�N���v�g�͈ȉ���2�̖����������܂��B
/// 1. ���GameObject�ɃA�^�b�`���A�̗͂ƃ_���[�W�ݒ���Ǘ�����u�Ǘ��I�u�W�F�N�g�v
/// 2. �_���[�W���󂯂�L�����N�^�[(Player, CutterP, SwordP)�ɃA�^�b�`���A�����蔻����s���u����R���|�[�l���g�v
/// </summary>
public class SharedHealthManager : MonoBehaviour
{
    #region �V���O���g�� (�Ǘ��I�u�W�F�N�g�ւ̃A�N�Z�X�p)
    // ���̃N���X�̗B��̃C���X�^���X��ێ����܂��B
    public static SharedHealthManager Instance { get; private set; }
    #endregion

    #region Inspector�ݒ荀�� (�Ǘ��I�u�W�F�N�g���Őݒ肵�܂�)
    [Header("�̗͐ݒ�")]
    [Tooltip("���L����̗͂̍ő�l")]
    public float maxHealth = 100f;

    [Header("�_���[�W�ݒ�")]
    [Tooltip("Cutter�^�O�����I�u�W�F�N�g����󂯂�_���[�W")]
    public float cutterDamage = 10f;
    [Tooltip("Sword�^�O�����I�u�W�F�N�g����󂯂�_���[�W")]
    public float swordDamage = 15f;
    [Tooltip("CutterWeapon�^�O�����I�u�W�F�N�g����󂯂�_���[�W")]
    public float cutterWeaponDamage = 20f;
    [Tooltip("SwordWeapon�^�O�����I�u�W�F�N�g����󂯂�_���[�W")]
    public float swordWeaponDamage = 25f;
    #endregion

    #region ���L�f�[�^
    // ���݂̋��L�̗͂�ێ�����ϐ�
    private float _currentHealth;

    // �O�����猻�݂̗̑͂����S�ɓǂݎ�邽�߂̃v���p�e�B
    public float CurrentHealth => _currentHealth;
    #endregion

    /// <summary>
    /// �X�N���v�g�̃C���X�^���X�����[�h���ꂽ�Ƃ��ɌĂяo����܂��B
    /// </summary>
    void Awake()
    {
        // --- �V���O���g���p�^�[���̎��� ---
        // �܂��Ǘ��C���X�^���X(Instance)���ݒ肳��Ă��Ȃ��ꍇ
        if (Instance == null)
        {
            // ���̃C���X�^���X���Ǘ��C���X�^���X�Ƃ��Đݒ�
            Instance = this;
            // �̗͂�������
            _currentHealth = maxHealth;
            Debug.Log("�̗͊Ǘ��}�l�[�W���[������������܂����B");

            // (�C��)�V�[�����܂����ł����̊Ǘ��I�u�W�F�N�g��j�󂵂Ȃ��悤�ɂ���ꍇ
            // DontDestroyOnLoad(gameObject);
        }
        // ���ɊǗ��C���X�^���X�����݂���ꍇ�́A���̃C���X�^���X�͓����蔻��p�Ƃ݂Ȃ�
    }

    /// <summary>
    /// �_���[�W��K�p���郁�\�b�h�i�Ǘ��C���X�^���X����Ăяo����܂��j
    /// </summary>
    /// <param name="damageAmount">�󂯂�_���[�W��</param>
    public void ApplyDamage(float damageAmount)
    {
        // ���ɑ̗͂�0�ȉ��Ȃ牽�����Ȃ�
        if (_currentHealth <= 0) return;

        _currentHealth -= damageAmount;
        Debug.Log($"�_���[�W�I {damageAmount} �̃_���[�W���󂯂܂����B�c��̗�: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// �̗͂�0�ɂȂ������̏���
    /// </summary>
    private void Die()
    {
        Debug.LogWarning("�̗͂�0�ɂȂ�܂����B");
        // �����ɃQ�[���I�[�o�[������A�L�����N�^�[���j�󂳂��Ȃǂ̏������L�q�ł��܂��B
    }

    /// <summary>
    /// �g���K�[�ɂ��Փ˔���i�����蔻��p�R���|�[�l���g�Ƃ��ċ@�\�j
    /// </summary>
    /// <param name="other">�ڐG���������Collider</param>
    private void OnTriggerEnter(Collider other)
    {
        // ���̃C���X�^���X���Ǘ��p�C���X�^���X���g�̏ꍇ�́A�Փ˔�����s��Ȃ�
        if (Instance == this)
        {
            return;
        }

        // --- ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���_���[�W���󂯂�ׂ������� ---
        List<string> damageableTags = new List<string> { "Player", "CutterP", "SwordP" };
        if (!damageableTags.Contains(gameObject.tag))
        {
            // Player, CutterP, SwordP �ȊO�Ȃ珈���𒆒f
            return;
        }

        // --- �ڐG��������̃^�O�ɉ����ă_���[�W������ ---
        float damageToTake = 0f;
        switch (other.tag)
        {
            case "Cutter":
                damageToTake = Instance.cutterDamage;
                break;
            case "Sword":
                damageToTake = Instance.swordDamage;
                break;
            case "CutterWeapon":
                damageToTake = Instance.cutterWeaponDamage;
                break;
            case "SwordWeapon":
                damageToTake = Instance.swordWeaponDamage;
                break;
            default:
                // �_���[�W��^����^�O�ȊO�ɐڐG�����ꍇ�͉������Ȃ�
                return;
        }

        // �_���[�W�ʂ�0���傫���ꍇ�A�Ǘ��C���X�^���X�Ƀ_���[�W�������˗�
        if (damageToTake > 0)
        {
            Debug.Log($"{gameObject.name}({gameObject.tag}) �� {other.name}({other.tag}) �ƐڐG���܂����B");
            Instance.ApplyDamage(damageToTake);
        }
    }
}