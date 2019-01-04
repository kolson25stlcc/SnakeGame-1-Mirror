using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{

    bool eat = false;

    bool dead = false;

    public GameObject bodyPrefab;

    public float moveDistance = 0.03f;
    Vector2 dir = Vector2.zero;
    Vector2 right; bool righting = false;
    Vector2 down; bool downing = false;
    Vector2 left; bool lefting = false;
    Vector2 up; bool uping = false;

    List<Transform> tail = new List<Transform>();

    public float speed = 0.3f;

    void Start()
    {
        right = new Vector2(moveDistance, 0);
        down = new Vector2(0, -moveDistance);
        left = new Vector2(-moveDistance, 0);
        up = new Vector2(0, moveDistance);

        InvokeRepeating("Move", 1, speed);   
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (Input.GetKey(KeyCode.RightArrow) && !lefting)
            {
                dir = right;

            }
            else if (Input.GetKey(KeyCode.DownArrow) && !uping)
            {
                dir = down;

            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !righting)
            {
                dir = left;

            }
            else if (Input.GetKey(KeyCode.UpArrow) && !downing)
            {
                dir = up;

            }
            

        }
        else
        {
            if (Input.GetKey(KeyCode.R))
            {
                foreach (Transform t in tail)
                {
                    Destroy(t.gameObject);
                }
                tail.Clear();

                transform.position = Vector3.zero;
                dir = Vector2.zero;

                dead = false;
            }
        }
        
    }

    private void setDirBoolToTrueOthersToFalse()
    {
        if (dir == right)
        {
            righting = true;
            downing = false;
            lefting = false;
            uping = false;
        }
        else if (dir == down)
        {
            righting = false;
            downing = true;
            lefting = false;
            uping = false;
        }
        else if (dir == left)
        {
            righting = false;
            downing = false;
            lefting = true;
            uping = false;
        }
        else if (dir == up)
        {
            righting = false;
            downing = false;
            lefting = false;
            uping = true;
        }
    }

    private void Move()
    {
        if (!dead)
        {
            // Save current position
            Vector2 pos = transform.position;
            
            transform.Translate(dir);
            setDirBoolToTrueOthersToFalse();

            if (eat)
            {
                GameObject bodyPiece = Instantiate(bodyPrefab, pos, Quaternion.identity);

                tail.Insert(0, bodyPiece.transform);

                eat = false;
            }
            else if (tail.Count > 0)
            {
                // Move last tail element to where the head was ...
                // basically rotating the last body piece to behind the head *after* the head moves forward
                // so the rest stay in place and snake maintains shape
                tail.Last().position = pos;

                // Add to front of the list, remove from back
                tail.Insert(0, tail.Last());
                tail.RemoveAt(tail.Count - 1);

            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        //if (col.name.StartsWith("Food"))
        if (col.tag == "Food")
        {
            eat = true;

            Destroy(col.gameObject);
        }
        else if (col.tag == "Body")
        {

            dead = true;
        }
    }

}
