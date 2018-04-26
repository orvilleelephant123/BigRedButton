using UnityEngine;
using System.Collections.Generic;
using System;

//[Serializable]
public class PlayerAction
{
	[SerializeField]
	public int id;
	public string title;

	public Animation anim;

	public int baseDmg;
	public int actionType;
}

