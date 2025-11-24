# .Net 8.0 Microservice Project
<br />

**Microservices** architecture, or simply microservices, comprises a set of focused, independent, autonomous services that make up a larger business application. The architecture provides a framework for independently writing, updating, and deploying services without disrupting the overall functionality of the application. Within this architecture, every single service within the microservices architecture is self-contained and implements a specific business function. For example, building an e-commerce application involves processing orders, updating customer details, and calculating net prices. The app will utilize various microservices, each designed to handle specific functions, working together to achieve the overall business objectives.

<br />

<img src='img/Microservices-Architecture.png' />

<br />

Adopting a microservices architecture brings a range of benefits that can transform how organizations build and operate software. One of the primary advantages is the ability to scale individual services independently, which helps optimize resource usage and eliminates bottlenecks that can affect the entire application. This independent scalability also means that development teams can deploy services independently, reducing the risk of system-wide outages and enabling continuous delivery of new features. Deployments can be performed with one service at a time or span multiple services, depending on the specific needs of the deployment.

## 🗃️ Whats Including In This Repository
I have implemented below features.

#### Order microservice which includes
* NET 8.0 Web API application 
* REST API principles, CRUD operations
* **SQL Server database (MicroShop)** connection
* Repository Pattern Implementation
* Swagger Open API implementation
* Consume Events (BasketCheckoutConsumer)	
* Publish OrderCreateEvent event with using **MassTransit and RabbitMQ**
* Implementing **DDD, CQRS, and Clean Architecture** with using Best Practices
* Developing **CQRS with using MediatR, FluentValidation and AutoMapper packages**
* Consuming **RabbitMQ** BasketCheckout Event Queue with using **MassTransit-RabbitMQ** Configuration
* **SQL Server Database** connection
* Using **Entity Framework Core ORM** and migratation to SQL Server Manually.

#### Discount microservice which includes
* NET 8.0 **Grpc Server** application
* Build a Highly Performant **inter-service gRPC Communication** with Basket Microservice
* Exposing Grpc Services with creating **Protobuf messages**
* Using **ADO.Net implementation** to simplify data access and ensure high performance
* **SQL Server database (MicroShopDiscount)** connection

#### Basket microservice which includes
* NET 8.0 Web API application
* REST API principles, CRUD operations
* **Redis database** connection
* Consume Discount **Grpc Service** for inter-service sync communication to calculate product final price
* Publish BasketCheckout event with using **MassTransit and RabbitMQ**

#### Inventory microservice which includes
* NET 8.0 Web API application 
* Consume Events (ProcessInventoryConsumer)
* **SQL Server database (MicroShop)** connection
* Repository Pattern Implementation
* Swagger Open API implementation	
* Publish InventorySuccessEvent event with using **MassTransit and RabbitMQ**

#### Payment microservice which includes 
* NET 8.0 Web API application 
* Consume Events (ProcessPaymentConsumer)
* **SQL Server database (MicroShopPayment)** connection
* Repository Pattern Implementation
* Swagger Open API implementation	
* Publish PaymentSucceededEvent event with using **MassTransit and RabbitMQ**
	
#### API Gateway Ocelot Microservice
* Implement **API Gateways with Ocelot**
* Sample microservices/Consul to reroute through the API Gateway	
* The Gateway aggregation pattern in MicroShop.Aggregator

#### Microservices Communications
* Sync inter-service **gRPC Communication**
* Async Microservices Communication with **RabbitMQ Message-Broker Service**
* Using **RabbitMQ Publish/Subscribe Topic** Exchange Model
* Using **MassTransit** for abstraction over RabbitMQ Message-Broker system
* Publishing BasketCheckout event queue from Basket microservices and Subscribing this event from Ordering microservices (BasketCheckoutConsumer) and the rest of the ordering flow with **Masstransit Saga State Machine**  (MassTransitStateMachine : OrderStateMachine)
* Create **RabbitMQ EventBus.Messages library** and add references Microservices

#### Microservices Cross-Cutting Implementations
* Implementing **Centralized Logging with SeriLog** for Microservices
* Use the **Consul HealthChecks** feature in back-end ASP.NET microservices

#### Microservices Resilience Implementations
* Making Microservices more **resilient Use IHttpClientFactory** to implement resilient HTTP requests
* Implement **Retry patterns** with **MassTransit UseMessageRetry**

#### Microservices Deployments & Service Discovery
* Implementing **Consul Cluster** with a server and a client agent for **Service Discovery**
* **Register** each service to mentioned Consul nodes with Hostnames
* Using **Steeltoe** library to register and resolve the services


## 🎨 State Diagram
#### Ordering flow with Masstransit Saga State Machine (OrderStateMachine)

**Transitions** between these states are triggered by user actions or system processes (Saga), providing a clear overview of the customer journey from initial product search to final purchase and order confirmation.

<img src="img/state.png" style="width:70%;"  />


### 📜 OrderStateMachine Class (MassTransit Saga State Machine) :

```
public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {

        public State ProcessingInventory { get; set; }

        public State ProcessingPayment { get; set; }

        public State ProcessingEnd { get; set; }

        public State Canceled { get; set; }


        public Event<BasketCheckoutEvent> BasketCheckoutEvent { get; set; }

        public Event<OrderCreateEvent> OrderCreateEvent { get; set; }

        public Event<InventorySuccessEvent> InventorySuccessEvent { get; set; }

        public Event<InventoryFailedEvent> InventoryFailedEvent { get; set; }

        public Event<PaymentSucceededEvent> PaymentSucceededEvent { get; set; }

        public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

        public Event<ProcessEndedEvent> ProcessEndedEvent { get; set; }


        public OrderStateMachine()
        {

            InstanceState(x => x.CurrentState);

            Event(() => BasketCheckoutEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => OrderCreateEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventorySuccessEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventoryFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentSucceededEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => ProcessEndedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            //*******************************************************************

            // مرحله 1: ایجاد رکورد سفارش        
            Initially(
                When(OrderCreateEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Order {c.Data.OrderId} Created by CustomerId : {c.Data.CustomerId}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<OrderCreateEvent>>())
                    .PublishAsync(async c => new ProcessInventory()
                    {
                        OrderId = c.Message.OrderId,
                        CustomerId = c.Message.CustomerId,
                        CorrelationId = c.Message.CorrelationId,
                        Created = c.Message.Created
                    })
                    .TransitionTo(ProcessingInventory)
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingInventory for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 2: بررسی موجودی        
            During(ProcessingInventory,
                When(InventorySuccessEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Inventory reserved for Order {c.Data.OrderId}");
                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<InventorySuccessEvent>>())
                    // انتقال مرحله پرداخت خودکار به دستی و توسط دکمه پرداخت
                    .TransitionTo(ProcessingPayment)
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingPayment for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}")),

                When(InventoryFailedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Inventory reservation failed for Order {c.Data.OrderId}: {c.Data.Reason}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<InventoryFailedEvent>>())
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 3: پرداخت موفق
            During(ProcessingPayment,
                When(PaymentSucceededEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Payment done for Order {c.Data.OrderId}");
                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<PaymentSucceededEvent>>())
                    .PublishAsync(async c => new ProcessEnd()
                    {
                        OrderId = c.Message.OrderId,
                        CustomerId = c.Message.CustomerId,
                        CorrelationId = c.Message.CorrelationId,
                        Created = c.Message.Created
                    })
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingEnd for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
                    .TransitionTo(ProcessingEnd),


                When(PaymentFailedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Payment failed for Order {c.Data.OrderId}: {c.Data.Reason}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<PaymentFailedEvent>>())
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))

            );

            // مرحله 4: end
            During(ProcessingEnd,
                When(ProcessEndedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        
                        Log.Information($"Ending done for Order {c.Data.OrderId}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<ProcessEndedEvent>>())
                    .TransitionTo(Shipped)
                    .Then(c => Log.Information($"[Saga] Transitioned to Shipped for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))                    
                    .Finalize()

            );


            // Log any unexpected incoming events for debugging
            DuringAny(
                When(ProcessEndedEvent)
                    .Activity(x => x.OfType<GenericOrderEventActivity<ProcessEndedEvent>>())
                    .Then(ctx => Console.WriteLine($"⚠️ ProcessEndedEvent received in unexpected state {ctx.Saga.CurrentState}"))                  
            );

            SetCompletedWhenFinalized();

            Log.Information("✅ OrderStateMachine constructor called");

        }
    }
```


## 📦 Domain-Driven Design — Order Service

Order.API is an independent Bounded Context in the microservices architecture responsible for managing the complete lifecycle of an order.
It contains the core business logic for order creation, validation, inventory reservation, discount calculation, and payment initiation.

### 🧩 Core Domain

#### Order Aggregate

* Order acts as the Aggregate Root

* OrderItem (Child Entity)

* Contains order items (OrderItems)

* EnumOrderState → Domain Enum

* Event Bus Consumers → Domain Event Handler

* Enforces domain rules such as:

   * Calculating total price

   * Validating order status

   * Managing state transitions (ProcessingInventory → ProcessingPayment → ProcessingEnd → Canceled)

### 📚 Application Layer

* Implements CQRS

    * Commands: create order, update order state, cancel order

    * Queries: retrieve orders

* Uses Handlers to orchestrate processes

* Coordinates domain logic with external services

### 🏛  Infrastructure Layer

* Implements Repository pattern in alignment with DDD

* Uses EF Core for database communication

* Maps Domain Models ←→ Persistence Models

* Manages Unit of Work through EF Core context

## 🔗 Inter-Context Relationships (Context Map)

#### Order Context interacts with several other services:

| Service            | DDD Relationship    | Description                                                          |
| ------------------ | ------------------- | -------------------------------------------------------------------- |
| **Basket**         | Conformist          | Order follows the data model shaped by Basket.                       |
| **Discount**       | Customer → Supplier | Discount rules are defined by Discount service; Order consumes them. |
| **Inventory**      | Supplier + ACL      | Order requests stock reservation;            |
| **Payment**        | Supplier            | Order initiates the payment workflow.                                |
| **Ocelot Gateway** | Open-Host Service   | Order is exposed externally via the API Gateway.                     |


## 🎯 Responsibilities Summary

#### The Order Service is responsible for:

* Receiving the user's basket and converting it into an order

* Validating discount information via Discount service

* Check stock through Inventory service

* Initiating payment via Payment service

* Managing the full lifecycle of an order and its domain events


## ✈️ Databases Migrations
#### Entity Framework's Migrations & Scaffolding :
MicroShop.Infra.Sql :
```
Scaffold-DbContext "Data Source=.;Initial Catalog=MicroShop;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Context -Force
```
MicroShop.Infra.Sql :
```
Scaffold-DbContext "Data Source=.;Initial Catalog=MicroShopLogDB;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir LogContext -Force
```
Discount.API :
```
Scaffold-DbContext "Data Source=.;Initial Catalog=MicroShopDiscount;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Context -Force
```
Payment.API :
```
Scaffold-DbContext "Data Source=.;Initial Catalog=MicroShopPayment;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Context -Force
```
MicroShop.OrderApi.Rest :
```
Add-Migration InitOrderSaga -c OrderStateDbContext
Update-Database -Context OrderStateDbContext
```
MicroShop.OrderApi.Rest :
```
Add-Migration InitialOrderEventStoreMigration -c OrderEventStoreDbContext 
Update-Database -Context OrderEventStoreDbContext
```

#### 🛢️ Databases :

<img src="img/DataBases.png" />

<br />

## 🏗️ Solution Structure :

<img src="img/Projects.png" />


## 🚂 Service Discovery (Consul Registeration)

### On the all Services (e.g. Basket.API) :

#### 🔩 appsettings.json:
```
"spring": {
    "application": {
      "name": "basket-service"
    }
  },
  "steeltoe": {
    "client": {
      "consul": {
        "discovery": {
          "host": "Consul1",
          "port": 8500,
          "scheme": "http",

          "register": true,
          "serviceName": "basket-service",

          "healthCheckPath": "/health",
          "healthCheckInterval": "10s",

          "useNetUtils": false
        }
      }
    }
  }
```
#### 📑 Program.cs :
```
builder.Services.AddDiscoveryClient(builder.Configuration);

builder.Services.AddHealthChecks();

// HealthCheck endpoint for Consul
app.MapHealthChecks("/health");

// Register to Consul
app.UseDiscoveryClient();
```

### 🎡 Consul Nodes on Ubuntu 

<img src="img/Nodes2.png" />

```
Server IP, Host Name : 192.168.56.164, Consul1
Client IP, Host Name : 192.168.56.165, Consul2
```

### ⚒️ Install Consul on both Ubuntu :
```
wget -O - https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor | sudo tee /usr/share/keyrings/hashicorp-archive-keyring.gpg

echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list

sudo apt update && sudo apt install consul
```

#### 🖥️ On the Server :

<img src="img/ConsulVersion.png" />

* ⚙️ Consul Server Config :

<img src="img/ConsulServer1.png" />

* 🚑 Set Consul as a Service :

<img src="img/ConsulServer2.png" />

* 🩺🌡️ Service Status :

<img src="img/ConsulServer3.png" />

#### 💻 On the Client :

* ⚙️ Consul Client Config :

<img src="img/ConsulClient1.png" />

* 🚑 Set Consul as a Service :

<img src="img/ConsulClient2.png" />

* 🩺🌡️ Service Status :

<img src="img/ConsulClient3.png" />

#### 🛰️ DNS Settings On each Consul nodes :
<img src="img/dns.png" />

#### 📺 Access to the Consul UI from http://Consul1:8500/ui :

<img src="img/Nodes.png" />

#### 🏥 Registered Services except Gateway :
<img src="img/Services.png" />

#### 🚔 Order Service (e.g.) :
<img src="img/OrderService.png" />

### 🚀 Start to Running the all Services in Consul Nodes :
```
dotnet restore
dotnet build
dotnet run
```
### 🧨 Services Run at :
```
Discount.gRPC    --->  Consul1
Basket.API       --->  Consul2
OrderApi.Rest    --->  Consul1
Inventory.API    --->  Consul2
Payment.API      --->  Consul1
OcelotAPIGateway --->  Consul2
```

#### 💎 Discount.gRPC runs with http://Consul1:5046 :

<img src="img/gRPC.png" />

#### 💸 Getting discount info from above gRPC address at basket service :
<img src="img/Basket.png" />

#### 🚧 Ocelot Gateway runs on http://Consul2:8000 :

<img src="img/OcelotGateway.png" />

#### Get access to Order Service from the other Consul Machine :

<img src="img/OrderUrl.png" />

#### Get access to Basket Service from the other Consul Machine :

<img src="img/BasketUrl.png" />


## ☎️ Call the Services via gateway :

#### 🛒🛍️ Add items to the customer's basket :

<img src="img/Call1.png" />

#### 🛒 Checkout the basket with a tracking code (Awaiting Payment) :

<img src="img/Call2.png" />

#### 💳 Payment action for the related order :

<img src="img/Call3.png" />

#### ♻️ Order State Before the Payment :

<img src="img/OrderStateBeforePayment.png" />

#### 📖 Order & Items records :

<img src="img/OrderTable.png" />

#### 📝 Order Events Table (Event Store) :

<img src="img/OrderEvents.png" />

#### 🔀 Message Broker (RabbitMQ) :

<img src="img/RabbitMQ.png" />

#### 🎢 Rabbit Exchanges

<img src="img/RabbitExchanges.png" />

#### Message Rates :

<img src="img/RabbitMQ2.png" />

#### 🌵 Git Commits :

<img src="img/Commits.png" />
