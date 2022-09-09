**How to use Meter Readings Uploader**

- Running the project in Debug mode will bring up the Swagger UI which shows the expected inputs for each endpoint.
- To upload a file, please use the POST /meter-reading-uploads endpoint and attach a CSV file in the required format which is like in the below example:

>AccountId,MeterReadingDateTime,MeterReadValue,
>
>2344,22/04/2019 09:24,01002,

- Accepted values:
  - Account ID must exist (view available accounts in GET /accounts)
  - Account ID must be an integer
  - MeterReadingDateTime must be in the format dd/MM/yyyy HH:mm
  - MeterReadingDateTime must be unique for that account ID, and more recent than the last submitted reading
  - MeterReadValue must be in the format NNNNN where N is a digit i.e. 00001 not 1
- Any rows which have invalid data will not be processed and will be added to the Failed count.
- Any rows which are valid will be added to the internal SQLite database and will be added to the Succeeded count.
- Once the upload is completed, the request will return with a result in the format:
>{
>
>  "Succeeded": 1,
>
>  "Failed": 1
>
>}

- To view all uploaded meter readings, you can query the GET endpoints on /meter-reading-uploads
  - GET /meter-reading-uploads will return all uploads for all accounts
  - GET /meter-reading-uploads/{id} will return all uploads for a given account ID


**Configuring the database**
- You may configure the expected name of the SQLite database via appsettings.json.
- The database will be created if it does not already exist when the program is first run.

**Seeding the database**
- The database is seeded from a CSV file, you can configure the filename for this in appsettings.json.
- This file is included in the source code.
- This will happen when the program is first run, but will not add to the database if any existing account data is found in the database.
