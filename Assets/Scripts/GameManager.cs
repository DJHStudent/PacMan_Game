using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //get most of the referneces other objects need to use and store them here
    public static PacStudentController pacStudentController;
    public static LevelUIManager levelUIManager;
    public static AudioManager audioManager;
    public static LevelGenerator levelGenerator;
    public static RandomMapGenerator randomMap;
    public static SaveManager saveManager;
    public static List<GhostController> ghost1 = new List<GhostController>();
    public static List<GhostController> ghost2 = new List<GhostController>();
    public static List<GhostController> ghost3 = new List<GhostController>();
    public static List<GhostController> ghost4 = new List<GhostController>();
    public static InnovationSettings innovationSettings;

    public static int activeScene;
    public LayerMask wall, innerWall;
    public enum ActiveScene { recreation = 1, innovation, loading = 4};
    void Awake()
    {
        activeScene = SceneManager.GetActiveScene().buildIndex;
        pacStudentController = GameObject.FindGameObjectWithTag("Player").GetComponent<PacStudentController>();
        levelUIManager = GetComponent<LevelUIManager>();
        audioManager = GetComponent<AudioManager>();
        if (activeScene == (int)ActiveScene.recreation)
        {
            levelGenerator = GetComponent<LevelGenerator>();
            pacStudentController.leftTeloPoint = -14;
            pacStudentController.rightTeloPoint = 13;
        }
        else if (activeScene == (int)ActiveScene.innovation || activeScene == (int)ActiveScene.loading)
        {
            innovationSettings = GameObject.Find("InnovSceneManager").GetComponent<InnovationSettings>();
            randomMap = GetComponent<RandomMapGenerator>();
            setMapSettings();
            randomMap.createGhosts();
            pacStudentController.setStartPos();

            pacStudentController.leftTeloPoint = 0;
            pacStudentController.rightTeloPoint = randomMap.width - 1;
        }
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        initializeGhosts();
        pacStudentController.initilize();
        audioManager.initialize();
        levelUIManager.initilize();
    }
    void initializeGhosts()
    {
        GameObject[] ghosts;
        ghosts = GameObject.FindGameObjectsWithTag("Ghost1");
        GhostController.ghostSpawnDelay = -0.5f;
        ghost1.Clear(); //remove any current objects which exist stored as a ghost
        ghost2.Clear();
        ghost3.Clear();
        ghost4.Clear();
        foreach (GameObject ghost in ghosts)
        {
            ghost1.Add(ghost.GetComponent<GhostController>());
        }
        ghosts = GameObject.FindGameObjectsWithTag("Ghost2");
        foreach (GameObject ghost in ghosts)
        {
            ghost2.Add(ghost.GetComponent<GhostController>());
        }
        ghosts = GameObject.FindGameObjectsWithTag("Ghost3");
        foreach (GameObject ghost in ghosts)
        {
            ghost3.Add(ghost.GetComponent<GhostController>());
        }
        ghosts = GameObject.FindGameObjectsWithTag("Ghost4");
        foreach (GameObject ghost in ghosts)
        {
            ghost4.Add(ghost.GetComponent<GhostController>());
        }
    }
    void setMapSettings()//for the innovation set the values to the users input
    {
        randomMap.width = innovationSettings.width;
        randomMap.height = innovationSettings.height;
        randomMap.ghost1Amount = innovationSettings.ghost1Amount;
        randomMap.ghost2Amount = innovationSettings.ghost2Amount;
        randomMap.ghost3Amount = innovationSettings.ghost3Amount;
        randomMap.ghost4Amount = innovationSettings.ghost4Amount;
        Destroy(innovationSettings.gameObject);
    }
}
