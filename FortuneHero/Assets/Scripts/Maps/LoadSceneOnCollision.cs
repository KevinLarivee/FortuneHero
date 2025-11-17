using UnityEngine;

public class LoadSceneOnCollision : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadManager.Instance.Load(sceneToLoad);
        }
    }

}
