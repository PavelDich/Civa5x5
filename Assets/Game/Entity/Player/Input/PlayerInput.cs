using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    [DisallowMultipleComponent]
    public class PlayerInput : MonoBehaviour
    {
        [Header("Settings")]
        public InputSettings InputCode;
        [field: SerializeField]
        public bool isMenuOpen { get; set; }
        [field: SerializeField]
        public MenuEvents menuEvents { get; private set; }
        [System.Serializable]
        public struct MenuEvents
        {
            [field: SerializeField]
            public UnityEvent OnEscape { get; private set; }
            [field: SerializeField]
            public UnityEvent OnMenuOpen { get; private set; }
            [field: SerializeField]
            public UnityEvent OnMenuClose { get; private set; }
        }
        [field: SerializeField]
        public MoveEvents moveEvents { get; private set; }
        [System.Serializable]
        public struct MoveEvents
        {
            [field: SerializeField]
            public UnityEvent OnStay { get; private set; }
            [field: SerializeField]
            public UnityEvent<Vector2> OnWalk { get; private set; }
            [field: SerializeField]
            public UnityEvent<Vector2> OnSprint { get; private set; }
            [field: SerializeField]
            public UnityEvent<Vector2> OnCrawl { get; private set; }
            [field: SerializeField]
            public UnityEvent<Vector2> OnRotate { get; private set; }
            [field: SerializeField]
            public UnityEvent OnJump { get; private set; }
        }

        [field: SerializeField]
        public InventoryEvents inventoryEvents { get; private set; }
        [System.Serializable]
        public struct InventoryEvents
        {
            public UnityEvent<int> OnSwith { get; private set; }
            [field: SerializeField]
            public UnityEvent OnGrab { get; private set; }
            [field: SerializeField]
            public UnityEvent OnPut { get; private set; }
            [field: SerializeField]
            public UnityEvent OnDrop { get; private set; }
        }
        [field: SerializeField]
        public ItemEvents itemEvents { get; private set; }
        [System.Serializable]
        public struct ItemEvents
        {
            [field: SerializeField]
            public UnityEvent OnClick { get; private set; }
            [field: SerializeField]
            public UnityEvent OnAltClick { get; private set; }
            [field: SerializeField]
            public UnityEvent OnReload { get; private set; }
        }
        [Header("Other")]
        public UnityEvent OnInteract;


        private void Start()
        {
            isMenuOpen = false;
            menuEvents.OnMenuClose.Invoke();
            //Cursor.lockState = CursorLockMode.Locked;
        }
        private void FixedUpdate()
        {
            if ((Input.GetKey(InputCode.move.Forward) || Input.GetKey(InputCode.move.Back) ||
             Input.GetKey(InputCode.move.Left) || Input.GetKey(InputCode.move.Right)) && !isMenuOpen)
            {
                Vector2 direction = Vector2.zero;
                if (Input.GetKey(InputCode.move.Forward)) direction += Vector2.up;
                if (Input.GetKey(InputCode.move.Back)) direction += Vector2.down;
                if (Input.GetKey(InputCode.move.Left)) direction += Vector2.left;
                if (Input.GetKey(InputCode.move.Right)) direction += Vector2.right;

                if (Input.GetKey(InputCode.move.Sprint)) moveEvents.OnSprint.Invoke(new Vector2(direction.x, direction.y));
                else if (Input.GetKeyUp(InputCode.move.Crawl)) moveEvents.OnCrawl.Invoke(new Vector2(direction.x, direction.y));
                else moveEvents.OnWalk.Invoke(new Vector2(direction.x, direction.y));

            }
            else moveEvents.OnStay.Invoke();
        }
        public void SwithcMenu()
        {
            isMenuOpen = !isMenuOpen;
            if (isMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                menuEvents.OnMenuOpen.Invoke();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                //Cursor.lockState = CursorLockMode.Locked;
                menuEvents.OnMenuClose.Invoke();
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(InputCode.menu.Open))
            {
                SwithcMenu();
            }

            if (isMenuOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    menuEvents.OnEscape.Invoke();
                }
                return;
            }

            moveEvents.OnRotate.Invoke(new Vector2(Input.GetAxis("Mouse X") * InputCode.move.SensativityX, Input.GetAxis("Mouse Y") * InputCode.move.SensativityY));
            if (Input.GetKeyDown(InputCode.move.Jump))
                moveEvents.OnJump.Invoke();

            if (Input.GetKeyDown(InputCode.Inventory.Grab))
                inventoryEvents.OnGrab.Invoke();
            if (Input.GetKeyDown(InputCode.Inventory.Put))
                inventoryEvents.OnPut.Invoke();
            if (Input.GetKeyDown(InputCode.Inventory.Drop))
                inventoryEvents.OnDrop.Invoke();

            if (Input.GetKeyDown(InputCode.item.Click))
                itemEvents.OnClick.Invoke();
            if (Input.GetKeyDown(InputCode.item.AltClick))
                itemEvents.OnAltClick.Invoke();
            if (Input.GetKeyDown(InputCode.item.Reload))
                itemEvents.OnReload.Invoke();

            if (Input.GetKeyDown(InputCode.Interact))
                OnInteract.Invoke();
        }
    }
}
