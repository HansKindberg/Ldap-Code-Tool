using System;
using Application.Models;
using Application.Models.Code;
using Application.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RegionOrebroLan.DirectoryServices.Protocols.DependencyInjection;

namespace Application
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
		}

		#endregion

		#region Properties

		public virtual IConfiguration Configuration { get; }
		public virtual IHostEnvironment HostEnvironment { get; }

		#endregion

		#region Methods

		public virtual void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder
				.UseDeveloperExceptionPage()
				.UseStaticFiles()
				.UseRouting()
				.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.Configure<CodeOptions>(this.Configuration.GetSection("Code"));

			var mvcBuilder = services.AddControllersWithViews();
#if DEBUG
			mvcBuilder.AddRazorRuntimeCompilation();
#endif
			services.AddDirectory(this.Configuration);
			services.AddScoped<IFacade, Facade>();
			services.AddSingleton<ICodeGenerator, CodeGenerator>();
			services.AddSingleton<ISystemClock, SystemClock>();
		}

		#endregion
	}
}