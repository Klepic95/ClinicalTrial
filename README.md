The Clinical Trial Data Processor is a system that processes clinical trial data in JSON format. It validates the input data against a predefined schema, transforms it into the appropriate data models, and stores the processed data into a database using Entity Framework Core. The application ensures that clinical trial records conform to the required business rules and database schema before storage.

How to successfully configure the project locally:

Clone this repository, 
Install and configure Docker locally (Link for download: https://www.docker.com/products/docker-desktop/)

If you want to build and run application and create ms sql database using Docker, please use PowerShell and go in the root Directory of the project (Where both Dockerfile and ClinicalTrial.sln are located), and run following command: 
"**docker-compose -f docker-compose.yml up --build**". 

After running this command, please check if the containers are running in Docker Desktop app - If the API container is not running, please run it via Docker Desktop app.
Now you can access the Swagger page of the application following this link, and also you can create requests via swagger UI: "http://localhost:8080/swagger/index.html"

JSON file for testing application can be find in the root of the project and it is called: "testingFile.json". User can use this file for testing, while changing parameters inside it to achieve different results.
Database and the table is automatically created once Powershell script is done.

It is necessary to follow these steps in order to sucessfully run application locally!

If you run into any issue, do not hessitate to contact me via email: Klepicpredrag@gmail.com
