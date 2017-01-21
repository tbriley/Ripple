using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlphaECS.Unity;

public class InputSystem : SystemBehaviour
{
	public GameObject Ground;
	private Rigidbody Player;
	public float ExplosionForce = 1f;

	public override void Setup ()
	{
		base.Setup ();

		Player = GameObject.Find ("Player").GetComponent<Rigidbody> ();
	}

	void Update()
	{
//		if (!Input.GetMouseButton (0))
		if (!Input.GetMouseButtonDown (0))
			return;

		var screenPosition = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay (new Vector3(screenPosition.x, screenPosition.y, 0));

		Debug.DrawRay (ray.origin, ray.direction * 1000f, Color.red);
		RaycastHit raycastHit;

		if (Physics.Raycast (ray, out raycastHit))
		{
			Player.AddExplosionForce (ExplosionForce, raycastHit.point, 5f, 0f, ForceMode.Impulse);
		}
	}
}
