# CARE Samples



## Sample Project

Currently, you can find a sample API project (in .NET Core 3.1 Framework) that uses the CARE library to handle:

- Input-Validation errors,
- Application exceptions and,
- Unhandled exceptions.

The sample is based on the default `WeatherForecast` example (when creating an API). The structure of the sample project is organized as follows:

- /**Controllers/WeatherForecastController.cs:** The API controller that handles the requests. A  dummy`POST` endpoint has been included to create the  `WeatherForecast` resource. It's recommended to create thin API Controllers, containing only the intended application code logic calls. This is just an example to show how the `ConsistentApiResponseErrors` library handle the aforementioned input-validation errors and exceptions.
- **/DTOs/:** Containing the data transfer object (DTOs) and the relative validations for the request DTO.
- **/Exceptions/:** Containing  the application exceptions classes, which maps the exception with the HTTP error code and message.



## Using the Sample Project:

To use the sample project just start debugging (F5) and your default browser will open. As in the default `WeatherForecast` example, a list of forecasts is returned.

To evaluate how the CARE library handles Input-Validation errors and Exception:

1. Install and run the [Postman](https://www.postman.com/downloads/) app.
2. Import the sample collection file (`CARE_Sample.postman_collection.json`)  in Postman > Import > File.



The provided collection contains the following POST requests:

1. Create a Weather Forecast (Successfully): Returns the new resource.
2. Create a Weather Forecast (Validation Error): An HTTP 400 - Bad Request error is returned.
3. Create a Weather Forecast (Application Exception): The defined HTTP error code is returned (409 - Conflict, for this example).
4. Create a Weather Forecast (Unhandled Exception):  A HTTP 500 - Internal Server Error, is returned for unhandled exceptions.

