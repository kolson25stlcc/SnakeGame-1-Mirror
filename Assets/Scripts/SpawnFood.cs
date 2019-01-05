using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject food;

    private GameObject fiq; // food in question

    private Vector3 loc;

    //private float[] validXLocs = {2.511f, 2.232f, 1.953f, 1.674f, 1.395f, 1.116f, 0.837f, 0.558f, 0.279f, 0,
    //                                -2.511f, -2.232f, -1.953f, -1.674f, -1.395f, -1.116f, -0.837f, -0.558f, -0.279f};
    //private float[] validYLocs = {3.431f, 3.152f, 2.873f, 2.315f, 2.036f, 1.757f, 1.478f, 1.199f, 0.92f, 0.641f, 0.362f, 0.083f,
    //                                -0.196f, -0.475f, -0.754f, -1.033f, -1.312f, -1.591f, -1.87f, -2.149f, -2.428f, -2.707f, -2.986f,
    //                                -3.265f, -3.544f, -3.823f, -4.102f, -4.381f, -4.66f};

    private float[] validXLocs = new float[19];
    private float[] validYLocs = new float[30];

    // Start is called before the first frame update
    void Start()
    {
        float minX = -2.511f;
        validXLocs[0] = minX;
        for (int i = 1; i < validXLocs.Length; i++)
        {
            minX += Player.moveDistance;
            validXLocs[i] = minX;
        }

        float minY = -4.66f;
        validYLocs[0] = minY;
        for (int i = 1; i < validYLocs.Length; i++)
        {
            minY += Player.moveDistance;
            validYLocs[i] = minY;
        }


        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (fiq == null) // if has been destroyed in another script
        {
            Spawn();
        }
    }

    private float[] getCoods()
    {
        float[] coods = new float[2];

        float x = Random.Range(-2.6f, 2.6f);
        float y = Random.Range(-4.7f, 3.5f);
        //Debug.Log("Random X: "+x+" Random Y: "+ y);

        float difference = 0;
        float smallestDifference = 10000;
        int locOfSmallestDifference = 0;

        for (int i = 0; i < validXLocs.Length; i++)
        {
            difference = Mathf.Abs(validXLocs[i] - x);
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                locOfSmallestDifference = i;
            }
        }
        x = validXLocs[locOfSmallestDifference];

        // reset smallest difference
        smallestDifference = 10000;

        for (int i = 0; i < validYLocs.Length; i++)
        {
            difference = Mathf.Abs(validYLocs[i] - y);
            //Debug.Log("Diff: "+difference);
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                locOfSmallestDifference = i;
            }
        }
        y = validYLocs[locOfSmallestDifference];

        coods[0] = x;
        coods[1] = y;


        return coods;

    }

    private void Spawn()
    {

        float[] coods = getCoods();

        loc = new Vector3(coods[0], coods[1], 0);
        
        fiq = Instantiate(food, loc, Quaternion.identity);
    }
}
