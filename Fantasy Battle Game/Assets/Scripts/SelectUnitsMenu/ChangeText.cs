using UnityEngine;
using TMPro;

namespace SelectUnitsMenu
{
    public class ChangeText : MonoBehaviour
    {
        public TextMeshProUGUI textMeshH;
        private int counter_;

        public void changeText()
        {
            counter_++;
            if(counter_%2 == 1)
            {
                textMeshH.text = "SELECTED";
                CharacterCreation.Instance.SelectUnitsMenu();
            }
            else
            {
                textMeshH.text = "SELECT";
                CharacterCreation.Instance.UnselectUnitsMenu();
            }
        }
    }
}
