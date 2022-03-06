using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] levelSection;
    public GameObject mapSection;
    GameObject instance;
    public LayerMask wall;
    int currElement;
    public int pelletAmount = 0;
    //public static int[] emptyChanges;
    public GameObject wayPointStart;

    public int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,8,8,4,5,4,8,8,8,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {9,9,9,9,9,2,5,4,3,4,4,3,0,3},
        {9,9,9,9,9,2,5,4,4,0,0,0,0,0},
        {9,9,9,9,9,2,5,4,4,0,3,4,4,0},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };
    private void Awake() //generate the first quadrant of the map
    {
        GenerateMap();
        mapSection.transform.position = new Vector2(levelMap.GetLength(1) + 2, levelMap.GetLength(0) - 1);
        mapSection.transform.localScale = new Vector2(-1, -1);
    }
    void Start() //generate the other three quadrants of the map based on original quadrant
    {
        GenerateSections(-levelMap.GetLength(1) + 2, -levelMap.GetLength(0) + 2, 1, 1, true);
        GenerateSections(-levelMap.GetLength(1) + 2, levelMap.GetLength(0) - 1, 1, -1, true);
        GenerateSections(levelMap.GetLength(1) + 2, -levelMap.GetLength(0) + 2, -1, 1, false);
        mapSection.transform.parent.transform.rotation = Quaternion.Euler(0, 0, 90); //sets entire map rotation
        GameManager.levelUIManager.statsManager.determinePellets();
        Ghost4Waypoints.currDir = Vector2.right;
        wayPointStart.AddComponent<Ghost4Waypoints>();
        GameManager.levelUIManager.statsManager.initilize();
    }
    void GenerateSections(int xPos, int yPos, int xScale, int yScale, bool delete) //determine position and scale of the other section
    {
        GameObject section = Instantiate(mapSection, new Vector2(xPos, yPos), Quaternion.identity, mapSection.transform.parent.transform);
        section.transform.localScale = new Vector2(xScale, yScale);
        if(delete)
        {
            for(int i = section.transform.childCount - 1; i >= section.transform.childCount - 2; i--)
            {
                Destroy(section.transform.GetChild(i).gameObject); //delete the sections which overlapp
            }
        }
    }
    void GenerateMap() //generates a quadrant
    {
        for (int i = 0; i < levelMap.GetLength(0); i++)
        {
            for (int j = 0; j < levelMap.GetLength(1); j++)
            {
                currElement = levelMap[i, j];
                if (currElement != 0)
                {
                    instance = Instantiate(levelSection[currElement], new Vector2(i, j), Quaternion.identity, mapSection.transform);
                    if (currElement == 6 || currElement == 5)
                        pelletAmount++;
                    instance.transform.rotation = determineRote(i, j);
                    if (i == 13 && j == 0)
                        wayPointStart = instance;
                }
            }
        }
    }
   
    Quaternion determineRote(int i, int j) //determines objects rotation based on type of piece it is
    {
        switch (currElement)
        {
            case 1: return cornerRote(i, j);
            case 2: return wallRote(i, j);
            case 3: return cornerRote(i, j);
            case 4:  return wallRote(i, j);
            default: return Quaternion.identity;
        }
    }
    Quaternion wallRote(int i, int j) //determines rotation of the wall elements
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(i, j), -instance.transform.right, 1, wall);
        if (hit)
            if (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == instance.GetComponent<SpriteRenderer>().sprite)//checks if left side is another wall
                return hit.collider.transform.rotation * Quaternion.identity; //if hiting another wall set rotation to that ones
            else
                return Quaternion.Euler(0, 0, 90);//if hit map and not another wall must be horizontally rotated
        else
            return Quaternion.identity; //if no wall hit must be vertical
    }
    Quaternion cornerRote(int i, int j)//determine the rotation of a corner piece
    {
        //special cases where raycasts not most efficent way
        if (j == levelMap.GetLength(1) - 1 && i == 7)
            return Quaternion.Euler(0, 0, -90);
        else if (i == 9 && j == 8)
            return Quaternion.Euler(0, 0, 90);
        else if (i == 10 && j == 8)
            return Quaternion.Euler(0, 0, 0);
        //cases based on raycast findings
        else
        {
            //detect if another map piece is either to the left or below the current piece as during generation these only two pieces which matter for determing corner rotation
            RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(i, j), -instance.transform.right, 1, wall);
            RaycastHit2D downtHit = Physics2D.Raycast(new Vector2(i, j), -instance.transform.up, 1, wall);
            if (leftHit && downtHit)
                return Quaternion.Euler(0, 0, 180);
            else if (leftHit)
                return Quaternion.Euler(0, 0, 90);
            else if (downtHit)
                return Quaternion.Euler(0, 0, -90);
            else return Quaternion.identity;
        }
    }
}
