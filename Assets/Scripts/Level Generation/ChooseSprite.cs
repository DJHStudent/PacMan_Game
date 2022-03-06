using UnityEngine;

public class ChooseSprite : MonoBehaviour
{
    public Sprite[] sprite; 
    public float[] probs;
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
        float totalProbs = 0;
        foreach (float item in probs)
        {
            totalProbs += item;
        }
        float randElement = Random.value * totalProbs;

        for (int i = 0; i < sprite.Length; i++)
        {
            if (randElement < probs[i])
                return sprite[i];
            else
                randElement -= probs[i];
        }
        return sprite[sprite.Length - 1]; 
    }
}
