using UnityEngine;

namespace Assets.HeroEditor.Common.CharacterScripts
{
    /// <summary>
    /// Bow shooting behaviour (charge/release bow, create arrow).
    /// </summary>
    public class BowShooting : MonoBehaviour
    {
        public Character Character;
        public AnimationClip ClipCharge;

        /// <summary>
        /// Should be set outside (by input manager or AI).
        /// </summary>
        [HideInInspector] public bool ChargeButtonDown;
        [HideInInspector] public bool ChargeButtonUp;

        private float _chargeTime;

        public void Update()
        {
            if (ChargeButtonDown)
            {
                _chargeTime = Time.time;
                Character.Animator.SetInteger("Charge", 1);
            }

            if (ChargeButtonUp)
            {
                var charged = Time.time - _chargeTime > ClipCharge.length;

                Character.Animator.SetInteger("Charge", charged ? 2 : 3);

                if (charged)
                {
                    // TODO: Instantiate arrow
                }
            }
        }
    }
}