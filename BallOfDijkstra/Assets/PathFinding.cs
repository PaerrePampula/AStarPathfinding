using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// A general description of the A* pathfinding algorithm:
/// The problem:
/// A path is need from point A to point B. We first of all need to have a two-dimensional graph, where the agent of the pathfinding
/// travels on. The graph has weighted nodes, each of which might be considered a part of a final path to take to the goal of the pathfinding agent.
/// The path of the agent depends on heuristics and is built by using the connections of each currently being considered nodes.
/// The algorithm works as follows:
/// The algorithm is very similar to Dijikstra's Algorithm, in fact, this algorithm builds upon that. A brief description is given below:
///     Dijikstras algorithm works by first of all finding the "smallest cost" node to travel to, which, when starting, is the start node of the agent
///     Cost of reaching this is naturally zero. All nodes converging here are then considered, and the smallest one is chosen. Cost to that node
///     is simply the cost between the start point and the node chosen. After the first iteration, the algorithm considers the node with the smallest value 
///     that is connected to the current node, then checks all the connections to that specific node, after which the costs-so-far are updated to those connections as well
///     where the connection is the cost-so-far + the connection between the converging node. 
///     
///     This repeats yet again, finding all the connections of the smallest value node
///     where the connections are not mapped out. The algorithm continues this process, until the goal node is reached (or optionally if the whole graph is mapped, 
///     and the shortest possible path is found). Generally speaking games tend to shorten this process just to reach the first path, because it might not be important, and it 
///     does optimize the algorithm a lot, especially with really big graphs. Even though most games prefer to use the more clever-for-this-purpose algorithm of A*.
///     
///     The final path is retrieved using the connections as a tool. Each considered path saves the shortest path as the connecting node, and this information can be used to build the
///     path (in reverse) back to the start.
///     
///     The weakness of Dijikstra is that it searches the entire graph indiscriminately for the shortest path, which means there is going to be a lot of potential inefficiency, this is a problem that A* ties to solve.
///     
/// A* is a bit different as it uses heuristics to optimize the pathfinding a bit. Instead of just looking at what cost it takes to reach any random point nearby, a predictive calculation to the target is 
/// made using distance. For this algorithm, we will use manhattan distance as a heuristical cost. (described in the record class)
/// This algorithm could be improved by preventing the agent from traveling diagonally through obstacles.
/// </summary>
public class PathFinding : MonoBehaviour
{
    //Most of the code will use singletons, to make the demonstration straightforward when accessing information from other classes.
    static PathFinding singleton;
    public static PathFinding Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<PathFinding>();
            }
            return singleton;
        }
    }
    //This dictionary saves records of the start node, and any nodes that are recorded when a node gets its' connections checked.
    Dictionary<Vector2, NodeRecord> allRecords = new Dictionary<Vector2, NodeRecord>();
    //All the known nodes that are not yet checked in any iteration are considered to be in the "open" list of nodes.
    //Some versions of the algorithm also place the nodes in "closed node" lists, but this is worthless and inefficient,
    //When instead of expensive Contains comparisons, you could simply compare if an enum matches "closed"
    MinHeap<NodeRecord> minRecordHeap;

    //List<NodeRecord> closedNodes = new List<NodeRecord>();
    //This is saved based on the current location of the pathfinding agent.
    Vector2 startVector = new Vector2();
    //This is saved based on the input of the user.
    [SerializeField] Vector2 goal = new Vector2();
    //This is the currently considered nodes' information.
    NodeRecord currentRecord;
    List<Vector2> path = new List<Vector2>();
    // Start is called before the first frame update
    private void Start()
    {
        minRecordHeap  = new MinHeap<NodeRecord>(Gridder.Singleton.getGridCellSize());

    }

    public void PathFindTo(Vector2 place)
    {
        Stopwatch timer;
        timer = new Stopwatch();
        timer.Restart();
        allRecords.Clear();
        minRecordHeap.ClearHeap();
        path.Clear();
        //Initially, the algorithm works pretty much the same as Dijikstra. But we have to consider nodes that are more likely to lead to the shortest path.
        //We first generate the initial node.
        goal = place;

        NodeRecord startingRecord = new NodeRecord();
        startingRecord.NodeLocation = AStarAgent.Singleton.getAgentLocation();
        startVector = startingRecord.NodeLocation;
        //This is the beginning node, so there is no cost.
        startingRecord.ThisNodeCostSoFar = 0;
        //But we will also save the heuristic distance
        //Total cost (including estimate) is automatically returned by the property in the getter method.
        startingRecord.HeuristicDistance = startingRecord.calculateHeuristicDistance(goal);
        startingRecord.VisitState = VisitState.Open;
        //the starting record will be the only node on the open list in the beginning.
        minRecordHeap.InsertKey(startingRecord);

        allRecords.Add(startingRecord.NodeLocation, startingRecord);
        //start considering all the nodes on the list
        while (minRecordHeap.CurrentHeapSize > 0)
        {
            //We first need to find the smallest element according to the totalcost
            currentRecord = minRecordHeap.extractMinKey();
            //The smallest node might also be the goal, so if it is found, terminate early.
            if (isSameNodeLocation(currentRecord.NodeLocation, goal)) break;
            //If the above evaluated false, we then need to consider all the outgoing connections from the current node.
            List<Connection> connections = getAllOutGoingConnectionsForNode(currentRecord);

            for (int i = 0; i < connections.Count; i++)
            {
                //Get the cost estimate for this end node
                float endNodeCost = currentRecord.ThisNodeCostSoFar + connections[i].ConnectionCost;
                //Try to find this record

                NodeRecord endNodeRecord;
                allRecords.TryGetValue(connections[i].ToNode, out endNodeRecord);
                if (endNodeRecord != null)
                {
                    if (endNodeRecord.VisitState == VisitState.Closed)
                    {
                        //We might have found this node already, but from a different connection
                        //We will check if the node cost can be updated with a smaller value from this connection.
                        if (endNodeRecord.ThisNodeCostSoFar > endNodeCost)
                        {
                            //We found a shorter route! update the node to be open yet again 
                            //and save the cost
                            endNodeRecord.VisitState = VisitState.Open;
                            minRecordHeap.InsertKey(endNodeRecord);

                        }
                        else continue;

                    }
                    else
                    {
                        //In this case we found a record in the open state
                        if (endNodeRecord.ThisNodeCostSoFar < endNodeCost)
                        {
                            continue;
                        }

                    }
                }
                else
                {
                    //Node is unkown! (not in the open list, and its not closed either...)
                    //We need to record this first.
                    endNodeRecord = new NodeRecord();
                    endNodeRecord.NodeLocation = new Vector2(connections[i].ToNode.x, connections[i].ToNode.y);
                    endNodeRecord.HeuristicDistance = endNodeRecord.calculateHeuristicDistance(goal);
                    allRecords.Add(endNodeRecord.NodeLocation,endNodeRecord);
                }
                endNodeRecord.NodeCost = endNodeCost;
                endNodeRecord.Connection = connections[i];
                endNodeRecord.VisitState = VisitState.Open;
                endNodeRecord.ThisNodeCostSoFar = endNodeCost;
                //Total cost heuristic is handled by the getter property TotalCostUsingEstimate
                minRecordHeap.InsertKey(endNodeRecord);
            }
            currentRecord.VisitState = VisitState.Closed;

        }
        if (!isSameNodeLocation(currentRecord.NodeLocation, goal))
        {
            //PATH NOT FOUND!!!!!
        }
        else
        {
            path = new List<Vector2>();
            path.Add(goal);
            while (!isSameNodeLocation(currentRecord.NodeLocation, startVector))
            {
               path.Add(currentRecord.Connection.ToNode);
               currentRecord = allRecords[currentRecord.Connection.FromNode];
            }
            //Finally, the start is added to the path

        }
        //The path is wrong way around, reverse it
        path.Reverse();
        AStarAgent.Singleton.MoveToTargets(path);
        timer.Stop();
        UnityEngine.Debug.Log(timer.Elapsed);

    }


    List<Connection> getAllOutGoingConnectionsForNode(NodeRecord nodeRecord)
    {
        List<Connection> gottenConnections = new List<Connection>();
        int nodeX = (int)nodeRecord.NodeLocation.x;
        int nodeY = (int)nodeRecord.NodeLocation.y;
        //This version of the algorithm only considers positive grid positions for simplicity.
        //Get all the surrounding nodes.
        for (int x = Mathf.Max(nodeX-1, 0); x <= nodeX + 1; x++)
        {
            for (int y = Mathf.Max(nodeY-1,0); y <= nodeY + 1; y++)
            {
                if (y == nodeY)
                {
                    if (x == nodeX)
                    {
                        continue;
                    }
                }
                //A bit cheap way of just checking the perpendicular nodes. (no south/north-west/south blocks included)
                else if (x != nodeX) continue;
                Vector2 connectionVector = new Vector2(x, y);
                Cell connectionNode = null;

                try
                {
                    connectionNode = Gridder.Singleton.Cells[x,y];

                    //There is no try get for this collection type, so we will skip any index out of range errors manually
                }
                catch (IndexOutOfRangeException exception)
                {

                    //The collection didnt contain any of this, this iteration can be skipped.
                }
                if (connectionNode != null)
                {
                    float cost = Mathf.Abs(Vector2.Distance(nodeRecord.NodeLocation, connectionVector));
                    Connection connection = new Connection(cost, nodeRecord.NodeLocation, connectionVector);
                    gottenConnections.Add(connection);
                }

            }
        }
        return gottenConnections;
    }
    bool isSameNodeLocation(Vector2 a, Vector2 b)
    {
        if (a.x == b.x)
        {
            if (a.y == b.y)
            {
                return true;
            }
        }
        return false;
    }
}
