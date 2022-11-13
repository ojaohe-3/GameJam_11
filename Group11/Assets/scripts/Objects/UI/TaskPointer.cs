using System;
using UnityEngine;

namespace Objects.UI
{
    public class TaskPointer : MonoBehaviour
    {
        public GameObject target;
        private RectTransform _transform;
        [SerializeField] private float _float_radius = 100f;

        public void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void FixedUpdate()
        {
            if (target == null)
            {
                gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            var current = Camera.main.transform.position;
            var dir = (target.transform.position - current).normalized;
            float angle =  (float)Math.Acos(dir.magnitude);
            _transform.localEulerAngles = new Vector3(0, 0, angle);

        }
        
    }
}