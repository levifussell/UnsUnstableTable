using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        //RectTransform r = playButton.GetComponent<RectTransform>();
        //r.localScale *= 2.0f;
        //r.sizeDelta = new Vector2(10.0f, 100.0f);
        playButton.onClick.AddListener(OnPlayClick);
        playerController.enabled = false;

        transitionMenu.gameObject.SetActive(true);
        hudMenu.gameObject.SetActive(false);

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
        transitionMenu.onTransitionEnd += StartLevel;
        transitionMenu.Transition();
    }

    void StartLevel()
    {
        playerController.enabled = true;
        this.gameObject.SetActive(false);

        // close current level.

        //Destroy(m_levelState);
        ProjectileDeathManager.Instance.DestroyAllProjectiles();

        // spawn new level.
        //GameObject.Instantiate(mainMenuLevel);
        GameObject level = GameObject.Instantiate(levelFormations[hubController.currentLevelIndex]);
        level.transform.position = hubController.currentLevelOrigin;

        dialogueMenu.enabled = true;

        hudMenu.gameObject.SetActive(true);
        hudMenu.SetupNewLevelHud(level);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //RectTransform r = playButton.GetComponent<RectTransform>();
        //r.localScale = new Vector3(2.0f, 0.0f);
        //r.sizeDelta = new Vector2(10.0f, 100.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //RectTransform r = playButton.GetComponent<RectTransform>();
        //r.localScale = new Vector3(0.5f, 0.0f);
        //r.sizeDelta = new Vector2(10.0f, 300.0f);
    }

    #region controls
    #endregion
}
