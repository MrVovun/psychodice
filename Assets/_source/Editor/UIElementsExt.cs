using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIElementsExt
{
    public abstract class Tracker : VisualElement
    {
        public SerializedProperty targetProperty { get; private set; }
        public SerializedObject targetObject { get; private set; }

        public void SetTarget(SerializedProperty property)
        {
            this.targetProperty = property;
            this.TrackPropertyValue(property, OnPropertyChanged);
        }

        public void SetTarget(SerializedObject serializedObject)
        {
            this.targetObject = serializedObject;
            this.TrackSerializedObjectValue(serializedObject, OnSerializedObjectChanged);
        }

        public virtual void Update()
        {
        }

        public void OnPropertyChanged(SerializedProperty property)
        {
            Update();
        }

        public void OnSerializedObjectChanged(SerializedObject serializedObject)
        {
            Update();
        }
    }

    public class TemplateUtil
    {
        public TemplateContainer container { get; private set; }
        public VisualElement originalParent { get; private set; }
        public string targetElement { get; private set; }

        public TemplateUtil(VisualElement templateContainer, string targetElement, bool removeAfterCaching = true)
        {
            this.targetElement = targetElement;
            this.container = templateContainer as TemplateContainer;
            this.originalParent = container.parent;
            if (removeAfterCaching)
                this.container.parent.Remove(this.container);
        }

        public As CloneAndInsert<As>(VisualElement parent, int index = -1) where As : VisualElement
        {
            var newve = container.templateSource.Instantiate();
            if (index < 0 || index >= parent.childCount)
                index = parent.childCount;
            parent.Insert(index, newve);
            var target = newve.Q(targetElement);
            return target as As;
        }
    }

    public class ToggleableElement
    {
        public bool Active { get; private set; } = true;
        public bool Singleton { get; private set; }
        public VisualElement element { get; private set; }
        public VisualElement originalParent { get; private set; }

        public ToggleableElement(VisualElement element, bool singleton = true)
        {
            this.Singleton = singleton;
            this.element = element;
            this.originalParent = element.parent;
        }

        public void Toggle()
        {
            SetActive(!Active);
        }

        public void SetActive(bool active)
        {
            if (active == Active)
                return;

            if (active)
            {
                if (Singleton)
                {
                    var existing = originalParent.Q(element.name);
                    if (existing == null)
                        originalParent.Add(element);
                }
                Active = true;
            }
            else
            {
                if (Singleton)
                {
                    originalParent.Remove(element);
                }
                Active = false;
            }
        }
    }

    public static T As<T>(this VisualElement obj) where T : class
    {
        if (obj == null)
            return null;
        return obj as T;
    }

    public static void SetFlex1(this VisualElement elem)
    {
        elem.style.flexGrow = elem.style.flexShrink = 1;
    }
}
