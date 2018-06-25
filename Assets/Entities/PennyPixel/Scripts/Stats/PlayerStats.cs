using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Handles the players stats and adds/removes modifiers when equipping items. */

public class PlayerStats : CharacterStats {
	public Animator anim;
	public override void Die() {
		base.Die();
		anim.SetBool("hurt",true);
		StartCoroutine(Timer());

	}


	IEnumerator Timer() {
		yield return new WaitForSeconds(2); //this will wait 5 seconds
		Destroy (GameObject.FindGameObjectWithTag("Player"));
	}
}