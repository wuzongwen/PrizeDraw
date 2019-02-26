using DAL;
using Model;
using Senparc.CO2NET.Helpers;
using Senparc.NeuChar.Agents;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Linq;

namespace DataWall.WeCaht
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private string appId = Config.SenparcWeixinSetting.WeixinAppId;
        private string appSecret = Config.SenparcWeixinSetting.WeixinAppSecret;

        /// <summary>
        /// 模板消息集合（Key：checkCode，Value：OpenId）
        /// </summary>
        public static Dictionary<string, string> TemplateMessageCollection = new Dictionary<string, string>();

        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //比如MessageHandler<MessageContext>.GlobalGlobalMessageContext.ExpireMinutes = 3。
            GlobalMessageContext.ExpireMinutes = 3;

            //注册Token
            AccessTokenContainer.TryGetAccessToken(appId, appSecret);
            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }


        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }

        /// <summary>
        /// 处理文字请求
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var defaultResponseMessage = base.CreateResponseMessage<ResponseMessageText>();

            var requestHandler =
                requestMessage.StartHandler()
                //.Keyword("OPENID", () =>
                //{
                //    var openId = requestMessage.FromUserName;//获取OpenId
                //    var userInfo = UserApi.Info(appId, openId, Language.zh_CN);//获取用户信息
                //    defaultResponseMessage.Content = string.Format(
                //        "您的OpenID为：{0}\r\n昵称：{1}\r\n性别：{2}\r\n地区（国家/省/市）：{3}/{4}/{5}\r\n关注时间：{6}\r\n关注状态：{7}",
                //        requestMessage.FromUserName, userInfo.nickname, (WeixinSex)userInfo.sex, userInfo.country, userInfo.province, userInfo.city, DateTimeHelper.GetDateTimeFromXml(userInfo.subscribe_time), userInfo.subscribe);
                //    return defaultResponseMessage;
                //})
                .Keyword("CY", () =>
                {
                    var openId = requestMessage.FromUserName;//获取OpenId
                    var userInfo = UserApi.Info(appId, requestMessage.FromUserName, Senparc.Weixin.Language.zh_CN);//获取用户信息
                    using (PrizeDrawContext db = new PrizeDrawContext())
                    {
                        var PrizeDrawUser = db.SysPrizeDrawUsers.AsNoTracking().FirstOrDefault(s => s.openid == userInfo.openid);
                        if (PrizeDrawUser == null)
                        {
                            //添加参与活动用户
                            SysPrizeDrawUser sysPrizeDrawUser = new SysPrizeDrawUser
                            {
                                openid = userInfo.openid,
                                nickname = userInfo.nickname,
                                city = userInfo.city,
                                province = userInfo.province,
                                country = userInfo.country,
                                headimgurl = userInfo.headimgurl,
                                CrateTime = DateTime.Now,
                                EditTime = DateTime.Now
                            };
                            db.SysPrizeDrawUsers.Add(sysPrizeDrawUser);
                            db.SaveChanges();
                            defaultResponseMessage.Content = "未参与任何抽奖活动";
                        }
                        else
                        {
                            var SysPrizeDrawRecords = db.SysPrizeDrawRecords.AsNoTracking().Where(s => s.SysPrizeDrawUserId == PrizeDrawUser.ID).ToList();
                            var result = new StringBuilder();
                            int i = 0;
                            foreach (var item in SysPrizeDrawRecords)
                            {
                                i++;
                                result.AppendFormat(i.ToString() + "." + db.SysLibrarys.AsNoTracking().FirstOrDefault(s => s.ID == item.SysLibraryId).LibraryName + "\r\n");
                            }
                            defaultResponseMessage.Content = result.ToString();
                        }
                    }
                    return defaultResponseMessage;
                })
                .Keyword("ZJZT", () =>
                {
                    var openId = requestMessage.FromUserName;//获取OpenId
                    var userInfo = UserApi.Info(appId, requestMessage.FromUserName, Senparc.Weixin.Language.zh_CN);//获取用户信息
                    using (PrizeDrawContext db = new PrizeDrawContext())
                    {
                        var PrizeDrawUser = db.SysPrizeDrawUsers.AsNoTracking().FirstOrDefault(s => s.openid == userInfo.openid);
                        if (PrizeDrawUser == null)
                        {
                            //添加参与活动用户
                            SysPrizeDrawUser sysPrizeDrawUser = new SysPrizeDrawUser
                            {
                                openid = userInfo.openid,
                                nickname = userInfo.nickname,
                                city = userInfo.city,
                                province = userInfo.province,
                                country = userInfo.country,
                                headimgurl = userInfo.headimgurl,
                                CrateTime = DateTime.Now,
                                EditTime = DateTime.Now
                            };
                            db.SysPrizeDrawUsers.Add(sysPrizeDrawUser);
                            db.SaveChanges();
                            defaultResponseMessage.Content = "未参与任何抽奖活动";
                        }
                        else
                        {
                            var SysPrizeDrawRecords = db.SysPrizeDrawRecords.AsNoTracking().Where(s => s.SysPrizeDrawUserId == PrizeDrawUser.ID).ToList();
                            var result = new StringBuilder();
                            int i = 0;
                            foreach (var item in SysPrizeDrawRecords)
                            {
                                i++;
                                string State = string.Empty;
                                switch (item.State)
                                {
                                    case 0:
                                        State = "活动未开始";
                                        break;
                                    case 1:
                                        State = "恭喜您抽中" + db.SysContents.FirstOrDefault(s => s.ID == item.SysContentId).Title;
                                        break;
                                    case 2:
                                        State = "未中奖";
                                        break;
                                    default:
                                        State = "活动进行中";
                                        break;
                                }
                                result.AppendFormat(i.ToString() + "." + db.SysLibrarys.AsNoTracking().FirstOrDefault(s => s.ID == item.SysLibraryId).LibraryName + "【" + State + "】" + "\r\n");
                            }
                            defaultResponseMessage.Content = result.ToString();
                        }
                    }
                    return defaultResponseMessage;
                })
                .Default(() =>
                {
                    var result = new StringBuilder();
                    result.AppendFormat("回复【CY】查询参与的抽奖活动\r\n");
                    result.AppendFormat("回复【CX】查询中奖状态\r\n");
                    //if (CurrentMessageContext.RequestMessages.Count > 1)
                    //{
                    //    result.AppendFormat("您刚才还发送了如下消息（{0}/{1}）：\r\n", CurrentMessageContext.RequestMessages.Count,
                    //        CurrentMessageContext.StorageData);
                    //    for (int i = CurrentMessageContext.RequestMessages.Count - 2; i >= 0; i--)
                    //    {
                    //        var historyMessage = CurrentMessageContext.RequestMessages[i];
                    //        result.AppendFormat("{0} 【{1}】{2}\r\n",
                    //            historyMessage.CreateTime.ToString("HH:mm:ss"),
                    //            historyMessage.MsgType.ToString(),
                    //            (historyMessage is RequestMessageText)
                    //                ? (historyMessage as RequestMessageText).Content
                    //                : "[非文字类型]"
                    //            );
                    //    }
                    //    result.AppendLine("\r\n");
                    //}

                    //result.AppendFormat("如果您在{0}分钟内连续发送消息，记录将被自动保留（当前设置：最多记录{1}条）。过期后记录将会自动清除。\r\n",
                    //    GlobalMessageContext.ExpireMinutes, GlobalMessageContext.MaxRecordCount);
                    //result.AppendLine("\r\n");
                    //result.AppendLine(
                    //    "您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：http://sdk.weixin.senparc.com");

                    defaultResponseMessage.Content = result.ToString();
                    return defaultResponseMessage;
                });

            return requestHandler.GetResponseMessage() as IResponseMessageBase;
        }


        /// <summary>
        /// 处理事件请求（这个方法一般不用重写，这里仅作为示例出现。除非需要在判断具体Event类型以外对Event信息进行统一操作
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEventRequest(IRequestMessageEventBase requestMessage)
        {
            var eventResponseMessage = base.OnEventRequest(requestMessage);//对于Event下属分类的重写方法，见：CustomerMessageHandler_Events.cs
            //TODO: 对Event信息进行统一操作
            return eventResponseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
            * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
            * 只需要在这里统一发出委托请求，如：
            * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
            * return responseMessage;
            */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "欢迎关注上业科技抽奖平台";
            return responseMessage;
        }


        public override IResponseMessageBase OnUnknownTypeRequest(RequestMessageUnknownType requestMessage)
        {
            /*
             * 此方法用于应急处理SDK没有提供的消息类型，
             * 原始XML可以通过requestMessage.RequestDocument（或this.RequestDocument）获取到。
             * 如果不重写此方法，遇到未知的请求类型将会抛出异常（v14.8.3 之前的版本就是这么做的）
             */
            var msgType = Senparc.NeuChar.Helpers.MsgTypeHelper.GetRequestMsgTypeString(requestMessage.RequestDocument);
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "未知消息类型：" + msgType;

            WeixinTrace.SendCustomLog("未知请求消息类型", requestMessage.RequestDocument.ToString());//记录到日志中

            return responseMessage;
        }
    }
}