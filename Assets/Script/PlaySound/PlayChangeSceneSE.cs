//------------------------------------------------------------------------------
//  �{�c���s
//  GameManager����Q�[���N���A�A�Q�[���I�[�o�[�̃t���O���󂯎��SE���Đ�����
//------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayChangeSceneSE : MonoBehaviour
{
    [Header("�V�[���J�ڎ���SE")]
    public AudioClip gameClearSE;
    public AudioClip gameOverSE;
    [Header("SE�Đ����牽�b��Ɏ��g���폜���邩")]
    public float destroySecond = 6.0f;
    [Header("�t���O�������Ă��牽�b��ɍĐ����邩")]
    public float playSecond = 1.0f;

    private AudioSource audioSource;
    private GameManager gameManager;

    private int playCnt = 0;    // �Đ���

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
        // GameManager�R���|�[�l���g���擾
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // �V�[���J�ڂ��Ă��폜����Ȃ��悤�ɂ���
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        // �Q�[���N���A�������A1��Đ�
        if(gameManager.GetGameClearFg() && playCnt < 1)
        {
            Invoke("PlayGameClearSE", playSecond);               // playSecond�b���SE�Đ�
;           Destroy(gameObject, destroySecond + playSecond);     // DestroySecond�b��Ɏ��g���폜
            playCnt++;
        }

        // �Q�[���I�[�o�[�ɂȂ������A1��Đ�
        if (gameManager.GetGameOverFg() && playCnt < 1)
        {
            Invoke("PlayGameOverSE", playSecond);                // playSecond�b���SE�Đ�
            Destroy(gameObject, destroySecond + playSecond);     // DestroySecond�b��Ɏ��g���폜
            playCnt++;
        }
    }

    // �Q�[���N���A����SE�Đ�
    private void PlayGameClearSE()
    {
        audioSource.PlayOneShot(gameClearSE);   // �Q�[���N���ASE�Đ�
    }

    // �Q�[���I�[�o�[����SE�Đ�
    private void PlayGameOverSE()
    {
        audioSource.PlayOneShot(gameOverSE);   // �Q�[���I�[�o�[SE�Đ�
    }
}