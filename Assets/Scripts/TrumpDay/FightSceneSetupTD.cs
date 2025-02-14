﻿using UnityEngine;
using UnityEngine.UI;

public class FightSceneSetupTD : MonoBehaviour
{
    // Prefabs
    public EnemyTD enemyPrefab;
    public EnemyTD pressPrefab;
    public EnemyTD houseRepPrefab;
    public EnemyTD houseDemPrefab;
    public EnemyTD senateRepPrefab;
    public EnemyTD senateDemPrefab;
    public EnemyTD foxAnchorPrefab;
    public LineRenderer targetLinePrefab;

    public Transform canvasTrans;


    // Enemy stuff
    public Transform enemyLocRef;
    public Font enemyHPFont;

    // Camera to use camera.WorldToScreen
    public Camera isoCamera;


    // Use this for initialization
    void Awake() 
	{
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
        player.GetPrunedList((int)PersistentData.nextItem.type);
        SetPlayerTextPosition();

        CreateEnemyObjects();
	}

    EnemyTD MakeEnemyAtPosition(Vector3 p)
    {
        // Check the enemy type for the schedule item
        EnemyTD result = null;
        Debug.Log("In MakeEnemyAtPosition, enemyType: " + PersistentData.nextItem.enemyType);
        switch (PersistentData.nextItem.enemyType)
        {
            case Common.EnemyType.FOX_ANCHOR:
                result = Instantiate(foxAnchorPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            case Common.EnemyType.HOUSE_D:
                result = Instantiate(houseDemPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            case Common.EnemyType.HOUSE_R:
                result = Instantiate(houseRepPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            case Common.EnemyType.PRESS:
                result = Instantiate(pressPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            case Common.EnemyType.SENATE_D:
                result = Instantiate(senateDemPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            case Common.EnemyType.SENATE_R:
                result = Instantiate(senateRepPrefab, p, Quaternion.identity) as EnemyTD;
                break;
            default:
                result = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;
                break;
        }

        return result;
    }



    /*
	 * Create objects for each Enemy
	 */
    void CreateEnemyObjects()
    {
        // Get the total number of enemies from somewhere based on encounter
        int n = Random.Range(1, PersistentData.nextItem.numEnemies);

        int x_offset = 0;
        int z_offset = 0;

        // Make rows of enemies, 2 per row
        // Start at 2 to not run into issues with i=0 or 1
        // i/2 for i=0 and i=1 will be equal locations, and doing i+1 also runs into problems with making rows
        for (int i = 2; i <= n + 1; i++)
        {
            if (i % 2 == 0)
            {
                x_offset = (i / 2) * 4;
                z_offset = (i / 2) * 4;
            }
            else
            {
                x_offset = -(i / 2) * 4;
                z_offset = (i / 2) * 4;
            }
            //Debug.Log(string.Format("i: {0} i/2: {1} x_offset: {2} z_offset: {3}", i, i / 2, x_offset, z_offset));

            // Create vector for position and instantiate an enemy
            Vector3 p = new Vector3(enemyLocRef.position.x + x_offset, enemyLocRef.position.y, enemyLocRef.position.z + z_offset);

            // Make the enemy type for the schedule item
            EnemyTD e = MakeEnemyAtPosition(p);

            // Add enemy to list
            FightManager.self.enemies.Add(e);
        }
        
        // Sort the enemies array by X position
        // This makes the targeting more intuitive 
        // so that pressing left and right goes to enemy to the left or right
        FightManager.self.enemies.Sort();

        // Create other stuff for enemies (hp texts, target lines, etc)
        foreach(EnemyTD e in FightManager.self.enemies)
        {
            // Create a text object based on the enemy
            GameObject t = CreateEnemyHPText(e, canvasTrans, string.Format("HP: {1}", e.name, e.hp));

            // Get location for pHP
            Vector3 pHP = isoCamera.WorldToScreenPoint(e.gameObject.transform.position);
            t.transform.position = pHP;

            // Add the text object to list
            FightManager.self.enemiesHP.Add(t.GetComponent<Text>());

            // Make line to show if enemy is being targeted
            FightManager.self.targetLines.Add(CreateEnemyTargetLines(e));
        }
    }

    GameObject CreateEnemyHPText(EnemyTD enemy, Transform canvas_transform, string text_to_print)
    {
        // Create object and set parent
        GameObject textObject = new GameObject(string.Format("{0} HP", enemy.name));
        textObject.transform.SetParent(canvas_transform);

        // Make a rect transform
        RectTransform trans = textObject.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.y);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = 24;
        text.color = Color.red;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = enemyHPFont;

        return textObject;
    }

    LineRenderer CreateEnemyTargetLines(EnemyTD enemy)
    {
        // Create the line renderer
        Vector3 p = new Vector3(0, 0.25f, 1.25f);
        LineRenderer l = Instantiate(targetLinePrefab, p, Quaternion.identity) as LineRenderer;
        l.transform.SetParent(enemy.transform);
        l.useWorldSpace = false;

        // Set the local position
        l.transform.localPosition = p;

        // Set to be inactive initially
        l.gameObject.SetActive(false);

        return l;
    }


    private void SetPlayerTextPosition()
    {
        // Set offsets
        int x_offsetTurn = 40;
        int y_offsetTurn = -55;

        /*
		 * Player
		 */

        // Set playerHP text to player location
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();
        Vector3 playerPos = isoCamera.WorldToScreenPoint(player.transform.position);
        FightManager.self.playerHp.transform.position = playerPos;

        // Player turn indicator
        playerPos.x += x_offsetTurn;
        playerPos.y += y_offsetTurn;
        FightManager.self.playerTurnText.transform.position = playerPos;
    }

}
