﻿using UnityEngine;
using UnityEngine.UI;

public class FightSceneSetupTD : MonoBehaviour
{

    int iEnc;
    EnemyTD enemyPrefab;
    public Transform canvasTrans;
    
	// Use this for initialization
	void Awake() 
	{
        iEnc = 0;
	}

    private void Start()
    {
        SetupPlayer();
    }


    /*
	 * Setup Player stuff for the scene
	 * new position, disable movement, etc
	 */
    void SetupPlayer()
	{
		// Set new player position
		PlayerTD player = GameObject.Find ("Player").GetComponent<PlayerTD> ();
		Vector3 playerPos = new Vector3 (25f, 0.5f, 15f);
		player.transform.position = playerPos;

		// Set player action list for player object
		player.actionList = GameObject.Find ("PlayerActionList").GetComponent<PlayerActionListTD> ();

        // Prune list?
        player.GetPrunedList(iEnc);
	}
    
}
