using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Minicop.Game.GravityRave
{
    public class Joystick : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float _moveRadius;
        private Vector3 _startPosition;
        private Vector2 _direction;
        [SerializeField]
        private Transform _transform;

        public UnityEvent<Vector2> OnMove = new UnityEvent<Vector2>();
        private void Start()
        {
            _startPosition = transform.position;
            _transform = transform;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _transform.position = new Vector3(eventData.position.x, eventData.position.y);
            _transform.position = Vector3.ClampMagnitude(transform.position - _startPosition, _moveRadius);
            _transform.position += _startPosition;

            _direction = new Vector2((_transform.position.x - _startPosition.x) / _moveRadius, (_transform.position.y - _startPosition.y) / _moveRadius);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _transform.position = _startPosition;
            _direction = Vector2.zero;
        }

        private void FixedUpdate()
        {
            OnMove.Invoke(_direction);
        }
    }
}