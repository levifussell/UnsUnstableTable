using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    #region parameters
    [SerializeField]
    Button playButton = null;
    [SerializeField]
    PlayerController playerController = null;
    [SerializeField]
    GameObject mainMenuLevel = null;
    [SerializeField]
    TransitionMenu transitionMenu = null;
    [SerializeField]
    DialogueMenu dialogueMenu = null;
    [SerializeField]
    HudMenu hudMenu = null;
    [SerializeField]
    RetryMenu retryMenu = null;
    [SerializeField]
    KjuAnimation kjuAnimation = null;

    [Header("Hub References")]
    [SerializeField]
    HubController hubController = null;
    [SerializeField]
    Button nextLevelButton = null;
    [SerializeField]
    Button previousLevelButton = null;
    [SerializeField]
    Text levelText = null;

    [Header("Level Prefabs")]
    [SerializeField]
    List<GameObject> levelFormations = new List<GameObject>();
    #endregion

    #region variables
    GameObject m_levelState = null;
    #endregion

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayClick);
        playerController.enabled = false;

        transitionMenu.gameObject.SetActive(true);
        hudMenu.gameObject.SetActive(false);
        hudMenu.onGameOver += OnGameOver;

        retryMenu.gameObject.SetActive(false);
        retryMenu.enabled = false;
        retryMenu.menuButton.onClick.AddListener(OnMenuClick);

        //m_levelState = GameObject.Instantiate(mainMenuLevel);

        nextLevelButton.onClick.AddListener(hubController.NextLevel);
        previousLevelButton.onClick.AddListener(hubController.PreviousLevel);

        if (levelFormations.Count != hubController.levelCount)
            Debug.LogError(string.Format("There are {0} levels and only {1} formations given!", hubController.levelCount, levelFormations.Count));
    }

    private void OnDisable()
    {
        //playerController.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogueMenu.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        levelText.text = string.Format("level {0}", hubController.currentLevelIndex + 1);
    }

    void OnPlayClick()
    {
        playButton.interactable = false;
        retryMenu.retryButton.interactable = false;
        transitionMenu.onTransitionEnd += StartLevel;
        transitionMenu.Transition();
    }

    void OnPlayNextLevelClick()
    {
        hubController.NextLevel();
        OnPlayClick();
    }

    void OnMenuClick()
    {
        retryMenu.retryButton.interactable = false;
        retryMenu.menuButton.interactable = false;
        transitionMenu.onTransitionEnd += StartMenu;
        transitionMenu.Transition();
    }

    void StartLevel()
    {
        playerController.enabled = true;
        playerController.LifStick();
        this.gameObject.SetActive(false);
        retryMenu.gameObject.SetActive(false);

        // close current level.

        //Destroy(m_levelState);
        ProjectileDeathManager.Instance.DestroyAllProjectiles();

        // spawn new level.
        //GameObject.Instantiate(mainMenuLevel);
        if (m_levelState != null)
        {
            foreach(FormationSpawner sp in m_levelState.GetComponentsInChildren<FormationSpawner>())
            {
                sp.UnspawnAndDestroyAllSoldiers();
            }
        }
        m_levelState = GameObject.Instantiate(levelFormations[hubController.currentLevelIndex]);
        m_levelState.SetActive(true);
        m_levelState.transform.position = hubController.currentLevelOrigin;

        dialogueMenu.enabled = true;

        hudMenu.gameObject.SetActive(true);
        hudMenu.SetupNewLevelHud(m_levelState);

        // setup the enemy AI.
        if(hubController.currentLevelIndex != 0)
        {
            FormationSpawner[] formations = m_levelState.GetComponentsInChildren<FormationSpawner>();
            EnemyAI.Instance.formationSpawnerRebel = formations[0];
            EnemyAI.Instance.formationSpawnerEnemy = formations[1];
        }
    }

    void StartMenu()
    {
        kjuAnimation.Reset();

        playerController.enabled = false;
        playButton.interactable = true;
        playButton.GetComponent<PlayButton>().Reset();
        this.gameObject.SetActive(true);
        retryMenu.gameObject.SetActive(false);

        ProjectileDeathManager.Instance.DestroyAllProjectiles();

        if (m_levelState != null)
            Destroy(m_levelState);

        dialogueMenu.enabled = false;

        hudMenu.gameObject.SetActive(false);
    }


    void OnGameOver(bool playerWins)
    {
        playerController.LifStick();
        playerController.enabled = false;
        retryMenu.retryButton.interactable = true;
        retryMenu.menuButton.interactable = true;
        dialogueMenu.enabled = false;

        ProjectileDeathManager.Instance.DestroyAllProjectiles();

        retryMenu.gameObject.SetActive(true);
        retryMenu.enabled = true;
        retryMenu.NewGameOver(playerWins);

        retryMenu.retryButton.onClick.RemoveListener(OnPlayClick);
        retryMenu.retryButton.onClick.RemoveListener(OnPlayNextLevelClick);
        if(playerWins)
            retryMenu.retryButton.onClick.AddListener(OnPlayNextLevelClick);
        else
            retryMenu.retryButton.onClick.AddListener(OnPlayClick);
    }

}
