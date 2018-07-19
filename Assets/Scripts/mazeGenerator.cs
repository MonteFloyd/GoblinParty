using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;




public class Cell
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public Cell()
    {
        up = true;
        down = true;
        left = true;
        right = true;
 
    }
}

public class mazeGenerator : NetworkBehaviour{
    public int width;
    public int height;

    private int currHeight;

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public Shader wallShader;
    public Shader floorShader;

    public GameObject wallDarkPrefab;
    public GameObject floorDarkPrefab;

    public Material wallMaterial;
    public Material floorMaterial;
    public float mazeStartX;
    public float mazeStartY;
    public float mazeStartZ;

    private Cell[] Maze;
    

    private int currentCell;
    private bool[] isVisited;

    private Stack<int> path;

    private List<GameObject>[] wallArray;

    private List<GameObject>[] floorArray;



    private List<GameObject>[] wallArraycopy;

    private List<GameObject>[] floorArraycopy;

    private cameraFollow cam;

    System.Random rand;

    void Awake()
    {
        currHeight = height-1;
        Maze = new Cell[width * height];

        for(int i=0;i< width*height; ++i)
        {
            Maze[i] = new Cell();
        }

        isVisited = new bool[width * height];

        path = new Stack<int>();
        rand = new System.Random();
        currentCell = 0;

        isVisited = new bool[width * height];

        for (int i = 0; i < (width * height) ;  ++i)
        {
            isVisited[i] = false;
        }


        wallArray = new List<GameObject>[height];

        for(int i = 0; i < height; ++i)
        {
            wallArray[i] = new List<GameObject>();
        }

        floorArray = new List<GameObject>[height];

        for (int i = 0; i < height; ++i)
        {
            floorArray[i] = new List<GameObject>();
        }


        wallArraycopy = new List<GameObject>[height];

        for (int i = 0; i < height; ++i)
        {
            wallArraycopy[i] = new List<GameObject>();
        }

        floorArraycopy = new List<GameObject>[height];

        for (int i = 0; i < height; ++i)
        {
            floorArraycopy[i] = new List<GameObject>();
        }
    }

    void Start()
    {
        cam = Camera.main.GetComponent<cameraFollow>();

        
        generateMaze();
        drawMaze();

        for(int i = 0; i < height; ++i){
            foreach(GameObject wall in wallArray[i])
            {
                wall.SetActive(false);

            }

            foreach (GameObject floor in floorArray[i])
            {
                floor.SetActive(false);
            }

        }
        
        for(int i = 0; i < height; ++i){

            foreach(GameObject wall in wallArray[i])
            {
                wall.SetActive(false);

            }

            foreach (GameObject floor in floorArray[i])
            {
                floor.SetActive(false);
            }

        }


        StartCoroutine(tileMaze());


    }


    void switchShader(int row)
    {

        foreach (var a in wallArray[row])
        {
            var darkA = Instantiate(wallDarkPrefab, a.transform.position, a.transform.rotation);
            NetworkServer.Spawn(darkA);
            //a.GetComponent<Renderer>().material = wallMaterial;
            NetworkServer.Destroy(a);
        }

        foreach (var a in floorArray[row])
        {
            var darkA = Instantiate(floorDarkPrefab, a.transform.position, a.transform.rotation);
            NetworkServer.Spawn(darkA);
            NetworkServer.Destroy(a);

            //a.GetComponent<Renderer>().material = floorMaterial;
        }



    }





    IEnumerator tileMaze()
    {

        for (int i = 0; i < height; ++i)
        {
            foreach (GameObject wall in wallArray[i])
            {
                wall.SetActive(true);
                NetworkServer.Spawn(wall);
                yield return new WaitForSeconds(0.05f);
            }

            foreach (GameObject floor in floorArray[i])
            {
                NetworkServer.Spawn(floor);
                floor.SetActive(true);
            }

            yield return new WaitForSeconds(0.1f);

        }

        while(!(currHeight < 0))
        {
            
            switchShader(currHeight);
            currHeight--;
            yield return new WaitForSeconds(0.15f);

        }

        cam.setIncutscene(false);




    }



    void resetMaze()
    {
        path.Clear();

        for(int i = 0; i< (width* height) ; ++i)
        {
            Maze[i].down = true;
            Maze[i].up = true;
            Maze[i].left = true;
            Maze[i].right = true;
        }

    }

    void generateMaze()
    {
        //resetMaze();
        //start from random point
        int startPos = rand.Next(width * height);
        int currentPosition = startPos;
        int newDirection;

        //loop carve
        path.Push(startPos);
        isVisited[startPos] = true;
        currentPosition = startPos;
        newDirection = getValidDirection(startPos);

        if (newDirection == width)
        {
            //We go up
            Maze[currentPosition].up = false;
            Maze[currentPosition + newDirection].down = false;

        }
        else if (newDirection == -1)
        {
            //We go left
            Maze[currentPosition].left = false;
            Maze[currentPosition + newDirection].right = false;

        }
        else if (newDirection == 1)
        {
            //We go right
            Maze[currentPosition].right = false;
            Maze[currentPosition + newDirection].left = false;

        }
        else
        {
            //We go down
            Maze[currentPosition].down = false;
            Maze[currentPosition + newDirection].up = false;
        }

        currentPosition += newDirection;

        while (currentPosition != startPos)
        {
            //Debug.Log("loop");
            isVisited[currentPosition] = true;
            newDirection = getValidDirection(currentPosition);
            if (newDirection == 0)
            {
                path.Pop();
                currentPosition = path.Peek();
            }
            else
            {
                if (newDirection == width)
                {
                    //We go up
                    Maze[currentPosition].up = false;
                    Maze[currentPosition + newDirection].down = false;

                }
                else if (newDirection == -1 )
                {
                    //We go left
                    Maze[currentPosition].left = false;
                    Maze[currentPosition + newDirection].right = false;

                }
                else if (newDirection == 1 )
                {
                    //We go right
                    Maze[currentPosition].right = false;
                    Maze[currentPosition + newDirection].left = false;

                }
                else
                {
                    //We go down
                    Maze[currentPosition].down = false;
                    Maze[currentPosition + newDirection].up = false;
                }
                
                path.Push(currentPosition);
                currentPosition += newDirection;

            }
        }


    }

    int getValidDirection(int currentPosition)
    {
        //up = +width
        //down -width
        //right = +1
        //left = -1

        int[] list= new int[4];
        int arraySize = 0;

        if (currentPosition >= width)
        {
            if (!((currentPosition - width < 0) || isVisited[currentPosition - width]))
            {
                list[arraySize] = -1 * width;
                ++arraySize;
            }
        }

        if (currentPosition+1 < height * width)
        {
            if (!(((currentPosition - width + 1) % (width) == 0) || isVisited[currentPosition + 1]))
            {
                list[arraySize] = 1;
                ++arraySize;
            }
        }

        if (currentPosition != 0)
        {
            if (!((currentPosition % width == 0) || isVisited[currentPosition - 1]))
            {
                list[arraySize] = -1;
                ++arraySize;
            }
        }

        if (currentPosition + width < height * width)
        {
            if (!(((width * height) - currentPosition < width) || isVisited[currentPosition + width]))
            {
                list[arraySize] = width;
                ++arraySize;
            }
        }




        if (arraySize == 0)
        {
            return 0;
        }
        else
        {
            return list[rand.Next(arraySize)];
        }

    }

    
    void drawMaze()
    {
        float leftMost = mazeStartX;

        int currentHeight = 0;
        int countdown = width/2;
        int exitCountdown = rand.Next(0, width);
        

        //left wall :   rotate y:90 ,    startX,      startY,    startZ

        //UP :          no rotate,       startX+5.5,    startY,    startZ+6

        //RIGHT wall:   rotate y:90,     startX+11,   startY,    startZ

        //DOWN wall:    no rotate,       startX+5.5,    startY,    startZ-6
        

        for (int i=0; i< (width*height); ++i)//whole Maze array
        {

            //Instantiate the floor first
            var floor = Instantiate(floorPrefab, new Vector3(mazeStartX - 9 + 6f, 0.01f, mazeStartZ), Quaternion.Euler(0, 90, 0));
            floorArray[currentHeight].Add(floor);
            //NetworkServer.Spawn(floor);

            //if first row- draw down
            if ((i - width < 0))
            {
                if (countdown != 0)
                {
                    var wall = Instantiate(wallPrefab, new Vector3(mazeStartX + 5.5f, mazeStartY, mazeStartZ - 6f), Quaternion.Euler(0, 0, 0));
                    wallArray[currentHeight].Add(wall);
                    //NetworkServer.Spawn(wall);
        
                }
                countdown--;
            }


            //draw left and up
            if (Maze[i].left)
            {
                var wall = Instantiate(wallPrefab, new Vector3(mazeStartX, mazeStartY, mazeStartZ), Quaternion.Euler(0, 90, 0));
                wallArray[currentHeight].Add(wall);
                //NetworkServer.Spawn(wall);
                
            }

            if (Maze[i].up)
            {
                if (currentHeight != height-1)
                {
                    var wall = Instantiate(wallPrefab, new Vector3(mazeStartX + 5.5f, mazeStartY, mazeStartZ + 6f), Quaternion.Euler(0, 0, 0));
                    wallArray[currentHeight].Add(wall);
                    //NetworkServer.Spawn(wall);

                }
                else
                {
                    if (exitCountdown != 0)
                    {
                        var wall = Instantiate(wallPrefab, new Vector3(mazeStartX + 5.5f, mazeStartY, mazeStartZ + 6f), Quaternion.Euler(0, 0, 0));
                        wallArray[currentHeight].Add(wall);
                        //NetworkServer.Spawn(wall);
                    }
                    --exitCountdown;
                }
                

            }

            //if right side, draw right as well
            if (((i - width + 1) % (width)) == 0)
            {
                var wall = Instantiate(wallPrefab, new Vector3(mazeStartX + 11f, mazeStartY, mazeStartZ), Quaternion.Euler(0, 90, 0));
                wallArray[currentHeight].Add(wall);
                //NetworkServer.Spawn(wall);

                mazeStartX = leftMost;
                mazeStartZ += 12f;
                currentHeight++;

            }
            else {

                mazeStartX += 11f;
            }





        }

    }
}
