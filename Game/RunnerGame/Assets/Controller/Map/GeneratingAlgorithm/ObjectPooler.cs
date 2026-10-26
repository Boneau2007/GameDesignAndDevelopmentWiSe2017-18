using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Maintains a reusable collection of inactive game objects.
/// </summary>
public class ObjectPooler : MonoBehaviour {

	/// <summary>
	/// Prefab used to create pooled instances.
	/// </summary>
	public GameObject pooledObject; 
	/// <summary>
	/// Number of objects to create when the pool is initialized.
	/// </summary>
	public int pooledAmount; 

	/// <summary>
	/// Objects currently managed by this pool.
	/// </summary>
	public List<GameObject> pooledObjects; 

	/// <summary>
	/// Creates the initial batch of inactive pooled objects.
	/// </summary>
	void Start () {
		pooledObjects = new List<GameObject> (); 

		for(int i = 0; i < pooledAmount; i++){
			GameObject obj = (GameObject) Instantiate (pooledObject);
			obj.SetActive (false);
			pooledObjects.Add (obj);
		}

	}
	
	/// <summary>
	/// Gets an inactive pooled object, creating one if the pool has none available.
	/// </summary>
	/// <returns>An inactive object ready to be configured and activated.</returns>
	public GameObject GetPooledObject(){

		for (int i = 0; i < pooledObjects.Count; i++) {
			if (!pooledObjects [i].activeInHierarchy) {
				return pooledObjects [i];
			}
		}

		GameObject obj = (GameObject) Instantiate (pooledObject);
		obj.SetActive (false);
		pooledObjects.Add (obj);
		return obj; 
	}
	
}
