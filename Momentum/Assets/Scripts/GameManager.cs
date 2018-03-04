using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    Quadtree quad = null;
    GameObject[] sceneObjects;

    private Vector2 worldSize;

	void Start () {
        worldSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        worldSize = Camera.main.ScreenToWorldPoint(worldSize);
        quad = new Quadtree(0, new Rect(-worldSize.x, -worldSize.y, 2 * worldSize.x, 2 * worldSize.y));
	}
	
	// Update is called once per frame
	void Update () {
        quad.clear();
        sceneObjects = GameObject.FindGameObjectsWithTag("gameObjects");
        foreach(GameObject go in sceneObjects)
        {
            quad.insert(go);
        }
        List<GameObject> returnObjects = new List<GameObject>();
        for(int i = 0; i < sceneObjects.Length; i++)
        {
            returnObjects.Clear();
            quad.retrieve(returnObjects, sceneObjects[i]);

            for(int j = 0; j < returnObjects.Count; j++)
            {
                //run collision 
            }
        }
		
	}

    void OnDrawGizmos()
    {
        if (quad != null)
        {
            quad.DrawDebug();
        }
    }
}
