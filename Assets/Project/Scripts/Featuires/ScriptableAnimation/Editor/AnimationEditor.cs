#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using LitMotion;
using static AnimationLibrary;
using System;

[CustomPropertyDrawer(typeof(IAnimationContext), true)]
public class AnimationContextDrawer : PropertyDrawer
{
    private const float INDENT_PIXELS = 8f;

    private static readonly Color HEADER_BG_COLOR = new Color(0.12f, 0.12f, 0.12f, 0.5f);
    private static readonly Color BACKGROUND_COLOR = new Color(0.15f, 0.15f, 0.15f, 0.3f);
    private static readonly Color HEADER_TEXT_COLOR = Color.white;
    private static readonly Color PROPERTY_TEXT_COLOR = new Color(0.8f, 0.8f, 0.8f, 1f);

    private static GUIStyle _headerLabelStyle;
    private static GUIStyle HeaderLabelStyle
    {
        get
        {
            if (_headerLabelStyle == null)
            {
                _headerLabelStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = HEADER_TEXT_COLOR },
                    onNormal = { textColor = HEADER_TEXT_COLOR },
                    hover = { textColor = HEADER_TEXT_COLOR },
                    onHover = { textColor = HEADER_TEXT_COLOR },
                    focused = { textColor = HEADER_TEXT_COLOR },
                    onFocused = { textColor = HEADER_TEXT_COLOR },
                    active = { textColor = HEADER_TEXT_COLOR },
                    onActive = { textColor = HEADER_TEXT_COLOR }
                };
            }
            return _headerLabelStyle;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        DrawBackground(position);
        DrawPropertyHeader(position, property, label);

        if (property.isExpanded)
        {
            DrawExpandedContent(position, property);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            totalHeight += CalculateExpandedHeight(property);
        }

        return totalHeight;
    }

    private void DrawBackground(Rect position)
    {
        EditorGUI.DrawRect(position, BACKGROUND_COLOR);
    }

    private void DrawHeaderBackground(Rect rect)
    {
        EditorGUI.DrawRect(rect, HEADER_BG_COLOR);
    }

    private void DrawPropertyHeader(Rect position, SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect foldoutRect = new Rect(position.x, position.y, position.width, lineHeight);
        Rect headerBgRect = new Rect(position.x, position.y, position.width, lineHeight);

        var originalFoldoutColor = HeaderLabelStyle.normal.textColor;
        HeaderLabelStyle.normal.textColor = HEADER_TEXT_COLOR;

        DrawHeaderBackground(headerBgRect);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label.text, true, HeaderLabelStyle);

        HeaderLabelStyle.normal.textColor = originalFoldoutColor;
    }

    private void DrawExpandedContent(Rect position, SerializedProperty property)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float currentY = position.y + lineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.indentLevel++;
        float indentOffset = EditorGUI.indentLevel * INDENT_PIXELS;

        DrawAllFields(ref currentY, position.x + indentOffset, position.width - indentOffset, property);

        EditorGUI.indentLevel--;
    }

    private void DrawAllFields(ref float currentY, float x, float width, SerializedProperty parentProperty)
    {
        SerializedProperty endProperty = parentProperty.GetEndProperty();
        SerializedProperty childProperty = parentProperty.Copy();

        if (!childProperty.NextVisible(true)) return;

        do
        {
            if (SerializedProperty.EqualContents(childProperty, endProperty))
                break;

            DrawSingleField(ref currentY, x, width, childProperty);

        } while (childProperty.NextVisible(false));
    }

    private void DrawSingleField(ref float currentY, float x, float width, SerializedProperty property)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, true);
        Rect propertyRect = new Rect(x, currentY, width, propertyHeight);

        var originalLabelColor = EditorStyles.label.normal.textColor;
        EditorStyles.label.normal.textColor = PROPERTY_TEXT_COLOR;

        EditorGUI.PropertyField(propertyRect, property, true);

        EditorStyles.label.normal.textColor = originalLabelColor;

        currentY += propertyHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    private float CalculateExpandedHeight(SerializedProperty property)
    {
        float totalHeight = 0f;
        SerializedProperty endProperty = property.GetEndProperty();
        SerializedProperty childProperty = property.Copy();

        if (!childProperty.NextVisible(true)) return totalHeight;

        do
        {
            if (SerializedProperty.EqualContents(childProperty, endProperty))
                break;

            totalHeight += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;

        } while (childProperty.NextVisible(false));

        return totalHeight;
    }
}

#endif