/*
 PlayerMovePhys.cs
 By: Liam Binford
 Date: 8/24/20
 Description: Uses rigidbody for movement because charcontroller is FUCKING USELESS
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovePhys : MonoBehaviour
{
    //Movement
    [SerializeField] private float moveSpeed = default;
    [SerializeField] private float maxMoveSpeed = default;
    [SerializeField] private float airSpeedScale = default;
    private bool isGrounded = true;
    private bool doubleJump = true;
    [SerializeField] private float friction = default;
    [SerializeField] private float jumpForce = default;

    //Inputs
    float xInput;
    float yInput;
    private bool jumping = false;

    //Other
    private Rigidbody myrb;

    // Start is called before the first frame update
    void Start()
    {
        myrb = GetComponent<Rigidbody>();
        jumping = Input.GetButton("Jump");
    }

    // Fixed update is called once per physics frame
    void FixedUpdate()
    {
        PlayerMovement();
        JumpEvent();

        //if the player is moving faster than the max speed, lower their velocity
        if(myrb.velocity.magnitude > maxMoveSpeed)
        {
            myrb.velocity = myrb.velocity.normalized * maxMoveSpeed;
        }
    }

    private void PlayerMovement()
    {
        //extra gravity
        myrb.AddForce(Vector3.down * Time.fixedDeltaTime * 10);

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        float speedScale = 1;

        // Scale the acceleration along each axis accordingly
        if(xInput != 0 && yInput != 0)
        {
            speedScale /= 1.41421356237f;
		}

        // Don't accelerate as violently while in the air
        if(!isGrounded)
        {
            speedScale *= airSpeedScale;
        }

        //Push the player along different axes depending on the input being pressed
        myrb.AddForce(transform.forward * moveSpeed * yInput * speedScale);
        myrb.AddForce(transform.right * moveSpeed * xInput * speedScale);
        
        CounterForce();
    }

    private void JumpEvent()
    {
        jumping = Input.GetButton("Jump");
        //if the player is touching the ground or hasn't double jumped
        if (isGrounded && jumping || doubleJump && jumping)
        {
            myrb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            //if the player is in the air and has only jumped once, allow for a second jump
            if(!isGrounded)
            {
                doubleJump = false;
            }
            isGrounded = false;
        }
    }

    private void CounterForce()
    {
        Vector2 vel = new Vector2(myrb.velocity.x, myrb.velocity.z);
        Vector2 opp;

        //provide a force to slow the player faster if when they stop moving.
        if(vel.x * vel.x + vel.y * vel.y > friction * friction)
        {
            opp = vel.normalized * -friction;
        }
        else
        {
            opp = -vel;
        }
        Vector3 force = new Vector3(opp.x, 0, opp.y);
        myrb.AddForce(force);
    }

    private void OnCollisionStay(Collision collision)
    {
        //check if the player is touching the ground. if so, set grounded and double jump to true.
        if(collision.gameObject.layer == 8)
        {
            
            // Get all the contact points
            ContactPoint[] cps = new ContactPoint[1];
            collision.GetContacts(cps);
            RaycastHit hit;
            bool hasHit = false;
            float bestDot = -1;
            // Figure out how close to being aligned with gravity the surface of each contact point is, and grab the best
            for(int i = 0; i < cps.Length; i++) {
                if(Physics.Raycast(cps[i].point, cps[i].normal, out hit)) {
                    hasHit = true;
                    float dot = Vector3.Dot(new Vector3(0, -1, 0), hit.normal);
                    bestDot = Math.Max(bestDot, dot);
                }
            }
            if(hasHit) {
                Debug.Log(bestDot);
                // Check if the best contact point's normal is close enough to being level
                if(bestDot > 0.5)
                {
                    isGrounded = true;
                    doubleJump = true;
                }
            } 

            else
            {
                Debug.Log("NORMAL DETERMINATION FAILED");
			}
        }
    }

}
