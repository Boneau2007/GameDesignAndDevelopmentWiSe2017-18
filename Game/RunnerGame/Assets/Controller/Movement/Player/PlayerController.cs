using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float moveSpeed; 
	public float jumpForce; 
	public int lifepoints; 

	private Rigidbody2D myRigidbody;


	public bool grounded;
	public bool isAlive;
    public bool gameIsRunning;

    public Animator animator;
	public LayerMask whatIsGround;
	public LayerMask deathZone;
	public LayerMask lava; 
	public LayerMask ice; 
	private Collider2D myCollider;
	private bool wantsToJump = false;

	private GameManager gameManager; 

	// Use this for initialization
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

	void Update () {
		detectJump ();
		checkPlayerIsAlive ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		checkGrounded ();
		moveSpeed = gameManager.getCameraSpeed () * 1.2f ;
		movePlayer ();

	}

    /// <summary>
    /// Raises the collision enter2 d event, when the player collides with a lava block
    /// </summary>
    /// <param name="coll">Coll.</param>
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
    /// Gets the Position in x direction. 
    /// The method is called from the GameManager and grants access to the player position.
    /// </summary>
    /// <returns>The Position in X.</returns>
    public float getPositionX()
    {
        return myRigidbody.position.x;
    }

    /// <summary>
    /// Gets the Position in y direction. 
    /// The method is called from the GameManager and grants access to the player position.
    /// </summary>
    /// <returns>The Position in Y.</returns>
    public float getPositionY()
    {
        return myRigidbody.position.y;
    }

    public float getSpeed() {
		return moveSpeed;
	}

	public bool isPlayerAlive(){
		return this.isAlive;
	}

	/// <summary>
	/// Checks the player is alive.
	/// Later can be shorter -> isAlive = !Physics2D.IsTouchingLayers (myCollider, deathZone)
	/// For testing purpose we need the log statement
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
	/// Moves the player.
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
				myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce);
			}
			wantsToJump = false;
		}
	}

	/// <summary>
	/// Detects the jump. When the customer presses SPACE | W | UP_ARROW, the player will jump.
	/// </summary>
	private void detectJump(){
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ) {
			wantsToJump = true;
		}
	}
		
	/// <summary>
	/// Checks if the player is grounded and can jump. 
	/// On normal ground or lava,the player is allowed to jump. 
	/// </summary>
	private void checkGrounded(){
		if (Physics2D.IsTouchingLayers (myCollider, whatIsGround) || Physics2D.IsTouchingLayers (myCollider, lava) || Physics2D.IsTouchingLayers (myCollider, ice)) {
			grounded = true;
		} else {
			grounded = false; 
		}
	}

	private void increaseHealth(){
		if(lifepoints < 5){
			lifepoints++;
		}
	}

	/// <summary>
	/// Gets the life points.
	/// </summary>
	/// <returns>The life points.</returns>
	public int getLifePoints() {
		return this.lifepoints;
	}
}
