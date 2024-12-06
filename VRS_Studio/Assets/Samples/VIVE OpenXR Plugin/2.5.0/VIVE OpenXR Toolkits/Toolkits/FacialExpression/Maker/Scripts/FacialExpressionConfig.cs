// Copyright HTC Corporation All Rights Reserved.
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
//using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VIVE.OpenXR.Toolkits.FacialExpression.Maker
{
    public class GraphChanges
    {
        public FacialExpressionSerializableEdge removedEdge;
        public FacialExpressionSerializableEdge addedEdge;
        public FacialTrackingNode removedNode;
        public FacialTrackingNode addedNode;
        public FacialTrackingNode nodeChanged;
    }



    [System.Serializable]
    public class FacialExpressionConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeReference]
        public List<FacialTrackingNode> nodes = new List<FacialTrackingNode>();

        [System.NonSerialized]
        public Dictionary<string, FacialTrackingNode> nodesPerGUID = new Dictionary<string, FacialTrackingNode>();

        [SerializeField]
        public List<FacialExpressionSerializableEdge> edges = new List<FacialExpressionSerializableEdge>();

        //[SerializeField, HideInInspector]
        //public List<SerializableEdgeData> edgesSaved = new List<SerializableEdgeData>();

        [System.NonSerialized]
        public Dictionary<string, FacialExpressionSerializableEdge> edgesPerGUID = new Dictionary<string, FacialExpressionSerializableEdge>();

        [NonSerialized]
        Scene linkedScene;

        // Trick to keep the node inspector alive during the editor session
        //[SerializeField]
        //internal UnityEngine.Object nodeInspectorReference;

        //graph visual properties
        public Vector3 position = Vector3.zero;
        public Vector3 scale = Vector3.one;

        /// Triggered when the graph is linked to an active scene.
        public event Action<Scene> onSceneLinked;

        /// Triggered when the graph is enabled
        public event Action onEnabled;

        /// Triggered when the graph is changed
        public event Action<GraphChanges> onGraphChanges;

        [System.NonSerialized]
        bool _isEnabled = false;
        public bool isEnabled { get => _isEnabled; private set => _isEnabled = value; }

        public HashSet<FacialTrackingNode> graphOutputs { get; private set; } = new HashSet<FacialTrackingNode>();

        private static bool isSceneLoaded = false;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void OnEditorStart()
        {
            isSceneLoaded = true;
            Debug.Log("Unity Editor Open Editor:" + isSceneLoaded+",active:"+SceneManager.GetActiveScene().isLoaded);
        }
#endif
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if (BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) { return; }
#endif

            Debug.Log("BaseGraph OnEnable() isEnable:" + isEnabled + ",SceneLoad:" + isSceneLoaded + ",active:" + SceneManager.GetActiveScene().isLoaded);
            if (!isSceneLoaded)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }
            if (isEnabled)
                OnDisable();

            //MigrateGraphIfNeeded();
            InitializeGraphElements();
            DestroyBrokenGraphElements();
            //UpdateComputeOrder();
            isEnabled = true;
            onEnabled?.Invoke();

            //Debug.Log("BaseGraph OnEnable() isEnable:" + isEnabled);
        }

        void InitializeGraphElements()
        {
            //Debug.Log("BaseGraph InitializeGraphElements()");
            // Sanitize the element lists (it's possible that nodes are null if their full class name have changed)
            // If you rename / change the assembly of a node or parameter, please use the MovedFrom() attribute to avoid breaking the graph.
            nodes.RemoveAll(n => n == null);
            //exposedParameters.RemoveAll(e => e == null);

            foreach (var node in nodes.ToList())
            {
                nodesPerGUID[node.GUID] = node;
                node.Initialize(this);
            }

            foreach (var edge in edges.ToList())
            {
                edge.Deserialize();
                edgesPerGUID[edge.GUID] = edge;

                // Sanity check for the edge:
                if (edge.inputPort == null || edge.outputPort == null)
                {
                    Disconnect(edge.GUID);
                    continue;
                }

                // Add the edge to the non-serialized port data
                edge.inputPort.owner.OnEdgeConnected(edge);
                edge.outputPort.owner.OnEdgeConnected(edge);
            }
        }

        protected virtual void OnDisable()
        {
            Debug.Log("BaseGraph OnDisable() isEnable:"+ isEnabled + ",SceneLoad:" + isSceneLoaded + ",active:" + SceneManager.GetActiveScene().isLoaded);
            isEnabled = false;
            foreach (var node in nodes)
                node.DisableInternal();
            isSceneLoaded = false;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"BaseGraph Scene '{scene.name}' has been loaded successfully.");
            //Debug.Log("BaseGraph OnSceneLoaded() isEnable:" + isEnabled + ",SceneLoad:" + isSceneLoaded+ ",active:" + SceneManager.GetActiveScene().isLoaded);
            isSceneLoaded = true;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnEnable();
        }
        public virtual void OnAssetDeleted() { /*Debug.Log("BaseGraph OnAssetDeleted()");*/ }

        /// <summary>
        /// Adds a node to the graph
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public FacialTrackingNode AddNode(FacialTrackingNode node)
        {
            //Debug.Log("BaseGraph AddNode()");
            nodesPerGUID[node.GUID] = node;

            nodes.Add(node);
            node.Initialize(this);

            onGraphChanges?.Invoke(new GraphChanges { addedNode = node });

            return node;
        }

        /// <summary>
        /// Removes a node from the graph
        /// </summary>
        /// <param name="node"></param>
        public void RemoveNode(FacialTrackingNode node)
        {
            //Debug.Log("BaseGraph RemoveNode()");
            node.DisableInternal();
            node.DestroyInternal();

            nodesPerGUID.Remove(node.GUID);

            nodes.Remove(node);

            onGraphChanges?.Invoke(new GraphChanges { removedNode = node });
        }

        /// <summary>
        /// Connect two ports with an edge
        /// </summary>
        /// <param name="inputPort">input port</param>
        /// <param name="outputPort">output port</param>
        /// <param name="DisconnectInputs">is the edge allowed to disconnect another edge</param>
        /// <returns>the connecting edge</returns>
        public FacialExpressionSerializableEdge Connect(FacialExpressionNodePort inputPort, FacialExpressionNodePort outputPort, bool autoDisconnectInputs = true)
        {
            //Debug.Log("Maker BaseGraph Connect() weight:"+outputPort.weightValue); //connect edge callback
            var edge = FacialExpressionSerializableEdge.CreateNewEdge(this, inputPort, outputPort);

            //If the input port does not support multi-connection, we remove them
            if (autoDisconnectInputs && !inputPort.portData.acceptMultipleEdges)
            {
                foreach (var e in inputPort.GetEdges().ToList())
                {
                    // TODO: do not disconnect them if the connected port is the same than the old connected
                    Disconnect(e);
                }
            }
            // same for the output port:
            if (autoDisconnectInputs && !outputPort.portData.acceptMultipleEdges)
            {
                foreach (var e in outputPort.GetEdges().ToList())
                {
                    // TODO: do not disconnect them if the connected port is the same than the old connected
                    Disconnect(e);
                }
            }

            edges.Add(edge);
	        //Debug.Log("Maker BaseGraph Connect() edges add to asset file");

            // Add the edge to the list of connected edges in the nodes
            inputPort.owner.OnEdgeConnected(edge);
            outputPort.owner.OnEdgeConnected(edge);

            onGraphChanges?.Invoke(new GraphChanges { addedEdge = edge });

            return edge;
        }

        /// <summary>
        /// Disconnect two ports
        /// </summary>
        /// <param name="inputNode">input node</param>
        /// <param name="inputFieldName">input field name</param>
        /// <param name="outputNode">output node</param>
        /// <param name="outputFieldName">output field name</param>
        public void Disconnect(FacialTrackingNode inputNode, string inputFieldName, FacialTrackingNode outputNode, string outputFieldName)
        {
            //Debug.Log("BaseGraph Disconnect() in:" + inputFieldName + ", out:" + outputFieldName);
            edges.RemoveAll(r => {
                bool remove = r.inputNode == inputNode
                && r.outputNode == outputNode
                && r.outputFieldName == outputFieldName
                && r.inputFieldName == inputFieldName;

                if (remove)
                {
                    r.inputNode?.OnEdgeDisconnected(r);
                    r.outputNode?.OnEdgeDisconnected(r);
                    onGraphChanges?.Invoke(new GraphChanges { removedEdge = r });
                }

                return remove;
            });
        }

        /// <summary>
        /// Disconnect an edge
        /// </summary>
        /// <param name="edge"></param>
        public void Disconnect(FacialExpressionSerializableEdge edge) => Disconnect(edge.GUID);

        /// <summary>
        /// Disconnect an edge
        /// </summary>
        /// <param name="edgeGUID"></param>
        public void Disconnect(string edgeGUID)
        {
            //Debug.Log("BaseGraph Disconnect() edgeGUID");
            List<(FacialTrackingNode, FacialExpressionSerializableEdge)> disconnectEvents = new List<(FacialTrackingNode, FacialExpressionSerializableEdge)>();

            edges.RemoveAll(r => {
                if (r.GUID == edgeGUID)
                {
                    disconnectEvents.Add((r.inputNode, r));
                    disconnectEvents.Add((r.outputNode, r));
                    onGraphChanges?.Invoke(new GraphChanges { removedEdge = r });
                }
                return r.GUID == edgeGUID;
            });

            // Delay the edge disconnect event to avoid recursion
            foreach (var (node, edge) in disconnectEvents)
                node?.OnEdgeDisconnected(edge);
        }

        /// <summary>
        /// Invoke the onGraphChanges event, can be used as trigger to execute the graph when the content of a node is changed 
        /// </summary>
        /// <param name="node"></param>
        public void NotifyNodeChanged(FacialTrackingNode node) => onGraphChanges?.Invoke(new GraphChanges { nodeChanged = node });



        public void OnBeforeSerialize()
        {
            //Debug.Log("BaseGraph OnBeforeSerialize()");//auto call in editor
            // Cleanup broken elements
            //stackNodes.RemoveAll(s => s == null);
            nodes.RemoveAll(n => n == null);
        }

        // We can deserialize data here because it's called in a unity context
        // so we can load objects references
        public void Deserialize()
        {
            //Debug.Log("BaseGraph Deserialize()");
            // Disable nodes correctly before removing them:
            if (nodes != null)
            {
                foreach (var node in nodes)
                    node.DisableInternal();
            }

            //MigrateGraphIfNeeded();

            InitializeGraphElements();
        }

        public void MigrateGraphIfNeeded()
        {
            //#pragma warning disable CS0618
            //			// Migration step from JSON serialized nodes to [SerializeReference]
            //			if (serializedNodes.Count > 0)
            //			{
            //				nodes.Clear();
            //				foreach (var serializedNode in serializedNodes.ToList())
            //				{
            //					var node = JsonSerializer.DeserializeNode(serializedNode) as BaseNode;
            //					if (node != null)
            //						nodes.Add(node);
            //				}
            //				serializedNodes.Clear();

            //				// we also migrate parameters here:
            //				var paramsToMigrate = serializedParameterList.ToList();
            //				exposedParameters.Clear();
            //				foreach (var param in paramsToMigrate)
            //				{
            //					if (param == null)
            //						continue;

            //					var newParam = param.Migrate();

            //					if (newParam == null)
            //					{
            //						Debug.LogError($"Can't migrate parameter of type {param.type}, please create an Exposed Parameter class that implements this type.");
            //						continue;
            //					}
            //					else
            //						exposedParameters.Add(newParam);
            //				}
            //			}
            //#pragma warning restore CS0618
        }

        public void OnAfterDeserialize() { }

        /// <summary>
        /// Link the current graph to the scene in parameter, allowing the graph to pick and serialize objects from the scene.
        /// </summary>
        /// <param name="scene">Target scene to link</param>
        public void LinkToScene(Scene scene)
        {
            //Debug.Log("BaseGraph LinkToScene()");
            linkedScene = scene;
            onSceneLinked?.Invoke(scene);
        }

        /// <summary>
        /// Return true when the graph is linked to a scene, false otherwise.
        /// </summary>
        public bool IsLinkedToScene() => linkedScene.IsValid();

        /// <summary>
        /// Get the linked scene. If there is no linked scene, it returns an invalid scene
        /// </summary>
        public Scene GetLinkedScene() => linkedScene;

        //HashSet<BaseNode> infiniteLoopTracker = new HashSet<BaseNode>();
        //int UpdateComputeOrderBreadthFirst(int depth, BaseNode node)
        //{
        //	int computeOrder = 0;

        //	if (depth > maxComputeOrderDepth)
        //	{
        //		Debug.LogError("Recursion error while updating compute order");
        //		return -1;
        //	}

        //	if (computeOrderDictionary.ContainsKey(node))
        //		return node.computeOrder;

        //	if (!infiniteLoopTracker.Add(node))
        //		return -1;

        //	if (!node.canProcess)
        //	{
        //		node.computeOrder = -1;
        //		computeOrderDictionary[node] = -1;
        //		return -1;
        //	}

        //	foreach (var dep in node.GetInputNodes())
        //	{
        //		int c = UpdateComputeOrderBreadthFirst(depth + 1, dep);

        //		if (c == -1)
        //		{
        //			computeOrder = -1;
        //			break ;
        //		}

        //		computeOrder += c;
        //	}

        //	if (computeOrder != -1)
        //		computeOrder++;

        //	node.computeOrder = computeOrder;
        //	computeOrderDictionary[node] = computeOrder;

        //	return computeOrder;
        //}

        //void UpdateComputeOrderDepthFirst()
        //{
        //	Stack<BaseNode> dfs = new Stack<BaseNode>();

        //	GraphUtils.FindCyclesInGraph(this, (n) => {
        //		PropagateComputeOrder(n, loopComputeOrder);
        //	});

        //	int computeOrder = 0;
        //	foreach (var node in GraphUtils.DepthFirstSort(this))
        //	{
        //		if (node.computeOrder == loopComputeOrder)
        //			continue;
        //		if (!node.canProcess)
        //			node.computeOrder = -1;
        //		else
        //			node.computeOrder = computeOrder++;
        //	}
        //}

        //void PropagateComputeOrder(BaseNode node, int computeOrder)
        //{
        //	Stack<BaseNode> deps = new Stack<BaseNode>();
        //	HashSet<BaseNode> loop = new HashSet<BaseNode>();

        //	deps.Push(node);
        //	while (deps.Count > 0)
        //	{
        //		var n = deps.Pop();
        //		n.computeOrder = computeOrder;

        //		if (!loop.Add(n))
        //			continue;

        //		foreach (var dep in n.GetOutputNodes())
        //			deps.Push(dep);
        //	}
        //}

        void DestroyBrokenGraphElements()
        {
            //if (edges.Count > 0)
            //{
            //    edgesSaved.Clear();
            //    foreach (var e in edges) { edgesSaved.Add(SerializableEdgeData.SerializableEdgeDataNew(e, this)); Debug.Log("BaseGraph DestroyBrokenGraphElements() add"); }
            //    Debug.Log("BaseGraph DestroyBrokenGraphElements() edgesSaved**:" + edgesSaved.Count + " /Original:" + edges.Count);
            //}

            edges.RemoveAll(e => e.inputNode == null
                || e.outputNode == null
                || string.IsNullOrEmpty(e.outputFieldName)
                || string.IsNullOrEmpty(e.inputFieldName)
            );
            nodes.RemoveAll(n => n == null);

        }

        /// <summary>
        /// Tell if two types can be connected in the context of a graph
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool TypesAreConnectable(Type t1, Type t2)
        {
            //Debug.Log("BaseGraph TypesAreConnectable()");

            if (t1 == null || t2 == null)
            {
                Debug.Log("BaseGraph TypesAreConnectable() either node is null");
                return false;
            }
            //if (TypeAdapter.AreIncompatible(t1, t2))
            //	return false;

            //Check if there is custom adapters for this assignation
            //if (CustomPortIO.IsAssignable(t1, t2))
            //	return true;

            //Check for type assignability
            //if (t2.IsReallyAssignableFrom(t1))
                return true;

            // User defined type convertions
            //if (TypeAdapter.AreAssignable(t1, t2))
            //	return true;

            return false;
        }
    }
}