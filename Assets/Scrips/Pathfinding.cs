using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    grid_A grid;
    PathfindingManager requestmanager;


    
    void Awake()
    {
        requestmanager = GetComponent<PathfindingManager>();
        grid = GetComponent<grid_A>();
    } 

    public void StartFindPath(Vector2 startpos, Vector2 targetpos)
    {
        StartCoroutine(FindPath(startpos, targetpos));
    }

   IEnumerator FindPath(Vector2 startPos, Vector2 endPos)
   {
        // timer 
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSucces = false;


        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(endPos);

        if(startNode.walkable && targetNode.walkable){

            Heap<Node> openSet = new Heap<Node>(grid.maxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                
                closedSet.Add(currentNode);

                // check if the current node is the target
                if (currentNode == targetNode)
                {
                    
                    sw.Stop();
                    // print("Path found: " + sw.ElapsedMilliseconds + "ms");
                    pathSucces = true;
                    break;
                }


                // loop over all neighbours to evaluate
                foreach (Node neighbour in grid.Getneighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    
                    int newMovementCostToNeighbour = currentNode.Gcost + Getdistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.Gcost || !openSet.Contains(neighbour))
                    {

                        neighbour.Gcost = newMovementCostToNeighbour;
                        neighbour.Hcost = Getdistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                            openSet.UpdateItem(neighbour);

                    }
                }
            }
             
            yield return null;
            if (pathSucces)
            {
                waypoints = retracePath(startNode, targetNode);
            }
            requestmanager.FinishedProcessingPath(waypoints,pathSucces);

        }
    
    Vector2[] retracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;

        }
        Vector2[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);


        return waypoints;

    }

    Vector2[] SimplifyPath(List<Node> path)
    {
            List<Vector2> waypoints = new List<Vector2>();
            Vector2 directionOld = Vector2.zero;
            for(int i = 1; i< path.Count; i++)
            {
                Vector2  directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;

            }
        return waypoints.ToArray();    
    }

    int Getdistance(Node A, Node B)
    {
        int dstX = Mathf.Abs(A.gridX - B.gridX);
        int dstY = Mathf.Abs(A.gridY - B.gridY);
        
        // return the distance by adding the straight and angled distance
        if (dstX > dstY){
            return 14*dstY + 10 * (dstX - dstY);
        }
        return 14*dstX + 10 * (dstY - dstX);
    }  


   }
}
