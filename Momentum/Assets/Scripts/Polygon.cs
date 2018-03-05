using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ploygon: MonoBehaviour {

    public List<Vector2> axes;
    private SpriteRenderer spriteRenderer;
    private Sprite sprite;
    public Vector2[] vertices;

    public void Start()
    {
        //int vertexCount = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = spriteRenderer.sprite;
        vertices = sprite.vertices;

        ////determines the vertices in a game object 
        //if (gameObject.name.Contains("Cube"))
        //{
        //    vertices.Add(new Vector2(-0.5f, -0.5f));
        //    vertices.Add(new Vector2(0.5f, -0.5f));
        //    vertices.Add(new Vector2(0.5f, 0.5f));
        //    vertices.Add(new Vector2(-0.5f, 0.5f));
        //}
        //else if(gameObject.name.Contains("Hexagon"))
        //{
            
        //}
        //else if(gameObject.name.Contains("Triangle"))
        //{
        //    vertices.Add(new Vector2(.5f, -.5f));
        //    vertices.Add(new Vector2(.5f, .5f));
        //    vertices.Add(new Vector2(-.5f, -.5f));
        //}
        ////add vertices
        //for(int i = 0; i < vertexCount; i++)
        //{
        //    vertices.Add(sprite.vertices[i]);
        //}
        //sets the axes for polygons
        SetAxes(vertices);

    }

    private void SetAxes(Vector2[] vertices)
    {
        for(int i = 0; i < vertices.Length; i++)
        {
            //get the current vertex
            Vector2 p1 = vertices[i];
            //get the next vertex
            Vector2 p2 = vertices[i + 1 == vertices.Length ? 0 : i + 1];
            //edge vector 
            Vector2 edge = p1 - p2;
            //get perpendicular vector
            Vector2 normal = perp(edge);
            axes.Add(normal);
        }
    }

    private Vector2 perp(Vector2 edge)
    {
        return new Vector2(-edge.x, edge.y);
    }

    void OnDrawGizmos()
    {
        const int LENGTH = 100;

        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f) * Color.red;
        for (int i = 0, len = axes.Count; i < len; i++)
        {
            Gizmos.DrawLine(-LENGTH / 2f * axes[i], LENGTH / 2f * axes[i]);
        }
    }

}
