﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleSceneSetup : MonoBehaviour {

    private List<ScheduleItem> items;
    public Transform canvasTrans;
    public Font itemFont;
    public int curTime;

    // Use this for initialization
    void Start()
    {
        // The default time for starting the day is 8am
        curTime = 800;

        items = new List<ScheduleItem>();

        // Make some schedule items
        ScheduleItem sa = new ScheduleItem
        {
            title = "Fox and Friends",
            durInMin = 60,
            numEnemies = 3
        };

        ScheduleItem sb = new ScheduleItem
        {
            title = "Press conference",
            durInMin = 30,
            numEnemies = 6
        };

        ScheduleItem sc = new ScheduleItem
        {
            title = "Intelligence briefing",
            durInMin = 60,
            numEnemies = 4
        };

        items.Add(sa);
        items.Add(sb);
        items.Add(sc);

        for (int i=0;i<items.Count;i++)
        {
            GameObject obj = CreateTextForItem(items[i], curTime);
            curTime = AddDurToTime(curTime, items[i].durInMin);

            // Get location for pHP
            obj.transform.localPosition = new Vector3(0, (items.Count-i) * 100);
        }
    }
	

	void Update ()
    {
		
	}

    // Consider when dur >= 60
    private int AddDurToTime(int t, int dur)
    {
        int hours = dur / 60;
        int minutes = dur % 60;

        // Initialize
        int result = t;

        // Hours
        // Were using military time to multiply by 100
        result += (hours * 100);

        // Minutes
        result += minutes;        

        return result;
    }

    // Time is the actual time right now
    // range is [800-2000]
    private string GetTimeString(int time)
    {
        string result = "";

        int hours = time / 100;
        int minutes = time % 100;
        bool am = true;


        // Check if minutes is >60
        if(minutes >= 60)
        {
            minutes -= 60;
            hours++;
        }

        // Check for PM
        if (hours > 12)
        {
            hours -= 12;
            am = false;
        }

        result = string.Format("{0}:{1} {2}", hours.ToString("00"), minutes.ToString("00"), am ? "AM" : "PM");

        return result;
    }

    private GameObject CreateTextForItem(ScheduleItem item, int curTime)
    {
        int t = AddDurToTime(curTime, item.durInMin);
        string tStr = GetTimeString(t);
        GameObject textObject = new GameObject(string.Format("{0} Text", item.title));

        // Set parent tf
        textObject.transform.SetParent(canvasTrans);

        // Add a rect transform to the object
        RectTransform rtf = textObject.AddComponent<RectTransform>();
        //rtf.anchoredPosition = new Vector2(0,0);
        rtf.sizeDelta = new Vector2(500, 50);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = string.Format("{0} - {1}", tStr, item.title);
        text.fontSize = 36;
        text.color = Color.black;
        text.alignment = TextAnchor.UpperCenter;
        text.font = itemFont;


        return textObject;
    }
}
