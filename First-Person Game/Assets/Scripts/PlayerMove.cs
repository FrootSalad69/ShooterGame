/*
 PlayerMove.cs
 By: Liam Binford
 Date: 7/28/20
 Description: Allows the player to move the cahracter using WASD or Arrow Keys
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    //To anyone wondering why everything is set to default, blame unity for not being able to recognize a serialized field.
    [SerializeField] private string horizontalInputName = default;
    [SerializeField] private string verticalInputName = default;
    private float movementSpeed = default;

    [SerializeField] private float walkSpeed = default;
    [SerializeField] private float runSpeed = default;
    [SerializeField] private float runBuildUpSpeed = default;
    [SerializeField] private KeyCode runKey = default;

    [SerializeField] private float slopeForce = default;
    [SerializeField] private float slopeForceRayLength = default;

    private CharacterController charController;

    [SerializeField] private AnimationCurve jumpFallOff = default;
    [SerializeField] private float jumpMultiplier = default;
    [SerializeField] private KeyCode jumpKey = default;


    private bool isJumping;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);

        if((vertInput != 0 || horizInput != 0) && OnSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);
        }

        SetMovementSpeed();
        JumpInput();
    }

    private void SetMovementSpeed()
    {
        if(Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUpSpeed);
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUpSpeed);
        }
    }

    private bool OnSlope()
    {
        if(isJumping)
        {
            return false;
        }

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))
        {
            if(hit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = 45.0f;
        isJumping = false;
    }

}