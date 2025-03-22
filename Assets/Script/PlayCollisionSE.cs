//--------------------------------------------------------------------------
// �{�c���s
// �w�肵���I�u�W�F�N�g�������������Ɏw�肵��SE���Đ�����
//--------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCollisionSE : MonoBehaviour
{
    [Header("�Փˌ��m����I�u�W�F�N�g")]
    public Object collisionObject = null;
    [Header("�Đ�����SE")]
    public AudioClip collisionSE;
    [Header("1��̂ݍĐ����邩�ǂ���")]
    public bool playOnceFg = false;

    private AudioSource audioSource;    // AudioSource
    private int playCnt = 0;            // �Đ���

    // Start is called before the first frame update
    void Start()
    {
        // �R���|�[�l���g���擾
        audioSource = GetComponent<AudioSource>();

        // AudioSource��null�̎��A�G���[���b�Z�[�W��\��
        if (audioSource == null)
        {
            Debug.LogError(this.name + "��AudioSource�����݂��܂���");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �Փˌ��m����I�u�W�F�N�g���w�肳��Ă��鎞
        if (collisionObject != null)
        {
            // ���������I�u�W�F�N�g���w�肵���I�u�W�F�N�g�Ɠ�����
            if (collision.gameObject == collisionObject)
            {
                // 1��̂ݍĐ����鎞
                if (playOnceFg && playCnt < 1)
                {
                    audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                    playCnt++;
                }
                // ������Đ����鎞
                else if(!playOnceFg)
                {
                    audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                }
            }
        }
        // �w�肳��Ă��Ȃ����͂��ׂẴI�u�W�F�N�g�ƏՓˌ��m
        else
        {
            // 1��̂ݍĐ����鎞
            if (playOnceFg && playCnt < 1)
            {
                audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                playCnt++;
            }
            // ������Đ����鎞
            else if (!playOnceFg)
            {
                audioSource.PlayOneShot(collisionSE);   // SE���Đ�
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �Փˌ��m����I�u�W�F�N�g���w�肳��Ă��鎞
        if (collisionObject != null)
        {
            // ���������I�u�W�F�N�g���w�肵���I�u�W�F�N�g�Ɠ�����
            if (other.gameObject == collisionObject)
            {
                // 1��̂ݍĐ����鎞
                if (playOnceFg && playCnt < 1)
                {
                    audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                    playCnt++;
                }
                // ������Đ����鎞
                else if (!playOnceFg)
                {
                    audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                }
            }
        }
        // �w�肳��Ă��Ȃ����͂��ׂẴI�u�W�F�N�g�ƏՓˌ��m
        else
        {
            // 1��̂ݍĐ����鎞
            if (playOnceFg && playCnt < 1)
            {
                audioSource.PlayOneShot(collisionSE);   // SE���Đ�
                playCnt++;
            }
            // ������Đ����鎞
            else if (!playOnceFg)
            {
                audioSource.PlayOneShot(collisionSE);   // SE���Đ�
            }
        }
    }
}
