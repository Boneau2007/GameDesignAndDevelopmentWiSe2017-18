using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player input, movement, jumping, health, and collision interactions.
/// </summary>
public class PlayerController : MonoBehaviour {

	private const float GroundCheckDistance = 0.1f;
	private const float MinimumGroundNormalY = 0.5f;
	/// <summary>
	/// Player's horizontal movement speed.
	/// </summary>
	public float moveSpeed; 
	/// <summary>
	/// Upward velocity applied when the player jumps.
	/// </summary>
	public float jumpForce; 
	/// <summary>
	/// Player's remaining life points.
	/// </summary>
	public int lifepoints; 

	private Rigidbody2D myRigidbody;


	/// <summary>
	/// Indicates whether the player is standing on a walkable surface.
	/// </summary>
	public bool grounded;
	/// <summary>
	/// Indicates whether the player is alive.
	/// </summary>
	public bool isAlive;
    /// <summary>
    /// Indicates whether the game is running.
    /// </summary>
    public bool gameIsRunning;

    /// <summary>
    /// Animator that controls the player's animations.
    /// </summary>
    public Animator animator;
	/// <summary>
	/// Layers that count as ordinary ground for jumping.
	/// </summary>
	public LayerMask whatIsGround;
	/// <summary>
	/// Layers that cause the player to die on contact.
	/// </summary>
	public LayerMask deathZone;
	/// <summary>
	/// Layers that can be used as walkable surfaces; collision damage is handled by object tags.
	/// </summary>
	public LayerMask lava; 
	/// <summary>
	/// Layers that can be used as walkable surfaces for jumping.
	/// </summary>
	public LayerMask ice; 
	private Collider2D myCollider;
	private bool wantsToJump = false;
	private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];

	private GameManager gameManager; 

	/// <summary>
	/// Caches required components and initializes the player's starting state.
	/// </summary>
	void Start ()
	{
		myRigidbody = GetComponent<Rigidbody2D> ();
		gameManager = FindObjectOfType<GameManager> ();
		myCollider = GetComponent<CapsuleCollider2D> (); 
		isAlive = true;
		moveSpeed = gameManager.getCameraSpeed();
	    jumpForce = 20;
		lifepoints = 5;
	}

	/// <summary>
	/// Reads jump input and updates the player's alive state.
	/// </summary>
	void Update () {
		detectJump ();
		checkPlayerIsAlive ();

	}
	
	/// <summary>
	/// Updates grounded state and applies physics-based movement.
	/// </summary>
	void FixedUpdate () {

		checkGrounded ();
		moveSpeed = gameManager.getCameraSpeed () * 1.2f ;
		movePlayer ();

	}

    /// <summary>
    /// Applies damage when the player collides with lava or a snowball.
    /// </summary>
    /// <param name="coll">Collision information for the contacted object.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (lifepoints > 0 && coll.gameObject.tag == "lava")
        {
            lifepoints--;
        }
        else if (lifepoints > 0 && coll.gameObject.tag == "snowball")
        {
            Destroy(coll.gameObject);
            lifepoints--;
        }
       
    }


	/// <summary>
	/// Handles collectible pickups and health-restoring cookies.
	/// </summary>
	/// <param name="collect">The trigger collider that the player entered.</param>
	void OnTriggerEnter2D(Collider2D collect){
		if(collect.gameObject.CompareTag ("collectable")){
			Destroy (collect.gameObject);
			gameManager.addPointsFromCollectable (100);
		}
		if (collect.gameObject.CompareTag ("cookie")) {
			increaseHealth ();
			Destroy (collect.gameObject);
		}
	}

    /// <summary>
    /// Gets the player's horizontal world position.
    /// </summary>
    /// <returns>The player's X coordinate.</returns>
    public float getPositionX()
    {
        return myRigidbody.position.x;
    }

    /// <summary>
    /// Gets the player's vertical world position.
    /// </summary>
    /// <returns>The player's Y coordinate.</returns>
    public float getPositionY()
    {
        return myRigidbody.position.y;
    }

	/// <summary>
	/// Gets the player's current horizontal movement speed.
	/// </summary>
	/// <returns>The player speed in world units per second.</returns>
    public float getSpeed() {
		return moveSpeed;
	}

	/// <summary>
	/// Reports whether the player is alive.
	/// </summary>
	/// <returns><see langword="true"/> if the player is alive; otherwise, <see langword="false"/>.</returns>
	public bool isPlayerAlive(){
		return this.isAlive;
	}

	/// <summary>
	/// Marks the player dead if they touch a death-zone layer or lose all life points.
	/// </summary>
	private void checkPlayerIsAlive(){
		if (Physics2D.IsTouchingLayers (myCollider, deathZone)) {
			lifepoints = 0;
			isAlive = false;
			//Debug.Log ("Player died! You failed!!!");
		}
		if (lifepoints <= 0) {
			isAlive = false;
		}
	}


	/// <summary>
	/// Applies horizontal input, updates the walking animation, and performs a buffered jump.
	/// </summary>
	private void movePlayer(){

		var x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
		this.transform.Translate(x, 0, 0);
		// Vertricale bewegung
		// var z = Input.GetAxis("Vertical") * Time.deltaTime * jumpForce;
		// this.transform.Translate(z,0,0);

		if (x != 0)
		{
			animator.SetBool("Walking", true);
		} else {
			animator.SetBool("Walking", false);
		}

		if (wantsToJump){
			if (grounded) {
				myRigidbody.linearVelocity = new Vector2 (myRigidbody.linearVelocity.x, jumpForce);
			}
			wantsToJump = false;
		}
	}

	/// <summary>
	/// Buffers a jump request when Space, Up Arrow, or W is pressed.
	/// </summary>
	private void detectJump(){
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ) {
			wantsToJump = true;
		}
	}
		
	/// <summary>
	/// Determines whether a walkable surface is beneath the player, rather than beside them.
	/// Ordinary ground, lava, and ice layers are considered walkable.
	/// </summary>
	private void checkGrounded(){
		grounded = false;
		LayerMask walkableLayers = whatIsGround | lava | ice;
		int hitCount = myCollider.Cast (Vector2.down, groundHits, GroundCheckDistance);
		for (int i = 0; i < hitCount; i++) {
			RaycastHit2D hit = groundHits [i];
			if (hit.collider != null &&
				(walkableLayers.value & (1 << hit.collider.gameObject.layer)) != 0 &&
				hit.normal.y >= MinimumGroundNormalY) {
				grounded = true;
				return;
			}
		}
	}

	/// <summary>
	/// Restores one life point without exceeding the player's maximum.
	/// </summary>
	private void increaseHealth(){
		if(lifepoints < 5){
			lifepoints++;
		}
	}

	/// <summary>
	/// Gets the player's remaining life points.
	/// </summary>
	/// <returns>The current life-point count.</returns>
	public int getLifePoints() {
		return this.lifepoints;
	}
}
