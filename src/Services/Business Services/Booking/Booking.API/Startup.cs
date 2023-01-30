using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Booking.Domain.Booking;
using Booking.Persistence.Repositories;
using MediatR;
using Booking.Application.Booking.Queries.GetBooking;
using System.Reflection;
using Booking.Persistence;
using Microsoft.EntityFrameworkCore;
using Booking.Application.Booking.Commands.CreateBooking;
using Booking.Application.Booking.Commands.UpdateBooking;
using Booking.Application.IntegrationEvents.Events;
using EventBusRabbitMQ;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using EventBus.Abstractions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions.CustomEvent;
using EventBusRabbitMQ.CustomEvents;
using Booking.Application.IntegrationEvents;
using Booking.API.Services;

namespace Booking.API
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

            services.AddControllers();
            services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("MicroCouriersDataBase")));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking.API", Version = "v1" });
            });
            //services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IBookingRespository, BookingRepository>();

            services.AddScoped<IMessageProducer, MessageProducer>();
            services.AddScoped<IMessageConsumer, MessageConsumer>();

            //services.AddScoped<IBookingIntegrationEventService, BookingIntegrationEventService>();

            services.AddMediatR(typeof(GetBookingQueryHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(CreateBookingCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(UpdateBookingHandler).GetTypeInfo().Assembly);

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory();
                factory.DispatchConsumersAsync = true;
                if (!string.IsNullOrEmpty(Configuration["HostName"]))
                {
                    factory.HostName = Configuration["HostName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusUserName"]))
                {
                    factory.UserName = Configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(Configuration["EventBusPassword"]))
                {
                    factory.Password = Configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            RegisterEventBus(services);
            //RegisterEngagementEventBus(services);
        }
        private void RegisterEventBus(IServiceCollection services)
        {
            var subscriptionClientName = "order_event";
            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddScoped<OrderStatusChangedIntegrationEventHandler>();
            //services.AddScoped<AddEngagementIntegrationEventHandler>();
            //services.AddScoped<UpdateEngagementIntegrationEventHandler>();
        }
        private void RegisterEngagementEventBus(IServiceCollection services)
        {
            var subscriptionClientName = "engagement_event";

            services.AddSingleton<IEngagementEventBus, EngagementEventBusRabbitMQ>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EngagementEventBusRabbitMQ>>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(Configuration["EventBusRetryCount"]);
                }

                return new EngagementEventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking.API v1"));
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            ConfigureEventBus(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void ConfigureEventBus(IApplicationBuilder app)
        {
            //app.ApplicationServices.GetRequiredService<IEngagementEventBus>();
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderStatusChangedIntegrationEvent, OrderStatusChangedIntegrationEventHandler>();
        }

    }
    
}
