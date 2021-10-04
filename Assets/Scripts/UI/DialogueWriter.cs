using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class DialogueWriter : MonoBehaviour
{
    [SerializeField]
    float m_secondsPerChar = 0.2f;
    [SerializeField]
    Text underTextBox;

    Text textBox;

    private void Awake()
    {
        textBox = this.GetComponent<Text>();
    }
    
    public void ClearDialogue()
    {
        textBox.text = "";
        underTextBox.text = "";
    }

    public void StartNewDialogue(string dialogue)
    {
        StartCoroutine(RunDialogueStream(dialogue));
    }

    public IEnumerator RunDialogueStream(string dialogue)
    {
        textBox.text = "" + dialogue[0];
        underTextBox.text = "";

        int currentCharIndex = 0;
        int totalCharacters = dialogue.Length;

        while(currentCharIndex < totalCharacters)
        {
            yield return new WaitForSeconds(m_secondsPerChar);

            underTextBox.text += dialogue[currentCharIndex];
            if(currentCharIndex < totalCharacters - 1)
            {
                textBox.text += dialogue[currentCharIndex + 1];
            }
            currentCharIndex++;
        }
    }
}
