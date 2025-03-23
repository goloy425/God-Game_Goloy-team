//------------------------------------------------------------------------
// �{�c���s
// ���Ԋu�Ŏw�肵��SE���Đ�
//------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySEAtRegularIntervals : MonoBehaviour
{
    [Header("�Đ�����SE")]
    public AudioClip[] audioClips;
    [Header("���b�Ԋu�ōĐ����邩")]
    public float second = 5.0f;
    [Header("���Ԓʂ�ɍĐ����邩�ǂ���")]
    public bool inOrderFg = true;
    [Header("�ŏ���1��Đ����邩�ǂ���")]
    public bool playOnAwakeFg = true;

    private AudioSource audioSource;
    private int playNum = 0;            // �Đ�����SE�̔ԍ�
    private float elapsedTime = 0.0f;   // �o�ߎ���

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

        // �ŏ��Ɉ�񂾂��Đ����鎞
        if (playOnAwakeFg)
        {
            // ���Ԓʂ�ɍĐ����鎞
            if (inOrderFg)
            {
                // playNum�Ԗڂ�SE���Đ�
                audioSource.PlayOneShot(audioClips[playNum]);
                playNum = (playNum + 1) % audioClips.Length;    // �v�f���ȏ�̔ԍ��ɂȂ�Ȃ��悤�ɍX�V
            }
            // ���Ԓʂ�ɍĐ����Ȃ����̓����_���ȏ��ԂōĐ�
            else
            {
                playNum = Random.Range(0, audioClips.Length - 1);
                audioSource.PlayOneShot(audioClips[playNum]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �w�肵���b���ɂȂ�����
        if (elapsedTime >= second)
        {
            // ���Ԓʂ�ɍĐ����鎞
            if (inOrderFg)
            {
                // playNum�Ԗڂ�SE���Đ�
                audioSource.PlayOneShot(audioClips[playNum]);
                playNum = (playNum + 1) % audioClips.Length;    // �v�f���ȏ�̔ԍ��ɂȂ�Ȃ��悤�ɍX�V
            }
            // ���Ԓʂ�ɍĐ����Ȃ����̓����_���ȏ��ԂōĐ�
            else
            {
                playNum = Random.Range(0, audioClips.Length - 1);
                audioSource.PlayOneShot(audioClips[playNum]);
            }
            elapsedTime = 0.0f;     // �o�ߎ��Ԃ����Z�b�g
        }
        elapsedTime += Time.deltaTime;
    }

    public void SetElapsedTime(float _elapsedTime)
    {
        elapsedTime = _elapsedTime;
    }
}
