
# Context

A Truck Plan describes a single driver driving a truck for a continuous period. For example; a five
hour drive through Germany on a specific date. A driver is a person with a name, birthdate, etc.

Each truck has a GPS device installed. This device provides the system with the current truck
position approximately every 5 minutes.

## 1. Design and implement a model for representing the domain.

The domain is basically a `TruckPlan`. It contains the information of the driver, `start` and `stop` and the expected duration.

`start` and `stop` are of type `Location`, which contains the `Latitude` and `Longitude`. It is used to represent a `GPS` location

Lastly, I create a `TrackingRecord` type. It receives a `Location` and the `Date and Time` it was registered.

## 2. Implement functionality to calculate the approximate distance driven for a single TruckPlan.

The distance between two `Location` can be calculated using the [Haversive formula](https://en.wikipedia.org/wiki/Haversine_formula).

For this, I created a `DistanceCalculatorService`, adding a unit tests as well. The method is passed as parameter to `TruckPlan.CalculateDistanceDriven` in order to write the unit tests and mock the dependency on `DistanceCalculatorService`  

The logic for the total amount driven is the sum of all the distances in the tracking records.

**Note: I did not implement the formula. I used an LLM code assistant to get the algorithm**

