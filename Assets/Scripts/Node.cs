using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public string annotation;

    public string uri;

    public string imageURL;

    public string modelURL;

    // Type based on 'halle' ontology
    public string type;

    private TextMesh textMesh;

    // Start is called before the first frame update
    void Start() {
        textMesh = this.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update() {
        // Update the rotation of the text
        transform.rotation = Camera.main.transform.rotation;
    }

}
