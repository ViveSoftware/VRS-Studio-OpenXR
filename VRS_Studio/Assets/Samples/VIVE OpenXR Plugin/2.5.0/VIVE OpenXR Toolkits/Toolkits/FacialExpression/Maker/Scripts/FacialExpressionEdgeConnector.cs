// Copyright HTC Corporation All Rights Reserved.
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using System;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using VIVE.OpenXR.Toolkits.FacialExpression.Maker.Editor;

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    public class EdgeView : Edge
    {
        public bool isConnected = false;

        public FacialExpressionSerializableEdge serializedEdge { get { return userData as FacialExpressionSerializableEdge; } }

        //readonly string				edgeStyle = "GraphProcessorStyles/EdgeView";

        protected FacialTrackingGraphView owner => ((input ?? output) as FacialExpressionPortView).owner.owner;

        public EdgeView() : base()
        {
            //styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void OnPortChanged(bool isInput)
        {
            base.OnPortChanged(isInput);
            UpdateEdgeSize();
        }

        public void UpdateEdgeSize()
        {
            if (input == null && output == null)
                return;

            PortData inputPortData = (input as FacialExpressionPortView)?.portData;
            PortData outputPortData = (output as FacialExpressionPortView)?.portData;

            for (int i = 1; i < 20; i++)
                RemoveFromClassList($"edge_{i}");
            int maxPortSize = Mathf.Max(inputPortData?.sizeInPixel ?? 0, outputPortData?.sizeInPixel ?? 0);
            if (maxPortSize > 0)
                AddToClassList($"edge_{Mathf.Max(1, maxPortSize - 6)}");
        }

        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            //base.OnCustomStyleResolved(styles);

            UpdateEdgeControl();
        }

        void OnMouseDown(MouseDownEvent e)
        {
            if (e.clickCount == 2)
            {
                // Empirical offset:
                var position = e.mousePosition;
                position += new Vector2(-10f, -28);
                Vector2 mousePos = owner.ChangeCoordinatesTo(owner.contentViewContainer, position);

                //owner.AddRelayNode(input as FacialExpressionPortView, output as FacialExpressionPortView, mousePos);
            }
        }
    }

    public class FacialExpressionEdgeConnector : EdgeConnector
	{
		protected FacialExpressionEdgeDragHelper dragHelper;
        Edge edgeCandidate;
        protected bool active;
        Vector2 mouseDownPosition;
        protected FacialTrackingGraphView graphView;

        internal const float k_ConnectionDistanceTreshold = 10f;

		public FacialExpressionEdgeConnector(IEdgeConnectorListener listener) : base()
		{
            //Debug.Log("BaseEdgeConnector BaseEdgeConnector()");
            graphView = (listener as FacialExpressionEdgeConnectorListener)?.graphView;
            active = false;
            InitEdgeConnector(listener);
        }

        protected virtual void InitEdgeConnector(IEdgeConnectorListener listener)
        {
            //Debug.Log("BaseEdgeConnector InitEdgeConnector()");
            dragHelper = new FacialExpressionEdgeDragHelper(listener);
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

		public override EdgeDragHelper edgeDragHelper => dragHelper;

        protected override void RegisterCallbacksOnTarget()
        {
            //Debug.Log("BaseEdgeConnector RegisterCallbacksOnTarget()");
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            //Debug.Log("BaseEdgeConnector UnregisterCallbacksFromTarget()");
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            //Debug.Log("BaseEdgeConnector OnMouseDown() begin");
            if (active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (!CanStartManipulation(e))
            {
                return;
            }

            var graphElement = target as Port;
            if (graphElement == null)
            {
                return;
            }

            mouseDownPosition = e.localMousePosition;

            edgeCandidate = graphView != null ? graphView.CreateEdgeView() : new EdgeView();
            edgeDragHelper.draggedPort = graphElement;
            edgeDragHelper.edgeCandidate = edgeCandidate;

            if (edgeDragHelper.HandleMouseDown(e))
            {
                active = true;
                target.CaptureMouse();

                e.StopPropagation();
            }
            else
            {
                edgeDragHelper.Reset();
                edgeCandidate = null;
            }
            //Debug.Log("BaseEdgeConnector OnMouseDown() end");
        }

        void OnCaptureOut(MouseCaptureOutEvent e)
        {
            //Debug.Log("BaseEdgeConnector OnCaptureOut()");
            active = false;
            if (edgeCandidate != null)
                Abort();
        }

        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            if (!active) return;

            edgeDragHelper.HandleMouseMove(e);
            edgeCandidate.candidatePosition = e.mousePosition;
            edgeCandidate.UpdateEdgeControl();
            e.StopPropagation();
            //Debug.Log("BaseEdgeConnector OnMouseMove()");
        }

        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!active || !CanStopManipulation(e))
                return;

            if (CanPerformConnection(e.localMousePosition))
                edgeDragHelper.HandleMouseUp(e);
            else
                Abort();

            active = false;
            edgeCandidate = null;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode != KeyCode.Escape || !active)
                return;

            Abort();

            active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        void Abort()
        {
            var graphView = target?.GetFirstAncestorOfType<GraphView>();
            graphView?.RemoveElement(edgeCandidate);

            edgeCandidate.input = null;
            edgeCandidate.output = null;
            edgeCandidate = null;

            edgeDragHelper.Reset();
        }

        bool CanPerformConnection(Vector2 mousePosition)
        {
            //Debug.Log("BaseEdgeConnector CanPerformConnection()");
            return Vector2.Distance(mouseDownPosition, mousePosition) > k_ConnectionDistanceTreshold;
        }
    }
}
#endif