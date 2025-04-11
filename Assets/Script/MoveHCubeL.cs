using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

//=================================================
// �쐬�ҁF�{�{�a��
// ���L���[�u�iL�j�𓮂����X�N���v�g
//=================================================

public class MoveHCubeL : MonoBehaviour
{
	[Header("�v���C���[�I�u�W�F�N�g��ݒ�")]
	public GameObject playerL;

	[Header("���΂̓o�^")]
    public Transform magnet1;

    public bool isCarryingL = false;
	private Rigidbody rb;

	// ���͋����t���O�����ꂼ��擾����p
	private AugMagL magL_Aug;

    private PlaySEAtRegularIntervals playSE;    // PlaySEAtRegularIntervals�R���|�[�l���g

    // Start is called before the first frame update
    void Start()
	{
		rb = GetComponent<Rigidbody>();
		playerL.TryGetComponent<AugMagL>(out magL_Aug);

        playSE = GetComponent<PlaySEAtRegularIntervals>();
    }

	private void Update()
	{
		if (magL_Aug.isAugmenting)
		{
			StartCarryingL();
		}
		else
		{
			StopCarryingL();
		}
	}

	private void FixedUpdate()
	{
		if (isCarryingL)
		{
			Vector3 direction = (magnet1.position - transform.position);
			float distance = direction.magnitude;
			float speed = Mathf.Clamp(distance, 0.1f, 3.0f);

			Vector3 velocity = direction.normalized * speed;
			rb.velocity = Vector3.Lerp(rb.velocity, velocity, Time.fixedDeltaTime * 10f);
        }
	}

	// --- �����グ�J�n --- //
	public void StartCarryingL()
	{
		isCarryingL = true;
		rb.useGravity = false;
		rb.angularDrag = 5f;

        playSE.enabled = true;      // SE�Đ��X�N���v�g��L����
    }

	// --- �����グ�I�� --- //
	public void StopCarryingL()
	{
		isCarryingL = false;
		rb.useGravity = true;
		rb.angularDrag = 10f;
		rb.velocity = Vector3.zero;

        playSE.SetElapsedTime(0);   // SE�Đ��X�N���v�g�̌o�ߎ��Ԃ����Z�b�g
        playSE.SetPlayCnt(0);		// SE�Đ��X�N���v�g�̍Đ��񐔂����Z�b�g
        playSE.enabled = false;     // SE�Đ��X�N���v�g�𖳌���
    }
}
