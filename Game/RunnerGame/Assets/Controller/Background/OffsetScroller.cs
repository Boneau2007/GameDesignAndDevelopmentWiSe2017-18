using UnityEngine;
using System.Collections;


/// <summary>
/// Scrolls a renderer's main texture horizontally over time.
/// </summary>
public class OffsetScroller : MonoBehaviour {

	/// <summary>
	/// Horizontal texture offset change per second.
	/// </summary>
	public float scrollSpeed;
	private Vector2 savedOffset;

	/// <summary>
	/// Saves the material's initial texture offset.
	/// </summary>
	void Start () {
		savedOffset = GetComponent<Renderer>().sharedMaterial.GetTextureOffset ("_MainTex");

	}

	/// <summary>
	/// Updates the horizontal texture offset to create a repeating scroll.
	/// </summary>
	void Update () {
		float x = Mathf.Repeat (Time.time * scrollSpeed, 1);
		Vector2 offset = new Vector2 (x, savedOffset.y);
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", offset);
	}

	/// <summary>
	/// Restores the material's original texture offset.
	/// </summary>
	void OnDisable () {
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex", savedOffset);
	}
}