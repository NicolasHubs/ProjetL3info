using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// Makes character to look at cursor side (flip by X scale).
    /// </summary>
    /// 
    public class CharacterFlip : MonoBehaviour
    {
        public bool armed = false;
        public void Update()
        {
            if (!("Hub".Equals(SceneManager.GetActiveScene().name)))
            {
                transform.GetChild(0).localScale = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x ? 0.30f : -0.30f, 0.36f, 1);
                /*for (int i = 1; i < transform.childCount - 2; i++)
                    transform.GetChild(i).localScale = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x ? 1 : -1, 1, 1);
            }*/
            }
        }
    }
}