namespace merge;
public class Program
{
    ///<summary>
    /// A program that extracts response message from OpenAI Completions response Json object.
    /// Inputs:
    /// args[0]: Completions response json object.
    /// args[1]: Index to extract from response messages.
    /// args[2]: Completions input json to be updated.
    ///</summary>
    public static void Main(string[] args)
    {
        var responseFile = args[0];
        var requesteIndex = args[1];
        var inputFile = args[2];


        JArray? targetArray = GetTargetArray(inputFile, "messages");
        if (targetArray == null)
        {
            Console.Write("Target file missing required 'messages' element.");
            return;
        }

        var newElement = ExtractChoiceElement(responseFile, requesteIndex);
        if (newElement == null)
        {
            Console.Write("Unable to find element to insert");
            return;
        }

        targetArray.Add(newElement);
        targetArray.Add(GetNewMessageElement());
        UpdateInputObjectWithResponse(inputFile, targetArray);
        Console.Write("Request model updated with response....");
    }


    private static void UpdateInputObjectWithResponse(string filename, JArray inputElement)
    {
        string jsonContent = File.ReadAllText(filename);
        JObject jsonObject = JObject.Parse(jsonContent);

        if (jsonObject["messages"] == null)
        {
            Console.Write("Missing required element");
            return;
        }

        jsonObject["messages"] = inputElement;
        File.WriteAllText(filename, jsonObject.ToString());
    }

    private static JArray? GetTargetArray(string targetFile, string element)
    {
        string jsonContent = File.ReadAllText(targetFile);
        JObject jsonObject = JObject.Parse(jsonContent);
        if (jsonObject == null)
        {
            return null;
        }
        var ret = jsonObject[element];
        return ret == null ? null : (JArray)ret;
    }

    private static JToken GetNewMessageElement(string role = "user")
    {
		var settings = new JsonSerializerSettings
		{
			ContractResolver = new CamelCasePropertyNamesContractResolver()
		};

        var message = new Message()
        {
            Role = role
        };

        var el = JsonConvert.SerializeObject(message, settings);
		return JToken.Parse(el);
    }

    private static JToken? ExtractChoiceElement(string sourceFile, string searchIndex)
    {
        var responseJson = File.ReadAllText(sourceFile);
        JObject jsonObject = JObject.Parse(responseJson);

        var jArray = jsonObject["choices"];
        var choices = jArray == null ? null : (JArray)jArray;
        if (choices == null)
        {
            return null;
        }

        foreach (JObject choice in choices)
        {
            var index = choice["index"]?.ToString();
            if (index == searchIndex)
            {
                var message = choice["message"]?.ToString();
                if (message == null)
                {
                    return null;
                }
                return JToken.Parse(message);
            }
        }
        return null;
    }
}
