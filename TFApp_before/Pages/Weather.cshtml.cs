namespace TFApp.Pages;

public class WeatherModel : PageModel
{
    private readonly TFAppContext _context;
    // [演習による変更]HTTPクライアントの依存注入
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    // [演習による変更]コンテキストアクセサ
    private readonly IHttpContextAccessor _httpContextAccessor;

    // [演習による変更]ロガー
    private readonly ILogger<WeatherModel> _logger;

    public WeatherModel(
        TFAppContext context,
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<WeatherModel> logger)
    {
        _context = context;
        // [演習による変更]IHttpClientFactory インターフェースにアクセスする
        _httpClientFactory = httpClientFactory;
        // [演習による変更]ContextへのアクセスはDIを使用する
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
    }

    public string? UserId { get; private set; }

    public Weather? Weather { get; set; }

    public async Task OnGetAsync()
    {
        var session = _httpContextAccessor.HttpContext.Session;
        // [演習による変更]SessionはDIから取得するように修正
        // var session = HttpContext.Session;
        var key = session?.GetString(RegisterModel.SessionKey);
        UserId = key;

        if (_context.User != null)
        {
            // セッションと同じユーザーをDBから取得
            var user = await _context.User.FindAsync(key);

            // weather-apiをたたく
            if (user != null)
            {
                //[演習による変更]依存性の注入をしたライブラリでAPIを実行する
                var client = _httpClientFactory.CreateClient("weather");
                client.DefaultRequestHeaders.Add("x-api-key", _configuration.GetValue<string>("ApiKey"));
                var response = await client.GetAsync($"api/weather/{user.City}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    Weather = JsonSerializer.Deserialize<Weather>(responseBody);

                    _logger.LogInformation($"{DateTime.Now:F}: weather-apiのコールに成功しました");
                }
                /*
                // 依存性の注入対応前
                using (var client = new HttpClient())
                {
                    // ヘッダーにApiKeyを付与
                    client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("ApiKey"));

                    var response = await client.GetAsync($"https://fnappdf4khwh57ruqq.azurewebsites.net/api/weather/{user.City}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();

                        Weather = JsonSerializer.Deserialize<Weather>(responseBody);

                        System.IO.File.AppendAllText(@"./log.txt", $"{DateTime.Now:F}: weather-apiのコールに成功しました\n");
                    }
                }
                */
            }
            else
            {
                Weather = null;

                _logger.LogError($"{DateTime.Now:F}: ユーザーの登録処理が失敗しました\n");
            }
        }
    }
}
