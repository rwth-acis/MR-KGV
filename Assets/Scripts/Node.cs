using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Node : MonoBehaviour {

    public string annotation = "";

    public string uri = "";

    public string imageURL = "";

    //public string modelURL;

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

    private Color transparentRed = new Color(1, 0, 0, 0.5f);
    private Color transparentWhite = new Color(1, 1, 1, 0.5f);

    // Start is called before the first frame update
    void Start() {
        // Find text mesh
        textMesh = this.GetComponent<TextMesh>();

        // Find canvas
        Canvas arScreen = GameObject.Find("ARScreen").GetComponent<Canvas>();

        // Find annotation screen, options screen, and menu button
        annotationScreen = arScreen.transform.Find("AnnotationScreen").gameObject;
        optionsScreen = arScreen.transform.Find("OptionsScreen").gameObject;
        arButton = arScreen.transform.Find("OptionsButton").gameObject;
    }

    // Update is called once per frame
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
                annotationScreen.GetComponent<AnnotationScreenButton>().selectedNode.UnselectedColor();
            }

            // Change color to indicate selected state
            SelectedColor();

            // Pass necessary values BEFORE annotation screen is activated
            //GameObject.Find("Label").GetComponent<TMPro.TextMeshProUGUI>().text = uri;

            // Deactivate options button, activate annotation screen (if needed) and pass on this node
            annotationScreen.SetActive(true);
            annotationScreen.GetComponent<AnnotationScreenButton>().selectedNode = this;
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
        }

        textMesh.color = Color.red;
    }

    public void UnselectedColor() {
        foreach (GameObject edge in edges) {
            edge.GetComponent<LineRenderer>().material.color = transparentWhite;
        }

        textMesh.color = Color.black;
    }
}
