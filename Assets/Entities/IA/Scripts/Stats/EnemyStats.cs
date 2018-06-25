using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats {
	public Animator anim;
	public override void Die() {
		base.Die();
		anim.SetBool ("isDead", true);
		StartCoroutine(Timer());
	}

	IEnumerator Timer() {
		yield return new WaitForSeconds(2); //this will wait 5 seconds
		Destroy (GameObject.FindGameObjectWithTag("Squirrel"));
	}


}