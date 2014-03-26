Service Broker for Cloud Foundry
====================

Server Broker for Cloud Foundry Service is a Microsoft .Net based service broker for a Cloud Foundry v2.0. It uses WebAPI to host the services. The server broker ships with one service for Microsoft Sql Server. This service will provision databases and users for applications. It can be extend to include any plans you want to offer.

The broker assumes SQL Server 2012. It uses [Contained Databases](http://technet.microsoft.com/en-us/library/ff929071.aspx) which is new functionality in SQL Server 2012. 

### Setup

Before you begin you should install `cf` Command Line Interface. Installation instructions are available [here](http://docs.cloudfoundry.com/docs/using/managing-apps/cf/index.html).

1. Installing the Service Broker
 * Update SQL Server to support contained databases:
 
 ```
 sp_configure 'contained database authentication', 1;
 GO
 RECONFIGURE
 GO
 ```
 * Deploy the service broker to IIS
     * Clone the repository
     * Publish the web project from Visual Studio
     * Add the "Application Server" role to the Windows server that will host IIS
     * Add the "Web Server (IIS) Suppport" Application Server role service
     * Add the published project as a new web site in IIS
 * If you want to setup authentication, you can setup auth in IIS. You can also enable SSL through IIS.
 * Configure your network environment to make the service be accessible from the Cloud Foundry.
 * Update the `appSettings` in the Web.config file. The following keys are in the `appSettings` 
        * sqlDashboardUrl - The url to the dashboard returned from provisioning a database
        * sqlFilePath - The path to where the database files will be store.
        * databaseNameFormat - The prefix to apply to the name of the database. It prefixes this to the binding id.
        * databaseUserFormat - The prefix to apply to users created in the database. It prefixes this to the binding id.
        * sqlDataSource - The datasource for the database created. This will be used as the data source in the connection string for the application.
        * sqlServerPort - This is the port that is used to connect to SQL Server.
 * [Register your service broker](http://docs.cloudfoundry.org/services/managing-service-brokers.html#register-broker) and make your [plan(s) public](http://docs.cloudfoundry.org/services/access-control.html#make-plans-public).
 

### Collaborate

You are welcome to contribute via
[pull request](https://help.github.com/articles/using-pull-requests).
