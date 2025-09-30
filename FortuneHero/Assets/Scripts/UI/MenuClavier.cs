using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuClavier : MonoBehaviour
{
    [SerializeField] private Button[] boutons;
    private int index = 0;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(boutons[0].gameObject);
    }
    public void OnNavigate(InputAction.CallbackContext context)
    {
        //Vector2 dir = context.ReadValue<Vector2>();
        //if (dir.y > 0) // flèche haut
        //{
        //    index = (index - 1 + boutons.Length) % boutons.Length;
        //    boutons[index].Select();
        //}
        //else if (dir.y < 0) // flèche bas
        //{
        //    index = (index + 1) % boutons.Length;
        //    boutons[index].Select();
        //}
        //Debug.Log(index);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            boutons[index].onClick.Invoke();
        }
    }

    public void OnButtonHover(int newIndex)
    {
        index = newIndex;
        boutons[index].Select();
    }
}
