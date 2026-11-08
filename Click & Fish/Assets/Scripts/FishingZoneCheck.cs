using UnityEngine;
using UnityEngine.Events;

public class FishingZoneCheck : MonoBehaviour
{
   public UnityEvent OnGameOver;
   [SerializeField] private GameObject[] _fishingZones;
   
   int _finishCount;
   
   public void AddFinishCount()
   {
      _finishCount++;
      if (_finishCount >= _fishingZones.Length)
      {
         CheckScore();
      }
   }

   void CheckScore()
   {
      if (UpdateScore.Instance.SCORE < UpdateScore.Instance.SCORE_TO_REACH)
      {
         OnGameOver?.Invoke();
      }
   }
}
