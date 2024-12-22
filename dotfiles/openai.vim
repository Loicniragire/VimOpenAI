" OPENAI
" Directory structure
" - OPENAI_WORKING_DIR
"   - OPENAI_PROJECT_HOME_DIR
"       - requests
"       - responses
"       - output
"       - source
"
" Within a working directory, there can be multiple projects.
" To set the working directory to default, use the command :OpenAISetWorkingProject <project_path>
"
" Notes:
" A thread is created by namespace. When creating a thread message, user specifies the classname and the request message.
" Using the classname, the assistant determines which namespace the class belong to and creates a thread for that namespace if it doesn't exist.
" When user requests the assistant to execute the thread, all the messages in the thread are sent to the assistant for processing. The
" assistant will process the messages in the order they were received. The assistant will also keep track of the state of the thread.
" The assistant will return the response for each message in the thread. The assistant will also return the state of the thread.
"
" Usage:
" 0. Updated g:OPENAI_PROJECT_HOME_DIR as needed. Using the command :OpenAISetHomeDir <project_path>. Use :OpenAIGetHomeDir to get the current value.
" 1. Setup working project. This is the cs project containing the files to word with. Command: OpenAISetupProject <project_path>
" 2. Navigate the the requests directory and create a message file. There should be a sample request file. Note that classname is case sensitive.
" 3. Submit created message. Command: OpenAICreateMessage <message_file_name>. This should return the created message with a thread id.
" 4. Execute the thread. Command: OpenAIExecuteThread <thread_id>. This should return the response for each message in the thread. 
" The assistant has access to all previously created messages - as found in the thread responses.
"
" When a new message is created, the same thread will be used only if the classes are in the same namespace. Otherwise a new thread is
" created for the new namespace.
"
set encoding=utf-8

let g:OPENAI_WORKING_DIR = '~/source/repos/AI_ProjectAssistant/'
let g:OPENAI_COMPLETION_CMD = 'curl -sS -X POST https://api.openai.com/v1/chat/completions -H "Content-Type: application/json" -H "Authorization: Bearer ' . $OPENAI_API_KEY . '" -d @'

let g:OPENAI_ASSISTANTS_CMD = 'curl -sS -X POST https://api.openai.com/v1/assistants -H "Content-Type: application/json" -H "Authorization: Bearer ' . $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_FILES_CMD = 'cur -sSl -X POST https://api.openai.com/v1/files -H "Content-Type: multipart/form-data" -H "Authorization: Bearer ' . $OPENAI_API_KEY . '" -F "purpose=assistants" -F "file="@'
let g:OPENAI_ASSIGN_FILE_TO_ASSISTANT_CMD = 'curl -sS -X POST https://api.openai.com/v1/assistants/{assistant_id}/files -H "Content-Type: application/json" -H "Authorization: Bearer ' . $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" -d '
let g:OPENAI_DELETE_FILE_CMD = 'curl -sS -X DELETE -sS https://api.openai.com/v1/files/{file_id} -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" '
let g:OPENAI_RETRIEVE_FILE_CMD = 'curl https://api.openai.com/v1/files/{file_id}/content -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" '
let g:OPENAI_LIST_FILES_CMD = 'curl https://api.openai.com/v1/files -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" '
let g:OPENAI_LIST_ASSISTANTS_CMD = 'curl https://api.openai.com/v1/assistants -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_DELETE_ASSISTANT_CMD = 'curl -sS -X DELETE  https://api.openai.com/v1/assistants/{assistant_id} -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_ASSISTANT_THREAD_CMD = 'curl https://api.openai.com/v1/threads -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" -d ""  '
let g:OPENAI_DELETE_ASSISTANT_THREAD_CMD = 'curl -X DELETE  https://api.openai.com/v1/threads/{thread_id} -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_ASSISTANT_THREAD_MESSAGE_CMD = 'curl https://api.openai.com/v1/threads/{thread_id}/messages -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" -d '
let g:OPENAI_ASSISTANT_LIST_THREAD_MESSAGE_CMD = 'curl https://api.openai.com/v1/threads/{thread_id}/messages -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_ASSISTANT_THREAD_RUN_CMD = 'curl https://api.openai.com/v1/threads/{thread_id}/runs -H "Content-Type: application/json" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" -d '
let g:OPENAI_ASSISTANT_RETRIEVE_THREAD_RUN_CMD = 'curl https://api.openai.com/v1/threads/{thread_id}/runs/{run_id} -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -H "OpenAI-Beta:assistants=v1" '
let g:OPENAI_ASSISTANT_MODEL = 'gpt-4-1106-preview'

let g:OPENAI_CS_PROJECT_ASSISTANT_INSTUCTIONS = 'The assistant is tasked to work on various .NetCore projects, ' .
    \ 'each involving different files and functionalities. The projects are primarily developed in CSharp. ' .
	\ 'All the project files have been uploaded as text (.txt) files. Each file represents a single namespace from the project' .
	\ 'and the filename corresponds to the namespace name. Within each file, all classes, interfaces, and other types that belong to that namespace ' .
	\ 'are included.' .
    \ 'Tasks: ' .
    \ '1. Code Analysis: Perform a comprehensive analysis on the provided .NetCore project files, ' .
    \ 'focusing on performance, security, and adherence to best practices. Identify areas for improvement and suggest optimizations. ' .
    \ '2. Feature Implementation and Enhancement: Based on the project requirements, implement new features or ' .
    \ 'enhance existing functionalities. Ensure that implementations are efficient, maintainable, and align with the project''s overall architecture. ' .
    \ '3. Code Generation: Generate CSharp code snippets or entire modules as required by the specific project. ' .
    \ 'The code should be compatible with the .NetCore version in use and follow established coding conventions. ' .
	\ 'Requirements will be provided in the format of GIVEN, WHEN, THEN statements. ' .
    \ '4. Testing and Documentation: Develop testing strategies, including unit and integration tests, ' .
    \ 'to ensure the robustness of new or modified code. Produce concise documentation for the implemented code, ' .
    \ 'outlining its purpose, usage, and any special considerations. ' .
    \ '5. File Management: Manage and organize project files efficiently, ensuring that the project structure is maintained and easily navigable. ' .
    \ 'Expected Deliverables: For each project, deliver a report on code analysis, implemented features or enhancements with corresponding CSharp code, ' .
    \ 'testing strategies, and documentation. Projects should be left in a clean, well-documented state. ' .
    \ 'General Considerations: Adapt to the specific requirements of each project, including but not limited to different .NetCore versions, ' .
    \ 'third-party libraries, and database integrations. Ensure compatibility and seamless integration with existing project components. ' .
    \ 'Deadline: As per individual project timelines.' .
    \ 'For any code generated by the assistant, the assistant should return a file_id that can be used to download generated code. ' 

function! SetToDefaultWorkingDir()
	let l:defaultHome = fnamemodify(g:OPENAI_WORKING_DIR, ':p')
    let l:projectHomeDirectory = l:defaultHome . 'default/'
	let l:requestsDir = l:projectHomeDirectory . 'requests'
	let l:responsesDir = l:projectHomeDirectory . 'responses'
	let l:outputDir = l:projectHomeDirectory . 'output'
	let l:sourceDir = l:projectHomeDirectory . 'source'

	let g:OPENAI_PROJECT_HOME_DIR = l:defaultHome
	let g:OPENAI_REQUESTS_DIR = l:requestsDir
	let g:OPENAI_RESPONSES_DIR = l:responsesDir
	let g:OPENAI_OUTPUT_DIR = l:outputDir
	let g:OPENAI_SOURCE_DIR = l:sourceDir

    " Create the directories if they don't exist
    call mkdir(l:projectHomeDirectory, 'p')
    call mkdir(l:requestsDir, 'p')
    call mkdir(l:responsesDir, 'p')
    call mkdir(l:outputDir, 'p')
    call mkdir(l:sourceDir, 'p')

    "echo directory paths
    echo 'OPENAI_PROJECT_HOME_DIR: ' . g:OPENAI_PROJECT_HOME_DIR
    echo 'OPENAI_REQUESTS_DIR: ' . g:OPENAI_REQUESTS_DIR
    echo 'OPENAI_RESPONSES_DIR: ' . g:OPENAI_RESPONSES_DIR
    echo 'OPENAI_OUTPUT_DIR: ' . g:OPENAI_OUTPUT_DIR
    echo 'OPENAI_SOURCE_DIR: ' . g:OPENAI_SOURCE_DIR
endfunction

" command to reset the working directory to default
" It creates a default directory structure under the OPENAI_WORKING_DIR
" and sets the global variables to point to the newly created directories.
command! -nargs=0 OpenAISetToDefault call SetToDefaultWorkingDir()

" Function to attach the given fileId to the specified assistantId
" Parameters:
" Payload model: { "file_id": ""}
function! AssignOpenAIFileToAssistant(assistantId, requestPayload, outputfile)
	let l:cmd = g:OPENAI_ASSIGN_FILE_TO_ASSISTANT_CMD
	let l:response = substitute(l:cmd,'{assistant_id}',a:assistantId,'')
	let l:curl_cmd = l:response. '@' . a:requestPayload . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
	execute 'edit ' . a:outputfile
endfunction


" Define a function to execute the curl command
function! SendToOpenAI(requestPayload, outputfile, endpoint)
	" Construct the curl command
	let l:curl_cmd = ''
	if a:endpoint == 'Completions'
		let l:curl_cmd = g:OPENAI_COMPLETION_CMD. a:requestPayload . ' -o ' . a:outputfile
	elseif a:endpoint == 'Assistants'
		let l:curl_cmd = g:OPENAI_ASSISTANTS_CMD. ' -d @' . a:requestPayload . ' -o ' . a:outputfile
	elseif a:endpoint == 'AssistantFiles'
		let l:curl_cmd = g:OPENAI_FILES_CMD. a:requestPayload . ' -o ' . a:outputfile
	endif
	" Execute the curl command
	" execute '!' . l:curl_cmd
	" execute 'edit ' . a:outputfile
	"echo 'curl_cmd: ' . l:curl_cmd
endfunction

function! GetOpenAIUploadedFiles(outputfile)
	let l:curl_cmd = g:OPENAI_LIST_FILES_CMD . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
	execute 'edit ' . a:outputfile
endfunction

function! GetOpenAIAssistants(outputfile)
	let l:curl_cmd = g:OPENAI_LIST_ASSISTANTS_CMD . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
	" open outputfile in a new buffer
	execute 'edit ' . a:outputfile
	
endfunction

" Deletes an uploaded Assistant file
" Parameters:
" fileId: Id of the file to delete
" Returns:
" None
function! DeleteOpenAIAssistantFile(fileId)
	let l:cmd = g:OPENAI_DELETE_FILE_CMD
	let l:curl_cmd = substitute(l:cmd,'{file_id}',a:fileId,'')
	execute '!' . l:curl_cmd
endfunction

function! DeleteOpenAIAssistant(assistantId)
	let l:cmd = g:OPENAI_DELETE_ASSISTANT_CMD
	let l:curl_cmd = substitute(l:cmd,'{assistant_id}',a:assistantId,'')
	execute '!' . l:curl_cmd
endfunction

function! CreateOpenAIAssistantThread()
	let l:outputfile = g:OPENAI_OUTPUT_DIR . '/threads_output_tmp.json'
	let l:cmd = g:OPENAI_ASSISTANT_THREAD_CMD . ' -o ' . l:outputfile

    "echo 'cmd: ' . l:cmd
	execute 'silent !' . l:cmd
	" Read the content of the output file as a string
	let output = join(readfile(l:outputfile), "\n")
	return output
endfunction

" Create a thread pool of assistants
" Parameters:
" count: Number of threads to create
"command! -nargs=1 OpenAICreateAssistantThreadPool call CreateOpenAIAssistantThreads(<f-args>)

function! DeleteOpenAIAssistantThread(threadId)
	let l:cmd = g:OPENAI_ASSISTANT_THREAD_CMD
	let l:curl_cmd = substitute(l:cmd,'{thread_id}',a:threadId,'')
	execute '!' . l:cmd
endfunction

function! MessageJsonPayload(message, fileIds)
	let l:messageRole = 'user'
	let l:file_ids = CreateJSONPayload(a:fileIds)
	let l:payload = "{" .  "\"role\": "  . '"'. l:messageRole . '",' .  "\"content\": " . '"' . a:message . '",' .  "\"file_ids\": " . l:file_ids .  "}"
     "return json_encode(l:payload)
     return l:payload
endfunction

function! ThreadMessageJsonPayload(threadId, message, fileIds)
	let l:file_ids = CreateJSONPayload(a:fileIds)
	return "{" .  "\"threadId\": " . '"' . a:threadId . '",' . "\"message\": " . '"' . a:message . '",' . "\"fileIds\":"  . l:file_ids .  "}"
endfunction

function! CreateThreadMessage(messageRequest, outputfile)
	let l:fileIds = []
	let jsonObj = json_decode(a:messageRequest)

	" convert json object to dictionary
	let l:jsonObjDict = {}
	for [key, value] in items(jsonObj)
		let l:jsonObjDict[key] = value
	endfor

	if has_key(jsonObjDict, "threadId") && has_key(jsonObjDict, "message")
		let l:threadId = jsonObjDict["threadId"]
		let l:messageContent = jsonObjDict["message"]
		"check if it has optional fileIds key
		if has_key(jsonObjDict, "fileIds")
			let l:fileIds = jsonObjDict["fileIds"]
		endif
	else
		echoerr "Invalid JSON structure: 'threadid' or 'message' key not found"
		return
	endif

	let l:requestPayload = MessageJsonPayload(l:messageContent, l:fileIds)
    let l:jsonEncodedPayload = json_encode(l:requestPayload)

	" Properly escape the JSON payload for shell command
	"let l:escapedPayload = shellescape(l:requestPayload, 1)

	"echo 'requestPayload: ' . l:escapedPayload
	let l:cmd = g:OPENAI_ASSISTANT_THREAD_MESSAGE_CMD
	let l:response = substitute(l:cmd,'{thread_id}',l:threadId,'')

    let l:generatedOutputFile = g:OPENAI_REQUESTS_DIR . '/' . l:threadId . '_' . a:outputfile

	let l:curl_cmd = l:response. l:jsonEncodedPayload . ' -o ' . l:generatedOutputFile
    "echo 'curl_cmd - CreateThreadMessage: ' . l:curl_cmd
	execute '!' . l:curl_cmd
    " Open the output file
    execute 'edit ' . l:generatedOutputFile
endfunction

function! OpenAIAssistantThreadListMessages(threadId, outputfile)
	let l:cmd = g:OPENAI_ASSISTANT_LIST_THREAD_MESSAGE_CMD
	let l:response = substitute(l:cmd,'{thread_id}',a:threadId,'')
	let l:curl_cmd = l:response . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
	execute 'edit ' . a:outputfile
endfunction

function! CreateOpenAIAssistantThreadRun(threadId, requestPayload, outputfile)
	let l:cmd = g:OPENAI_ASSISTANT_THREAD_RUN_CMD
	let l:response = substitute(l:cmd,'{thread_id}',a:threadId,'')
	let l:curl_cmd = l:response. "@" . a:requestPayload . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
	execute 'edit ' . a:outputfile
endfunction

function! RetrieveOpenAIAssistantThreadRun(threadId, runId, outputfile)
	let l:cmd = g:OPENAI_ASSISTANT_RETRIEVE_THREAD_RUN_CMD
	let l:response = substitute(l:cmd,'{thread_id}',a:threadId,'')
	let l:response = substitute(l:response,'{run_id}',a:runId,'')
	let l:curl_cmd = l:response . ' -o ' . a:outputfile
	execute '!' . l:curl_cmd
endfunction

" Define a Vim command that calls the function
"command! -nargs=+ OpenAICompletions call SendToOpenAI(<f-args>, 'Completions')
"command! -nargs=+ OpenAIAssistants call SendToOpenAI(<f-args>, 'Assistants')
"command! -nargs=+ OpenAIAAssistantFiles call SendToOpenAI(<f-args>, 'AssistantFiles')

"command! -nargs=+ OpenAIAssignFileToAssistant call AssignOpenAIFileToAssistant(<f-args>, g:OPENAI_OUTPUT_DIR . '/openai_assign_file_to_assistant_output.json')
command! -nargs=0 OpenAIListUploadedFiles call GetOpenAIUploadedFiles(g:OPENAI_OUTPUT_DIR . '/openai_list_files_output.json')
command! -nargs=0 OpenAIListAssistants call GetOpenAIAssistants(g:OPENAI_OUTPUT_DIR . '/openai_list_assistants_output.json')
command! -nargs=1 OpenAIDeleteAssistant call DeleteOpenAIAssistant(<f-args>)

"
"command! -nargs=1 OpenAIDeleteAssistantThread call DeleteOpenAIAssistantThread(<f-args>)

" arg1: json file containing threadId and message
" Sample call:
" :OpenAIAssistantThreadMessage openai_assistant_thread_message_request.json
" file structure:
" {
"	"threadId": "thrd_1GJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5",
"	"message": "Hello, I am a message",
"	"fileIds": ['file_1GJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5', 'file_1GJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5'] }
"command! -nargs=1 OpenAIAssistantThreadMessage call CreateThreadMessage(<f-args>, 'openai_assistant_thread_message_output.json')

" arg1: threadId
command! -nargs=1 OpenAIAssistantListThreadMessages call OpenAIAssistantThreadListMessages(<f-args>, g:OPENAI_OUTPUT_DIR . '/openai_assistant_thread_list_messages_output.json');

" arg1: threadId
" arg2: requestPayload
" payload model:
"      {
"         "assistant_id": "[Required]",
"         "model": "[Optional]",
"         "instructions": "[Optional]",
"         "tools": "[Optional]",
"      }
" To retrieve the response from thread run, run OpenAIAssistantListThreadMessages command
" Note: Thread Run's status: "Completed" is expected prior to receiving the response. Use OpenAIAssistantRetrieveThreadRun command to retrieve the response.
"command! -nargs=+ OpenAIAssistantThreadRun call CreateOpenAIAssistantThreadRun(<f-args>, 'openai_assistant_thread_run_output.json');

" arg1: threadId
" arg2: runId
"command! -nargs=+ OpenAIAssistantRetrieveThreadRun call RetrieveOpenAIAssistantThreadRun(<f-args>, 'openai_assistant_retrieve_thread_run_output.json');

" Define a function to list files with specified extensions in a directory
" Parameters:
" directoryPath: Path to the directory containing the files to list
" extMap: Dictionary mapping original extensions to new extensions
" outputDir: Path to the directory where the files will be copied. It will create the directory if it doesn't exist.
" Returns:
" None
"
"
" Sample call:
" :call ListFilesWithExtensions('C:\Users\user\Documents\test', {'cs': 'txt'}, 'C:\Users\user\Documents\test\output')
" Output: Copied with UTF-8 encoding: C:\Users\user\Documents\test\test.cs -> C:\Users\user\Documents\test\output\test.txt
" 	   Copied with UTF-8 encoding: C:\Users\user\Documents\test\test2.cs -> C:\Users\user\Documents\test\output\test2.txt
" 	   Copied with UTF-8 encoding: C:\Users\user\Documents\test\test3.cs -> C:\Users\user\Documents\test\output\test3.txt
function! ListFilesWithExtensions(directoryPath, extMap, outputDir)
    " Create the output directory if it doesn't exist
	if !isdirectory(a:outputDir)
		call mkdir(a:outputDir, 'p')
	endif

    " Define a helper function for recursive listing and copying
    function! s:ListDirRecursively(path, extMap, outputDir)
        " List all items in the current directory
        for item in split(glob(a:path . '/*'), '\n')
            " Skip if item is an empty string (happens when glob finds nothing)
            if empty(item)
                continue
            endif

            " Check if item is a directory
            if isdirectory(item)
                " Recursively list and copy files in this directory
                call s:ListDirRecursively(item, a:extMap, a:outputDir)
            elseif filereadable(item)
                " Iterate through each extension in the map
                for origExt in keys(a:extMap)
                    " Check if file has the original extension
                    if item =~ '\.' . origExt . '$'
                        " Determine the new extension
                        let newExt = get(a:extMap, origExt, origExt)
                        if newExt == ''
                            let newExt = origExt
                        endif
                        let newFileName = fnamemodify(item, ':t:r') . '.' . newExt
                        let newFilePath = a:outputDir . '/' . newFileName
						" Copy the file using the shell
						if has('win32')
                            execute 'silent !copy ' . shellescape(item) . ' ' . shellescape(newFilePath)
                        else
                            execute 'silent !cp ' . shellescape(item) . ' ' . shellescape(newFilePath)
                        endif

						" Output the operation
                        echo 'Copied with UTF-8 encoding: ' . item . ' -> ' . newFilePath
                        break
                    endif
                endfor
            endif
        endfor
    endfunction

    " Save the current buffer number
    let currentBufNum = bufnr('%')

    " Check if the directory exists
    if isdirectory(a:directoryPath)
        " Start the recursive listing and copying
        call s:ListDirRecursively(a:directoryPath, a:extMap, a:outputDir)

        " Restore the original buffer
        execute 'buffer ' . currentBufNum
    else
        echo 'Directory does not exist: ' . a:directoryPath
    endif
endfunction

" Define a Vim command that calls the function with a map of extensions
"command! -nargs=1 ExtractCsFiles call ListFilesWithExtensions(<f-args>, {'cs': 'txt'}, fnamemodify(<f-args>, ':t') .  '_project')


" arg1: directoryPath containing artifacts from ExtractCsFiles command
"command! -nargs=1 OpenAICreateCsProjectAssistant call CreateOpenAICsProjectAssistant(<f-args>,fnamemodify(<f-args>, ':t') . '_assistant')


" Function to create an OpenAI assistant for a project
" Parameters:
" directoryPath: Path to the directory containing the files to upload
" projectName: Name to assign to the assistant
" Returns:
" Assistant creation response
function! CreateOpenAICsProjectAssistant(directoryPath, projectName)
	"get the full path of the directory
	let l:directoryPath = fnamemodify(a:directoryPath, ':p')
	echo 'Creating assistant for project: ' . a:projectName . ' with files in directory: ' . l:directoryPath
	"Generate an assistant for the project
	let l:assistantId = CreateProjectAssistant(a:projectName, 'A generic .NetCore project assistant', g:OPENAI_CS_PROJECT_ASSISTANT_INSTUCTIONS)
	"let l:assistantId = CreateProjectAssistant(a:projectName, "A generic .NetCore project assistant", "Test...")

    "echo 'Assistant created with id: ' . l:assistantId
    "
	" Upload all files in the directory to OpenAI
	let l:uploadedFiles = OpenAIUploadAssistantFiles(l:directoryPath)
	" write the json responses to a file
	call WriteJSONToFile(l:uploadedFiles, g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json')

	let l:keys = ExtractKeys(l:uploadedFiles, ['id', 'filename'])
	" create a dictionary with filename as key and id as value
	let l:fileIdMap = MapValues(l:keys, 'filename', 'id')

	" get a json string of the filenames with empty values
	let l:fileThreadMapping = CreateJsonWithEmptyValues(l:fileIdMap)
	" write the json string to a file
	call writefile(split(l:fileThreadMapping, '\n'), g:OPENAI_OUTPUT_DIR . '/openai_namespace_file_thread_mapping.json', 'b')

	" assign the cs files to the assistant
    " Define a list to store the json responses as strings
    let l:json_responses = []

	" loop through the fileIdMap and create a json payload with "file_id": set to the dictionary value
	for [key, value] in items(l:fileIdMap)
        let l:fileIdMapJson = "{" . "\"file_id\": " . '"' . value . '"' .  "}"
		let l:fileAssignPayload = json_encode(l:fileIdMapJson)
		let l:outputfile = g:OPENAI_OUTPUT_DIR . '/openai_assign_file_to_assistant_output.json'

		let l:cmd = g:OPENAI_ASSIGN_FILE_TO_ASSISTANT_CMD
		let l:response = substitute(l:cmd,'{assistant_id}',l:assistantId,'')
		let l:curl_cmd = l:response. l:fileAssignPayload . ' -o ' . l:outputfile
		execute 'silent !' . l:curl_cmd
        "
		" Read the content of the output file as a string
		let output = join(readfile(l:outputfile), "\n")

		" Add the response string to the list
		call add(l:json_responses, output)
	endfor

	return l:json_responses
endfunction


" Function to map the values of two arrays in a dictionary
" Parameters:
" input_dict: Dictionary containing the arrays
" search_key: Key for the array containing the keys
" search_value: Key for the array containing the values
" Returns:
" Dictionary with the keys and values mapped
" Sample call:
" :echo MapValues({'id': ['1', '2'], 'filename': ['abc', 'rf']}, 'filename', 'id')
" Output: {'abc': '1', 'rf': '2'}
function! MapValues(input_dict, search_key, search_value)
    " Create a dictionary to store the results
    let resultDict = {}

    " Ensure that both search_key and search_value exist in input_dict
    if has_key(a:input_dict, a:search_key) && has_key(a:input_dict, a:search_value)
        " Get the arrays associated with the search_key and search_value
        let searchKeyArray = a:input_dict[a:search_key]
        let searchValueArray = a:input_dict[a:search_value]

        " Ensure the arrays are of the same length
        if len(searchKeyArray) == len(searchValueArray)
            " Iterate over the arrays and map the values
            for i in range(len(searchKeyArray))
                let key = searchKeyArray[i]
                let value = searchValueArray[i]
                let resultDict[key] = value
            endfor
        else
            echo "Error: Arrays for search_key and search_value are of different lengths."
        endif
    else
        echo "Error: One or both keys not found in the input dictionary."
    endif

    return resultDict
endfunction


function! WriteJSONToFile(jsonArray, filename)
    " Check if jsonArray is indeed a list
    if type(a:jsonArray) != type([])
        echoerr 'Invalid input: jsonArray should be a list'
        return
    endif

    " Start the JSON array string with indentation
    let l:jsonString = "[\n"

    " Iterate over each JSON string in the array
    for idx in range(len(a:jsonArray))
        let jsonStr = a:jsonArray[idx]
        " Validate that jsonStr is a string
        if type(jsonStr) != type('')
            echoerr 'Invalid input: each item in jsonArray should be a string'
            return
        endif
        " Add this JSON string to the array string with indentation
        let l:jsonString .= '    ' . jsonStr
        " Add a comma except for the last element, and start a new line
        if idx < len(a:jsonArray) - 1
            let l:jsonString .= ','
        endif
        let l:jsonString .= "\n"
    endfor

    " Close the array string
    let l:jsonString .= ']'

    " Write the JSON array string to the file
    call writefile(split(l:jsonString, '\n'), a:filename, 'b')
endfunction


" Function to delete all files uploaded to OpenAI Assistant.
" Parameters:
" None
" Returns:
" None
function! DeleteAllAssistantFiles()
	let l:outputfile = g:OPENAI_OUTPUT_DIR . '/openai_list_files_output.json'
	let l:cmd = g:OPENAI_LIST_FILES_CMD . ' -o ' . l:outputfile
	execute '!' . l:cmd

	" if the output file doesn't exist, or is empty, return
	if !filereadable(l:outputfile) || getfsize(l:outputfile) == 0
		echo 'No OPENAI files found'
		return
	endif

	function! s:DeleteFilesById(jsonStr)
		let l:cmd = g:OPENAI_DELETE_FILE_CMD
		" Decode the JSON string
		let jsonObj = json_decode(a:jsonStr)
		" Check if 'data' key exists and is a List
		if has_key(jsonObj, 'data') && type(jsonObj['data']) == type([])
			" Iterate over each item in the 'data' array
			for item in jsonObj['data']
				" Check if 'id' key exists in the item
				if has_key(item, 'id')
					" delete the file
					let l:curl_cmd = substitute(l:cmd,'{file_id}',item['id'],'')
					execute 'silent !' . l:curl_cmd
				endif
			endfor
		else
			echoerr "Invalid JSON structure: 'data' key not found or is not an array"
		endif
	endfunction

	" pass the json output file to the function
	let l:files = join(readfile(l:outputfile), "\n")
	call s:DeleteFilesById(l:files)
endfunction

" Command to delete all uploaded files regargedless of project.
" Note that it does not delete the assistant itself.
command! -nargs=0 OpenAIDeleteAllAssistantFiles call DeleteAllAssistantFiles()

" Function to delete all files uploaded to OpenAI for a project. See OpenAIUploadAssistantFiles for uploading files.
" Parameters:
" filepath: Path to the file containing the JSON responses
" Returns:
" None
function! DeleteCsProjectUploadedAssistantFiles(filepath)
    let l:inputfile = a:filepath
    " if filepath is empty, set it to the default
    if empty(a:filepath)
        let l:inputfile = g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json'
    endif

    " Read the file into a list of lines
    let lines = readfile(l:inputfile)

    " Join the lines into a single string and decode JSON
    let jsonObjects = json_decode(join(lines, ''))

    " Check if the decoded data is a list
    if type(jsonObjects) != type([])
        echoerr 'Invalid JSON: Expected a JSON array'
        return
    endif

    " Iterate over each object in the JSON array
    for jsonObject in jsonObjects
        " Check if jsonObject is a dictionary
        if type(jsonObject) == type({})
            " Check if jsonObject has an 'id' key
            if has_key(jsonObject, 'id')
				echo 'Deleting file with id: ' . jsonObject['id']
				call DeleteOpenAIAssistantFile(jsonObject['id'])
            endif
        endif
    endfor
endfunction

" Command to delete all uploaded files by a project.
" Note that it does not delete the assistant itself.
command! -nargs=0 OpenAIDeleteCsProjectUploadedAssistantFiles call DeleteCsProjectUploadedAssistantFiles('')

" Function to upload all files in a directory to OpenAI
" Parameters:
" directoryPath: Path to the directory containing the files to upload
" Returns:
" List of JSON responses
function! OpenAIUploadAssistantFiles(directoryPath)
    " If directoryPath is not a directory, return
    if !isdirectory(a:directoryPath)
        echo 'Directory does not exist: ' . a:directoryPath
        return
    endif

	let l:outputfile = g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json'
    " Define a list to store the json responses as strings
    let l:json_responses = []

    " loop through all the files in the directory
    for file in split(glob(a:directoryPath . '/*'), '\n')
        " Skip if item is an empty string (happens when glob finds nothing)
        if empty(file)
            continue
        endif
        " Check if item is a directory
        if isdirectory(file)
            " Recursively list and copy files in this directory
            call OpenAIUploadAssistantFiles(file)
        elseif filereadable(file)
            " upload the file
            let l:cmd = 'curl -sS -X POST https://api.openai.com/v1/files -H "Content-Type: multipart/form-data" -H "Authorization: Bearer ' .  $OPENAI_API_KEY . '" -F "purpose=assistants" -F "file="@' . shellescape(file, 1)

            let l:curl_cmd = l:cmd . ' -o ' . l:outputfile
            execute 'silent !' . l:curl_cmd

            " Read the content of the output file as a string
            let output = join(readfile(l:outputfile), "\n")

            " Add the response string to the list
            call add(l:json_responses, output)
        endif
    endfor

    return l:json_responses
endfunction

" Define the function
function! ExtractKeys(jsonArray, keys)
    " Create a dictionary to store the results
    let resultDict = {}

    " Iterate over the array of JSON objects
    for jsonItem in a:jsonArray
        " Decode each JSON object
        let jsonObject = json_decode(jsonItem)

        " Iterate over the specified keys
        for key in a:keys
            " Check if the key exists in the JSON object
            if has_key(jsonObject, key)
                " Get the value for the key
                let value = jsonObject[key]

                " Check if the key already exists in the result dictionary
                if !has_key(resultDict, key)
                    " Initialize an empty list for this key
                    let resultDict[key] = []
                endif

                " Append the value to the list for this key
                call add(resultDict[key], value)
            endif
        endfor
    endfor

    " Return the result dictionary
    return resultDict
endfunction





" Function to create an OpenAI assistant for a project
" Parameters:
" projectName: Name of the project
" description: Description of the project
" instructions: Instructions for the project
" Returns:
" Assistant Id
function! CreateProjectAssistant(projectName, description, instructions)
    let l:requestPayload = CreateAssistantRequestJSONPayload(a:projectName, a:description, a:instructions)
    let l:jsonEncodedPayload = json_encode(l:requestPayload)

    "
    " Properly escape the JSON payload for shell command
    "let l:escapedPayload = shellescape(l:jsonEncodedPayload, 1)

    " we need to extract the generated Assistant Id from the response
    let l:outputfile = g:OPENAI_OUTPUT_DIR . '/openai_create_project_assistant_output.json'
    let l:cmd = g:OPENAI_ASSISTANTS_CMD
    "let l:curl_cmd = l:cmd . ' -d ' . l:escapedPayload . ' -o ' . l:outputfile
    "enclose the payload in double quotes
    let l:curl_cmd = l:cmd . ' -d ' . l:jsonEncodedPayload .  ' -o ' . l:outputfile
    "echo 'curl command: ' . l:curl_cmd

    execute 'silent !' . l:curl_cmd

    " read the output file and extract the assistant id
    let l:assistantId = ''
    let l:file = readfile(l:outputfile)
    for line in l:file
        if line =~ 'id'
            let l:assistantId = matchstr(line, '"id": "\zs[^"]*')
            echo 'Created assistant for project: ' . a:projectName . ' with id: ' . l:assistantId
            return l:assistantId
            break
        endif
    endfor

    echo 'Failed to create assistant for project: ' . a:projectName . '. Please check the output file: ' . l:outputfile
endfunction



" Generates a JSON payload for creating an OpenAI assistant
" Parameters:
" assistantName: Name of the assistant
" assistantDescription: Description of the assistant
" assistantInstructions: Instructions for the assistant
" Returns:
" JSON payload for creating an assistant
" sample call: 
	":echo CreateAssistantRequestJSONPayload("test assistant","a simple model for testing","You perform actions
" on numbers")
function! CreateAssistantRequestJSONPayload(assistantName, assistantDescription, assistantInstructions)
    let l:tools = [{"type":"code_interpreter"},{"type":"retrieval"}]
    let l:stringfyTools = CreateJSONPayload(l:tools)

    let l:payload = "{" .  "\"model\": " . '"' . g:OPENAI_ASSISTANT_MODEL . '",' . "\"name\": " . '"' . a:assistantName . '",' .  "\"description\": " . '"' . a:assistantDescription . '",' . "\"instructions\": " . '"' . a:assistantInstructions . '",' . "\"tools\": "  . l:stringfyTools .  "}"

    return l:payload
endfunction




function! CreateJSONPayload(dictParam)
    return s:JSONStringify(a:dictParam)
endfunction

function! s:JSONStringify(value)
    if type(a:value) == type([])
        " Handle list
        let l:jsonList = '['
        for item in a:value
            let l:jsonList .= s:JSONStringify(item) . ','
        endfor
        let l:jsonList = substitute(l:jsonList, ',$', '', '') . ']'
        return l:jsonList
    elseif type(a:value) == type({})
        " Handle dictionary
        let l:jsonDict = '{'
        for [key, value] in items(a:value)
            let l:jsonDict .= '"' . escape(key, '"\') . '": ' . s:JSONStringify(value) . ','
        endfor
        let l:jsonDict = substitute(l:jsonDict, ',$', '', '') . '}'
        return l:jsonDict
    elseif type(a:value) == type('')
        " Handle string
        return '"' . escape(a:value, '"\') . '"'
    elseif type(a:value) == type(0) || type(a:value) == type(0.0)
        " Handle number
        return a:value
    endif
    " Add more type checks if necessary
    return ''
endfunction


"Command to group files by namespace
" Creates a dictionary with filenames as keys and namespaces as values
" Parameters:
" dir: Path to the directory containing the files from ExtractCsFiles command
" Returns:
" None
" Writes the dictionary to a JSON file named openai_group_files_by_namespace_output.json
"command! -nargs=1 GroupFilesByNamespace call GroupFilesByNamespace(<f-args>)
function! GroupFilesByNamespace(dir, ignoreFilename)
    let fileDict = {}
    let namespaceDict = {}
    let processedFiles = []
	let l:outputFile = g:OPENAI_OUTPUT_DIR . '/openai_group_files_by_namespace_output.json'

    for file in split(glob(a:dir . '/*.txt'), '\n')
        if filereadable(file) 
            let filename = fnamemodify(file, ':t:r')  " Get filename without path and extension
            if filename == fnamemodify(a:ignoreFilename, ':t:r')
                continue
            endif
            let content = readfile(file)
            for line in content
                if line =~ '^namespace\s\+\zs\S\+'
                    let namespace = matchstr(line, '^namespace\s\+\zs\S\+')
                    if !has_key(fileDict, namespace)
                        let fileDict[namespace] = []
                    endif
                    if !has_key(namespaceDict, filename)
                        let namespaceDict[filename] = []
                    endif
                    if index(namespaceDict[filename], namespace) == -1
                        call add(namespaceDict[filename], namespace . '.txt')
                    endif
                    call add(fileDict[namespace], content)
                    call add(processedFiles, file)
                    break
                endif
            endfor
        endif
    endfor

    for ns in keys(fileDict)
        call writefile(flatten(fileDict[ns]), a:dir . '/' . ns . '.txt', 'b')
    endfor

    " Encode the dictionary to a JSON string
    let jsonStr = json_encode(namespaceDict)

    " Write the JSON string to the specified output file
    call writefile(split(jsonStr, '\n'), l:outputFile, 'b')

    " Delete the processed files
    for file in processedFiles
        call delete(file)
    endfor
	echo 'Files grouped by namespace'
endfunction

" Given a json file containing a dictionary with filenames as keys and namespaces as values,
" this function returns a list of namespaces for the given file,
" or an empty list if the file is not found in the dictionary
" Parameters:
" file: Path to the file containing the dictionary. filename is key sensitive without extension.
" Returns:
" List of namespaces for the given file
function! GetFileNamespaces(filesJoinNamespacesJsonFile, searchFile)
	let namespaces = []
	if filereadable(a:filesJoinNamespacesJsonFile)
		"json file containing a dictionary with filenames as keys and namespaces as values
		let jsonStr = join(readfile(a:filesJoinNamespacesJsonFile), "\n")
		let jsonObj = json_decode(jsonStr)
		" check if the dictionary has the searchFile key. If it does, return the value
		if has_key(jsonObj, a:searchFile)
			let namespaces = jsonObj[a:searchFile]
		else
			echo 'File not found in the dictionary: ' . a:searchFile
		endif
	endif
	return namespaces
endfunction


" Function to find assigned file ids for a given namespace.
" Parameters:
" filenameArray: array of filenames to search for. These are the namespaces uploaded to OpenAI from GroupFilesByNamespace command.
" jsonFilePath: [Optional] Path to the file containing the JSON response creating cs project assistant. This file contains upload assistant files output.
"Returns:
" List of file ids for the given namespace
" Sample call: echo GetUploadedFileIdByNamespace(['VirtualPaymentService.Controllers.txt'],'')
function! GetUploadedFileIdByNamespace(filenameArray, jsonFilePath)
	"if jsonFilePath is empty, set it to 'openai_upload_assistant_files_output.json'
	if empty(a:jsonFilePath)
		let l:uploadedFiles = g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json'
	else
		let l:uploadedFiles = a:jsonFilePath
	endif
    " Read and decode the JSON file
    let jsonContent = json_decode(join(readfile(l:uploadedFiles), ''))
    
    " Initialize an empty list for matching IDs
    let matchingIds = []

    " Iterate over each object in the JSON array
    for jsonObject in jsonContent
        " Check if the 'filename' is in the given array and 'id' is present
        if has_key(jsonObject, 'filename') && has_key(jsonObject, 'id')
            if index(a:filenameArray, jsonObject['filename']) != -1
                " Add the 'id' to the list of matching IDs
                call add(matchingIds, jsonObject['id'])
            endif
        endif
    endfor

    return matchingIds
endfunction


" Function to find uploaded file ids containing the given classname.
" Parameters:
" classname: Name of the class to search for, case sensitive and without extension.
" groupFilesByNamespaceJsonFile: [Optional] Path to the file containing the JSON response creating cs project assistant. This file contains group files by namespace output.
" Returns: List of file ids for the given classname
" Sample call: echo GetUploadedFileIdByClassname('JITDecisionController','')
function! GetUploadedFileIdByClassname(classname, groupFilesByNamespaceJsonFile)
	"if jsonFilePath is empty, set it to 'openai_upload_assistant_files_output.json'
	if empty(a:groupFilesByNamespaceJsonFile)
		let l:filesByNamespaceJsonFile = g:OPENAI_OUTPUT_DIR . '/openai_group_files_by_namespace_output.json'
	else
		let l:filesByNamespaceJsonFile = a:groupFilesByNamespaceJsonFile
	endif
	"Get matching namespaces for the given classname
	let l:namespaces = GetFileNamespaces(l:filesByNamespaceJsonFile, a:classname)
    function! s:GetAllUploadedFileIds(jsonFilePath)
        " Read and decode the JSON file
        let jsonContent = json_decode(join(readfile(a:jsonFilePath), ''))
        
        " Initialize an empty list for matching IDs
        let matchingIds = []

        " Iterate over each object in the JSON array and add the 'id' to the list of matching IDs
        for jsonObject in jsonContent
            " Check if the 'id' is present and add it to the list of matching IDs
            if has_key(jsonObject, 'id')
                echo "Adding id: " . jsonObject['id'] . " for file: " . jsonObject['filename'] . " to the list"
                call add(matchingIds, jsonObject['id'])
            endif
        endfor

        return matchingIds
    endfunction

	let l:uploadedFiles = g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json'
    let l:mergedFile = g:OPENAI_SOURCE_DIR . '/merged.txt'

	"if no namespaces found, return
	if len(l:namespaces) == 0
		echo 'No namespaces found for classname: ' . a:classname
        echo 'Using all available files'
		let l:uploadedJsonFiles = g:OPENAI_OUTPUT_DIR . '/openai_upload_assistant_files_output.json'
        let l:uploadedFileIds = s:GetAllUploadedFileIds(l:uploadedJsonFiles)
        echo 'A total of ' . len(l:uploadedFileIds) . ' files found in the project assistant'

        " If the number of files is greater than 10, merge them into a single file
        if len(l:uploadedFileIds) > 10
            echo 'Thread files are greater than 10. Using merged file instead...'
            " get the namespace from the merged file by taking the filename from the path
            let l:namespace = fnamemodify(l:mergedFile, ':t')
            echo 'Using namespace: ' . l:namespace
            return GetUploadedFileIdByNamespace([l:namespace], l:uploadedFiles)
        endif
		return l:uploadedFileIds
	endif
	"Get file ids for the given namespaces
	let l:fileIds = GetUploadedFileIdByNamespace(l:namespaces, l:uploadedFiles )
	return l:fileIds
endfunction

" Function to create a thread message for a given class and assign corresponding file ids from uploaded namespace files.
" It creates a new thread or uses an existing thread if one exists for the given class.
" Parameters:
" classNames: List of class names to create threads for
" Returns:
" List of thread ids
" Sample call: echo CreateThreadsByNamespace(['VirtualPaymentService.Controllers'])
" threadMessagePayload = {
"	"classname": "thrd_1GJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5",
"	"message": "Hello, I am a message",
" }
function! CreateAssistantThreadMessageByClassname(threadMessagePayload)
	" get the filename of the thread message payload
	let l:threadMessagePayloadFilename = fnamemodify(a:threadMessagePayload, ':t')
    " Read and decode the JSON file
    let jsonContent = json_decode(join(readfile(a:threadMessagePayload), ''))
	"get the classname from the json payload
	let l:classname = jsonContent['classname']
	"get the message from the json payload
	let l:message = jsonContent['message']
	"get array of uploaded file ids for the given classname
	let l:uploadedFileIds = GetUploadedFileIdByClassname(l:classname, '')

	function! s:ReplaceDotsInFilename(filename, replacementChar)
		" Replace all dots in the filename, except for the last one (file extension)
		let namePart = fnamemodify(a:filename, ':r')
		let extPart = fnamemodify(a:filename, ':e')

		" Replace dots in the name part
		let namePart = substitute(namePart, '\.', a:replacementChar, 'g')

		" Reconstruct the filename
		if len(extPart) > 0
			return namePart . '.' . extPart
		else
			return namePart
		endif
	endfunction


	function! s:GetNamespacesByClassname(classname, groupFilesByNamespaceJsonFile)
        " if classname is empty
        if empty(a:classname)
            let l:namespaceMappingFile = g:OPENAI_OUTPUT_DIR . '/openai_namespace_file_thread_mapping.json'
            let l:namespaces = s:GetUploadedNamespaces(l:namespaceMappingFile)
            echo 'A total of ' . len(l:namespaces) . ' namespaces found in the project assistant'
            return l:namespaces
        endif

		"if jsonFilePath is empty, set it to 'openai_upload_assistant_files_output.json'
		if empty(a:groupFilesByNamespaceJsonFile)
			let l:filesByNamespaceJsonFile = g:OPENAI_OUTPUT_DIR . '/openai_group_files_by_namespace_output.json'
		else
			let l:filesByNamespaceJsonFile = a:groupFilesByNamespaceJsonFile
		endif
		"Get matching namespaces for the given classname
		let l:namespaces = GetFileNamespaces(l:filesByNamespaceJsonFile, a:classname)
		return l:namespaces
	endfunction

    function! s:GetUploadedNamespaces(jsonFilePath)
        " Read the JSON file into a Dictionary and return all the keys as a list
        let jsonContent = json_decode(join(readfile(a:jsonFilePath), ''))
        return keys(jsonContent)
    endfunction

	" Get array of namespaces for the given classname and create a thread for each namespace
	let l:classNamespaces = s:GetNamespacesByClassname(l:classname, '')

	" Create a thread for each namespace
	for namespace in l:classNamespaces
		" Get a thread id for the given namespace
		"echo 'Creating thread for namespace: ' . namespace
		let l:threadId = GetThreadIdByNamespace(namespace, '')
		let l:threadMessageRequest = ThreadMessageJsonPayload(l:threadId, l:message, l:uploadedFileIds)
		let l:messageFilename = fnamemodify(namespace, ':t:r') . '.' . l:threadMessagePayloadFilename
		let l:threadMessageOutputFile = s:ReplaceDotsInFilename(l:messageFilename, '_')
		"echo 'threadMessageOutputFile: ' . l:threadMessageOutputFile
		call CreateThreadMessage(l:threadMessageRequest, l:threadMessageOutputFile)
	endfor
endfunction

" command to create a thread message for a given class and assign corresponding file ids from uploaded namespace files.
" It creates a new thread or uses an existing thread if one exists for the given class.
" Parameters:
" json file containing request payload. Classname is case-sensitive and without extension. Structure:
" {
"	"classname": "thrd_1GJ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5",
"	"message": "Hello, I am a message",
" }
" Returns:
" A message object inside the 'requests' folder.
" This object should contain the thread_id, file_ids, and the message id - amonth other things.
" Sample call: :OpenAICreateMessage VirtualPaymentService.Controllers
command! -nargs=1 OpenAICreateThreadMessage call CreateAssistantThreadMessageByClassname(<f-args>)

" Creates a JSON string where each key is a filename and the value is an empty string
" returns a json string
function! CreateJsonWithEmptyValues(fileDict)
    " Initialize a new dictionary for the JSON object
    let jsonDict = {}

    " Iterate over each key-value pair in the input dictionary
    for [key, _] in items(a:fileDict)
        " Use the key (filename) with an empty string as the value
        let jsonDict[key] = ''
    endfor

    " Encode the dictionary to a JSON string
    let jsonString = json_encode(jsonDict)

    " Return the JSON string
    return jsonString
endfunction



function! GetThreadIdByNamespace(namespace, namespaceJoinThreadJsonFile)
	" if namespaceJoinThreadJsonFile is empty, set it to 'openai_namespace_join_thread_mapping.json'
	let l:namespaceThreadJsonFile = a:namespaceJoinThreadJsonFile
	if empty(a:namespaceJoinThreadJsonFile)
		let l:namespaceThreadJsonFile = g:OPENAI_OUTPUT_DIR . '/openai_namespace_file_thread_mapping.json'
	endif
    " Check if the file exists
    if !filereadable(l:namespaceThreadJsonFile)
        echoerr 'File not found: ' . l:namespaceThreadJsonFile
        return ''
    endif

    " Load the file and decode the JSON string to a dictionary
    let l:threadsJson = json_decode(join(readfile(l:namespaceThreadJsonFile), "\n"))

    " Check if the namespace is in the dictionary and its value is not null or empty
    if has_key(l:threadsJson, a:namespace) && l:threadsJson[a:namespace] != ''
        " Return the thread ID for the existing namespace
        echo 'Thread ID found for namespace: ' . a:namespace
        let l:threadId = l:threadsJson[a:namespace]
    else
        " Create a new thread for the given namespace
        let l:threadJsonString = CreateOpenAIAssistantThread()
        " Parse thread JSON string to a dictionary and get the thread ID
        let l:threadId = json_decode(l:threadJsonString)['id']
        " Add the new namespace and thread ID to the map
        let l:threadsJson[a:namespace] = l:threadId
        let l:jsonPayload = CreateJSONPayload(l:threadsJson) 
        "overwrite the file with the updated map
        call writefile(split(l:jsonPayload, '\n'), l:namespaceThreadJsonFile, 'b')
    endif

    return l:threadId
endfunction

function! ExecuteThreadMessage(messageJson)
	let l:outputfile = g:OPENAI_RESPONSES_DIR . '/openai_thread_message_output.json'
	" load project assistant output file to retrieve the assistant id
	let l:projectAssistantOutputFile = g:OPENAI_OUTPUT_DIR . '/openai_create_project_assistant_output.json'
	let l:projectAssistantOutput = join(readfile(l:projectAssistantOutputFile), "\n")
	let l:projectAssistantId = json_decode(l:projectAssistantOutput)['id']

	" load messageJson file to retrieve the thread id
	let l:threadId = json_decode(join(readfile(a:messageJson), "\n"))['thread_id']

	" create a json payload with the thread id
	let l:threadRunPayload = {"assistant_id": l:projectAssistantId, "additional_instructions": "When asked to perform a code review, rewrite, or generate a piece of code, the assistant shall generate request code and return file_ids to download all generated code files. Please do not simply return links to generated files. "}
	let l:threadRunJson = CreateJSONPayload(l:threadRunPayload)
	" stringfy the json payload
    let l:encodedThreadRunJson = json_encode(l:threadRunJson)

	let l:cmd = g:OPENAI_ASSISTANT_THREAD_RUN_CMD
	let l:response = substitute(l:cmd,'{thread_id}',l:threadId,'')
	let l:curl_cmd = l:response. l:encodedThreadRunJson . ' -o ' . l:outputfile
    execute '!' . l:curl_cmd

    " Get execution status
    let l:threadRunStatus = GetThreadRunStatus(l:outputfile)
    echo 'Thread run status: ' . l:threadRunStatus

endfunction

" command to execute a thread message.
" Parameters:
" json file containing the generated thread message from "OpenAICreateMessage" command.
" returns:
" A json response inside the 'responses' directory. The file contains a collection of thread messages on the thread.
" Sample call: :OpenAIExecuteThreadMessage requests/namespace_file_thread_message.json
command! -nargs=1 OpenAIExecuteThreadMessage call ExecuteThreadMessage(<f-args>)

function! GetThreadRunStatus(threadExecusionJson)
	let l:threadRunStatus = json_decode(join(readfile(a:threadExecusionJson), "\n"))['status']
	" get thread_id and run_id
	let l:threadId = json_decode(join(readfile(a:threadExecusionJson), "\n"))['thread_id']
	let l:runId = json_decode(join(readfile(a:threadExecusionJson), "\n"))['id']

	" retrieve thread run status until it is completed
	while l:threadRunStatus != 'expired' && l:threadRunStatus != 'completed'
		echo 'Thread run status: ' . l:threadRunStatus
		let l:tmp_out = g:OPENAI_RESPONSES_DIR . '/thread_run.json'
		call RetrieveOpenAIAssistantThreadRun(l:threadId, l:runId, l:tmp_out)
		let l:threadRunStatus = json_decode(join(readfile(l:tmp_out), "\n"))['status']
        " Sleep for 5 second
        call timer_start(5000, {-> v:null})
	endwhile

	call OpenAIAssistantThreadListMessages(l:threadId, g:OPENAI_RESPONSES_DIR .  '/' . l:threadId .'_execution_responses.json')
	return l:threadRunStatus
endfunction

function! CreateProjectAssistantWorkingDir(projectPath, projectHomeDirectory, directoriesToPreserve)
	let l:outputDir = fnamemodify(a:projectPath, ':t') .  '_project'
	let l:defaultHome = fnamemodify(g:OPENAI_WORKING_DIR, ':p')
	let l:projectDir = l:defaultHome. l:outputDir
	"if projectHomeDirectory is empty, set it to the project directory
	if empty(a:projectHomeDirectory)
		"call delete (l:projectDir, 'rf')
		"let l:projectHomeDirectory = l:projectDir
		"call mkdir(l:projectHomeDirectory, 'p')
        call PreserveAndRecreate(l:projectDir, a:directoriesToPreserve)
		let l:projectHomeDirectory = l:projectDir
	else
		" if directory doesn't exist, create it
		if !isdirectory(l:projectHomeDirectory)
			call mkdir(l:projectHomeDirectory, 'p')
		else
			" if directory exists, delete it and create a new one
			"call delete (l:projectHomeDirectory, 'rf')
			"call mkdir(l:projectHomeDirectory, 'p')
            call PreserveAndRecreate(l:projectHomeDirectory, a:directoriesToPreserve)
		endif
	endif

	" create required subdirectories
	let l:requestsDir = l:projectHomeDirectory . '/requests'
	let l:responsesDir = l:projectHomeDirectory . '/responses'
	let l:outputDir = l:projectHomeDirectory . '/output'
	let l:sourceDir = l:projectHomeDirectory . '/source'

    if !isdirectory(l:requestsDir)
        call mkdir(l:requestsDir, 'p')
    endif
	call mkdir(l:responsesDir, 'p')
	call mkdir(l:outputDir, 'p')
	call mkdir(l:sourceDir, 'p')

	" set global variables for the project home directory and requests and responses directories
	let g:OPENAI_PROJECT_HOME_DIR = l:projectHomeDirectory
	let g:OPENAI_REQUESTS_DIR = l:requestsDir
	let g:OPENAI_RESPONSES_DIR = l:responsesDir
	let g:OPENAI_OUTPUT_DIR = l:outputDir
	let g:OPENAI_SOURCE_DIR = l:sourceDir
endfunction

function! PreserveAndRecreate(dir, preserveDirs)
    let tempDir = $HOME . '/temp_preserve_dir' 
    call mkdir(tempDir, 'p')

    " Step 1: Copy contents of preserveDirs to tempDir
    for preserveDir in a:preserveDirs
        let preserveDirPath = a:dir . '/' . preserveDir
        if isdirectory(preserveDirPath)
            execute 'silent !cp -r ' . shellescape(preserveDirPath) . ' ' . tempDir
        endif
    endfor

    " Step 2: Delete contents of dir
    call delete(a:dir, 'rf')

    " Step 3: Recreate directory and restore preserved contents
    call mkdir(a:dir, 'p')
    for preserveDir in a:preserveDirs
        let preserveDirPath = tempDir . '/' . preserveDir
        if isdirectory(preserveDirPath)
            execute 'silent !cp -r ' . shellescape(preserveDirPath) . ' ' . shellescape(a:dir)
        endif
    endfor

    " Step 4: Delete tempDir
    call delete(tempDir, 'rf')
endfunction




function! SetupProjectAssistant(projectPath, directoryToPreserve)
	" create a working directory for the project
	call CreateProjectAssistantWorkingDir(a:projectPath, '', a:directoryToPreserve)

    " create a json file with the project path and the source directory
    let l:projectAssistantJson = {"project_path": fnamemodify(a:projectPath, ':p')}
    " stringfy the json payload
    let l:encodedProjectAssistantJson = json_encode(l:projectAssistantJson)
    " write the json string to output file
    let l:projectAssistantOutputFile = g:OPENAI_OUTPUT_DIR . '/openai_project_assistant_path_output.json'
    call writefile(split(l:encodedProjectAssistantJson, '\n'), l:projectAssistantOutputFile, 'b')

	call ListFilesWithExtensions(a:projectPath, {'cs': 'txt'}, g:OPENAI_SOURCE_DIR)

    let l:mergedFile = g:OPENAI_SOURCE_DIR . '/merged.txt'
    call MergeFiles(g:OPENAI_SOURCE_DIR, l:mergedFile)
	call GroupFilesByNamespace(g:OPENAI_SOURCE_DIR, l:mergedFile)
	let l:assistantName = fnamemodify(g:OPENAI_PROJECT_HOME_DIR, ':t') . '_assistant'
	let l:assistantResponse = CreateOpenAICsProjectAssistant(g:OPENAI_SOURCE_DIR, l:assistantName)

    function! s:CreateSampleJsonMessageRequest(dir)
        let l:sampleclassname = 'Classname.without.namespace'
        let l:samplemessage = 'Hello, I am a message'

        let l:sampleJsonMessageRequest = '{' . "\n" .
           \ '"classname": "' . l:sampleclassname . '",' . "\n" .
           \ '"message": "' . l:samplemessage . '"' . "\n" .
           \ '}'
        " write the json string to g:OPENAI_REQUESTS_DIR
        let l:sampleJsonMessagRequestFile = g:OPENAI_REQUESTS_DIR . '/sample_json_message_request.json'
        call writefile(split(l:sampleJsonMessageRequest, '\n'), l:sampleJsonMessagRequestFile, 'b')
    endfunction

	" create a sample json message request
	call s:CreateSampleJsonMessageRequest(g:OPENAI_REQUESTS_DIR)

	return g:OPENAI_PROJECT_HOME_DIR
endfunction

" Function to merge files in a directory into a single file.
" Parameters:
" dir: The directory containing the files to merge.
" outputfile: The file to write the merged contents to.
" Returns:
" None
" Sample call: call MergeFiles('C:\Users\user\Documents\OpenAI\OpenAIAssistant\requests', 'C:\Users\user\Documents\OpenAI\OpenAIAssistant\requests\merged.txt')
function! MergeFiles(dir, outputfile)
    echo 'Merging files in directory: ' . a:dir. ' into file: ' . a:outputfile
    let l:filePaths = split(glob(a:dir . '/*'), "\n")
    echo 'Files to merge: ' . join(l:filePaths, ', ')
    let l:mergedFileContents = []

    for l:file in l:filePaths
        if filereadable(l:file)
            let l:fileContents = readfile(l:file)
            let l:mergedFileContents += l:fileContents + ['']
        endif
    endfor

    call writefile(l:mergedFileContents, a:outputfile, 'b')
endfunction

" command to setup a project assistant.
" Parameters:
" projectPath: The path to the project to setup.
" returns:
" The project home directory.
command! -nargs=1 OpenAISetupProject call SetupProjectAssistant(<f-args>, [])

" Function to determine if directory paths are related.
" Parameters:
" path1: The first path to compare.
" path2: The second path to compare.
" Returns:
" 0 if the paths are not related.
" 1 if path1 is a subdirectory of path2.
" 2 if path2 is a subdirectory of path1.
function! ComparePaths(path1, path2)
    " Replace backslashes with forward slashes and remove trailing slashes
    let normalizedPath1 = substitute(a:path1, '\\', '/', 'g')
    let normalizedPath2 = substitute(a:path2, '\\', '/', 'g')

    let normalizedPath1 = substitute(normalizedPath1, '/\+$', '', '')
    let normalizedPath2 = substitute(normalizedPath2, '/\+$', '', '')

    " use stridx to check if normalizedPath1 is a substring of normalizedPath2
    if stridx(normalizedPath2, normalizedPath1) == 0
        " path2 is a subdirectory of path1
        return 2
    elseif stridx(normalizedPath1, normalizedPath2) == 0
        " path1 is a subdirectory of path2
        return 1
    else
        " path1 and path2 are not related
        return 0
    endif

endfunction

" Function to allow the user to select a project directory and set it as the working project.
" Parameters:
" projectPath: The path to the project to set as the working project.
" Returns:
" The project home directory.
function! SetWorkingProject(projectPath)
    " Check that the project setup is complete
    if !ProjectSetupComplete(a:projectPath)
        echo 'Project setup is not complete. Please run :OpenAIProjectSetup'
        return ''
    endif

    let l:projectHomeDirectory = fnamemodify(a:projectPath, ':p')
	let l:defaultHome = fnamemodify(g:OPENAI_WORKING_DIR, ':p')
    let l:comparePaths = ComparePaths(l:projectHomeDirectory, l:defaultHome)

    " Make sure that l:projectHomeDirectory is inside g:OPENAI_WORKING_DIR
    " If not, return an error message
    if l:comparePaths != 1
        echoerr 'A working project directory must be inside the current working directory: ' . g:OPENAI_WORKING_DIR
        return ''
    else
        " if directory doesn't exist, create it
        if !isdirectory(l:projectHomeDirectory)
            echoerr "Project not setup. Please run :OpenAISetupProject to setup the project"
            return ''
        endif
    endif
    "
	let l:requestsDir = l:projectHomeDirectory . '/requests'
	let l:responsesDir = l:projectHomeDirectory . '/responses'
	let l:outputDir = l:projectHomeDirectory . '/output'
	let l:sourceDir = l:projectHomeDirectory . '/source'

	" set global variables for the project home directory and requests and responses directories
	let g:OPENAI_PROJECT_HOME_DIR = l:projectHomeDirectory
	let g:OPENAI_REQUESTS_DIR = l:requestsDir
	let g:OPENAI_RESPONSES_DIR = l:responsesDir
	let g:OPENAI_OUTPUT_DIR = l:outputDir
	let g:OPENAI_SOURCE_DIR = l:sourceDir

    echo 'Working Project set to: ' . g:OPENAI_PROJECT_HOME_DIR
endfunction
 command! -nargs=1 OpenAISetWorkingProject call SetWorkingProject(<f-args>)

function! GetWorkingProject()
    " if g:OPENAI_PROJECT_HOME_DIR is empty, return an error message
    if empty(g:OPENAI_PROJECT_HOME_DIR)
        echo 'No working project set. Please run :OpenAISetWorkingProject'
        return ''
    endif

    if !isdirectory(g:OPENAI_PROJECT_HOME_DIR)
        echo 'Working project directory does not exist. Please run :OpenAISetupProject'
        return ''
    endif

    echo 'Working Project: ' . g:OPENAI_PROJECT_HOME_DIR
endfunction

command! -nargs=0 OpenAIGetWorkingProject call GetWorkingProject()

function! ProjectSetupComplete(projectPath)
    " check that the project path is a directory with the following subdirectories:
    " requests, responses, output, source
    " if any of the subdirectories is missing, return false
    " if all subdirectories exist, return true
    if !isdirectory(a:projectPath)
        return 0
    endif
    let l:projectHomeDir = fnamemodify(a:projectPath, ':p')
    let l:requestsDir = l:projectHomeDir . '/requests'
    let l:responsesDir = l:projectHomeDir . '/responses'
    let l:outputDir = l:projectHomeDir . '/output'
    let l:sourceDir = l:projectHomeDir . '/source'

    if !isdirectory(l:requestsDir) || !isdirectory(l:responsesDir) || !isdirectory(l:outputDir) || !isdirectory(l:sourceDir)
        return 0
    endif

    return 1
endfunction

command! -nargs=0 OpenAIGetHomeDirectory echo fnamemodify(g:OPENAI_WORKING_DIR, ':p')

function! SetHomeDirectory(homeDirectory)
    let l:projectHomeDir = fnamemodify(a:homeDirectory, ':p')
    " check that the home directory exists
    if !isdirectory(l:projectHomeDir)
        " create the directory
        echo 'Creating directory: ' . l:projectHomeDir
        call mkdir(l:projectHomeDir, 'p')
    endif

    let g:OPENAI_WORKING_DIR = l:projectHomeDir
    echo 'Home directory set to: ' . g:OPENAI_WORKING_DIR

    " clear project's directories
     let g:OPENAI_PROJECT_HOME_DIR = ''
     let g:OPENAI_REQUESTS_DIR = ''
     let g:OPENAI_RESPONSES_DIR = ''
     let g:OPENAI_OUTPUT_DIR = ''
     let g:OPENAI_SOURCE_DIR = ''
    "
endfunction

command! -nargs=1 OpenAISetHomeDirectory call SetHomeDirectory(<f-args>)


function! RetrieveFileById(id)
    let l:cmd = substitute(g:OPENAI_RETRIEVE_FILE_CMD, '{file_id}', a:id, 'g')
    let l:curl_cmd = l:cmd . ' | clip'
    execute '!' . l:curl_cmd
endfunction

" comand to copy file content to clipboard
command! -nargs=1 OpenAIRetrieveFileContentById call RetrieveFileById(<f-args>)

function! ReloadWorkingProject()
    " get current working project
    let l:projectHomeDir = g:OPENAI_PROJECT_HOME_DIR
    " open /output/openai_create_project_assistant_output.json and parse out the id property
    let l:outputFile = l:projectHomeDir . '/output/openai_create_project_assistant_output.json'
    let l:outputJson = join(readfile(l:outputFile), "\n")
    let l:outputJsonObject = json_decode(l:outputJson)
    let l:assistantId = l:outputJsonObject['id']

    echo 'Deleting project assistant: ' . l:assistantId
    call DeleteOpenAIAssistant(l:assistantId)

    " Delete all uploaded files by the project assistant
    call DeleteCsProjectUploadedAssistantFiles('')

    " Get project to reload
    let l:projectReload = g:OPENAI_OUTPUT_DIR . '/openai_project_assistant_path_output.json'
    let l:outputJson = join(readfile(l:projectReload), "\n")
    " Parse the file and return the project_path value
    let l:projectFileObj = json_decode(l:outputJson)
    let l:projectPath = l:projectFileObj['project_path']
    let l:dirName = GetDirectoryName(l:projectPath)
    echo 'Project name: ' . l:dirName
    "Create the project assistant but retain the contents of the requests directory
    call SetupProjectAssistant(l:dirName, ['requests'])
    " Open the working project in a vertical split
    execute 'vsplit ' . l:projectHomeDir
    "
endfunction

command! -nargs=0 OpenAIReloadWorkingProject call ReloadWorkingProject()

function! GetDirectoryName(dir)
    let l:projectAssistantJson = fnamemodify(a:dir, ':p')
    let l:normalizedPath = substitute(l:projectAssistantJson, '\\', '/', 'g')
    let l:trimmedPath = substitute(l:normalizedPath, '/\+$', '', '')
    return l:trimmedPath
endfunction
