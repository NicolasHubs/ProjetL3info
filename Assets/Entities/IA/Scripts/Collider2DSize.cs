using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DSize : MonoBehaviour {

	/* Retourne 3 valeurs :
	 * x = taille horizontale du collider
	 * y = taille verticale du collider
	 * z = position à laquelle commence le raycast de l'IA
	 */
	public static Vector3 GetColliderSize(Collider2D[] colliders) {

		Vector3 res = Vector3.zero;

		float positiveSizeX = 0; // représente la partie X positive à partir du centre (l'origine) du collider
		float negativeSizeX = 0; // représente la partie X négative à partir du centre (l'origine) du collider

		float positiveSizeY = 0; // représente la partie Y positive à partir du centre (l'origine) du collider
		float negativeSizeY = 0; // représente la partie Y négative à partir du centre (l'origine) du collider

		// x for positive value & y for negative value
		Vector2 maxX = Vector2.zero; 
		Vector2 maxY = Vector2.zero;

		foreach (Collider2D collider in colliders) {
			if (collider.enabled){
					
				switch (collider.GetType ().Name) {
				case "BoxCollider2D":
					BoxCollider2D boxCollider = (BoxCollider2D)collider;

					if (boxCollider.offset.x >= 0) {
						positiveSizeX = boxCollider.size.x + Mathf.Abs (boxCollider.offset.x) - (boxCollider.size.x / 2);
						negativeSizeX = Mathf.Clamp (boxCollider.size.x - positiveSizeX, 0.0f, positiveSizeX);
					} else {
						negativeSizeX = boxCollider.size.x + Mathf.Abs (boxCollider.offset.x) - (boxCollider.size.x / 2);
						positiveSizeX = Mathf.Clamp (boxCollider.size.x - positiveSizeX, 0.0f, positiveSizeX);
					}

					if (boxCollider.offset.y >= 0) {
						positiveSizeY = boxCollider.size.y + Mathf.Abs (boxCollider.offset.y) - (boxCollider.size.y / 2);
						negativeSizeY = Mathf.Clamp (boxCollider.size.y - positiveSizeY, 0.0f, positiveSizeY);
					} else {
						negativeSizeY = boxCollider.size.y + Mathf.Abs (boxCollider.offset.y) - (boxCollider.size.y / 2);
						positiveSizeY = Mathf.Clamp (boxCollider.size.y - positiveSizeY, 0.0f, positiveSizeY);
					}

					break;
				case "CapsuleCollider2D": 
					// Ne marche que pour les capsule colliders horizontaux, pour le vertical il faudrait copier coller le code ci dessous et l'inverser si il est vertical

					CapsuleCollider2D capsuleCollider = (CapsuleCollider2D)collider;

					// La valeur X minimale d'un capsule collider horizontal est de 0.1
					float capsuleColliderSizeX = capsuleCollider.size.y;

					if (capsuleCollider.size.x > capsuleColliderSizeX)
						capsuleColliderSizeX = capsuleCollider.size.x;

					if (capsuleCollider.offset.x >= 0) {
						positiveSizeX = capsuleColliderSizeX + Mathf.Abs (capsuleCollider.offset.x) - (capsuleColliderSizeX / 2);
						negativeSizeX = Mathf.Clamp (capsuleColliderSizeX - positiveSizeX, 0.0f, positiveSizeX);
					} else {
						negativeSizeX = capsuleColliderSizeX + Mathf.Abs (capsuleCollider.offset.x) - (capsuleColliderSizeX / 2);
						positiveSizeX = Mathf.Clamp (capsuleColliderSizeX - negativeSizeX, 0.0f, negativeSizeX);
					}

					if (capsuleCollider.offset.y >= 0) {
						positiveSizeY = capsuleCollider.size.y + Mathf.Abs (capsuleCollider.offset.y) - (capsuleCollider.size.y / 2);
						negativeSizeY = Mathf.Clamp (capsuleCollider.size.y - positiveSizeY, 0.0f, positiveSizeY);
					} else {
						negativeSizeY = capsuleCollider.size.y + Mathf.Abs (capsuleCollider.offset.y) - (capsuleCollider.size.y / 2);
						positiveSizeY = Mathf.Clamp (capsuleCollider.size.y - negativeSizeY, 0.0f, negativeSizeY);
					}

					break;
				
				case "CircleCollider2D":
					CircleCollider2D circleCollider = (CircleCollider2D)collider;

					if (circleCollider.offset.x >= 0) {
						positiveSizeX = circleCollider.radius*2 + Mathf.Abs (circleCollider.offset.x) - circleCollider.radius;
						negativeSizeX = Mathf.Clamp (circleCollider.radius*2 - positiveSizeX, 0.0f, positiveSizeX);
					} else {
						negativeSizeX = circleCollider.radius*2 + Mathf.Abs (circleCollider.offset.x) - circleCollider.radius;
						positiveSizeX = Mathf.Clamp (circleCollider.radius*2 - negativeSizeX, 0.0f, negativeSizeX);
					}

					if (circleCollider.offset.y >= 0) {
						positiveSizeY = circleCollider.radius*2 + Mathf.Abs (circleCollider.offset.y) - circleCollider.radius;
						negativeSizeY = Mathf.Clamp (circleCollider.radius*2 - positiveSizeY, 0.0f, positiveSizeY);
					} else {
						negativeSizeY = circleCollider.radius*2 + Mathf.Abs (circleCollider.offset.y) - circleCollider.radius;
						positiveSizeY = Mathf.Clamp (circleCollider.radius*2 - negativeSizeY, 0.0f, negativeSizeY);
					}
					break;
				/*
				case "PolygonCollider2D":
					PolygonCollider2D polygonCollider = (PolygonCollider2D)collider;

					break;
				case "EdgeCollider2D":
					EdgeCollider2D edgeCollider = (EdgeCollider2D)collider;

					break;
				*/
				default:
					Debug.LogError ("Non-treated Collider2D type.");
					break;
				}
				if (maxX.x < positiveSizeX * 2)
					maxX.x = positiveSizeX * 2;

				if (maxX.y < negativeSizeX * 2)
					maxX.y = negativeSizeX * 2;

				if (maxY.x < positiveSizeY * 2)
					maxY.x = positiveSizeY * 2;

				if (maxY.y < negativeSizeY * 2)
					maxY.y = negativeSizeY * 2;
			}
		}

		res.z = maxY.x; // Valeur fausse (= 0 par défaut) si tous les colliders sont sous le centre des collider

		res.x = maxX.x + maxX.y;
		res.y = maxY.x + maxY.y;
		return res;
	}
}
