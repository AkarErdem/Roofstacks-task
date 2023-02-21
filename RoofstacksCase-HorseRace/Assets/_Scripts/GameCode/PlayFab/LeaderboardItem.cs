using TMPro;
using UnityEngine;

namespace GameCode.PlayFab
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _rank;
        [SerializeField] private TMP_Text _nickname;
        [SerializeField] private TMP_Text _score;

        public void SetTexts(string rank, string nickname, string score)
        {
            this._rank.SetText(rank);
            this._nickname.SetText(nickname);
            this._score.SetText(score);
        }
    }
}
