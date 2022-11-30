using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Writing;

public class Test : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        IGraph g = new Graph();

        IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
        IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
        ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
        ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

        g.Assert(new Triple(dotNetRDF, says, helloWorld));
        g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

        foreach (Triple t in g.Triples) {
            Debug.Log(t.ToString());
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
