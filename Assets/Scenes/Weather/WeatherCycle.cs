using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCycle : MonoBehaviour {

	public Color[] backgroundColors;
	public Color[] mountainColors;
	public GameObject fog;
	public GameObject rain;
	public GameObject lightning;
	public GameObject snow;
	public GameObject Background;
	public GameObject stars;
	public Color fogColor;

	public GameObject background;
	public GameObject[] mountains;

	public float timeMin;
	public float timeMax;

	public static int type;
	private float time;
	private float timeFinal;
	public static bool weatherChanging = false;
	private float frac;
	public static float initTime;
	public static float initChangeBackTime;
	private Color[] backgroundColorsCurrent;
	private Color[] mountainColorsPresent;
	private Color[] backgroundColorsFinal;
	private Color[] mountainColorsFinal;
	private CycleJourNuit dayNightCycle;
	private int x;
	private int numLightning;
	private float[] randomTime;

	// Use this for initialization
	void Start() {
        time = 0;
		timeFinal = 1000 * Random.Range(timeMin, timeMax);
		weatherChanging = false;
		backgroundColorsCurrent = new Color[2];
		mountainColorsPresent = new Color[3];
		backgroundColorsFinal = new Color[2];
		mountainColorsFinal = new Color[3];
		dayNightCycle = Background.GetComponent<CycleJourNuit>();
		randomTime = new float[2];
	}

	// Update is called once per frame
	void Update() {
		time = time + Time.deltaTime * 100 * 5;
		if (time >= timeFinal)
		{
			time = 0;
			timeFinal = 1000 * Random.Range(timeMin, timeMax);
			type = (int)Random.Range(0, 2);
			weatherChanging = true;
			initTime = Time.time;
			if (type == 0)
			{
				fog.SetActive(true);
				rain.SetActive(true);
				numLightning = Random.Range(0, 1);
				x = 0;
				for (int i = 0; i < randomTime.Length; i++)
				{
					randomTime[i] = Random.Range(10, 20);
				}
			} else
			{
				snow.SetActive(true);
			}
		}
		if (weatherChanging)
		{
			WeatherChange();
		}
	}

	private void WeatherChange()
	{
		if (type == 0)
		{
			if (Time.time - initTime <= 25)
			{
				backgroundColorsCurrent[0] = background.GetComponent<Renderer>().material.GetColor("_TopColor");
				backgroundColorsCurrent[1] = background.GetComponent<Renderer>().material.GetColor("_BottomColor");
				if (CycleJourNuit.timeOfDay < 12000 || CycleJourNuit.timeOfDay >= 75000) //Night Time
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[0], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[1], Time.deltaTime));
					for (int i = 0; i < 3; i++)
					{
						mountains[i].GetComponent<SpriteRenderer>().color = Color.Lerp(mountainColorsPresent[i], mountainColors[0], Time.deltaTime);
					}
				}
				else if (CycleJourNuit.timeOfDay >= 12000 && CycleJourNuit.timeOfDay < 29500) //Sun Rise
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[2], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[3], Time.deltaTime));
					for (int i = 0; i < 3; i++)
					{
						mountains[i].GetComponent<SpriteRenderer>().color = Color.Lerp(mountainColorsPresent[i], mountainColors[1], Time.deltaTime);
					}
				}
				else if (CycleJourNuit.timeOfDay >= 29500 && CycleJourNuit.timeOfDay < 56000)//Day Time
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[4], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[5], Time.deltaTime));
					for (int i = 0; i < 3; i++)
					{
						mountains[i].GetComponent<SpriteRenderer>().color = Color.Lerp(mountainColorsPresent[i], mountainColors[2], Time.deltaTime);
					}
				}
				else if (CycleJourNuit.timeOfDay >= 56000 && CycleJourNuit.timeOfDay < 75000)//Sun set
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[6], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[7], Time.deltaTime));
					for (int i = 0; i < 3; i++)
					{
						mountains[i].GetComponent<SpriteRenderer>().color = Color.Lerp(mountainColorsPresent[i], mountainColors[3], Time.deltaTime);
					}
				}
				if (x > numLightning)
				{
					lightning.SetActive(false);
				}
				if (x <= numLightning)
				{
					if (Time.time - initTime > randomTime[0] || Time.time - initTime > randomTime[1])
					{
						lightning.GetComponent<Transform>().position = new Vector3(Random.Range(-6.5f, 6.5f) + GameObject.FindGameObjectWithTag("Player").transform.position.x, lightning.GetComponent<Transform>().position.y, lightning.GetComponent<Transform>().position.z);
						lightning.SetActive(true);
						background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.white);
						background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.white);
						for (int i = 0; i < 3; i++)
						{
							mountains[i].GetComponent<SpriteRenderer>().color = Color.white;
						}
						x++;
					}
				} 

				initChangeBackTime = CycleJourNuit.timeOfDay;
				backgroundColorsFinal[0] = dayNightCycle.GetBackgroundColorTop(initChangeBackTime);
				backgroundColorsFinal[1] = dayNightCycle.GetBackgroundColorBottom(initChangeBackTime);
				for (int i = 0; i < 3; i++)
				{
					mountainColorsFinal[i] = dayNightCycle.GetMountainColor(initChangeBackTime, i);
				}
			}
			else if (Time.time - initTime > 25 && Time.time - initTime <= 30)
			{
				frac = (CycleJourNuit.timeOfDay - initChangeBackTime) / 2500;
				background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColorsFinal[0], frac));
				background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColorsFinal[1], frac));

				for (int i = 0; i < 3; i++)
				{
					mountains[i].GetComponent<SpriteRenderer>().color = Color.Lerp(mountainColorsPresent[i], mountainColorsFinal[i], frac);
				}
			} else
			{
				weatherChanging = false;
				fog.SetActive(false);
				rain.SetActive(false);
			}
		} else
		{
			if (Time.time - initTime <= 25)
			{
				backgroundColorsCurrent[0] = background.GetComponent<Renderer>().material.GetColor("_TopColor");
				backgroundColorsCurrent[1] = background.GetComponent<Renderer>().material.GetColor("_BottomColor");
				if (CycleJourNuit.timeOfDay < 12000 && CycleJourNuit.timeOfDay >= 75000) //Night Time
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[8], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[9], Time.deltaTime));
				}
				else if (CycleJourNuit.timeOfDay >= 12000 && CycleJourNuit.timeOfDay < 29500) //Sun Rise
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[10], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[11], Time.deltaTime));
				}
				else if (CycleJourNuit.timeOfDay >= 29500 && CycleJourNuit.timeOfDay < 56000)//Day Time
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[12], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[13], Time.deltaTime));
				}
				else if (CycleJourNuit.timeOfDay >= 56000 && CycleJourNuit.timeOfDay < 75000)//Sun set
				{
					background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColors[14], Time.deltaTime));
					background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColors[15], Time.deltaTime));
				}
				initChangeBackTime = CycleJourNuit.timeOfDay;
				backgroundColorsFinal[0] = dayNightCycle.GetBackgroundColorTop(initChangeBackTime);
				backgroundColorsFinal[1] = dayNightCycle.GetBackgroundColorBottom(initChangeBackTime);
			}
			else if (Time.time - initTime > 25 && Time.time - initTime <= 30)
			{
				frac = (CycleJourNuit.timeOfDay - initChangeBackTime) / 2500;
				background.GetComponent<Renderer>().material.SetColor("_TopColor", Color.Lerp(backgroundColorsCurrent[0], backgroundColorsFinal[0], frac));
				background.GetComponent<Renderer>().material.SetColor("_BottomColor", Color.Lerp(backgroundColorsCurrent[1], backgroundColorsFinal[1], frac));
			}
			else
			{
				weatherChanging = false;
				snow.SetActive(false);
			}
		}
	}

}
