using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sub_Controller : MonoBehaviour
{
    private Rigidbody2D rigidBody;


    private float speed = 10;
    private float acceleration = 10;
    private float deceleration = 5;
    private float minSpeed = 0.1f;

    private float diagonalPerAxisSpeed; //calculated using speed

    private float angularSpeed = 20;
    private float angularAcceleration = 15;
    private float angularDeceleration = 25;
    private float minAngularSpeed = 0.1f;

    private float angleErrorMargin = 0.1f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        diagonalPerAxisSpeed = Mathf.Sqrt(speed * speed * 0.5f);
    }

    private void movement()
    {
        //fetch axis values for later use
        int axisUpDownValue = axisUpDown();
        int axisLeftRightValue = axisLeftRight();

        float axisSpeedCap = (Mathf.Abs(axisUpDownValue) + Mathf.Abs(axisLeftRightValue) > 1 ? diagonalPerAxisSpeed : speed);

        if (axisUpDownValue != 0 || axisLeftRightValue != 0) //accelerate if any axis != 0
        {
            Vector2 forceVector = Vector2.zero;

            if (axisUpDownValue != 0) //calculate up/down-axis force
            {
                forceVector += acceleration * axisUpDownValue * Vector2.up;
            }

            if (axisLeftRightValue != 0) //calculate left/right-axis force
            {
                forceVector += acceleration * axisLeftRightValue * Vector2.right;
            }

            if (rigidBody.velocity.magnitude < speed) rigidBody.AddForce(forceVector, ForceMode2D.Impulse); //apply force as long as max speed isnt reached
        }
        
        if (rigidBody.velocity.magnitude != 0) //check if sub is moving and can be decelerated
        {
            if ((axisLeftRightValue == 0 && rigidBody.velocity.x != 0) || Mathf.Abs(rigidBody.velocity.x) > axisSpeedCap) //decelerate x velocity if applicable
            {
                rigidBody.AddForce(-deceleration * rigidBody.velocity.x * Vector2.right, ForceMode2D.Impulse);

                if (Mathf.Abs(rigidBody.velocity.x) < minSpeed) rigidBody.velocity = new Vector2(0, rigidBody.velocity.y); //set x velocity to 0 if it is below minSpeed
            }

            if ((axisUpDownValue == 0 && rigidBody.velocity.y != 0) || Mathf.Abs(rigidBody.velocity.y) > axisSpeedCap) //decelerate y velocity if applicable
            {
                rigidBody.AddForce(-deceleration * rigidBody.velocity.y * Vector2.up, ForceMode2D.Impulse);

                if (Mathf.Abs(rigidBody.velocity.y) < minSpeed) rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0); //set y velocity to 0 if it is below minSpeed
            }
        }
    }

    private int axisUpDown()
    {
        if (Input.GetKey(Keybinds.moveUp)) return 1;
        else if (Input.GetKey(Keybinds.moveDown)) return -1;
        else return 0;
    }

    private int axisLeftRight()
    {
        if (Input.GetKey(Keybinds.moveRight)) return 1;
        else if (Input.GetKey(Keybinds.moveLeft)) return -1;
        else return 0;
    }

    private void rotation()
    {
        Vector2 viewDir = transform.rotation * Vector2.right;
        Vector2 rotationAlignmentDir = calcRotationAlignmentDir();

        float angle = Vector2.SignedAngle(viewDir, rotationAlignmentDir);
        float absAngle = Mathf.Abs(angle);

        if(absAngle > angleErrorMargin && Mathf.Abs(rigidBody.angularVelocity) < angularSpeed * Mathf.Clamp(absAngle * 0.05f, 0.8f, 2))
        {
            int sign = angle < 0 ? -1 : 1;
            rigidBody.AddTorque(sign * angularAcceleration, ForceMode2D.Impulse);
        }
        else if(rigidBody.angularVelocity != 0)
        {
            int sign = rigidBody.angularVelocity < 0 ? 1 : -1; //gets the opposite sign of angularVelocity
            rigidBody.AddTorque(sign * angularDeceleration, ForceMode2D.Impulse);

            if (Mathf.Abs(rigidBody.angularVelocity) < minAngularSpeed) rigidBody.angularVelocity = 0;
        }
    }

    private Vector2 calcRotationAlignmentDir() 
    {
        return (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z)) - transform.position).normalized;
    }

    void FixedUpdate()
    {
        movement();
        rotation();
    }
}
