var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// [演習による変更]HttpContext へのアクセスを登録する
builder.Services.AddHttpContextAccessor();

// 保存先をDBにする
builder.Services.AddDbContext<TFAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TFAppContext")));

// [演習による変更]メモリへのキャッシュはしない
// builder.Services.AddDistributedMemoryCache();

// [演習による変更]Azure Cache for Redis サービスにキャッシュする
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = builder.Environment.EnvironmentName.ToLower();
});

// [演習による変更]HTTPクライアントを登録する
builder.Services.AddHttpClient("weather", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://fnappdf4khwh57ruqq.azurewebsites.net/");
});

// セッションの設定
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
