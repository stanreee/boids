using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{

    public float boidSensitivity = 0.9f;
    public float boidSpeed = 5f;

    public Boid[] boids;

    public GameObject boidPrefab;

    public bool separation = true;
    public bool alignment = true;
    public bool cohesion = true;

    public bool spawn = true;

    public bool wrap = true;
    
    [Header("Separation")]
    [Header("Debug Settings")]
    public bool interactions = false;
    [Header("Alignment")]
    public bool clusterColour = true;
    [Header("Cohesion")]
    public bool centerOfMassDirection = false;
    public bool absoluteCenterOfMassDirection = false;
    public bool rayToParent = false;
    [Header("General Boid Settings")]
    public bool travelDirection = false;

    [Header("Cluster Settings")]
    public int maximumClusterSize = 35;
    public float maximumAxisDistance = 0.25f;

    [Header("Boid Range")]
    public int[] boidRange = new int[2];

    [HideInInspector]
    public int maxBoids;
    public bool simulating = true;

    void Start()
    {
        if(spawn) spawnRandomBoids();
    }

    void spawnRandomBoids() {
        maxBoids = Random.Range(boidRange[0], boidRange[1]);
        boids = new Boid[maxBoids];
        int numBoids = 0;

        float xSpawn = Random.Range(-8, 8);
        float ySpawn = Random.Range(-4, 4);

        while(numBoids != maxBoids) {
            GameObject boid = Instantiate(boidPrefab, new Vector3(xSpawn, ySpawn, 0), Quaternion.Euler(0, 0, Random.Range(0, 360)));
            xSpawn = Random.Range(-8, 8);
            ySpawn = Random.Range(-4, 4);
            boids[numBoids] = boid.GetComponent<Boid>();
            numBoids++;
        }
    }

    public void stopSimulation() {
        foreach(Boid b in boids) {
            Destroy(b.gameObject);
        }
        boids = new Boid[0];
        simulating = false;
    }

    public void startSimulation() {
        spawnRandomBoids();
        simulating = true;
    }
}
