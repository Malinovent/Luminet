using UnityEngine;

public class Raycasting : MonoBehaviour
{

    IRaycastable raycastable;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (raycastable != null)
            {
                raycastable.OnRaycastHit();
            }

            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            IRaycastable raycastable = hit.collider.GetComponent<IRaycastable>();

            if (raycastable != null && raycastable != this.raycastable)
            {
                this.raycastable?.OnRaycastExit();
                this.raycastable = raycastable;
                raycastable.OnRaycastEnter();
            }
        }
        else
        {
            this.raycastable?.OnRaycastExit();
            raycastable = null;
        }
    }
}

public interface IRaycastable
{    
    void OnRaycastEnter();
    void OnRaycastExit();
    void OnRaycastHit();
}
