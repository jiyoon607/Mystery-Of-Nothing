using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class AIChat : MonoBehaviour
{
    private const string API_KEY = "gpt_api_key";
    private const string MODEL = "gpt-4o-mini";
    // English
    // private const string outputFormat = "\n\nUse <b> tags instead of **. Make your answers clear and concise.";

    // Korean
    private const string outputFormat = "\n\n** 대신 <b>태그를 사용하세요. 간결하고 명확하게 답변하세요.";
    private const int MAX_RETRIES = 3;
    private const int RETRY_DELAY = 5; // seconds

    private static readonly HttpClient client = new HttpClient();

    private string userInput = null;

    private string caseInformation;
    private string lastText;

    private void Start()
    {
        caseInformation = GameManager.caseData.getCaseInformationToString();
    }

    public async Task<string> getCaseInfoFromAI()
    {
        string generatedText = null;
        // English
        // string startPrompt = "Based on the following information, describe a murder case and begin a mystery game. Provide the player with choices to make." + caseInformation;
        // Korean
        string startPrompt = "다음 정보를 기반으로, 살인 사건을 구상하고 추리 게임을 시작하세요. 플레이어에게 추리를 위한 선택지를 제공하세요." + caseInformation;

        for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
        {
            try
            {
                var requestBody = new
                {
                    model = MODEL,
                    messages = new[]
                    {
                        // English
                        // new { role = "system", content = "You are the host of a mystery game. Create and describe a murder case, and provide the situation and clues to help the player successfully identify the suspect." },
                        // Korean
                        new { role = "system", content = "당신은 추리 게임의 사회자입니다. 한국어로 사건을 서술하고, 플레이어에게 한국어로 선택지를 제공합니다. ** 대신 <b>태그를 사용합니다. 간결하게 답변합니다." },
                        new { role = "user", content = startPrompt }
                    },
                    temperature = 0.3
                };

                var requestJson = JsonConvert.SerializeObject(requestBody);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Add("Authorization", $"Bearer {API_KEY}");
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                generatedText = json["choices"][0]["message"]["content"].ToString();

                break;
            }
            catch (HttpRequestException e)
            {
                if (attempt < MAX_RETRIES - 1)
                {
                    Debug.LogWarning($"Retrying in {RETRY_DELAY} seconds... ({e.Message})");
                    await Task.Delay(RETRY_DELAY * 1000);
                }
                else
                {
                    Debug.LogError("Max retries reached. Aborting.");
                    throw;
                }
            }
        }

        caseInformation = generatedText;
        lastText = generatedText;
        await getSummaryFromAI(generatedText);

        return generatedText;
    }

    public async Task<string> getTextFromAI()
    {
        if (string.IsNullOrEmpty(userInput))
        {
            throw new ArgumentException("userInput cannot be null or empty.");
        }

        string generatedText = null;
        string prompt = userInput;
        for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
        {
            try
            {
                var requestBody = new
                {
                    model = MODEL,
                    messages = new[]
                    {
                        // English
                        // new { role = "system", content = "You are the host of a mystery game. Create and describe a murder case, and provide the situation and clues to help the player successfully identify the suspect." },
                        // Korean
                        new { role = "system", content = "당신은 추리 게임의 사회자입니다. 한국어로 사건을 서술하고, 플레이어에게 한국어로 선택지를 제공합니다. ** 대신 <b>태그를 사용합니다. 간결하게 답변합니다. 플레이어가 범인을 맞추려고 한다면, 추리 성공 혹은 실패 결과를 발표하며 게임을 종료합니다." },
                        new { role = "assistant", content = caseInformation + lastText },
                        new { role = "user", content = userInput }
                    },
                    temperature = 0.3
                };

                var requestJson = JsonConvert.SerializeObject(requestBody);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Add("Authorization", $"Bearer {API_KEY}");
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                generatedText = json["choices"][0]["message"]["content"].ToString();

                break;
            }
            catch (HttpRequestException e)
            {
                if (attempt < MAX_RETRIES - 1)
                {
                    Debug.LogWarning($"Retrying in {RETRY_DELAY} seconds... ({e.Message})");
                    await Task.Delay(RETRY_DELAY * 1000);
                }
                else
                {
                    Debug.LogError("Max retries reached. Aborting.");
                    throw;
                }
            }
        }
        lastText = generatedText;
        await getSummaryFromAI(generatedText);

        if (caseInformation == "END")
        {
            Debug.Log("END");
            generatedText += "\nTHE END.";
        }

        return generatedText;
    }

    public async Task getSummaryFromAI(string newText)
    {
        if (string.IsNullOrEmpty(caseInformation))
        {
            throw new ArgumentException("caseInformation cannot be null or empty.");
        }

        string generatedText = null;
        string summaryPrompt = newText;

        for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
        {
            try
            {
                var requestBody = new
                {
                    model = MODEL,
                    messages = new[]
                    {
                        // English
                        // new { role = "system", content = "Your role is to summarize the game's progress." },
                        // Korean
                        new { role = "system", content = "** 대신 <b>태그를 사용합니다. 간결하게 답변합니다. 게임을 종료한다는 내용이 있으면 END 세 글자만 답변합니다." },
                        new { role = "user", content = summaryPrompt }
                    },
                    temperature = 0.3
                };

                var requestJson = JsonConvert.SerializeObject(requestBody);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
                request.Headers.Add("Authorization", $"Bearer {API_KEY}");
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                generatedText = json["choices"][0]["message"]["content"].ToString();

                break;
            }
            catch (HttpRequestException e)
            {
                if (attempt < MAX_RETRIES - 1)
                {
                    Debug.LogWarning($"Retrying in {RETRY_DELAY} seconds... ({e.Message})");
                    await Task.Delay(RETRY_DELAY * 1000);
                }
                else
                {
                    Debug.LogError("Max retries reached. Aborting.");
                    throw;
                }
            }
        }
        caseInformation = generatedText;
    }

    public void setUserInput(string userInput)
    {
        this.userInput = userInput;
    }
}