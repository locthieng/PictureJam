using UnityEngine;

public class UILife : MonoBehaviour
{
    [SerializeField] private UIDigitalClock lifeClock;
    LifeSystem lifeSystem => LifeSystem.Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        lifeClock.OnEnd = OnLifeClockEnd;
    }    

    // Update is called once per frame
    void Update()
    {
        
    }

    // nếu hết giờ 
    private void OnLifeClockEnd()
    {
        LifeSystem.Instance.ConsumeLife();
    }    
}
