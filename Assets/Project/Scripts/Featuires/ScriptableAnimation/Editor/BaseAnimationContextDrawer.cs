using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static AnimationLibrary;

[CustomPropertyDrawer(typeof(BaseAnimationContext))]
public class BaseAnimationContextDrawer : PropertyDrawer
{
    private static Type[] _derivedTypes;
    private static GUIContent[] _typeNames;

    static BaseAnimationContextDrawer()
    {
        RefreshTypeCache();
    }

    [InitializeOnLoadMethod]
    private static void RefreshTypeCache()
    {
        var typeCache = TypeCache.GetTypesDerivedFrom<BaseAnimationContext>();
        _derivedTypes = typeCache.Where(t => !t.IsAbstract && !t.IsGenericType).ToArray();
        _typeNames = _derivedTypes.Select(t => new GUIContent(t.Name)).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // ТЕМНЫЙ КВАДРАТНЫЙ ФОН
        Color originalColor = GUI.color;
        GUI.color = new Color(0.2f, 0.2f, 0.25f, 0.3f);
        EditorGUI.DrawRect(position, GUI.color);
        GUI.color = originalColor;

        float lineHeight = EditorGUIUtility.singleLineHeight;

        // 1. 3 ЭЛЕМЕНТА НА ОДНОЙ СТРОКЕ
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldNameWidth = position.width * 0.4f;
        float typeLabelWidth = 40f;  // "Type"
        float popupWidth = position.width * 0.5f - typeLabelWidth;

        // Field Name (слева)
        Rect fieldNameRect = new Rect(position.x + 4, position.y + 2, fieldNameWidth, lineHeight);
        EditorGUI.LabelField(fieldNameRect, label.text);

        // "Type" лейбл
        Rect typeLabelRect = new Rect(position.x + 4 + fieldNameWidth, position.y + 2, typeLabelWidth, lineHeight);
        EditorGUI.LabelField(typeLabelRect, "Type");

        // Dropdown (справа)
        Rect popupRect = new Rect(position.x + 4 + fieldNameWidth + typeLabelWidth, position.y + 2, popupWidth, lineHeight);
        int currentIndex = GetTypeIndex(property);
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(popupRect, currentIndex, _typeNames);
        if (EditorGUI.EndChangeCheck() && newIndex != currentIndex)
        {
            AssignType(property, _derivedTypes[newIndex]);
        }

        // 2. FOLDOUT С ПОЛЯМИ
        if (property.managedReferenceValue != null)
        {
            EditorGUI.indentLevel += 1;

            Rect foldoutRect = new Rect(position.x + 4, position.y + lineHeight + 4, position.width - 8, lineHeight);
            bool wasExpanded = SessionState.GetBool(GetPropertyKey(property), true);
            EditorGUI.BeginChangeCheck();
            bool expanded = EditorGUI.Foldout(foldoutRect, wasExpanded, "Animation Parameters", true);
            if (EditorGUI.EndChangeCheck())
            {
                SessionState.SetBool(GetPropertyKey(property), expanded);
            }

            if (expanded)
            {
                EditorGUI.indentLevel += 1;

                Rect fieldsArea = new Rect(position.x + 8, position.y + lineHeight * 2 + 8,
                                         position.width - 12, position.height - lineHeight * 2 - 12);

                float currentY = fieldsArea.y;
                SerializedProperty endProperty = property.GetEndProperty();
                SerializedProperty iterator = property.Copy();

                while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    if (IsDirectChild(property, iterator))
                    {
                        float fieldHeight = EditorGUI.GetPropertyHeight(iterator, true);
                        Rect fieldRect = new Rect(fieldsArea.x, currentY, fieldsArea.width, fieldHeight);

                        EditorGUI.PropertyField(fieldRect, iterator, true);
                        currentY += fieldHeight + 2;
                    }
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.indentLevel -= 1;
        }

        EditorGUI.EndProperty();
    }




    private string GetPropertyKey(SerializedProperty property)
    {
        return $"BaseAnimationContext_{property.propertyPath}";
    }

    private bool IsDirectChild(SerializedProperty parent, SerializedProperty child)
    {
        if (!child.propertyPath.StartsWith(parent.propertyPath))
            return false;

        string parentPath = parent.propertyPath;
        string childPath = child.propertyPath;

        int parentEnd = parentPath.Length;
        if (!childPath.StartsWith(parentPath))
            return false;

        string relativePath = childPath.Substring(parentEnd);
        return !relativePath.Contains(".") || relativePath.IndexOf('.') == relativePath.LastIndexOf('.');
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 2 + 6; // Тип + foldout + отступы

        if (property.managedReferenceValue != null)
        {
            bool isExpanded = SessionState.GetBool(GetPropertyKey(property), true);

            if (isExpanded)
            {
                SerializedProperty endProperty = property.GetEndProperty();
                SerializedProperty iterator = property.Copy();

                while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, endProperty))
                {
                    if (IsDirectChild(property, iterator))
                    {
                        height += EditorGUI.GetPropertyHeight(iterator, true) + 2;
                    }
                }
            }
        }

        return height;
    }

    private int GetTypeIndex(SerializedProperty property)
    {
        string currentType = property.managedReferenceFullTypename ?? "";
        for (int i = 0; i < _derivedTypes.Length; i++)
        {
            if (currentType.Contains(_derivedTypes[i].Name))
                return i;
        }
        return 0;
    }

    private static void AssignType(SerializedProperty property, Type type)
    {
        try
        {
            var instance = (BaseAnimationContext)Activator.CreateInstance(type);
            property.managedReferenceValue = instance;
            property.serializedObject.ApplyModifiedProperties();
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка создания {type.Name}: {e.Message}");
        }
    }
}
