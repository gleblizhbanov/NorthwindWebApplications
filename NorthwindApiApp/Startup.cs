using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Northwind.DataAccess;
using Northwind.DataAccess.SqlServer;
using Northwind.Services.Blogging;
using Northwind.Services.DataAccess.Employees;
using Northwind.Services.DataAccess.Products;
using Northwind.Services.Employees;
using Northwind.Services.EntityFrameworkCore.Blogging;
using Northwind.Services.EntityFrameworkCore.Context;
using Northwind.Services.Products;

namespace NorthwindApiApp
{

    /// <summary>
    /// A startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">A project configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the app configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services used by the app.
        /// </summary>
        /// <param name="services">A collection of services.</param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeIsoFormatJsonConverter());
            });

            services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = true;
            });

            services.AddScoped(service => new SqlConnection(this.Configuration.GetConnectionString("Northwind")));
            services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(this.Configuration.GetConnectionString("Northwind")));
            services.AddTransient<string[]>();
            services.AddTransient<IProductManagementService, ProductManagementDataAccessService>();
            services.AddTransient<IProductCategoryManagementService, ProductCategoriesManagementDataAccessService>();
            services.AddTransient<IProductCategoryPictureManagementService, ProductCategoryPicturesManagementDataAccessService>();
            services.AddTransient<IEmployeeManagementService, EmployeeManagementDataAccessService>();
            services.AddTransient<IEmployeePhotoManagementService, EmployeePhotoManagementDataAccessService>();
            services.AddTransient<IBloggingService, BloggingService>();
            services.AddTransient<IBlogArticleProductsManagementService, BlogArticleProductManagementService>();
            services.AddTransient<IBlogCommentsManagementService, BlogCommentsManagementService>();
            services.AddTransient<NorthwindDataAccessFactory, SqlServerDataAccessFactory>();

            //services.AddDbContext<BloggingContext>(options => options.UseInMemoryDatabase("NorthwindBlogging"));
        }

        /// <summary>
        /// Configures the app.
        /// </summary>
        /// <param name="app">An application builder.</param>
        /// <param name="env">A web host environment.</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
