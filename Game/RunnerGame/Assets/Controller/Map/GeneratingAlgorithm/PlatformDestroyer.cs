using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Removes platforms after they pass the platform destruction point.
/// </summary>
public class PlatformDestroyer : MonoBehaviour {

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
	/// Destroys this platform when it passes the destruction marker.
	/// </summary>
	void Update () {
		if(transform.position.x < platformDestructionPoint.transform.position.x){
			Destroy (gameObject);
			//gameObject.SetActive (false);
		}
	}

}
