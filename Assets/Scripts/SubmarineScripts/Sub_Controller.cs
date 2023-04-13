using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sub_Controller : MonoBehaviour
{
    private Rigidbody rigidBody;


    private float speed = 10;
    private float acceleration = 10;
    private float deceleration = 5;
    private float minSpeed = 0.1f;

    private float diagonalPerAxisSpeed; //calculated using speed

    private float angularSpeed = 1;
    private float angleErrorMargin = 0.1f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        diagonalPerAxisSpeed = Mathf.Sqrt(speed * speed * 0.5f);
    }

    /// <summary>
    /// Moves the submarine using the controls from Keybinds class.
    /// </summary>
    private void move()
    {
        //fetch axis values for later use
        int axisUpDownValue = axisUpDown();
        int axisLeftRightValue = axisLeftRight();

        float axisSpeedCap = (Mathf.Abs(axisUpDownValue) + Mathf.Abs(axisLeftRightValue) > 1 ? diagonalPerAxisSpeed : speed);

        if (axisUpDownValue != 0 || axisLeftRightValue != 0) //accelerate if any axis != 0
        {
            Vector2 forceVector = Vector2.zero;

            if (axisUpDownValue != 0)       //calculate up/down-axis force
            {
                forceVector += acceleration * axisUpDownValue * Vector2.up;
            }

            if (axisLeftRightValue != 0)    //calculate left/right-axis force
            {
                forceVector += acceleration * axisLeftRightValue * Vector2.right;
            }

            if (rigidBody.velocity.magnitude < speed) rigidBody.AddForce(forceVector, ForceMode.Impulse); //apply force as long as max speed isnt reached
        }
        
        if (rigidBody.velocity.magnitude != 0) //check if sub is moving and can be decelerated
        {
            if ((axisLeftRightValue == 0 && rigidBody.velocity.x != 0) || Mathf.Abs(rigidBody.velocity.x) > axisSpeedCap)   //decelerate x velocity if applicable
            {
                rigidBody.AddForce(-deceleration * rigidBody.velocity.x * Vector2.right, ForceMode.Impulse);

                if (Mathf.Abs(rigidBody.velocity.x) < minSpeed) rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);  //set x velocity to 0 if it is below minSpeed
            }

            if ((axisUpDownValue == 0 && rigidBody.velocity.y != 0) || Mathf.Abs(rigidBody.velocity.y) > axisSpeedCap)      //decelerate y velocity if applicable
            {
                rigidBody.AddForce(-deceleration * rigidBody.velocity.y * Vector2.up, ForceMode.Impulse);

                if (Mathf.Abs(rigidBody.velocity.y) < minSpeed) rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);  //set y velocity to 0 if it is below minSpeed
            }
        }
    }

    private int axisUpDown()
    {
        if (Input.GetKey(Keybinds.MoveUp)) return 1;
        else if (Input.GetKey(Keybinds.MoveDown)) return -1;
        else return 0;
    }

    private int axisLeftRight()
    {
        if (Input.GetKey(Keybinds.MoveRight)) return 1;
        else if (Input.GetKey(Keybinds.MoveLeft)) return -1;
        else return 0;
    }

    /// <summary>
    /// Rotates the submarine towards the cursor.
    /// </summary>
    private void rotate()
    {
        if (!Input.GetKey(Keybinds.Aim))
        {
            if (rigidBody.angularVelocity.z != 0) rigidBody.angularVelocity = Vector3.zero;
            return;
        }

        Vector2 viewDir = transform.rotation * Vector2.right;                   //get current view direction vector
        Vector2 rotationAlignmentDirection = calcRotationAlignmentDirection();  //get the direction vector to rotate towards

        float angle = Vector2.SignedAngle(viewDir, rotationAlignmentDirection); //get the signed angle between the view direction and alignment direction
        float absAngle = Mathf.Abs(angle);                                      //get the absolute angle
        int sign = angle < 0 ? -1 : 1;                                          //get the sign of the signed angle


        //accelerate towards alignment direction if not aligned, otherwise decelerate to stop rotation
        if(absAngle > angleErrorMargin)
        {
            rigidBody.angularVelocity = Vector3.forward * sign * angularSpeed * Mathf.Lerp(1, 0.1f, 1 / absAngle);
        }
        else if(rigidBody.angularVelocity.z != 0)
        {
            rigidBody.angularVelocity = Vector3.zero;
        }

    }

    /// <summary>
    /// Calculate the rotation alignment direction using the cursor.
    /// </summary>
    /// <returns></returns>
    private Vector2 calcRotationAlignmentDirection()
    {
        return (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z)) - transform.position).normalized;
    }

    private float calculateDecelerationDistance()
    {
        // Check if the game object is moving
        if (rigidBody.velocity != Vector3.zero)
        {
            // Calculate the time required to decelerate the game object to zero velocity
            float decelerationTime = rigidBody.velocity.magnitude / deceleration;

            // Calculate the distance required to decelerate the game object to zero velocity
            float decelerationDistance = (rigidBody.velocity.magnitude * decelerationTime) / 2f;

            return decelerationDistance;
        }

        return 0;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J)) 
        {
            Debug.Log(calculateDecelerationDistance());
        }
    }

    void FixedUpdate()
    {
        move();
        rotate();
    }
}
