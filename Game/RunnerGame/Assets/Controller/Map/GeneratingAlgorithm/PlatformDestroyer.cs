using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recycles generated objects after they pass the platform destruction point.
/// </summary>
public class PlatformDestroyer : MonoBehaviour {
	private ObjectPooler objectPooler;

	/// <summary>
	/// Transform marker behind which platforms are destroyed.
	/// </summary>
	public GameObject platformDestructionPoint; 

	/// <summary>
	/// Finds the scene's platform destruction marker.
	/// </summary>
	void Start () {
		platformDestructionPoint = GameObject.Find ("PlatformDestructionPoint"); 

	}
	
	/// <summary>
	/// Returns this object to its pool when it passes the destruction marker.
	/// </summary>
	void Update () {
		if(transform.position.x < platformDestructionPoint.transform.position.x){
			if (objectPooler == null || !objectPooler.ReturnPooledObject (gameObject)) {
				Destroy (gameObject);
			}
		}
	}

	/// <summary>
	/// Associates a generated platform with the pool that should receive it when recycled.
	/// </summary>
	public void SetObjectPooler(ObjectPooler pooler) {
		objectPooler = pooler;
	}

}
