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
	public float RippleForce = 1f;
	public float RippleRadius = 5f;

	public float DragForce = 1f;
	public float DragRadius = 1f;

	IGroup Rigidbodies;
	IGroup DownTriggers;
	IGroup DragTriggers;

	public override void Setup ()
	{
		base.Setup ();

		Rigidbodies = GroupFactory.Create(new Type[] { typeof(Rigidbody) });
		DownTriggers = GroupFactory.Create (new Type[]{ typeof(ObservablePointerDownTrigger) });
		DragTriggers = GroupFactory.Create (new Type[]{ typeof(ObservableDragTrigger) });

		DownTriggers.Entities.ObserveAdd ().Select (x => x.Value).StartWith (DownTriggers.Entities).Subscribe (entity =>
		{
			var trigger = entity.GetComponent<ObservablePointerDownTrigger>();

			trigger.OnPointerDownAsObservable ().Subscribe (eventData =>
			{
				var position = eventData.pointerPressRaycast.worldPosition;
				foreach (var rbEntity in Rigidbodies.Entities)
				{
					var _rigidbody = rbEntity.GetComponent<Rigidbody> ();
					_rigidbody.AddExplosionForce(RippleForce, position, RippleRadius, 0f, ForceMode.Impulse);
                    FindObjectOfType<GraphicMeshUpdater>().Impulse(position, -1);
				}
			}).AddTo (this.Disposer).AddTo(trigger.gameObject);
		}).AddTo (this.Disposer);

		DragTriggers.Entities.ObserveAdd ().Select (x => x.Value).StartWith (DragTriggers.Entities).Subscribe (entity =>
		{
			var trigger = entity.GetComponent<ObservableDragTrigger>();
			trigger.OnDragAsObservable ().Subscribe (eventData =>
			{
				var position = eventData.pointerCurrentRaycast.worldPosition;
				foreach (var rbEntity in Rigidbodies.Entities)
				{
					var _rigidbody = rbEntity.GetComponent<Rigidbody> ();
					_rigidbody.AddExplosionForce(DragForce, position, DragRadius, 0f, ForceMode.Impulse);
                    FindObjectOfType<GraphicMeshUpdater>().Impulse(position, -1);
                }
            }).AddTo (this.Disposer).AddTo(trigger.gameObject);
		}).AddTo (this.Disposer);
	}
}
