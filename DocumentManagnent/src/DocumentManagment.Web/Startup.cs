using DocumentManagment.DataAccess.Repository;
using DocumentManagment.DataAccess.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using DocumentManagment.Services;
using DocumentManagment.Services.Providers;
using DocumentManagment.Web.Filters;
using DocumentManagment.Web.Handlers;
using DocumentManagment.Web.Infrastructure;
using DocumentManagment.Web.Mappers;

namespace DocumentManagment.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnection = this.Configuration.GetSection("cosmosDbConnection");
            if (dbConnection == null)
            {
                throw new ArgumentException("CosmosDBConnection is empty");
            }

            var storageConnection = this.Configuration.GetSection("blobStorageConnection");
            if (storageConnection == null)
            {
                throw new ArgumentException("StorageConnection is empty");
            }

            services.AddSingleton<CosmosClient, CosmosClient>(provider => new CosmosClient(
                dbConnection.Value,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                }));
            services.AddSingleton<DocumentMapper, DocumentMapper>();
            services.AddSingleton<IBlobStorage, BlobStorage>(provider => new BlobStorage(storageConnection.Value));
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IUserProvider, UserProvider>();
            services.AddTransient<IExceptionHandler, ExceptionHandler>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var cosmosClient = provider.GetService<CosmosClient>();

                // Just for testing purpose
                this.CreateDocumentContainerAsync(cosmosClient).GetAwaiter().GetResult();
            }

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseMvc();
        }

        public async Task CreateDocumentContainerAsync(CosmosClient client)
        {
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync("document-managment");
            var containerProperties = new ContainerProperties()
            {
                Id = "documents",
                PartitionKeyPath = "/userId",
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = false,
                    IndexingMode = IndexingMode.Lazy,
                }
            };
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerProperties);
        }
    }
}
