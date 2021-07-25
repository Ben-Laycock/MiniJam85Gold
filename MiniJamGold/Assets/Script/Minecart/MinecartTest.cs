using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartTest : MonoBehaviour
{
    List<Transform> trackMarkers = new List<Transform>();
    Rigidbody rb;
    int targetMarkerIndex = 1;
    float targetDistanceTolerance = 0.25f;
    float rotationInterpolationSpeed = 150f;

    // Velocity stuff
    float maxVelocity = 20;
    float velocity = 0;
    float maxAcceleration = 0.1f;
    float acceleration = 0;

    //Cart animator
    Animator cartAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Transform parent = transform.parent.Find("Markers");
        foreach (Transform child in parent) { trackMarkers.Add(child); child.gameObject.SetActive(false); }

        if (trackMarkers.Count < 2) this.enabled = false;

        transform.position = trackMarkers[0].position;

        cartAnimator = gameObject.GetComponent<Animator>();
    }


    void Update()
    {
        // Set acceleration as wanted by player
        UserInput();

        // Is the cart close enough to target?
        float distanceToTarget = (trackMarkers[targetMarkerIndex].position - transform.position).magnitude;
        if (distanceToTarget <= targetDistanceTolerance)
        {
            //transform.position = trackMarkers[targetMarkerIndex].position;


            if (targetMarkerIndex == 0 && (acceleration <= 0 || velocity < 0))
            { // If the player wants to travel backwards past the first target, do not let them
                velocity = 0;
                acceleration = 0;
                rb.velocity = Vector3.zero;
                Debug.Log("Start: " + targetMarkerIndex);
            }
            else if (targetMarkerIndex == trackMarkers.Count - 1 && (acceleration >= 0 || velocity > 0))
            { // If the player wants to trabel forwards past the last target, do not let them. 
                velocity = 0;
                acceleration = 0;
                rb.velocity = Vector3.zero;
                Debug.Log("End: " + targetMarkerIndex);
            }
            else
            { // If the user is not at a start / end target and has valid velocities then update the target
                UpdateTarget();
                Debug.Log("Other: " + targetMarkerIndex);
            }

        }

        // There is nothing to do if the velocity is zero
        if (velocity == 0) return;

        // Get the target direction of the cart, taking into account if the user wants to go FORWARD or BACKWARD
        Vector3 targetDirection = Mathf.Sign(velocity) * (trackMarkers[targetMarkerIndex].position - transform.position);
        rb.velocity = targetDirection.normalized * velocity;

        // Update the rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, trackMarkers[targetMarkerIndex].rotation, Time.deltaTime * rotationInterpolationSpeed);
        //transform.forward = Vector3.MoveTowards(transform.forward, trackMarkers[targetMarkerIndex].forward, Time.deltaTime * rotationInterpolationSpeed);
        //transform.up = Vector3.MoveTowards(transform.up, trackMarkers[targetMarkerIndex].up, Time.deltaTime * rotationInterpolationSpeed);
    }

    void FixedUpdate()
    {
        float prevVelocity = velocity;

        // Apply acceleration
        velocity += acceleration;
        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);

        // Used to detect a velocity change, and therefore switch to a new target
        if (prevVelocity != 0)
        {
            float sign1 = prevVelocity == 0 ? 0 : Mathf.Sign(prevVelocity);
            float sign2 = velocity == 0 ? 0 : Mathf.Sign(velocity);
            if (sign1 != sign2)
            {
                UpdateTarget();
            }
        }
    }

    // Gets desired cart direction from player
    void UserInput()
    {
        if (IsPlayerInCart())
        {
            if (Input.GetKey(KeyCode.E))
            {
                cartAnimator.ResetTrigger("CartRollToggle");
                cartAnimator.SetTrigger("CartUnstableToggle");
                Debug.Log("E Pressed!");
            }

            acceleration = maxAcceleration * Input.GetAxisRaw("Vertical");
            if(acceleration > 0)
            {
                cartAnimator.ResetTrigger("CartUnstableToggle");
                cartAnimator.SetTrigger("CartRollToggle");
            }
        }
        else
        {
            acceleration = 0;
            velocity = 0;
        }
    }

    // Updates mincart target depending on direcion
    void UpdateTarget()
    {
        // Can not update the target when the direction is unknown
        if (velocity == 0) return;

        if (velocity > 0 && targetMarkerIndex < trackMarkers.Count - 1)
        { // If the player is moving forward and they are not at the end of the track, then move onto next target
            targetMarkerIndex++;
        }
        else if (velocity < 0 && targetMarkerIndex > 0)
        { // If the player is moving backward and they are not at the start of the track, then move onto the next target
            targetMarkerIndex--;
        }
        else
        {
            velocity = 0;
            acceleration = 0;
        }
    }

    // Is the player in the cart?
    bool IsPlayerInCart()
    {
        return true;
        return transform.Find("Player") != null;
    }

    // Retrieve player that is in cart
    GameObject GetPlayerInCart()
    {
        if (!IsPlayerInCart()) return null;
        return transform.Find("Player").gameObject;
    }

    // Enter cart
    void Enter()
    {

    }

    // Leave cart
    void Leave()
    {

    }
}
