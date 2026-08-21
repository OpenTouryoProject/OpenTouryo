//**********************************************************************************
//* テスト・コントローラー
//**********************************************************************************

// テスト・コントローラーなので、必要に応じて流用 or 削除して下さい。

//**********************************************************************************
//* クラス名        ：SampleDataController
//* クラス日本語名  ：疎通確認用
//*
//* 作成日時        ：－
//* 作成者          ：生技
//* 更新履歴        ：
//* 
//*  日時        更新者            内容
//*  ----------  ----------------  -------------------------------------------------
//*  2026/04/02  西野 大介         復元
//**********************************************************************************

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETWebService.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SampleDataController : ControllerBase
    {
        /// <summary>
        /// GET api/sampledata/weatherforecasts
        /// </summary>
        /// <returns>
        /// IEnumerable(string)
        /// </returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}
