using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    private float maxspeed;
    private float minspeed;
    private float speed;
    public float jumpstr;
    public GameObject rayOrigin;
    public float rayCheckDistance;
    private Rigidbody2D rb; 
    private Animator m_Anim; // Ref to player's animator
    private bool m_FacingRight = true; // For determining which way the player is currently facing.
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        m_Anim = GetComponent<Animator>();
        maxspeed = 10;
        minspeed = 5;
        speed = 10;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float x = Input.GetAxis("Horizontal"); // Movements
        if (x != 0)
        {
            if (x < 0 && m_FacingRight )
            {
                m_FacingRight = false;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            if(x > 0 && !m_FacingRight)
            {
                m_FacingRight = true;
                Vector3 theScale = transform.localScale;
                theScale.x *= -1;
                transform.localScale = theScale;
            }
            m_Anim.SetBool("Run", true);
        }
        else
        {
            m_Anim.SetBool("Run", false);
        }
        if (Input.GetAxis("Jump") > 0) //Jump
        {
           
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin.transform.position, Vector2.down, rayCheckDistance);
            if (hit.collider != null)
            {
                rb.AddForce(Vector2.up * jumpstr, ForceMode2D.Impulse);
                m_Anim.SetBool("Jump", true);
            }
            else
            {
                m_Anim.SetBool("Jump", false);
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftControl)) // Crouch (go slower if you're crouching)
        {
            m_Anim.SetBool("Crouch", true);
            speed = minspeed;
            
            

        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            m_Anim.SetBool("Crouch", false);
            speed = maxspeed;
        }
        rb.velocity = new Vector3(x * speed, rb.velocity.y, 0);

    }
}
