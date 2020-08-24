using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SessionTokenManager : MonoBehaviour
{
    public static SessionTokenManager Instance;
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    #endregion

    // a request to retrieve a token from the database. The session token is used in each request to avoid getting the 
    // same question twice
    private const string getSessionTokenUrl = "https://opentdb.com/api_token.php?command=request";
    private const string resetTokenUrl = "https://opentdb.com/api_token.php?command=reset&token=";
    // the session token
    private string SessionToken = null;

    public string GetToken()
    {
        if (SessionToken == null)
        {
            StartCoroutine(GetSessionToken());
        }
        return SessionToken;
    }

    public void ResetToken()
    {
        StartCoroutine(ResetSessionToken());
    }

    // an Async function that requests a session token from the api 
    IEnumerator GetSessionToken()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(getSessionTokenUrl))
        {
            yield return request.SendWebRequest();
            // error occured
            if (ErrorOccured(request))
            {
                Debug.LogError(request.error);
            }
            else
            {
                string retrievedData = request.downloadHandler.text; // get the text of the data retrieved
                Token generatedToken = JsonUtility.FromJson<Token>(retrievedData); // deserialize the json text into a class
                SessionToken = generatedToken.token; // set the session token
            }
        }
    }

    // Async resets the session token back to initial condition
    IEnumerator ResetSessionToken()
    {
        // add a postfix that is the current session token to be reset
        var requestURL = resetTokenUrl + SessionToken;

        using (UnityWebRequest request = UnityWebRequest.Get(requestURL))
        {
            yield return request.SendWebRequest();
            // error
            if (ErrorOccured(request))
            {
                Debug.LogError(request.error);
            }
            else
            {
                string retrievedData = request.downloadHandler.text; // get the text
                Token token = JsonUtility.FromJson<Token>(retrievedData); // deserialize the retrieved data into a token object
                if (TokenDoesNotExist(token))
                {
                    StartCoroutine(GetSessionToken()); // gets a new token
                }
            }
        }
    }

    bool ErrorOccured(UnityWebRequest request)
    {
        return request.isNetworkError || request.isHttpError;
    }

    bool TokenDoesNotExist(Token token)
    {
        return token.response_code == (int)ResponseType.TokenNotFound;
    }
}
