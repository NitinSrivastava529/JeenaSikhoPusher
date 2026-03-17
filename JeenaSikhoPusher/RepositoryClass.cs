using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JeenaSikhoPusher
{
    class RepositoryClass : IDisposable
    {
        BackgroundWorker bgWorker = new BackgroundWorker();
        Timer timer = new Timer();
        string _result = string.Empty;
        public RepositoryClass()
        {
            bgWorker.DoWork += new DoWorkEventHandler(bgDataSynck_DoWork);
            timer.Tick += new EventHandler(timer_Tick);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync();
            }            
        }
        public void StartSyncProcess()
        {
            timer.Enabled = true;
            timer.Start();

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        private void bgDataSynck_DoWork(object sender, DoWorkEventArgs e)
        {
            InitTestTask().GetAwaiter().GetResult();
        }
        public async Task InitTestTask()
        {
            try
            {
                await PostPatientData();
                await CancelPatientData();
            }
            catch (Exception ex)
            {
                SyncLog log = new SyncLog();
                log.process_result = ex.Message;
                log.process_time = System.DateTime.Now;
                ContextMenus.SyncLogList.Add(log);
                ContextMenus.lastmessage = _result;
            }
        }
        public async Task CancelPatientData()
        {
            ContextMenus.lastrun = System.DateTime.Now.ToString();
            DataSet ds = JeenaSikhoPusherQueries("-", "-", "-", "GetCancelPatientForSync");
            DataSet dsData = new DataSet();
            _result = "Success";
            CancelReportModel request = new CancelReportModel();
            string visitNoForPush = "-";
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    visitNoForPush = dr["VisitNo"].ToString();
                    dsData = JeenaSikhoPusherQueries("-", dr["VisitNo"].ToString(), "-", "CancelReportData");
                    try
                    {
                        if (dsData.Tables[0].Rows.Count > 0)
                        {
                            DataRow row = dsData.Tables[0].Rows[0];

                            request.order_id = row["order_id"].ToString();
                            request.externalVisitNo = row["externalVisitNo"].ToString();
                            request.cancelRemark = row["cancelRemark"].ToString();
                        }
                        request.testBookingItems = new List<TestItem>();
                        foreach (DataRow row in dsData.Tables[1].Rows)
                        {
                            request.testBookingItems.Add(new TestItem
                            {
                                itemId = row["ItemId"].ToString()
                            });
                        }
                        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyAgImV4cCI6IDMxLTEyLTIwMjMsICAiaWF0IjogMzEtMTItMjAyMywgICJpc3MiOiAiYWRkbWluLmhpaW1zLmluIiwgICJwYXRuYXIiOiAiY2hhbmRhbiBsYWIifQ==";
                        ResultResponse res = await CancelPatientReportAsync(request, token);
                        _result = res.message;
                        if (res.message.Contains("successfully"))
                        {
                            JeenaSikhoPusherQueries("-", visitNoForPush, "-", "CancelUpdateSync");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            ContextMenus.lastmessage = _result;
        }
        public async Task PostPatientData()
        {
            ContextMenus.lastrun = System.DateTime.Now.ToString();
            DataSet ds = JeenaSikhoPusherQueries("-", "-", "-", "GetPatientForSync");
            DataSet dsData = new DataSet();
            _result = "Success";
            string visitNoForPush = "-";
            PostDataObject request = new PostDataObject();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    visitNoForPush = dr["VisitNo"].ToString();
                    dsData = JeenaSikhoPusherQueries("-", dr["VisitNo"].ToString(), "-", "PatientReport");
                    try
                    {

                        if (dsData.Tables[0].Rows.Count > 0)
                        {
                            DataRow row = dsData.Tables[0].Rows[0];

                            request.uhid = row["uhid"].ToString();
                            request.contactNumber = row["contactNumber"].ToString();
                            request.reportUrl = row["reportUrl"].ToString();
                            request.clientId = row["clientId"].ToString();
                            request.externalVisitNo = row["externalVisitNo"].ToString();
                            request.remark = row["remark"].ToString();
                        }
                        request.testBookingItems = new List<TestBookingItem>();
                        foreach (DataRow row in dsData.Tables[1].Rows)
                        {
                            request.testBookingItems.Add(new TestBookingItem
                            {
                                itemId = Convert.ToInt32(row["ItemId"]),
                                rate = Convert.ToInt32(row["rate"]),
                                discount = Convert.ToInt32(row["discount"]),
                                net_rate = Convert.ToInt32(row["net_rate"])
                            });
                        }

                        request.payment = new Payment
                        {
                            payment_mode = new List<string>(),
                            payment_method = new List<string>(),
                            amount = new List<int>()
                        };

                        foreach (DataRow row in dsData.Tables[2].Rows)
                        {
                            request.payment.payment_mode.Add(row["payment_mode"].ToString());
                            request.payment.payment_method.Add(row["payment_method"].ToString());
                            request.payment.amount.Add(Convert.ToInt32(row["amount"]));
                        }
                        string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyAgImV4cCI6IDMxLTEyLTIwMjMsICAiaWF0IjogMzEtMTItMjAyMywgICJpc3MiOiAiYWRkbWluLmhpaW1zLmluIiwgICJwYXRuYXIiOiAiY2hhbmRhbiBsYWIifQ==";
                        ResultResponse res = await SendPatientReportAsync(request, token);
                        _result = res.message;
                        if (res.message.Contains("successfully"))
                        {
                            JeenaSikhoPusherQueries("-", visitNoForPush, res.order_id.ToString(), "UpdateSync");
                        }
                    }
                    catch (Exception ex) { }
                }

            }
            ContextMenus.lastmessage = _result;
        }
        public async Task<ResultResponse> SendPatientReportAsync(PostDataObject requestData, string token)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://stagging.hiims.in/diagnosticServices/sendPatientReportViaChandan"
                );

                // Authorization header
                request.Headers.Add("Authorization", "Bearer " + token);

                //// Cookie header (exact same as Postman)
                request.Headers.Add("Cookie", "ci_session=90a050d0ae87f6c39e1ac5f4e8e1317e78fe04fb; ci_session=d0c6fe2fc887da87661081abbee127144bb8084f");

                // Body
                var json = JsonConvert.SerializeObject(requestData);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResultResponse>(result);
            }
        }
        public async Task<ResultResponse> CancelPatientReportAsync(CancelReportModel requestData, string token)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://stagging.hiims.in/diagnosticServices/cancelReportFromChandan"
                );

                // Authorization header
                request.Headers.Add("Authorization", "Bearer " + token);

                //// Cookie header (exact same as Postman)
                request.Headers.Add("Cookie", "ci_session=90a050d0ae87f6c39e1ac5f4e8e1317e78fe04fb; ci_session=d0c6fe2fc887da87661081abbee127144bb8084f");

                // Body
                var json = JsonConvert.SerializeObject(requestData);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ResultResponse>(result);
            }
        }
        public DataSet JeenaSikhoPusherQueries(string UHID, string IPOPNo, string Prm1, string Logic)
        {
            string processInfo = string.Empty;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(GlobalConfig.strConnLimsDB))
            {
                using (SqlCommand cmd = new SqlCommand("pJeenaSikhoPusherQueries", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 2500;
                    cmd.Parameters.Add("@UHID", SqlDbType.VarChar, 30).Value = UHID;
                    cmd.Parameters.Add("@IPOPNo", SqlDbType.VarChar, 30).Value = IPOPNo;
                    cmd.Parameters.Add("@Prm1", SqlDbType.VarChar, 100).Value = Prm1;
                    cmd.Parameters.Add("@Logic", SqlDbType.VarChar, 1000).Value = Logic;
                    try
                    {
                        con.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(ds);
                        processInfo = "Success";
                        con.Close();
                    }
                    catch (SqlException sqlEx)
                    {
                        ds = null;
                        processInfo = sqlEx.Message;
                    }
                    finally { con.Close(); }
                    return ds;
                }
            }
        }
    }
}
