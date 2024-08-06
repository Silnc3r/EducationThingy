using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public List<string> currentLetters = new(); // stores the current on screen letters

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
        float wordLength = targetWord.Length;

        //make the position for letters right
        wordLength *= letters[0].GetComponent<Renderer>().bounds.size.x;
        Vector3 newPos = new Vector3(targetWordParent.transform.position.x - wordLength/2 + 0.5f, targetWordParent.transform.position.y, targetWordParent.transform.position.z);

        foreach (char letter in targetWord)
        {
            Instantiate(letters[LetterToNumber(letter.ToString())], newPos, Quaternion.identity, targetWordParent.transform);
            newPos = new Vector3(newPos.x + letters[0].GetComponent<Renderer>().bounds.size.x, newPos.y, 0);
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
        currentLetters.Add(NumberToLetter(chosenLetter));
        newLetter.GetComponent<LetterController>().letter = NumberToLetter(chosenLetter);

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
    public string NumberToLetter(int number)
    {
        switch (number)
        {
            default:
                return "?";
            case 0:
                return "a";
            case 1:
                return "b";
            case 2:
                return "c";
            case 3:
                return "d";
            case 4:
                return "e";
            case 5:
                return "f";
            case 6:
                return "g";
            case 7:
                return "h";
            case 8:
                return "i";
            case 9:
                return "j";
            case 10:
                return "k";
            case 11:
                return "l";
            case 12:
                return "m";
            case 13:
                return "n";
            case 14:
                return "o";
            case 15:
                return "p";
            case 16:
                return "q";
            case 17:
                return "r";
            case 18:
                return "s";
            case 19:
                return "t";
            case 20:
                return "u";
            case 21:
                return "v";
            case 22:
                return "w";
            case 23:
                return "x";
            case 24:
                return "y";
            case 25:
                return "z";
        }
    }
}
