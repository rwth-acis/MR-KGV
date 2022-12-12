using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;

public class Input : MonoBehaviour {

    private IGraph graph;

    public float radius;

    public GameObject nodePrefab;

    // This dictionary stores the corresponding game objects of the nodes in the graph 
    private Dictionary<INode, GameObject> nodePositions = new Dictionary<INode, GameObject>();

    // Start is called before the first frame update
    void Start() {
        ReadTurtleFile("Assets/Files/example-modell.ttl");

        analyzeGraphNodes(graph);
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
