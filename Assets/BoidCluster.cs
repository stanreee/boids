using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoidCluster
{

    List<Boid> boids;
    public Vector3 clusterDirection;
    public Vector3 clusterCenterOfMass;

    public Color boidColor;
    public int clusterID;

    public Boid parentBoid;

    public BoidCluster() {
        boids = new List<Boid>();
        boidColor = new Color(Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), 1f);
        clusterID = Random.Range(1, 1000); // debug purposes, no practical use
    }

    public bool contains(Boid boid) {
        return boids.Contains(boid);
    }

    public void add(Boid boid){
        if(boids.Count > boid.boidManager.maximumClusterSize) return;
        boids.Add(boid);
        boid.cluster = this;
        if(boids.Count == 1) {
            clusterDirection = boid.transform.right;
            parentBoid = boid;
        }else{
            updateDirection(boid);
            updateCenterOfMass();
        }
    }

    public void remove(Boid boid) {
        boids.Remove(boid);
        boid.cluster = null;
        updateCenterOfMass();
    }

    public int size() {
        return boids.Count;
    }

    private void updateDirection(Boid b) {
        float weight = 1.0f / boids.Count;
        Vector3 newDirection = Vector3.Lerp(clusterDirection, b.transform.right, weight);
        clusterDirection = newDirection;
    }

    public void merge(BoidCluster b) {
        if(boids.Count + b.boids.Count > parentBoid.boidManager.maximumClusterSize) return;
        float weight = b.size() / (size() + b.size());
        Vector3 newDirection = Vector3.Lerp(clusterDirection, b.clusterDirection, weight);
        int old = boids.Count;
        boids = boids.Union(b.boids).ToList<Boid>();
        clusterDirection = newDirection;
        foreach(Boid boid in b.boids) {
            boid.cluster = this;
        }
    }

    public void updateCenterOfMass() {
        if(parentBoid.boidManager.cohesion) {
            Vector2 center = Vector2.zero;
            foreach(Boid b in boids) {
                center += new Vector2(b.transform.position.x, b.transform.position.y);
            }
            clusterCenterOfMass = center;
        }
    }
}
