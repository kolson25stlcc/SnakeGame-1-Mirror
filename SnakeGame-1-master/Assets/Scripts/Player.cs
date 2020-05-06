using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Player : MonoBehaviour
{
    private ScoreController scoreController;

    private ScoreKeeper scoreKeeper;

    #region Player Vars

    bool eat = false;
    private int growBy = 5;
    private int growThisMany = 0;
    private int growCounter = 0;

    public static int[] changeAtScore = {  45, 101, 200, 300 };

    bool dead = false;

    public GameObject bodyPrefab;

    // to get/set location
    private int xCounter = 10;
    private int yCounter = 15;

    public static List<Transform> tail = new List<Transform>();
    public List<int[,]> tailLocCounters = new List<int[,]>(); // used to prevent food from spawning on tail in SpawnFood.cs
    public int[,] headLoc = new int[1, 2]; // used to prevent food from spawning on head in SpawnFood.cs

    
    private List<Directions> pieceDirs = new List<Directions>();    

    private float speed = 0.3f;

    #endregion

    #region Control Vars

    private enum ControlType
    {
        swipe,
        accelerometer,
        arrowKeys
    } private ControlType controller;

    private enum Directions
    {
        right,
        down,
        left,
        up,
        start,
        stop
    }
    Directions direction = Directions.stop;

    bool righting = false;
    bool downing = false;
    bool lefting = false;
    bool uping = false;

    bool transition = false;
    #endregion

    #region OldButDontWantToGetRidOf
    //private float[] validXLocs = {2.511f, 2.232f, 1.953f, 1.674f, 1.395f, 1.116f, 0.837f, 0.558f, 0.279f, 0,
    //                                -2.511f, -2.232f, -1.953f, -1.674f, -1.395f, -1.116f, -0.837f, -0.558f, -0.279f};
    //private float[] validYLocs = {3.431f, 3.152f, 2.873f, 2.315f, 2.036f, 1.757f, 1.478f, 1.199f, 0.92f, 0.641f, 0.362f, 0.083f,
    //                                -0.196f, -0.475f, -0.754f, -1.033f, -1.312f, -1.591f, -1.87f, -2.149f, -2.428f, -2.707f, -2.986f,
    //                                -3.265f, -3.544f, -3.823f, -4.102f, -4.381f, -4.66f};


    #endregion

    /*
     * Need to be able to adjust grid size
     */ 
    #region Grid Vars
    public static float moveDistance = 0.25305f; // *old = .241* specific to the size of the snake blocks

    private float[] validXLocs = new float[20];
    private float[] validYLocs = new float[30];
    #endregion

    /*
     * Need to configure tilt sensitivity
     * Need to configure swipe sensitivity and make it adjust to different screen resolutions and sizes. Maybe use dpi?
     */ 
    #region ControlMethods
    #region ArrowKeyControl
    private void SetDirByArrowKeys()
    {
        if (Input.GetKey(KeyCode.RightArrow) && !lefting)
        {
            direction = Directions.right;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && !uping)
        {
            direction = Directions.down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !righting)
        {
            direction = Directions.left;
        }
        else if (Input.GetKey(KeyCode.UpArrow) && !downing)
        {
            direction = Directions.up;
        }
    }
    #endregion

    #region AccelerometerControl
    private void SetDirByAccelerometer()
    {
        Vector2 acc = Input.acceleration;
        float threshold = 0.1f;
        float maxDir = Mathf.Abs(acc.x) > Mathf.Abs(acc.y) ? Mathf.Abs(acc.x) : Mathf.Abs(acc.y);
        if (direction != Directions.stop)
        {
            Directions maxTilt = Mathf.Abs(acc.x) > Mathf.Abs(acc.y) ? (acc.x > 0 ? Directions.right : Directions.left) : (acc.y > 0 ? Directions.up : Directions.down);
            if (direction == Directions.start)
                direction = Mathf.Abs(acc.x) > Mathf.Abs(acc.y) ? (acc.x > 0 ? Directions.right : Directions.left) : (acc.y > 0 ? Directions.up : Directions.down);
            else if ((righting || lefting) && maxDir > threshold)
            {
                if (maxTilt == Directions.up || maxTilt == Directions.down)
                    direction = acc.y > 0 ? Directions.up : Directions.down;
                // else don't change direction
            }
            else if ((uping || downing) && maxDir > threshold)
            {
                if (maxTilt == Directions.left || maxTilt == Directions.right)
                    direction = acc.x > 0 ? Directions.right : Directions.left;
                // else don't change direction
            }
        }
        else
        {
            if (Input.touchCount > 0)
                direction = Directions.start;
        }
    }
    #endregion

    #region SwipeControl
    float startTime;
    Vector2 startPos;
    bool couldBeSwipe;
    float comfortZone;
    public static float minSwipeDistance = 75f; // in pixels
    float swipeTime;
    float minSwipeTime = 0.05f;
    float maxSwipeTime = 0.2f;
    List<Vector2> touchPositions = new List<Vector2>();
    List<float> timeTouchPositionCreated = new List<float>();
    private void SetDirBySwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {
                swipeTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchPositions.Clear();
                timeTouchPositionCreated.Clear();
            }

            touchPositions.Add(touch.position);
            timeTouchPositionCreated.Add(Time.time);
            swipeTime = Time.time - startTime;
            //float swipeDistance = (touch.position - startPos).magnitude;

            if (swipeTime > maxSwipeTime)
            {
                float timeBack = Time.time - maxSwipeTime;

                for (int i = 0; i < timeTouchPositionCreated.Count; i++)
                {
                    if (timeTouchPositionCreated[i] < timeBack)
                    {
                        timeTouchPositionCreated.RemoveAt(i);
                        touchPositions.RemoveAt(i);
                    }
                }
                startTime = timeTouchPositionCreated.First();
                //swipeTime = Time.time - startTime;
            }


            Vector2 lastPosition = touchPositions.First();
            float swipeDistance = (lastPosition - touch.position).magnitude;

            if (swipeDistance > minSwipeDistance /*&& swipeTime > minSwipeTime*/)
            {
                Vector2 difference = touch.position - lastPosition;
                if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
                {
                    if (!lefting && !righting)
                    {
                        if (difference.x > 0)
                            direction = Directions.right;
                        else
                            direction = Directions.left;
                    }
                }
                else
                {
                    if (!uping && !downing)
                    {
                        if (difference.y > 0)
                            direction = Directions.up;
                        else
                            direction = Directions.down;
                    }
                }
            }
        }
    }
    #endregion 
    #endregion

    // Set variables to playerPrefs
    /*
     * Need to config different speeds to 'Magic Numbers'
     */ 
    private void Awake()
    {

        DontDestroyOnLoad(GameObject.Find("ScoreKeeper"));
        scoreController = Camera.main.GetComponent<ScoreController>();
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();

        eyes = GameObject.Find("eyes").GetComponent<Transform>();
        tongue = GameObject.Find("tongue").GetComponent<Transform>();

        minSwipeDistance = PlayerPrefs.GetInt("Sensitivity");

        int difficulty = PlayerPrefs.GetInt("Difficulty");
        if (difficulty == (int)GameSettings.Difficulty.Easy)
            speed = 0.3f;
        else if (difficulty == (int)GameSettings.Difficulty.Medium)
            speed = 0.22f;
        else if (difficulty == (int)GameSettings.Difficulty.Hard)
            speed = 0.15f;

        //PlayerPrefs.SetInt("SnakeType", (int)GameSettings.SnakeType.RGB);
        snakeType = (GameSettings.SnakeType)PlayerPrefs.GetInt("SnakeType");
        if (snakeType == GameSettings.SnakeType.Original)
            snakeColors = new Color32[] { GameSettings.clrOriginal };
        else if (snakeType == GameSettings.SnakeType.Rainbow)
            snakeColors = GameSettings.clrRainbow;
        else if (snakeType == GameSettings.SnakeType.Pastel)
            snakeColors = GameSettings.clrPastel;
        else if (snakeType == GameSettings.SnakeType.RGB)
        {
            rgb = true;
        }


        if (!rgb)
        {
            MakeColor(gameObject.GetComponent<SpriteRenderer>(), snakeColors[snakeColors.Length - 1]); // initialize head color
        }
    }


    #region RGB vars and method
    private bool rgb = false;
    private Color32 clr4RGB = new Color32(255, 0, 0, 255);
    private readonly float rateOfChange = 150;

    private void RGBGlowTheSnake()
    {
        int R = clr4RGB.r; int G = clr4RGB.g; int B = clr4RGB.b;

        int changeBy = Mathf.RoundToInt(rateOfChange * Time.deltaTime);

        if (R == 255 && G < 255 && B == 0) // add to G til G == 255
        {
            G += changeBy;
            if (G > 255)
                G = 255;
        }
        else if (R > 0 && G == 255 && B == 0) // subtract from R until R == 0
        {
            R -= changeBy;
            if (R < 0)
                R = 0;
        }
        else if (R == 0 && G == 255 && B < 255) // add to B until B == 255 
        {
            B += changeBy;
            if (B > 255)
                B = 255;
        }
        else if (R == 0 && G > 0 && B == 255) // subtract from G until G == 0
        {
            G -= changeBy;
            if (G < 0)
                G = 0;
        }
        else if (R < 255 && G == 0 && B == 255) // add to R until R == 255
        {
            R += changeBy;
            if (R > 255)
                R = 255;
        }
        else if (R == 255 && G == 0 && B > 0) // subtract from B until B == 0
        {
            B -= changeBy;
            if (B < 0)
                B = 0;
        }
        clr4RGB = new Color32((byte)R, (byte)G, (byte)B, 255);

        gameObject.GetComponent<SpriteRenderer>().color = clr4RGB;
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Body"))
        {
            obj.GetComponent<SpriteRenderer>().color = clr4RGB;
        }
    } 
    #endregion

    void Start()
    {

        float X = -2.41f;
        validXLocs[0] = X;
        for (int i = 1; i < validXLocs.Length; i++)
        {
            X += moveDistance;
            validXLocs[i] = X;
        }

        float Y = -4.59f;
        validYLocs[0] = Y;
        for (int i = 1; i < validYLocs.Length; i++)
        {
            Y += moveDistance;
            validYLocs[i] = Y;
        }

        transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);
        headLoc[0, 0] = xCounter; headLoc[0, 1] = yCounter;

        InvokeRepeating("Move", 1, speed);   
    }

    void Update()
    {

       // GameObject.FindGameObjectWithTag("tongue").GetComponent<Transform>().RotateAround(transform.position, new Vector3(0,0,1), 90);
        if (!dead)
        {
            SetDirByArrowKeys();
            //SetDirByAccelerometer();
            //SetDirBySwipe();

            if (rgb)
                RGBGlowTheSnake();

        }
        else // if (dead)
        {
            //scoreController.ResetScore();

            foreach (Transform t in tail)
            {
                Destroy(t.gameObject);
            }
            tail.Clear();
            tailLocCounters.Clear();
            pieceDirs.Clear();


            Destroy(GameObject.FindGameObjectWithTag("Food"));

            //direction = Directions.stop;
            //xCounter = 10;
            //yCounter = 15;
            //transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);

            //setDirBools();
            //dead = false;

            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);

        }

    }

    private GameSettings.SnakeType snakeType;
    private Color32[] snakeColors;
    private int colorCounter = 0;
    private void MakeColor(SpriteRenderer sr, Color32 color)
    {
        sr.color = color;
    }

    private void setDirBools()
    {


        if (direction == Directions.right)
        {
            righting = true;
            downing = false;
            lefting = false;
            uping = false;
        }
        else if (direction == Directions.down)
        {
            righting = false;
            downing = true;
            lefting = false;
            uping = false;
        }
        else if (direction == Directions.left)
        {
            righting = false;
            downing = false;
            lefting = true;
            uping = false;
        }
        else if (direction == Directions.up)
        {
            righting = false;
            downing = false;
            lefting = false;
            uping = true;
        }
        else if (dead)
        {
            righting = false;
            downing = false;
            lefting = false;
            uping = false;
        }
    }

    Directions oldDir = Directions.stop;
    Transform eyes;
    Transform tongue;
    private void rotEyes(Directions oldDir, Directions newDir)
    {
        if (oldDir != newDir)
        {
            if (oldDir == Directions.stop)
            {
                if (newDir == Directions.up)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 0);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 0);
                }
                else if (newDir == Directions.down)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 180);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 180);
                }
                else if (newDir == Directions.right)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                }
                else if (newDir == Directions.left)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                }
            }


            if (oldDir == Directions.up)
            {
                if (newDir == Directions.right)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                }
                else if (newDir == Directions.left)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                }
            }
            else if (oldDir == Directions.down)
            {
                if (newDir == Directions.right)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                }
                else if (newDir == Directions.left)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                }
            }
            else if (oldDir == Directions.right)
            {
                if (newDir == Directions.up)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                }
                else if (newDir == Directions.down)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                }
            }
            else if (oldDir == Directions.left)
            {
                if (newDir == Directions.up)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                }
                else if (newDir == Directions.down)
                {
                    eyes.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                    tongue.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                }
            }
        }
        this.oldDir = newDir; // track for next run
    }

    // worked, but I think overloaded the system because pieces would change position also. UNUSABLE
    //private void rotPiece(Transform obj, Directions oldDir, Directions newDir)
    //{
    //    if (oldDir != newDir)
    //    {
    //        if (oldDir == Directions.stop)
    //        {
    //            if (newDir == Directions.up)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 0);
    //            }
    //            else if (newDir == Directions.down)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 180);
    //            }
    //            else if (newDir == Directions.right)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), -90);
    //            }
    //            else if (newDir == Directions.left)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 90);
    //            }
    //        }


    //        if (oldDir == Directions.up)
    //        {
    //            if (newDir == Directions.right)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), -90);
    //            }
    //            else if (newDir == Directions.left)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 90);
    //            }
    //        }
    //        else if (oldDir == Directions.down)
    //        {
    //            if (newDir == Directions.right)
    //            {
    //                obj.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
    //            }
    //            else if (newDir == Directions.left)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), -90);
    //            }
    //        }
    //        else if (oldDir == Directions.right)
    //        {
    //            if (newDir == Directions.up)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 90);
    //            }
    //            else if (newDir == Directions.down)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), -90);
    //            }
    //        }
    //        else if (oldDir == Directions.left)
    //        {
    //            if (newDir == Directions.up)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), -90);
    //            }
    //            else if (newDir == Directions.down)
    //            {
    //                obj.RotateAround(obj.position, new Vector3(0, 0, 1), 90);
    //            }
    //        }
    //    }
    //}

    private void Move()
    {
        if (!dead)
        {
            //if (transition)
            //{
            //    Vector3 pos = transform.position;
            //    float transAmt = moveDistance / 2f;
            //    if (direction == Directions.right)
            //    {
            //        transform.position = new Vector3(pos.x + transAmt, pos.y, pos.z);
            //    }
            //    else if (direction == Directions.down)
            //    {
            //        transform.position = new Vector3(pos.x, pos.y - transAmt, pos.z);
            //    }
            //    else if (direction == Directions.left)
            //    {
            //        transform.position = new Vector3(pos.x - transAmt, pos.y, pos.z);
            //    }
            //    else if (direction == Directions.up)
            //    {
            //        transform.position = new Vector3(pos.x, pos.y + transAmt, pos.z);
            //    }
            //}

            if (!transition)
            {
                // Save current position
                Vector2 pos = new Vector2(validXLocs[xCounter], validYLocs[yCounter]);
                int[,] posCounters = { { xCounter, yCounter } };

                
                setDirBools();
                Directions headDir = direction;

                rotEyes(oldDir, direction);

                if (direction == Directions.right)
                {
                    xCounter++;
                }
                else if (direction == Directions.down)
                {
                    yCounter--;
                }
                else if (direction == Directions.left)
                {
                    xCounter--;
                }
                else if (direction == Directions.up)
                {
                    yCounter++;
                }
                
                if (xCounter > 19 || xCounter < 0 || yCounter > 29 || yCounter < 0) // if out of bounds
                    dead = true;

                if (!dead)
                {
                    Vector3 posToAddEndTailPieceIfEat = Vector3.zero;
                    Directions dirToAddEndTailPieceIfEat = Directions.start;
                    if (tailLocCounters.Count > 0)
                    {
                        posToAddEndTailPieceIfEat = new Vector3(validXLocs[tailLocCounters.Last()[0, 0]], validYLocs[tailLocCounters.Last()[0, 1]], 0);
                        dirToAddEndTailPieceIfEat = pieceDirs.Last();
                    }
                    if (eat)
                    {
                        scoreController.AddScore();

                        int score = scoreKeeper.pubScore;
                        if (score > changeAtScore[0] - growBy && score < changeAtScore[1])
                        {
                            growBy = 4;
                        }
                        else if (score > changeAtScore[1] - growBy && score < changeAtScore[2])
                        {
                            growBy = 3;
                        }
                        else if (score >= changeAtScore[2] - growBy && score < changeAtScore[3])
                        {
                            growBy = 2;
                        }
                        else if (score >= changeAtScore[3] - growBy)
                        {
                            growBy = 1;
                        }

                        GameObject bodyPiece = Instantiate(bodyPrefab, pos, Quaternion.identity);


                        if (!rgb)
                        {
                            MakeColor(bodyPiece.GetComponent<SpriteRenderer>(), snakeColors[colorCounter]);
                            if (snakeColors.Length > 1)
                                colorCounter = (colorCounter + 1) % snakeColors.Length; 
                        }
                        else
                        {
                            bodyPiece.GetComponent<SpriteRenderer>().color = clr4RGB;
                        }

                        tail.Add(bodyPiece.transform);

                        if (tail.Count == 1) // if this is the first piece added
                        {
                            tail[0].position = pos;
                            tailLocCounters.Add(posCounters);

                            pieceDirs.Add(headDir);
                        }
                        else
                        {
                            bodyPiece.transform.position = posToAddEndTailPieceIfEat;
                            tailLocCounters.Add(tailLocCounters[tailLocCounters.Count - 1]);

                            pieceDirs.Add(pieceDirs.Last());
                        }

                        //rotPiece(bodyPiece.GetComponentInChildren<Transform>(), Directions.stop, pieceDirs.Last());

                        growCounter++;
                        if (growCounter == growThisMany)
                        {
                            growThisMany = 0;
                            growCounter = 0;
                            eat = false;
                        }

                    }

                    transform.position = new Vector3(validXLocs[xCounter], validYLocs[yCounter]);
                    headLoc[0, 0] = xCounter; headLoc[0, 1] = yCounter;

                    // move the tail (each where the one in front of it was) *special case for the first where it moves to the head location and the second where it needs to move to the firsts previous location*
                    if (tail.Count > 0)
                    {
                        Vector2 ptmp = tail[0].position;
                        int[,] ctmp = tailLocCounters[0];
                        Directions dtmp = pieceDirs[0];

                        tail[0].position = pos;
                        tailLocCounters[0] = posCounters;
                        pieceDirs[0] = headDir;

                        //rotPiece(tail[0].GetComponentInChildren<Transform>(), dtmp, pieceDirs[0]);

                        Vector2 ptmptmp = Vector2.zero;
                        int[,] ctmptmp = { { 0, 0 } };
                        Directions dtmptmp = Directions.start;
                        for (int i = 1; i < tail.Count; i++)
                        {
                            if (i == 1)
                            {
                                ptmptmp = tail[i].position;
                                tail[i].position = ptmp;

                                ctmptmp = tailLocCounters[i];
                                tailLocCounters[i] = ctmp;

                                dtmptmp = pieceDirs[i];
                                pieceDirs[i] = dtmp;

                                //rotPiece(tail[i].GetComponentInChildren<Transform>(), dtmptmp, pieceDirs[i]);
                            }
                            else
                            {
                                ptmp = tail[i].position;
                                tail[i].position = ptmptmp;
                                ptmptmp = ptmp;

                                ctmp = tailLocCounters[i];
                                tailLocCounters[i] = ctmptmp;
                                ctmptmp = ctmp;

                                dtmp = pieceDirs[i]; 
                                pieceDirs[i] = dtmptmp; 
                                dtmptmp = dtmp;

                                //rotPiece(tail[i].GetComponentInChildren<Transform>(), dtmp, pieceDirs[i]);
                            }
                        }

                        //string msg = "";
                        //foreach (Directions d in pieceDirs)
                        //{
                        //    msg += " " + d;
                        //}
                        //Debug.Log(msg);
                    }

                    //transition = !transition;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        //if (col.name.StartsWith("Food"))
        if (col.tag == "Food")
        {
            growThisMany += growBy;

            eat = true;

            tongue.gameObject.GetComponent<Animator>().SetTrigger("eat");

            Destroy(col.gameObject);
        }
        else if (col.tag == "Body")
        {

            dead = true;
        }
    }

}
