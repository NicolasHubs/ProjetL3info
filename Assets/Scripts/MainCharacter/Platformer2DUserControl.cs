using System;
using UnityEngine;
using TheNewWorld.CrossPlatformInput;
using System.Collections;

namespace TheNewWorld.characterScripts
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }

        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

			if (Input.GetButton("Fire1") || Input.GetMouseButtonDown (0)) {
				Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				float distMax = Vector2.Distance (mouseWorldPosition, m_Character.transform.position);
				if (distMax <= 3.0f) {
					RaycastHit2D hit2d = Physics2D.Raycast (transform.position, mouseWorldPosition - transform.position, distMax);
					if (hit2d.collider != null) {
					
						GameObject clickedGameObject = hit2d.collider.gameObject;

						GetComponent<Inventory> ().Add (clickedGameObject.GetComponent<TileData> ().tileType, 1);
						Destroy (clickedGameObject);

						// With block hardness
						/*
						if (clickedGameObject.GetComponent<TileData> ().tileHardness <= 0) {
							GetComponent<Inventory> ().Add (clickedGameObject.GetComponent<TileData> ().tileType, 1);
							Destroy (clickedGameObject);
						} else {
							clickedGameObject.GetComponent<TileData> ().tileHardness--;
						}*/
					}
				}
			}
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
       	 }
    }
}
