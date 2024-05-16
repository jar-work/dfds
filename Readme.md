
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

## 3. Find a way to get the country from a coordinate. A solution could, for example, be to call an
external web service.

For this, I created a new project: `Infrastructure`. It is expected that all external dependencies will be implemented there (like the web service call). I also modified the domain a bit, so every time we get a new tracking record we can 
calculate the country of the location.

For the webservice, I'm using [OpenCage](https://opencagedata.com). The API key is hardcoded for now, but it should be configured during the Dependency Injection settings. I will try to clean that if I have enough time

## 4. Implement functionality for answering the following question: "How many kilometers did drivers over the age of 50 drive in Germany in February 2018?"

Here the easiest (and more expensive) solution is to query all the `TruckPlans` for all the drivers that match the criteria:

- Each `TruckPlan` has a `Driver`
- Each `Driver` has a `Birthdate`
- The `TruckPlan` has a list of `TrackingRecords` with the information of the `DateTime` and the `Country` of the record

a pseudo algorithm for this could be:

```csharp
var truckPlans = FROM truckPlan IN truckPlans 
                    WHERE age(truckPlan.Driver.Birthday) >= 50
                    AND truckPlan.TrackingRecords.Any(tr => tr.Country == "Germany")
                    AND truckPlan.TrackingRecords.Any(tr => tr.DateTime.Year == 2018)
                 SELECT truckPlan;

// if we assume that all the truckPlans are ALWAYS inside the same country, then the logic is quite simple (and we should change the domain a bit to reflect that, as now the country is part of the tracking record)
var total = 0;
var distanceCalculatorService = new DistanceCalculatorService() // or injected if using dependency injection
foreach (var item in truckPlans) {
    total += item.CalculateDistanceDriven(distanceCalculatorService);
}
Console.WriteLine($"Total sum of Kilometers: {total});

double age(DateOnly date) {
    var today = DateOnly.FromDateTime(DateTime.Today);
    var age = today.Year - birthDate.Year;

    // Checking if the time difference is less than a year, as age is absolut based on the Year 
    if (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day))
    {
        return age -1;
    }
    return age;
}
```

however, this algorithm requires us to store the data in their raw representation (in documents that represent the domain objects, or in normalized tables). 

### problems with this approach

1. It doesn't consider that the driver could have driven outside the country. A `TruckPlan` could, for example, drive between Germany and Poland. If we are interested in Germany, then, we shouldn't sum all the time the driers was in Poland
2. Considering we have maybe thousands of `TruckPlans` being recorded every day, this way of querying could be too expensive.

### Solutions to the raw data

One approach could be to partition the data by country, using a database like `CosmosDB`. In this case, if we define the country as part of the `TruckPlan`, we can store all the information 
related to Germany:

1. A GPS signal comes from the truck
2. We identify the current `TruckPlan` of that truck. The `TruckPlan` contains the information of the country.
3. We append the `TrackingRecord` to that `TruckPlan`

we can also use `Cosmos change feed` to react to the documents, updating the current amount of kilometers of the `TruckPlan`, saving it to another table that contains the summary:

```csharp

private Task HandleChangeAsync(IReadOnlyCollection<TruckPlan> changes, CancellationToken cancellationToken) {
    var distanceCalculatorService = new DistanceCalculatorService() // or injected if using dependency injection      
    foreach (var item in changes) {
        var distance = distanceCalculatorService.CalculateDistance(item) // the logic will be in the service, instead of the domain
        item.SetCalculatedDistanceDriven(distance)
        // update the distance driven to another storage where we get these metrics, like (truckPlanId, driver_birthdate, country, date, kilometers) 
    }
}

// then the algorithm is more simple
var total = SELECT SUM(kilometers) FROM statistics 
                    WHERE age(driver_birthdate) >= 50
                    AND country == "Germany"
                    AND year(date) == 2018;


```