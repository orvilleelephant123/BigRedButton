﻿using UnityEngine;

public class SceneSetupTD : MonoBehaviour
{
    
	// Use this for initialization
	void Awake() 
	{
		SetupPlayer ();
	}

	/*
	 * Setup Player stuff for the scene
	 * new position, disable movement, etc
	 */ 
	void SetupPlayer()
	{
		// Set new player position
		Player player = GameObject.Find ("Player").GetComponent<Player> ();
		Vector3 playerPos = new Vector3 (25f, 0.5f, 15f);
		player.transform.position = playerPos;

		// Set player action list for player object
		player.actionList = GameObject.Find ("PlayerActionList").GetComponent<PlayerActionList> ();

		// Disable player movement
        player.GetComponent<IsoCharControl> ().enabled = false;
	}


}
