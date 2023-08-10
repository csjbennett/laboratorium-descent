using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColorController : MonoBehaviour
{
    public SpriteRenderer[] bodySprites;
    public SpriteRenderer visorSprite;

    private Color bodyColor;
    private Color visorColor;

    // Start is called before the first frame update
    void Start()
    {
        // Get colors stored in PlayerPrefs
        bodyColor = GetBodyColor();
        visorColor = GetVisorColor();

        // Apply body color
        foreach (SpriteRenderer sprite in bodySprites)
            sprite.color = bodyColor;

        // Apply visor color
        visorSprite.color = visorColor;
    }

    // Get body color from playerprefs
    Color GetBodyColor()
    {
        float r = PlayerPrefs.GetFloat("Body Red", 0.9f);
        float g = PlayerPrefs.GetFloat("Body Green", 0.9f);
        float b = PlayerPrefs.GetFloat("Body Blue", 0.2f);
       
        return new Color(r, g, b, 1f);
    }

    // Get visor color from playerprefs
    Color GetVisorColor()
    {
        float r = PlayerPrefs.GetFloat("Visor Red", 0.9f);
        float g = PlayerPrefs.GetFloat("Visor Green", 0.9f);
        float b = PlayerPrefs.GetFloat("Visor Blue", 0.9f);
        float a = 0.85f;

        return new Color(r, g, b, a);
    }
}
