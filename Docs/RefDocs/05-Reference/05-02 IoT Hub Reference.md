# OBD Reference

The myDriving phone app retrieves the following data from the OBD device if it is present in the vehicle. Note that some of the OBD fields may be empty if the specific vehicle doesn’t support them: 


| Name | Comments |
| ---- | -------- |
| el   | PID=04 (EngineLoad)  |
| stfb | PID=06 (ShortTermFuelBank1)  |
| ltfb | PID=07 (LongTermFuelBank1)  |
| rpm  | PID=0C (EngineRPM)  |
| spd  | PID=0D (Speed)  |
| fr   | PID=10 (MAFFlowRate)  |
| tp   | PID=11 (ThrottlePosition)  |
| rt   | PID=1F (Runtime)  |
| dis  | PID=21 (DistancewithMIL)  |
| rtp  | PID=45 (RelativeThrottlePosition)  |
| ot   | PID=46 (OutsideTemperature)  |
| efr  | PID=5E (EngineFuelRate)  |
| vin  | VIN number  |


# IoT Hub Reference

The myDriving phone app sends telemetry messages to the IoT Hub serialized as JSON. These JSON messages contain the following data:

A **Trip** message encapsulates all the data associated with a specific trip recorded by the myDriving phone app. A **Trip** object has the following fields:

| Field | Description |
| ----- | ----------- |
| Id                | A unique identifier for the trip |
| Name              | A name for the trip as chosen by the user |
| UserId            | The User Id |
| Points            | A list of **TripPoint** data points (see below) |
| RecordedTimeStamp | The time the trip recording started |
| EndTimeStamp      | The time the trip recording stopped |
| Rating            | The driver skills rating for the trip |
| IsComplete        | Specifies whether the trip is complete |
| AverageSpeed      | AverageSpeed for the trip |
| FuelUsed          | Fuel usage for the trip |
| HardStops         | Number of hard stops during the trip |
| HardAccelerations | Number of hard accelerations during the trip |
| MainPhotoUrl      | URL os the main photo associated with the trip |
| Distance          | The total distance covered during the trip |

Each **TripPoint** object has the following fields:

| Field | Description |
| ----- | ----------- |
| Id                           | A unique identifier for the trip point |
| TripId                       | The unique identifier of the trip (see previous table) |
| Latitiude                    | Current latitude reported by phone's GPS |
| Longitude                    | Current longitude reported by phone's GPS |
| Speed                        | Current speed reported by OBD |
| RecordedTimeStamp            | Time the trip point was recorded |
| Sequence                     | Sequence number for trip point |
| RPM                          | Current engine RPM reported by OBD |
| ShortTermFuelBank            | Current short-term fuel trim reported by OBD |
| LongTermFuelBank             | Current long-term fuel trim reported by OBD |
| ThrottlePosition             | Current throttle position reported by OBD |
| RelativeThrottlePosition     | Current relative throttle position reported by OBD |
| Runtime                      | The length of time the engine has been running |
| DistanceWithMalfunctionLight | The distance travelled since the malfunction indicator light switched on |
| EngineLoad                   | The current engine load |
| MassFlowRate                 | The current air flow rate from mass air flow sensor |
| OutsideTemperature           | The external temperature |
| EngineFuelRate               | The current engine fuel rate |
| VIN                          | The unique vehicle identification number (with identity bits removed) |
| HasOBDData                   | Flag indicating whether the trip point has OBD data |