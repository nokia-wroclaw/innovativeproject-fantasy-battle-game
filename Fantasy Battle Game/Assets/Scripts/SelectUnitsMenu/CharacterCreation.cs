using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelectUnitsMenu
{
    public class CharacterCreation : MonoBehaviour
    {
        public List<GameObject> Models;
        private List<GameObject> models_;
        private int selectionIndex_ = 0;
        private static CharacterCreation instance_;

        private void Start()
        {
            instance_ = this;
            models_ = new List<GameObject>();
            Models = new List<GameObject>();
            
            foreach (Transform singleTransform in transform)
            {
                instance_.models_.Add(singleTransform.gameObject);
                singleTransform.gameObject.SetActive(false);
            }

            models_[selectionIndex_].SetActive(true);
            NotDestroyObjects();
        }

        public void Select(int index)
        {
            if (index == selectionIndex_ || index < 0 || index >= models_.Count)
            {
                return;
            }

            models_[selectionIndex_].SetActive(false);
            selectionIndex_ = index;
            models_[selectionIndex_].SetActive(true);
        }

        public static CharacterCreation Instance
        {
            get { return instance_; }
            set { instance_ = value; }
        }

        public void SelectUnitsMenu()
        {
            Models.Add(models_[selectionIndex_]);
        }

        public void UnselectUnitsMenu()
        {
            Models.Remove(models_[selectionIndex_]);
        }

        public void NotDestroyObjects()
        {
            foreach (var model in models_)
            {
                Transform parentTransform = model.transform;
                while (parentTransform.parent != null)
                {
                    parentTransform = parentTransform.parent;
                }
                GameObject.DontDestroyOnLoad(parentTransform.gameObject);
            }
        }
    }
}
