using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{

    public BoidManager boidManager;

    private float boidSpeed;
    private float boidSensitivity;

    public GameObject eyes;

    public bool translate = true;
    public bool adjusting = false;
    public bool focused = false;

    public Vector3 direction;

    public BoidCluster cluster;
    public Vector3 clusterDirection;
    public bool hasCluster;
    public Color boidColor;
    public int clusterID;
    public Vector3 clusterCenterOfMass;

    public GameObject centerOfMass;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        boidManager = FindObjectOfType<BoidManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        hasCluster = cluster != null;
        boidSpeed = boidManager.boidSpeed;
        boidSensitivity = boidManager.boidSensitivity;
        direction = transform.right;
        if(hasCluster) {
            if(boidManager.clusterColour) {
                if(cluster.parentBoid == GetComponent<Boid>()) {
                    spriteRenderer.color = Color.Lerp(Color.blue, Color.green, Mathf.PingPong(Time.time, 1.0f));
                }
                else spriteRenderer.color = cluster.boidColor;//Color.Lerp(Color.white, cluster.boidColor, 1);
            }else{
                spriteRenderer.color = Color.white;
            }
            boidColor = cluster.boidColor;
            clusterID = cluster.clusterID;
            if(cluster.parentBoid == GetComponent<Boid>()) {
                cluster.updateCenterOfMass();
            }
            clusterCenterOfMass = cluster.clusterCenterOfMass;
        }else{
            spriteRenderer.color = Color.white;
        }
        move();
    }

    void move() {
        if(translate) {
            transform.Translate(transform.right * Time.deltaTime * boidSpeed, Space.World);
        }

        // the direction the boid is currently travelling in
        Vector2 path = transform.right;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1.0f);

        if(colliders.Length == 0) return;

        Vector3 newHeading = transform.right;

        if(boidManager.separation) {
            newHeading += separation(nearby);
        }

        if(boidManager.alignment) {
            newHeading += alignment(colliders);
        }else{
            if(cluster != null) cluster = null;
        }

        if(boidManager.cohesion) {
            newHeading += cohesion();
        }

        Vector3 dir = newHeading / newHeading.magnitude;
        dir.z = 0;
        dir.Normalize();

        if(boidManager.travelDirection) {
            Debug.DrawRay(transform.position, dir, Color.white);
        }

        transform.right = dir;
        

        if(boidManager.wrap) {
            wrapScreen();
        }
    }

    void wrapScreen() {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if(pos.x > 1.0) transform.position = new Vector3(-9.0f, -transform.position.y, transform.position.z);
        if(pos.x < 0.0) transform.position = new Vector3(9.0f, -transform.position.y, transform.position.z);
        if(pos.y > 1.0) transform.position = new Vector3(-transform.position.x, -4f, transform.position.z);
        if(pos.y < 0.0) transform.position = new Vector3(-transform.position.x, 4f, transform.position.z);
    }

    Vector3 cohesion() {
        Vector3 vectorToAdd = Vector2.zero;
        if(cluster != null) {
            Vector2 cmVector = cluster.clusterCenterOfMass - transform.position;
            Vector2 desiredVector = Vector2.zero;

            float regDistance = cmVector.magnitude;

            float boidToEdgeX = 9.0f - Mathf.Abs(transform.position.x);
            float boidToEdgeY = 4.0f - Mathf.Abs(transform.position.y);

            float cmToEdgeX = 9.0f - Mathf.Abs(cluster.clusterCenterOfMass.x);
            float cmToEdgeY = 4.0f - Mathf.Abs(cluster.clusterCenterOfMass.y);

            float altDistX = boidToEdgeX + cmToEdgeX;
            float altDistY = boidToEdgeY + cmToEdgeY;

            float dist = Mathf.Pow(altDistX, 2) + Mathf.Pow(altDistY, 2);
            if(Mathf.Sqrt(dist) < regDistance) {
                desiredVector = -cmVector;
            }else{
                desiredVector = cmVector;
            }

            if(boidManager.centerOfMassDirection) {
                Debug.DrawRay(transform.position, desiredVector, Color.magenta);
            }
            if(boidManager.absoluteCenterOfMassDirection) {
                Debug.DrawRay(transform.position, cmVector, Color.black);
            }

            Vector3 rotate = Vector3.RotateTowards(transform.right, desiredVector, 5.0f * Time.deltaTime, 0.0f);
            rotate.Normalize();
            vectorToAdd += rotate;

            Vector2 desiredDirection = clusterDirection;

            float distanceFromParentAxis;
            Vector2 difference = cluster.parentBoid.transform.position - transform.position;

            if(Vector2.Dot(difference, transform.right) == 0) {
                distanceFromParentAxis = difference.magnitude;
            }else{
                float angle = Vector2.Angle(difference, transform.right);
                distanceFromParentAxis = (difference.magnitude) * Mathf.Sin(angle * Mathf.Deg2Rad);
            }

                
            if(distanceFromParentAxis > boidManager.maximumAxisDistance) {
                rotate = Vector3.RotateTowards(transform.right, difference, 1.0f * Time.deltaTime, 0.0f);
                rotate.Normalize();
                vectorToAdd += rotate;
            }

            if(boidManager.rayToParent) {
                Debug.DrawRay(transform.position, difference, Color.red);
            }
            Debug.Log(distanceFromParentAxis);
        }
        return vectorToAdd;
    }

    bool canCreateCluster = true;

    Vector3 alignment(Collider2D[] colliders) {
        Vector3 vectorToAdd = Vector3.zero;
        foreach(Collider2D collider in colliders) {
            if(collider.gameObject != gameObject && canCreateCluster) {
                Boid b = collider.gameObject.GetComponent<Boid>();
                if(b.cluster == null && cluster == null) {
                    BoidCluster cluster = new BoidCluster();
                    cluster.add(GetComponent<Boid>());
                    cluster.add(b);
                }else if(cluster == null && !b.cluster.contains(GetComponent<Boid>())) {
                    b.cluster.add(GetComponent<Boid>());
                }else if(b.cluster == null && !cluster.contains(b)) {
                    cluster.add(b);
                }else if(b.cluster != cluster){
                    if(cluster.size() >= b.cluster.size()) cluster.add(b);
                    else b.cluster.add(GetComponent<Boid>());
                }
            }
        }
        if(cluster != null) {
            clusterDirection = cluster.clusterDirection;
            Vector3 rotate = Vector3.RotateTowards(transform.right, clusterDirection, 10.0f * Time.deltaTime, 0.0f);
            rotate.Normalize();
            vectorToAdd += rotate;
        }
        return vectorToAdd;
    }

    Vector3 separation(Collider2D[] nearby) {
        Vector3 vectorToAdd = Vector3.zero;
        foreach(Collider2D collider in nearby) {
            if(collider.gameObject != gameObject) {

                Vector2 direction = collider.transform.position - transform.position; // direction to surrounding boid

                Vector2 desiredDirection = -direction;

                float dot = Vector2.Dot(transform.right, direction);

                if(dot > boidSensitivity) {
                    if(!collider.gameObject.GetComponent<Boid>().adjusting) {
                        
                        Vector2 lerp = Vector2.Lerp(transform.right, desiredDirection, 0.6f);
                        Vector2 rotate = Vector3.RotateTowards(transform.right, lerp, 5.0f * Time.deltaTime, 0.0f);
                        rotate.Normalize();
                        vectorToAdd += new Vector3(rotate.x, rotate.y, 0);
                        

                        if(focused || boidManager.interactions) {
                            Debug.DrawRay(transform.position, direction, Color.blue);
                        }
                    }
                }
            }
        }
        return vectorToAdd;
    }

    IEnumerator clusterDelay() {
        canCreateCluster = false;
        yield return new WaitForEndOfFrame();
        canCreateCluster = true;
    }
}
