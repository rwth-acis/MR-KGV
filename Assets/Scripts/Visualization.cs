using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;

public class Visualization : MonoBehaviour {

    // Currently loaded graph
    private IGraph graph;

    // Radius in which nodes get initialized around the center point
    public float radius;

    // Center point of the graph visualization; used in layout
    public GameObject center;

    public GameObject edgePrefab;

    public GameObject nodePrefab;

    public GameObject sphereRepresentationPrefab;

    public GameObject imageRepresentationPrefab;

    // This dictionary stores the corresponding game objects of the nodes in the graph 
    private Dictionary<INode, GameObject> nodes = new Dictionary<INode, GameObject>();

    // This dictionary stores the corresponding game objects of the edges in the graph
    private Dictionary<Triple, GameObject> edges = new Dictionary<Triple, GameObject>();

    // Start is called before the first frame update
    void Start() {
        ReadTurtleFile("Assets/Files/example-modell.ttl");

        InitializeGraph();

        InitializeSphereRepresentation();

        InitializeImageRepresentation();

        //UpdateNodePositions();

        //analyzeGraphNodes(graph);
    }

    /// <summary>
    /// Initialize the visualization of the given graph 'graph'
    /// </summary>
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
                float x = Random.Range(-radius + center.transform.position.x, radius + center.transform.position.x);
                float y = Random.Range(-radius + center.transform.position.y, radius + center.transform.position.y);
                float z = Random.Range(-radius + center.transform.position.z, radius + center.transform.position.z);

                // Create a node prefab at a random position within the specified radius
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(x, y, z), Quaternion.identity);

                // Store the node object in the dictionary
                nodes[node] = nodeObject;

                // Set the scale of the node object
                nodeObject.transform.localScale = new Vector3(1f, 1f, 1f);

                // Get the text mesh of the node
                TextMesh text = nodeObject.GetComponent<TextMesh>();

                // Set the text of the node label
                text.text = SplitString(uriNode.Uri.ToString());

                // Get the script of the node object
                Node nodeScript = nodeObject.GetComponent<Node>();

                // Store the uri of the node
                nodeScript.uri = uriNode.Uri.ToString();
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
            }
        }
    }

    public void InitializeSphereRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject sphereRepresentation = Instantiate(sphereRepresentationPrefab);

            sphereRepresentation.transform.SetParent(node.transform);

            sphereRepresentation.transform.position = node.transform.position; // transform main camera?
        }
    }

    public void InitializeImageRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject imageRepresentation = Instantiate(imageRepresentationPrefab);

            imageRepresentation.transform.SetParent(node.transform);

            imageRepresentation.transform.position = node.transform.position; // transform main camera?
        }
    }

    public void InitializeModelRepresentation() {
        // TODO
    }

    void UpdateNodePositions() {
        foreach (GameObject node1 in nodes.Values) {
            Vector3 netForce = Vector3.zero;

            Rigidbody rigidBody = node1.GetComponent<Rigidbody>();

            foreach (GameObject node2 in nodes.Values) {
                if (node1 != node2) {
                    netForce += repulsiveForce(node1, node2);
                }
            }
            netForce += attractiveForce(node1);
            //netForce += dampingForce(node1);
            rigidBody.velocity += netForce * Time.deltaTime;
            node1.transform.position += rigidBody.velocity * Time.deltaTime;
        }
    }

    Vector3 repulsiveForce(GameObject node1, GameObject node2) {
        float distance = Vector3.Distance(node1.transform.position, node2.transform.position);
        Vector3 direction = (node2.transform.position - node1.transform.position).normalized;
        float forceMagnitude = (2f * 2f) / (distance * distance);
        return direction * forceMagnitude;
    }

    Vector3 attractiveForce(GameObject node) {
        Vector3 direction = (center.transform.position - node.transform.position).normalized;
        float distance = Vector3.Distance(node.transform.position, center.transform.position);
        float forceMagnitude = distance * distance;
        return direction * forceMagnitude;
    }

    Vector3 dampingForce(GameObject node) {
        Rigidbody rigidBody = node.GetComponent<Rigidbody>();

        return -rigidBody.velocity * 30;
    }

    /// <summary>
    /// Given a path to a Turtle file, parse the graph data into the global IGraph 'graph'
    /// </summary>
    /// <param name="filePath"></param>
    public void ReadTurtleFile(string filePath) {
        try {
            // Create a new instance of the Turtle parser
            TurtleParser parser = new TurtleParser();

            graph = new Graph();

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

    /// <summary>
    /// Given a string (representing a URI), return the substring that comes after the last character '/'
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string SplitString(string s) {
        int lastIndex = s.LastIndexOf('/');

        return s.Substring(lastIndex + 1);
    }

    /// <summary>
    /// Given a graph, output the number of different types of nodes contained within the graph
    /// </summary>
    /// <param name="g"></param>
    void analyzeGraphNodes(IGraph g) {
        int numberOfBlankNodes = 0;
        int numberOfLiteralNodes = 0;
        int numberOfUriNodes = 0;
        int numberOfGraphLiteralNodes = 0;
        int numberOfVariableNodes = 0;

        foreach (INode n in g.AllNodes) {
            switch (n.NodeType) {
                case NodeType.Blank:
                    numberOfBlankNodes++;
                    break;
                case NodeType.Literal:
                    numberOfLiteralNodes++;
                    break;
                case NodeType.Uri:
                    numberOfUriNodes++;
                    break;
                case NodeType.GraphLiteral:
                    numberOfGraphLiteralNodes++;
                    break;
                case NodeType.Variable:
                    numberOfVariableNodes++;
                    break;
            }
        }

        Debug.Log("Blank Nodes: " + numberOfBlankNodes + "\n");
        Debug.Log("Literal Nodes: " + numberOfLiteralNodes + "\n");
        Debug.Log("Uri Nodes: " + numberOfUriNodes + "\n");
        Debug.Log("GraphLiteral Nodes: " + numberOfGraphLiteralNodes + "\n");
        Debug.Log("Variable Nodes: " + numberOfVariableNodes + "\n");
    }
}
