using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PeartreeGames.Evt.Topiary.Editor
{
    [CustomPropertyDrawer(typeof(EnumReference))]
    public class TopiEnumReferenceDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var enumObjectProperty = property.FindPropertyRelative("enumObject");
            var enumValueProperty = property.FindPropertyRelative("enumValue");
            var value = property.serializedObject.FindProperty("evtT.value");

            var enumObjField = new PropertyField(enumObjectProperty);
            var dropdownField = new PopupField<string>("Value", new List<string>(), 0);

            enumObjField.RegisterCallback<ChangeEvent<Object>>(_ =>
            {
                var enumObject = enumObjectProperty.objectReferenceValue as EnumObject;
                if (enumObject == null) return;

                dropdownField.choices.Clear();
                dropdownField.choices.AddRange(enumObject.Values);
                dropdownField.value = enumValueProperty.stringValue;
                value.stringValue = enumValueProperty.stringValue;
            });

            dropdownField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                enumValueProperty.stringValue = evt.newValue;
                value.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();

                if (enumValueProperty.serializedObject.targetObject is { } targetObject)
                {
                    EditorUtility.SetDirty(targetObject);
                }
            });

            var elem = new VisualElement();
            elem.Add(enumObjField);
            elem.Add(dropdownField);

            return elem;
        }
    }
}