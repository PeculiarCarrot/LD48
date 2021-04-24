using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CallFuncOnColliderInteraction : MonoBehaviour
{
	public LayerMask whitelist = ~0;

	public ColliderEvent onTriggerEnter;
	public ColliderEvent onTriggerExit;
	public ColliderEvent onTriggerStay;
	public CollisionEvent onCollisionEnter;
	public CollisionEvent onCollisionExit;
	public CollisionEvent onCollisionStay;

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (Utils.LayerIsInMask(other.gameObject.layer, whitelist))
			onTriggerEnter?.Invoke(other);
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (Utils.LayerIsInMask(other.gameObject.layer, whitelist))
			onTriggerExit?.Invoke(other);
	}

	public void OnTriggerStay2D(Collider2D other)
	{
		if (Utils.LayerIsInMask(other.gameObject.layer, whitelist))
			onTriggerStay?.Invoke(other);
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (Utils.LayerIsInMask(collision.gameObject.layer, whitelist))
			onCollisionEnter?.Invoke(collision);
	}

	public void OnCollisionExit2D(Collision2D collision)
	{
		if (Utils.LayerIsInMask(collision.gameObject.layer, whitelist))
			onCollisionExit?.Invoke(collision);
	}

	public void OnCollisionStay2D(Collision2D collision)
	{
		if (Utils.LayerIsInMask(collision.gameObject.layer, whitelist))
			onCollisionStay?.Invoke(collision);
	}
}

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider2D> { }

[System.Serializable]
public class CollisionEvent : UnityEvent<Collision2D> { }