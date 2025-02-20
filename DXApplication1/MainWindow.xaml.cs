using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Grid;

namespace DXApplication1
{
    public class RootResponse
    {
        [JsonProperty("Ondate")]
        public DateTime Ondate { get; set; }

        [JsonProperty("Status")]
        public int Status { get; set; }

        [JsonProperty("Error")]
        public ErrorInfo Error { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }

        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("ClientId")]
        public string ClientId { get; set; }
    }

    public class ErrorInfo
    {
        [JsonProperty("Code")]
        public int Code { get; set; }

        [JsonProperty("Sig")]
        public string Sig { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    public class ConsumptionItem
    {
        [JsonProperty("CUSTOMER"), DisplayName("Покупатель")]
        public string CUSTOMER { get; set; }

        [JsonProperty("NAME_PCLASS")]
        public string NAME_PCLASS { get; set; }

        [JsonProperty("ART")]
        public string ART { get; set; }

        [JsonProperty("NAME")]
        public string NAME { get; set; }

        [JsonProperty("USERNAME")]
        public string USERNAME { get; set; }

        [JsonProperty("COMMENTS")]
        public string COMMENTS { get; set; }
        
        [JsonProperty("CHECKED")]
        [DefaultValue(false)]
        public bool? CHECKED { get; set; } = false;

        [JsonProperty("DT_CONFIRMED"), DisplayName("Дата подтверждения")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? DT_CONFIRMED { get; set; }
    }

    public class ApiRequest
    {
        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("Module")]
        public string Module { get; set; }

        [JsonProperty("Object")]
        public string Object { get; set; }

        [JsonProperty("Action")]
        public string Action { get; set; }

        [JsonProperty("Params")]
        public ApiParams Params { get; set; }
    }

    public class ApiParams
    {
        [JsonProperty("DATE_FROM")]
        public string DATE_FROM { get; set; }

        [JsonProperty("DATE_TO")]
        public string DATE_TO { get; set; }
    }

    public partial class MainWindow : ThemedWindow
    {
        public ObservableCollection<ConsumptionItem> ConsumptionList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                await LoadDataToGrid();
            };

            var cond1 = new FormatCondition
            {
                FieldName = "CHECKED",
                ValueRule = ConditionRule.Equal,
                Expression = "[CHECKED] = null",
                ApplyToRow = true,
                Format = new Format
                {
                    Background = new SolidColorBrush(Colors.Coral)
                }
            };
            tableView1.FormatConditions.Add(cond1);
        }

        public async Task LoadDataToGrid()
        {
            var requestBody = new ApiRequest
            {
                Token = "EF27C4B772178EE489EE8E067A3525CC",
                Module = "Rig",
                Object = "Consumption",
                Action = "List",
                //Params = new ApiParams
                //{
                //    DATE_FROM = "13.01.2020",
                //    DATE_TO = "13.02.2025"
                //}
            };

            string jsonRequest = JsonConvert.SerializeObject(requestBody);

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://localhost:5678/api/4/", content);

                    string result = await response.Content.ReadAsStringAsync();

                    var root = JsonConvert.DeserializeObject<RootResponse>(result);

                    var consumptionList = JsonConvert.DeserializeObject<List<ConsumptionItem>>(root.Data);

                    ConsumptionList = new ObservableCollection<ConsumptionItem>(consumptionList);
                    gridControl1.ItemsSource = ConsumptionList;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Task task = LoadDataToGrid();
        }
    }
}
