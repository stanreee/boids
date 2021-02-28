using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{

    public Button hideMenuButton;
    public Button separationButton;
    public Button alignmentButton;
    public Button cohesionButton;
    public Button clusterColourButton;
    public Button toggleMenuButton;
    public Button restartSimulationButton;

    public Slider clusterSizeSlider;
    public Slider axisDistanceSlider;

    public Slider minBoids;
    public Slider maxBoids;

    public BoidManager boidManager;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayedStart());
    }

    IEnumerator delayedStart() {
        yield return new WaitForEndOfFrame();
        clusterSizeSlider.maxValue = boidManager.maxBoids;
        clusterSizeSlider.minValue = 50;
        clusterSizeSlider.value = boidManager.maximumClusterSize;
        clusterSizeSlider.GetComponentInChildren<Text>().text = "Max. Cluster Size: " + clusterSizeSlider.value;

        axisDistanceSlider.value = 0.25f;
        axisDistanceSlider.maxValue = 1f;
        axisDistanceSlider.minValue = 0f;
        axisDistanceSlider.GetComponentInChildren<Text>().text = "Max Axis Dist: " + axisDistanceSlider.value;

        separationButton.GetComponentInChildren<Text>().text = boidManager.separation ? "Disable Separation" : "Enable Separation";
        alignmentButton.GetComponentInChildren<Text>().text = boidManager.alignment ? "Disable Alignment" : "Enable Alignment";
        cohesionButton.GetComponentInChildren<Text>().text = boidManager.cohesion ? "Disable Cohesion" : "Enable Cohesion";
        clusterColourButton.GetComponentInChildren<Text>().text = boidManager.clusterColour ? "Disable Cluster Colour" : "Enable Cluster Colour";

        maxBoids.minValue = 0;
        maxBoids.maxValue = 500;
        maxBoids.value = boidManager.boidRange[1];
        maxBoids.GetComponentInChildren<Text>().text = "Max Boids: " + maxBoids.value;

        minBoids.maxValue = maxBoids.value;
        minBoids.minValue = 0;
        minBoids.value = boidManager.boidRange[0];
        minBoids.GetComponentInChildren<Text>().text = "Min Boids: " + minBoids.value;
        
        setPreSimulationMenu(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeClusterSize() {
        boidManager.maximumClusterSize = (int) clusterSizeSlider.value;
        clusterSizeSlider.GetComponentInChildren<Text>().text = "Max. Cluster Size: " + clusterSizeSlider.value;
    }

    public void changeAxisDistance() {
        boidManager.maximumAxisDistance = (int) axisDistanceSlider.value;
        axisDistanceSlider.GetComponentInChildren<Text>().text = "Max Axis Dist: " + axisDistanceSlider.value;
    }

    public void changeMinBoidValue() {
        boidManager.boidRange[0] = (int) minBoids.value;
        minBoids.GetComponentInChildren<Text>().text = "Min Boids: " + minBoids.value;
    }

    public void changeMaxBoidValue() {
        boidManager.boidRange[1] = (int) maxBoids.value;
        maxBoids.GetComponentInChildren<Text>().text = "Max Boids: " + maxBoids.value;
        minBoids.maxValue = maxBoids.value;
        minBoids.GetComponentInChildren<Text>().text = "Min Boids: " + minBoids.value;
    }

    public void setSimulationMenu(bool active) {
        alignmentButton.gameObject.SetActive(active);
        separationButton.gameObject.SetActive(active);
        cohesionButton.gameObject.SetActive(active);
        clusterColourButton.gameObject.SetActive(active);
    }

    public void setPreSimulationMenu(bool active) {
        maxBoids.gameObject.SetActive(active);
        minBoids.gameObject.SetActive(active);
        clusterSizeSlider.gameObject.SetActive(active);
        axisDistanceSlider.gameObject.SetActive(active);
    }

    public void hideMenu() {
        restartSimulationButton.gameObject.SetActive(!alignmentButton.gameObject.active);
        setSimulationMenu(!alignmentButton.gameObject.active);
    }

    public void disableSimulationMenu() {
        setSimulationMenu(false);
        toggleMenuButton.gameObject.SetActive(false);
    }

    public void toggleSimulation() {
        if(boidManager.simulating) {
            restartSimulation();
        }else{
            startSimulation();
        }
    }

    public void restartSimulation() {
        disableSimulationMenu();
        restartSimulationButton.GetComponentInChildren<Text>().text = "Start Simulation";
        setPreSimulationMenu(true);
        boidManager.stopSimulation();
    }

    public void startSimulation() {
        setSimulationMenu(true);
        toggleMenuButton.gameObject.SetActive(true);
        setPreSimulationMenu(false);
        restartSimulationButton.GetComponentInChildren<Text>().text = "Restart Simulation";
        boidManager.startSimulation();

        if(!boidManager.separation) toggleSeparation();
        if(!boidManager.alignment) toggleAlignment();
        if(!boidManager.cohesion) toggleCohesion();
    }

    public void toggleSeparation() {
        boidManager.separation = !boidManager.separation;
        separationButton.GetComponentInChildren<Text>().text = boidManager.separation ? "Disable Separation" : "Enable Separation";
    }

    public void toggleAlignment() {
        boidManager.alignment = !boidManager.alignment;
        alignmentButton.GetComponentInChildren<Text>().text = boidManager.alignment ? "Disable Alignment" : "Enable Alignment";
        if(boidManager.cohesion) toggleCohesion();
    }

    public void toggleCohesion() {
        boidManager.cohesion = !boidManager.cohesion;
        cohesionButton.GetComponentInChildren<Text>().text = boidManager.cohesion ? "Disable Cohesion" : "Enable Cohesion";
        if(!boidManager.alignment) toggleAlignment();
    }

    public void toggleClusterColour() {
        boidManager.clusterColour = !boidManager.clusterColour;
        clusterColourButton.GetComponentInChildren<Text>().text = boidManager.clusterColour ? "Disable Cluster Colour" : "Enable Cluster Colour";
    }

    public void exit() {
        Application.Quit();
    }

}
