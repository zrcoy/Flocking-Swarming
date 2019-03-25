using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotCar : MonoBehaviour
{
    public Catmul spline;
    // Use this for initialization
    public float speed = 0.00f;
    public float maxrotationspeed = 0.4f; // radians
    public float angle;
    public float crashLimit = 5.0f;
    public int nextwaypoint = 1;
    public float closeEnough = 0.1f;
    public float prevAngle;
    public int randPath = 1;//1 is middle, 2 is left lane,  3 is right
    public int splineIndex = 1;
    public int clockwise = 1;
    void Start()
    {
        //Vector3 pos = spline.newPoints1[0];
        //gameObject.transform.position = pos;
        //Vector3 next = spline.newPoints1[1];
        //Vector3 direction = next - pos;

        //Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, direction, maxrotationspeed, 0.0f);
        //transform.rotation = Quaternion.LookRotation(newDir);
        //prevAngle = 0.0f;
        Vector3 pos = spline.getPoint(splineIndex, randPath, 0);
        gameObject.transform.position = pos;
        Vector3 next = spline.getPoint(splineIndex, randPath, 0);
        Vector3 direction = next - pos;

        Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, direction, maxrotationspeed, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        prevAngle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        AdjustSpeed();
        // try to move towards the next point
        // if close enough, select the subsequent point to get to.
        // we also need to know if we've gone to fast for the track.  For now, lets not worry about that, and just
        // animate along it.
        Vector3 pos = gameObject.transform.position;
        Vector3 next = new Vector3(0, 0, 0);
        if(splineIndex==1)
        {
            if (randPath == 1)
            {
                next = spline.newPoints1[nextwaypoint];
            }
            if (randPath == 2)
            {
                next = spline.leftPoints1[nextwaypoint];
            }
            if (randPath == 3)
            {
                next = spline.rightPoints1[nextwaypoint];
            }
        }
        else if(splineIndex==2)
        {
            if (randPath == 1)
            {
                next = spline.newPoints2[nextwaypoint];
            }
            if (randPath == 2)
            {
                next = spline.leftPoints2[nextwaypoint];
            }
            if (randPath == 3)
            {
                next = spline.rightPoints2[nextwaypoint];
            }
        }
        else if (splineIndex==3)
        {
            if (randPath == 1)
            {
                next = spline.newPoints3[nextwaypoint];
            }
            if (randPath == 2)
            {
                next = spline.leftPoints3[nextwaypoint];
            }
            if (randPath == 3)
            {
                next = spline.rightPoints3[nextwaypoint];
            }
        }
        //distance to next waypoint
        float dist = Vector3.Distance(next, pos);
        if (dist < 0.03f)
        {
            //switch to a random path
            randPath = Random.Range(1, 4);
        }
        Vector3 direction = next - pos;
        angle = Vector3.Angle(direction, gameObject.transform.forward);
        if (angle > prevAngle)
        {
        
            nextwaypoint = (nextwaypoint + clockwise + spline.getCount(splineIndex)) % spline.getCount(splineIndex);

            next = spline.getPoint(splineIndex, randPath, nextwaypoint);
            direction = next - pos;
        }

        // Handle special waypoint case
        // now the car is on the middle spline and middle lane
        if (randPath == 1)
        {
            // continue along middle spline, which means do nothing

        }
   

        else if (randPath == 2 || randPath == 3) // only check if on the outside lanes
        {
            int crossingSplineNumber = -1;
            int crossIndex = spline.FindCrossingSpline(splineIndex, spline.getPoint(splineIndex, randPath, nextwaypoint), out crossingSplineNumber);
            if (crossIndex != -1) // we found a matching point where another spline crosses ours
            {
                // at this point, we know what spline we want to turn onto. that is in
                // crossingSplineNumber. 
                // we also know where we were along that line when we crossed.
                // crossIndex is the equivalent of nextWaypoint on that spline.
                // find the vector between spline.getPoint(splineIndex,randPath,nextwaypoint) and spline.getPoint(splineIndex,randPath,nextwaypoint+1)
                // this is your A vector.
                // do same but using crossingSplineNumber instead of spline index
                // this is your B vector.
                // find the angle from A to B
                Vector3 A = spline.getPoint(splineIndex, randPath, nextwaypoint + 1) - spline.getPoint(splineIndex, randPath, nextwaypoint);
                Vector3 B = spline.getPoint(crossingSplineNumber, 1, crossIndex + 1) - spline.getPoint(crossingSplineNumber, 1, crossIndex);
                float AB = Vector3.SignedAngle(A, B, Vector3.up);
                // now start on your 8 if else statements.  When you decide which way you want to turn,
                // set splineIndex to crossingSplineIndex, set nextwaypoint to crossIndex, and set clockwise like below
                splineIndex = crossingSplineNumber;
                nextwaypoint = crossIndex;
                if (randPath == 2 && clockwise == 1 && AB < 0) // turning left, was going in +ve direction along A, need to turn to +ve direction on B
                {
                    clockwise = 1; // we are now moving along B in the positive direction
                }
                else if (randPath == 2 && clockwise == 1 && AB > 0) // left lane, positive dir, turning right
                {
                    clockwise = -1; // negative on B
                }
                else if (randPath == 2 && clockwise == -1 && AB < 0)
                {
                    clockwise = -1;
                }
                else if (randPath == 2 && clockwise == -1 && AB > 0)
                {
                    clockwise = 1;
                }
                else if (randPath == 3 && clockwise == 1 && AB < 0)//right lane, positive dir, turning left
                {
                    clockwise = 1;
                }
                else if (randPath == 3 && clockwise == 1 && AB > 0)
                {
                    clockwise = -1;
                }
                else if (randPath ==3 && clockwise ==-1 && AB<0) 
                {
                    clockwise = -1;
                }
                else if (randPath==3 && clockwise ==-1 && AB>0)
                {
                    clockwise = 1;
                }
            }
        }



        Vector3 newDir = Vector3.RotateTowards(gameObject.transform.forward, direction, maxrotationspeed, 0.0f);
        gameObject.transform.rotation = Quaternion.LookRotation(newDir);
        gameObject.transform.Translate(Vector3.forward * speed);

        prevAngle = angle;



    }

    void AdjustSpeed()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            speed += 0.02f;
            if (speed > 0.3f)
            {
                speed = 0.3f;
            }
        }
        else if (d < 0f)
        {
            speed -= 0.02f;
            if (speed < 0.0f)
            {
                speed = 0.0f;
            }
        }
    }

    void CheckForCrash()
    {
        if (speed * angle > crashLimit)
        {
            Debug.Log(speed * angle);
            Debug.Log("Crashed");
        }
        else if (speed * angle > crashLimit * .7f)
        {
            Debug.Log(speed * angle);
            Debug.Log("Skidding");
        }
    }
}
