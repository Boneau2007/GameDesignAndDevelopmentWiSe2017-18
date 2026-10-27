using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player input, movement, jumping, health, and collision interactions.
/// </summary>
public class PlayerController : MonoBehaviour {

	private const float GroundCheckDistance = 0.1f;
	private const float GroundLandingSkin = 0.02f;
	private const float MinimumGroundNormalY = 0.5f;
	private const float StopSlideFrames = 3f;
	private const float CoyoteTime = 0.1f;
	private const float JumpBufferDuration = 0.1f;
	private const float JumpReleaseVelocityMultiplier = 0.5f;
	private const float FallingGravityMultiplier = 1.8f;
	private const float ApexGravityMultiplier = 0.55f;
	private const float ApexVelocityThreshold = 1.5f;
	private const float IceAcceleration = 18f;
	private const float IceDeceleration = 4f;
	private const float SnowballStompMomentumMultiplier = 0.2f;
	private const float MinimumLandingNormalY = 0.5f;
	private const float HitCooldownDuration = 1f;
	private const float HitFlashInterval = 0.08f;
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
	private float horizontalInput;
	private float coyoteTimeRemaining;
	private float jumpBufferRemaining;
	private float baseGravityScale;
	private bool jumpReleaseRequested;
	private bool groundedOnIce;
	private float airborneHorizontalMomentum;
	private float nextDamageTime;
	private SpriteRenderer[] playerRenderers;
	private readonly RaycastHit2D[] groundHits = new RaycastHit2D[8];

	private GameManager gameManager; 

	/// <summary>
	/// Caches required components and initializes the player's starting state.
	/// </summary>
	void Start ()
	{
		myRigidbody = GetComponent<Rigidbody2D> ();
		myRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		gameManager = FindAnyObjectByType<GameManager> ();
		myCollider = GetComponent<CapsuleCollider2D> (); 
		playerRenderers = GetComponentsInChildren<SpriteRenderer> ();
		isAlive = true;
		moveSpeed = gameManager.getCameraSpeed();
		baseGravityScale = myRigidbody.gravityScale;
	    jumpForce = 20;
		lifepoints = 5;
	}

	/// <summary>
	/// Reads jump input and updates the player's alive state.
	/// </summary>
	void Update () {
		horizontalInput = Input.GetAxisRaw("Horizontal");
		detectJump ();
		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow) ||
		    Input.GetKeyUp(KeyCode.W))
		{
			jumpReleaseRequested = true;
		}

		checkPlayerIsAlive ();

	}
	
	/// <summary>
	/// Updates grounded state and applies physics-based movement.
	/// </summary>
	void FixedUpdate () {

		PreventGroundTunneling ();
		checkGrounded ();
		moveSpeed = gameManager.getCameraSpeed () * 1.5f;
		movePlayer ();
		ApplyJumpGravity();

	}

	private void PreventGroundTunneling()
	{
		float verticalVelocity = myRigidbody.linearVelocity.y;
		if (verticalVelocity >= 0f)
		{
			return;
		}

		float fixedDeltaTime = Time.fixedDeltaTime;
		float gravity = Mathf.Abs (Physics2D.gravity.y * myRigidbody.gravityScale);
		float fallDistance = -verticalVelocity * fixedDeltaTime +
			0.5f * gravity * fixedDeltaTime * fixedDeltaTime;
		int hitCount = myCollider.Cast (Vector2.down, groundHits, fallDistance + GroundLandingSkin);
		for (int i = 0; i < hitCount; i++)
		{
			RaycastHit2D hit = groundHits [i];
			if (hit.collider == null ||
				!IsWalkableSurface (hit.collider) ||
				hit.normal.y < MinimumGroundNormalY)
			{
				continue;
			}

			float landingAdjustment = Mathf.Max (0f, hit.distance - GroundLandingSkin);
			if (landingAdjustment > 0f)
			{
				myRigidbody.position += Vector2.down * landingAdjustment;
			}

			myRigidbody.linearVelocity = new Vector2 (myRigidbody.linearVelocity.x, 0f);
			return;
		}
	}

    /// <summary>
    /// Applies damage for hazards, or slows a snowball when landed on from above.
    /// </summary>
    /// <param name="coll">Collision information for the contacted object.</param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag ("lava"))
        {
			TakeDamage ();
        }
        else if (coll.gameObject.CompareTag ("snowball"))
        {
			if (IsLandingOnSnowball (coll))
			{
				Rigidbody2D snowballRigidbody = coll.rigidbody;
				if (snowballRigidbody != null)
				{
					snowballRigidbody.linearVelocity *= SnowballStompMomentumMultiplier;
					snowballRigidbody.angularVelocity *= SnowballStompMomentumMultiplier;
				}
				return;
			}

            Destroy(coll.gameObject);
			TakeDamage ();
        }
       
    }

	private void TakeDamage()
	{
		if (lifepoints <= 0 || Time.time < nextDamageTime)
		{
			return;
		}

		lifepoints--;
		nextDamageTime = Time.time + HitCooldownDuration;
		StartCoroutine (FlashAfterHit ());
	}

	private System.Collections.IEnumerator FlashAfterHit()
	{
		float elapsed = 0f;
		bool visible = true;
		while (elapsed < HitCooldownDuration)
		{
			visible = !visible;
			for (int i = 0; i < playerRenderers.Length; i++)
			{
				if (playerRenderers [i] != null)
				{
					playerRenderers [i].enabled = visible;
				}
			}

			yield return new WaitForSeconds (HitFlashInterval);
			elapsed += HitFlashInterval;
		}

		for (int i = 0; i < playerRenderers.Length; i++)
		{
			if (playerRenderers [i] != null)
			{
				playerRenderers [i].enabled = true;
			}
		}
	}

	private bool IsLandingOnSnowball(Collision2D collision)
	{
		if (myRigidbody.linearVelocity.y > 0f)
		{
			return false;
		}

		Bounds playerBounds = myCollider.bounds;
		float lowerHalfTop = playerBounds.min.y + playerBounds.size.y * 0.5f;
		for (int i = 0; i < collision.contactCount; i++)
		{
			ContactPoint2D contact = collision.GetContact (i);
			if (contact.normal.y >= MinimumLandingNormalY && contact.point.y <= lowerHalfTop)
			{
				return true;
			}
		}

		return false;
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
		if (grounded)
		{
			if (groundedOnIce && Mathf.Abs (airborneHorizontalMomentum) > 0.01f)
			{
				myRigidbody.linearVelocity = new Vector2 (airborneHorizontalMomentum,
					myRigidbody.linearVelocity.y);
			}
			airborneHorizontalMomentum = 0f;
		}
		else if (Mathf.Abs (myRigidbody.linearVelocity.x) > 0.01f)
		{
			airborneHorizontalMomentum = myRigidbody.linearVelocity.x;
		}

		float horizontalVelocity;
		if (groundedOnIce)
		{
			float currentVelocity = myRigidbody.linearVelocity.x;
			if (horizontalInput == 0f)
			{
				horizontalVelocity = currentVelocity;
			}
			else if (currentVelocity * horizontalInput < 0f)
			{
				horizontalVelocity = Mathf.MoveTowards (currentVelocity, 0f,
					IceDeceleration * Time.fixedDeltaTime);
			}
			else
			{
				float targetSpeed = Mathf.Max (moveSpeed, Mathf.Abs (currentVelocity));
				horizontalVelocity = Mathf.MoveTowards (currentVelocity, horizontalInput * targetSpeed,
					IceAcceleration * Time.fixedDeltaTime);
			}
		}
		else if (horizontalInput != 0f)
		{
			horizontalVelocity = horizontalInput * moveSpeed;
		}
		else
		{
			horizontalVelocity = grounded
				? Mathf.MoveTowards (myRigidbody.linearVelocity.x, 0f, moveSpeed / StopSlideFrames)
				: 0f;
		}
		myRigidbody.linearVelocity = new Vector2(horizontalVelocity, myRigidbody.linearVelocity.y);
		animator.SetBool("Walking", Mathf.Abs(horizontalVelocity) > 0.01f);

		if (grounded)
		{
			coyoteTimeRemaining = CoyoteTime;
		}
		else
		{
			coyoteTimeRemaining = Mathf.Max(0f, coyoteTimeRemaining - Time.fixedDeltaTime);
		}

		if (wantsToJump)
		{
			jumpBufferRemaining = JumpBufferDuration;
			wantsToJump = false;
		}
		else
		{
			jumpBufferRemaining = Mathf.Max(0f, jumpBufferRemaining - Time.fixedDeltaTime);
		}

		if (jumpBufferRemaining > 0f && coyoteTimeRemaining > 0f)
		{
			float jumpHorizontalVelocity = horizontalInput * moveSpeed;
			myRigidbody.linearVelocity = new Vector2(jumpHorizontalVelocity, jumpForce);
			jumpBufferRemaining = 0f;
			coyoteTimeRemaining = 0f;
			grounded = false;
		}

		if (jumpReleaseRequested)
		{
			if (myRigidbody.linearVelocity.y > 0f)
			{
				myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x,
					myRigidbody.linearVelocity.y * JumpReleaseVelocityMultiplier);
			}

			jumpReleaseRequested = false;
		}
	}

	private void ApplyJumpGravity()
	{
		float verticalVelocity = myRigidbody.linearVelocity.y;
		if (verticalVelocity > 0f && verticalVelocity <= ApexVelocityThreshold)
		{
			myRigidbody.gravityScale = baseGravityScale * ApexGravityMultiplier;
		}
		else if (verticalVelocity < 0f)
		{
			myRigidbody.gravityScale = baseGravityScale * FallingGravityMultiplier;
		}
		else
		{
			myRigidbody.gravityScale = baseGravityScale;
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
		groundedOnIce = false;
		int hitCount = myCollider.Cast (Vector2.down, groundHits, GroundCheckDistance);
		for (int i = 0; i < hitCount; i++) {
			RaycastHit2D hit = groundHits [i];
			if (hit.collider != null &&
				IsWalkableSurface (hit.collider) &&
				hit.normal.y >= MinimumGroundNormalY) {
				grounded = true;
				groundedOnIce = (ice.value & (1 << hit.collider.gameObject.layer)) != 0;
				return;
			}
		}
	}

	private bool IsWalkableSurface(Collider2D surface)
	{
		int layerBit = 1 << surface.gameObject.layer;
		return ((whatIsGround.value | lava.value | ice.value) & layerBit) != 0 ||
			surface.CompareTag ("snowball");
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
