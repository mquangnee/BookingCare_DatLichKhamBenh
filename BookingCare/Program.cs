using BookingCare.Data.Seed;
using BookingCare.Models;
using BookingCare.Repository;
using BookingCare.Services;
using BookingCare.Services.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BookingCare
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Kết nối đến SQL Server
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
            });

            //Add identity
            //builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<DataContext>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            //Yêu cầu về mật khẩu
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true; //Yêu cầu chữ số
                options.Password.RequireLowercase = true; //Yêu cầu chữ thường
                options.Password.RequireNonAlphanumeric = true; //Yêu cầu ký tự đặc biệt
                options.Password.RequireUppercase = true; //Yêu cầu chữ hoa
                options.Password.RequiredLength = 6; //Độ dài tối thiểu
                options.Password.RequiredUniqueChars = 1; //Số ký tự đặc biệt
            });

            //Cấu hình dịch vụ email
            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IEmailTemplate, EmailTemplate>();
            builder.Services.AddTransient<OtpService>();

            // Thêm dịch vụ Session
            builder.Services.AddDistributedMemoryCache(); // Lưu session trong bộ nhớ tạm
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian tồn tại của session
                options.Cookie.HttpOnly = true; // Không cho client script truy cập cookie
                options.Cookie.IsEssential = true; // Bắt buộc có cookie ngay cả khi user từ chối cookie
            });

            var app = builder.Build();

            //Gọi hàm khởi tạo dữ liệu
            using (var scope = app.Services.CreateScope())
            {
                await DbInitializer.SeedAsync(scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // Kích hoạt Session
            app.UseSession();

            //---Cấu hình HTTPS và tệp tĩnh---
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //---Cấu hình Routing---
            app.UseRouting();

            //---Cấu hình xác thực và phân quyền---
            app.UseAuthentication();
            app.UseAuthorization();

            //---Cài đặt Route cho Project---
            //Route cho Area Admin
            app.MapControllerRoute(
                name: "Admin",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
            //Route mặc định
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
