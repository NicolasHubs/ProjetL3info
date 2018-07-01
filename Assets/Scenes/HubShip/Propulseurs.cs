using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propulseurs : MonoBehaviour {

	public GameObject p0;
	public GameObject p1;

	private float time = 0;
	private float interpolationDayPeriod = 0.3f;
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time >= interpolationDayPeriod) {
			p0.SetActive (!p0.activeSelf);
			p1.SetActive (!p1.activeSelf);
			time = 0.0f;
		}
	}
}
