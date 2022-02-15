using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Checkout.ExternalClients;
using Checkout.ExternalClients.WestBank;
using Checkout.Api.Filters;
using Checkout.Services;
using Checkout.Api.Configuration;

namespace Checkout.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Checkout API", Version = "v1" });
            });

            services.AddControllers(options =>
                                        options.Filters.Add(new HttpResponseExceptionFilter()));

            services.AddControllers();

            var westBankApiConfig = Configuration
                .GetSection(nameof(WestBankApi))
                    .Get<WestBankApi>();

            var restClientFactory = new RestClientFactory();

            services.AddSingleton<IRestClientFactory>(restClientFactory);

            services.AddTransient<IApiClient>(x => ActivatorUtilities.CreateInstance<ApiClient>(x, westBankApiConfig.BaseUrl, restClientFactory));
            services.AddTransient<IPaymentProcessingService, PaymentProcessingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
