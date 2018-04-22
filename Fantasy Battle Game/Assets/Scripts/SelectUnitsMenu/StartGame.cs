using System.Collections;
using System.Collections.Generic;
using Menu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SelectUnitsMenu
{
    public class StartGame : MonoBehaviour
    {
        public void PlayGame()
        {
            if (CharacterCreation.Instance.Models.Count==3)
            {
                GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadLevel(2);
            }
            else
            {
                Debug.LogError("Select 3 units");
            }
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
