using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon : MonoBehaviour
{

    public List<Vector2> axes;
    private SpriteRenderer spriteRenderer;
    private Sprite sprite;
    public List<Vector2> vertices;
    private MTV mtv;
    Vector2 collidePoint;


    struct MTV
    {
        public Vector2 smallAxis;
        public double smallestOverlap;
    }

    public List<Vector2> Axes
    {
        get { return axes; }
    }

    public void Start()
    {
        Vector2[] meshVertices;
        mtv = new MTV();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sprite = spriteRenderer.sprite;
        meshVertices = sprite.vertices;

        //determines the vertices in a game object 
        if (gameObject.name.Contains("Square"))
        {
            vertices.Add(meshVertices[0]);
            vertices.Add(meshVertices[2]);
            vertices.Add(meshVertices[1]);
            vertices.Add(meshVertices[3]);
        }
        else if (gameObject.name.Contains("Hexagon"))
        {
            vertices.Add(meshVertices[0]);
            vertices.Add(meshVertices[5]);
            vertices.Add(meshVertices[4]);
            vertices.Add(meshVertices[3]);
            vertices.Add(meshVertices[2]);
            vertices.Add(meshVertices[1]);
        }
        else if (gameObject.name.Contains("Triangle"))
        {
            for (int i = 0; i < 3; i++)
            {
                vertices.Add(sprite.vertices[i]);
            }
        }

        //sets the axes for polygons
        SetAxes(vertices);

    }

    private void SetAxes(List<Vector2> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            //get the current vertex
            Vector2 p1 = vertices[i];
            //get the next vertex
            Vector2 p2 = vertices[i + 1 == vertices.Count ? 0 : i + 1];
            //edge vector 
            Vector2 edge = p1 - p2;
            //get perpendicular vector
            Vector2 normal = perp(edge);
            axes.Add(normal);
        }
    }

    public bool CollideWith(GameObject other)
    {
        Polygon otherPloygon = other.GetComponent<Polygon>();
        double overlap = double.MaxValue;
        Vector2 smallest = Vector2.zero; //smallest axes
        List<Vector2> axes2 = other.GetComponent<Polygon>().Axes;

        if (other == null || other.tag != "gameObjects")
        {
            return false;
        }


        //axes 1
        for (int i = 0; i < axes.Count; i++)
        {
            Vector2 currentAxis = axes[i];
            //project both shapes onto the axes
            Vector2 projA = project(vertices, currentAxis, gameObject.transform);
            Vector2 projB = project(otherPloygon.vertices, currentAxis, other.transform);

            //Debug.DrawLine(projA.x * currentAxis, projA.y * currentAxis, Color.white);
            //Debug.DrawLine(projB.x * currentAxis, projB.y * currentAxis, Color.blue);

            //overlap 
            float o = Overlap(projA, projB);
            if (o < Mathf.Epsilon)
            {
                return false;
            }
            else
            {
                if (o < overlap)
                {
                    overlap = o;
                    smallest = currentAxis;
                }
            }
        }
        //axes 2
        for (int i = 0; i < axes2.Count; i++)
        {
            Vector2 currentAxis = axes2[i];
            //project both shapes onto the axes
            Vector2 projA = project(vertices, currentAxis, gameObject.transform);
            Vector2 projB = project(otherPloygon.vertices, currentAxis, other.transform);

            //Debug.DrawLine(projA.x * currentAxis, projA.y * currentAxis, Color.red);
            //Debug.DrawLine(projB.x * currentAxis, projB.y * currentAxis, Color.green);

            //overlap 
            float o = Overlap(projA, projB);
            if (o < Mathf.Epsilon)
            {
                return false;
            }
            else
            {
                if (o < overlap)
                {
                    overlap = o;
                    smallest = currentAxis;
                }
            }
        }
        mtv.smallAxis = smallest;
        mtv.smallestOverlap = overlap;

        //Always MTV point towards object 1
        Vector2 bToa;
        bToa = transform.position - other.transform.position;
        if (Vector2.Dot(bToa, mtv.smallAxis) < 0.0f)
        {
            mtv.smallAxis *= -1;
        }
        Debug.Log(mtv.smallestOverlap);
        Debug.DrawLine(transform.position, mtv.smallAxis, Color.yellow);

        collidePoint = PointOfCollision(other, mtv.smallAxis);
        return true;
    }


    //private void ClosestEdge(Vector2 separationNorm)
    //{
    //    //max projection along separationNorm
    //    double max = double.MinValue;
    //    //index of farthest vertex
    //    int index = -1;
    //    //find farthest vertex in the polygon 
    //    //along the separation normal
    //    for(int i = 0; i < vertices.Count; i++)
    //    {
    //        double projection = Vector2.Dot(separationNorm, vertices[i]);
    //        if(projection > max)
    //        {
    //            max = projection;
    //            index = i;
    //        }
    //    }

    //    //find the edge that is most perpendicular
    //    Vector2 v = vertices[index];
    //    Vector2 v1 = vertices[index + 1 % vertices.Count];
    //    Vector2 v0 = vertices[index - 1 < 0 ? 0 : index - 1];
    //    //v1 to v 
    //    Vector2 l = v - v1;
    //    //v0 to v 
    //    Vector2 r = v - v0;
    //    //normalize
    //    l.Normalize();
    //    r.Normalize();
    //    //the edge is most perpendicular
    //    //to n will have a dot product closer to zero
    //    if (Vector2.Dot(r, separationNorm) <= Vector2.Dot(l, separationNorm))
    //    {
    //        return
    //    }

    //}

    private Vector2 PointOfCollision(GameObject other, Vector2 MTV)
    {
        //edge collision tolerance
        float tolerance = 0.01f;

        //find the point lease in the direction of MTV
        List<Vector2> closestPoints1 = new List<Vector2>();
        float currentMin;
        float dotProd;
        int numPoint = vertices.Count;
        Vector2 currentPoint;
        Vector2 currentPos = transform.position;
        //other colliding polygon
        Polygon otherPolygon = other.GetComponent<Polygon>();

        //begin by setting the current min to the first point of
        //currentPoint min to the 1st point
        currentPoint = vertices[0] + currentPos;
        currentMin = Vector2.Dot(currentPoint, MTV);
        closestPoints1.Add(currentPoint);

        //loops through remaining points
        for (int i = 1; i < numPoint; i++)
        {
            //translate to world space
            currentPoint = vertices[i] + currentPos;
            //calc distance towars object 2
            dotProd = Vector2.Dot(currentPoint, MTV);
            //if the current point is the same distance in the direction
            //of obj2 along the MTV
            if (Mathf.Abs(dotProd - currentMin) < Mathf.Epsilon + tolerance)
            {
                //add new point
                closestPoints1.Add(currentPoint);
            }
            //else if it is less then the current distance
            else if (dotProd < currentMin - Mathf.Epsilon)
            {
                //clear the list of current points and set the new one 
                currentMin = dotProd;
                closestPoints1.Clear();
                closestPoints1.Add(currentPoint);
            }
        }
        //if only one closest point: collsions
        if (closestPoints1.Count == 1)
        {
            return closestPoints1[0];
        }

        //check 2nd polygon
        List<Vector2> closestPoint2 = new List<Vector2>();
        float currentMax;
        numPoint = otherPolygon.vertices.Count;
        Vector2 otherPos = other.transform.position;
        List<Vector2> otherVertices = otherPolygon.vertices;

        //begin by setting the current max of the first 
        //point of the 2nd polygon 
        currentPoint = otherVertices[0] + otherPos;
        currentMax = Vector2.Dot(currentPoint, MTV);
        closestPoint2.Add(currentPoint);

        //loops through remaining points
        for (int i = 1; i < numPoint; i++)
        {
            //translate to world space
            currentPoint = otherVertices[i] + currentPos;
            //calc distance towars object 2
            dotProd = Vector2.Dot(currentPoint, MTV);
            //if the current point is the same distance in the direction
            //of obj2 along the MTV
            if (Mathf.Abs(dotProd - currentMax) < Mathf.Epsilon + tolerance)
            {
                //add new point
                closestPoint2.Add(currentPoint);
            }
            //else if it is less then the current distance
            else if (dotProd > currentMax + Mathf.Epsilon)
            {
                //clear the list of current points and set the new one 
                currentMax = dotProd;
                closestPoint2.Clear();
                closestPoint2.Add(currentPoint);
            }
        }
        //if only one closest point: collsions
        if (closestPoint2.Count == 1)
        {
            return closestPoint2[0];
        }
        //find the two inner points
        //if needed
        Vector2 edge = new Vector2(-MTV.y, -MTV.x);
        closestPoints1.AddRange(closestPoint2);
        //determines the min and max 
        currentMin = currentMax = Vector2.Dot(closestPoints1[0], edge);
        int minIndex, maxIndex;
        minIndex = maxIndex = 0;
        numPoint = closestPoints1.Count;
        for(int i = 0; i < numPoint; i++)
        {
            dotProd = Vector2.Dot(closestPoints1[i], edge);
            if(dotProd < currentMin)
            {
                currentMin = dotProd;
                minIndex = i;
            }
            if (dotProd > currentMax)
            {
                currentMax = dotProd;
                maxIndex = i;
            }
        }
        //remove min and max indices
        closestPoints1.RemoveAt(minIndex);
        if (minIndex < maxIndex) --maxIndex;
        closestPoints1.RemoveAt(maxIndex);

        //take the avg of the two remaining indices
        Vector2 closestPoint = (closestPoints1[0] + closestPoint2[0]) * .5f;
        return closestPoint;
    }

    private float Overlap(Vector2 projA, Vector2 projB)
    {
        if (projA.x < projB.y)
        {
            if (projA.y < projB.x)
            {
                return 0;
            }
            return projA.y - projB.x;
        }
        if (projB.y > projA.x)
        {
            return 0;
        }
        return projB.y - projA.x;
    }

    private Vector2 project(
        List<Vector2> verts,
        Vector2 axis,
        Transform localTransform)
    {
        float projMin = float.MaxValue;
        float projMax = float.MinValue;

        for (int i = 0; i < verts.Count; i++)
        {
            float val = FindScalarProj(localTransform.TransformPoint(verts[i]), axis);
            if (val < projMin)
            {
                projMin = val;
            }
            if (val > projMax)
            {
                projMax = val;
            }
        }
        return new Vector2(projMin, projMax);
    }

    private float FindScalarProj(Vector2 point, Vector3 axis)
    {
        return Vector2.Dot(point, axis) / axis.magnitude;
    }

    private Vector2 perp(Vector2 edge)
    {
        return new Vector2(-edge.y, edge.x);
    }

    void OnDrawGizmos()
    {
        const int LENGTH = 100;

        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f) * Color.red;
        for (int i = 0, len = axes.Count; i < len; i++)
        {
            Gizmos.DrawLine(-LENGTH / 2f * axes[i], LENGTH / 2f * axes[i]);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(collidePoint, .1f);
    }

}
