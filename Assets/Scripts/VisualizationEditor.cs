using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.IO;
using System.Net;

public class VisualizationEditor : MonoBehaviour {
    // Radius in which nodes get initialized around the center point
    public float radius;

    // Center point of the graph visualization; used in layout
    public GameObject centerPoint;

    // Prefabs
    public GameObject edgePrefab;
    public GameObject nodePrefab;
    public GameObject sphereRepresentationPrefab;
    public GameObject imageRepresentationPrefab;

    // This dictionary stores the corresponding game objects of the nodes in the graph 
    private Dictionary<INode, GameObject> nodes = new Dictionary<INode, GameObject>();

    // This dictionary stores the corresponding game objects of the edges in the graph
    private Dictionary<Triple, GameObject> edges = new Dictionary<Triple, GameObject>();

    // Currently loaded graph
    private IGraph graph = new Graph();

    private GameObject graphGO;

    private Dictionary<string, string> imageURLs = new Dictionary<string, string>();

    Layout layout;

    void Start() {
        //TextAsset turtleFile = Resources.Load("example-modell") as TextAsset;
        //string turtleFileContents = turtleFile.text;

        string savingPath = Application.persistentDataPath + "/rdf";

        Directory.CreateDirectory(savingPath);

        //string loadPath = savingPath + "/example-modell.ttl";
        string loadPath = savingPath + "/Neuroscience-modell.ttl";

        ReadTurtleFile(loadPath);

        Debug.Log(loadPath);

        graphGO = GameObject.Find("Graph");

        InitializeGraph();

        // First child
        InitializeSphereRepresentation();

        // Second child
        InitializeImageRepresentation();

        layout = GameObject.Find("LayoutHandler").GetComponent<Layout>();
        layout.ForceDirectedLayout(nodes, edges, centerPoint, radius);

        InitializeImageURLs();

        FetchImageURLsFromDic();

        //ActivateImageRepresentation();
    }

    void Update() {
        
    }

    public void InitializeImageURLs() {
        imageURLs.Add("Progress", "https://i.guim.co.uk/img/media/9c4164a3454b77912be9ad36a90f79885075eb34/9_46_1423_854/master/1423.jpg?width=1200&height=900&quality=85&auto=format&fit=crop&s=02d0e75f8c738d515cfa5ccf7f2ceebc");
        imageURLs.Add("Signals", "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f8/Polynomialdeg2.svg/1200px-Polynomialdeg2.svg.png");
    }

    public void FetchImageURLsFromDic() {
        foreach (GameObject node in nodes.Values) {
            Node nodeComponent = node.GetComponent<Node>();

            //nodeComponent.annotation = "test";

            imageURLs.TryGetValue(nodeComponent.label, out nodeComponent.imageURL);
        }
    }

    public void LayoutRedirect() {
        layout.ForceDirectedLayout(nodes, edges, centerPoint, radius);
    }

    // This function takes the path to a Turtle file and parses the graph data into 'graph'
    public void ReadTurtleFile(string filePath) {
        try {
            // Create a new instance of the Turtle parser
            TurtleParser parser = new TurtleParser();

            parser.Load(graph, filePath);
        }
        catch (RdfParseException parseEx) {
            //This indicates a parser error e.g unexpected character, premature end of input, invalid syntax etc.
            Debug.Log("Parser Error");
            Debug.Log(parseEx.Message);
        }
        catch (RdfException rdfEx) {
            //This represents a RDF error e.g. illegal triple for the given syntax, undefined namespace
            Debug.Log("RDF Error");
            Debug.Log(rdfEx.Message);
        }
    }

    // Return the substring that comes after the character '/'
    public string SplitString(string s) {
        int lastIndex = s.LastIndexOf('/');

        return s.Substring(lastIndex + 1);
    }

    // Define a method to initialize the graph visualization
    public void InitializeGraph() {
        // Loop through the nodes in the graph and create a node object for each one
        foreach (INode node in graph.Nodes) {
            // Only create nodes if it is a uri node
            if (node.NodeType == NodeType.Uri) {
                // Cast node
                IUriNode uriNode = (IUriNode)node;

                // If term node, do not create
                if (uriNode.Uri.ToString() == "http://halle/ontology/Term") {
                    break;
                }

                // Generate random coordinates within the specified radius around the center point
                float x = Random.Range(-radius + centerPoint.transform.position.x, radius + centerPoint.transform.position.x);
                float y = Random.Range(-radius + centerPoint.transform.position.y, radius + centerPoint.transform.position.y);
                float z = Random.Range(-radius + centerPoint.transform.position.z, radius + centerPoint.transform.position.z);

                // Create a node prefab at a random position within the specified radius
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(x, y, z), Quaternion.identity);

                // Store the node object in the dictionary
                nodes[node] = nodeObject;

                // Set the scale of the node object
                nodeObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                // Get the text mesh of the node
                TextMesh text = nodeObject.GetComponent<TextMesh>();

                // Set the text of the node label
                text.text = SplitString(uriNode.Uri.ToString());

                // Get the script of the node object
                Node nodeScript = nodeObject.GetComponent<Node>();

                // Store the uri and label of the node
                nodeScript.uri = uriNode.Uri.ToString();
                nodeScript.label = SplitString(uriNode.Uri.ToString());

                // Sharpen the text by resizing it
                text.fontSize = 150;
                text.characterSize = .02f;

                nodeObject.transform.SetParent(graphGO.transform);

                nodeObject.layer = LayerMask.NameToLayer("GraphElements");
            }
        }

        // Loop through the triples in the graph and create an edge object for each one
        foreach (Triple edge in graph.Triples) {
            // Only create edges between uri nodes that were previously created
            if (nodes.ContainsKey(edge.Subject) && nodes.ContainsKey(edge.Object)) {
                // Create an edge prefab
                GameObject edgeObject = Instantiate(edgePrefab);

                // Store the edge object in the dictionary
                edges[edge] = edgeObject;

                // Get the line renderer of the edge
                LineRenderer lineRenderer = edgeObject.GetComponent<LineRenderer>();

                // Set the position of the line's start and end points using the positions stored in the dictionary
                lineRenderer.SetPosition(0, nodes[edge.Subject].transform.position);
                lineRenderer.SetPosition(1, nodes[edge.Object].transform.position);

                // Get the text mesh of the edge
                TextMesh text = edgeObject.GetComponent<TextMesh>();

                // Set the text of the uri node representing the predicate
                IUriNode uriNode = (IUriNode)edge.Predicate;
                text.text = SplitString(uriNode.Uri.ToString());

                // Calculate midpoint between the two nodes of the edge
                Vector3 midpoint = Vector3.Lerp(nodes[edge.Subject].transform.position, nodes[edge.Object].transform.position, 0.5f);

                // Assign the position of the text to the midpoint
                edgeObject.transform.position = midpoint;

                // Get the script of the edge object
                Edge edgeScript = edgeObject.GetComponent<Edge>();

                // Store the subject and object node of the edge
                edgeScript.subjectNode = nodes[edge.Subject];
                edgeScript.objectNode = nodes[edge.Object];

                // Store the uri of the text
                edgeScript.uri = uriNode.Uri.ToString();

                // Store edge in edge list of adjacent nodes
                Node subjectNode = nodes[edge.Subject].GetComponent<Node>();
                Node objectNode = nodes[edge.Object].GetComponent<Node>();

                subjectNode.edges.Add(edgeObject);
                objectNode.edges.Add(edgeObject);

                // Make the edge less wide
                lineRenderer.startWidth = 0.04f;
                lineRenderer.endWidth = 0.04f;

                // Sharpen the text by resizing it
                text.fontSize = 150;
                text.characterSize = .01f;

                edgeObject.transform.SetParent(graphGO.transform);

                edgeObject.layer = LayerMask.NameToLayer("GraphElements");
            }
        }
    }

    public void InitializeSphereRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject sphereRepresentation = Instantiate(sphereRepresentationPrefab);

            sphereRepresentation.transform.SetParent(node.transform);

            sphereRepresentation.transform.position = node.transform.position;

            sphereRepresentation.layer = LayerMask.NameToLayer("GraphElements");
        }
    }

    public void InitializeImageRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject imageRepresentation = Instantiate(imageRepresentationPrefab);

            imageRepresentation.transform.SetParent(node.transform);

            imageRepresentation.transform.position = node.transform.position;

            imageRepresentation.layer = LayerMask.NameToLayer("GraphElements");

            // Sphere representation is the first representation to show after initialization
            imageRepresentation.SetActive(false);
        }
    }

    public void InitializeModelRepresentation() {
        // TODO
    }

    public void ActivateSphereRepresentation() {
        foreach (GameObject node in nodes.Values) {
            // Activate sphere representation
            node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(true);

            // Deactivate image representation
            node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(false);
        }
    }

    // TODO: Only activate image, deactivate sphere representation for nodes with image URL
    public void ActivateImageRepresentation() {
        foreach (GameObject node in nodes.Values) {
            // Deactivate sphere representation
            node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(false);

            // Activate image representation
            node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(true);

            //MeshRenderer renderer = node.transform.Find("SphereRepresentation(Clone)").gameObject.GetComponent<MeshRenderer>();

            //renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0);
        }
    }

}
