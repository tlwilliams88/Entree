Thank you for installing the Commerce Server Presentation Configuration package. 

This package has configured your web application to use Commerce Server as a WCF server hosted in IIS.  

Quick Start:

1. If you already have a Foundation Service grab its URL and paste it in the IOperationService endpoint in this project's web.config
<client>
      <endpoint name="IOperationService" address="{Your Service's URL Here}" binding="wsHttpBinding" bindingConfiguration="wsHttp" contract="CommerceServer.Foundation.IOperationService">

2. If you don't have a Foundation Service set one up using the CommerceServerContrib.FoundationServerConfiguration Nuget package.  Create a new project (it can be an empty web project) in VS and add the Foundation package.

You can also set up an all-in-one (Two-Tier) site using the CommerceServerContrib.TwoTierConfiguration package. 

3. Get started with some code. We've included a class called CommerceFoundationServiceAgent to make it easier to talk to the Commerce Foundation. 
Start with something like.....

var simpleCatalogQuery = new CommerceQuery<CommerceEntity>("Catalog");
simpleCatalogQuery.SearchCriteria.Model.Id = "Adventure Works Catalog";

var response = CommerceFoundationServiceAgent.Execute(simpleCatalogQuery);

Check out our CommerceServerContrib.CodeGeneration Nuget package for helpers that will generate CommerceEntity classes for each the entities defined in your metadata system.
CommerceServerContrib.CodeGeneration will also generate helpers for each of your request templates so you can easily call them from C#.  