using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;
using VDS.RDF.Parsing;
using System.IO;
using System.Net;
using UnityEngine.Networking;

public class Visualization : MonoBehaviour {
    // Radius in which nodes get initialized around the center point
    public float radius;

    // Center point of the graph visualization; used in layout
    public GameObject centerPoint;

    // Prefabs
    public GameObject edgePrefab;
    public GameObject nodePrefab;
    public GameObject sphereRepresentationPrefab;
    public GameObject imageRepresentationPrefab;
    public GameObject modelRepresentationPrefab;

    // This dictionary stores the corresponding game objects of the nodes in the graph 
    private Dictionary<INode, GameObject> nodes = new Dictionary<INode, GameObject>();

    // This dictionary stores the corresponding game objects of the edges in the graph
    private Dictionary<Triple, GameObject> edges = new Dictionary<Triple, GameObject>();

    // Currently loaded graph
    private IGraph graph = new Graph();

    private GameObject graphGO;

    private Layout layout;

    private Dictionary<string, string> imageURLs = new Dictionary<string, string>();

    private Dictionary<string, string> modelURLs = new Dictionary<string, string>();

    string savingPath;

    void Start() {
        // On first loadup of app, create folder for graphs to be loaded from
        savingPath = Application.persistentDataPath + "/rdf";
        Directory.CreateDirectory(savingPath);

        graphGO = GameObject.Find("Graph");
        layout = GameObject.Find("LayoutHandler").GetComponent<Layout>();

        InitializeImageURLs();
        InitializeModelURLs();

        // App starts with 'graph1.ttl' loaded
        LoadGraph1FromFile();

        //DEBUGGING
        //Initizalization();
    }

    /// <summary>
    /// Initialize the visualization for the graph, node representations, and pre-defined URLs.
    /// The first representation will be the sphere representation. Also call the layout algorithm from here.
    /// </summary>
    public void Initizalization() {
        graphGO.SetActive(true);
        InitializeGraph();
        InitializeSphereRepresentation();
        InitializeImageRepresentation();
        InitializeModelRepresentation();

        FetchImageURLsFromDic();
        FetchModelURLsFromDic();

        ActivateSphereRepresentation();

        LayoutRedirect();
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

                // If node node, do not create
                if (uriNode.Uri.ToString() == "http://example.com/Node") {
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
                text.characterSize = .003f;

                edgeObject.transform.SetParent(graphGO.transform);

                edgeObject.layer = LayerMask.NameToLayer("GraphElements");
            }
        }
    }

    /// <summary>
    /// Initialize the sphere representation for nodes of the currently loaded graph.
    /// </summary>
    public void InitializeSphereRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject sphereRepresentation = Instantiate(sphereRepresentationPrefab);

            sphereRepresentation.transform.SetParent(node.transform);

            sphereRepresentation.transform.position = node.transform.position;

            sphereRepresentation.layer = LayerMask.NameToLayer("GraphElements");
        }
    }

    /// <summary>
    /// Initialize the image representation for nodes of the currently loaded graph.
    /// </summary>
    public void InitializeImageRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject imageRepresentation = Instantiate(imageRepresentationPrefab);

            imageRepresentation.transform.SetParent(node.transform);

            imageRepresentation.transform.position = node.transform.position;

            imageRepresentation.layer = LayerMask.NameToLayer("GraphElements");
        }
    }

    /// <summary>
    /// Initialize the model representation for nodes of the currently loaded graph.
    /// </summary>
    public void InitializeModelRepresentation() {

        foreach (GameObject node in nodes.Values) {
            GameObject modelRepresentation = Instantiate(modelRepresentationPrefab);

            modelRepresentation.transform.SetParent(node.transform);

            modelRepresentation.transform.position = node.transform.position;

            modelRepresentation.layer = LayerMask.NameToLayer("GraphElements");
        }
    }

    /// <summary>
    /// Activate the sphere representation for the currently loaded graph.
    /// </summary>
    public void ActivateSphereRepresentation() {
        foreach (GameObject node in nodes.Values) {
            // Activate sphere representation
            node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(true);

            // Deactivate image representation
            node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(false);

            // Deactivate model representation
            node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activate the image representation for the currently loaded graph.
    /// </summary>
    public void ActivateImageRepresentation() {
        foreach (GameObject node in nodes.Values) {
            // Deactivate sphere representation
            node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(false);

            // Activate image representation
            node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(true);

            // Deactivate model representation
            node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activate the 3D model representation for the currently loaded graph.
    /// </summary>
    public void ActivateModelRepresentation() {
        foreach (GameObject node in nodes.Values) {
            // Deactivate sphere representation
            node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(false);

            // Deactivate image representation
            node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(false);

            // Activate model representation
            node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Activate the sphere representation for the given node.
    /// </summary>
    /// <param name="node"></param>
    public void ActivateSphereRepresentationSingle(GameObject node) {
        // Activate sphere representation
        node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(true);

        // Deactivate image representation
        node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(false);

        // Deactivate model representation
        node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate the image representation for the given node.
    /// </summary>
    /// <param name="node"></param>
    public void ActivateImageRepresentationSingle(GameObject node) {
        // Deactivate sphere representation
        node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(false);

        // Activate image representation
        node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(true);

        // Deactivate model representation
        node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate the 3D model representation for the given node.
    /// </summary>
    /// <param name="node"></param>
    public void ActivateModelRepresentationSingle(GameObject node) {
        // Deactivate sphere representation
        node.transform.Find("SphereRepresentation(Clone)").gameObject.SetActive(false);

        // Deactivate image representation
        node.transform.Find("ImageRepresentation(Clone)").gameObject.SetActive(false);

        // Activate model representation
        node.transform.Find("ModelRepresentation(Clone)").gameObject.SetActive(true);
    }

    /// <summary>
    /// Checks which representation is active for a given node and returns an integer value that corresponds to the dropdown logic in the annotation screen.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public int CheckRepresentation(GameObject node) {
        if (node.transform.Find("SphereRepresentation(Clone)").gameObject.activeSelf) {
            return 0;
        } else if (node.transform.Find("ImageRepresentation(Clone)").gameObject.activeSelf) {
            return 1;
        } else if (node.transform.Find("ModelRepresentation(Clone)").gameObject.activeSelf) {
            return 2;
        } else {
            return -1;
        }
    }

    /// <summary>
    /// Fetches image URLs for all nodes in the graph from the Google Custom Search API.
    /// </summary>
    public void FetchImagesFromAPI() {
        foreach (GameObject node in nodes.Values) {
            //Debug.Log(node.GetComponent<Node>().label + "FetchImagesFromAPI");
            StartCoroutine(FetchImageURLForNode(node));
        }
    }

    private IEnumerator FetchImageURLForNode(GameObject node) {
        //Debug.Log(node.GetComponent<Node>().label + "FetchImageURLForNode");
        
        string apiKey = "APIKey";
        string searchEngineId = "SearchEngineID";
        string query = UnityWebRequest.EscapeURL(node.GetComponent<Node>().label);
        string url = $"https://www.googleapis.com/customsearch/v1?q={query}&num=1&searchType=image&cx={searchEngineId}&key={apiKey}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success) {
                string jsonResponse = webRequest.downloadHandler.text;
                string fetchedImageURL = ExtractImageURLFromJson(jsonResponse);
                node.GetComponent<Node>().imageURL = fetchedImageURL;
            } else {
                Debug.LogError($"Error fetching image URL: {webRequest.error}");
            }
        }
    }

    private string ExtractImageURLFromJson(string json) {
        const string imageUrlPrefix = "\"link\": \"";
        int startIndex = json.IndexOf(imageUrlPrefix) + imageUrlPrefix.Length;
        int endIndex = json.IndexOf('"', startIndex);
        string imageURL = json.Substring(startIndex, endIndex - startIndex);
        return imageURL;
    }

    /// <summary>
    /// Given a path to a Turtle file, parse the graph data into the IGraph 'graph'.
    /// </summary>
    /// <param name="filePath"></param>
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

    /// <summary>
    /// Destroy all GameObjects of the graph 'graph' and clear all the variables that hold information about 'graph'.
    /// </summary>
    public void ClearGraph() {
        // Destroy nodes and edges in scene
        Transform graphGOTransform = graphGO.transform;

        for (int i = graphGOTransform.childCount - 1; i >= 0; i--) {
            Transform childTransform = graphGOTransform.GetChild(i);
            Destroy(childTransform.gameObject);
        }

        // Clear dictionaries and graph
        nodes.Clear();
        edges.Clear();
        graph.Clear();
    }

    /// <summary>
    /// Function gets called when button 'Load Graph 1' is clicked. Will load file 'graph1.ttl' from '/rdf' folder in persistent data path
    /// and activate placement again.
    /// </summary>
    public void LoadGraph1FromFile() {
        ClearGraph();
        graphGO.SetActive(false);

        string loadPath = savingPath + "/graph1.ttl";
        ReadTurtleFile(loadPath);

        GameObject.Find("PlacementHandler").GetComponent<Placement>().ReactivatePlacement();
    }

    /// <summary>
    /// Function gets called when button 'Load Graph 2' is clicked. Will load file 'graph2.ttl' from '/rdf' folder in persistent data path
    /// and activate placement again.
    /// </summary>
    public void LoadGraph2FromFile() {
        ClearGraph();
        graphGO.SetActive(false);

        string loadPath = savingPath + "/graph2.ttl";
        ReadTurtleFile(loadPath);

        GameObject.Find("PlacementHandler").GetComponent<Placement>().ReactivatePlacement();
    }

    /// <summary>
    /// Write the pre-defined image URLs into the variable 'imageURL' of the appropriate nodes in the scene.
    /// </summary>
    public void FetchImageURLsFromDic() {
        foreach (GameObject node in nodes.Values) {
            Node nodeComponent = node.GetComponent<Node>();

            imageURLs.TryGetValue(nodeComponent.label, out nodeComponent.imageURL);
        }
    }

    /// <summary>
    /// Write the pre-defined model URLs into the variable 'modelURL' of the appropriate nodes in the scene.
    /// </summary>
    public void FetchModelURLsFromDic() {
        foreach (GameObject node in nodes.Values) {
            Node nodeComponent = node.GetComponent<Node>();

            modelURLs.TryGetValue(nodeComponent.label, out nodeComponent.modelURL);
        }
    }

    /// <summary>
    /// Populate dictonary 'imageURLs' with image URLs.
    /// </summary>
    public void InitializeImageURLs() {
        imageURLs.Add("Problems", "https://i.guim.co.uk/img/media/9c4164a3454b77912be9ad36a90f79885075eb34/9_46_1423_854/master/1423.jpg?width=1200&height=900&quality=85&auto=format&fit=crop&s=02d0e75f8c738d515cfa5ccf7f2ceebc");
        imageURLs.Add("Cell", "https://www.genomicseducation.hee.nhs.uk/wp-content/uploads/2016/10/Cells-182516812_900px-858x286.jpg");
        imageURLs.Add("Types", "https://mir-s3-cdn-cf.behance.net/project_modules/max_1200/3201bd70166659.5b9a640c875c3.png");
        imageURLs.Add("Many", "https://i.ytimg.com/vi/AXnTwFjrnMY/maxresdefault.jpg");
        imageURLs.Add("Body", "https://homehealth-uk.com/wp/wp-content/uploads/the-human-body-diagram.jpg");
        imageURLs.Add("Signals", "https://maslosoft.com/signals/signals-1024.png");
        imageURLs.Add("Other", "https://upload.wikimedia.org/wikipedia/commons/thumb/5/5d/Symbol_Resin_Code_7_OTHER.svg/1000px-Symbol_Resin_Code_7_OTHER.svg.png");
        imageURLs.Add("Electrical", "https://www.thoughtco.com/thmb/oVbenYpwnRpN7IBWOFAjkrbEE6Q=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/electricity-cable-with-sparks-artwork-525442015-5804fee23df78cbc28a71d9f.jpg");
        imageURLs.Add("Neuron", "https://upload.wikimedia.org/wikipedia/commons/1/10/Blausen_0657_MultipolarNeuron.png");
        imageURLs.Add("Have", "https://media.pronunciationstudio.com/2013/08/Blog-have.png");
        imageURLs.Add("Microscopy", "https://www.zeiss.de/content/dam/metrology/products/systems/microscopy/zeiss-industrial-microscopy-in-action.png");
        imageURLs.Add("Progress", "https://thumbs.dreamstime.com/b/progress-report-icon-vector-isolated-white-background-progre-transparent-sign-black-symbols-133752892.jpg");
        imageURLs.Add("Brain", "https://www.brainline.org/sites/all/modules/custom/bl_brain/images/brain-lateral.png");
        imageURLs.Add("Functions", "https://www.mometrix.com/academy/wp-content/uploads/2021/01/graphs-3-1024x508.png");
        imageURLs.Add("Cord", "https://www.polsterer-shop.de/wp-content/uploads/2020/06/cordstoff-1024x585.jpg");
        imageURLs.Add("Spinal", "https://nci-media.cancer.gov/pdq/media/images/797134-571.jpg");
        imageURLs.Add("System", "https://img.freepik.com/vektoren-kostenlos/solarsystem-fuer-den-naturwissenschaftlichen-unterricht_1308-89626.jpg?w=2000");
        imageURLs.Add("Study", "https://www.brainscape.com/academy/content/images/2022/02/Ultimate-study-guide-header.png");
        imageURLs.Add("Scientific", "https://s7g10.scene7.com/is/image/biomerieux/MicrosoftTeams-image-2?qlt=85&wid=1200&ts=1646065963617&dpr=off");

        imageURLs.Add("Zelle", "https://www.genomicseducation.hee.nhs.uk/wp-content/uploads/2016/10/Cells-182516812_900px-858x286.jpg");
        imageURLs.Add("Typen", "https://mir-s3-cdn-cf.behance.net/project_modules/max_1200/3201bd70166659.5b9a640c875c3.png");
        imageURLs.Add("Viele", "https://i.ytimg.com/vi/AXnTwFjrnMY/maxresdefault.jpg");
        imageURLs.Add("K�rper", "https://homehealth-uk.com/wp/wp-content/uploads/the-human-body-diagram.jpg");
        imageURLs.Add("Signale", "https://maslosoft.com/signals/signals-1024.png");
        imageURLs.Add("Elektrische", "https://www.thoughtco.com/thmb/oVbenYpwnRpN7IBWOFAjkrbEE6Q=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/electricity-cable-with-sparks-artwork-525442015-5804fee23df78cbc28a71d9f.jpg");
        imageURLs.Add("Haben", "https://media.pronunciationstudio.com/2013/08/Blog-have.png");
        imageURLs.Add("Gehirn", "https://www.brainline.org/sites/all/modules/custom/bl_brain/images/brain-lateral.png");
        imageURLs.Add("Funktionen", "https://www.mometrix.com/academy/wp-content/uploads/2021/01/graphs-3-1024x508.png");
        imageURLs.Add("Studie", "https://www.brainscape.com/academy/content/images/2022/02/Ultimate-study-guide-header.png");
        imageURLs.Add("Wissenschaftlich", "https://s7g10.scene7.com/is/image/biomerieux/MicrosoftTeams-image-2?qlt=85&wid=1200&ts=1646065963617&dpr=off");

        imageURLs.Add("Rome", "https://upload.wikimedia.org/wikipedia/commons/c/c0/Rome_Montage_2017.png");
        imageURLs.Add("Pope", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/ab/Pope_Francis_Korea_Haemi_Castle_19.jpg/1200px-Pope_Francis_Korea_Haemi_Castle_19.jpg");
        imageURLs.Add("Christianity", "https://upload.wikimedia.org/wikipedia/commons/thumb/8/87/Christian_cross.svg/1200px-Christian_cross.svg.png");
    }

    /// <summary>
    /// Populate dictonary 'modelURLs' with model URLs.
    /// </summary>
    public void InitializeModelURLs() {
        modelURLs.Add("Cell", "https://raw.githubusercontent.com/rwth-acis/i5-Toolkit-for-Unity/master/Assets/i5%20Toolkit%20for%20Unity/Samples%7E/Importers/ObjImporter/Obj%20Models/Monkey_textured.obj");
    }


    /// <summary>
    /// Call the layout algorithm with the graph data that is currently stored in the variables 'nodes', 'edges', 'centerPoint', and 'radius'.
    /// </summary>
    public void LayoutRedirect() {
        layout.ForceDirectedLayout(nodes, edges, centerPoint, radius);
    }

    /// <summary>
    /// Helper function. Return the substring that comes after the last character '/'.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string SplitString(string s) {
        int lastIndex = s.LastIndexOf('/');

        return s.Substring(lastIndex + 1);
    }

    /// <summary>
    /// Helper function. Output the number of different types of nodes contained within a given graph to the console.
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
