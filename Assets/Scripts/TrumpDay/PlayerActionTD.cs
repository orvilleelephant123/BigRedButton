using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerActionTD
{
	[SerializeField]
	public int id;
	public string title;

	public Animation anim;

	public int baseDmg;
	public int actionType;
    public int encType;
}

