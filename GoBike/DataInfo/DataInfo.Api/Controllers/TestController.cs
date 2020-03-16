using System.Threading.Tasks;
using DataInfo.Core.Extensions;
using DataInfo.Service.Interfaces.Server;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace DataInfo.Api.Controllers
{
    /// <summary>
    /// 測試 Api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger logger = LogManager.GetLogger("TestController");
        private readonly IWebSocketService webSocketService;

        public TestController(IWebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            this.logger.LogInfo("測試連線 Start", "1", null);
            await this.webSocketService.Connect();
            this.logger.LogInfo("測試連線 Start", "2", null);
            return Ok("123");
        }

        //private readonly IHostingEnvironment hostingEnvironment;
        //private readonly ILogger logger = LogManager.GetLogger("TestController");

        //public TestController(IHostingEnvironment _hostingEnvironment)
        //{
        //    this.hostingEnvironment = _hostingEnvironment;
        //}

        //[HttpGet]
        //[Authorize]
        //public IActionResult Get()
        //{
        //    return Ok("123");
        //}

        ///// <summary>
        ///// Get
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpGet]
        //public IActionResult Get()
        //{
        //    try
        //    {
        //        string da = "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";
        //        //byte[] bytes = Convert.FromBase64String(da);
        //        //var filePath = Path.Combine($"{Environment.CurrentDirectory}/Test/{Guid.NewGuid()}.jpg");
        //        //System.IO.File.WriteAllBytes(filePath, bytes);

        // //var bytes = Convert.FromBase64String(da); //using (var imageFile = new
        // FileStream("D://DoubleHappiness//dh.cdn.com//gobike/test.png", FileMode.Create)) //{ //
        // imageFile.Write(bytes, 0, bytes.Length); // imageFile.Flush(); //}
        // //this.logger.LogInfo("Test", $"Data: {JsonConvert.SerializeObject(bytes)}", null);

        // List<RideContentDto> dtos = new List<RideContentDto>(); for (int i = 1; i <= 10; i++) {
        // RideContentDto dto = new RideContentDto() { Text = "132456", Url = "4564646546454" };
        // dtos.Add(dto); };

        // this.logger.LogInfo("Test", $"Dtos: {JsonConvert.SerializeObject(dtos)} aes:
        // {Utility.EncryptAES(JsonConvert.SerializeObject(dtos))} base64:
        // {Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dtos)))}", null);

        //        return Ok("OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogInfo("Test", string.Empty, ex);
        //        return BadRequest("Error");
        //    }
        //}

        ///// <summary>
        ///// Post
        ///// </summary>
        ///// <returns>IActionResult</returns>
        //[HttpPost]
        //public IActionResult Post(PostData data)
        //{
        //    this.logger.Info("Test Img", $"Data: {data.Photo}", null);
        //    string da = data.Photo.Replace("data:image/jpeg;base64,", string.Empty);
        //    byte[] bytes = Convert.FromBase64String(da);
        //    var filePath = Path.Combine($"{Environment.CurrentDirectory}/Test/{Guid.NewGuid()}.jpg");
        //    System.IO.File.WriteAllBytes(filePath, bytes);

        //    return Ok("OK");
        //}

        ///// <summary>
        ///// Post
        ///// </summary>
        //public class PostData
        //{
        //    /// <summary>
        //    /// Gets or sets Data
        //    /// </summary>
        //    public string Photo { get; set; }
        //}
    }
}