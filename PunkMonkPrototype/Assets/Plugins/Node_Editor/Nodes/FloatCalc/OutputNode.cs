using UnityEngine;

namespace NodeEditorFramework.Standard
{
    [Node(false, "Float/Output")]
    public class OutputNode : Node
    {
        public const string ID = "outputNode";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Output Node"; } }
        public override Vector2 DefaultSize { get { return new Vector2(150, 50); } }

        public float GetValue { get { return value; } }

        private float value = 0;

        [ValueConnectionKnob("Value", Direction.In, "Float")]
        public ValueConnectionKnob inputKnob;

        public override void NodeGUI()
        {
            inputKnob.DisplayLayout(new GUIContent("Value : " + value.ToString(), "The input value to output"));
        }

        public override bool Calculate()
        {
            value = inputKnob.GetValue<float>();
            return true;
        }
    }
}
