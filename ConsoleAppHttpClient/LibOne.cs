using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppHttpClient
{
    public class LibOne
    {
        private static string fo_WorkingHoursPlanID = "";
        public static async Task<string> TestPostAsync()
        {
            //参数设置
            var SessionId = "ASP.NET_SessionId="+"xzahv422vvjq0045dxcd3l55";//"ASP.NET_SessionId=xzahv422vvjq0045dxcd3l55"
            var fo_Date = "2019-01-29";//填写日期？
            var fv_WorkingHoursWeek = "周一";//日报日期
            var fd_WorkingHoursDate = "2019-01-28";//日报日期
            var fo_SubmitType = "value_plansubmit"; //value_plansubmit:计划保存 value_workinghourssubmit:每天任务保存
            var ft_WorkingHoursPlan = "工作流完善，维度定义、维度系数接口完成";//计划内容
            var fv_WorkingHoursType = "研发";
            var fn_WorkingHours = "12";//工时
            var fv_DepartmentID = "0^167";//
            var fv_WorkingHoursContent = "testContent";//

            var handler = new HttpClientHandler() { UseCookies = false }; 
           
            using (HttpClient httpClient = new HttpClient(handler))
            {
                #region 设置请求头信息 不设置也有默认  
                httpClient.DefaultRequestHeaders.Add("Host", "183.129.165.98:8082"); 
                httpClient.DefaultRequestHeaders.Add("Referer", "http://183.129.165.98:8082/xQuant_xOA/SysFile/WebPage/WorkingHours_InfoListAdd.aspx?fo_Type=1");
                httpClient.DefaultRequestHeaders.Add("Cookie", SessionId);
                #endregion

                // 构造POST参数
                HttpContent postContent = new FormUrlEncodedContent(new Dictionary<string, string>()
                   { 
                        {"__VIEWSTATE", "/wEPDwULLTE5MjE1OTI0NjVkZPGLNn6dbcy6tZs//B0F6AV+jtQV"},
                        {"__EVENTVALIDATION","/wEWFwLiyridAgKQq462BQLulMr0DwLvhKXUDgLfkbD4DQKhu5rcAQKDk4y+CQLxjNa8BwL+3825DALfzI3pBwK3ysSlBALLjb7HBALt59SLBAKChcmoCAL4no2uAQKt+dGODwLX+e2+DwLxhICbCALh5sykBwKP0IezDgKC8qWrDwLX5dq1BAKMz6WHA14DD8Dcue5+A+Uvs9vGz3jX21Te"},
                        {"fo_SelectIndex",""},
                        {"fo_SelectOne",""},
                        {"fo_Date",fo_Date},
                        {"ctl00$page_Body$ft_WorkingHoursPlan",ft_WorkingHoursPlan},
                        {"ctl00$page_Body$ft_WorkingHoursPlan_1",""},
                        {"ctl00$page_Body$fv_WorkingHoursWeek",fv_WorkingHoursWeek},
                        {"ctl00$page_Body$fv_WorkingHoursWeek_Select",fv_WorkingHoursWeek},
                        {"ctl00$page_Body$fd_WorkingHoursDate",fd_WorkingHoursDate},
                        {"ctl00$page_Body$fv_WorkingHoursStage","全天"},
                        {"ctl00$page_Body$fv_WorkingHoursStage_Select","全天"},
                        {"ctl00$page_Body$fv_WorkingHoursContent",fv_WorkingHoursContent},
                        {"ctl00$page_Body$fv_WorkingHoursType",fv_WorkingHoursType},
                        {"ctl00$page_Body$fv_WorkingHoursType_Select",fv_WorkingHoursType},
                        {"ctl00$page_Body$fv_WorkingHoursPlace","杭州"},
                        {"ctl00$page_Body$fn_WorkingHours", fn_WorkingHours},
                        {"ctl00$page_Body$fv_DepartmentID",fv_DepartmentID},
                        {"ctl00$page_Body$fv_DepartmentID_Select",fv_DepartmentID},
                        {"ctl00$page_Body$fn_WorkingHoursComplete","100"},
                        {"ctl00$page_Body$fo_Type_1",""},
                        {"ctl00$page_Body$fo_SubmitType",fo_SubmitType},
                        {"ctl00$page_Body$fo_WorkingHoursPlanID",fo_WorkingHoursPlanID},//"100841"
                        {"ctl00$page_Body$fo_AID",""},
                        {"fo_WebBack",""}
            });

                HttpResponseMessage response = await httpClient.PostAsync("http://183.129.165.98:8082/xQuant_xOA/SysFile/WebPage/WorkingHours_InfoListAdd.aspx?fo_Type=1 ", postContent);

                response.EnsureSuccessStatusCode();
                string resultStr = await response.Content.ReadAsStringAsync();

                #region response的其他信息

                //Console.WriteLine("响应是否成功：" + response.IsSuccessStatusCode); 
                //Console.WriteLine("响应头信息如下：\n");
                //var headers = response.Headers;
                //foreach (var header in headers)
                //{
                //    Console.WriteLine("{0}: {1}", header.Key, string.Join("", header.Value.ToList()));
                //}

                #endregion
                if (string.IsNullOrEmpty(fo_WorkingHoursPlanID))
                {
                    fo_WorkingHoursPlanID = resultStr.Substring(resultStr.IndexOf("ctl00_page_Body_fo_WorkingHoursPlanID") + 46, 6);
                } 

                return resultStr;
            }
        }
    }
}
