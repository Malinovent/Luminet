using UnityEngine;

namespace Mali.Utils
{ 
    public class DestroyOverTime : MonoBehaviour
    {
        [SerializeField] private float deathTime = 1f;

        private void Awake()
        {
            Destroy(gameObject, deathTime);
        }

    }
}
