using UnityEngine;
using UnityEngine.UI;

public class CreateCaseManager : MonoBehaviour
{
    // instance 멤버변수는 private하게 선언
    private static CreateCaseManager instance = null;
    // InputField 참조 변수
    public InputField crimeScene;
    public InputField timeOfDiscovery;
    public InputField numberOfSuspects;
    public InputField victimIntroduction;

    private void Awake()
    {
        if (null == instance)
        {
            // 씬 시작될때 인스턴스 초기화, 씬을 넘어갈때도 유지되기위한 처리
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // instance가, CreateCaseManager가 존재한다면 CreateCaseManager 제거 
            Destroy(this.gameObject);
        }
    }

    // Public 프로퍼티로 선언해서 외부에서 private 멤버변수에 접근만 가능하게 구현
    public static CreateCaseManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    public void SaveInputData()
    {
        Case newCase = new Case();
        
        // InputField에서 값 가져오기
        newCase.setCrimeScene(crimeScene.text);
        newCase.setTimeOfDiscovery(timeOfDiscovery.text);
        newCase.setNumberOfSuspects(numberOfSuspects.text);
        newCase.setVictimIntroduction(victimIntroduction.text);

        GameManager.setCase(newCase);
    }
}
