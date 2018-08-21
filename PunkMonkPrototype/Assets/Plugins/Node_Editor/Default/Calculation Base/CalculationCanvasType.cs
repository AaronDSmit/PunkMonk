using UnityEngine;

namespace NodeEditorFramework.Standard
{
	[NodeCanvasType("Calculation")]
	public class CalculationCanvasType : NodeCanvas
	{
        public float Output { get { return output; } }
        private float output = 0;

        private string OutputNodeID { get { return "outputNode"; } }
        private OutputNode outputNode = null;

        public override string canvasName { get { return "Calculation Canvas"; } }

		protected override void OnCreate () 
		{
			Traversal = new CanvasCalculator (this);
            outputNode = (OutputNode)Node.Create("outputNode", Vector2.zero);
        }

		public void OnEnable () 
		{
            // Register to other callbacks, f.E.:
            //NodeEditorCallbacks.OnDeleteNode += OnDeleteNode;
        }

		protected override void ValidateSelf ()
		{
			if (Traversal == null)
				Traversal = new CanvasCalculator (this);
            if (outputNode == null && (outputNode = nodes.Find((Node n) => n.GetID == OutputNodeID) as OutputNode) == null)
                outputNode = Node.Create(OutputNodeID, Vector2.zero) as OutputNode;
        }

        public override bool CanAddNode(string nodeID)
        {
            //Debug.Log ("Check can add node " + nodeID);
            //if (nodeID == OutputNodeID)
            //    return !nodes.Exists((Node n) => n.GetID == OutputNodeID);
            return true;
        }

        public void Calculate()
        {
            if (Traversal == null)
                Traversal = new CanvasCalculator(this);

            Traversal.TraverseAll();

            if (outputNode == null)
            {
                outputNode = nodes.Find((Node n) => n.GetID == OutputNodeID) as OutputNode;
            }

            output = outputNode.GetValue;
        }
    }
}
