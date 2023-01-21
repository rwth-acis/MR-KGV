using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF;

public class Layout : MonoBehaviour {
    // Repulsion constant
    public float k = 0.5f;

    // Cooling factor for the incremental position changes
    public float coolingFactor = 0.99f;

    // Threshold for the combined net force of all nodes
    public float threshold = 10f;

    // Maximum number of iterations for the algorithm
    public int maxIterations = 100;

    // Minimum distance between two nodes
    public float minDistance = 0.05f;

    // Simply graph layout algorithm using repulsive and attractive forces
    public void ForceDirectedLayout(Dictionary<INode, GameObject> nodes, Dictionary<Triple, GameObject> edges, GameObject centerPoint, float radius) {
        int currentIteration = 0;
        while (currentIteration < maxIterations) {
            float netForce = 0;
            foreach (KeyValuePair<INode, GameObject> pair in nodes) {
                GameObject node1 = pair.Value;
                Vector3 force = Vector3.zero;

                // Calculate the repulsion force between each pair of nodes
                foreach (KeyValuePair<INode, GameObject> otherPair in nodes) {
                    if (pair.Key != otherPair.Key) {
                        GameObject node2 = otherPair.Value;
                        force += RepulsiveForce(node1, node2);
                    }
                }

                // Calculate the attraction force between each pair of connected nodes
                foreach (KeyValuePair<Triple, GameObject> edgePair in edges) {
                    if (edgePair.Key.Subject.Equals(pair.Key) || edgePair.Key.Object.Equals(pair.Key)) {
                        GameObject otherNode = (edgePair.Key.Subject.Equals(pair.Key)) ? nodes[edgePair.Key.Object] : nodes[edgePair.Key.Subject];
                        force += AttractiveForce(node1, otherNode);
                    }
                }

                // Calculate the attraction force between each node and the center point
                force += AttractiveForceCenter(node1, centerPoint);

                // Update the position of the node based on the net force
                node1.transform.position += force * Time.deltaTime;
                netForce += force.magnitude;

                // Check if the position of the node exceeds the maximum radius
                if (Vector3.Distance(node1.transform.position, centerPoint.transform.position) > radius) {
                    node1.transform.position = centerPoint.transform.position + (node1.transform.position - centerPoint.transform.position).normalized * radius;
                }
            }

            // Update the cooling factor
            coolingFactor *= coolingFactor;

            // Check if the threshold for the combined net force is reached
            if (netForce < threshold) {
                break;
            }
            currentIteration++;
        }
    }

    private Vector3 RepulsiveForceMinDistance(GameObject node1, GameObject node2) {
        float distance = Vector3.Distance(node1.transform.position, node2.transform.position);

        // Only apply the repulsion force if the distance is below the minimum distance
        if (distance < minDistance) {
            Vector3 direction = (node1.transform.position - node2.transform.position).normalized;
            return direction * (k * k) / distance;
        } else {
            return Vector3.zero;
        }
    }

    private Vector3 RepulsiveForce(GameObject node1, GameObject node2) {
        Vector3 direction = (node1.transform.position - node2.transform.position).normalized;
        float distance = Vector3.Distance(node1.transform.position, node2.transform.position);
        return direction * (k * k) / distance;
    }

    private Vector3 AttractiveForce(GameObject node1, GameObject node2) {
        Vector3 direction = (node2.transform.position - node1.transform.position).normalized;
        float distance = Vector3.Distance(node1.transform.position, node2.transform.position);
        return direction * distance;
    }

    private Vector3 AttractiveForceCenter(GameObject node, GameObject centerPoint) {
        Vector3 direction = (centerPoint.transform.position - node.transform.position).normalized;
        float distance = Vector3.Distance(node.transform.position, centerPoint.transform.position);
        float forceMagnitude = distance * distance;
        return direction * forceMagnitude;
    }
}
