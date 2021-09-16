# Payment API

This project demonostrates a payment API and simulated bank service.

## Assumptions & Limitations


 - I did separate layers with models and basic mappings but of course not completely, to save time. In the real world, I would use Automapper or alike to properly map objects between all layers.
 - Bank simulator had to be injected into the Api project to simulate it.
 - There is a simple exception handling in place using Filters.
 - To emulate a database I instantiate a singleton DbContext with a List to create and retrieve the payments. 

# Structure

**Checkout.Api** is the web api project with one controller exposing two endpoints:
**POST** */payments*  accepts payment details and submits them to bank simulator for processing. **Important**, I did not implement input validation at this stage to save time BUT it is implemented down the process in the bank simulator.
Returns:
Status CREATED and location of the endpoint where details can be obtained
Status BAD REQUEST when the payment declined, along with the reason

**GET** */payments/{paymentId}* returns payment details
Returns:
Status OK with the payment details
Status NOT FOUND if payment was not found

**In a case when an exception occurs, it returns SERVICE UNAVAILABLE status**

It provides Swagger UI, so you can try it out with:
Request for successful transaction:

    {
      "paymentCardNumber": "1298555555555555",
      "expiryDate": "2021-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 110,
      "currencyCode": "GBP"
    }

or, for a declined

    {
      "paymentCardNumber": "4298555555555555",
      "expiryDate": "2021-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 110,
      "currencyCode": "GBP"
    }

for successful but with a warning

    {
      "paymentCardNumber": "1298555555555555",
      "expiryDate": "2021-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 6000,
      "currencyCode": "GBP"
    }

**Checkout.Api.Tests** is a set of tests for the API controller

**Checkout.Models** a library of shared models

**Checkout.Services** is the service layer with:

 - **PaymentProcessingService** Functional layer where all business logic is happening. In our case communication with an instance of a bank service, which could be an external API.

**Checkout.Services.Tests** a set of tests for the service. 

**Checkout.Bank** is a bank simulator. It demonstrates some logic to fail or accept payments based on the card number. 

 - **PaymentCardValidator** is a validator class that validates the card details.

**Checkout.Bank.Data** a simple Data layer for the test purposes, emulates a database where we store the payment details. 

Sergey