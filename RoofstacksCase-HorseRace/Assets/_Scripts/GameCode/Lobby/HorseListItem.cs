using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCode.Lobby
{
    public class HorseListItem : MonoBehaviour
    {
        [SerializeField] private Image _horseIcon;
        [SerializeField] private Image _horseBackground;
        [SerializeField] private TMP_Text _horseName;
        [SerializeField] private Button _selectButton;
        
        public Image HorseIcon => _horseIcon;
        public Image HorseBackground => _horseBackground;
        public TMP_Text HorseName => _horseName;
        public Button SelectButton => _selectButton;
    }
}
