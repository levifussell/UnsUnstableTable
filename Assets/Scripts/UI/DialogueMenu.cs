using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueMenu : MonoBehaviour
{
    string[] QUOTES =
    {
        "You will face the wrath of the Jung!",
        "Kowa-BOOM-ga! Ha Ha HA.",
        "That was just another petty soldier -- no bother.",
        "Un is h-un-gry.",
        "Yeeeeee-haw.",
        "Tasty, tasty Americans.",
        "Hey baby, come checkout my nuke stash.",
        "North, east, south, west, \nfind me another nuke to test.",
    };


    [SerializeField]
    KjuAnimation kjuAnimation = null;
    [SerializeField]
    DialogueWriter m_dialogue = null;

    RectTransform m_dialogueRect;
    Vector2 m_targetDialoguePosition;

    bool m_isActive = false;

    private void Awake()
    {
        m_dialogueRect = m_dialogue.GetComponent<RectTransform>();

        ProjectileDeathManager.Instance.onProjectileDeath += DecideBeginDialogue;
    }
    private void OnDisable()
    {
        EndDialogue();

        kjuAnimation.enabled = false;
        m_dialogue.enabled = false;
    }
    private void OnEnable()
    {
        kjuAnimation.enabled = true;
        m_dialogue.enabled = true;
    }
    private void Start()
    {
        EndDialogue();
    }
    private void Update()
    {
        m_dialogueRect.anchoredPosition += (m_targetDialoguePosition - m_dialogueRect.anchoredPosition) * 0.1f;
    }

    #region controls
    void DecideBeginDialogue(Vector3 force, Vector3 position)
    {
        if(!m_isActive)
        {
            StartCoroutine(RunDialogue());
        }
    }

    IEnumerator RunDialogue()
    {
        m_isActive = true;
        BeginDialogue();

        yield return new WaitForSeconds(0.5f);
        yield return m_dialogue.RunDialogueStream(QUOTES[Random.Range(0, QUOTES.Length)]);
        yield return new WaitForSeconds(2.0f);

        EndDialogue();
        m_isActive = false;
    }

    void BeginDialogue()
    {
        kjuAnimation.m_targetPosition = new Vector2(-89.0f, 87.0f);
        m_targetDialoguePosition = new Vector2(-293.0f, -128.0f);
        m_dialogue.ClearDialogue();
    }

    void EndDialogue()
    {
        kjuAnimation.m_targetPosition = new Vector2(-448.0f, 87.0f);
        m_targetDialoguePosition = new Vector2(-293.0f, 100.0f);
        m_dialogue.ClearDialogue();
    }
    #endregion

#if UNITY_EDITOR
    #region editor
    public static void DrawInspector(DialogueMenu menu)
    {
        if(Application.isPlaying)
        {
            if(GUILayout.Button("Try Random Dialogue"))
            {
                menu.DecideBeginDialogue(Vector3.zero, Vector3.zero);
            }
        }
    }
    #endregion
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(DialogueMenu))]
public class E_DialogueMenu : Editor
{
    DialogueMenu dialogueMenu;

    private void OnEnable()
    {
        dialogueMenu = (DialogueMenu)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DialogueMenu.DrawInspector(dialogueMenu);
    }
}
#endif
