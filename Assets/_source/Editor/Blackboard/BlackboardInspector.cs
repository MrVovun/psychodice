using BlackboardSource;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Blackboard), true)]
public class BlackboardInspector : Editor
{
    public VisualTreeAsset m_InspectorUXML;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new();
        bool isRuntime = serializedObject.FindProperty("isRuntime").boolValue;
        Color color = isRuntime ? Color.orange  : Color.gray5;
        var s = root.style;
        s.borderTopColor = s.borderBottomColor = s.borderLeftColor = s.borderRightColor = color;
        s.borderBottomWidth = s.borderTopWidth = s.borderLeftWidth = s.borderRightWidth = 2;
        s.borderTopLeftRadius = s.borderTopRightRadius = s.borderBottomLeftRadius = s.borderBottomRightRadius = 5;
        s.paddingLeft = 3;
        s.paddingRight = 6;

        var stateLabel = new Label();
        stateLabel.text = isRuntime ? "Runtime" : "Editor";
        stateLabel.style.color = color;
        stateLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        stateLabel.SetFlex1();

        var group = new GroupBox();

        bool isPlaying = EditorApplication.isPlaying;

        var selectEditorButton = new Button();
        selectEditorButton.text = "Select Editor";
        selectEditorButton.SetEnabled(isPlaying && isRuntime);
        selectEditorButton.SetFlex1();
        selectEditorButton.clicked += () =>
        {
            Blackboard target = serializedObject.targetObject as Blackboard;

            if (!isPlaying)
                return;

            if(target.IsRuntime && target.originalBlackboard)
            {
                Selection.activeObject = target.originalBlackboard;
            }
        };

        var selectRuntimeButton = new Button();
        selectRuntimeButton.text = "Select Runtime";
        selectRuntimeButton.SetEnabled(isPlaying && !isRuntime);
        selectRuntimeButton.SetFlex1();
        selectRuntimeButton.clicked += () =>
        {
            Blackboard target = serializedObject.targetObject as Blackboard;

            if (!isPlaying)
                return;

            if(target && !target.IsRuntime)
            {
                var manager = GameObject.FindFirstObjectByType<BlackboardManager>();
                var bb = manager.GetBlackboard(target.name);
                if(bb)
                    Selection.activeObject = bb;
            }
        };

        group.style.flexDirection = FlexDirection.Row;
        group.Add(stateLabel);
        group.Add(selectEditorButton);
        group.Add(selectRuntimeButton);

        var mode = new PropertyField(serializedObject.FindProperty("_serializedMode"));
        mode.SetEnabled(!isPlaying);
        mode.SetFlex1();
        group.Add(mode);

        root.Add(group);
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        return root;
    }
}