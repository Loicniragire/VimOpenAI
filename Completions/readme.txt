The OpenAI directory contains examples of calling GPT model APIs using Curl command.

Assumptions:
- Environment variable: OPENAI_API_KEY

Example: % curl \
			https://api.openai.com/v1/chat/completions \
			-H "Content-Type:application/json" \
			-H Authorization:Bear $OPENAI_API_KEY \
			-o modelResponse.txt \
			-d @path/to/input_file_data.json


Template_input.json
- This input file specifies the API request model for the 'Completions' endpoint.
- It contains three variables: model, messages, and temperature.
- The 'messages' variable is an array of inputs to the model.

Available endpoints
- Chat Completion based on input
- [ POST ] https://api.openai.com/v1/chat/completions
- Request model:
{
	"model": "",
	"messages":[],
}

- Create speech: Generating audio from input text
- [ POST ] https://api.openai.com/v1/audio/speech
	ex: curl https://api.openai.com/v1/audio/speech \
		  -H "Authorization: Bearer $OPENAI_API_KEY" \
		  -H "Content-Type: application/json" \
		  -d '{
			"model": "tts-1",
			"input": "The quick brown fox jumped over the lazy dog.",
			"voice": "alloy" }' \   ( Supported voices are alloy, echo, fable, onyx, nova, and shimmer )
		  --output speech.mp3


- Creating Assistants
- [ POST ] https://api.openai.com/v1/assistants
- Request model:
{
"model":"", // see availables models [ GET ] https://api.openai.com/v1/models
"name":"", // optional: name of the assistant
"description":"",
"instructions":"System instructions that the assistant uses",
"tools":"", // List of enabled tools: code_interpreter, retrieval, or function.
"file_ids":"" // Listing of file IDs attached to this assistant. A max of 20 files allowed.
}

- Ex: curl https://api.openai.com/v1/assistants \
		  -H "Content-Type: application/json" \
		  -H "Authorization: Bearer $OPENAI_API_KEY" \
		  -H "OpenAI-Beta: assistants=v1" \
		  -d '{
			"instructions": "You are an HR bot, and you have access to files to answer employee questions about company
		policies.",
			"tools": [{"type": "retrieval"}],
			"model": "gpt-4",
			"file_ids": ["file-abc123"]
	  }'


- To upload files: [ POST ] https://api.openai.com/v1/files
Request body:
{
"file":"",
"purpose":""
}
- Example:
	curl https://api.openai.com/v1/files \
	  -H "Authorization: Bearer $OPENAI_API_KEY" \
	  -F purpose="fine-tune" \  **use "file-tune" for Fine-Tuning or "assistant" for Assistant or Messages
	  -F file="@mydata.jsonl"

- To retrieve assistant: [ GET ] https://api.openai.com/v1/assistants/{assistant_id}
- To list assistants: [ GET ] https://api.openai.com/v1/assistants

For additional documentation see: https://platform.openai.com/docs/api-reference/chat/create
	


