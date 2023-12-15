# VirtualPaymentService #

The VirtualPaymentService (VPS) handles the communication with our virtual card provider Marqeta, manages virtual card operations, and persists information to database/s. Most calls to VPS originate from the [VirtualPaymentOrchestrator (VPO)](https://bitbucket.org/progfin-ondemand/virtualpaymentorchestrator/src/master/) that may call 1-n number of endpoints.

### CircleCI ###
All pushes to any branch of the VirtualPaymentService Git repository on the Bitbucket server will run a CI job in CircleCI for that branch.  The CircleCI server will automatically create and run a new CI Job for each branch created, no additional setup is required.  

The status and history of the CircleCI job for the service can be found here [CircleCI VirtualPaymentService CI Job](https://app.circleci.com/pipelines/bitbucket/progfin-ondemand/virtualpaymentservice)

### Octopus Deploy ###
All deployments (Releases) to any branch after CircleCI has created them, will be available to deploy on Octopus Deploy here [Octopus Deploy VirtualPaymentService Releases](https://progleasing.octopus.app/app#/Spaces-42/projects/virtualpaymentservice/deployments/releases) 

### Code Quality ###
Code quality is measured using SonarQube, you can view the current quality status here [VirtualPaymentService Code Quality Measures](https://sonarui.api.progleasing.com/dashboard?id=virtualpaymentservice)

### Health Check ###
The health of the service can be obtained using the /health endpoint.  The following are the health check endpoints in each environment:

QA      - [https://vdc-qaswebapp16.stormwind.local/VirtualPaymentService/health](https://vdc-qaswebapp16.stormwind.local/VirtualPaymentService/health)  
RC      - [https://slc-rcpwebvdo.stormwind.local/VirtualPaymentService/health](https://slc-rcpwebvdo.stormwind.local/VirtualPaymentService/health)  
Demo    - [https://slc-dmowebvdo.stormwind.local/VirtualPaymentService/health](https://slc-dmowebvdo.stormwind.local/VirtualPaymentService/health)  
Prod    - [https://slc-prdwebvdo.stormwind.local/VirtualPaymentService/health](https://slc-prdwebvdo.stormwind.local/VirtualPaymentService/health)  
Prod DR - [https://phx-prdwebvdo.stormwind.local/VirtualPaymentService/health](https://phx-prdwebvdo.stormwind.local/VirtualPaymentService/health)  

### API Contract Definitions ###
The available APIs provided in this service are documented in the Swagger page of the service.  The Swagger page includes the request/response contracts, schemas and typical HTTP response codes to expect for the most common success/failure responses.  Please review the page for details on how to call the endpoints and to try them out (in lower lanes of course).

Swagger pages for all environments:  
QA      - [https://vdc-qaswebapp16.stormwind.local/VirtualPaymentService/swagger](https://vdc-qaswebapp16.stormwind.local/VirtualPaymentService/swagger)  
RC      - [https://slc-rcpwebvdo.stormwind.local/VirtualPaymentService/swagger](https://slc-rcpwebvdo.stormwind.local/VirtualPaymentService/swagger)  
Demo    - [https://slc-dmowebvdo.stormwind.local/VirtualPaymentService/swagger](https://slc-dmowebvdo.stormwind.local/VirtualPaymentService/swagger)  
Prod    - [https://slc-prdwebvdo.stormwind.local/VirtualPaymentService/swagger](https://slc-prdwebvdo.stormwind.local/VirtualPaymentService/swagger)  
Prod DR - [https://phx-prdwebvdo.stormwind.local/VirtualPaymentService/swagger](https://phx-prdwebvdo.stormwind.local/VirtualPaymentService/swagger)  

### Configuration ###
The service configuration is contained in the appsettings.json file.  The version stored here in the VirtualPaymentService Git repository is used locally by developers.

The configuration to be replaced for other environments can be located in the Variables Section of Octopus Deploy (Select the branch on the left side of the page) [VirtualPaymentService Variables](https://progleasing.octopus.app/app#/Spaces-42/projects/virtualpaymentservice/branches/refs%2Fheads%2Fmaster/variables)

#### Secret Server Config Items ####
In the appsettings.config there are some settings that are required to properly communicate with the Secret Server:  

**AppSettings.SecretServerKeyNames** Section - Must contain the **MarqetaApiUserKeyName** and **MarqetaApiPasswordKeyName** string properties with the values being the key names present in Thycotic that contain the secret values.  

**SecretManagement** Section - Must contain the settings required by the [progleasing.platform.secretconfiguration](https://bitbucket.org/progfin-ondemand/progleasing.platform.secretconfiguration/src/master/) library. Please refer to the repo and Confluence pages for required settings.  

**AppSettings.SecretServerGetSecretRetryLimit** Property (int) - Optional setting to add the number of retry operations when retrieving a secret using the library. If the property is not present then no retries will occur.  If present, the operation will be retried this number of times after the initial execution fails.





