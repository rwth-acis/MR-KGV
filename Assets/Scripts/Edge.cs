using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour {

    public GameObject subjectNode;

    public GameObject objectNode;

    public string uri;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start() {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update() {
        // Update the start and end point of the line
        lineRenderer.SetPosition(0, subjectNode.transform.position);
        lineRenderer.SetPosition(1, objectNode.transform.position);

        // Calculate midpoint between the two nodes of the edge
        //Vector3 midpoint = Vector3.Lerp(subjectNode.transform.position, objectNode.transform.position, 0.5f);

        // Update the position of the text to the midpoint
        //this.transform.position = midpoint;

        // Update the rotation of the text
        transform.rotation = Camera.main.transform.rotation;
    }
}
