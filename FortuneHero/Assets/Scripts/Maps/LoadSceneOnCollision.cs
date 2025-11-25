using UnityEngine;

public class LoadSceneOnCollision : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActiveAndEnabled)
        {
            LoadManager.Instance.Load(sceneToLoad);
            Destroy(gameObject);
        }
    }

}
