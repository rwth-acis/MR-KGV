using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Node : MonoBehaviour {
    // Node data
    public string annotation = "";
    public string uri = "";
    public string label = "";
    public string imageURL = "";
    public string modelURL = "";

    private TextMesh textMesh;

    // AR UI related GOs
    private GameObject annotationScreen;
    private GameObject optionsScreen;
    private GameObject arButton;

    // Double tap variables
    private bool tapped = false;
    private float tapTime;
    private float tapDelay = 0.5f;

    // List of outgoing edges
    public List<GameObject> edges = new List<GameObject>();

    // Color for outgoing edges
    private Color transparentRed = new Color(1, 0, 0, 0.5f);
    private Color transparentWhite = new Color(1, 1, 1, 0.5f);

    void Start() {
        // Find text mesh
        textMesh = this.GetComponent<TextMesh>();

        // Find canvas
        Canvas mainScreen = GameObject.Find("MainScreen").GetComponent<Canvas>();

        // Find annotation screen, options screen, and menu button
        annotationScreen = mainScreen.transform.Find("AnnotationScreen").gameObject;
        optionsScreen = mainScreen.transform.Find("OptionsScreen").gameObject;
        arButton = mainScreen.transform.Find("OptionsButton").gameObject;
    }

    void Update() {
        // Update the rotation of the text
        transform.rotation = Camera.main.transform.rotation;
    }

    void OnMouseDown() {
        //Debug.Log(edges.Count);
        if (tapped) {
            if (optionsScreen.activeSelf) {
                optionsScreen.SetActive(false);
            }

            // If another node is still selected, unselect the other node
            if (annotationScreen.activeSelf) {
                annotationScreen.GetComponent<AnnotationScreen>().CloseAnnotationScreen();
            }

            // Change color to indicate selected state
            SelectedColor();

            // First reference the node, then activate annotation screen and deactivate options button
            annotationScreen.GetComponent<AnnotationScreen>().selectedNode = this;
            annotationScreen.SetActive(true);
            arButton.SetActive(false);
        } else {
            tapped = true;
            StartCoroutine(ResetDoubleTap());
        }
    }

    IEnumerator ResetDoubleTap() {
        yield return new WaitForSeconds(tapDelay);
        tapped = false;
    }

    void OnMouseDrag() {
        float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
    }

    public void SelectedColor() {
        foreach (GameObject edge in edges) {
            edge.GetComponent<LineRenderer>().material.color = transparentRed;
            edge.GetComponent<TextMesh>().color = Color.red;
        }

        textMesh.color = Color.red;
    }

    public void UnselectedColor() {
        foreach (GameObject edge in edges) {
            edge.GetComponent<LineRenderer>().material.color = transparentWhite;
            edge.GetComponent<TextMesh>().color = Color.white;
        }

        textMesh.color = Color.black;
    }
}
