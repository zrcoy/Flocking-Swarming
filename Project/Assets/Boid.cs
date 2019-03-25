using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {
    public Flock flock;
    float speed = 0;
    public Vector3 direction = Vector3.zero;
    public Vector3 forward = Vector3.zero;
    public int NeighborCount = 0;
    Vector3 noise = Vector3.zero;
    public string name;
    //public Vector3
	// Use this for initialization
	void Start () {
        // set the speed the same as the whole flock
        speed = flock.speed;
	}
	
	// Update is called once per frame
	void Update () {
        //Get the object this script component is attached to , then get its forward direction
        forward = gameObject.transform.forward;
        Motivate();
	}
    private void LateUpdate()
    {
        Move();
    }

    bool isNeighBor(Boid b)
    {
        if (b == this)
        {
            return false;
        }
        // distance check
        Vector3 offset = b.gameObject.transform.position- transform.position;
        //distance square
        float ds = offset.sqrMagnitude;
        if (ds > flock.NeighborDistanceSquared)
        {
            return false;
        }
        // line of sight check
        float angle = Vector3.Angle(offset, transform.forward);
        // idk: Fov should be the half of the whole sight range angle?
        if (angle > flock.FOV)
        {
            return false;
        }

        return true;
    }

    void Move()
    {
        // rotate
        float turnspeed = flock.turnspeed;
        // rotate to the target direction which is direction in this case
        Quaternion turnDirection = Quaternion.FromToRotation(Vector3.forward,direction);
        transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, turnDirection, Time.deltaTime* turnspeed);
        // then move

        transform.position = transform.position + transform.forward * speed * Time.deltaTime;
    }

    void Motivate()
    {
        // not run into stuff is top priority
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward * 2, out hit))
        {
            //when hit something a red line is drawn
            Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
            //get the normal of the surcface the ray hit by hit.normal, inDerect is forward vector, so that the result is the reflected vector 
            direction = Vector3.Reflect(transform.forward, hit.normal);
            return;
            // we need to turn away from it
        }
        Vector3 Alignment = Vector3.zero;
        Vector3 Cohesion = Vector3.zero;
        Vector3 Avoidance = Vector3.zero;
        NeighborCount = 0;
        foreach (GameObject g in flock.boids)
        {
            //b is neighbour object's script component, g is neighbour object
            Boid b = g.GetComponent<Boid>();
            if (isNeighBor(b))
            {
                NeighborCount++;
                //add up every neighbour's position
                Cohesion += g.transform.position;
                //add up every neighbour's forward direction
                Alignment += g.transform.forward;
                // if two object too close
                if (Vector3.Distance(g.transform.position, transform.position) < flock.AvoidMininum)
                {
                    //add up distance between neighbour and current object
                    Avoidance += (transform.position - g.transform.position);
                }
            }
        }
        // make sure there's at least one neighbour, even some boids are not neighbours and don't have hit result on the raycast, they are still affected by acohesion and alignment
        if (NeighborCount != 0)
        {
            // average cohesion and alignment
            Cohesion = Cohesion / NeighborCount;
            Alignment = Alignment / NeighborCount;
        }
        // normalize the three motivations

        // delta position between the cohesion the current boid position
        Cohesion = Cohesion - transform.position;
        // delta position between the target and current boid position
        Vector3 SwarmDir = flock.target.transform.position - transform.position;
        //Alignment = Vector3.Normalize(Alignment);
        //Avoidance = Vector3.Normalize(Avoidance);
        //if (Random.Range(0, 100) < 5)
        //{
        //    noise = UnityEngine.Random.onUnitSphere * flock.noise * (flock.boids.Length - NeighborCount);
        //}
        // and add and scale them
        direction = SwarmDir + Cohesion * flock.cohesionWeight + Alignment * flock.alignmentWeight + Avoidance * flock.avoidanceWeight + noise;
        // then renormalize again
        direction = Vector3.Normalize(direction);
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position+direction, 0.1f);
    //}
}
