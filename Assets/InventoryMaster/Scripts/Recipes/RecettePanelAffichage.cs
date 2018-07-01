using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class RecettePanelAffichage : MonoBehaviour
    {
        // Use this for initialization
        public bool canBeActive;
        public bool scrollImageCreated;
        public int largeurPanel;

        public Scrollbar sc;

        private float oldWidth;
        void Start()
        {
            // On cache le panel accueillant le panel des recettes
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(500, 500, 0);
            canBeActive = true;
            scrollImageCreated = false;
            largeurPanel = 0;
        }

        public void affichage()
        {
            // Affichage du code accueillant le panel des recettes
            if (this.gameObject.GetComponent<CanvasGroup>().alpha == 0)
            {
                this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                this.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        public void close()
        {
            canBeActive = true;
            Destroy(GameObject.FindGameObjectWithTag("RecettePanel"));
            this.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            this.gameObject.transform.localPosition = new Vector3(900, 1200, 0);
        }
    }
