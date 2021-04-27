using AutoMapper;
using DataInfo.Api.Filters;
using DataInfo.Core.Applibs;
using DataInfo.Core.Models.Dto.Interactive.Content;
using DataInfo.Core.Models.Dto.Member.Content;
using DataInfo.Core.Models.Dto.Ride.Content;
using DataInfo.Core.Models.Dto.Team.Content;
using DataInfo.Repository.Interfaces.Common;
using DataInfo.Repository.Interfaces.Interactive;
using DataInfo.Repository.Interfaces.Member;
using DataInfo.Repository.Interfaces.Post;
using DataInfo.Repository.Interfaces.Ride;
using DataInfo.Repository.Interfaces.Team;
using DataInfo.Repository.Managers.Common;
using DataInfo.Repository.Managers.Interactive;
using DataInfo.Repository.Managers.Member;
using DataInfo.Repository.Managers.Post;
using DataInfo.Repository.Managers.Ride;
using DataInfo.Repository.Managers.Team;
using DataInfo.Service.Interfaces.Common;
using DataInfo.Service.Interfaces.Interactive;
using DataInfo.Service.Interfaces.Member;
using DataInfo.Service.Interfaces.Post;
using DataInfo.Service.Interfaces.Ride;
using DataInfo.Service.Interfaces.Server;
using DataInfo.Service.Interfaces.Team;
using DataInfo.Service.Managers.Common;
using DataInfo.Service.Managers.Interactive;
using DataInfo.Service.Managers.Member;
using DataInfo.Service.Managers.Post;
using DataInfo.Service.Managers.Ride;
using DataInfo.Service.Managers.Server;
using DataInfo.Service.Managers.Team;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataInfo.Api
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration">configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Auth �B�z��
        /// </summary>
        /// <param name="services">services</param>
        private void AuthHandler(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //// �ˬd HTTP Header �� Authorization �O�_�� JWT Bearer Token
                    .AddJwtBearer(options =>  //// �]�w JWT Bearer Token ���ˬd�ﶵ
                    {
                        options.RequireHttpsMetadata = false; ////����γ]�m���ƾڦa�}���v���O�_�ݭnHTTPS�C�q�{�Ȭ�true�C�Ȧb�}�o���Ҥ����T�Φ��\��C
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = AppSettingHelper.Appsetting.Jwt.Iss,
                            ValidateAudience = true,
                            ValidAudience = AppSettingHelper.Appsetting.Jwt.Iss,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingHelper.Appsetting.Jwt.Secret))
                        };
                    });
        }

        /// <summary>
        /// Config �B�z��
        /// </summary>
        private void ConfigurationHandler()
        {
            AppSettingHelper.Appsetting = Configuration.Get<AppSettingHelper>();
        }

        /// <summary>
        /// �̪ۨ`�J�B�z��
        /// </summary>
        /// <param name="services">services</param>
        private void DependencyInjectionHandler(IServiceCollection services)
        {
            #region Service

            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IMemberService, MemberService>();
            services.AddSingleton<IRideService, RideService>();
            services.AddSingleton<IInteractiveService, InteractiveService>();
            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<ITeamInteractiveService, TeamInteractiveService>();
            services.AddSingleton<ITeamActivityService, TeamActivityService>();
            services.AddSingleton<ITeamBulletinService, TeamBulletinService>();
            services.AddSingleton<IPostService, PostService>();
            services.AddSingleton<IUploadService, UploadService>();
            services.AddSingleton<IVerifyCodeService, VerifyCodeService>();
            services.AddSingleton<IServerService, ServerService>();

            #endregion Service

            #region Repository

            services.AddSingleton<IMemberRepository, MemberRepository>();
            services.AddSingleton<IRideRepository, RideRepository>();
            services.AddSingleton<IInteractiveRepository, InteractiveRepository>();
            services.AddSingleton<ITeamRepository, TeamRepository>();
            services.AddSingleton<ITeamActivityRepository, TeamActivityRepository>();
            services.AddSingleton<ITeamBulletinRepository, TeamBulletinRepository>();
            services.AddSingleton<IPostRepository, PostRepository>();
            services.AddSingleton<IRedisRepository, RedisRepository>();

            #endregion Repository
        }

        /// <summary>
        /// FluentValidation �B�z��
        /// </summary>
        private void FluentValidationHandler(IServiceCollection services)
        {
            //// ���U API ���e�˴�
            services.AddMvc(setup =>
            {
                setup.Filters.Add(typeof(ContentAttribute));
            }).AddFluentValidation();
            //// ���U Content Validator
            services.AddTransient<IValidator<MemberCardInfoContent>, MemberCardInfoContentValidator>();
            services.AddTransient<IValidator<MemberForgetPasswordContent>, MemberForgetPasswordContentValidator>();
            services.AddTransient<IValidator<MemberLoginContent>, MemberLoginContentValidator>();
            services.AddTransient<IValidator<MemberMobileBindContent>, MemberMobileBindContentValidator>();
            services.AddTransient<IValidator<MemberRegisterContent>, MemberRegisterContentValidator>();
            services.AddTransient<IValidator<MemberRequestForgetPasswordContent>, MemberRequestForgetPasswordContentValidator>();
            services.AddTransient<IValidator<MemberRequestMobileBindContent>, MemberRequestMobileBindContentValidator>();
            services.AddTransient<IValidator<MemberUpdateNotifyTokenContent>, MemberUpdateNotifyTokenContentValidator>();
            services.AddTransient<IValidator<MemberUpdatePasswordContent>, MemberUpdatePasswordContentValidator>();

            services.AddTransient<IValidator<InteractiveContent>, InteractiveContentValidator>();

            services.AddTransient<IValidator<AddRideDataContent>, AddRideDataContentValidator>();
            services.AddTransient<IValidator<ReplyRideGroupContent>, ReplyRideGroupContentValidator>();
            services.AddTransient<IValidator<UpdateRideGroupContent>, UpdateRideGroupContentValidator>();
            services.AddTransient<IValidator<UpdateRideGroupCoordinateContent>, UpdateRideGroupCoordinateContentValidator>();

            services.AddTransient<IValidator<TeamContent>, TeamContentValidator>();
            services.AddTransient<IValidator<TeamCreateContent>, TeamCreateContentValidator>();
            services.AddTransient<IValidator<TeamSearchContent>, TeamSearchContentValidator>();
            services.AddTransient<IValidator<TeamKickContent>, TeamKickContentValidator>();
            services.AddTransient<IValidator<TeamBrowseContent>, TeamBrowseContentValidator>();
            services.AddTransient<IValidator<TeamChangeLeaderContent>, TeamChangeLeaderContentValidator>();
            services.AddTransient<IValidator<TeamUpdateViceLeaderContent>, TeamUpdateViceLeaderContentValidator>();
            services.AddTransient<IValidator<TeamAddActivityContent>, TeamAddActivityContentValidator>();
            services.AddTransient<IValidator<TeamActivityContent>, TeamActivityContentValidator>();
            services.AddTransient<IValidator<TeamActivityDetailContent>, TeamActivityDetailContentValidator>();
            services.AddTransient<IValidator<TeamApplyJoinContent>, TeamApplyJoinContentValidator>();
            services.AddTransient<IValidator<TeamResponseApplyJoinContent>, TeamResponseApplyJoinContentValidator>();
            services.AddTransient<IValidator<TeamBulletinContent>, TeamBulletinContentValidator>();
            services.AddTransient<IValidator<TeamAddBulletinContent>, TeamAddBulletinContentValidator>();

            //// ���� Model Binding ���Ҥ��L�� 400 Error ���\��A�H�K�Ȼs�Ʀ^���T��
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        /// <summary>
        /// Swagger �B�z��
        /// </summary>
        /// <param name="services">services</param>
        private void SwaggerHandler(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoBike API", Version = "v1", Description = "GoBike ���� API" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "GoBike.API.Swagger.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="app">app</param>
        /// <param name="env">env</param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseMiddleware<ContentMiddleware>();
            //app.UseHttpsRedirection(); // �j��ϥ� HTTPS Cors (�����ѱ��A�������ҦA�^�Ӹ�)
            app.UseRouting();
            app.UseCors("ProductNoPolicy"); // �����إߦb  app.UseMvc ���e
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoBike API");
            });

            #endregion Swagger
        }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services">services</param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));

            this.ConfigurationHandler();
            this.AuthHandler(services);
            this.DependencyInjectionHandler(services);
            this.FluentValidationHandler(services);
            this.SwaggerHandler(services);
            services.AddCors(options =>
            {
                // CorsPolicy �O�ۭq�� Policy �W��
                options.AddPolicy("ProductNoPolicy", policy =>
                {
                    //policy.WithOrigins("http://mgtgobike.hopto.org:18595")
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }
}