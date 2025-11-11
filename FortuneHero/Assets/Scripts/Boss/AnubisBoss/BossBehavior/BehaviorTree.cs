using TMPro;
using UnityEngine;

public abstract class BehaviorTree : MonoBehaviour
{
    protected Node root;
    public Node activeNode;
    //[SerializeField] TextMeshProUGUI activeNodeText;

    protected abstract void InitializeTree();


    void Start()
    {
        InitializeTree();
        EvaluateTree();
    }

    void Update()
    {
        if(activeNode != null)
        {
            activeNode.Tick(Time.deltaTime);
            //activeNodeText.SetText("Node : " + activeNode);
        }
    }

    public void EvaluateTree()
    {
        root.EvaluateAction();
    }
    public void Interrupt()
    {
        activeNode.Interrupt();     
        EvaluateTree();
    }
}
