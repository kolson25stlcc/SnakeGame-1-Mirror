using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject food;

    private GameObject fiq; // food in question

    private Vector3 loc;

    // Start is called before the first frame update
    void Start()
    {
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

    private void Spawn()
    {
       


        loc = new Vector3(Random.Range(-2.6f, 2.6f), Random.Range(-4.8f, 4.8f), 0);
        
        fiq = Instantiate(food, loc, Quaternion.identity);
    }
}
