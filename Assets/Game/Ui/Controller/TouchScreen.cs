using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Minicop.Game.GravityRave
{
    public class TouchScreen : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Vector2 _startPosition;
        private Vector3 _differenceTouch;
        private Vector2 _differencePositions;
        private bool _isTouched = false;
        private int _pointerId;
        private Transform _transform;

        public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();
        private void Start()
        {
            _transform = transform;
            _startPosition = transform.position;
        }

        //private void 
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isTouched) return;

            _isTouched = true;
            _pointerId = eventData.pointerId;
            _differenceTouch = _startPosition;
            _differencePositions = new Vector2(eventData.position.x - _startPosition.x, eventData.position.y - _startPosition.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) return;

            _transform.position = new Vector3(eventData.position.x - _differencePositions.x, eventData.position.y - _differencePositions.y);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) return;

            _transform.position = _startPosition;
            _isTouched = false;
        }

        private void Drag()
        {
            if (!_isTouched) return;

            OnMove.Invoke(_transform.position - _differenceTouch);
            _differenceTouch = transform.position;
        }


        private void FixedUpdate()
        {
            Drag();
        }
    }
}