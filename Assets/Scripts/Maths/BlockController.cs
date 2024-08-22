using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [Header("References")]
    public MathsGameManager gm;

    private void Start()
    {
        gm = GameObject.Find("MathsGameManager").GetComponent<MathsGameManager>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        gm.BlockLanded();

        Destroy(this);
    }
}
