using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridder : MonoBehaviour
{
    static Gridder _singleton;
    public static Gridder Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = FindObjectOfType<Gridder>();
            }
            return _singleton;
        }
    }
    [SerializeField] GameObject canWalkPrefab;
    [SerializeField] GameObject cantWalkPrefab;
    [SerializeField] int obstaclesAmount = 20;
    //All obstacles are tracked as coordinates here.
    Obstacle[,] obstacleLocations;
    Cell[,] cells;
    int gridSize = 50;
    public int getGridCellSize()
    {
        return gridSize * gridSize;
    }
    public Cell[,] Cells { get => cells; set => cells = value; }

    // Start is called before the first frame update
    void Awake()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {

        //Lets make accessing obstacles a bit optimized by first allocating some memory to an array equal size of the grid...
        obstacleLocations = new Obstacle[gridSize, gridSize];
        //The whole for loop could be replaced by a Coroutine running a while loop, but I think that this is not a crime
        for (int i = 0; i < obstaclesAmount; i++)
        {
            int randomX = UnityEngine.Random.Range(0, gridSize);
            int randomY = UnityEngine.Random.Range(0, gridSize);

            if (obstacleLocations[randomX, randomY].ObstacleObject != null || (randomX == 0 && randomY == 0))
            {
                i--;
                
            }
            else
            {
                //We found a non-accessed location for an obstacle, lets first of all save that to the pre-existing array.
                obstacleLocations[randomX, randomY].Location = new Vector2(randomX, randomY);
                //The vector2 space of the obstacle needs to be translated to 3D space, with y replacing Z (Topdown vector2 coords)
                //The obstacle is instantiated with info about location and default rotation.
                obstacleLocations[randomX, randomY].ObstacleObject = Instantiate(cantWalkPrefab, new Vector3(randomX, 0, randomY), cantWalkPrefab.transform.rotation);
            }

        }
        //List of cells must be initialized too, to be honest they could just be also combined with the obstacle struct but this should be fine too
        //on modern computers...
        Cells = new Cell[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (obstacleLocations[x, y].ObstacleObject == null)
                {
                    GameObject go = Instantiate(canWalkPrefab, new Vector3(x, 0, y), canWalkPrefab.transform.rotation);
                    go.name = x + "," + y;
                    Cell cell = new Cell(new Vector2(x,y));
                    Cells[x, y] = cell;

                }
            }
        }
    }


}
struct Obstacle
{
    Vector2 _location;
    GameObject _obstacleObject;

    public Vector2 Location { get => _location; set => _location = value; }
    public GameObject ObstacleObject { get => _obstacleObject; set => _obstacleObject = value; }
}
public class Cell
{
    Vector2 _cellLocation;
    public Cell()
    {

    }
    public Cell(Vector2 cellLocation)
    {
        _cellLocation = cellLocation;
    }
}