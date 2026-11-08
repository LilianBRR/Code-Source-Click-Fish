using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.MeshOperations;

public class FishingCollection : MonoBehaviour
{

    [Header("Drop Rate")]
    [SerializeField] private float _commonDrop;
    [SerializeField] private float _rareDrop;
    [SerializeField] string[] _tags;
    public UnityEvent OnReward;

    public enum FISH
    {
        NONE = 0,
        COMMON_GREY = 1,
        COMMON_LITTLE = 2,
        COMMON_RED = 3,
        RARE_BLUE = 4,
        RARE_GREEN = 5,
        RARE_ORANGE = 6,
        LEGENDARY_BLUE = 7,
        LEGENDARY_SHARK = 8,
        LEGENDARY_CLOWN = 9
    }

    FISH _fish;
    void Start()
    {
        _commonDrop = 0.8f;
        _rareDrop = 0.95f;
    }

    public void Reward()
    {
        if (_fish != FISH.NONE)
        {
            Save(_fish);
            OnReward?.Invoke();
        }
        
    }

    public FISH GetFish()
    {
        float rng = Random.Range(0, 1f);
        Debug.Log(rng);
        if(rng <= _commonDrop)
        {
            //COMMON
            int randomFish = Random.Range(1, 4);
            switch (randomFish)
            {
                case 1:
                    _fish = FISH.COMMON_GREY;
                    break;
                case 2:
                    _fish = FISH.COMMON_LITTLE;
                    break;
                case 3:
                    _fish = FISH.COMMON_RED;
                    break;
                default:
                    _fish = FISH.NONE;
                    break;
            }
        }
        else if ( rng <= _rareDrop)
        {
            //RARE
            int randomFish = Random.Range(4, 7);
            switch (randomFish)
            {
                case 4:
                    _fish = FISH.RARE_BLUE;
                    break;
                case 5:
                    _fish = FISH.RARE_GREEN;
                    break;
                case 6:
                    _fish = FISH.RARE_ORANGE;
                    break;
                default:
                    _fish = FISH.NONE;
                    break;
            }
        }
        else
        {
            //LEGENDARY
            int randomFish = Random.Range(7,10);
            switch (randomFish)
            {
                case 7:
                    _fish = FISH.LEGENDARY_BLUE;
                    break;
                case 8:
                    _fish = FISH.LEGENDARY_SHARK;
                    break;
                case 9:
                    _fish = FISH.LEGENDARY_CLOWN;
                    break;
                default:
                    _fish = FISH.NONE;
                    break;
            }
        }

        return _fish;
    }

    void Save(FISH fish)
    {
        for(int i = 1; i <= 10; i++)
        {
            if(((int)fish) == i )
            {
                if (PlayerPrefs.GetInt(_tags[i]) == 1)
                    return;
                PlayerPrefs.SetInt(_tags[i], 1);
                return;
            }
        }
    }  
}
