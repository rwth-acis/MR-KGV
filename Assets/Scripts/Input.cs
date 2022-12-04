using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;

public class Input : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        try {
            IGraph graph = new Graph();
            TurtleParser ttlparser = new TurtleParser();

            //Load using Filename
            ttlparser.Load(graph, "Assets/Files/example-modell.ttl");
            //ttlparser.Load(graph, "Assets/Files/vocab.ttl");

            analyzeGraphNodes(graph);

            // Get all nodes used as predicates in triples of the graph
            foreach (INode n in graph.Triples.PredicateNodes) {
                Debug.Log(n);
            }

            // "graph.Nodes" gets all nodes used as objects or subjects in triples of the graph
            //foreach (ILiteralNode ln in graph.Nodes.LiteralNodes()) {
            //    Debug.Log(ln.Value);
            //}

            //foreach (IUriNode un in graph.Nodes.UriNodes()) {
            //    Debug.Log(un.Uri);
            //}
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

    // Update is called once per frame
    void Update() {

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
