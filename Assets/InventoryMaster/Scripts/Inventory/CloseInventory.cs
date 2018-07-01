using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

    public class CloseInventory : MonoBehaviour, IPointerDownHandler
    {

        Inventory inv;
        void Start()
        {
            inv = transform.parent.GetComponent<Inventory>();

        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inv.closeInventory();
                if (transform.parent.tag.Equals("CraftSystem"))
                    transform.parent.GetComponent<CraftSystem>().backToInventory();
                if (transform.parent.tag.Equals("MainInventory"))
                {
                    if (GameObject.FindGameObjectWithTag("PanelAccueil").transform.childCount > 0)
                        GameObject.FindGameObjectWithTag("PanelAccueil").transform.GetComponent<RecettePanelAffichage>().close();
                }

            }
        }
    }
