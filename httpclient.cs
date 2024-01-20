using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace etcdii
{
    public enum Operate
    {
        Version,
        AllKeys,
        PutKey,  //插入KEY
        DeleteRange, //删除
    }


    //ETCD Struct Version
    public struct EtcdVersion
    {
        public string etcdserver { get; set; }
        public string etcdcluster { get; set; }
    }

    public struct EtcdKvRange
    {
        public string count { get; set; }
        public EtcdHeader header { get; set; }
        public EtcdKvs[] kvs { get; set; }
    }

    public struct EtcdHeader
    {
        public string cluster_id { get; set; }
        public string member_id { get; set; }
        public string revision { get; set; }
        public string raft_term { get; set; }
    }

    public struct EtcdKvs
    {
        public string key { get; set; }
        public string create_revision { get; set; }
        public string mod_revision { get; set; }
        public string version { get; set; }
        public string value { get; set; }
    }

    internal class httpclient
    {
        public static string GLOBAL_ENDPOINT;
        public static bool GLOBAL_CONNECT_STATUS;


        public static string GetEtcdValue(Operate o)
        {
            HttpClient httpClient = new HttpClient();
            string urlStr = "";
            switch (o)
            {
                case Operate.Version:
                    urlStr = String.Format("http://{0}/version", httpclient.GLOBAL_ENDPOINT);
                    break;
                default:
                    break;
            }


            try
            {
                var response = httpClient.GetStringAsync(urlStr).Result;

                return response.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }


        }


        public static string PostEtcdValue(Operate o, string data)
        {
            //set client
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent(data);
            string urlStr = "";
            switch (o)
            {
                case Operate.AllKeys:
                    urlStr = String.Format("http://{0}/v3/kv/range", httpclient.GLOBAL_ENDPOINT);
                    break;
                case Operate.PutKey:
                    urlStr = String.Format("http://{0}/v3/kv/put", httpclient.GLOBAL_ENDPOINT);
                    break;
                case Operate.DeleteRange:
                    urlStr = String.Format("http://{0}/v3/kv/deleterange", httpclient.GLOBAL_ENDPOINT);
                    break;
                default:
                    break;
            }

            try
            {
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = httpClient.PostAsync(urlStr, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }

    }
}
