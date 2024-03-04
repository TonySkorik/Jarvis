using Autofac;

namespace Jarvis.Server;

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
	}
		
	// ConfigureContainer is where you can register things directly
	// with Autofac. This runs after ConfigureServices so the things
	// here will override registrations made in ConfigureServices.
	// Don't build the container; that gets done for you by the factory.
	public void ConfigureContainer(ContainerBuilder builder)
	{
		//var settings = Configuration.Get<AppSettings>();
		//builder.RegisterInstance(settings)
		//	.SingleInstance();
		//builder.RegisterType<SstCloudSettingsProvider>()
		//	.As<ISstCloudSettingsProvider>()
		//	.SingleInstance();
		//builder.RegisterType<SstCloudClient>()
		//	.InstancePerDependency();
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		//app.UseHttpsRedirection();
			
		app.UseRouting();

		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
}