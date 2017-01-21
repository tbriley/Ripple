using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlphaECS.Unity;
using AlphaECS;

public class EnemySystem : SystemBehaviour
{
	public float EnemyForce = 1f;
	public float InfluenceRadius = 5f;

	IGroup Rigidbodies;

	public override void Setup ()
	{
		base.Setup ();

//		foreach (var rbEntity in Rigidbodies.Entities)
//		{
//			var _rigidbody = rbEntity.GetComponent<Rigidbody> ();
//			_rigidbody.AddExplosionForce(EnemyForce, position, RippleRadius, 0f, ForceMode.Impulse);
//		}
	}
}
