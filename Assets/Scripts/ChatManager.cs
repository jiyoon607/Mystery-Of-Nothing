using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatManager : MonoBehaviour
{
    // instance 멤버변수는 private하게 선언
    private static ChatManager instance = null;
    public AIChat aiChat;
    public InputField inputField;
    public Text outputField;
    public Button sendButton;
    public GameObject endingPanel;

    private async void Start()
    {
        // AIChat이 먼저 초기화됐는지 확인 필요
        aiChat = GetComponent<AIChat>();
        if (aiChat != null)
        {
            string aiText = await aiChat.getCaseInfoFromAI();
            outputField.text = aiText;
        }

        sendButton.onClick.AddListener(OnSend);
        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Public 프로퍼티로 선언해서 외부에서 private 멤버변수에 접근만 가능하게 구현
    public static ChatManager Instance
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

    // 버튼 클릭 시 호출
    void OnSend()
    {
        ProcessInput(inputField.text);
    }

    // 엔터 누르면 호출됨
    void OnEndEdit(string text)
    {
        // 엔터키 눌렀을 때만 처리 (포커스 해제 등은 무시)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ProcessInput(text);
        }
    }

    async Task ProcessInput(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
        {
            Debug.Log("Text empty.");
            return;
        }

        aiChat.setUserInput(inputText);
        outputField.text = outputField.text + "\n\n<color=yellow>" + inputText + "</color>\n";

        // 입력 처리 후 텍스트 초기화
        inputField.text = "";
        inputField.ActivateInputField(); // 다시 포커스 주기

        string aiText = await aiChat.getTextFromAI();

        outputField.text = outputField.text + "\n" + aiText;

        StartCoroutine(WaitFiveSeconds());

        if (aiText.EndsWith("THE END."))
            endingPanel.SetActive(true);
    }

    IEnumerator WaitFiveSeconds()
    {
        yield return new WaitForSeconds(5.0f);
    }
}
