using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF14GetChatlog
{
    using System.IO;
    using System.Windows.Forms;
    using Advanced_Combat_Tracker;
    public class ActPluginFirst : UserControl, IActPluginV1
    {
        private Button button1;

        //処理対象のログコード(例：001a say)を入れるリスト
        List<String> listChatType = new List<string> { };

        //コンストラクタ
        public ActPluginFirst()
        {
            //InitializeComponent();
        }
        public void DeInitPlugin()
        {
            //throw new NotImplementedException();
            ActGlobals.oFormActMain.OnLogLineRead -= new LogLineEventDelegate(oFormActionMain_OnLogLineRead);
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {

            pluginScreenSpace.Controls.Add(this);
            this.Dock = DockStyle.Fill;
            //throw new NotImplementedException();
            ActGlobals.oFormActMain.OnLogLineRead += new LogLineEventDelegate(oFormActionMain_OnLogLineRead);

            //処理対象とするチャットログのリストを作成する
            if (setChatType() == 1)
            {
                System.Diagnostics.Debug.WriteLine("チャットログのリストが作成できませんでした");
            }

        }
        /// <summary>
        /// ログ行が解析エンジンを通過する都度の処理
        /// </summary>
        /// 
        void oFormActionMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            String _strLogText = "";
            String _strLogType = "";
            String _strChatType = "";

            String _strDirname = Properties.Settings.Default.strChatLogDirname;
            string _strFilename = Properties.Settings.Default.strChatLogFilename;

            System.Diagnostics.Debug.WriteLine("■oFormActionMain_OnLogLineRead=Start：" + DateTime.Now);

            _strLogText = logInfo.logLine;
            //ログの23文字目～24文字目までを取得   -> 00
            _strLogType = _strLogText.Substring(23, 2);
            // ログが"00"(チャット)で、チャットタイプが処理対象リストに含まれる場合
            if (_strLogType.Equals("00"))                               //同値性比較
            {
                //ログの26文字目～29文字目までを取得   -> 0038
                _strChatType = _strLogText.Substring(26, 4);
                if (listChatType.Contains(_strChatType))
                {
                    if (!Directory.Exists(@".\" + _strDirname))
                    {
                        Directory.CreateDirectory(@".\" + _strDirname);
                    }
                    //ログの31文字目から最後までを切り取りファイルに出力する
                    File.AppendAllText(@".\"+ _strDirname + "./" + DateTime.Today.ToString($"{DateTime.Today:yyyyMMdd}") + _strFilename, _strChatType + "："+_strLogText.Substring(31) + "\r\n");
                }
            }
            System.Diagnostics.Debug.WriteLine("■oFormActionMain_OnLogLineRead=End：" + DateTime.Now);

        }

        /// <summary>
        /// 処理対象とするチャットログのリストを作成する
        /// </summary>
        int setChatType()
        {
            int ret = 1;
            string[] _arrayStrWork;
            listChatType = new List<string> { };

            foreach (String line in System.IO.File.ReadLines(@".\ChatTypeList.txt"))
            {
                _arrayStrWork = line.Split(',');
                //処理対象にするフラグが立っていた場合、処理対象リストに追加する
                if (_arrayStrWork[1] == "1")
                {
                    listChatType.Add(_arrayStrWork[0]);
                }
            }
            ret = 0;

            return ret;
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(332, 173);
            this.button1.TabIndex = 0;
            this.button1.Text = "Reload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ActPluginFirst
            // 
            this.Controls.Add(this.button1);
            this.Name = "ActPluginFirst";
            this.Size = new System.Drawing.Size(1012, 735);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            setChatType();
        }
    }


}
