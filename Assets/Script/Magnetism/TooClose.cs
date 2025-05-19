using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//=====================================================================
// �쐬�ҁF�{�{�a��
// ���΂Ǝ��΁A���΂Ǝ��̓I�u�W�F�N�g�̋߂Â����������m����X�N���v�g
//=====================================================================

public class TooClose : MonoBehaviour
{ 
	[Header("GameManager��ݒ�")]
	public GameManager gm;

	[Header("�v���C���[�̎��΂�ݒ�")]
	public Magnetism magnet1;
	public Magnetism magnet2;

	[Header("Plate��ݒ�")]
	public GameObject plate1;
	public GameObject plate2;

	public bool stopSlow = false;

	private float dangerDist;	// ����ȏ�߂Â�������ƃ��o���I�̋���
	private float safetyDist;	// �X���[���[�V�������������鋗���AdangerDistn�̂�����Ə��ݒ肷��

	// Start is called before the first frame update
	void Start()
	{
		// �e�����̐ݒ�
		dangerDist = magnet1.deadRange + 1.0f;
		safetyDist = dangerDist + 0.3f;
	}

	// Update is called once per frame
	void Update()
	{
		if (magnet1.isSlow || magnet2.isSlow)
		{
			StartCoroutine(TriggerSlowMotionEffect());
		}
		else
		{
			StopCoroutine(TriggerSlowMotionEffect());
		}
	}

	////--- �v���C���[�̎��Α΃I�u�W�F�N�g�̐ڋߔ��� ---//
	//private void CheckTooClose()
	//{
	//	// �����̌v��
	//	float dist = Vector3.Distance(plate1.transform.position, plate2.transform.position);

	//	if (dist <= dangerDist)
	//	{
	//		magnet1.isSlow = true;
	//	}

	//	if (magnet1.isSlow && dist > safetyDist)
	//	{
	//		magnet1.isSlow = false;
	//	}

	//	// �e��ނ̎��̓X�N���v�g�i������X�e�[�W�j���擾
	//	var sphereList = gm.GetCurrentStageSphereMagnetisms();
	//	var split1List = gm.GetCurrentStageSplit1Magnetisms();
	//	var split2List = gm.GetCurrentStageSplit2Magnetisms();
	//	var connecterList = gm.GetCurrentStageConnecterMagnetisms();

	//	//--- �����̌v�� ---//
	//	// ��
	//	foreach (var mag in sphereList)
	//	{
	//		// �X�e�[�W���ɊY���I�u�W�F�N�g�������A�������̓X�N���v�g���L���łȂ��ꍇ�X���[����
	//		if (mag == null || !mag.enabled) continue;

	//		// �����̌v��
	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);
	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		// �댯�������ɓ�������
	//		if (dist1 <= dangerDist || dist2 <= dangerDist)
	//		{
	//			if (dist1 <= dangerDist) { magnet1.isSlow = true; }	// magnet1
	//			if (dist2 <= dangerDist) { magnet2.isSlow = true; }	// magnet2
	//		}
	//		// �댯�������ɂ����Ԃň��S�����܂ŗ��ꂽ��
	//		else if ((magnet1.isSlow && dist1 > safetyDist) || (magnet2.isSlow && dist2 > safetyDist))
	//		{
	//			if (magnet1.isSlow) { magnet1.isSlow = false; }			// magnet1
	//			else if (magnet2.isSlow) { magnet2.isSlow = false; }	// magnet2
	//		}
	//	}

	//	// 2�ɕ��������(1)
	//	foreach (var mag in split1List)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);

	//		if (dist1 < dangerDist)
	//		{
	//			Debug.Log("�߂����isplit1Magnetism�j");
	//		}
	//	}

	//	// 2�ɕ��������(1)
	//	foreach (var mag in split2List)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		if (dist2 < dangerDist)
	//		{
	//			Debug.Log("�߂����isplit2Magnetism�j");
	//			// �Q�[���I�[�o�[������x���Ȃ�
	//		}
	//	}

	//	// 2�ɕ������O�̂��
	//	foreach (var mag in connecterList)
	//	{
	//		if (mag == null || !mag.enabled) continue;

	//		dist1 = Vector3.Distance(plate1.transform.position, mag.transform.position);
	//		dist2 = Vector3.Distance(plate2.transform.position, mag.transform.position);

	//		if (dist1 < dangerDist || dist2 < dangerDist)
	//		{
	//			Debug.Log("�߂����iconnecterMagnetism�j");
	//			// �Q�[���I�[�o�[������x���Ȃ�
	//		}
	//	}
	//}

	//--- �X���[���[�V�����̏��� ---//
	IEnumerator TriggerSlowMotionEffect()
	{
		// �X���[���[�V����
		Time.timeScale = 0.2f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;

		// ���������S�ɂȂ邩���΂��������܂ő҂i���A���^�C���ŊĎ��j
		while ((magnet1.isSlow || magnet2.isSlow) &&
			   !magnet1.isSnapping && !magnet2.isSnapping)
		{
			yield return null;  // ���̃t���[���܂őҋ@
		}

		// �X���[���[�V��������
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f;

		stopSlow = false;
	}

	//--- �댯�����E���S�����̃Q�b�^�[ ---//
	public float GetDangerDist()
	{
		return dangerDist;
	}
	public float GetSafetyDist()
	{
		return safetyDist;
	}
}
