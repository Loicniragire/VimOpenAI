The OpenAI directory contains examples of calling GPT model APIs using Curl command.

Note: Assistants API require that your pass a beta HTTP header. OpenAI-Beta: assistants=v1


Assumptions:
- Environment variable: OPENAI_API_KEY

Example: % curl https://api.openai.com/v1/assistants \
			-H "Content-Type:application/json" \
			-H Authorization:Bear $OPENAI_API_KEY \
			-H OpenAI-Beta: assistants=v1 \
			-o modelResponse.txt \
			-d @path/to/request_model.json

Request model
{
  "model": "gpt-3.5-turbo",
  "name":"Optional name of the assistant",
  "description":"Optional description of the assistant",
  "instructions":"The system instructions that the assistant uses",
  "tools":[{"type":"retrieval"}], // A list of tool enabled on the assistant. Tools can be of type "code_interpreter",
"retrieval", or "function".
  "file_ids": (Optional)[A list of file IDs attached to this assistant. A max of 20 files is allowed.],
  "metadata": (Optional) Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional
information about a structured format.
}

Example:

curl "https://api.openai.com/v1/assistants" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $OPENAI_API_KEY" \
  -H "OpenAI-Beta: assistants=v1" \
  -d '{
    "instructions": "You are a personal math tutor. When asked a question, write and run Python code to answer the question.",
    "name": "Math Tutor",
    "tools": [{"type": "code_interpreter"}],
    "model": "gpt-4"
  }'

The above request return an Assistant object.
Sample response:
{
  "id": "asst_Dp8Bv0PASjTe08NybMvF9nls",
  "object": "assistant",
  "created_at": 1701182821,
  "name": "Math Tutor",
  "description": "A general Math tutor",
  "model": "gpt-3.5-turbo",
  "instructions": "You are a personal math tutor.When asked a question, write an run Python code to answer the question.",
  "tools": [
    {
      "type": "code_interpreter"
    }
  ],
  "file_ids": [],
  "metadata": {}
}

To retrieve/delete an assistant object:
[GET] or [DELETE] https://api.openai.com/v1/assistants/{assistant_id}



To upload a file that can be used by an assistant:
[ POST ] https://api.openai.com/v1/files
Files can be up to 100 GB

Request body:
Simply pass in the path to the file - Ex: OpenAIAssistantFile "path-to-file" "output-file.json"

Supported formats: ['c', 'cpp', 'csv', 'docx', 'html', 'java', 'json', 'md', 'pdf', 'php', 'pptx', 'py', 'rb', 'tex', 'txt', 'css', 'jpeg', 'jpg', 'js', 'gif', 'png', 'tar', 'ts', 'xlsx', 'xml', 'zip']",

- Example:
	curl https://api.openai.com/v1/files \
	  -H "Authorization: Bearer $OPENAI_API_KEY" \
	  -F purpose="fine-tune" \  **use "file-tune" for Fine-Tuning or "assistant" for Assistant or Messages
	  -F file="@mydata.jsonl"

- To retrieve assistant: [ GET ] https://api.openai.com/v1/assistants/{assistant_id}
- To list assistants: [ GET ] https://api.openai.com/v1/assistants

To create/attach an assistant file
[POST] https://api.openai.com/v1/assistants/{assistant_id}/files
Request model:
{
	"file_id": "A File ID (with purpose="assistants") that the assistant should use."
}
A maximum of 20 files can be attached to an assistant.




For additional documentation see: https://platform.openai.com/docs/api-reference/chat/create
	


