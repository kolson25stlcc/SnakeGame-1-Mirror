using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFood : MonoBehaviour
{
    public GameObject food;

    private List<GameObject> fiq; // food in question
    private List<int[,]> fiqLocCounters;

    private Color32 color = new Color32(255, 0, 0, 255);

    //public static int[,] foodLocCounters;

    private float[] validXLocs = new float[20];
    private float[] validYLocs = new float[30];

    private List<int[,]> allLocs = new List<int[,]>();

    private int howMany = 5;

    private ScoreKeeper scoreKeeper;

    void Start()
    {
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();

        // populate allLocs (counters)
        string msg = "All locs: ";
        for (int i = 0; i < validXLocs.Length; i++)
        {
            for (int j = 0; j < validYLocs.Length; j++)
            {
                int[,] loc = { { i, j } };
                allLocs.Add(loc);
                msg += loc[0, 0] + " " + loc[0, 1] + " - ";
            }
        }

        // populate valid locations
        float X = -2.41f;
        validXLocs[0] = X;
        for (int i = 1; i < validXLocs.Length; i++)
        {
            X += Player.moveDistance;
            validXLocs[i] = X;
        }

        float Y = -4.59f;
        validYLocs[0] = Y;
        for (int i = 1; i < validYLocs.Length; i++)
        {
            Y += Player.moveDistance;
            validYLocs[i] = Y;
        }


        fiq = new List<GameObject>();
        fiqLocCounters = new List<int[,]>();
        for(int i = 0; i < howMany; i++)
        {
            fiq.Add(null);
            fiqLocCounters.Add(null);
            fiq[i] = Spawn(i);
        }
        //Spawn();
    }

    void Update()
    {
        int score = scoreKeeper.pubScore;

        //if (score >= Player.changeAtScore[0] && score < Player.changeAtScore[1])
        if (score == Player.changeAtScore[0] - howMany)
        {
            howMany = 4;

            color = new Color32(255, 127, 0, 255);
        }
        else if (score == Player.changeAtScore[1] - howMany)
        {
            howMany = 3;

            color = new Color32(245, 247, 0, 255);

        }
        else if (score == Player.changeAtScore[2] - howMany)
        {
            howMany = 2;

            color = new Color32(149, 225, 247, 255);

        }
        else if (score == Player.changeAtScore[3] - howMany)
        {
            howMany = 1;

            color = new Color32(255, 225, 255, 255);
        }

        for (int i = 0; i < fiqLocCounters.Count; i++)
        {
            if (fiq[i] == null)
            {
                if (fiqLocCounters.Count > howMany)
                {
                    //Destroy(fiq[i]);
                    fiq.RemoveAt(i);
                    fiqLocCounters.RemoveAt(i);

                    foreach (GameObject obj in fiq)
                        obj.GetComponent<SpriteRenderer>().color = color;
                }
                else
                {
                    fiq[i] = Spawn(i); 
                }
            }
        }
        //if (fiq == null) // if has been destroyed in another script 
        //    Spawn();   
    }

    private Vector2 getCoods(int idx)
    {
        Vector2 coods = Vector2.zero;
        int[,] tmpCoods = new int[1, 2];

        Player playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        List<int[,]> tail = playerScript.tailLocCounters; //Debug.Log("Tail count: " + tail.Count);
        int[,] head = playerScript.headLoc;

        #region Debug
        /*
         * 
         * Test for random placement of food at high scores
         * 
         */
        //string msg = "This many times: ";
        //bool asdf = false;
        //int num = 377;
        //int counter = 0;
        //do
        //{
        //    asdf = false;
        //    if (Random.Range(0, 569) != num)
        //    {
        //        counter++;
        //        asdf = true;
        //    }
        //    coods = allLocs[Random.Range(0, allLocs.Count - 1)];
        //    foreach (int[,] item in tail)
        //    {
        //        if (item[0, 0] == coods[0, 0] && item[0, 1] == coods[0, 1])
        //              continue;
        //    }
        //} while (asdf);
        //Debug.Log(msg + counter); 
        #endregion

        
        bool badLoc = false;
        do
        {
            badLoc = false;
            tmpCoods = allLocs[Random.Range(0, allLocs.Count - 1)];
            foreach (int[,] item in tail)
            {
                if ((item[0, 0] == tmpCoods[0, 0] && item[0, 1] == tmpCoods[0, 1]) || (head[0, 0] == tmpCoods[0, 0] && head[0, 1] == tmpCoods[0, 1]))
                {
                    badLoc = true;
                    Debug.Log("Happened on body");
                }
            }

            if (!badLoc)
            {
                foreach (int[,] loc in fiqLocCounters)
                {
                    if (loc != null)
                    {
                        if (tmpCoods[0, 0] == loc[0, 0] && tmpCoods[0, 1] == loc[0, 1])
                        {
                            badLoc = true;
                        } 
                    }
                } 
            }

        } while (badLoc);

        coods = new Vector2(validXLocs[tmpCoods[0, 0]], validYLocs[tmpCoods[0, 1]]); 
        

        fiqLocCounters[idx] = new int[,] { { tmpCoods[0,0], tmpCoods[0, 1]} };

        //foodLocCounters = new int[,] { { tmpCoods[0, 0], tmpCoods[0, 1] } };

        return coods;
    }

    private GameObject Spawn(int idx)
    {
        Vector2 coods = getCoods(idx);

        GameObject food = Instantiate(this.food, coods, Quaternion.identity);
        food.GetComponent<SpriteRenderer>().color = color;

        return food;

       // fiq = Instantiate(food, coods, Quaternion.identity);
    }
}
