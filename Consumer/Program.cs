using Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RabbitMqConsumer>();

var host = builder.Build();
host.Run();