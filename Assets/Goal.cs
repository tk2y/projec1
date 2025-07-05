using UnityEngine;

using TMPro; // TextMeshPro���g�p���邽�߂ɕK�v

public class GoalController : MonoBehaviour

{

    // Inspector����Q�[���N���AUI�����蓖�Ă邽�߂̕ϐ�

    public GameObject gameClearUI;

    void Start()

    {

        //�O�̂��߁A�Q�[���J�n���ɂ�UI���\���ɂ��Ă���

        if (gameClearUI != null)

        {

            gameClearUI.SetActive(false);

        }

    }

    // ����Collider�����̃I�u�W�F�N�g�̃g���K�[�ɐN�������Ƃ��ɌĂ΂�郁�\�b�h

    private void OnTriggerEnter(Collider other)

    {

        // �ڐG�����I�u�W�F�N�g�̃^�O�� "Player" ���ǂ������`�F�b�N

        if (other.CompareTag("Player"))

        {

            Debug.Log("�S�[���I"); // ����m�F�p�̃��O

            // �Q�[���N���AUI������Ε\������

            if (gameClearUI != null)

            {

                gameClearUI.SetActive(true);

            }

            // (�C��) �Q�[���̎��Ԃ��~�߂āA�v���C���[�̓����Ȃǂ��~����

            Time.timeScale = 0f;

        }

    }

}
