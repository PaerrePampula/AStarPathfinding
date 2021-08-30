
using UnityEngine;

class NodeRecord : IMinimumHeapable
{
    const float diagonalMoveCost = 1.4f;
    const float perpendicularMoveCost = 1f;
    Vector2 _nodeLocation;
    Connection _connection;
    float _thisNodeCostSoFar;
    float _nodeCost;
    float _heuristicDistance;

    VisitState visitState = VisitState.NotKnown;
    public float ThisNodeCostSoFar { get => _thisNodeCostSoFar; set => _thisNodeCostSoFar = value; }
    public Vector2 NodeLocation { get => _nodeLocation; set => _nodeLocation = value; }
    public float HeuristicDistance { get => _heuristicDistance; set => _heuristicDistance = value; }
    public float TotalCostUsingEstimate
    {
        get
        { 
            return _heuristicDistance+ThisNodeCostSoFar; 
        }

    }

    public float NodeCost { get => _nodeCost; set => _nodeCost = value; }
    internal Connection Connection { get => _connection; set => _connection = value; }
    internal VisitState VisitState { get => visitState; set => visitState = value; }

    public float calculateHeuristicDistance(Vector2 goal)
    {
        //Distance using diagonal distance
        //Implies we can move in 8 directions
        return Mathf.Abs(NodeLocation.x - goal.x) + Mathf.Abs(NodeLocation.y - goal.y);

    }

    public float getElementValue()
    {
        return TotalCostUsingEstimate;
    }
}
class Connection
{
    float _connectionCost;
    Vector2 _fromNode;
    Vector2 _toNode;

    public Connection(float connectionCost, Vector2 fromNode, Vector2 toNode)
    {
        ConnectionCost = connectionCost;
        FromNode = fromNode;
        if (toNode != null)
        ToNode = toNode;
    }

    public float ConnectionCost { get => _connectionCost; set => _connectionCost = value; }
    public Vector2 FromNode { get => _fromNode; set => _fromNode = value; }
    public Vector2 ToNode { get => _toNode; set => _toNode = value; }


}
enum VisitState
{
    Open,
    Closed,
    NotKnown
}