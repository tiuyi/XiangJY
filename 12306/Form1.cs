using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;

namespace _12306
{
    public partial class Form1 : Form
    {
        public static CookieCollection co = new CookieCollection();
        public string randCode = "";
        string path = AppDomain.CurrentDomain.BaseDirectory + "aa.jpg";
        public Form1()
        {
            InitializeComponent();

            HttpWebResponse result = WebTools.CreateGetHttpResponse("https://kyfw.12306.cn/otn/passcodeNew/getPassCodeNew?module=login&rand=sjrand&0.008961794161896242", null, null, null);


            Stream st = result.GetResponseStream();
            //Image img = Image.FromStream(st);
            //img.Save(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write), System.Drawing.Imaging.ImageFormat.Jpeg);
            //img.Dispose();
            Bitmap sourcebm = new Bitmap(st);//初始化Bitmap图片
            this.pictureBox2.Image = sourcebm;
            st.Close();
             
            //JSESSIONID=0A01E83FFCAB01B83251C1BA949422B34BAAAE898A; Path=/otn,current_captcha_type=Z; Path=/,BIGipServerotn=1072169226.64545.0000; path=/

            string cookie = result.Headers.Get("Set-Cookie");
            SaveCookie(cookie);
            result.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            #region 模拟验证码
            IDictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1["randCode"] = randCode.TrimEnd(',').Replace(",", "%2C"); 
            dic1["rand"] = "sjrand";
            
            HttpWebResponse result4 = WebTools.CreatePostHttpResponse("https://kyfw.12306.cn/otn/passcodeNew/checkRandCodeAnsyn", dic1, null, null, Encoding.Default, co);
            #endregion

            #region 模拟登陆
            IDictionary<string, string> dic = new Dictionary<string, string>();
            dic["loginUserDTO.user_name"] = "583477919%40qq.com"; //"583477919@qq.com";
            dic["userDTO.password"] = "qaz123";
            dic["randCode"] = randCode.TrimEnd(',').Replace(",", "%2C");

            HttpWebResponse result = WebTools.CreatePostHttpResponse("https://kyfw.12306.cn/otn/login/loginAysnSuggest", dic, null, null, Encoding.Default, co);
            SaveCookie(result.Headers.Get("Set-Cookie"));
            Stream stream = result.GetResponseStream();     //获取流，该流用于读取来自服务器的响应的体
            StreamReader readerOfStream = new StreamReader(stream, Encoding.GetEncoding("utf-8"));


            string strHTML = readerOfStream.ReadToEnd();   //读取来自流的当前位置到结尾的所有字符。
            readerOfStream.Close();
            stream.Close();
            result.Close();

            HttpWebResponse result1 = WebTools.CreatePostHttpResponse("https://kyfw.12306.cn/otn/login/loginAysnSuggest", dic, null, null, Encoding.Default, co);
            Stream stream1 = result1.GetResponseStream();     //获取流，该流用于读取来自服务器的响应的体
            StreamReader readerOfStream1 = new StreamReader(stream1, Encoding.GetEncoding("utf-8"));


            string strHTML1 = readerOfStream1.ReadToEnd();   //读取来自流的当前位置到结尾的所有字符。
            readerOfStream1.Close();
            stream1.Close();
            result1.Close(); 
            #endregion
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            var con = (sender) as Control;
            var point = con.PointToClient(Control.MousePosition);
            randCode += point.X+","+(point.Y-30)+","; 
        }

        /// <summary>
        /// 保存cookie
        /// </summary>
        public void SaveCookie(string cookieStr)
        { 
            string[] strArr = cookieStr.Split(',');
            for (int i = 0; i < strArr.Length; i++)
            {
                string[] newArr = strArr[i].Split('=');
                string name = newArr[0];
                string value = newArr[1].Split(';')[0];
                string path = newArr[2];
                Cookie coki = new Cookie(name, value, path, "kyfw.12306.cn");

                int flag = 0;
                for (int j = 0; j < co.Count; j++)
                {
                    if (co[j].Name == name)
                    {
                        co[j].Value = value;
                        flag++;
                    } 
                }
                if (flag == 0)
                {
                    co.Add(coki); 
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }


    
}
