using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMagnetism : MonoBehaviour
{
	[Header("���́E�͈͂̐ݒ�")]
	[SerializeField] private float magnetismRange = 10.0f;
	[SerializeField] private float deadRange = 1.0f;
	public float magnetism = 200.0f;
	public float strongMagnetism = 999.0f;

	public float MagnetismRange => magnetismRange;
	public float DeadRange => deadRange;

	[Header("�v���C���[�I�u�W�F�N�g��ݒ�")]
	public GameObject playerL;
	public GameObject playerR;

	[Header("�������Ă�L���[�u��ݒ�")]
	public Transform cube1;
	public Transform cube2;

	//--- ���΂̃��X�g�Ǘ� ---//
	private static List<Magnetism> registeredMagnets = new();

    public static void Register(Magnetism magnet)
	{
		if (!registeredMagnets.Contains(magnet))
		{
			registeredMagnets.Add(magnet);
		}
	}
	public static void Unregister(Magnetism magnet)
	{
		registeredMagnets.Remove(magnet);
	}

    private void FixedUpdate()
	{
		//--- ���΂̈����񂹏��� ---//
		foreach (var magnet in registeredMagnets)
		{
			if (magnet == null || magnet.isSnapping) continue;

			Vector3 magnetPos = magnet.myPlate.position;

			// Collider�𗘗p���Ĉ�ԋ߂��\�ʂ̍��W���擾
			Vector3 surface1 = cube1.GetComponent<SphereCollider>().ClosestPoint(magnetPos);
			Vector3 surface2 = cube2.GetComponent<SphereCollider>().ClosestPoint(magnetPos);

			// �\�ʍ��W�Ƃ̋������v�Z
			float distance1 = Vector3.Distance(surface1, magnetPos);
			float distance2 = Vector3.Distance(surface2, magnetPos);

			Vector3 targetSurface = (distance1 < distance2) ? surface1 : surface2;
			float surfaceDistance = Mathf.Min(distance1, distance2);

			if (surfaceDistance > MagnetismRange)
			{
				magnet.inHCubeObjMagArea = false;
				continue;
			}

			magnet.inHCubeObjMagArea = true;

			Vector3 direction = (targetSurface - magnetPos).normalized;
			float force = (surfaceDistance < deadRange) ? strongMagnetism : magnetism;
			magnet.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Acceleration);

			if (surfaceDistance < magnet.snapDistance)
			{
				Rigidbody rb = magnet.GetComponent<Rigidbody>();
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;

				AttachToSurface(magnet, targetSurface);
				magnet.isSnapping = true;
			}
		}
	}

	private void AttachToSurface(Magnetism magnet, Vector3 snapPosition)
	{
		if (magnet.isSnapping) return;

		FixedJoint joint = magnet.gameObject.AddComponent<FixedJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();

		magnet.myPlate.position = snapPosition;

		magnet.GetComponent<AudioSource>().PlayOneShot(magnet.magnetSE);
    }
}
