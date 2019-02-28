using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
	// Variables for generating the maze
	public GameObject wallPrefab, floorPrefab, marblePrefab;
	public Transform mazeSpawn;
    public Material floorMaterial, wallMaterial;

	// holds the maze size
	public int gridSizeX;
	public int gridSizeY;
    
    // holds the location of the goal
    Vector2 goal;

    // an array to store info about each cell in the maze
    GridSpace[,] spaces;

    // List of cells that are already in the maze
    List<Vector2> inMaze = new List<Vector2> ();

    //List of cells currently on the maze's frontier
    List<Vector2> frontierCells = new List<Vector2>();

    //List of the corners of the maze to later spawn a marble in
    List<Vector2> corners = new List<Vector2>();


	public void GenerateMaze(int sizeX, int sizeY){
        //Strategy
        //Choose a cell at random and place it in the maze
        //mark all cells around it as 'frontier cells'
        //chose a frontier cell at random
        //mark the wall connecting it to the pathway as false
        //add any cells around it as frontier cells
        //repeat until no more frontier cells

        gridSizeX = sizeX;
        gridSizeY = sizeY;
        spaces = new GridSpace[gridSizeX, gridSizeY]; // set the spaces array to the proper size
        FillSpaces(); // Fills out the spaces array

        //Ensure the maze has a valid size
        if (gridSizeX <= 0) {
            gridSizeX = 6;
        }
        if (gridSizeY <= 0) {
            gridSizeY = 6;
        }

        //Choose a cell at random and add it as a frontier cell
        Vector2 first = new Vector2(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
        frontierCells.Add(first);

        //Loop through until there are no more frontier cells
        while (frontierCells.Count > 0) {
            //chose a frontier cell at random
            Vector2 randomCell = frontierCells[Random.Range(0, frontierCells.Count)];

            //Mark it as being in the maze
            inMaze.Add(randomCell);
            //Remove it from the Frontier
            frontierCells.Remove(randomCell);
            //Add it's neighbors
            AddFrontier(randomCell);
            //Connect it to the maze
            ConnectToMaze(randomCell);
        }

        //Populate corners list
        corners.Add(Vector2.zero);
        corners.Add(new Vector2(gridSizeX - 1, gridSizeY - 1));
        corners.Add(new Vector2(0, gridSizeY - 1));
        corners.Add(new Vector2(gridSizeX - 1, 0));

        //Choose a random position for the goal
        goal = new Vector2(Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
        //Set it as the goal
        spaces[(int) goal.x, (int) goal.y].setAsGoal();
    }

    void ConnectToMaze(Vector2 cell) {
        //Make a list of potential neighbors
        List<Vector2> neighbors = new List<Vector2>
        {
            cell + Vector2.up,
            cell + Vector2.down,
            cell + Vector2.right,
            cell + Vector2.left
        };
        //Check for out of bounds neighbors
        for(int i = 0; i < neighbors.Count; i++) {
            Vector2 v = neighbors[i];
            if(v.x >= gridSizeX | v.x < 0 | v.y < 0 | v.y >= gridSizeY) {
                neighbors.RemoveAt(i);
            }
        }
        int index;
        Vector2 neighbor;                                              
        //Loop through the neighbors looking for one already in the maze
        do {
            index = Random.Range(0, neighbors.Count); // Get a random neighbor
            neighbor = neighbors[index];
            neighbors.Remove(neighbor); // Remove it from list
            if (inMaze.Contains(neighbor)) {
                break; // if the neighbor is in the maze break out of for loop
            }
        } while (neighbors.Count > 0);
        //Should have neighbor now, record who it is in the new cell
        spaces[(int)cell.x, (int)cell.y].setNeighbor(neighbor);
        //decode the direction to that neighbor
        //remove the walls inbetween the cell and neighbor
        Debug.Log("Neighbor: " + neighbor);
        if (neighbor.y == cell.y + 1) {
            //neighbor is up/north
            spaces[(int)cell.x, (int)cell.y].DestroyWall("north");
            spaces[(int)neighbor.x, (int)neighbor.y].DestroyWall("south");
        } else if (neighbor.y == cell.y - 1) {
            //neighbor is down/south
            spaces[(int)cell.x, (int)cell.y].DestroyWall("south");
            spaces[(int)neighbor.x, (int)neighbor.y].DestroyWall("north");
        } else if (neighbor.x == cell.x + 1) {
            //neighbor is right/east
            spaces[(int)cell.x, (int)cell.y].DestroyWall("east");
            spaces[(int)neighbor.x, (int)neighbor.y].DestroyWall("west");
        } else {
            //neighbor is left/west
            spaces[(int)cell.x, (int)cell.y].DestroyWall("west");
            spaces[(int)neighbor.x, (int)neighbor.y].DestroyWall("east");
        }
    }

	void AddFrontier(Vector2 cell){
        //check to see if its neighbors are in the maze or already in the frontier, if neither add them to the frontier
        // also check boundry conditions
        List<Vector2> neighbors = new List<Vector2>
        {
            cell + Vector2.up,
            cell + Vector2.down,
            cell + Vector2.right,
            cell + Vector2.left
        };

        foreach (Vector2 v in neighbors){
            // if not in maze AND not in frontierCells AND not out of bounds, add to frontier
            if (!(inMaze.Contains(v)) && !( frontierCells.Contains(v)) && v.x <  gridSizeX && v.x >= 0 && v.y >= 0 && v.y < gridSizeY){
				frontierCells.Add(v);
			} 
		}
	}


	
	void FillSpaces(){
		// Loop the array, adding a space variable
		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector2 temp = new Vector2 (x, y);
					spaces [x, y] = new GridSpace (temp);
			}
		}
	}

    public GameObject SpawnMarble() {
        List<Vector3> spawns = new List<Vector3>();
        Vector2 spawn = Vector2.zero; //Holds the spawn location, initialize to zero as fallback

        float maxDistance = 0;
        //Loop through the corners to find the furthest from the goal
        foreach (Vector2 v in corners) {
            float distance = Mathf.Sqrt(Mathf.Pow((v.x-goal.x),2)+ Mathf.Pow((v.y-goal.y),2)); // Pythagoras to find the distance to the goal
            if(distance > maxDistance) {
                maxDistance = distance;
            }
            spawns.Add(new Vector3(v.x, v.y, distance));
        }

        //Loop through the spawns and find the one with the previously saved max distance
        foreach (Vector3 v in spawns) {
            if(v.z == maxDistance) {
                spawn = new Vector2(v.x, v.y);
            }
        }


        //Spawn the marble
        GameObject marble = Object.Instantiate(marblePrefab, Vector3.zero, Quaternion.identity);
        marble.transform.SetParent(mazeSpawn);
        marble.transform.localPosition = new Vector3(spawn.x, 3f, spawn.y);
        
        //Return the Game Object to the game Controller
        return marble;
    }

	public void InstantiateMaze(){
         foreach (GridSpace s in spaces) {

            //Move the maze spawn so the rotation point will be about it's center later
            mazeSpawn.transform.localPosition = new Vector3(-(gridSizeX / 2), 0, -(gridSizeY / 2));
            
            //Instantiate floor per cell
            if (!s.isGoal) {
                GameObject floor = Object.Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);
                floor.transform.SetParent(mazeSpawn);
                floor.transform.localPosition = new Vector3(s.gridPos.x, 1.5f, s.gridPos.y);
                floor.GetComponent<MeshRenderer>().material = floorMaterial;
            }
            //Instantiate walls
            if (s.northWall) {
                GameObject northWall = Object.Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
                northWall.transform.SetParent(mazeSpawn);
                northWall.transform.localPosition = new Vector3(s.gridPos.x, 1.9f, s.gridPos.y + 0.45f);
                northWall.GetComponent<MeshRenderer>().material = wallMaterial;
                northWall.transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            if (s.southWall) {
                GameObject southWall = Object.Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
                southWall.transform.SetParent(mazeSpawn);
                southWall.transform.localPosition = new Vector3(s.gridPos.x, 1.9f, s.gridPos.y - 0.45f);
                southWall.GetComponent<MeshRenderer>().material = wallMaterial;
                southWall.transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            if (s.eastWall) {
                GameObject eastWall = Object.Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
                eastWall.transform.SetParent(mazeSpawn);
                eastWall.transform.localPosition = new Vector3(s.gridPos.x + 0.45f, 1.9f, s.gridPos.y);
                eastWall.GetComponent<MeshRenderer>().material = wallMaterial;
                eastWall.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (s.westWall) {
                GameObject westWall = Object.Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
                westWall.transform.SetParent(mazeSpawn);
                westWall.transform.localPosition = new Vector3(s.gridPos.x - 0.45f, 1.9f, s.gridPos.y);
                westWall.GetComponent<MeshRenderer>().material = wallMaterial;
                westWall.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
	}

	// end of class
	}



