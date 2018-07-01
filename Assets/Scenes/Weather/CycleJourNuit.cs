using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleJourNuit : MonoBehaviour {

	public GameObject worldLight;
	public float initTime;
	public Color[] colorsTime;
	public Color[] colorsWeather;
	public float speed;
	public GameObject sun;
	public float transYMinSun = - 4.3f;
	public float transYMaxSun = 4f;
	public float transXMinSun = -5f;
	public float transXMaxSun = 5f;
	public float transYToggleSun = 0.38f;
	public GameObject moon;
	public Sprite[] moonSprites;
	public GameObject[] mountains;
	public Color[] mountainColors;
	public float transYMinMoon = - 4.3f;
	public float transYMaxMoon = 4f;
	public float transXMinMoon = -5f;
	public float transXMaxMoon = 5f;
	public float transYToggleMoon = 0.13f;
	public GameObject background;
	public GameObject stars;

	public static float timeOfDay;
	private Vector3 playerPosition;
	private int day;
	private bool hasMoonSet;
	private float fracSun;
	private float ySun;
	private float xSun;
	private float fracMoon;
	private float yMoon;
	private float xMoon;
	private float fracColor;
	private Color colorTop;
	private Color colorBottom;
	private float fracObjects;
	private Color colorObjects;

	private float[] randomTime;

	// Use this for initialization
	void Start () {
		playerPosition =  GameObject.FindGameObjectWithTag("Player").transform.position;
		timeOfDay = initTime * 60 * 60;
		day = 0;
		moon.GetComponent<SpriteRenderer>().sprite = moonSprites[0];
		if (timeOfDay > 21600 && timeOfDay <= 43200)
		{
			fracSun = (timeOfDay - 21600) / 21600;
			ySun = Mathf.Lerp(transYMinSun, transYMaxSun, fracSun);
			sun.GetComponent<Transform>().position = new Vector3(sun.GetComponent<Transform>().position.x, ySun + playerPosition.y, sun.GetComponent<Transform>().position.z);
			hasMoonSet = false;
			moon.GetComponent<Transform>().position = new Vector3(moon.GetComponent<Transform>().position.x, transYMinMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
		} else if (timeOfDay > 43200 && timeOfDay <= 64800) {
			fracSun = (timeOfDay - 43200) / 21600;
			ySun = Mathf.Lerp(transYMaxSun, transYMinSun, fracSun);
			sun.GetComponent<Transform>().position = new Vector3(sun.GetComponent<Transform>().position.x, ySun + playerPosition.y, sun.GetComponent<Transform>().position.z);
			hasMoonSet = false;
			moon.GetComponent<Transform>().position = new Vector3(moon.GetComponent<Transform>().position.x, transYMinMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
		} else if (timeOfDay <= 86400 && timeOfDay > 64800) {
			fracMoon = (timeOfDay - 64800) / 21600;
			yMoon = Mathf.Lerp(transYMinMoon, transYMaxMoon, fracMoon);
			hasMoonSet = false;
			moon.GetComponent<Transform>().position = new Vector3(moon.GetComponent<Transform>().position.x, yMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
			sun.GetComponent<Transform>().position = new Vector3(sun.GetComponent<Transform>().position.x, transYMinSun + playerPosition.y, sun.GetComponent<Transform>().position.z);
		} else {
			fracMoon = timeOfDay / 21600;
			yMoon = Mathf.Lerp(transYMaxMoon, transYMinMoon, fracMoon);
			if (yMoon >= transYToggleMoon) {
				hasMoonSet = false;
			} else {
				hasMoonSet = true;
			}
			moon.GetComponent<Transform>().position = new Vector3(moon.GetComponent<Transform>().position.x, yMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
			sun.GetComponent<Transform>().position = new Vector3(sun.GetComponent<Transform>().position.x, transYMinSun + playerPosition.y, sun.GetComponent<Transform>().position.z);
		}
		if (timeOfDay > 21600 && timeOfDay <= 64800) {
			mountains[0].GetComponent<SpriteRenderer>().color = mountainColors[0];
			mountains[1].GetComponent<SpriteRenderer>().color = mountainColors[1];
			mountains[2].GetComponent<SpriteRenderer>().color = mountainColors[2];
		} else {
			mountains[0].GetComponent<SpriteRenderer>().color = mountainColors[3];
			mountains[1].GetComponent<SpriteRenderer>().color = mountainColors[4];
			mountains[2].GetComponent<SpriteRenderer>().color = mountainColors[5];
		}

		randomTime = new float[4];
		randomTime[0] = Random.Range(59000, 62000);
		randomTime[1] = Random.Range(60000, 63000);
		randomTime[2] = Random.Range(75600, 86400);
		randomTime[3] = Random.Range(76000, 85000);
	}

	// Update is called once per frame
	void Update () {
		playerPosition =  GameObject.FindGameObjectWithTag("Player").transform.position;
		timeOfDay = timeOfDay + Time.deltaTime * speed;
		if (timeOfDay > 86400)
		{
			timeOfDay = 0;
			day++;
			randomTime[0] = Random.Range(59000, 62000);
			randomTime[1] = Random.Range(60000, 63000);
			randomTime[2] = Random.Range(75600, 86400);
			randomTime[3] = Random.Range(76000, 85000);
		}

		if (timeOfDay < 43200)
			worldLight.GetComponent<Light> ().intensity = 80 + (timeOfDay / 60);
		else if (timeOfDay >= 43200 && timeOfDay < 86400)
			worldLight.GetComponent<Light> ().intensity = 800 - ((timeOfDay-43200) / 60);


		if (day % 5 == 0 || day % 5 == 4) {
			if (moon.GetComponent<Transform>().position.y <= playerPosition.y-3.0f && hasMoonSet)
				moon.GetComponent<SpriteRenderer>().sprite = moonSprites[0];
		} else if (day % 5 == 1 || day % 5 == 3) {
			if (moon.GetComponent<Transform>().position.y <= playerPosition.y-3.0f && hasMoonSet)
				moon.GetComponent<SpriteRenderer>().sprite = moonSprites[1];
		} else if (day % 5 == 2) {
			if (moon.GetComponent<Transform>().position.y <= playerPosition.y-3.0f && hasMoonSet)
				moon.GetComponent<SpriteRenderer>().sprite = moonSprites[2];
		}

		SunMoonPosition();
		if (!WeatherCycle.weatherChanging)
		{
			ColorChangeBackgroundWithTime();
			ColorChangeObjects();
		} else if (WeatherCycle.type == 1)
		{
			ColorChangeObjects();
		}
		if (timeOfDay >= 56000)
		{
			stars.SetActive(true);
		}
		if (timeOfDay >= 29500 && timeOfDay < 56000)
		{
			stars.SetActive(false);
		}
	}

	private float findY(float min, float max, float x){
		float y = -4*Mathf.Pow(x, 2) + 4 * x;
		return min + (max - min) * y;
	}

	private float risingY(float min, float max, float x){
		float y = -Mathf.Pow(x, 2) + 2 * x;
		return min + (max - min) * y;
	}

	private float fallingY(float min, float max, float x){
		float y = -Mathf.Pow(x, 2) + 1;
		return min + (max - min) * y;
	}

	private void SunMoonPosition()
	{
		if (timeOfDay > 21600 && timeOfDay <= 43200)
		{
			fracSun = (timeOfDay - 21600) / 21600;
			xSun = Mathf.Lerp(transXMinSun, (transXMaxSun+transXMinSun)/2, fracSun);
			ySun = risingY(transYMinSun, transYMaxSun, fracSun);

			sun.GetComponent<Transform>().position = new Vector3(xSun + playerPosition.x, ySun + playerPosition.y, sun.GetComponent<Transform>().position.z);
			hasMoonSet = false;
		} else if (timeOfDay > 43200 && timeOfDay <= 64800) {
			fracSun = (timeOfDay - 43200) / 21600;
			xSun = Mathf.Lerp((transXMaxSun+transXMinSun)/2, transXMaxSun, fracSun);
			ySun = fallingY(transYMinSun, transYMaxSun, fracSun);

			sun.GetComponent<Transform>().position = new Vector3(xSun + playerPosition.x, ySun + playerPosition.y, sun.GetComponent<Transform>().position.z);
			hasMoonSet = false;
		} else if (timeOfDay <= 86400 && timeOfDay > 64800) {
			fracMoon = (timeOfDay - 64800) / 21600;
			xMoon = Mathf.Lerp(transXMinMoon, (transXMaxMoon+transXMinMoon)/2, fracMoon);
			yMoon = risingY(transYMinMoon, transYMaxMoon, fracMoon);

			hasMoonSet = false;
			moon.GetComponent<Transform>().position = new Vector3(xMoon + playerPosition.x, yMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
		} else {
			fracMoon = timeOfDay / 21600;
			xMoon = Mathf.Lerp((transXMaxMoon+transXMinMoon)/2, transXMaxMoon, fracMoon);
			yMoon = fallingY(transYMinMoon, transYMaxMoon, fracMoon);

			if (yMoon >= transYToggleMoon) {
				hasMoonSet = false;
			} else {
				hasMoonSet = true;
			}
			moon.GetComponent<Transform>().position = new Vector3(xMoon + playerPosition.x, yMoon + playerPosition.y, moon.GetComponent<Transform>().position.z);
		}
	}

	private void ColorChangeBackgroundWithTime ()
	{
		if (timeOfDay >= 0 && timeOfDay < 10800) {
			fracColor = timeOfDay / 10800;
			colorTop = Color.Lerp(colorsTime[0], colorsTime[2], fracColor);
			colorBottom = Color.Lerp(colorsTime[1], colorsTime[3], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 10800 && timeOfDay < 27000) {
			fracColor = (timeOfDay - 10800) / 16200;
			colorTop = Color.Lerp(colorsTime[2], colorsTime[4], fracColor);
			colorBottom = Color.Lerp(colorsTime[3], colorsTime[5], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 27000 && timeOfDay < 32400) {
			fracColor = (timeOfDay - 27000) / 5400;
			colorTop = Color.Lerp(colorsTime[4], colorsTime[6], fracColor);
			colorBottom = Color.Lerp(colorsTime[5], colorsTime[7], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 32400 && timeOfDay < 43200) {
			fracColor = (timeOfDay - 32400) / 10800;
			colorTop = Color.Lerp(colorsTime[6], colorsTime[8], fracColor);
			colorBottom = Color.Lerp(colorsTime[7], colorsTime[9], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 43200 && timeOfDay < 54000) {
			fracColor = (timeOfDay - 43200) / 10800;
			colorTop = Color.Lerp(colorsTime[8], colorsTime[10], fracColor);
			colorBottom = Color.Lerp(colorsTime[9], colorsTime[11], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 54000 && timeOfDay < 62000) {
			fracColor = (timeOfDay - 54000) / 8000;
			colorTop = Color.Lerp(colorsTime[10], colorsTime[12], fracColor);
			colorBottom = Color.Lerp(colorsTime[11], colorsTime[13], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 62000 && timeOfDay < 75600) {
			fracColor = (timeOfDay - 62000) / 13600;
			colorTop = Color.Lerp(colorsTime[12], colorsTime[14], fracColor);
			colorBottom = Color.Lerp(colorsTime[13], colorsTime[15], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		} else if (timeOfDay >= 75600 && timeOfDay < 86400) {
			fracColor = (timeOfDay - 75600) / 10800;
			colorTop = Color.Lerp(colorsTime[14], colorsTime[16], fracColor);
			colorBottom = Color.Lerp(colorsTime[15], colorsTime[17], fracColor);
			background.GetComponent<Renderer>().material.SetColor("_TopColor", colorTop);
			background.GetComponent<Renderer>().material.SetColor("_BottomColor", colorBottom);
		}
	}

	private void ColorChangeObjects()
	{   
		if (timeOfDay >= 57000 && timeOfDay < 62000) {
			fracObjects = (timeOfDay - 57000) / 5000;
			colorObjects = Color.Lerp(mountainColors[0], mountainColors[3], fracObjects);
			mountains[0].GetComponent<SpriteRenderer>().color = colorObjects;
			colorObjects = Color.Lerp(mountainColors[1], mountainColors[4], fracObjects);
			mountains[1].GetComponent<SpriteRenderer>().color = colorObjects;
			colorObjects = Color.Lerp(mountainColors[2], mountainColors[5], fracObjects);
			mountains[2].GetComponent<SpriteRenderer>().color = colorObjects;
		}
		if (timeOfDay >= 23000 && timeOfDay < 29000) {
			fracObjects = (timeOfDay - 23000) / 6000;
			colorObjects = Color.Lerp(mountainColors[3], mountainColors[0], fracObjects);
			mountains[0].GetComponent<SpriteRenderer>().color = colorObjects;
			colorObjects = Color.Lerp(mountainColors[4], mountainColors[1], fracObjects);
			mountains[1].GetComponent<SpriteRenderer>().color = colorObjects;
			colorObjects = Color.Lerp(mountainColors[5], mountainColors[2], fracObjects);
			mountains[2].GetComponent<SpriteRenderer>().color = colorObjects;
		}
	}

	public Color GetBackgroundColorTop(float initTimeChange)
	{
		float timeVal = initTimeChange + 2500;
		if (timeVal >= 0 && timeVal < 10800) {
			fracColor = timeVal / 10800;
			return Color.Lerp(colorsTime[0], colorsTime[2], fracColor);
		} else if (timeVal >= 10800 && timeVal < 27000) {
			fracColor = (timeVal - 10800) / 16200;
			return Color.Lerp(colorsTime[2], colorsTime[4], fracColor);
		} else if (timeVal >= 27000 && timeVal < 32400) {
			fracColor = (timeVal - 27000) / 5400;
			return Color.Lerp(colorsTime[4], colorsTime[6], fracColor);
		} else if (timeVal >= 32400 && timeVal < 43200) {
			fracColor = (timeVal - 32400) / 10800;
			return Color.Lerp(colorsTime[6], colorsTime[8], fracColor);
		} else if (timeVal >= 43200 && timeVal < 54000) {
			fracColor = (timeVal - 43200) / 10800;
			return Color.Lerp(colorsTime[8], colorsTime[10], fracColor);
		} else if (timeVal >= 54000 && timeVal < 62000) {
			fracColor = (timeVal - 54000) / 8000;
			return Color.Lerp(colorsTime[10], colorsTime[12], fracColor);
		} else if (timeVal >= 62000 && timeVal < 75600) {
			fracColor = (timeVal - 62000) / 13600;
			return Color.Lerp(colorsTime[12], colorsTime[14], fracColor);
		} else {
			fracColor = (timeVal - 75600) / 10800;
			return Color.Lerp(colorsTime[14], colorsTime[16], fracColor);
		}
	}

	public Color GetBackgroundColorBottom(float initTimeChange)
	{
		float timeVal = initTimeChange + 2500;
		if (timeVal >= 0 && timeVal < 10800) {
			fracColor = timeVal / 10800;
			return Color.Lerp(colorsTime[1], colorsTime[3], fracColor);
		} else if (timeVal >= 10800 && timeVal < 27000) {
			fracColor = (timeVal - 10800) / 16200;
			return Color.Lerp(colorsTime[3], colorsTime[5], fracColor);
		} else if (timeVal >= 27000 && timeVal < 32400) {
			fracColor = (timeVal - 27000) / 5400;
			return Color.Lerp(colorsTime[5], colorsTime[7], fracColor);
		} else if (timeVal >= 32400 && timeVal < 43200) {
			fracColor = (timeVal - 32400) / 10800;
			return Color.Lerp(colorsTime[7], colorsTime[9], fracColor);
		} else if (timeVal >= 43200 && timeVal < 54000) {
			fracColor = (timeVal - 43200) / 10800;
			return Color.Lerp(colorsTime[9], colorsTime[11], fracColor);
		} else if (timeVal >= 54000 && timeVal < 62000) {
			fracColor = (timeVal - 54000) / 8000;
			return Color.Lerp(colorsTime[11], colorsTime[13], fracColor);
		} else if (timeVal >= 62000 && timeVal < 75600) {
			fracColor = (timeVal - 62000) / 13600;
			return Color.Lerp(colorsTime[13], colorsTime[15], fracColor);
		} else {
			fracColor = (timeVal - 75600) / 10800;
			return Color.Lerp(colorsTime[15], colorsTime[17], fracColor);
		}
	}

	public Color GetMountainColor(float initTimeChange, int i)
	{
		float timeVal = initTimeChange + 2500;
		if (timeVal >= 57000 && timeVal < 62000) {
			fracObjects = (timeVal - 57000) / 5000;
			if (i == 0) {
				return Color.Lerp(mountainColors[0], mountainColors[3], fracObjects);
			} else if (i == 1) {
				return Color.Lerp(mountainColors[1], mountainColors[4], fracObjects);
			} else {
				return Color.Lerp(mountainColors[2], mountainColors[5], fracObjects);
			}
		} else if (timeVal >= 23000 && timeVal < 29000) {
			fracObjects = (timeVal - 23000) / 6000;
			if (i == 0) {
				return Color.Lerp(mountainColors[3], mountainColors[0], fracObjects);
			} else if (i == 1) {
				return Color.Lerp(mountainColors[4], mountainColors[1], fracObjects);
			} else {
				return Color.Lerp(mountainColors[5], mountainColors[2], fracObjects);
			}
		} else if (timeVal >= 62000 && timeVal < 23000) {
			if (i == 0) {
				return mountainColors[3];
			} else if (i == 1) {
				return mountainColors[4];
			} else {
				return mountainColors[5];
			}
		} else {
			if (i == 0) {
				return mountainColors[0];
			} else if (i == 1) {
				return mountainColors[1];
			} else {
				return mountainColors[2];
			}
		}
	}
}
