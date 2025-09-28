namespace BlackboardSource
{
    using System.Reflection;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class ValueDrawerBase : PropertyDrawer
    {
        static int counter;
        protected bool isEvent;

        class StyleUpdater : UIElementsExt.Tracker
        {
            public ValueDrawerBase drawer;
            public Label label;
            public GroupBox group;
            public VisualElement circle;
            public VisualElement valueField;
            public int index;

            public override void Update()
            {
                Blackboard bb = targetProperty.serializedObject.targetObject as Blackboard;

                bool overridenByBlackboard = false;
                bool editableInCurrentContext = false;
                string t_editorAndRuntime = "Fully Editable, both in editor and runtime.";
                string t_runtime = "Only editable at Runtime";
                string t_editor = "Only editable in Editor";
                string t_overriden = overridenByBlackboard ? " ( Blackboard Overrides this value mode. )" : "";

                Mode bbMode = default;
                Mode valueMode = default;

                if (!drawer.isEvent)
                {
                    bbMode = (Mode)targetProperty.serializedObject.FindProperty("_serializedMode").enumValueIndex;
                    valueMode = (Mode)targetProperty.FindPropertyRelative("_serializedMode").enumValueIndex;

                    switch (bbMode)
                    {
                        case Mode.Any:
                            break;
                        case Mode.OnlyEditor:
                            valueMode = Mode.OnlyEditor;
                            overridenByBlackboard = true;
                            break;
                        case Mode.OnlyRuntime:
                            valueMode = Mode.OnlyRuntime;
                            overridenByBlackboard = true;
                            break;
                        default:
                            break;
                    }
                }

                label.style.unityFontStyleAndWeight = FontStyle.Bold;
                label.style.marginTop = 0;
                label.style.fontSize = 14;
                label.style.color = index % 2 == 0 ? new Color(1, 1, 1, 1) : new Color(0.9f, 0.9f, 0.9f, 1);

                group.style.flexDirection = FlexDirection.Row;
                group.style.borderTopColor = new StyleColor(new UnityEngine.Color(.4f, .4f, .4f, 1));
                group.style.borderTopWidth = 1f;
                group.style.paddingTop = 0;
                group.style.paddingBottom = 0;
                group.style.marginBottom = 0;
                group.style.marginTop = 0;
                group.style.backgroundColor = index % 2 == 0 ? new Color(0, 0, 0, 0.1f) : new Color();

                bool isPlaying = EditorApplication.isPlaying;
                Color circleColor = Color.white;

                if (drawer.isEvent)
                {
                    circleColor = Color.skyBlue;
                    circle.tooltip = "Event, disregards Mode and works only at Runtime, in runtime copies.";
                    editableInCurrentContext = false;
                }
                else
                {
                    switch (valueMode)
                    {
                        case Mode.Any:
                            circleColor = Color.green;
                            circle.tooltip = t_editorAndRuntime;
                            editableInCurrentContext = true;
                            break;
                        case Mode.OnlyEditor:
                            circleColor = Color.yellow;
                            circle.tooltip = t_editor + t_overriden;
                            if (!isPlaying && !bb.IsRuntime)
                                editableInCurrentContext = true;
                            break;
                        case Mode.OnlyRuntime:
                            circleColor = Color.orange;
                            circle.tooltip = t_runtime + t_overriden;
                            if (isPlaying && bb.IsRuntime)
                                editableInCurrentContext = true;
                            break;
                        default:
                            break;
                    }

                    valueField.SetEnabled(editableInCurrentContext);
                }


                var s = circle.style;
                s.borderTopColor = s.borderBottomColor = s.borderLeftColor = s.borderRightColor = circleColor;
                s.borderBottomWidth = s.borderTopWidth = s.borderLeftWidth = s.borderRightWidth = 2;
                s.borderTopLeftRadius = s.borderTopRightRadius = s.borderBottomLeftRadius = s.borderBottomRightRadius = 5;
                s.width = 10;
                s.height = 10;
                s.marginTop = s.marginRight = 4;

                s.backgroundColor = overridenByBlackboard ? circleColor : Color.clear;
            }
        }

        class BlackboardTracker : UIElementsExt.Tracker
        {
            public StyleUpdater styleUpdater;

            public override void Update()
            {
                base.Update();
                styleUpdater.Update();
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var group = new GroupBox(property.name);
            var label = group[0] as Label;

            var styleUpdater = new StyleUpdater() { name = nameof(StyleUpdater), drawer = this };
            styleUpdater.index = counter;
            counter++;
            group.Add(styleUpdater);

            styleUpdater.SetTarget(property);

            var blackboardTracker = new  BlackboardTracker() {  name = nameof(BlackboardTracker) };
            blackboardTracker.SetTarget(property.serializedObject.FindProperty("_serializedMode"));
            blackboardTracker.styleUpdater = styleUpdater;
            group.Add(blackboardTracker);

            var circle = new VisualElement();
            circle.name = "circle";
            group.Insert(0, circle);

            PropertyField valueField = null;

            if (isEvent)
            {
                var val = property.boxedValue;
                var type = val.GetType();
                Label typeLabel = new Label();
                typeLabel.style.color = Color.gray;
                group.Add(typeLabel);

                if (type.IsGenericType)
                {
                    var args = type.GetGenericArguments();
                    typeLabel.text = args[0].Name;
                }
                else
                    typeLabel.text = "(No args)";
                Button copy = new Button();
                copy.text = "Copy Name";
                copy.clicked += () =>
                {
                    string eventName = property.name;
                    GUIUtility.systemCopyBuffer = eventName;
                    Debug.Log("Name " + eventName + " Copied to clipboard.");
                };
                group.Add(copy);
            }
            else
            {
                valueField = new PropertyField(property.FindPropertyRelative("value"));
                valueField.label = "";
                valueField.style.flexGrow = 1;
                //valueField.RegisterCallback<(x =>
                //{
                //    var propItself = property.boxedValue;
                //    var flags = BindingFlags.NonPublic | BindingFlags.Instance;
                //    propItself.GetType().GetMethod("EditorTrigger", flags).Invoke(propItself, new object[] { this });
                //});
                group.Add(valueField);
            }

            container.Add(group);

            styleUpdater.valueField = valueField;
            styleUpdater.group = group;
            styleUpdater.circle = circle;
            styleUpdater.label = label;

            styleUpdater.Update();

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(BlackboardValue<>))]
    public class ValueDrawer : ValueDrawerBase
    {

    }

    [CustomPropertyDrawer(typeof(BlackboardEvent<>))]
    [CustomPropertyDrawer(typeof(BlackboardEvent))]
    public class EventDrawer : ValueDrawerBase
    {
        public EventDrawer()
        {
            this.isEvent = true;
        }
    }
}