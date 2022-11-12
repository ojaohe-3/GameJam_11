using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   
   PlayerCharacter character;
    // Start is called before the first frame update
    Rigidbody2D body;

    float horizontal;
    float vertical;
    bool interact;

    float interactionRadius = 1.0f;
    float moveLimiter = 0.7f;
    public float runSpeed = 20.0f;

   

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        interact = Input.GetKey("f");
        
    }

    private void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed) * Time.deltaTime;
    }
}
