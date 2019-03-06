using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {
    public GameObject ObstaclePrefab;
    public GameObject boidPrefab;
    public int numberOfBoids = 20;
    public int numberOfObstacles = 10;
    public GameObject[] boids;
    public float spawnRadius = 10.0f;
    public float speed = 1.0f;
    public float turnspeed = 30.0f;
    public float FOV = 60; // degrees
    public float NeighborDistanceSquared = 64.0f; // avoid sqrt
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 0.0f;
    public float avoidanceWeight = 1.0f;
    public float noise = 0.1f;
    public float AvoidMininum = 3.0f;
    public GameObject target;
    // Use this for initialization
    void Start () {
        // Instantiate the first obstacle right on top of the flock position
        Instantiate(ObstaclePrefab, transform.position, Random.rotation);

        //Randomize position of obstacles
        for (int i = 0; i < numberOfObstacles; i++)
        {
            // Spawn within a range called spawnRadius
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Instantiate(ObstaclePrefab, pos, Random.rotation);
        }

        //Create an array with 20(numberOfBoids) boids 
        boids = new GameObject[numberOfBoids];
        for (int i = 0; i < numberOfBoids; i++)
        {
            // Spawn within a range called spawnRadius
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            boids[i] = (GameObject) Instantiate(boidPrefab, pos,Random.rotation);

            // Get Boid Script component of this boid object, Assign each boid 's flock is this flock
            boids[i].GetComponent<Boid>().flock = this;
            boids[i].GetComponent<Boid>().name = "Boid " + i;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
