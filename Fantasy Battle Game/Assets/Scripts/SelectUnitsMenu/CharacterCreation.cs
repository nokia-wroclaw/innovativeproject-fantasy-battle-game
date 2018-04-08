using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectUnitsMenu
{
    public class CharacterCreation : MonoBehaviour
    {
        private List<GameObject> models_;
        private int selectionIndex_ = 0;

        private void Start()
        {
            models_ = new List<GameObject>();
            foreach (Transform singleTransform in transform)
            {
                models_.Add(singleTransform.gameObject);
                singleTransform.gameObject.SetActive(false);
            }

            models_[selectionIndex_].SetActive(true);
        }

        public void Select(int index)
        {
            if (index == selectionIndex_ || index < 0 || index >= models_.Count)
                return;

            models_[selectionIndex_].SetActive(false);
            selectionIndex_ = index;
            models_[selectionIndex_].SetActive(true);
        }
    }
}