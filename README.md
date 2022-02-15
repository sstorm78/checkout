
# Payment & Bank APIs

This project demonostrates a payment API, second level Bank API and an API Client.

## Assumptions & Limitations


 - I did separate layers with models and basic mappings but of course not completely, to save time. In the real world, I would use Automapper or alike to properly map objects between all layers.
 - Bank API pretty much identical to the Payment API but the Payment API has a client to demonstrate how it can work with external clients.
 - There is a simple exception handling in place using Filters.
 - To emulate a database I instantiate a singleton DbContext with a List to create and retrieve the payments. 
 - Normally I use async all the way, but since we have a simple db layer and no external api to call, it is not necessary

## Demo Steps 

 1. Open the solution in VS 2019
 2. Click Start and it will launch two web browser tabs for Checkout API and Bank simulation API
 3. Open Swagger page for Checkout API and test the POST endpoint with this payload:

    {
      "paymentCardNumber": "1298555555555555",
      "expiryDate": "2028-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 110,
      "currencyCode": "GBP"
    }
4. It will process the request all the way to the WestBank API and return 201 with Location header where the transaction details can be obtained.

# Structure

**CHECKOUT** directory

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
      "expiryDate": "2028-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 110,
      "currencyCode": "GBP"
    }

or, for a declined

    {
      "paymentCardNumber": "4298555555555555",
      "expiryDate": "2028-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 110,
      "currencyCode": "GBP"
    }

for successful but with a warning

    {
      "paymentCardNumber": "1298555555555555",
      "expiryDate": "2028-09-16T21:16:18.380Z",
      "cvvNumber": 123,
      "amount": 6000,
      "currencyCode": "GBP"
    }

**Checkout.Api.Tests** is a set of tests for the API controller

**Checkout.Models** a library of shared models

**Checkout.Services** is the service layer with:

 - **PaymentProcessingService** Functional layer where we use process the request and forward it to our bank for actual payment. It uses an API Client for fictional bank called WestBank.

**Checkout.Services.Tests** a set of tests for the service. 


**WESTBANK** directory

**WestBank.Api** is a bank simulator API. It demonstrates some logic to fail or accept payments based on the card number. 

**WestBank.Models** a library of shared models

**WestBank.Services** Functional layer where we mimic payment flows. For the test purposes with any card number that starts with 5, the API will return "insuficient funds" response.

 - **PaymentCardValidator** is a validator class that validates the card details.
 - **Rules** for the pupose of the original test where any transaction with amount equal or greater than £5000 must success with a warning. I've created a simple collection of rules that can be extended and for the test has a rule that checks the transaction amount. The rules can return a collection of warning messages that we can pass along with the payment response.

**Checkout.Bank.Data** a simple Data layer for the test purposes, emulates a database where we store the payment details. 

Sergey