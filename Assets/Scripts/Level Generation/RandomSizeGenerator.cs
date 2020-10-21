using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class prevPos
{
    public Vector2 currPos, currDir;
    public prevPos(Vector2 currPos, Vector2 currDir)
    {
        this.currPos = currPos;
        this.currDir = currDir;
    }
}
//probably add case where after changed direction just ensure move in that direction first so get three wide gaps
//max 93 by 93 before too much
public class RandomSizeGenerator : MonoBehaviour
{
    public int width, height;
    public int[,] map;
    enum TileType { empty, path, wall, borderWall };
    enum CurrDirection { Up, Down, Left, Right };
    public GameObject[] mapComponents;
    Vector2 currPos, currDir, lastDir;

    List<prevPos> prevPos = new List<prevPos>();
    List<Vector2> joins = new List<Vector2>();

    void Start()
    {
        map = new int[width, height];
        generateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void generateMap()
    {
        makeBorder();
        instanciateMap();
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
        makePath();
    }
    void makePath()
    {
        if (prevPos.Count > 0)
            prevPos.RemoveAt(prevPos.Count - 1);
        else
        {
            map[1, height - 2] = (int)TileType.path;
            currPos = new Vector2(1, height - 2);
        }
        List<Vector2> nextPos = validDirections();

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
            if (posX - 3 > 0 && map[posX - 1, posY] == (int)TileType.empty && map[posX - 2, posY] == (int)TileType.empty && map[posX - 3, posY] == (int)TileType.path
                && map[posX - 1, posY + 1] == (int)TileType.empty && map[posX - 1, posY + 2] == (int)TileType.empty &&
                map[posX - 1, posY - 1] == (int)TileType.empty && map[posX - 1, posY - 2] == (int)TileType.empty
                && map[posX - 2, posY + 1] == (int)TileType.empty && map[posX - 2, posY + 2] == (int)TileType.empty &&
                map[posX - 2, posY - 1] == (int)TileType.empty && map[posX - 2, posY - 2] == (int)TileType.empty)
            {
                map[posX - 1, posY] = (int)TileType.path;
                map[posX - 2, posY] = (int)TileType.path;
            }

            //down
            if (posY - 3 > 0 && map[posX, posY - 1] == (int)TileType.empty && map[posX, posY - 2] == (int)TileType.empty && map[posX, posY - 3] == (int)TileType.path
                && map[posX + 1, posY - 1] == (int)TileType.empty && map[posX + 2, posY - 1] == (int)TileType.empty &&
                map[posX - 1, posY - 1] == (int)TileType.empty && map[posX - 2, posY - 1] == (int)TileType.empty
                && map[posX + 1, posY - 2] == (int)TileType.empty && map[posX + 2, posY - 2] == (int)TileType.empty &&
                map[posX - 1, posY - 2] == (int)TileType.empty && map[posX - 2, posY - 2] == (int)TileType.empty)

            {
                map[posX, posY - 1] = (int)TileType.path;
                map[posX, posY - 2] = (int)TileType.path;
            }

        }
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
               //// if (mapValue != 0)
                    Instantiate(mapComponents[mapValue], new Vector2(i, j), Quaternion.identity, this.transform);
            }
        }
    }
}
