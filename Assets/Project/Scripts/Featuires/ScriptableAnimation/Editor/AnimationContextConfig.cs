#if UNITY_EDITOR
using UnityEngine;
#endif

#if UNITY_EDITOR

[CreateAssetMenu(fileName = "AnimationContextConfig", menuName = "Editor/Animation Context Config")]
public class AnimationContextConfig : ScriptableObject
{
    [Header("Colors")]
    [SerializeField] private Color _headerBgColor = new Color(0.12f, 0.12f, 0.12f, 0.5f);
    [SerializeField] private Color _backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0.3f);
    [SerializeField] private Color _headerTextColor = Color.white;
    [SerializeField] private Color _propertyTextColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    [Header("Layout")]
    [SerializeField] private float _indentPixels = 8f;
    [SerializeField] private FontStyle _headerFontStyle = FontStyle.Bold;

    public Color HeaderBgColor => _headerBgColor;
    public Color BackgroundColor => _backgroundColor;
    public Color HeaderTextColor => _headerTextColor;
    public Color PropertyTextColor => _propertyTextColor;
    public float IndentPixels => _indentPixels;
    public FontStyle HeaderFontStyle => _headerFontStyle;
}
#endif