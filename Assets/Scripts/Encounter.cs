﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.WSA;
using UDB;
using System.Xml.Schema;


public class Encounter : MonoBehaviour {

	[SerializeField]
	public Dropdown dropDown;


	public bool active;
	public Enemy enemy;
	public Player player;
	private bool choiceMade;


	// Use this for initialization
	void Awake () 
	{
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/TopicList").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);
	}

	void ApplyPlayerAction()
	{
		
	}

	void DisplayEnemyResponse()
	{
		
	}

	private void DropdownValueChanged(int choice)
	{
		Debug.Log ("In DropdownValueChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

	public void init(Player p, Enemy e)
	{
		player = p;
		enemy = e;
		dropDown.ClearOptions ();

		List<string> topicStrs = player.GetTopicStrings ();
		topicStrs.Insert (0, "Make a selection");
		dropDown.AddOptions (topicStrs);
	}

	private void setOptions()
	{
		List<string> topicStrs = player.GetTopicStrings ();
		topicStrs.Insert (0, "Make a selection");
		dropDown.ClearOptions ();
		dropDown.AddOptions (topicStrs);
	}

	public void checkNewTopics()
	{
		foreach(int i_topics in enemy.lastResponse.topicsToObtain)
		{
			Debug.Log ("i_topics: " + i_topics);
			if(player.topics.Contains(i_topics))
			{
				Debug.Log ("Player already has topic i");
			}
			else
			{
				Debug.Log ("Player does not have topic "+i_topics);
				player.topics.Add (i_topics);
			}
		}
	}

	public void Update()
	{
		if(enemy.hp > 0)
		{
			if(choiceMade)
			{
				// Player's turn
				// Get the choice from the Dropdown
				// Subtract 1 because the first index is "Make a selection"
				int choice = dropDown.value-1;
				//Debug.Log ("choice: " + player.topics[choice].title);

				// Apply topic to enemy
				enemy.ApplyTopic (player.GetTopic(choice));

				// Choose and display enemy response
				DisplayEnemyResponse ();

				// Check enemy response for new topics
				checkNewTopics ();

				setOptions ();

				// Reset dropdown
				dropDown.value = 0;

				// Reset choiceMade
				choiceMade = false;
			}
		}
		else
		{
			enemy.gameObject.SetActive (false);
			GameObject.Find ("Enemy Text").SetActive (false);
			player.GetComponent<IsoCharControl> ().enabled = true;
			Destroy (gameObject);
		}
	}
	public void go()
	{
		Debug.Log ("Encounter started with "+enemy.enemyName);

		Debug.Log (String.Format ("Enemy: {0}", enemy.initEncounter));

		choiceMade = false;
	}
}
