using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace{
	public Vector2 gridPos, connectingNeighbor;
	public bool northWall, southWall, eastWall, westWall, isGoal;

	public GridSpace(Vector2 pos){
		gridPos = pos;
		northWall = southWall = eastWall = westWall = true;
        isGoal = false;
	}

    public void setAsGoal() {
        isGoal = true;
    }
    public void setNeighbor(Vector2 neighbor) {
        connectingNeighbor = neighbor;
    }
	public void DestroyWall(string w){
		if(w == "north"){
			northWall = false;
		}
		if(w == "south"){
			southWall = false;
		}
		if(w == "east"){
			eastWall = false;
		}
		if(w == "west"){
			westWall = false;
		}
	}

}
