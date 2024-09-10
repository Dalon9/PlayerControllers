using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.ClimbingHelper.Runtime
{
	public class ClimbableObject : MonoBehaviour
	{
		public Transform LadderEntry;
		public Transform LadderExit;

		public float LadderPointDistance = 0.15f;

		private void Awake()
		{
			RaycastHit hit;
			Vector3 direction = Vector3.Scale(transform.position - LadderEntry.position, new Vector3(1f, 0f, 1f)).normalized;
			Physics.Raycast(LadderEntry.position, direction, out hit);
			LadderEntry.position = hit.point + hit.normal * LadderPointDistance;
            LadderExit.position = new Vector3(LadderEntry.position.x, LadderExit.position.y, LadderEntry.position.z);
		}
	}
}