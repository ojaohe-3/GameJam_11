using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Objects.Tasks.BatteryTask
{
    public class Battery : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform _rectTransform;
        public Action<string> TaskComplete { get; set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("pointer down");
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Dragging");
            _rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("EndDrag");

        }
    }
}
