# Some Thoughts on the Exercise

I raised question about the requirement for .NET core, as it is no longer supported version, didn't get any reply so I used .NET 8 as It is version I ahve worked latest.

## Data source

### SQL approach

If I was to implement production version, one approach could be relation database storage, that allows me to use composed types for Geolocation with methods for calculation distance out of the box. 

### Geolocation
Currently iam calculating distance with Latitude and Longitude, other approach could be using Geolocation aPI from any of bigh players that provide with same feature but with actual addresses, this would probably be paid solution, but its an option

## Testing

### Unit Testing
- This exercise has little logic into it, but if I were testing this mediator approach, the one unit test there would be a template for it. Testing the handler with mocked requests and inspecting services inside as well as the generated outcome.

### Integration Testing
- The idea here is to have black box testing where I call the endpoints in the application and compare results as plain JSON. There, I would mock any data source, in this case, the CSV client to fetch close-to-real data from mock storage (I use CSV there, but a DB provider would take it from an in-memory or dockerized populated database) and compare results as `JObjects`, parsed models, or ideally plain JSONs as it is the actual response from the endpoint. Similar with the incoming requests, I would have JSON that would mimic what would normally come in an HTTP request from the Front End.
