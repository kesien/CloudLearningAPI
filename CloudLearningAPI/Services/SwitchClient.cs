using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using CloudLearningAPI.Settings;

namespace CloudLearningAPI.Services
{
    public class SwitchClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _container;
        private readonly SwitchConnectionSettings _settings;

        #region Constructor
        public SwitchClient(HttpClient httpClient, IConfiguration config, CookieContainer container)
        {
            _httpClient = httpClient;
            _container = container;
            _settings = config.GetSection(nameof(SwitchConnectionSettings)).Get<SwitchConnectionSettings>();
        }
        #endregion

        #region LoginAsync
        public async Task<bool> LoginAsync(string username, string password)
        {
            var cookies = _container.GetCookies(new Uri(_settings.SwitchBoardHost));
            if (cookies is not null) 
            {
                foreach (Cookie cookie in cookies)
                {
                    cookie.Expired = true;
                }
            }
            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("usr", username),
                new KeyValuePair<string, string>("pwd", password),
            });

            HttpResponseMessage result = await _httpClient.PostAsync($"{_settings.SwitchBoardHost}fl-teach/main.php", content);
            string response = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != HttpStatusCode.OK || response.Contains("Invalid username or password"))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region GenerateExcelList
        /// <summary>
        /// Send a GET request to generate the excel list.
        /// </summary>
        /// <param name="courseNumbers">List of coursenumbers</param>
        /// <returns>The response as string. E.g.: 'documents/kursteilnehmer_6208.xls'</returns>
        private async Task<string> GenerateExcelListAsync(List<string> courseNumbers)
        {
            UriBuilder builder = new(_settings.SwitchBoardHost + "fl-teach/mkReport.php");
            builder.Port = -1;
            NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
            query["rType"] = "5";
            query["lesson_id"] = string.Join(",", courseNumbers);
            query["output"] = "2";
            builder.Query = query.ToString();
            var response = await _httpClient.GetStringAsync(builder.ToString());
            if (response.Contains("ERROR")) 
            {
                return null;
            }
            return response;
        }
        #endregion

        #region DownloadExcelFile
        /// <summary>
        /// Download the generated Excel list.
        /// </summary>
        /// <param name="courseNumbers">List of coursenumbers</param>
        /// <returns></returns>
        public async Task<Stream> DownloadListAsync(List<string> courseNumbers)
        {
            string path = await GenerateExcelListAsync(courseNumbers);
            if (path is null) 
            {
                return null;
            }
            var response = await _httpClient.GetAsync($"{_settings.SwitchBoardHost}cgi-bin/flshow.exe?doc={path}");
            string content = await response.Content.ReadAsStringAsync();
            if (content.Contains("invalid document id"))
            {
                return null;
            }
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion
    }
}