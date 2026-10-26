using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns hazards when the hazard-generation point reaches the configured position.
/// </summary>
public class HazardController : MonoBehaviour
{

    private GameManager gameManager;
    /// <summary>
    /// Position that triggers hazard generation.
    /// </summary>
    public Transform generationPoint;
    private int force = 500;
    /// <summary>
    /// Hazard prefab to instantiate.
    /// </summary>
    public GameObject hazards;

    private int spawnChance;

    /// <summary>
    /// Finds the game manager used to determine the score and player position.
    /// </summary>
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //Random.InitState((int)Time.time);
    }

    /// <summary>
    /// Checks whether a hazard should be spawned.
    /// </summary>
    void Update()
    {
        spawnHazard();
    }


    /// <summary>
    /// Spawns and propels a hazard when the generator reaches the generation point.
    /// The spawn probability increases with the current score.
    /// </summary>
    public void spawnHazard()
    {
        if (transform.position.x < generationPoint.position.x)
        {
            spawnChance = Random.Range(1, 1000);
            if (spawnChance <= (gameManager.getScore() / 1000))
            {
				Debug.Log("Spawning Snowball!");
                transform.position = new Vector2(generationPoint.position.x + 1, generationPoint.position.y);
                var clone = Instantiate(hazards, transform.position, transform.rotation);
                clone.AddComponent<Rigidbody2D>();
                clone.GetComponent<Rigidbody2D>().gravityScale = 0.3f;
                //Relative postion from player to camera
                var player_to_cam = new Vector2(gameManager.getPlayerPositionX(), gameManager.getPlayerPositionY());
                //Relative postion from spawnPoint to player
                var gen_to_player = player_to_cam - (Vector2)transform.position;
                //Debug.Log ("Score:  " + player_to_cam);
                clone.GetComponent<Rigidbody2D>().AddForce(gen_to_player.normalized * force);

            }
            Debug.Log ("SpawnChance:  " + spawnChance);
        }
    }
}