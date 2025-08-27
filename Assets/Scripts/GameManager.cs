using UnityEngine;

public class GameManager : MonoBehaviour
{
    // instance 멤버변수는 private하게 선언
    private static GameManager instance = null;
    public static Case caseData = null;

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
            // instance가, GameManager가 존재한다면 GameObject 제거 
            Destroy(this.gameObject);
        }
    }

    // Public 프로퍼티로 선언해서 외부에서 private 멤버변수에 접근만 가능하게 구현
    public static GameManager Instance
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

    public static void setCase(Case newCase)
    {
        if (instance != null)
            caseData = newCase;
        Debug.Log("setCase");
        Debug.Log(caseData.getCrimeScene());
    }
}