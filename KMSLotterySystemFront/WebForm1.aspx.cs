using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMSLotterySystemFront.BLL;
using System.Collections;
using System.Data;

namespace KMSLotterySystemFront
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        RechargeLotteryNewPf pf = new RechargeLotteryNewPf();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            string result, systemstate, lotteryLevel, lotteryName, lid;
            //(string factoryid, string digitcode, string channel, int language, string codeState, string codeResult, string mobile, Hashtable userHsah, string ip, string queryID, string token, string jkactivityid, out string result, out string systemstate, out string lotteryLevel, out string lotteryName, out string lid)

            Hashtable userHsah = new Hashtable();
            userHsah["OPENID"] = txtOpenid.Text.Trim();

            pf.QueryLotteryRechargeKMS(factoryid: "9667",
            digitcode: txtDigit.Text.Trim(),
            channel: "M",
            language: 1,
            mobile: txtTel.Text.Trim(),
            userHsah: userHsah,
            ip: "10.20.26.19",
            result: out result,
            systemstate: out systemstate,
            lotteryLevel: out lotteryLevel,
            lotteryName: out lotteryName,
            lid: out lid);



            Label1.Text = result + " " + systemstate + " ";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            //string systemState = "";
            //Dictionary<string, string> dicResult = pf.SelectOpenidKMS("9667", txtOpenid.Text.Trim(), out systemState);
            //Label1.Text = dt.Rows[0]["IP"] + " " + systemState + " ";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            string systemState = "";
            Hashtable userHash = new Hashtable();
            userHash["USER_NAME"] = txtUname.Text.Trim();
            userHash["USER_ADDRESS"] = txtUaddress.Text.Trim();
            userHash["USER_TELEPHONE"] = txtUmobile.Text.Trim();
            bool bRet = pf.ModifyPostAdrKMS("9667", txtDigit.Text.Trim(), txtlid.Text.Trim(), txtTel.Text.Trim(), userHash, out systemState);
            Label1.Text = bRet + " " + systemState + " ";
        }
    }
}