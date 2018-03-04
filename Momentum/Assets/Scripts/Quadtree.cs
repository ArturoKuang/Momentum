using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree {

    private int MAX_OBJECTS = 5;
    private int MAX_LEVELS = 5;

    private int level;
    private List<GameObject> objects;
    private Rect bounds;
    private Quadtree[] nodes;
   
    //constructor for quadtree
    public Quadtree(int pLevel, Rect pBounds)
    {
        level = pLevel;
        objects = new List<GameObject>();
        bounds = pBounds;
        nodes = new Quadtree[4];
    }

    //clears quadtree
    public void clear()
    {
        objects.Clear();
        for(int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i] != null)
            {
                nodes[i].clear();
                nodes[i] = null;
            }
        }
    }
    //splits the node into four subnodes 
    private void split()
    {
        float subWidth = bounds.width / 2;
        float subHeight = bounds.height / 2;
        float x = bounds.x;
        float y = bounds.y;

        //sub divide area
        nodes[0] = new Quadtree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
        nodes[1] = new Quadtree(level + 1, new Rect(x, y, subWidth, subHeight));
        nodes[2] = new Quadtree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
        nodes[3] = new Quadtree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    }

    //determines which node the object belongs to
    //-1: object does not fit in child node
    //and becomes part of parent node
    private int getIndex(GameObject pObject)
    {
        int index = -1;
        double verticalMidPoint = bounds.x + (bounds.width / 2);
        double horzMidPoint = bounds.y + (bounds.height / 2);
        float x = pObject.transform.position.x;
        float y = pObject.transform.position.y;

        //object can fit within the top quadrants
        bool topQuadrant = y < horzMidPoint;
        //object can fit within bot quadrants
        bool botQuadrant = y > horzMidPoint;
        //object can fit in left quadrants
        if(x < verticalMidPoint)
        {
            if (topQuadrant)
            {
                index = 1;
            }
            else if(botQuadrant)
            {
                index = 2;
            }
            //object can fit in right quadrants
        }
        else if (x > verticalMidPoint)
        {
            if (topQuadrant)
            {
                index = 0;
            }
            else if (botQuadrant)
            {
                index = 3;
            }
        }
        return index;
    }

    public void insert(GameObject pObject)
    {
        if(nodes[0] != null)
        {
            int index = getIndex(pObject);
            if(index != -1)
            {
                nodes[index].insert(pObject);
                return;
            }
        }
        objects.Add(pObject);

        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
        {
            if(nodes[0] == null)
            {
                split();
            }
            int i = 0;
            while (i < objects.Count)
            {
                GameObject obj = objects[i];
                int index = getIndex(objects[i]);
                if (index != -1)
                {
                    nodes[index].insert(obj);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public List<GameObject> retrieve(
        List<GameObject> returnObjects, 
        GameObject pObject)
    {
        int index = getIndex(pObject);
        if(index != -1 && nodes[0] != null)
        {
            nodes[index].retrieve(returnObjects, pObject);
        }
        returnObjects.AddRange(objects);
        return returnObjects;
    }

    public void DrawDebug()
    {
        float x = bounds.x;
        float y = bounds.y;
        
        Gizmos.DrawLine(new Vector2(x, y), new Vector2(x, y + bounds.height));
        Gizmos.DrawLine(new Vector2(x, y), new Vector2(x + bounds.width, y));
        Gizmos.DrawLine(new Vector2(x + bounds.width, y), new Vector2(x + bounds.width, y + bounds.height));
        Gizmos.DrawLine(new Vector2(x, y + bounds.height), new Vector2(x + bounds.width, y + bounds.height));
        Gizmos.DrawLine(new Vector2(x, y + bounds.height / 2), new Vector2(x + bounds.width, y + bounds.height / 2));
        Gizmos.DrawLine(new Vector2(x + bounds.width / 2, y), new Vector2(x + bounds.width / 2, y + bounds.height));

        if (nodes[0] != null)
        {
            for(int i = 0; i < nodes.Length; i++)
            {
                nodes[i].DrawDebug();
            }
        }

    }
}
