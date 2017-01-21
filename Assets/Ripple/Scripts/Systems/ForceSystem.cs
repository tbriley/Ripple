using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlphaECS.Unity;
using AlphaECS;
using System;
using UniRx;
using UniRx.Triggers;

public class ForceSystem : SystemBehaviour
{
	public GameObject Ground;
	private Rigidbody Player;
	public float ExplosionForce = 1f;
	public float ExplosionRadius = 5f;
	IGroup Rigidbodies;

	public override void Setup ()
	{
		base.Setup ();

		Rigidbodies = GroupFactory.Create (new Type[]{ typeof(Rigidbody) });
	}

	void Update()
	{
		if (!Input.GetMouseButtonDown (0))
			return;

		var screenPosition = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay (new Vector3(screenPosition.x, screenPosition.y, 0));

		RaycastHit raycastHit;
		if (Physics.Raycast (ray, out raycastHit))
		{
			foreach (var entity in Rigidbodies.Entities)
			{
				var _rigidbody = entity.GetComponent<Rigidbody> ();
				_rigidbody.AddExplosionForce(ExplosionForce, raycastHit.point, ExplosionRadius, 0f, ForceMode.Impulse);
			}
		}
	}
}
