using UnityEngine;
using System.Collections;
using Champions;
using Map;

namespace BattleManagement {
    public class AnimationController : MonoBehaviour
    {
        public static AnimationController Instance;
        private float move = 20;
        private bool stop = false;
        private float blend;
        private float delay = 0;
        public float AddRunSpeed = 1;
        public float AddWalkSpeed = 1;
        private bool hasAniComp = false;

        public AnimationController()
        {
            if (Instance == null)
            {
                Instance = this;
            }

        }

        public void MoveAnimation(Champion selectedChampion)
        {
            selectedChampion.GetComponent<Animation>().Play("walk");
            Debug.Log("tt");
        }
        public void DeadAnimation(Champion selectedChampion)
        {
            selectedChampion.GetComponent<Animation>().Play("dead");
        }
        public void DamageAnimation(Champion selectedChampion)
        {
            selectedChampion.GetComponent<Animation>().CrossFade("damage", 0.2f);
        }
        public void RestAnimation(Champion selectedChampion)
        {
            //if (CheckAniClip("idle01") == false) return;
            if (selectedChampion.GetComponent<Animation>().isPlaying)
                selectedChampion.GetComponent<Animation>().Play("idle01");
            else
                selectedChampion.GetComponent<Animation>().Play("idle01");
        }
        public void AttackAnimation(Champion selectedChampion)
        {
            //if (CheckAniClip("attack01") == false) return;
            Debug.Log(selectedChampion);
            if (selectedChampion.GetComponent<Animation>().isPlaying)
            {
                selectedChampion.GetComponent<Animation>().CrossFade("attack01", 0.2f);
                selectedChampion.GetComponent<Animation>().CrossFadeQueued("idle01");
            }
        }
       /* bool CheckAniClip(string clipname)
        {
            if (this.GetComponent<Animation>().GetClip(clipname) == null)
                return false;
            else if (this.GetComponent<Animation>().GetClip(clipname) != null)
                return true;

            return false;
        }
        */
    }
}