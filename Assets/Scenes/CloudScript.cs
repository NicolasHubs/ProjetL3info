using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

    public float speed;
    public Color colorNight;
    public Color colorDay;
    public Color colorGray;
    private Color colorPresent;

    private float frac;
    private Color color;
	private Vector3 playerPosition;

    private void Start()
    {
		if (CycleJourNuit.timeOfDay > 21600 && CycleJourNuit.timeOfDay <= 64800)
        {
            GetComponent<SpriteRenderer>().color = colorDay;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = colorNight;
        }
    }

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(transform.position.x + 0.01f * speed, transform.position.y, transform.position.z);
		playerPosition =  GameObject.FindGameObjectWithTag("Player").transform.position;
		if (transform.position.x >= playerPosition.x + 10f)
        {
            Destroy(gameObject);
        }

        if (!WeatherCycle.weatherChanging || (WeatherCycle.weatherChanging && WeatherCycle.type == 1))
        {
			if (CycleJourNuit.timeOfDay >= 57000 && CycleJourNuit.timeOfDay < 62000)
            {
				frac = (CycleJourNuit.timeOfDay - 57000) / 5000;
                color = Color.Lerp(colorDay, colorNight, frac);
                GetComponent<SpriteRenderer>().color = color;
            }
			if (CycleJourNuit.timeOfDay >= 23000 && CycleJourNuit.timeOfDay < 29000)
            {
				frac = (CycleJourNuit.timeOfDay - 23000) / 6000;
                color = Color.Lerp(colorNight, colorDay, frac);
                GetComponent<SpriteRenderer>().color = color;
            }
        } else
        {
            if (Time.time - WeatherCycle.initTime < 25)
            {
                color = Color.Lerp(GetComponent<SpriteRenderer>().color, colorGray, Time.deltaTime);
                GetComponent<SpriteRenderer>().color = color;
                colorPresent = GetComponent<SpriteRenderer>().color;
            }
            else if (Time.time - WeatherCycle.initTime > 25 && Time.time - WeatherCycle.initTime <= 30)
            {
				frac = (CycleJourNuit.timeOfDay - WeatherCycle.initChangeBackTime) / 2500;
                color = Color.Lerp(colorPresent, GetColorCloud(WeatherCycle.initChangeBackTime), frac);
                GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    private Color GetColorCloud(float initTime)
    {
        float timeVal = initTime + 2500;
        if (timeVal >= 57000 && timeVal < 62000)
        {
            frac = (timeVal - 57000) / 5000;
            return Color.Lerp(colorDay, colorNight, frac);
        }
        else if (timeVal >= 23000 && timeVal < 29000)
        {
            return Color.Lerp(colorNight, colorDay, frac);
        }
        else if (timeVal > 21600 && timeVal <= 64800)
        {
            return colorDay;
        }
        else
        {
            return colorNight;
        }
    }
}
