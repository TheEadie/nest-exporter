> [!WARNING]  
> This project is no longer maintained. I no longer have a Nest Themostat so I'm unable to test changes.

# Nest Exporter

Nest Exporter generates prometheus metrics for the Nest Thermostat using the Google Device Access API.

The following metrics are exported

| Metric                              | Description                                                    |
| ----------------------------------- | -------------------------------------------------------------- |
| nest_thermostat_connection_status   | 0 if the thermostat is offline, 1 if it is online              |
| nest_thermostat_status              | 0 if the heating is off, 1 if it is on                         |
| nest_thermostat_eco_mode            | 0 if the thermostat is not in eco mode, 1 if it is in eco mode |
| nest_thermostat_actual_temperature  | The actual temperature in the room (C)                         |
| nest_thermostat_target_temperature  | The target temperature for the room (C)                        |
| nest_thermostat_humidity            | The humidity in the room (%)                                   |
| nest_outside_actual_temperature     | The actual temperature outside (C)                             |
| nest_outside_feels_like_temperature | The temperature it feels like outside (C)                      |
| nest_outside_humidity               | The humidity outside (%))                                      |

## How to use

### Set up

#### Nest API

-   Sign up for the Google Device Access program. There is a a $5 charge
-   Follow the getting started guide [here](https://developers.google.com/nest/device-access/get-started)
-   Once you have tested the API and can see data using `curl` store the following as environment variables

```
export NestExporter_NestApi__ClientId=
export NestExporter_NestApi__ClientSecret=
export NestExporter_NestApi__ProjectId=
export NestExporter_NestApi__RefreshToken=
```

#### Open Weather API

-   Sign up for the free version of Open Weather API [here](https://openweathermap.org/price)
-   Generate an API Key
-   Once you have tested the API and can see data using `curl` store the following as environment variables

```
export NestExporter_OpenWeatherApi__ApiKey=
export NestExporter_OpenWeatherApi__Longitude=
export NestExporter_OpenWeatherApi__Latitude=
```

### Running

#### Docker

-   Run the following:

```
docker run --rm -i -p 5005:80 \
 --env NestExporter_NestApi__ClientId \
 --env NestExporter_NestApi__ClientSecret \
 --env NestExporter_NestApi__ProjectId \
 --env NestExporter_NestApi__RefreshToken \
 --env NestExporter_OpenWeatherApi__ApiKey \
 --env NestExporter_OpenWeatherApi__Longitude \
 --env NestExporter_OpenWeatherApi__Latitude \
 theeadie/nest-exporter
```

-   Visit http://localhost:5005 to confirm metrics are being recieved

#### Local

-   Clone this repo
-   Run `make start`
-   Visit http://localhost:5005 to confirm metrics are being recieved
-   Run `make stop` to shut down the service
