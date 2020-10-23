using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/*
 what needs doin
 1.fix when checking valid wall and next to border
 2.when generating and change corner ensure always move in same direction
     */
public class prevPos
{
    public Vector2 currPos, currDir;
    public prevPos(Vector2 currPos, Vector2 currDir)
    {
        this.currPos = currPos;
        this.currDir = currDir;
    }
}
//potentially not work with finsihing when all pellets eaten
//probably add case where after changed direction just ensure move in that direction first so get three wide gaps
//max 92 by 92 before too much
//min height is 18 min with is 28
public class RandomSizeGenerator : MonoBehaviour
{
    public int width, height;
    bool pathStarted = false;
    public int[,] map;
    public int pelletAmount;
    enum TileType { empty, path, wall, borderWall, spawnEmpty, spawnOpening, emptyOutside, emptyInside, insideCorner, outsideCorner, teloBlocker, powerPellet };
    enum CurrDirection { Up, Down, Left, Right };
    public GameObject[] mapComponents;
    Vector2 currPos, currDir, lastDir;

    List<prevPos> prevPos = new List<prevPos>();
    List<Vector2> joins = new List<Vector2>();

    //ghost4 waypoints
    public GameObject wayPointStart;
    int wayPointX, wayPointY;

    void Start()
    {
        map = new int[width, height];
        generateMap();
    }
    void generateMap() //call all methods in turn to generate the map
    {
        ghostSpawn();
        makeBorder();
        makeTeloporters();       
        makePath();
        instanciateMap();
        Ghost4Waypoints.currDir = Vector2.zero;
        wayPointStart.AddComponent<Ghost4Waypoints>();
    }
    void ghostSpawn() //generate the area for the ghosts to spawn in
    {
        int spawnWidth = 10, spawnHeight = 7;
        int startX = width / 2 - 5, startY = height / 2 - 4;
        //make outside path around
        for (int i = 0; i < spawnWidth; i++)
        {
            map[startX + i, startY] = (int)TileType.spawnEmpty;
        }
        for (int i = 0; i < spawnHeight; i++)
        {
            map[startX, startY + i] = (int)TileType.spawnEmpty;
        }
        for (int i = 0; i < spawnWidth; i++)
        {
            map[startX + i, startY + spawnHeight - 1] = (int)TileType.spawnEmpty;
        }
        for (int i = 0; i < spawnHeight; i++)
        {
            map[startX + spawnWidth - 1, startY + i] = (int)TileType.spawnEmpty;
        }
        prevPos.Add(new prevPos(new Vector2(startX + 5, startY -2), Vector2.zero)); //do this so that if path cannot reach below normally still can generate one down their
       // Debug.Log(new Vector2(startX, startY - 2));
        for (int i = 2; i < spawnWidth - 2; i++)
        {
            for (int j = 2; j < spawnHeight - 2; j++)
                map[startX+i, startY+j] = (int)TileType.spawnEmpty;
        }
        //make ghost exit
        map[startX + 4, startY + spawnHeight - 2] = (int)TileType.spawnOpening;
        map[startX + 5, startY + spawnHeight - 2] = (int)TileType.spawnOpening;

    }
    void makeTeloporters() //generate the two teloporters for pacStudent to use
    {
        //height of 11; width of 6
        int teloWidth = 6, teloHeight = 11;
        int startY = Random.Range(2, height - teloHeight - 5);
        //spawn actual paths left side
        makeWidthLine(1, startY - 1, 0, teloWidth, (int)TileType.path);
        makeWidthLine(1, startY + teloHeight, 0, teloWidth, (int)TileType.path);        
        for (int i = -1; i <= teloHeight; i++)
        {
            map[6, startY + i] = (int)TileType.path;
            joins.Add(new Vector2(6, startY + i));
        }
        //spawn actual paths right side
        makeWidthLine(1, startY - 1, width - teloWidth - 1, teloWidth, (int)TileType.path);
        makeWidthLine(1, startY + teloHeight, width - teloWidth -1 , teloWidth, (int)TileType.path);

        for (int i = -1; i <= teloHeight; i++)
        {
            map[width- teloWidth -1, startY + i] = (int)TileType.path;
            joins.Add(new Vector2(width - teloWidth -1, startY + i));
        }

        //spawn actuall walls left
        makeWidthLine(0, startY, 0, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + 4, 0, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + 5, 0, teloWidth, (int)TileType.spawnEmpty);
        //spawn actual walls right
        makeWidthLine(0, startY, width - teloWidth, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + 4, width - teloWidth, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + 5, width - teloWidth, teloWidth, (int)TileType.spawnEmpty);

        Instantiate(mapComponents[(int)TileType.teloBlocker], new Vector2(-1, startY + 5), Quaternion.identity, transform);
        Instantiate(mapComponents[(int)TileType.teloBlocker], new Vector2(width, startY + 5), Quaternion.identity, transform);

        //left walls
        makeWidthLine(0, startY + 6, 0, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + teloHeight - 1, 0, teloWidth, (int)TileType.borderWall);
        wayPointX = 0; wayPointY = startY + 6;

        //right walls
        makeWidthLine(0, startY + 6, width - teloWidth, teloWidth, (int)TileType.borderWall);
        makeWidthLine(0, startY + teloHeight - 1, width - teloWidth, teloWidth, (int)TileType.borderWall);
        //left walls
        for (int i = 1; i <= 3; i++)
            map[teloWidth - 1, startY + i] = (int)TileType.borderWall;
        for (int i = 1; i <= 3; i++)
            map[teloWidth - 1, startY + 6 + i] = (int)TileType.borderWall;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < teloWidth - 1; j++)
                map[j, startY + 1 + i] = (int)TileType.emptyOutside;
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < teloWidth - 1; j++)
                map[j, startY + 7 + i] = (int)TileType.emptyOutside;
        }
        //right walls
        for (int i = 1; i <= 3; i++)
            map[width - teloWidth, startY + i] = (int)TileType.borderWall;
        for (int i = 1; i <= 3; i++)
            map[width - teloWidth, startY + 6 + i] = (int)TileType.borderWall;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < teloWidth - 1; j++)
                map[width-j-1, startY + 1 + i] = (int)TileType.emptyOutside;
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < teloWidth - 1; j++)
                map[width-j-1, startY + 7 + i] = (int)TileType.emptyOutside;
        }
    }
    void makeWidthLine(int startPos, int startY, int startX, int teloWidth, int tileType)
    {
        for (int i = startPos; i < teloWidth; i++)
        {
            map[startX + i, startY] = tileType;
            if (tileType == (int)TileType.path || tileType == (int)TileType.powerPellet)
            {
                joins.Add(new Vector2(startX + i, startY));
              //  if(startX + i > 0 && startY > 0)
                //    prevPos.Add(new prevPos(new Vector2(startX + i, startY), Vector2.up));
            }
        }
    }
    void makeBorder()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                    map[i, j] = (int)TileType.borderWall;
            }
        }
    }
    void makePath()
    {
        if (prevPos.Count > 0 && pathStarted == true)
            prevPos.RemoveAt(prevPos.Count - 1);
        else
        {
            map[1, height - 2] = (int)TileType.path;
            currPos = new Vector2(1, height - 2);
        }
        List<Vector2> nextPos = validDirections();
        if(pathStarted == false)
            pathStarted = true;

        while (nextPos.Count > 0)
        {
            int rand = Random.Range(0, nextPos.Count);
            int posX = Mathf.RoundToInt(nextPos[rand].x), posY = Mathf.RoundToInt(nextPos[rand].y);
            map[posX, posY] = (int)TileType.path;
            currDir = (nextPos[rand] - currPos).normalized;
            currPos = new Vector2(posX, posY);
            joins.Add(currPos);
            prevPos.Add(new prevPos(currPos, currDir));
            nextPos = validDirections();
        }
        //check if no valid positions then if possible add join
        if (prevPos.Count > 0)
        {
            currPos = new Vector2((int)prevPos[prevPos.Count - 1].currPos.x, (int)prevPos[prevPos.Count - 1].currPos.y);
            currDir = prevPos[prevPos.Count - 1].currDir;
            makePath();
        }
        addJoin();
    }
    void addJoin()//after map made check if the joins are possible to be added so that less paths are one way
    {
        for (int i = 0; i < joins.Count; i++)
        {
            int posX = Mathf.RoundToInt(joins[i].x), posY = Mathf.RoundToInt(joins[i].y);
            //left
            if (posX - 3 > 0 && map[posX - 1, posY] == (int)TileType.empty && map[posX - 2, posY] == (int)TileType.empty && canSpawnPath(posX - 3, posY)
                && map[posX - 1, posY + 1] == (int)TileType.empty && map[posX - 1, posY + 2] == (int)TileType.empty &&
                map[posX - 1, posY - 1] == (int)TileType.empty && map[posX - 1, posY - 2] == (int)TileType.empty
                && map[posX - 2, posY + 1] == (int)TileType.empty && map[posX - 2, posY + 2] == (int)TileType.empty &&
                map[posX - 2, posY - 1] == (int)TileType.empty && map[posX - 2, posY - 2] == (int)TileType.empty)
            {
                map[posX - 1, posY] = (int)TileType.path;
                map[posX - 2, posY] = (int)TileType.path;
            }
            //right
            if (posX + 3 < width && map[posX + 1, posY] == (int)TileType.empty && map[posX + 2, posY] == (int)TileType.empty && canSpawnPath(posX + 3, posY)
                && map[posX + 1, posY + 1] == (int)TileType.empty && map[posX + 1, posY + 2] == (int)TileType.empty &&
                map[posX + 1, posY - 1] == (int)TileType.empty && map[posX + 1, posY - 2] == (int)TileType.empty
                && map[posX + 2, posY + 1] == (int)TileType.empty && map[posX + 2, posY + 2] == (int)TileType.empty &&
                map[posX + 2, posY - 1] == (int)TileType.empty && map[posX + 2, posY - 2] == (int)TileType.empty)
            {
                map[posX + 1, posY] = (int)TileType.path;
                map[posX + 2, posY] = (int)TileType.path;
            }

            //down
            if (posY - 3 > 0 && map[posX, posY - 1] == (int)TileType.empty && map[posX, posY - 2] == (int)TileType.empty && canSpawnPath(posX, posY - 3)
                && map[posX + 1, posY - 1] == (int)TileType.empty && map[posX + 2, posY - 1] == (int)TileType.empty &&
                map[posX - 1, posY - 1] == (int)TileType.empty && map[posX - 2, posY - 1] == (int)TileType.empty
                && map[posX + 1, posY - 2] == (int)TileType.empty && map[posX + 2, posY - 2] == (int)TileType.empty &&
                map[posX - 1, posY - 2] == (int)TileType.empty && map[posX - 2, posY - 2] == (int)TileType.empty)

            {
                map[posX, posY - 1] = (int)TileType.path;
                map[posX, posY - 2] = (int)TileType.path;
            }
            //up
            if (posY + 3 < height && map[posX, posY + 1] == (int)TileType.empty && map[posX, posY + 2] == (int)TileType.empty && canSpawnPath(posX, posY + 3)
                && map[posX + 1, posY + 1] == (int)TileType.empty && map[posX + 2, posY + 1] == (int)TileType.empty &&
                map[posX - 1, posY + 1] == (int)TileType.empty && map[posX - 2, posY + 1] == (int)TileType.empty
                && map[posX + 1, posY + 2] == (int)TileType.empty && map[posX + 2, posY + 2] == (int)TileType.empty &&
                map[posX - 1, posY + 2] == (int)TileType.empty && map[posX - 2, posY + 2] == (int)TileType.empty)

            {
                map[posX, posY + 1] = (int)TileType.path;
                map[posX, posY + 2] = (int)TileType.path;
            }

        }
    }
    bool canSpawnPath(int posX, int posY)
    {
        return isPath(posX, posY) || map[posX, posY] == (int)TileType.spawnEmpty;
    }
    List<Vector2> validDirections()
    {
        List<Vector2> directions = new List<Vector2>();
        //up, down, left, right
        int posX = Mathf.RoundToInt(currPos.x), posY = Mathf.RoundToInt(currPos.y);
        if (map[posX - 1, posY] == (int)TileType.empty && checkLeft(posX - 1, posY) && checkDown(posX - 1, posY) && checkUp(posX - 1, posY))//check left
        {
            directions.Add(new Vector2(posX - 1, posY));
            if (currDir == Vector2.left)
            {
                directions.Add(new Vector2(posX - 1, posY));
                directions.Add(new Vector2(posX - 1, posY)); 
                directions.Add(new Vector2(posX - 1, posY));
                directions.Add(new Vector2(posX - 1, posY));
            }
        }
        if (map[posX + 1, posY] == (int)TileType.empty && checkRight(posX + 1, posY) && checkDown(posX + 1, posY) && checkUp(posX + 1, posY)) //check right
        {
            directions.Add(new Vector2(posX + 1, posY));
            if (currDir == Vector2.right)
            {
                directions.Add(new Vector2(posX + 1, posY));
                directions.Add(new Vector2(posX + 1, posY)); 
                directions.Add(new Vector2(posX + 1, posY));
                directions.Add(new Vector2(posX + 1, posY));
            }
        }
        if (map[posX, posY + 1] == (int)TileType.empty && checkUp(posX, posY + 1) && checkLeft(posX, posY + 1) && checkRight(posX, posY + 1)) //check up
        {
            directions.Add(new Vector2(posX, posY + 1));
            if (currDir == Vector2.up)
            {
                directions.Add(new Vector2(posX, posY + 1));
                directions.Add(new Vector2(posX, posY + 1));
                directions.Add(new Vector2(posX, posY + 1));
                directions.Add(new Vector2(posX, posY + 1));
            }
        }
        if (map[posX, posY - 1] == (int)TileType.empty && checkDown(posX, posY - 1) && checkLeft(posX, posY - 1) && checkRight(posX, posY - 1)) //check down
        {
            directions.Add(new Vector2(posX, posY - 1));
            if (currDir == Vector2.down)
            {
                directions.Add(new Vector2(posX, posY - 1));
                directions.Add(new Vector2(posX, posY - 1));
                directions.Add(new Vector2(posX, posY - 1));
                directions.Add(new Vector2(posX, posY - 1));
            }
        }
        return directions;
    }
    bool checkRight(int posX, int posY) //check straight up as well as the diagonals ,x+2 etc
    {           
        if (posX + 2 < width && map[posX + 2, posY] != (int)TileType.empty && map[posX + 2, posY] != (int)TileType.borderWall)//check up
                return false;
        
        if (posX + 1 < width && map[posX + 1, posY] != (int)TileType.empty && map[posX + 1, posY] != (int)TileType.borderWall)//check up
                return false;
        if (currDir != Vector2.left)
        {

            if (posX + 2 < width && posY + 1 < height && map[posX + 2, posY + 1] != (int)TileType.empty && map[posX + 2, posY + 1] != (int)TileType.borderWall)//check up
                return false;
            if (posX + 2 < width && posY - 1 > 0 && map[posX + 2, posY - 1] != (int)TileType.empty && map[posX + 2, posY - 1] != (int)TileType.borderWall)//check up
                return false;
        }
        return true;
    }
    bool checkLeft(int posX, int posY) //check straight up as well as the diagonals ,x+2 etc
    {            
        if (posX - 2 > 0 && map[posX - 2, posY] != (int)TileType.empty && map[posX - 2, posY] != (int)TileType.borderWall)//check up
                return false;
        
        if (posX - 1 > 0 && map[posX - 1, posY] != (int)TileType.empty && map[posX - 1, posY] != (int)TileType.borderWall)//check up
                return false;
        if (currDir != Vector2.right)
        {

            //diagonal
            if (posX - 2 > 0 && posY + 1 < height && map[posX - 2, posY + 1] != (int)TileType.empty && map[posX - 2, posY + 1] != (int)TileType.borderWall)//check up
                return false;
            if (posX - 2 > 0 && posY - 1 > 0 && map[posX - 2, posY - 1] != (int)TileType.empty && map[posX - 2, posY - 1] != (int)TileType.borderWall)//check up
                return false;
        }
        return true;
    }
    bool checkDown(int posX, int posY) //check straight up as well as the diagonals ,x+2 etc
    {            
        if (posY - 2 > 0 && map[posX, posY - 2] != (int)TileType.empty && map[posX, posY - 2] != (int)TileType.borderWall)//check up
                return false;
        if (posY - 1 > 0 && map[posX, posY - 1] != (int)TileType.empty && map[posX, posY - 1] != (int)TileType.borderWall)//check up
            return false;
        if (currDir != Vector2.up)
        {
            if (posY - 2 > 0 && posX + 1 < width && map[posX + 1, posY - 2] != (int)TileType.empty && map[posX + 1, posY - 2] != (int)TileType.borderWall)//check up
                return false;
            if (posY - 2 > 0 && posX - 1 > 0 && map[posX - 1, posY - 2] != (int)TileType.empty && map[posX - 1, posY - 2] != (int)TileType.borderWall)//check up
                return false;
        }
        return true;
    }
    bool checkUp(int posX, int posY) //check straight up as well as the diagonals ,x+2 etc
    {      
        
        if (posY + 2 < height && map[posX, posY + 2] != (int)TileType.empty && map[posX, posY + 2] != (int)TileType.borderWall)//check up
                return false;
        if (posY + 1 < height && map[posX, posY + 1] != (int)TileType.empty && map[posX, posY + 1] != (int)TileType.borderWall)//check up
                return false;
        if (currDir != Vector2.down)
        {
            if (posY + 2 < height && posX + 1 < width && map[posX + 1, posY + 2] != (int)TileType.empty && map[posX + 1, posY + 2] != (int)TileType.borderWall)//check up
                return false;
            if (posY + 2 < height && posX - 1 > 0 && map[posX - 1, posY + 2] != (int)TileType.empty && map[posX - 1, posY + 2] != (int)TileType.borderWall)//check up
                return false;
        }
        return true;
    }
    void instanciateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int mapValue = map[i, j];
                if (map[i, j] == (int)TileType.path)
                {
                    pelletAmount++;
                    int rand = Random.Range(0, 50);
                    if (rand == 0) //randomly choose if a pellet should be a power pellet or not
                    {
                        map[i, j] = (int)TileType.powerPellet;
                        mapValue = map[i, j];
                    }
                }
                if (mapValue != (int)TileType.spawnEmpty)
                {
                    Quaternion rote;
                    if (mapValue == (int)TileType.spawnOpening || mapValue == (int)TileType.emptyOutside || mapValue == (int)TileType.emptyInside || isPath(i, j))
                        rote = Quaternion.identity;
                    else
                    {
                        rote = determineRote(i, j);
                        mapValue = map[i, j];
                    }
                    if(i == wayPointX && j == wayPointY) //if at the waypint start position then set this as the start position for the waypoint system
                        wayPointStart = Instantiate(mapComponents[mapValue], new Vector2(i, j), rote, this.transform);
                    else
                        Instantiate(mapComponents[mapValue], new Vector2(i, j), rote, this.transform);
                }
            }
        }
    }

    Quaternion determineRote(int i, int j)
    {
        //get all 8 positions around it
        int wallCount = 0, diagWallCount = 0;
        bool left = checkWallType(i - 1, j);
        if (left)
            wallCount++;
        bool right = checkWallType(i + 1, j);
        if (right)
            wallCount++;
        bool up = checkWallType(i, j + 1);
        if (up)
            wallCount++;
        bool down = checkWallType(i, j - 1);
        if (down)
            wallCount++;

        bool leftUp = checkWallType(i - 1, j + 1);
        if (leftUp)
            diagWallCount++;
        bool leftDown = checkWallType(i - 1, j - 1);
        if (leftDown)
            diagWallCount++;
        bool rightUp = checkWallType(i + 1, j + 1);
        if (rightUp)
            diagWallCount++;
        bool rightDown = checkWallType(i + 1, j - 1);
        if (rightDown)
            diagWallCount++;

        if (wallCount == 4)
        {
            if (diagWallCount == 4) 
            {
                map[i, j] = (int)TileType.emptyInside;
                return Quaternion.identity;
            }
            else if(diagWallCount == 3)
            {
                if (!rightUp)
                {
                    checkCornerType(i, j);
                    return Quaternion.identity;
                }
                else if (!leftUp)
                {
                    checkCornerType(i, j);
                    return Quaternion.Euler(0, 0, 90);
                }
                else if (!leftDown)
                {
                    checkCornerType(i, j);
                    return Quaternion.Euler(0, 0, 180);
                }
                else if (!rightDown)
                {
                    checkCornerType(i, j);
                    return Quaternion.Euler(0, 0, -90);
                }
            }
        }
        else if(wallCount == 3)
        {
            if (!up || !down)
                return Quaternion.Euler(0, 0, 90);
        }
        else if(wallCount == 2)
        {
            if (down && right)
            {
                checkCornerType(i, j);
                return Quaternion.Euler(0, 0, -90);
            }
            if (down && left)
            {
                checkCornerType(i, j);
                return Quaternion.Euler(0, 0, 180);
            }
            if (up && right)
            {
                checkCornerType(i, j);
                return Quaternion.identity;
            }
            if (up && left)
            {
                checkCornerType(i, j);
                return Quaternion.Euler(0, 0, 90);
            }
            if (left && right)
                return Quaternion.Euler(0, 0, 90);
        }
        else if(wallCount == 1)
        {
            if (left || right)
                return Quaternion.Euler(0, 0, 90);
        }
        return Quaternion.identity;
    }
    bool checkWallType(int i, int j)
    {
        bool safe = i >= 0 && j >= 0 && i < width && j < height;
        return safe && !isPath(i,j) && map[i, j] != (int)TileType.spawnEmpty && map[i, j] != (int)TileType.spawnOpening && map[i, j] != (int)TileType.emptyOutside;
    }
    void checkCornerType(int i, int j)
    {
        if (map[i, j] == (int)TileType.borderWall)
            map[i, j] = (int)TileType.outsideCorner;
        else
            map[i, j] = (int)TileType.insideCorner;
    }

    bool isPath(int i, int j)
    {
        return map[i, j] == (int)TileType.powerPellet || map[i, j] == (int)TileType.path;
    }
}
