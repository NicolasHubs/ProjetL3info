using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTarget : MonoBehaviour {

	[Header("Target")]
	public GameObject target;

	[Header("Clamp")]
	public bool x;
	public bool y;
	public bool z;

	
	private Transform targetTransform;
	private Transform thisTransform;

	// Use this for initialization
	void Start () {
		targetTransform = target.GetComponent<Transform>();
		thisTransform = gameObject.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		float xt = targetTransform.position.x;
		float yt = targetTransform.position.y;
		float zt = thisTransform.position.z;

		Vector3 v = new Vector3(thisTransform.position.x, thisTransform.position.y, thisTransform.position.z);
		if(x) v.x = xt;
		if(y) v.y = yt;
		if(z) v.z = zt;

		thisTransform.position = v;
	}
}
