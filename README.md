**Containerisation & Docker Compose Deployment Prototype**

**Project Overview**
This project aims to standardize our development environment and eliminate setup inconsistencies by containerizing the entire backend system.
The concrete goals derived from this objective were: 

1. Containerize .NET 8 ASP.NET Core applications using Docker. 
2. Automate image builds and pushes to Docker Hub (and GHCR) using GitHub Actions. 
3. Set up a reverse proxy using Nginx to route API traffic. 
4. Implement Docker Compose for local multi-container orchestration. 
5. Portainer UI for container monitoring.
6. CI/CD pipeline using GitHub Actions. 
7. Migrate the Docker Compose setup to Kubernetes using Minikube. 


**Scope**
Overall design including containerization, Kubernetes deployment, and CI/CD automation of two .NET 8 microservices:
 	
• User Management Service (USM)– handles identity and authentication. 
• MeterManagementService (MMS)–handles the meter catalogue and order management workflow.

**Architectural Overview**
The system follows a microservices architecture in which each service is independently deployable, runs in its own container, and communicates with sibling services over a shared network– a Docker bridge network in Compose, and cluster-internal DNS in Kubernetes. An Nginx reverse proxy sits in front of the services and acts as the single-entry point for all client requests, so that API consumers never need to know internal service ports.

1.	The flow of a typical request is as follows:
2.	A client sends a request to the Nginx reverse proxy.
3.	Nginx routes the request to the appropriate microservice.
4.	Protected endpoints validate the JWT access token.
5.	The target service performs business operations.
6.	The service interacts only with its own PostgreSQL database.
7.	The response is returned to the client through Nginx.
8.	For authenticated requests, users must first obtain a JWT token through the authentication endpoint exposed by the Meter Service. The issued token is then included in the Authorization header for subsequent requests.

The same logical system is deployable through two parallel paths:

• Docker Compose– for fast local development, run with a single docker compose up command. 
• Kubernetes (Minikube)– for a production-like orchestrated deployment, including self-healing, horizontal scaling, and rolling updates.


**Core Services**
The application consists of two primary microservices, each with its own PostgreSQL database to ensure data isolation and service independence.

Service	Description	Port (host : container)	Database
User Management (USM)	.NET 8 
API for identity, auth 	5000 : 8080	PostgreSQL
apimeter_db 
Meter Management 
(MMS)	 .NET 8
API for meter catalogue	5001 : 8080	PostgreSQL
order_db 
apimeter_db 	PostgreSQL	5434 : 5432	-
order_db 	PostgreSQL	5435 : 5432 	-
Nginx	Reverse proxy routing all traffic on port 80	80	-

Key responsibilities include:
•	User registration and authentication.
•	JWT token generation and validation.
•	Complete CRUD operations for meter and order records.
•	Protection of API endpoints through JWT-based authentication.


**Technology Stack**
Language	C# / .NET 8	Backend API development
Framework	ASP .NET Core 8	REST API framework
Containerization	Docker	Package and run services
Local Orchestration	Docker Compose	Multi-container local management
Cluster Orchestration	Kubernetes / Minikube	Production-like container orchestration
Reverse Proxy	Nginx 	Route and load-balance traffic
Database	PostgreSQL 15	Relational data storage
CI/CD	GitHub Actions	Automated build and deploy
Registry	GHCR + DockerHub	Image storage and distribution
Observability	Prometheus + Grafana	Metrics collection and virtualization
Version Control	GitHub	Source code management



**Service Design: USM and MMS**
• USM is responsible for identity and authentication – issuing JWT tokens via POST /api/auth/token (chosen in preference to a conventional /login route to better reflect OAuth-style token issuance) and enforcing admin-only provisioning of new user accounts.

• MMS owns the meter catalogue and the order lifecycle (Pending ? Approved ? Dispatched ? Delivered).

The two services are configured with the same JWT signing secret. This allows MMS to validate tokens issued by USM directly and locally (through standard JWT signature verification) without making a network call back to USM on every request– avoiding a synchronous dependency between the two services on the request hot path.

**Data Model**
	User Model (USM)	

	• Id – unique identifier for each user. 
	• Email – used as the username / login identifier. 
	• Password – BCrypt hash; plaintext passwords are never stored.
	• Role – a simple string, e.g. "user" or "admin". 
	• CreatedAt – account-creation timestamp.

   Meter Model (MSM)

	• Id – unique identifier for each meter. 
	• Name, Description – meter model and specification text. 
	• Quantity, Price – inventory count and unit price. 
	• CreatedAt, UpdatedAt – audit timestamps.
		
2.5.3 Order Model (USM)

• CustomerName 
• MeterType – meter model and specification text. 
• Quantity– order count. 
• TotalPrice - cost

2.6	API Endpoints

Meter Management APIs

Method	Endpoint	Authentication	Description
GET	/api/Meter	Required	Retrieve all meters
GET	/api/Meter/{id}	Required	Retrieve meter by ID
POST	/api/Meter	Required	Create a new meter
PUT	/api/Meter/{id}	Required	Update meter details
DELETE	/api/Meter/{id}	Required	Delete a meter


Method	Endpoint	Description
GET	/api/Order	Retrieve all orders
GET	/api/Order/{id}	Retrieve order by ID
POST	/api/Order	Create a new order
DELETE	/api/orders/{id}	Delete an order

User Management APIs

Method	Endpoint	Description
POST	/api/Auth/register	Register a new user
POST	/api/Auth/login	Authenticate user and generate JWT token
GET	/api/Auth/user	Retrieve user details
