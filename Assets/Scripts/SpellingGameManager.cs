using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellingGameManager : MonoBehaviour
{
    [Header("Information")]
    public GameObject[] letters;
    public int[] letterWeight;
    public int weightAdjustment;
    public string targetWord;
    public List<string> targetWordList = new();

    [Header("GamePlay")]
    public bool isPlaying;
    public float letterDelay;
    public float letterFallSpeed;

    [Header("References")]
    public GameObject targetWordParent;
    public GameObject letterHolder;

    private void Start()
    {
        GetWordList();
        NewWord();
        DisplayTargetWord();
        StartCoroutine(SpawnLetterLoop());
    }

    private void Update()
    {

    }

    public void GetWordList()
    {
        //gets words from a word doc and adds them to the targetWordList array
    }

    public void NewWord()
    {
        int i = Random.Range(0, targetWordList.Count);
        targetWord = targetWordList[i];
    }

    public void DisplayTargetWord()
    {
        int wordLength = targetWord.Length;

        //make the position for letters right

        foreach (char letter in targetWord)
        {
            //Debug.Log(letter);
            Instantiate(letters[LetterToNumber(letter.ToString())], targetWordParent.transform.position, Quaternion.identity, targetWordParent.transform);
        }
    }

    public IEnumerator SpawnLetterLoop()
    {
        while (true)
        {
            while (isPlaying)
            {
                SpawnLetter();
                yield return new WaitForSeconds(letterDelay);
            }
        }
    }

    public void SpawnLetter()
    {
        int weightTotal = 0;

        for (int i = 0; i < letters.Length; i++)
        {
            foreach (char l in targetWord)
            {
                if (LetterToNumber(l.ToString()) == i)
                {
                    weightTotal += weightAdjustment;
                }
            }
            weightTotal += letterWeight[i];
        }

        int chosenLetter = -1;
        int letterInt = Random.Range(0,weightTotal);
        int weightProgress = 0;

        for (int i = 0; i < letters.Length; i++)
        {
            foreach (char l in targetWord)
            {
                if (LetterToNumber(l.ToString()) == i)
                {
                    weightProgress += weightAdjustment;
                }
            }
            weightProgress += letterWeight[i];

            if (weightProgress >= letterInt)
            {
                chosenLetter = i;
                break;
            }
        }

        //determine position of new letter

        //get either side of letter holder
        float leftPos = letterHolder.GetComponent<Collider2D>().bounds.max.x;
        float rightPos = letterHolder.GetComponent<Collider2D>().bounds.min.x;
        float vertPos = letterHolder.transform.position.y;
        float horPos = Random.Range(leftPos, rightPos);

        Vector3 newPos = new Vector3(horPos, vertPos, 0);

        GameObject newLetter = Instantiate(letters[chosenLetter], newPos, Quaternion.identity, letterHolder.transform);
        newLetter.GetComponent<Rigidbody2D>().velocity = new Vector3(newLetter.GetComponent<Rigidbody2D>().velocity.x, letterFallSpeed);

        //spawns letters as children in the letter holder object
        //determine which letter to spawn
        //temporarily adjust the weight of the letters in the word so it's less likely useless letters show up
        //spawn letter
        //letters fall (of their own accord)
    }

    public int LetterToNumber(string letter)
    {
        letter = letter.ToLower();

        switch (letter)
        {
            default:
                return -1;
            case "a":
                return 0;
            case "b":
                return 1;
            case "c":
                return 2;
            case "d":
                return 3;
            case "e":
                return 4;
            case "f":
                return 5;
            case "g":
                return 6;
            case "h":
                return 7;
            case "i":
                return 8;
            case "j":
                return 9;
            case "k":
                return 10;
            case "l":
                return 11;
            case "m":
                return 12;
            case "n":
                return 13;
            case "o":
                return 14;
            case "p":
                return 15;
            case "q":
                return 16;
            case "r":
                return 17;
            case "s":
                return 18;
            case "t":
                return 19;
            case "u":
                return 20;
            case "v":
                return 21;
            case "w":
                return 22;
            case "x":
                return 23;
            case "y":
                return 24;
            case "z":
                return 25;
        }
    }
}
