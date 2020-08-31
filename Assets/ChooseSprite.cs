using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseSprite : MonoBehaviour
{
    public Sprite[] sprite;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = determineSprite();
        transform.rotation = determineRote();
    }

    Quaternion determineRote()
    {
        int randElement = Random.Range(0, 4);
        switch (randElement)
        {
            case 1: return Quaternion.Euler(0, 0, 90);
            case 2: return Quaternion.Euler(0, 0, 180);
            case 3: return Quaternion.Euler(0, 0, -90);
            default: return Quaternion.identity;
        }
    }
    Sprite determineSprite()
    {
        int randElement = Random.Range(0, 3);
        switch (randElement)
        {
            case 1: return sprite[1];
            case 2: return sprite[2];
            default: return sprite[0];
        }
    }
}
