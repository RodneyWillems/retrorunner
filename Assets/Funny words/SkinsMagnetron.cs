using UnityEngine;

public class SkinsMagnetron : MonoBehaviour
{
    #region Magnetron
    public static SkinsMagnetron Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    #region Skins
    [Header("Skin name")]
    // Keeping the skin in the magnetron so I can change it between scenes
    public string Skin;

    [Header("Skins")]
    [Header("Default")]
    public Sprite[] m_defaultSkin;
    public Sprite m_defaultFall;
    public Sprite m_defaultJump;

    [Header("Blue")]
    public Sprite[] m_blueSkin;
    public Sprite m_blueFall;
    public Sprite m_blueJump;

    [Header("Green")]
    public Sprite[] m_greenSkin;
    public Sprite m_greenFall;
    public Sprite m_greenJump;

    [Header("Yellow")]
    public Sprite[] m_yellowSkin;
    public Sprite m_yellowFall;
    public Sprite m_yellowJump;

    [Header("Red")]
    public Sprite[] m_redSkin;
    public Sprite m_redFall;
    public Sprite m_redJump;

    [Header("White")]
    public Sprite[] m_whiteSkin;
    public Sprite m_whiteFall;
    public Sprite m_whiteJump;

    public Sprite[] ReturnSprite()
    {
        return Skin switch
        {
            "Blue" => m_blueSkin,
            "Green" => m_greenSkin,
            "Yellow" => m_yellowSkin,
            "Red" => m_redSkin,
            "White" => m_whiteSkin,
            _ => m_defaultSkin,
        };
    }

    public Sprite ReturnJumpSprite() 
    {
        return Skin switch
        {
            "Blue" => m_blueJump,
            "Green" => m_greenJump,
            "Yellow" => m_yellowJump,
            "Red" => m_redJump,
            "White" => m_whiteJump,
            _ => m_defaultJump,
        };
    }

    public Sprite ReturnFallSprite()
    {
        return Skin switch
        {
            "Blue" => m_blueFall,
            "Green" => m_greenFall,
            "Yellow" => m_yellowFall,
            "Red" => m_redFall,
            "White" => m_whiteFall,
            _ => m_defaultFall,
        };
    }

    #endregion
}
