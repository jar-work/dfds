
# Context

A Truck Plan describes a single driver driving a truck for a continuous period. For example; a five
hour drive through Germany on a specific date. A driver is a person with a name, birthdate, etc.

Each truck has a GPS device installed. This device provides the system with the current truck
position approximately every 5 minutes.

## 1. Design and implement a model for representing the domain.

The domain is basically a `TruckPlan`. It contains the information of the driver, `start` and `stop` and the expected duration.

`start` and `stop` are of type `Location`, which contains the `Latitude` and `Longitude`. It is used to represent a `GPS` location

Lastly, I create a `TrackingRecord` type. It receives a `Location` and the `Date and Time` it was registered.

