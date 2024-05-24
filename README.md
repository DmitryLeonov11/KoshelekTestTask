## API Documentation
The API server has a web page with documentation that is automatically generated using the Swagger framework for the RESTful API specification. This web page is 
available by default at `http://localhost:8082/swagger/index.html`

## Logging
Logging is performed inside the Web API using the Serilog library and the Seq application, which is used for convenient storage and 
search of structural logs. This web page is available by default at `http://localhost:5341/#/events`


## Application launch
To launch the app, you need:
1.  Download and install Docker
2.  Download the app repository and go to the main folder of the downloaded repository using Command Prompt or PowerShell
3.  Enter the command: `docker-compose up`
