using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;

public class Visualization : MonoBehaviour {

    private IGraph graph;

    public float radius;

    public GameObject nodePrefab;

    // This dictionary stores the corresponding game objects of the nodes in the graph 
    private Dictionary<INode, GameObject> nodePositions = new Dictionary<INode, GameObject>();

    // Start is called before the first frame update
    void Start() {
        ReadTurtleFile("Assets/Files/example-modell.ttl");

        InitializeGraph();

        analyzeGraphNodes(graph);
    }

    /// <summary>
    /// Initialize the visualization of the given graph 'graph'
    /// </summary>
    public void InitializeGraph() {
        // Loop through the nodes in the graph and create a 3D object for each one
        foreach (INode node in graph.Nodes) {
            // Only create nodes if it is a uri node
            if (node.NodeType == NodeType.Uri) {
                // Generate random coordinates within the specified radius
                float x = Random.Range(-radius, radius);
                float y = Random.Range(-radius, radius);
                float z = Random.Range(-radius, radius);

                // Create a node prefab at a random position within the specified radius
                GameObject nodeObject = Instantiate(nodePrefab, new Vector3(x, y, z), Quaternion.identity);

                // Store the node object in the dictionary
                nodePositions[node] = nodeObject;

                // Set the scale of the node object
                nodeObject.transform.localScale = new Vector3(1f, 1f, 1f);

                // Create a text label for the node
                TextMesh text = new GameObject().AddComponent<TextMesh>();

                // Set the text of the label
                IUriNode uriNode = (IUriNode)node;
                text.text = SplitString(uriNode.Uri.ToString());

                // Set the font size of the label
                text.fontSize = 12;

                // Set the color of the label
                text.color = Color.black;

                // Set the position of the label
                text.transform.position = nodeObject.transform.position;

                // Set the rotation of the label to match the node object
                text.transform.rotation = nodeObject.transform.rotation;

                // Position the text in the middle of the node
                text.anchor = TextAnchor.MiddleCenter;
            }
        }

        // Loop through the edges in the graph and create a 3D line for each one
        foreach (Triple edge in graph.Triples) {

            // Only create edges between uri nodes that were previously created
            if (nodePositions.ContainsKey(edge.Subject) && nodePositions.ContainsKey(edge.Object)) {
                // Create a 3D line to represent the edge
                GameObject edgeObject = new GameObject();

                // Add a LineRenderer component to the edge object
                LineRenderer lineRenderer = edgeObject.AddComponent<LineRenderer>();

                // Set the position of the line's start and end points using the transforms of the node objects stored in the dictionary
                lineRenderer.SetPosition(0, nodePositions[edge.Subject].transform.position);
                lineRenderer.SetPosition(1, nodePositions[edge.Object].transform.position);

                // Set the width of the line
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;

                // Set color of the line
                lineRenderer.material.color = new Color(1, 1, 1, 0.5f);

                // Add a TextMesh component
                TextMesh text = edgeObject.AddComponent<TextMesh>();

                // Calculate midpoint between the two nodes of the edge
                Vector3 midpoint = Vector3.Lerp(nodePositions[edge.Subject].transform.position, nodePositions[edge.Object].transform.position, 0.5f);
                edgeObject.transform.position = midpoint;

                // Get a reference to the main camera in the scene
                Camera mainCamera = Camera.main;

                // Set the rotation of the text object to the rotation of the camera
                edgeObject.transform.rotation = mainCamera.transform.rotation;

                // Position the text in the middle of the line
                text.anchor = TextAnchor.MiddleCenter;

                // Set the font size of the label
                text.fontSize = 9;

                // Set the text
                text.text = "Test";
            }
        }
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
