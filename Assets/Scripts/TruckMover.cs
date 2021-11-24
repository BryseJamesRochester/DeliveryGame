using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckMover : MonoBehaviour
{
    #region Singleton Pattern

    private static TruckMover _instance;

    public static TruckMover Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public GameObject truck;

    //lists of transform points to travel to
    public GameObject pathPointsList;
    public GameObject levelPointsList;
    //truck animator
    public Animator anim;

    //Dictionary format is pathPoints["startpoint-endpoint"] = [startpoint Transform, endpoint Transform]
    private Dictionary<float[], Transform[]> pathPoints;
    private Dictionary<int, Transform> levelPoints;

    SceneManagement sceneManagement;

    public int currentLevel;
    private int nextLevel;

    //time to travel between levels
    public float travelTimeBetweenLevels = 2;
    private float travelTime = 2;
    private int maxLevel = 4;
    //a queue of all moves to make in a level transistion
    private Queue<float[]> movesToMake;

    //truck currently moving
    private bool moving;

    //transition starting position
    private Vector3 startingPos;
    //transition ending position
    private Vector3 targetPos;
    private float lerpingInterpolation;

    void Start()
    {
        sceneManagement = SceneManagement.Instance;
        //initializes all points truck can travel to
        initializePathPoints();
        foreach (KeyValuePair<float[], Transform[]> key in pathPoints)
            Debug.Log(key.Key[0] + " " + key.Key[1] + ":" + key.Value[0].position + " " + key.Value[1].position);
        initializeLevelPoints();
        movesToMake = new Queue<float[]>();
        currentLevel = 0;
        moving = false;
    }

    void Update()
    {
        //accepting user input(not moving)
        if (!moving)
        {
            if (Input.GetButtonDown("Submit"))
            {
                if(currentLevel!=0)
                    sceneManagement.goToLevel(currentLevel.ToString());
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                    if (currentLevel < maxLevel)
                    {
                        goToNextLevel();
                    }
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                    if (currentLevel > 0)
                    {
                        goToPreviousLevel();
                    }
            }
        }
        //truck is in motion
        if (moving)
        {
            //lerp truck to new position
            lerpingInterpolation += Time.deltaTime / travelTime;
            truck.transform.position = Vector3.Lerp(startingPos, targetPos, lerpingInterpolation);
            //if truck made it, check if it has any more moves, if not, set moving to false and await user input for next move
            if (truck.transform.position == targetPos)
            {
                if (movesToMake.Count > 0)
                {
                    makeNextMove();
                }
                else
                {
                    currentLevel = nextLevel;
                    setTruckLevelPosition(currentLevel);
                    anim.SetBool("Moving", false);
                    moving = false;
                }
            }
        }
 
    }


    private void move()
    {
        addMovesToQueue();
        travelTime = travelTimeBetweenLevels / movesToMake.Count;
        makeNextMove();
    }
    private void goToNextLevel()
    {
        nextLevel = currentLevel + 1;
        moving = true;
        anim.SetBool("Moving", true);
        move();
    }

    private void goToPreviousLevel()
    {
        nextLevel = currentLevel - 1;
        moving = true;
        anim.SetBool("Moving", true);
        move();
    }

    //add all of the necessary moves to go from currentLevel to NextLevel, in most cases will only be one
    private void addMovesToQueue()
    {
        float[] path = findPathPointsKey(new float[] { currentLevel, nextLevel });
        if (path[0] != -1)
            movesToMake.Enqueue(path);
        else
        {
            float start = currentLevel;
            Dictionary<float[], Transform[]>.KeyCollection keys = pathPoints.Keys;
            if (currentLevel < nextLevel)//going to next level, (whole if is for 1 comparison operator, will look into variable operators)
            {
                while (start != nextLevel)
                {
                    foreach(float[] key in keys)
                    {
                        if(key[0] == start && key[1] > key[0])
                        {
                            movesToMake.Enqueue(key);
                            start = key[1];
                            break;
                        }

                    }
                }
            }
            else//going to previous level
            {
                while (start != nextLevel)
                {
                    foreach (float[] key in keys)
                    {
                        if (key[0] == start && key[1] < key[0])
                        {
                            movesToMake.Enqueue(key);
                            start = key[1];
                            break;
                        }

                    }
                }
            }
            
        }





    }

    //set starting position and ending position from the next move in the queue, lerping in Update() will take care of the rest
    private void makeNextMove()
    {
        float[] currentMove = movesToMake.Dequeue();
        //build pathPoints key in the format of "currentPosition-nextPosition"
        Transform[] positions = pathPoints[currentMove];
        //ensure truck starts from correct position and rotation
        truck.transform.position = positions[0].position;
        truck.transform.localScale = positions[0].localScale;
        truck.transform.rotation = positions[0].rotation;
        lerpingInterpolation = 0;
        startingPos = positions[0].position;
        targetPos = positions[1].position;
    }

    void initializePathPoints()
    {
        pathPoints = new Dictionary<float[], Transform[]>();
        foreach (Transform child in pathPointsList.transform)
        {
            int index = child.name.IndexOf("s");
            if (index > 0)
            {
                string pathString = child.name.Substring(0, index);
                int length = pathString.Length;
                index = pathString.IndexOf("-");
                length = length - index - 1;
                float[] path = new float[2] { float.Parse(pathString.Substring(0, index)), float.Parse(pathString.Substring(index+1, length)) };
                float[] key = findPathPointsKey(path);
                if (key[0] == -1)
                {
                    pathPoints[path] = new Transform[2];
                    key = path;
                }
                pathPoints[key][0] = child.transform;
            }
            else
            {
                index = child.name.IndexOf("e");
                string pathString = child.name.Substring(0, index);
                int length = pathString.Length;
                index = pathString.IndexOf("-");
                length = length - index - 1;
                float[] path = new float[2] { float.Parse(pathString.Substring(0, index)), float.Parse(pathString.Substring(index + 1, length)) };
                float[] key = findPathPointsKey(path);
                if (key[0] == -1)
                {
                    pathPoints[path] = new Transform[2];
                    key = path;
                }
                pathPoints[key][1] = child.transform;
            }
        }
    }

    void setTruckLevelPosition(int level)
    {
        truck.transform.position = levelPoints[level].position;
        truck.transform.localScale = levelPoints[level].localScale;
        truck.transform.rotation = levelPoints[level].rotation;
    }

    void initializeLevelPoints()
    {
        levelPoints = new Dictionary<int, Transform>();
        foreach (Transform child in levelPointsList.transform)
        {
            levelPoints[int.Parse(child.name)] = child.transform;
        }
    }


    float[] findPathPointsKey(float[] toFind)
    {
        foreach(KeyValuePair<float[], Transform[]> kvp in pathPoints)
        {
            if (toFind[0] == kvp.Key[0] && toFind[1] == kvp.Key[1])
                return kvp.Key;
        }
        return new float[2] { -1, -1 };
    }


}
