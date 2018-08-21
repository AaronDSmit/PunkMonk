using UnityEngine;
using NodeEditorFramework.Utilities;

namespace NodeEditorFramework.Standard
{
	[System.Serializable]
	[Node(false, "Float/Input")]
	public class InputNode : Node
	{
		public const string ID = "inputNode";
		public override string GetID { get { return ID; } }

		public override string Title { get { return "Input Node"; } }
		public override Vector2 DefaultSize { get { return new Vector2(200, 100); } }

        public float Value { set { this.value = value; } }

		[ValueConnectionKnob("Value", Direction.Out, "Float")]
		public ValueConnectionKnob outputKnob;

        public InputType inputType = 0;

        public PlayerValue playerValue = 0;

        public AgentValue agentValue = 0;

        public WorldValue worldValue = 0;

        private float value = 0f;

        public override void NodeGUI()
		{
            // value = RTEditorGUI.FloatField(new GUIContent("Value", "The input value of type float"), value);
            inputType = (InputType)RTEditorGUI.EnumPopup(new GUIContent("Input type:", ""), inputType);
            RTEditorGUI.Space();

            switch (inputType)
            {
                case InputType.player:
                    playerValue = (PlayerValue)RTEditorGUI.EnumPopup(new GUIContent("Players value:", ""), playerValue);
                    break;
                case InputType.agent:
                    agentValue = (AgentValue)RTEditorGUI.EnumPopup(new GUIContent("This Agents value:", ""), agentValue);
                    break;
                case InputType.world:
                    worldValue = (WorldValue)RTEditorGUI.EnumPopup(new GUIContent("The Worlds value:", ""), worldValue);
                    break;
                default:
                    break;
            }

            outputKnob.SetPosition();

			if (GUI.changed)
				NodeEditor.curNodeCanvas.OnNodeChange(this);
		}

		public override bool Calculate()
		{
            outputKnob.SetValue(value);

			return true;
		}
	}
}

public enum InputType { player, agent, world }
public enum PlayerValue { health, maxHealth }
public enum AgentValue { damage, movement }
public enum WorldValue { movesToAttackRange }