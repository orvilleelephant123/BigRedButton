﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NUnit.Framework;

public class Enemy : MonoBehaviour {

	public Common.EnemyType enemyType;
	public Common.MovementType movementType;


	public float hp;
	public String enemyName;
	public String initEncounter;
	public Text textbox;
	public bool collidingWithPlayer;

	private Transform player;
	private List<Common.TopicType> weakTo;
	private List<Common.TopicType> strongTo;
	private float weakMod;
	private float strongMod;


	private BoxCollider boxCollider;
	private Rigidbody rb;

	private TopicList topicList;
	[SerializeField]
	public Dictionary<Topic, EnemyResponse> responses;
	public EnemyResponse lastResponse;

	public Vector3 start;
	public Vector3 goal;
	public Vector3 goalOrig;
	public List<Vector3> movementDetails;

	public float speed;
	public float goalThreshold;

	public void AddResponse(EnemyResponse er)
	{
		responses.Add (topicList.list [er.i_topic], er);
	}

	void Awake()
	{
		hp = 10f;
		weakMod = 3f;
		strongMod = -3f;
		collidingWithPlayer = false;
		speed = 3f;
		goalThreshold = 0.15f;

		weakTo = new List<Common.TopicType> ();
		strongTo = new List<Common.TopicType> ();
		weakTo.Add (Common.TopicType.HOSTILE_TALK);
		strongTo.Add (Common.TopicType.SHOP_TALK);

		// Get and store transform of the player
		player = GameObject.FindGameObjectWithTag ("Player").transform;

		textbox = GameObject.Find ("Enemy Text").GetComponent<Text> ();
		topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();

		boxCollider = GetComponent<BoxCollider> ();
		rb 			= GetComponent<Rigidbody> ();

		responses = new Dictionary<Topic, EnemyResponse> ();

		start = new Vector3 ();
		goal = new Vector3 ();
		movementDetails = new List<Vector3> ();
	}

	// Use this for initialization
	void Start () 
	{
	}


	
	// Update is called once per frame
	void Update () 
	{
		Move ();
	}

	void OnCollisionEnter(Collision coll)
	{
		Debug.Log ("Hey we're in collision!");
		if(coll.gameObject.name == "Player" && !collidingWithPlayer)
		{
			textbox.text = String.Format ("{0}, I'm {1}", initEncounter, enemyName);
			textbox.gameObject.SetActive (true);
			EncounterEventManager.TriggerEvent (Common.ENC_EVENT_STR, this);
			collidingWithPlayer = true;
		}
		else
		{
			Debug.Log (String.Format("coll.gameObject.name == {0}", coll.gameObject.name));
			collidingWithPlayer = false;
		}
	}

	private float CalculateDmg(Topic topic)
	{
		float result = topic.baseDmg;

		if(Common.weakTo[(int)enemyType].Contains(topic.type))
		{
			Debug.Log ("Adding weakMod");
			result += weakMod;
		}
		else if(Common.strongTo[(int)enemyType].Contains(topic.type))
		{
			Debug.Log ("Adding strongMod");
			result += strongMod;
		}

		Debug.Log ("Damage: " + result);
		return result;
	}


	public void ApplyTopic(Topic topic)
	{
		/*
		 *  Determine loss of hp
		 */
		hp -= CalculateDmg (topic);
		Debug.Log ("hp: " + hp);

		// Set response
		responses.TryGetValue (topic, out lastResponse);
		if(lastResponse.response.CompareTo("") == 0)
		{
			lastResponse = Common.enemyDefaultResp;
		}
	}

	public void Move()
	{
		// Get vector between goal and start
		Vector3 diff = goal - transform.position;
		Debug.Log (String.Format ("diff: {0} diff.mag: {1} goalThreshold: {2}", diff.ToString(), diff.magnitude, goalThreshold));

		// Check that we are greater than some threshold
		// TODO: Fix jittering when it gets close to goal
		if (diff.magnitude-transform.position.y > goalThreshold)
		{
			// Get the correct axes to move on
			Vector3 forward = Vector3.Normalize (Camera.main.transform.forward);
			Vector3 right = Vector3.Normalize (Camera.main.transform.right);
	
			// Project the diff vector onto the movement/camera axes
			Vector3 a = Vector3.Project (diff, forward);
			Vector3 b = Vector3.Project (diff, right);
			Debug.Log (String.Format ("a: {0} b: {0}", a.ToString (), b.ToString ()));

			// Get a heading vector
			Vector3 heading = Vector3.Normalize (a + b);

			// Get movement vector
			Vector3 moveVec = heading * speed * Time.deltaTime;
			Debug.Log (String.Format ("moveVec: {0}", moveVec.ToString ()));

			// Don't increase y value
			moveVec.y = 0;

			// Apply movement
			transform.position += moveVec;
		}
		// Else if it is at the goal (i.e. not at the start)
		else if( (goal - start).magnitude > 0.1 && movementDetails.Count > 0)
		{
			Debug.Log ("Changing goal = start");
			goal = start;
		}
		// Else if it's back at the start
		else
		{
			Debug.Log ("goal = goalOrig");
			goal = goalOrig;
		}
	}

	public override string ToString()
	{
		return String.Format ("Enemy:\n\tName: {0}\n\tHP: {1}", enemyName, hp.ToString ());
	}



}
