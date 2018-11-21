using System;

[Serializable]
public class RequestData
{
    public ResponseType Response_Code
    {
        get
        {
            return (ResponseType) response_code;
        }
    }

    public int response_code;
    public Question[] results;
    public int MyProperty { get; set; }
    public override string ToString()
    {
        return "Response Code: " + response_code + "\n Resulting Questions: " + results.ToString();
    }

}

public enum ResponseType
{
    Success,
    NoResults,
    InvalidParameter,
    TokenNotFound,
    TokenEmpty
};
