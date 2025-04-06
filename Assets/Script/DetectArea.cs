//--------------------------------------------------------------------------
// �{�c���s
// �w�肳�ꂽ�I�u�W�F�N�g���G���A���ɂ��邩�ǂ����𔻒�
//--------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectArea : MonoBehaviour
{
    [Header("�Q�[���J�n���̐F")]
    public Color color;
    [Header("����G���A�̒��S����̑傫��")]
    public float sizeZ = 0.0f;
    public float sizeX = 0.0f;
    [Header("�������鎥�̓I�u�W�F�N�g")]
    public GameObject[] detectedObjects;
    [Header("�ڑ���̉�H�̐F")]
    public Color circuitColor;
    [Header("����G���A�ƌq�����Ă����H")]
    public Renderer[] circuitsRenderer;
    [Header("�ڑ�����SE")]
    public AudioClip audioClip;

    private AudioSource audioSource;
    private bool isConnectFg = false; // ��H���q���������ǂ���

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // �F��ύX
        GetComponent<Renderer>().material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �I�u�W�F�N�g���Փ˂������̏���
    private void OnTriggerEnter(Collider other)
    {
        Vector3 thisPos = this.transform.position;      // ����G���A�̍��W
        Vector3 otherPos = other.transform.position;    // �Փ˂������̓I�u�W�F�N�g�̍��W

        // �Փ˂������̓I�u�W�F�N�g������G���A���ɂ��鎞
        if (thisPos.x - sizeX <= otherPos.x && otherPos.x <= thisPos.x + sizeX &&
           thisPos.z - sizeZ <= otherPos.z && otherPos.z <= thisPos.z + sizeZ)
        {
            // �w�肳�ꂽ�S���̓I�u�W�F�N�g������
            for (int i = 0; i < detectedObjects.Length; i++)
            {
                // ����G���A���̃I�u�W�F�N�g���w�肳�ꂽ���̓I�u�W�F�N�g�Ɠ������A�q����
                if (other.gameObject == detectedObjects[i])
                {
                    // ����G���A�ƌq�����Ă����H�̐F��ύX
                    for (int j = 0; j < circuitsRenderer.Length; j++)
                    {
                        circuitsRenderer[j].material.color = circuitColor;
                        audioSource.PlayOneShot(audioClip);     // SE�Đ�
                    }
                    isConnectFg = true;
                    Debug.Log(isConnectFg);
                }
            }
        }
    }

    // �q���������ǂ����̃t���O���擾
    public bool GetIsConnectFg()
    {
        return isConnectFg;
    }
}
