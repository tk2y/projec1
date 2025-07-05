using UnityEngine;

public class Bacum : MonoBehaviour
{
    // ���̃Q�[���I�u�W�F�N�g�����̃g���K�[�R���C�_�[�ɐڐG�����Ƃ��ɌĂяo����郁�\�b�h
    private void OnTriggerEnter(Collider other)
    {
        // �ڐG��������̃^�O�� "Cutter" �̏ꍇ
        if (other.CompareTag("Cutter"))
        {
            Debug.Log("Cutter�ɐڐG�BPlayer��CutterP�̖������������܂��B");
            SwapRolesAndCamera("Player", "CutterP");
        }
        // �ڐG��������̃^�O�� "Sword" �̏ꍇ
        else if (other.CompareTag("Sword"))
        {
            Debug.Log("Sword�ɐڐG�BPlayer��SwordP�̖������������܂��B");
            SwapRolesAndCamera("Player", "SwordP");
        }
    }

    /// <summary>
    /// �w�肳�ꂽ2�̃^�O�����I�u�W�F�N�g�̖����i�ʒu�A�����A�^�O�j���������A
    /// MainCamera��V�����v���C���[�ɒǏ]�����܂��B
    /// </summary>
    /// <param name="playerTag">���݂̃v���C���[�̃^�O�i��� "Player"�j</param>
    /// <param name="otherTag">��������̃^�O�i"CutterP" �܂��� "SwordP"�j</param>
    private void SwapRolesAndCamera(string playerTag, string otherTag)
    {
        // �^�O���g���ăV�[������I�u�W�F�N�g������
        GameObject currentPlayerObj = GameObject.FindWithTag(playerTag);
        GameObject otherObj = GameObject.FindWithTag(otherTag);

        // 'MainCamera' �^�O���g���ă��C���J����������
        Camera mainCamera = Camera.main;
        GameObject mainCameraObj = null;

        if (mainCamera != null)
        {
            mainCameraObj = mainCamera.gameObject;
        }

        // �K�v�ȃI�u�W�F�N�g���S�Č����������m�F
        if (currentPlayerObj == null || otherObj == null || mainCameraObj == null)
        {
            if (currentPlayerObj == null) Debug.LogError($"�G���[: �^�O '{playerTag}' �����I�u�W�F�N�g��������܂���B");
            if (otherObj == null) Debug.LogError($"�G���[: �^�O '{otherTag}' �����I�u�W�F�N�g��������܂���B");
            if (mainCameraObj == null) Debug.LogError("�G���[: 'MainCamera' �^�O�����J������������܂���B");
            return;
        }

        // --- 1. �ʒu(position)�ƌ���(rotation)�̌��� ---
        Vector3 tempPosition = currentPlayerObj.transform.position;
        Quaternion tempRotation = currentPlayerObj.transform.rotation;

        currentPlayerObj.transform.position = otherObj.transform.position;
        currentPlayerObj.transform.rotation = otherObj.transform.rotation;

        otherObj.transform.position = tempPosition;
        otherObj.transform.rotation = tempRotation;

        // --- 2. �^�O�̌��� ---
        // ����ɂ��A�ǂ��炪���݂̃v���C���[�ł��邩�����m�ɂȂ�
        currentPlayerObj.tag = otherTag;
        otherObj.tag = playerTag; // otherObj���V�����v���C���[�ɂȂ�

        // --- 3. MainCamera�̐e���A�V�����v���C���[�ɕt���ւ� ---
        // �V���� "Player" �^�O���t�����I�u�W�F�N�g (����otherObj) �ɃJ������Ǐ]������
        mainCameraObj.transform.SetParent(otherObj.transform);

        Debug.Log($"�������������B�V�����v���C���[�� '{otherObj.name}' �ł��B");

        // (����) �J�����̑��Έʒu�ƌ��������Z�b�g���āA���_�����肳���܂�
        // �D�݂̎��_�ɂȂ�悤�ɁA�ȉ��̐��l�𒲐����Ă��������B
        mainCameraObj.transform.localPosition = new Vector3(0, 5f, -10f); // ��: �L�����N�^�[�̏�������
        mainCameraObj.transform.localRotation = Quaternion.Euler(20f, 0, 0); // ��: ������������
    }
}