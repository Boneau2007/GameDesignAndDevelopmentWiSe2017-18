using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Maintains reusable collections of game objects grouped by prefab.
/// </summary>
public class ObjectPooler : MonoBehaviour {
	public const int InitialPlatformPoolSize = 4;
	private readonly Dictionary<GameObject, List<GameObject>> pools = new Dictionary<GameObject, List<GameObject>>();
	private readonly HashSet<GameObject> managedObjects = new HashSet<GameObject>();
	private Transform inactivePoolRoot;

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
		if (pooledObject != null && pooledAmount > 0) {
			InitializePools (new[] { pooledObject }, pooledAmount);
		}
	}

	/// <summary>
	/// Preloads reusable instances for each configured platform prefab.
	/// </summary>
	/// <param name="prefabs">Platform prefabs to pool.</param>
	/// <param name="initialSize">Number of instances to preload per prefab.</param>
	public void InitializePools(IEnumerable<GameObject> prefabs, int initialSize) {
		if (pooledObjects == null) {
			pooledObjects = new List<GameObject> ();
		}
		if (inactivePoolRoot == null) {
			GameObject poolRoot = new GameObject ("Inactive Platform Pool");
			poolRoot.SetActive (false);
			poolRoot.transform.SetParent (transform, false);
			inactivePoolRoot = poolRoot.transform;
		}

		foreach (GameObject prefab in prefabs) {
			if (prefab == null) {
				Debug.LogError ("Cannot initialize a pool for a missing platform prefab.", this);
				continue;
			}
			if (pools.ContainsKey (prefab)) {
				continue;
			}

			List<GameObject> pool = new List<GameObject> ();
			pools.Add (prefab, pool);
			for (int i = 0; i < Mathf.Max (0, initialSize); i++) {
				pool.Add (CreatePooledObject (prefab));
			}
		}
	}

	/// <summary>
	/// Activates a reusable instance of the requested prefab at the given pose.
	/// </summary>
	public GameObject SpawnPooledObject(GameObject prefab, Vector3 position, Quaternion rotation) {
		if (prefab == null) {
			Debug.LogError ("Cannot spawn a pooled platform because its prefab is missing.", this);
			return null;
		}
		if (!pools.ContainsKey (prefab)) {
			InitializePools (new[] { prefab }, InitialPlatformPoolSize);
		}

		List<GameObject> pool = pools [prefab];
		GameObject pooledInstance = null;
		for (int i = 0; i < pool.Count; i++) {
			if (pool [i] != null && !pool [i].activeSelf) {
				pooledInstance = pool [i];
				break;
			}
		}

		if (pooledInstance == null) {
			pooledInstance = CreatePooledObject (prefab);
			pool.Add (pooledInstance);
		}

		pooledInstance.transform.SetParent (null, false);
		pooledInstance.transform.SetPositionAndRotation (position, rotation);
		PlatformDestroyer destroyer = pooledInstance.GetComponent<PlatformDestroyer> ();
		if (destroyer != null) {
			destroyer.SetObjectPooler (this);
		}
		pooledInstance.SetActive (true);
		return pooledInstance;
	}

	/// <summary>
	/// Gets an inactive pooled object, creating one if the pool has none available.
	/// </summary>
	/// <returns>An inactive object ready to be configured and activated.</returns>
	public GameObject GetPooledObject(){
		if (pooledObject == null) {
			Debug.LogError ("Cannot get a pooled object because no prefab is configured.", this);
			return null;
		}
		if (!pools.ContainsKey (pooledObject)) {
			InitializePools (new[] { pooledObject }, Mathf.Max (0, pooledAmount));
		}
		List<GameObject> pool = pools [pooledObject];
		for (int i = 0; i < pool.Count; i++) {
			if (pool [i] != null && !pool [i].activeSelf) {
				PreparePooledObject (pool [i]);
				return pool [i];
			}
		}
		GameObject obj = CreatePooledObject (pooledObject);
		pool.Add (obj);
		PreparePooledObject (obj);
		return obj;
	}

	/// <summary>
	/// Returns an instance managed by this pool to its inactive storage root.
	/// </summary>
	/// <returns>Whether the object belonged to this pool.</returns>
	public bool ReturnPooledObject(GameObject pooledInstance) {
		if (pooledInstance == null || !managedObjects.Contains (pooledInstance)) {
			return false;
		}

		pooledInstance.SetActive (false);
		pooledInstance.transform.SetParent (inactivePoolRoot, false);
		return true;
	}

	private GameObject CreatePooledObject(GameObject prefab) {
		GameObject pooledInstance = Instantiate (prefab, inactivePoolRoot);
		pooledInstance.SetActive (false);
		managedObjects.Add (pooledInstance);
		if (pooledObjects == null) {
			pooledObjects = new List<GameObject> ();
		}
		pooledObjects.Add (pooledInstance);
		return pooledInstance;
	}

	private void PreparePooledObject(GameObject pooledInstance) {
		pooledInstance.transform.SetParent (null, false);
		PlatformDestroyer destroyer = pooledInstance.GetComponent<PlatformDestroyer> ();
		if (destroyer != null) {
			destroyer.SetObjectPooler (this);
		}
	}
}
