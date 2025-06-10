using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    public class PanelOpener : MonoBehaviour
    {
        public GameObject[] Panels;
        public int StartPanelId;
        private void Start()
        {
            Open(StartPanelId);
        }

        public void Open(int id)
        {
            foreach (var panel in Panels)
            {
                panel.gameObject.SetActive(false);
            }
            Panels[id].SetActive(true);
        }
    }
}