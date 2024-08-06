using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterController : MonoBehaviour
{
    [Header("Information")]
    public string letter;

    [Header("References")]
    public SpellingGameManager gm;

    private void Start()
    {
        gm = GameObject.Find("SpellingGameManager").GetComponent<SpellingGameManager>();
    }

    public void OnDestroy()
    {
        gm.currentLetters.Remove(letter);
    }
}
