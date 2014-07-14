Thank you for installing the Commerce Server Code Generation Nuget Package. 

Quick Start:

1. Build your solution
2. Open Models > CommerceEntities.tt and then save the file.  That'll make T4 generate CommerceEntity code for you.
3. Open RequestTemplates > RequestTemplates.tt and then save the file.  That'll make T4 generate RequestTemplate helpers for you.
4. Build your solution again.  You now have Commerce Entities and Request Template Helpers you can use to call Commerce Server.

Using the generated entities and Request Template helpers.

Where RT is the namespace to the generated RequestTemplates folder and Product is a generated Product entity class, do something like the following...

		public ActionResult Index(string productId)
        {
            if (!string.IsNullOrWhiteSpace(productId))
            {
                var product = RT.Catalog.GetProductSummary(productId, "Adventure Works Catalog");
                if (product != null)
                {
                    return View((Product)product);
                }
            }

            return View(new Product());
        }

Know Issues:

1. "Compiling transformation: Metadata file 'C:\projectpath\bin\CommerceServerContrib.CodeGeneration.dll' could not be found	C:\projectpath\Models\CommerceEntities.tt" will appear right after you install the package.  Building once will make that go away.

More Information and Recommended Reading: 

Check out the documentation at http://cscodegen.codeplex.com/documentation and if you have any issues or recommendations please contact us.

