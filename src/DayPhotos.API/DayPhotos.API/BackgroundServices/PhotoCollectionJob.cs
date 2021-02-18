using DayPhoto.API.Common;
using DayPhoto.API.Entities;
using DayPhotos.API.Common;
using DayPhotos.API.Entities;
using DayPhotos.API.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static DayPhoto.API.Common.Constants;

namespace DayPhotos.API.BackgroundServices
{
    [DisallowConcurrentExecution]
    public class PhotoCollectionJob : IJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private AppDbContext dbContext;

        public PhotoCollectionJob(ILogger<PhotoCollectionJob> logger,
            IServiceProvider serviceProvider)
        {
           this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this.logger.LogInformation($"[BackgroundService]PhotoCollectionJob run at {DateTime.Now}.");

            try
            {
                await this.GetBingPhotoAsync();
                await this.GetNGPhotoAsync(DateTime.Today);
                this.logger.LogInformation($"[BackgroundService]PhotoCollectionJob end at {DateTime.Now}.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
        }

        public async Task GetBingPhotoAsync(int from = 0, int takeCount = 1)
        {
            //Get photo url form Bing
            ExternalPhotoDto.BingImageOutput bingImageOutput = null;
            using (var httpClient = new HttpClient())
            {
                //httpClient.BaseAddress = new Uri(Constants.PhotoUrls.BingUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                var subUrl = string.Format(Constants.PhotoUrls.BingPhotoListSubUrl);
                var response = await httpClient.GetAsync(subUrl);
                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError(
                        $" Call {subUrl} error," +
                        $"StatusCode: {response.StatusCode}," +
                        $"Error: {await response.Content.ReadAsStringAsync()}- {DateTime.Now}");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                this.logger.LogInformation(responseString);
                bingImageOutput = JsonConvert.DeserializeObject<ExternalPhotoDto.BingImageOutput>(responseString);
            }

            //Save DB
            if (bingImageOutput != null && bingImageOutput.Images != null && bingImageOutput.Images.Count > 0)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    this.dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    foreach (var bingImage in bingImageOutput.Images)
                    {
                        if (!this.dbContext.Photos.Any(r => r.Date == bingImage.Date && r.Source == PhotoSource.Bing))
                        {
                            var photoEntity = new Photo();
                            photoEntity.Source = PhotoSource.Bing;
                            photoEntity.Date = bingImage.Date;
                            photoEntity.Url = Constants.PhotoUrls.BingUrl + bingImage.Url;
                            photoEntity.Title = bingImage.Title;
                            photoEntity.Description = "";
                            photoEntity.Copyright = bingImage.Copyright;
                            photoEntity.CreatedOn = DateTime.Now;

                            this.dbContext.Add(photoEntity);
                            await this.dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        public async Task GetNGPhotoAsync(DateTime date)
        {
            ExternalPhotoDto.NGImageOutput ngImageOutput = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var url = string.Format(Constants.PhotoUrls.NGPhotoListUrl, date);
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $" Call {url} error," +
                            $"StatusCode: {response.StatusCode}," +
                            $"Error: {await response.Content.ReadAsStringAsync()}- {DateTime.Now}");
                    }
                    var responseString = await response.Content.ReadAsStringAsync();
                    this.logger.LogInformation(responseString);
                    ngImageOutput = JsonConvert.DeserializeObject<ExternalPhotoDto.NGImageOutput>(responseString);
                }
                catch
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError(
                            $" Call {url} error," +
                            $"StatusCode: {response.StatusCode}," +
                            $"Error: {await response.Content.ReadAsStringAsync()}- {DateTime.Now}");
                    }
                    var responseString = await response.Content.ReadAsStringAsync();
                    this.logger.LogInformation(responseString);
                    ngImageOutput = JsonConvert.DeserializeObject<ExternalPhotoDto.NGImageOutput>(responseString);
                }
            }
            if (ngImageOutput != null && ngImageOutput.Images != null && ngImageOutput.Images.Count > 0)
            {
                using (var scope = this.serviceProvider.CreateScope())
                {
                    this.dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    foreach (var ngImage in ngImageOutput.Images)
                    {
                        if (!this.dbContext.Photos.Any(r => r.Date == ngImage.Date && r.Source == PhotoSource.NationalGeographic))
                        {
                            var photoEntity = new Photo();

                            photoEntity.Source = PhotoSource.NationalGeographic;
                            photoEntity.Date = ngImage.Date;
                            photoEntity.Url = ngImage.ImageDetail.Url;
                            photoEntity.Title = ngImage.ImageDetail.Title;
                            //photoEntity.Description = ngImage.ImageDetail.Description;
                            photoEntity.Copyright = ngImage.ImageDetail.Copyright;
                            photoEntity.CreatedOn = DateTime.Now;
                            if (!string.IsNullOrEmpty(ngImage.ImageDetail.Description))
                            {
                                var transDescription = await this.TranslateENToZHAsync(ngImage.ImageDetail.Description);
                                if (!string.IsNullOrEmpty(transDescription))
                                {
                                    photoEntity.Description = transDescription;
                                }
                                else
                                {
                                    photoEntity.Description = ngImage.ImageDetail.Description;
                                }
                            }

                            this.dbContext.Add(photoEntity);
                            await this.dbContext.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private async Task<string> TranslateENToZHAsync(string q)
        {
            var result = string.Empty;

            string from = "en";
            string to = "zh";
            string appId = BaiduTranslate.AppId;
            string secretKey = BaiduTranslate.SecretKey;
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            string sign = Helper.EncryptStringToMD5(appId + q + salt + secretKey);
            string url = BaiduTranslate.Url;
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;

            BaiduTranslateOutput baiduTranslateOutput = null;
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError(
                        $" Call {url} error," +
                        $"StatusCode: {response.StatusCode}," +
                        $"Error: {await response.Content.ReadAsStringAsync()}- {DateTime.Now}");
                }
                var responseString = await response.Content.ReadAsStringAsync();
                baiduTranslateOutput = JsonConvert.DeserializeObject<BaiduTranslateOutput>(responseString);
            }
            if (baiduTranslateOutput != null && baiduTranslateOutput.TransResult != null && baiduTranslateOutput.TransResult.Count > 0)
            {
                result = baiduTranslateOutput.TransResult.First().Dst;
            }
            return result;
        }
    }
}
