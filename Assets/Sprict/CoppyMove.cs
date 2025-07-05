using UnityEngine;

// ���̃X�N���v�g��Rigidbody�R���|�[�l���g��K�v�Ƃ��܂�
[RequireComponent(typeof(Rigidbody))]
public class SimpleCharacterMovement : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    [Tooltip("�L�����N�^�[�̈ړ����x")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("�W�����v�ݒ�")]
    [Tooltip("�W�����v�̋���")]
    [SerializeField] private float jumpForce = 7.0f;

    [Tooltip("�n�ʂƔ��肷��I�u�W�F�N�g�̃^�O")]
    [SerializeField] private string groundTag = "Ground";

    // �����Ŏg�p����ϐ�
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Awake()
    {
        // Rigidbody�R���|�[�l���g���擾���āA�ϐ�rb�ɃL���b�V������
        rb = GetComponent<Rigidbody>();
        // �O�̂��߁A�X���[�v���[�h���������Ă���
        rb.sleepThreshold = 0.0f;
    }

    void Update()
    {
        // --- 1. ���͂̎�t ---
        // WASD�L�[�܂��͖��L�[����̓��͂��擾 (-1����1�͈̔�)
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // A, D�L�[
        float verticalInput = Input.GetAxisRaw("Vertical");     // W, S�L�[

        // ���͂���ړ������̃x�N�g�����쐬
        moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // --- 2. �W�����v�̎�t ---
        // Space�L�[��������A���L�����N�^�[���n�ʂɂ���ꍇ�ɃW�����v�����s
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // �������Z�Ɋւ��鏈����FixedUpdate�ōs���̂���������܂�

        // --- 3. �ړ��Ɖ�]�̎��s ---
        Move();
    }

    private void Move()
    {
        // ���݂�Y�������̑��x��ێ����A���������̑��x���v�Z
        float yVelocity = rb.linearVelocity.y;
        Vector3 newVelocity = moveDirection * moveSpeed;
        newVelocity.y = yVelocity;

        // Rigidbody�̑��x���X�V���ăL�����N�^�[���ړ�������
        rb.linearVelocity = newVelocity;
    }

    private void Jump()
    {
        // �W�����v�̏u�Ԃ�isGrounded��false�ɂ��A�A���W�����v��h��
        isGrounded = false;
        // Y�������̑��x�����Z�b�g���Ă���͂������邱�ƂŁA���肵�������̃W�����v�ɂȂ�
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        // ������iVector3.up�j�ɗ͂������ăW�����v������
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // --- 4. �ڒn���� ---
    // ���̃R���C�_�[�ɐڐG�����u�ԂɌĂяo�����
    private void OnCollisionEnter(Collision collision)
    {
        // �ڐG�����I�u�W�F�N�g�̃^�O�� "Ground" �̏ꍇ
        if (collision.gameObject.CompareTag(groundTag))
        {
            // �ڒn�t���O��true�ɂ���
            isGrounded = true;
        }
    }

    // ���̃R���C�_�[�ɐڐG���Ă���ԁA�p�����ČĂяo�����
    // (�⓹�Ȃǂ����藎����ۂ�Enter/Exit���p������̂�h���A��������肳���邽��)
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isGrounded = true;
        }
    }

    // ���̃R���C�_�[���痣�ꂽ�u�ԂɌĂяo�����
    private void OnCollisionExit(Collision collision)
    {
        // ���ꂽ�I�u�W�F�N�g�̃^�O�� "Ground" �̏ꍇ
        if (collision.gameObject.CompareTag(groundTag))
        {
            // �ڒn�t���O��false�ɂ���
            isGrounded = false;
        }
    }
}