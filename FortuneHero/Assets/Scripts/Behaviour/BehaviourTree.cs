using UnityEngine;

abstract public class BehaviourTree : MonoBehaviour
{
    protected Behaviour_Composite root;
    public Behaviour_Node activeNode;


    abstract public void InitializeTree();

    public void RunTree()
    {
        root.ExecuteAction(null);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeTree();
        RunTree();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeNode != null)
            activeNode.Tick(Time.deltaTime);
    }
}
