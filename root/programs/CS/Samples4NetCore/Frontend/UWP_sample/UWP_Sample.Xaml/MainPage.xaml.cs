using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UWP_Sample.Xaml
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string rootUrl = "http://localhost:8888/JsonController/";

        /// <summary>constructor</summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region チェック

        /// <summary>toggleButton_Checked</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void toggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = true;
        }

        /// <summary>toggleButton_Unchecked</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void toggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = false;
        }

        #endregion

        #region ボタンイベント

        /// <summary>btnSelectCount_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnSelectCount_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                // Web API の URL
                Uri uri = new Uri(rootUrl + "SelectCount");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() }
                    });

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        message = jObject["Message"].ToString();
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnGetList_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnGetList_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                // Web API の URL
                Uri uri = new Uri(rootUrl + "SelectAll_DT");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpFormUrlEncodedContent content = new HttpFormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "ddlDap", ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString() },
                        { "ddlMode1", ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString() },
                        { "ddlMode2", ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString() },
                        { "ddlExRollback", ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString() }
                    });

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    // 正常終了
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var result = new { Message = "", Result = new List<Dictionary<string, string>>() };
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        List<Dictionary<string, string>> list =
                            JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jObject["Result"].ToString());

                        this.lstRecords.ItemsSource = list;
                        message = "正常終了しました";
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnGetRecord_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnGetRecord_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ


                if (string.IsNullOrEmpty(this.txtShipperID.Text))
                {
                    message = "検索する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに、検索する Shipper Id を入力してください。";
                    return;
                }

                // Web API の URL
                Uri uri = new Uri(rootUrl + "Select");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpStringContent content = new HttpStringContent(JsonConvert.SerializeObject(
                    new
                    {
                        ddlDap = ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString(),
                        ddlMode1 = ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString(),
                        ddlMode2 = ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString(),
                        ddlExRollback = ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString(),
                        Shipper = new
                        {
                            ShipperID = this.txtShipperID.Text
                        },
                    }),
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    // 正常終了
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        Dictionary<string, string> dic =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(jObject["Result"].ToString());

                        this.txtCompanyName.Text = dic["CompanyName"];
                        this.txtPhone.Text = dic["Phone"];
                        message = "正常終了しました";
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnInsert_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                if (string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text))
                {
                    message = "Company Name, Phone のいずれかが入力されていません。１件表示の Compnay Name, Phone テキストボックスに、追加する値を入力してください。（Shipper Id は、自動採番されます）";
                    return;
                }

                // Web API の URL
                Uri uri = new Uri(rootUrl + "Insert");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpStringContent content = new HttpStringContent(JsonConvert.SerializeObject(
                    new
                    {
                        ddlDap = ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString(),
                        ddlMode1 = ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString(),
                        ddlMode2 = ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString(),
                        ddlExRollback = ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString(),
                        Shipper = new
                        {
                            CompanyName = this.txtCompanyName.Text,
                            Phone = this.txtPhone.Text
                        },
                    }),
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    // 正常終了
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        message = jObject["Message"].ToString();
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnUpdate_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                if (string.IsNullOrEmpty(this.txtShipperID.Text) || string.IsNullOrEmpty(this.txtCompanyName.Text) || string.IsNullOrEmpty(this.txtPhone.Text))
                {
                    message = "Shipper Id, Company Name, Phone のいずれかが入力されていません。１件表示の Shipper Id テキストボックスに更新対象の Shipper Id の値を、Compnay Name, Phone テキストボックスに、更新する値をそれぞれ入力してください。";
                    return;
                }

                // Web API の URL
                Uri uri = new Uri(rootUrl + "Update");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpStringContent content = new HttpStringContent(JsonConvert.SerializeObject(
                    new
                    {
                        ddlDap = ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString(),
                        ddlMode1 = ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString(),
                        ddlMode2 = ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString(),
                        ddlExRollback = ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString(),
                        Shipper = new
                        {
                            ShipperId = this.txtShipperID.Text,
                            CompanyName = this.txtCompanyName.Text,
                            Phone = this.txtPhone.Text
                        },
                    }),
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    // 正常終了
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        message = jObject["Message"].ToString();
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnDelete_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                string message = string.Empty; // 結果メッセージ

                if (string.IsNullOrEmpty(this.txtShipperID.Text))
                {
                    message = "削除する Shipper Id が入力されていません。１件表示の Shipper Id テキストボックスに削除対象の Shipper Id の値を入力してください。";
                    return;
                }

                // Web API の URL
                Uri uri = new Uri(rootUrl + "Delete");

                // Request Header を定義する
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));

                // Web API に送信するデータを構築する
                HttpStringContent content = new HttpStringContent(JsonConvert.SerializeObject(
                    new
                    {
                        ddlDap = ((ComboBoxItem)this.ddlDap.SelectedItem).Tag.ToString(),
                        ddlMode1 = ((ComboBoxItem)this.ddlMode1.SelectedItem).Tag.ToString(),
                        ddlMode2 = ((ComboBoxItem)this.ddlMode2.SelectedItem).Tag.ToString(),
                        ddlExRollback = ((ComboBoxItem)this.ddlExRollback.SelectedItem).Tag.ToString(),
                        Shipper = new
                        {
                            ShipperId = this.txtShipperID.Text
                        },
                    }),
                    Windows.Storage.Streams.UnicodeEncoding.Utf8,
                    "application/json"
                );

                HttpResponseMessage response = await client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    // 正常終了
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject jObject = (JObject)JsonConvert.DeserializeObject(jsonResult);

                    if (jObject["Message"] != null)
                    {
                        // 正常終了
                        message = jObject["Message"].ToString();
                    }
                    else if (jObject["ErrorMSG"] != null)
                    {
                        // 業務例外
                        message = jObject["ErrorMSG"].ToString();
                    }
                    else if (jObject["ExceptionMSG"] != null)
                    {
                        // その他例外
                        message = jObject["ExceptionMSG"].ToString();
                    }
                }
                else
                {
                    // エラー終了
                    message = "HTTP Error! Status Code: " + response.StatusCode;
                }

                // 結果を表示
                this.txtResult.Text = message;
            }
        }

        /// <summary>btnClear_Click</summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // リストの内容をクリアする
            this.lstRecords.ItemsSource = null;
        }

        #endregion
    }
}
