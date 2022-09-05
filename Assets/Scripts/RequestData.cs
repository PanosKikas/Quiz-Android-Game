using System;

// The reuqest data is the object that contains the list of questions
// when first retrieved from the api as json as well as a response code
// indicating if the request was was successfull
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
    public Question[] results; // list of questions fetched
    public int MyProperty { get; set; }
    public override string ToString()
    {
        return "Response Code: " + response_code + "\n Resulting Questions: " + results.ToString() + "\n";
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
