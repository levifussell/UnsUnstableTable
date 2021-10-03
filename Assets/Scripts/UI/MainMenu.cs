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

        m_levelState = GameObject.Instantiate(mainMenuLevel);
    }

    private void OnDisable()
    {
        //playerController.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        Destroy(m_levelState);
        ProjectileDeathManager.Instance.DestroyAllProjectiles();

        // spawn new level.
        GameObject.Instantiate(mainMenuLevel);
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
