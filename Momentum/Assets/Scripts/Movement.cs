using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public Vector2 position;
    public Vector2 velocity;
    public Vector2 accelaration;

    public float mass = 1.0f;
    private Vector2 worldSize;
    private bool fPress;

    // Use this for initialization
    void Start () {
        position = transform.position;
        fPress = false;
        worldSize = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);
        worldSize = Camera.main.ScreenToWorldPoint(worldSize);
    }
	
	// Update is called once per frame
	void Update () {
        //ApplyForce (gravity);//Apply a force to our object
        if (Input.GetMouseButton(0))
        {
            ApplyMouse();
        }


        //apply friction
        if (Input.GetKeyDown(KeyCode.F))
        {
            fPress = !fPress;
            Debug.Log(fPress);
        }


        if (fPress == true)
        {
            ApplyFriction(1.0f);
        }
        UpdatePosition();
        //wrap on screen
        Bounce();
        SetTransform(); 
	}

    void UpdatePosition()
    {
        velocity += accelaration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        accelaration = Vector2.zero;
    }

    void ApplyForce(Vector2 force)
    {
        if(mass <= 0.0f)
        {
            mass = .01f;
        }
        accelaration += force / mass;
    }

    void ApplyFriction(float coeff)
    {
        Vector2 friction = velocity * -1.0f;
        friction.Normalize();
        friction *= coeff;
        accelaration += friction;
    }

    void SetTransform()
    {
        transform.position = position;
    }

    void ApplyMouse()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //gets the distance b/w the mouse and the monster 
        float distance = Vector2.Distance(mouseWorldPos, gameObject.transform.position);
        Vector2 dir = mouseWorldPos - position;
        Vector2 reducedForce = (dir * -10.0f) / distance;
        ApplyForce(reducedForce);
        Debug.Log("left click");
    }

    void Bounce()
    {
        if (position.x > worldSize.x)
        {
            position.x = worldSize.x;
            velocity.x *= -1;
        }
        else if (position.x < -worldSize.x)
        {
            velocity.x *= -1;
            position.x = -worldSize.x;
        }

        if (position.y > worldSize.y)
        {
            position.y = worldSize.y;
            velocity.y *= -1;
        }
        else if (position.y < -worldSize.y)
        {
            velocity.y *= -1;
            position.y = -worldSize.y;
        }

    }

}
