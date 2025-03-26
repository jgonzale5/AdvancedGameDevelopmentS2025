using UnityEngine;

public class TargetSwinging : MonoBehaviour
{
    //The point on the left it will move towards
    private Vector3 leftPoint;
    //The point on the right it will move towards
    private Vector3 rightPoint;
    //The distance between its initial position and the left and right points
    public float range = 1f;

    //The current value between 0 and 1, used to calculate its position
    private float positionLerp = 0.5f;
    //Whether its moving left or right
    private bool movingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Calculate the left and right point by getting the point on the left and the right of the target
        leftPoint = transform.position - transform.right * range;
        rightPoint = transform.position + transform.right * range;
    }

    // Update is called once per frame
    void Update()
    {
        //We update the position of this object to match its current positionLerp value
        transform.position = Vector3.Lerp(leftPoint, rightPoint, positionLerp);

        //If it's moving right
        if (movingRight)
        {
            //Add the delta seconds
            positionLerp += Time.deltaTime;

            //If it has reached the right, we set it to that position and tell it to move left after this
            if (positionLerp > 1)
            {
                movingRight = false;
                positionLerp = 1;
            }
        }
        else
        {
            //Subtract the delta seconds
            positionLerp -= Time.deltaTime;

            //If it has reached the left, we set it to that position and tell it to move right after this
            if (positionLerp < 0)
            {
                movingRight = true;
                positionLerp = 0;
            }
        }
    }
}
